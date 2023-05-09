using EdjCase.ICP.Agent.Agents.Http;
using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Networking;

public class UnityHttpClient : IHttpClient
{
    public async Task<HttpResponse> GetAsync(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(GetUri(url));
        await request.SendWebRequest();
        return this.ParseResponse(request);
    }

    public async Task<HttpResponse> PostAsync(string url, byte[] cborBody)
    {
        UnityWebRequest request = new();
        request.method = "POST";
        request.uri = GetUri(url);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.uploadHandler = new UploadHandlerRaw(cborBody);
        request.uploadHandler.contentType = "application/cbor";
        await request.SendWebRequest();
        return this.ParseResponse(request);
    }

    private static Uri GetUri(string path)
    {
        if (!path.StartsWith("/"))
        {
            path = "/" + path;
        }
        return new Uri("https://ic0.app" + path);
    }

    private HttpResponse ParseResponse(UnityWebRequest request)
    {
        if (request.result != UnityWebRequest.Result.Success)
        {
            throw new Exception("Failed UnityWebRequest: " + request.error);
        }
        HttpStatusCode statusCode = (HttpStatusCode)request.responseCode;
        return new HttpResponse(statusCode, () => Task.FromResult(request.downloadHandler.data));
    }
}

public class UnityWebRequestAwaiter : INotifyCompletion
{
    private UnityWebRequestAsyncOperation asyncOp;
    private Action continuation;

    public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
    {
        this.asyncOp = asyncOp;
        asyncOp.completed += OnRequestCompleted;
    }

    public bool IsCompleted { get { return asyncOp.isDone; } }

    public void GetResult() { }

    public void OnCompleted(Action continuation)
    {
        this.continuation = continuation;
    }

    private void OnRequestCompleted(AsyncOperation obj)
    {
        continuation();
    }
}

public static class ExtensionMethods
{
    public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
    {
        return new UnityWebRequestAwaiter(asyncOp);
    }
}
