using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Momento.Http {
    public class UnityWebRequestHttpMessageHandler : HttpMessageHandler {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            Debug.Log(Thread.CurrentThread.ManagedThreadId + "http: Creating request");
            using (var uwr = await CreateRequest(request)) {
                Debug.Log(Thread.CurrentThread.ManagedThreadId + "http: Sending request");
                await SendRequest(uwr);
                Debug.Log(Thread.CurrentThread.ManagedThreadId + "http: Parsing response");
                var response = ParseResponse(uwr, request);
                Debug.Log(Thread.CurrentThread.ManagedThreadId + "http: returning response " + response.ToString());
                return response;
            }
        }

        private async Task<UnityWebRequest> CreateRequest(HttpRequestMessage request) {
            var uwr = new UnityWebRequest {
                uri = request.RequestUri,
                method = request.Method.ToString(),
                downloadHandler = new DownloadHandlerBuffer()
            };

            if (request.Content != null) {
                uwr.uploadHandler = new UploadHandlerRaw(await request.Content.ReadAsByteArrayAsync());
                foreach (var header in request.Content.Headers) {
                    uwr.SetRequestHeader(header.Key, string.Join(',', header.Value));
                }
            }

            foreach (var header in request.Headers) {
                uwr.SetRequestHeader(header.Key, string.Join(',', header.Value));
            }

            return uwr;
        }

        private Task<UnityWebRequest> SendRequest(UnityWebRequest uwr) {
            var op = uwr.SendWebRequest();
            var tcs = new TaskCompletionSource<UnityWebRequest>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (op.isDone) {
                tcs.SetResult(uwr);
            }
            else {
                op.completed += context => {
                    Debug.Log(Thread.CurrentThread.ManagedThreadId + "http: Web request completed; setting result");
                    tcs.SetResult(uwr);
                };
            }
            return tcs.Task;
        }

        private bool HasErrors(UnityWebRequest uwr) {
    #if UNITY_2020_1_OR_NEWER
            return uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError; 
    #else
            return response.isHttpError || response.isNetworkError;
    #endif
        }

        private HttpResponseMessage ParseResponse(UnityWebRequest uwr, HttpRequestMessage originalRequest) {
            var response = new HttpResponseMessage {
                RequestMessage = originalRequest,
                StatusCode = (System.Net.HttpStatusCode)uwr.responseCode,
                Content = new ByteArrayContent(uwr.downloadHandler?.data)
            };

            var responseHeaders = uwr.GetResponseHeaders();
            foreach (var header in responseHeaders) {
                if (IsWellKnownContentHeader(header.Key)) {
                    response.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                else {
                    response.Headers.Add(header.Key, header.Value);
                }
            }

            return response;
        }

        private static readonly string[] _wellKnownContentHeaders = {
            "Content-Disposition",
            "Content-Encoding",
            "Content-Language",
            "Content-Length",
            "Content-Location",
            "Content-MD5",
            "Content-Range",
            "Content-Type",
            "Expires",
            "Last-Modified"
        };

        private bool IsWellKnownContentHeader(string header) {
            foreach (string contentHeaderName in _wellKnownContentHeaders) {
                if (string.Equals(header, contentHeaderName, StringComparison.OrdinalIgnoreCase)) {
                    return true;
                }
            }

            return false;
        }
    }
}
