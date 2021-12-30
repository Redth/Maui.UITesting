namespace Streamer
{
    public abstract class MethodHandler
    {
        public abstract string Name { get; }

        public abstract Task<Response> HandleAsync(Request request);
    }


    public class MethodHandler<TResponse, TRequest> : MethodHandler
        where TResponse : Response
        where TRequest : Request
    {
        public MethodHandler(string name, Func<TRequest, Task<TResponse>> handler)
        {
            Name = name;
            Handler = handler;
        }

        public override string Name { get; }

        protected readonly Func<TRequest, Task<TResponse>> Handler;

        public override async Task<Response> HandleAsync(Request request)
        {
            if (request is TRequest typedRequest)
            {
                return await Handler(typedRequest);
            }

            throw new ArgumentException($"Invalid request type, expected: {typeof(TRequest).FullName}.", nameof(request));
        }
    }
}