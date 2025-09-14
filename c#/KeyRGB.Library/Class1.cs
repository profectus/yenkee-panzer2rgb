using HidSharp;
using System.Drawing;

namespace KeyRGB.Library;

/// <summary>
/// SONiX USB Keyboard HID Interface for RGB lighting control
/// </summary>
public class SonixKeyboard : IDisposable
{
    private const int VendorId = 0x0c45;
    private const int ProductId = 0x8508;
    private const int TargetUsage = 146;
    private const int TargetUsagePage = 65308;
    
    private HidDevice? _device;
    private HidStream? _stream;
    private bool _disposed;

    /// <summary>
    /// Gets a value indicating whether the keyboard is connected
    /// </summary>
    public bool IsConnected => _stream?.CanWrite == true;

    /// <summary>
    /// Connect to the SONiX keyboard
    /// </summary>
    /// <returns>True if connection successful</returns>
    public bool Connect()
    {
        try
        {
            var devices = DeviceList.Local.GetHidDevices(VendorId, ProductId);
            
            foreach (var device in devices)
            {
                try
                {
                    // Try to open the device - HidSharp will filter by usage automatically
                    _device = device;
                    _stream = device.Open();
                    
                    Console.WriteLine($"Connected to device:");
                    Console.WriteLine($"Manufacturer: {device.GetManufacturer()}");
                    Console.WriteLine($"Product: {device.GetProductName()}");
                    
                    return true;
                }
                catch
                {
                    // Continue to next device if this one fails
                    continue;
                }
            }
            
            Console.WriteLine("Keyboard not found or unable to open!");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to keyboard: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Send packet to keyboard
    /// </summary>
    /// <param name="data">Packet data to send</param>
    /// <returns>True if packet sent successfully</returns>
    public bool SendPacket(byte[] data)
    {
        if (_stream == null || !_stream.CanWrite)
        {
            throw new InvalidOperationException("Keyboard not connected");
        }

        try
        {
            _stream.Write(data);
            
            // Read response for verification
            var buffer = new byte[64];
            var response = _stream.Read(buffer, 0, buffer.Length);
            
            return response > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending packet: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Disconnect from keyboard
    /// </summary>
    public void Disconnect()
    {
        _stream?.Close();
        _stream = null;
        _device = null;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Disconnect();
            _disposed = true;
        }
    }
}
