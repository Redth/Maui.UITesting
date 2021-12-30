using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Streamer
{
    public class ClientChannel : IDisposable
    {
        private int _id;
        private readonly Dictionary<long, Action<Response?>> _invocations = new ();
        private readonly Stream _stream;
        private readonly JsonSerializer _serializer = new ();

        public ClientChannel(Stream stream)
        {
            _stream = stream;
            _serializer.TypeNameHandling = TypeNameHandling.All;

            new Thread(() => ReadLoop()).Start();
        }

        public Task<TResponse?> InvokeAsync<TRequest, TResponse>(TRequest request) 
            where TRequest : Request
            where TResponse : Response
        {
            int id = Interlocked.Increment(ref _id);
            request.Id = id;

            var reqJson = JObject.FromObject(request, _serializer).ToString(Formatting.Indented);

            Console.WriteLine(reqJson);

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

            Write(request);

            return tcs.Task;
        }

        private void ReadLoop()
        {
            try
            {
                while (true)
                {
                    var reader = new JsonTextReader(new StreamReader(_stream));

                    var response = _serializer.Deserialize<Response>(reader);

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

        private void Write(object value)
        {
            // TODO: Pooling and async writes
            var jsonTextWriter = new JsonTextWriter(new StreamWriter(_stream) { AutoFlush = true });

            _serializer.Serialize(jsonTextWriter, value);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }

}
