using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Momento.Protos.CachePing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Ping = Momento.Protos.CachePing.Ping;

class Handler : HttpMessageHandler
{
    private readonly MonoBehaviour unityEnvironment;
    public Handler(MonoBehaviour unityEnvironment)
    {
        this.unityEnvironment = unityEnvironment;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // request.Method
        switch (request.Method)
        {
            case HttpMethod m when m == HttpMethod.Post:
            var raw = await request.Content.ReadAsByteArrayAsync();
            Debug.Log("headers: " + request.Headers);
            Debug.Log("properties: " + request.Properties.Keys);
            Debug.Log("properties: " + request.Properties.Values);
            var uploadHandler = new UploadHandlerRaw(raw)
            {
                contentType = request.Content.Headers.GetValues("Content-Type").FirstOrDefault(),
            };

            var urequest = new UnityWebRequest(request.RequestUri)
            {
                method = "POST",
                uploadHandler = uploadHandler,
                timeout = 1,
            };

            foreach (var header in request.Headers)
            {
                Debug.Log("Header: " + header.Key + ": " + string.Join(',', header.Value));
                urequest.SetRequestHeader(header.Key, string.Join(',', header.Value));
            }

            Debug.Log("sending " + urequest.ToString() + " to " + urequest.uri + " using " + urequest.method + " for data " + Encoding.UTF8.GetString(raw));
            var completion = new TaskCompletionSource<HttpResponseMessage>();
            unityEnvironment.StartCoroutine(CompleteHttpRequestViaCoroutine(urequest, completion));

            var theResult = await completion.Task; // This seems to go out to lunch.
            Debug.Log("got a response: " + urequest.result.ToString());

            if (urequest.result != UnityWebRequest.Result.Success)
            {
                throw new IOException("grpc always returns a 200 with a status. Something else broke.");
            }
            return new HttpResponseMessage((HttpStatusCode)urequest.responseCode);
            default:
            throw new IOException("I don't know how to do anything other than post at grpc servers");
        }
    }

    private IEnumerator CompleteHttpRequestViaCoroutine(UnityWebRequest unityRequest, TaskCompletionSource<HttpResponseMessage> completion)
    {
        yield return unityRequest.SendWebRequest();
        
        var result = unityRequest.result;
        
        switch (result)
        {
            case UnityWebRequest.Result.Success:
                var response = new HttpResponseMessage();
                response.Content = new ByteArrayContent(unityRequest.downloadHandler.data);
                completion.SetResult(response);
                break;
            default:
                completion.SetException(new Exception($"Unexpected UnityWebRequest result: {result}"));
                break;
        }
    }
}

public class Foo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Hello World: {Bar.bar()}");
        Debug.Log("Constructing the new handler");
        
        Debug.Log("Constructed the things 2.");

        var response = Ping();

        Debug.Log($"Successful PING RESPONSE: {response}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    _PingResponse Ping()
    {
        var channel = GrpcChannel.ForAddress(
            "https://cache.cell-alpha-dev.preprod.a.momentohq.com",
            new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(new Handler(this))
            });
        var pingClient = new Ping.PingClient(channel);
        var pingRequest = new _PingRequest();

        return pingClient.Ping(pingRequest);
    }
}
