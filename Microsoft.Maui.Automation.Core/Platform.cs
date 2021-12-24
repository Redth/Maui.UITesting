namespace Microsoft.Maui.Automation
{
    public enum Platform
    {
		MAUI = 0,
		iOS = 1,
		MacCatalyst = 2,
		MacOS = 3,
		tvOS = 4,
		Android = 10,
		WinAppSdk = 20
    }

    public enum ActionResultStatus
    {
        Unknown,
        Ok,
        Error
    }
}
