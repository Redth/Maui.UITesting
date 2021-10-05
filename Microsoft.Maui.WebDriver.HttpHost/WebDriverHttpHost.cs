using OpenQA.Selenium;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.Maui.WebDriver.HttpHost
{

    public partial class HttpServer
	{
		HttpListener? listener;

		readonly SessionManager sessionManager = new();

		public void Start()
		{
			listener = new HttpListener();
			listener.Prefixes.Add("http://*:8080/");
			listener.Start();
			while (true)
			{
				var c = listener.GetContext();

				_ = Task.Run(() =>
				{
					var ctx = c;
					try
					{
						var r = HandleRequest(ctx);

						WriteResponse(ctx, r.statusCode, r.value);
					}
					catch (WebDriverHttpHostException wex)
                    {
						WriteError(ctx, wex);
					}
					catch (Exception ex)
                    {
						WriteError(ctx, new WebDriverHttpHostException(WebDriverErrorCode.UnknownError, "Unknown Exception", ex));
                    }
				});
			}
		}

		(int statusCode, JToken value) HandleRequest(HttpListenerContext ctx)
        {
			var url = ctx.Request.Url ?? new Uri("http://localhost");
			var firstSegment = ctx.Request.Url?.Segments?.FirstOrDefault()?.ToLowerInvariant();
			var verb = ctx.Request.HttpMethod.ToLowerInvariant();
			var body = RequestBody.Parse(ctx);

			if (!string.IsNullOrEmpty(firstSegment))
			{
				if (firstSegment == "status")
				{
					//TODO: Status
					return Ok();
				}
				else if (firstSegment == "session")
				{
					// New session request
					if (url.Segments.Length <= 1)
					{
						var state = sessionManager.Create();
						return Ok(new JObject(new JProperty("sessionId", state.Id), new JProperty("capabilities", new JObject())));
					}
					else
					{
						var sessionId = url.Segments[1];

						if (!sessionManager.TryGet(sessionId, out var session))
						{
							throw new WebDriverHttpHostException(WebDriverErrorCode.InvalidSessionId, $"No session with the id {sessionId} was found.");
						}

						var sessionCmd = url.Segments?[2]?.ToLowerInvariant();

						if (!string.IsNullOrEmpty(sessionCmd))
						{
							var sessionSubCmd = url.Segments?[3]?.ToLowerInvariant();

							if (sessionCmd == "timeouts")
							{
								JObject getTimeouts()
								{
									var to = session?.Timeouts ?? new SessionTimouts();
									
									return new(
										new JProperty("script", (int)to.AsynchronousJavaScript.TotalMilliseconds),
										new JProperty("pageLoad", (int)to.PageLoad.TotalMilliseconds),
										new JProperty("implicit", (int)to.ImplicitWait.TotalMilliseconds));
								}

								// GET - Get Timeouts
								if (verb == "get")
								{
									return Ok(getTimeouts());
								}
								else if (verb == "post")
								{
									// POST - Set Timeouts
									var scriptTimeout = body?.Root?["script"]?.Value<long>();
									var pageLoadTimeout = body?.Root?["pageLoad"]?.Value<long>();
									var implicitTimeout = body?.Root?["implicit"]?.Value<long>();

									if (scriptTimeout.HasValue)
										session.Timeouts.AsynchronousJavaScript = TimeSpan.FromMilliseconds(scriptTimeout.Value);
									if (pageLoadTimeout.HasValue)
										session.Timeouts.PageLoad = TimeSpan.FromMilliseconds(pageLoadTimeout.Value);
									if (implicitTimeout.HasValue)
										session.Timeouts.ImplicitWait = TimeSpan.FromMilliseconds(implicitTimeout.Value);

									return Ok(getTimeouts());
								}
							}
							else if (sessionCmd == "url")
							{
								// POST - Go
								if (verb == "post")
								{
									var u = body?.Root?["url"]?.Value<string>();
									if (string.IsNullOrEmpty(u))
										throw new WebDriverHttpHostException(WebDriverErrorCode.InvalidArgument, "url is missing");

									throw new WebDriverHttpHostException(WebDriverErrorCode.UnsupportedOperation);
								}
								else if (verb == "get")
								{
									// GET - Get current
									return Ok(new JValue("app://"));
								}
							}
							else if (sessionCmd == "back")
							{
								// POST - Go back
								session.Driver?.Navigate()?.Back();
								return Ok();
							}
							else if (sessionCmd == "forward")
							{
								// POST - Go forward
								session.Driver?.Navigate()?.Forward();
								return Ok();
							}
							else if (sessionCmd == "refresh")
							{
								// POST - Refresh
								session.Driver?.Navigate()?.Refresh();
								return Ok();
							}
							else if (sessionCmd == "title")
							{
								// GET - Gets title
								var title = session.Driver?.Title;
								return Ok(new JValue(title));
							}
							else if (sessionCmd == "window")
							{
								if (string.IsNullOrEmpty(sessionSubCmd))
								{
									if (verb == "get")
									{
										// GET - Get window handle
										var h = session?.Driver?.CurrentWindowHandle;

										if (string.IsNullOrEmpty(h))
											throw new WebDriverHttpHostException(WebDriverErrorCode.NoSuchWindow);

										return Ok(new JValue(h));
									}
									else if (verb == "post")
									{
										// POST - Switch to window
										var windowHandle = body?.Root?["handle"]?.Value<string>();

										if (string.IsNullOrEmpty(windowHandle))
											throw new WebDriverHttpHostException(WebDriverErrorCode.InvalidArgument, "Invalid Window Handle");

										if (!(session?.Driver?.WindowHandles?.Any(h => h.Equals(windowHandle)) ?? false))
											throw new WebDriverHttpHostException(WebDriverErrorCode.NoSuchWindow);

										return Ok();
									}
									else if (verb == "delete")
									{
										// DELETE - Close window
										// POST - Switch to window
										var windowHandle = body?.Root?["handle"]?.Value<string>();

										if (string.IsNullOrEmpty(windowHandle))
											throw new WebDriverHttpHostException(WebDriverErrorCode.InvalidArgument, "Invalid Window Handle");

										if (!(session?.Driver?.WindowHandles?.Any(h => h.Equals(windowHandle)) ?? false))
											throw new WebDriverHttpHostException(WebDriverErrorCode.NoSuchWindow);

										return Ok();
									}
								}
								else if (sessionSubCmd == "handles" && verb == "get")
								{
									// /handles  GET - Get window handles
									var handles = session?.Driver?.WindowHandles?.ToArray() ?? Enumerable.Empty<string>();
									return Ok(new JArray(handles));
								}
								else if (sessionSubCmd == "rect" && verb == "get")
								{
									// /rect - GET - Get window rect, POST - Set window rect
									
								}
								else if (sessionSubCmd == "maximize" && verb == "post")
								{
									// /maximize - POST - maximize
									throw new WebDriverHttpHostException(WebDriverErrorCode.UnsupportedOperation);
								}
								else if (sessionSubCmd == "minimize" && verb == "post")
								{
									// /minimize - POST - minimize
									throw new WebDriverHttpHostException(WebDriverErrorCode.UnsupportedOperation);
								}
								else if (sessionSubCmd == "fullscreen" && verb == "post")
								{
									// /fullscreen - POST
									throw new WebDriverHttpHostException(WebDriverErrorCode.UnsupportedOperation);
								}
							}
							else if (sessionCmd == "frame")
							{
								// POST - switch to frame
								// /parent - Switch to parent frame
								throw new WebDriverHttpHostException(WebDriverErrorCode.UnsupportedOperation);
							}
							else if (sessionCmd == "element")
							{
								// POST - Find an element
								var usingType = body?.Root?["using"]?.Value<string>();
								var value = body?.Root?["value"]?.Value<string>();

								var by = new MobileBy(usingType, value);
								// TODO: We need to parse the location strategy into the
								// correct 'By' type... https://github.com/jlipps/simple-wd-spec#location-strategies
								
								var e = session?.Driver?.FindElement(by);

								// TODO: Find a way to surface an actual element id for the value here:
								// ALSO: yes this weird element-guid string is weird, but it is correct
								return Ok(new JProperty("element-6066-11e4-a52e-4f735466cecf", new JValue("ID")));
							}
							else if (sessionCmd == "element")
							{
								// POST - Find an element

								// /active - GET Gets active element

								// /{elementId} ->
								//	/screenshot - GET - Take a screenshot of an element
								//	/element - POST - Find element from element
								//	/elements - POST - Find elements from element
								//	/selected - GET - Is element selected
								//	/attribute/{name} - GET - Gets attribute
								//	/property/{name} - GET - Gets property
								//	/css/{propertyName} - GET - Gets CSS Property
								//	/text - GET - Gets element text
								//	/name - GET - Gets element Tag Name
								//	/rect - GET
								//	/enabled - GET
								//	/click - POST
								//	/clear - POST
								//	/value - POST - Send keys to element
							}
							else if (sessionCmd == "elements")
							{
								// POST - Find an element
							}
							else if (sessionCmd == "source")
							{
								// GET - Get page source
								return Ok(new JValue(string.Empty));
							}
							else if (sessionCmd == "execute")
							{
								// /sync POST - Execute script synchronously
								// /async POST - Execute script asynchronously
							}
							else if (sessionCmd == "cookie")
							{
								// GET - Get all cookies
								// POST - Add a cookie
								// DELETE - Delete All cookies

								// /{cookieName} - GET - Get named cookie
								// /{cookieName} - DELETE - Delete cookie by name
							}
							else if (sessionCmd == "actions")
							{
								// POST - Perform Actions
								// DELETE - Release Actions
							}
							else if (sessionCmd == "alert")
							{
								// /dismiss POST - Dismiss alert
								// /accept POST - Accept / ok
								// /text GET - Get alert text
								// /text POST - Send alert text
							}
							else if (sessionCmd == "screenshot")
							{
								// GET - Take a screenshot
							}
							else
							{
								throw new WebDriverHttpHostException(WebDriverErrorCode.UnknownCommand);
							}
						}
						else
                        {
							throw new WebDriverHttpHostException(WebDriverErrorCode.UnknownCommand);
						}
					}
				}
				else
                {
					throw new WebDriverHttpHostException(WebDriverErrorCode.UnknownCommand);
				}
			}
		
			throw new WebDriverHttpHostException(WebDriverErrorCode.UnknownError, "Unknown Error");
		}

		static void WriteResponse(HttpListenerContext ctx, int httpStatusCode, JToken value)
        {
			ctx.Response.StatusCode = httpStatusCode;
			using var sw = new StreamWriter(ctx.Response.OutputStream);

			if (value != null)
				sw.WriteLine(new JObject(new JProperty("value", value)).ToString());
		}

		static void WriteError(HttpListenerContext ctx, WebDriverHttpHostException ex)
		{
			var info = ex.ErrorCode.GetError();

			WriteResponse(ctx, info.httpStatusCode, new JObject(
				new JProperty("error", info.code),
				new JProperty("message", ex.Message),
				new JProperty("stacktrace", ex.StackTrace)));
		}

		static (int, JToken) Ok(JToken value)
			=> (200, value);

		static (int, JToken) Ok()
			=> (200, null);
	}


}
