using System;

namespace Xamarin.UITest;

/// <summary>
/// Runtime information and control of device.
/// </summary>
public interface IDevice
{
    /// <summary>
    /// The uri of the device.
    /// </summary>
    Uri DeviceUri { get; }


    /// <summary>
    /// The identifier of the device.
    /// </summary>
    string DeviceIdentifier { get; }

    /// <summary>
    /// Change GPS location of the device. 
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    void SetLocation(double latitude, double longitude);
}