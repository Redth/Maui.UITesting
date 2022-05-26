using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Streamer
{
    public class ServerChannel
    {
        private readonly Dictionary<string, Func<Request, Task<Response>>> _callbacks = new Dictionary<string, Func<Request, Task<Response>>>(StringComparer.OrdinalIgnoreCase);

        private bool _isBound;

        public ServerChannel()
        {
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
                    Response response = new();
                    response.Id = request.Id;

                    try
                    {
                        response = await m.HandleAsync(request);
                        response.Id = request.Id;
                    }
                    catch (TargetInvocationException ex)
                    {
                        response.Error = ex?.InnerException?.Message ?? ex?.Message;
                    }
                    catch (Exception ex)
                    {
                        response.Error = ex?.Message;
                    }

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
                    var request = Channel.ReadRequest(stream);

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

                        Channel.WriteResponse(stream, response);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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