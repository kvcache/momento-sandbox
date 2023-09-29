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

public class Foo : MonoBehaviour
{
    private Task<_PingResponse> later;

    // Start is called before the first frame update
    void Start()
    {
        later = Ping();
    }

    // Update is called once per frame
    void Update()
    {
        if (later != null && later.Status != TaskStatus.Running)
        {
            Debug.Log(Thread.CurrentThread.ManagedThreadId + "task status: " + later.Status.ToString());
            if (later.Status == TaskStatus.RanToCompletion)
            {
                if (later.IsCompleted) {
                    if (later.Exception == null) {
                        var response = later.Result;
                        Debug.Log(Thread.CurrentThread.ManagedThreadId + $"Successful PING RESPONSE: {response}");
                    } else {
                        var response = later.Exception;
                        Debug.Log(Thread.CurrentThread.ManagedThreadId + $"Error PING RESPONSE: {response}");
                    }
                }
                later = null;
            }
        }
    }

    async Task<_PingResponse> Ping()
    {
        Debug.Log(Thread.CurrentThread.ManagedThreadId + "creating channel");
        var channel = GrpcChannel.ForAddress(
            "https://cache.cell-alpha-dev.preprod.a.momentohq.com",
            new GrpcChannelOptions
            {
                // HttpHandler = new GrpcWebHandler(new Handler(this))
                HttpHandler = new GrpcWebHandler(new Momento.Http.UnityWebRequestHttpMessageHandler()) {
                    GrpcWebMode = GrpcWebMode.GrpcWebText,
                },
            });
        var pingClient = new Ping.PingClient(channel);
        var pingRequest = new _PingRequest();

        Debug.Log(Thread.CurrentThread.ManagedThreadId + "sending PingAsync");
        return await pingClient.PingAsync(pingRequest).ConfigureAwait(false);
    }
}
