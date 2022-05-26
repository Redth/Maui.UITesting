using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Streamer
{
    public class ClientChannel : IDisposable
    {
        private int _id;
        private readonly Dictionary<long, Action<Response?>> _invocations = new ();
        private readonly Stream _stream;
        
        public ClientChannel(Stream stream)
        {
            _stream = stream;

            new Thread(() => ReadLoop()).Start();
        }

        public Task<TResponse?> InvokeAsync<TRequest, TResponse>(TRequest request) 
            where TRequest : Request
            where TResponse : Response
        {
            int id = Interlocked.Increment(ref _id);
            request.Id = id;

            var tcs = new TaskCompletionSource<TResponse?>();

            lock (_invocations)
            {
                _invocations[id] = response =>
                {
                    try
                    {
                        // If there's no response then cancel the call
                        if (response == null)
                        {
                            tcs.TrySetCanceled();
                        }
                        else if (response.Error != null)
                        {
                            tcs.TrySetException(new InvalidOperationException(response.Error));
                        }
                        else
                        {
                            tcs.TrySetResult(response as TResponse);
                        }
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                };
            }

            Channel.WriteRequest(_stream, request);

            return tcs.Task;
        }

        private void ReadLoop()
        {
            try
            {
                while (true)
				{
                    var response = Channel.ReadResponse(_stream);

					if (response != null)
					{
						lock (_invocations)
						{
							if (_invocations.TryGetValue(response.Id, out var invocation))
							{
								invocation?.Invoke(response);

								_invocations.Remove(response.Id);
							}
						}
					}
				}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Any pending callbacks need to be cleaned up
                lock (_invocations)
                {
                    foreach (var invocation in _invocations)
                    {
                        invocation.Value(null);
                    }
                }
            }
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }

}
