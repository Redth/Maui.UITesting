using Grpc.Core;

namespace Microsoft.Maui.Automation.Driver;

public static class GrpcExtensions
{
    public static async Task RequestStream<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, TRequest request, Action<TResponse>? callback = null)
    {
        await call.RequestStream.WriteAsync(request);

        var tcsComplete = new TaskCompletionSource<bool>();

        var statusTask = Task.Run(async () =>
        {
            try
            {
                while (await call.ResponseStream.MoveNext(CancellationToken.None))
                {
                    var c = call.ResponseStream.Current;

                    callback?.Invoke(c);
                }

                tcsComplete.TrySetResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                tcsComplete.TrySetException(ex);
            }
        });

        await call.RequestStream.CompleteAsync();
        await tcsComplete.Task;
    }

    public static Task<TResponse> SendStream<TRequest, TResponse>(this AsyncClientStreamingCall<TRequest, TResponse> call, params TRequest[] requests)
        => call.SendStream<TRequest, TResponse>(TimeSpan.Zero, requests);

    public static async Task<TResponse> SendStream<TRequest, TResponse>(this AsyncClientStreamingCall<TRequest, TResponse> call, TimeSpan delayBetween, params TRequest[] requests)
    {
        var first = true;

        foreach (var r in requests)
        {
            if (!first && delayBetween != TimeSpan.Zero)
            {
                await Task.Delay(delayBetween);
                first = false;
            }

            await call.RequestStream.WriteAsync(r);
        }

        await call.RequestStream.CompleteAsync();

        return await call.ResponseAsync;
    }


    public static async Task ReceiveStream<TResponse>(this AsyncServerStreamingCall<TResponse> call, Action<TResponse> callback)
    {
        while (await call.ResponseStream.MoveNext(CancellationToken.None))
        {
            callback?.Invoke(call.ResponseStream.Current);
        }
    }
}
