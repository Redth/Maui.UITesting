using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Streamer
{
    public class ServerChannel
    {
        private readonly Dictionary<string, Func<Request, Task<Response>>> _callbacks = new Dictionary<string, Func<Request, Task<Response>>>(StringComparer.OrdinalIgnoreCase);

        private bool _isBound;

        private readonly JsonSerializer _serializer;

        public ServerChannel()
        {
            _serializer = new JsonSerializer();
            _serializer.TypeNameHandling = TypeNameHandling.All;
        }

        public IDisposable Bind(params MethodHandler[] methods)
        {
            if (_isBound)
            {
                throw new NotSupportedException("Can't bind to different objects");
            }

            _isBound = true;

            foreach (var m in methods)
            {
                var methodName = m.Name;


                if (_callbacks.ContainsKey(methodName))
                {
                    throw new NotSupportedException(String.Format("Duplicate definitions of {0}. Overloading is not supported.", m.Name));
                }

                _callbacks[methodName] = async request =>
                {
                    var response = new Response();
                    response.Id = request.Id;

                    try
                    {
                        var jsonArgs = request.Args ?? Array.Empty<JToken>();

                        if ((request?.Args?.Length ?? 0) != m.ArgumentTypes.Length)
                            throw new ArgumentOutOfRangeException();

                        // Convert JToken's into the arg types they should be
                        var typedArgs = new List<object>();
                        for (int i = 0; i < jsonArgs.Length; i++)
                        {
                            var convertedArg = jsonArgs[i].ToObject<object>(_serializer);
                            if (convertedArg != null)
                                typedArgs.Add(convertedArg);
                        }

                        var result = await m.Handle(typedArgs.ToArray());

                        if (result != null)
                        {
                            response.Result = JToken.FromObject(result, _serializer);
                        }
                    }
                    catch (TargetInvocationException ex)
                    {
                        response.Error = ex?.InnerException?.Message ?? ex?.Message;
                    }
                    catch (Exception ex)
                    {
                        response.Error = ex?.Message;
                    }

                    Console.WriteLine(response.Result);
                    return response;
                };
            }

            return new DisposableAction(() =>
            {
                foreach (var m in methods)
                {
                    lock (_callbacks)
                    {
                        _callbacks.Remove(m.Name);
                    }
                }
            });
        }

        public async Task StartAsync(Stream stream)
        {
            try
            {
                while (true)
                {
                    // REVIEW: This does a blocking read
                    var reader = new JsonTextReader(new StreamReader(stream));
                    _serializer.TypeNameHandling = TypeNameHandling.All;

                    var request = _serializer.Deserialize<Request>(reader);

                    if (request != null)
                    {
                        Response? response = null;

                        if (request.Method != null && _callbacks.TryGetValue(request.Method, out var callback))
                        {
                            response = await callback(request);
                        }
                        else
                        {
                            // If there's no method then return a failed response for this request
                            response = new Response
                            {
                                Id = request.Id,
                                Error = string.Format("Unknown method '{0}'", request.Method)
                            };
                        }

                        await WriteAsync(stream, response);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private Task WriteAsync(Stream stream, object value)
        {
            var data = JsonConvert.SerializeObject(
                value,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            var bytes = Encoding.UTF8.GetBytes(data);

            return stream.WriteAsync(bytes, 0, bytes.Length);
        }

        private class DisposableAction : IDisposable
        {
            private Action _action;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                Interlocked.Exchange(ref _action, () => { }).Invoke();
            }
        }
    }
}