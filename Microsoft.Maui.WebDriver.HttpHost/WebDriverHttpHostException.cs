using System;

namespace Microsoft.Maui.WebDriver.HttpHost
{
    public class WebDriverHttpHostException : Exception
    {
        public WebDriverHttpHostException(WebDriverErrorCode errorCode)
            : base()
        {
            ErrorCode = errorCode;
        }

        public WebDriverHttpHostException(WebDriverErrorCode errorCode, string message, Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }

        public WebDriverHttpHostException(WebDriverErrorCode errorCode, string message)
            : base(message)
        {
			ErrorCode = errorCode;
        }

		public WebDriverErrorCode ErrorCode { get; private set; }
    }
}
