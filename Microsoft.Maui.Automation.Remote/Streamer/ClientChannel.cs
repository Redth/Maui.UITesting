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

            new Thread(() => ReadLoop()).Start();
        }

        //public T As<T>()
        //{
        //    return TypedChannelBuilder<T>.Build(this);
        //}

        public Task InvokeAsync(string name, params object[] args)
        {
            return InvokeAsync<object>(name, args);
        }

        public Task<T?> InvokeAsync<T>(string name, params object[] args)
        {
            int id = Interlocked.Increment(ref _id);

            var request = new Request
            {
                Id = id,
                Method = name,
                Args = args.Select(a => JToken.FromObject(a)).ToArray()
            };

            var tcs = new TaskCompletionSource<T?>();

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
                        else if (response.Result != null)
                        {
                            Console.WriteLine(response.Result.ToString());
                            Console.WriteLine("TYPE: " + typeof(T).FullName);
                            tcs.TrySetResult(response.Result.ToObject<T>());
                        }
                        else
                        {
                            tcs.TrySetResult(default);
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
