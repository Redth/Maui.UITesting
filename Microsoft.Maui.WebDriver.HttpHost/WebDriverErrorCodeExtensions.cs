namespace Microsoft.Maui.WebDriver.HttpHost
{
	internal static class WebDriverErrorCodeExtensions
	{
		internal static (int httpStatusCode, string code) GetError(this WebDriverErrorCode code)
		=> code switch
		{
			WebDriverErrorCode.ElementClickIntercepted => (400, "element click intercepted"),
			WebDriverErrorCode.ElementNotSelectable => (400, "element not selectable"),
			WebDriverErrorCode.ElementNotInteractable => (400, "element not interactable"),
			WebDriverErrorCode.InsecureCertificate => (400, "insecure certificate"),
			WebDriverErrorCode.InvalidArgument => (400, "invalid argument"),
			WebDriverErrorCode.InvalidCookieDomain => (400, "invalid cookie domain"),
			WebDriverErrorCode.InvalidCoordinates => (400, "invalid coordinates"),
			WebDriverErrorCode.InvalidElementState => (400, "invalid element state"),
			WebDriverErrorCode.InvalidSelector => (400, "invalid selector"),
			WebDriverErrorCode.InvalidSessionId => (400, "invalid session id"),
			WebDriverErrorCode.JavascriptError => (500, "javascript error"),
			WebDriverErrorCode.MoveTargetOutOfBounds => (500, "move target out of bounds"),
			WebDriverErrorCode.NoSuchAlert => (400, "no such alert"),
			WebDriverErrorCode.NoSuchCookie => (404, "no such cookie"),
			WebDriverErrorCode.NoSuchElement => (404, "no such element"),
			WebDriverErrorCode.NoSuchFrame => (400, "no such frame"),
			WebDriverErrorCode.NoSuchWindow => (400, "no such window"),
			WebDriverErrorCode.ScriptTimeout => (408, "script timeout"),
			WebDriverErrorCode.SessionNotCreated => (500, "session not created"),
			WebDriverErrorCode.StaleElementReference => (400, "stale element reference"),
			WebDriverErrorCode.Timeout => (408, "timeout"),
			WebDriverErrorCode.UnableToSetCookie => (500, "unable to set cookie"),
			WebDriverErrorCode.UnableToCaptureScreen => (500, "unable to capture screen"),
			WebDriverErrorCode.UnexpectedAlertOpen => (500, "unexpected alert open"),
			WebDriverErrorCode.UnknownCommand => (404, "unknown command"),
			WebDriverErrorCode.UnknownError => (500, "unknown error"),
			WebDriverErrorCode.UnknownMethod => (405, "unknown method"),
			WebDriverErrorCode.UnsupportedOperation => (500, "unsupported operation"),
			_ => (500, "unknown error"),
		};
	}
}
