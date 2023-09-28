using System.Collections;
// using System.Collections.Generic;
using System.Net.Http;
// using Grpc.Net.Client;
// using Grpc.Net.Client.Web;
// using Momento.Protos.CachePing;
// using UnityEditor.VersionControl;
using UnityEngine;
// using Ping = Momento.Protos.CachePing.Ping;
// using System.Threading.Tasks;

public class Foo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting ");
        StartCoroutine(MakeRequests());
    }

    private IEnumerator MakeRequests()
    {
        Debug.Log("In MakeRequests");
        // System.Threading.Tasks.Task.Run(() => IssueRequestsAsync());
        // Debug.Log("Yielding from MakeRequests");
        //
        // // var httpClientHandler = new HttpClientHandler();
        // var httpClientHandler = new FancyHttpMessageHandler(this);
        // var httpClient = new HttpClient(httpClientHandler);
        // Debug.Log("Making http client async call");
        // var googleResponseTask = httpClient.GetStringAsync("https://google.com");
        // Debug.Log("About to call the blocking .Result on the response");
        // var googleResponse = googleResponseTask.Result;
        // Debug.Log($"Got a response from google: {googleResponse}");
        
        IssueRequestsAsync();
        
        yield return null;
    }

    private async void IssueRequestsAsync()
    {
        Debug.Log("In IssueRequestAsync");
        Debug.Log($"Hello World: {Bar.bar()}");
        Debug.Log("Constructing the thing");
        
        
        
        // var httpClientHandler = new HttpClientHandler();
        var httpClientHandler = new FancyHttpMessageHandler(this);
        var httpClient = new HttpClient(httpClientHandler);
        Debug.Log("Making http client async call");
        var googleResponseTask = httpClient.GetStringAsync("https://google.com");
        // Debug.Log("About to call the blocking .Result on the response");
        // var googleResponse = googleResponseTask.Result;
        Debug.Log("About to await the result");
        var googleResponse = await googleResponseTask;
        Debug.Log($"Got a response from google: {googleResponse}");
        
        
        
        
        // var channel =
        //     GrpcChannel.ForAddress("https://cache.cell-alpha-dev.preprod.a.momentohq.com", new GrpcChannelOptions
        //     {
                // HttpHandler = new GrpcWebHandler(
                // new HttpClientHandler()
                // )
            // });
        // var pingClient = new Ping.PingClient(channel);
        // var pingRequest = new _PingRequest();
        // Debug.Log("Constructed the things 3.");
        // var response = pingClient.Ping(pingRequest);
        // Debug.Log($"PING RESPONSE: {response}");
        //
        // var httpClientHandler = new HttpClientHandler();
        // var httpClient = new HttpClient(httpClientHandler);
        // var googleResponse = await httpClient.GetStringAsync("https://google.com");
        // Debug.Log($"Got a response from google: {googleResponse}");
    }

    // Update is called once per frame
    void Update()
    {
    }
}