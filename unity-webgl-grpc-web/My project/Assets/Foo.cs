using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Momento.Protos.CachePing;
using UnityEngine;
using Ping = Momento.Protos.CachePing.Ping;

public class Foo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Hello World: {Bar.bar()}");
        Debug.Log("Constructing the thing");
        var channel =
            GrpcChannel.ForAddress("https://cache.cell-alpha-dev.preprod.a.momentohq.com", new GrpcChannelOptions
            {
                 HttpHandler = new GrpcWebHandler(new HttpClientHandler()) 
            });
        var pingClient = new Ping.PingClient(channel);
        var pingRequest = new _PingRequest();
        Debug.Log("Constructed the things 2.");        
        var response = pingClient.Ping(pingRequest);
        Debug.Log($"PING RESPONSE: {response}");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
