using System;
using System.Collections.Generic;

namespace Microsoft.Maui.WebDriver.HttpHost
{
    internal class SessionManager
    {
		Dictionary<string, SessionState> sessions = new();

		public bool TryGet(string id, out SessionState sessionState)
			=> sessions.TryGetValue(id, out sessionState);

		public SessionState Create()
        {
			var newSessionId = Guid.NewGuid().ToString();
			var sessionState = new SessionState { Id = newSessionId };
			sessions.Add(newSessionId, sessionState);
			return sessionState;
		}
	}


}
