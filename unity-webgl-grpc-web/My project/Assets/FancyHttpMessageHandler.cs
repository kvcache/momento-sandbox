using System;
using System.Collections;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FancyHttpMessageHandler : HttpMessageHandler
{
    private readonly MonoBehaviour unityEnvironment;
    public FancyHttpMessageHandler(MonoBehaviour unityEnvironment)
    {
        this.unityEnvironment = unityEnvironment;
    }
    
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // var unityRequest = new UnityWebRequest();
        // unityRequest.url = "https://google.com";
        // unityRequest.method = "GET";
        // // var unityResponse = unityRequest.SendWebRequest();
        // yield return unityRequest.SendWebRequest();
        // var result = unityRequest.result;
        //
        // switch (result)
        // {
        //     case UnityWebRequest.Result.Success:
        //         var response = new HttpResponseMessage();
        //         response.Content = new StringContent("The unity web request was successful");
        //         return Task.FromResult(response);
        //     default:
        //         throw new Exception($"Unexpected UnityWebRequest result: {result}");
        // }
        //

        // var response = new HttpResponseMessage();
        // response.Content = new StringContent("Hallo there. Using a task completion now!");

        var completion = new TaskCompletionSource<HttpResponseMessage>();
        unityEnvironment.StartCoroutine(CompleteHttpRequestViaCoroutine(completion));
        // completion.SetResult(response);
        return completion.Task;

        // return Task.FromResult(response);
        // return new Task<HttpResponseMessage>(() => response);
        // throw new NotImplementedException();
    }

    private IEnumerator CompleteHttpRequestViaCoroutine(TaskCompletionSource<HttpResponseMessage> completion)
    {
        // var response = new HttpResponseMessage();
        // response.Content = new StringContent("Hallo there. Using a task completion from a coroutine now!");
        // completion.SetResult(response);
        //
        // yield return null;
        
        // var unityRequest = new UnityWebRequest();
        // unityRequest.url = "https://google.com";
        // unityRequest.method = "GET";
        // var unityRequest = UnityWebRequest.Get("https://google.com");
        // var unityRequest = UnityWebRequest.Get("https://7dyvt4pwyk.execute-api.us-west-2.amazonaws.com/prod/signup");
        var unityRequest = UnityWebRequest.Post("https://7dyvt4pwyk.execute-api.us-west-2.amazonaws.com/prod/signup",
            "{\"email\": \"taco.com\"}", "application/json");
        
        // completion.SetResult(response);

        Debug.Log("ABOUT TO YIELD / SEND UNITY WEB REQUEST");
        yield return unityRequest.SendWebRequest();
        Debug.Log("I AM NOW AT THE LOC AFTER THE YIELD");
        
        // completion.SetResult(response);
        
        var result = unityRequest.result;
        
        switch (result)
        {
            case UnityWebRequest.Result.Success:
                Debug.Log($"UNITY WEB REQUEST SUCCESS!");
                Debug.Log($"UNITY WEB REQUEST TEXT: {unityRequest.downloadHandler.text}");
                var response = new HttpResponseMessage();
                response.Content = new StringContent(unityRequest.downloadHandler.text);
                completion.SetResult(response);
                break;
            default:
                Debug.Log($"UNITY WEB REQUEST FAILURE: {result}");
                throw new Exception($"Unexpected UnityWebRequest result: {result}");
        }
    }
}