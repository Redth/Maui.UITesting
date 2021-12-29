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

        public Task InvokeAsync(string name, params object[] args)
        {
            return InvokeAsync<object>(name, args);
        }

        public Task<TResult?> InvokeAsync<TArg1, TResult>(string name, TArg1? arg1)
            => InvokeAsync<TResult>(name, new[] { arg1 });

        public Task<TResult?> InvokeAsync<TArg1, TArg2, TResult>(string name, TArg1? arg1, TArg2? arg2)
            => InvokeAsync<TResult>(name, new object?[] { arg1, arg2 });

        public Task<TResult?> InvokeAsync<TArg1, TArg2, TArg3, TResult>(string name, TArg1? arg1, TArg2? arg2, TArg3? arg3)
            => InvokeAsync<TResult>(name, new object?[] { arg1, arg2, arg3 });

        public Task<TResult?> InvokeAsync<TArg1, TArg2, TArg3, TArg4, TResult>(string name, TArg1? arg1, TArg2? arg2, TArg3? arg3, TArg4? arg4)
            => InvokeAsync<TResult>(name, new object?[] { arg1, arg2, arg3, arg4 });

        public Task<T?> InvokeAsync<T>(string name, params object?[] args)
        {
            int id = Interlocked.Increment(ref _id);

            var request = new Request
            {
                Id = id,
                Method = name,
                Args = args?.Select(a => a is null ? null : JToken.FromObject(a, _serializer))?.ToArray() ?? Array.Empty<JToken?>()
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
                            tcs.TrySetResult(response.Result.ToObject<T>(_serializer));
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
