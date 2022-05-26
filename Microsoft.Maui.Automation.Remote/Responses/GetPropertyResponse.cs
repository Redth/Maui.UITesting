
namespace Streamer
{
	public class GetPropertyResponse : Response
    {
        public GetPropertyResponse(object? result)
            => Result = result;

        public object? Result { get; set; }
    }
}