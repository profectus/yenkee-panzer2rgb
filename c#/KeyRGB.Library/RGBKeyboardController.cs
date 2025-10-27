using System.Drawing;
using System.Globalization;

namespace KeyRGB.Library;

/// <summary>
/// RGB Keyboard Controller for SONiX USB keyboards
/// Complete RGB lighting control for SONiX USB keyboards (VID: 0x0c45, PID: 0x8508)
/// NOTE: White colors are NOT supported by hardware - only available colors are used.
/// </summary>
public class RGBKeyboardController : IDisposable
{
    private readonly SonixKeyboard _keyboard;
    private bool _disposed;

    // Base packet template for white keys
    private readonly string _basePacket = "043b04062200000000060301fa00ffffff0000000000000000000004060008000000000000000000000000000000000000000000000000000000000000000000";

    // FINAL color mapping - 8 LOGICAL combinations (0-7)
    // Each number sets unified colors for both line and volume
    private readonly Dictionary<int, SpecialColorMapping> _specialColors = new()
    {
        { 0, new SpecialColorMapping(8, 9) },    // off
        { 1, new SpecialColorMapping(1, 3) },    // orange theme
        { 2, new SpecialColorMapping(2, 4) },    // yellow theme
        { 3, new SpecialColorMapping(3, 5) },    // green theme
        { 4, new SpecialColorMapping(4, 6) },    // light blue theme
        { 5, new SpecialColorMapping(6, 8) },    // purple theme (default)
        { 6, new SpecialColorMapping(10, 2) },   // red theme (line=10 discovered!)
        { 7, new SpecialColorMapping(5, 7) },    // dark blue theme
    };

    private readonly Dictionary<string, Color> _colorNames = new()
    {
        { "red", Color.FromArgb(255, 0, 0) },
        { "green", Color.FromArgb(0, 255, 0) },
        { "blue", Color.FromArgb(0, 0, 255) },
        { "white", Color.FromArgb(255, 255, 255) },
        { "black", Color.FromArgb(0, 0, 0) },
        { "yellow", Color.FromArgb(255, 255, 0) },
        { "cyan", Color.FromArgb(0, 255, 255) },
        { "magenta", Color.FromArgb(255, 0, 255) },
        { "orange", Color.FromArgb(255, 165, 0) },
        { "purple", Color.FromArgb(128, 0, 128) },
        { "pink", Color.FromArgb(255, 192, 203) },
        { "off", Color.FromArgb(0, 0, 0) }
    };

    public RGBKeyboardController()
    {
        _keyboard = new SonixKeyboard();
    }

    /// <summary>
    /// Gets a value indicating whether the keyboard is connected
    /// </summary>
    public bool IsConnected => _keyboard.IsConnected;

    /// <summary>
    /// Connect to the keyboard
    /// </summary>
    /// <returns>True if connection successful</returns>
    public bool Connect()
    {
        return _keyboard.Connect();
    }

    /// <summary>
    /// Disconnect from the keyboard
    /// </summary>
    public void Disconnect()
    {
        _keyboard.Disconnect();
    }

    /// <summary>
    /// Parse RGB color from various formats
    /// </summary>
    /// <param name="colorInput">Color in hex (#ff0000), RGB (255,0,0), or name format</param>
    /// <returns>Parsed Color or null if invalid</returns>
    public Color? ParseColor(string? colorInput)
    {
        if (string.IsNullOrWhiteSpace(colorInput))
            return null;

        var colorStr = colorInput.Trim().ToLowerInvariant();

        // Hex format (#ff0000)
        if (colorStr.StartsWith('#') && colorStr.Length == 7)
        {
            if (int.TryParse(colorStr[1..3], NumberStyles.HexNumber, null, out int r) &&
                int.TryParse(colorStr[3..5], NumberStyles.HexNumber, null, out int g) &&
                int.TryParse(colorStr[5..7], NumberStyles.HexNumber, null, out int b))
            {
                return Color.FromArgb(r, g, b);
            }
        }

        // RGB format (255,0,0)
        if (colorStr.Contains(','))
        {
            var parts = colorStr.Split(',');
            if (parts.Length == 3 &&
                int.TryParse(parts[0].Trim(), out int r) &&
                int.TryParse(parts[1].Trim(), out int g) &&
                int.TryParse(parts[2].Trim(), out int b) &&
                r >= 0 && r <= 255 && g >= 0 && g <= 255 && b >= 0 && b <= 255)
            {
                return Color.FromArgb(r, g, b);
            }
        }

        // Color names
        return _colorNames.TryGetValue(colorStr, out var color) ? color : null;
    }

    /// <summary>
    /// Parse special color index (0-7)
    /// </summary>
    /// <param name="colorInput">Special color index</param>
    /// <returns>Parsed index or 5 (purple) as default</returns>
    public int ParseSpecialColor(string? colorInput)
    {
        if (string.IsNullOrWhiteSpace(colorInput))
            return 5;

        if (int.TryParse(colorInput.Trim(), out int value) && value >= 0 && value <= 7)
            return value;

        return 5; // default purple
    }

    /// <summary>
    /// Apply complete RGB lighting - ALL IN ONE PACKET
    /// </summary>
    /// <param name="keyColor">Key color (default: white)</param>
    /// <param name="keyIntensity">Key intensity 0-100 (default: 100)</param>
    /// <param name="specialColor">Special color index 0-7 (default: 5)</param>
    /// <returns>True if lighting applied successfully</returns>
    public bool ApplyRgbLighting(string keyColor = "white", int keyIntensity = 100, int specialColor = 5)
    {
        var color = ParseColor(keyColor) ?? Color.White;
        var intensity = Math.Clamp(keyIntensity, 0, 100);
        var special = ParseSpecialColor(specialColor.ToString());

        // Apply intensity to color
        var adjustedColor = Color.FromArgb(
            (int)(color.R * intensity / 100.0),
            (int)(color.G * intensity / 100.0),
            (int)(color.B * intensity / 100.0)
        );

        // Convert base packet to bytes
        var packet = Convert.FromHexString(_basePacket);

        // Set RGB values for key lighting (positions 14-16)
        packet[14] = (byte)adjustedColor.R;
        packet[15] = (byte)adjustedColor.G;
        packet[16] = (byte)adjustedColor.B;

        // Set special colors
        if (_specialColors.TryGetValue(special, out var specialMapping))
        {
            packet[28] = (byte)specialMapping.LineColor;     // Line color
            packet[30] = (byte)specialMapping.VolumeColor;   // Volume button color
        }

        return _keyboard.SendPacket(packet);
    }

    /// <summary>
    /// Set a static color for the entire keyboard
    /// </summary>
    /// <param name="color">Color to set</param>
    /// <param name="intensity">Intensity from 0-100 (default: 100)</param>
    /// <returns>True if color was set successfully</returns>
    public bool SetStaticColor(Color color, int intensity = 100)
    {
        // Determine special color based on the color
        int specialColor = GetSpecialColorFromColor(color);
        
        return ApplyRgbLighting($"{color.R},{color.G},{color.B}", intensity, specialColor);
    }

    /// <summary>
    /// Get the best matching special color index for a given color
    /// </summary>
    /// <param name="color">Color to match</param>
    /// <returns>Special color index (0-7)</returns>
    private int GetSpecialColorFromColor(Color color)
    {
        // If it's green, use green theme
        if (color.G > color.R && color.G > color.B && color.G > 200)
            return 3; // green theme
        
        // If it's purple/magenta, use purple theme
        if (color.R > color.G && color.B > color.G && Math.Abs(color.R - color.B) < 50)
            return 5; // purple theme
        
        // If it's red, use red theme
        if (color.R > color.G && color.R > color.B && color.R > 200)
            return 6; // red theme
        
        // If it's blue, use blue theme
        if (color.B > color.R && color.B > color.G && color.B > 200)
            return 7; // dark blue theme
        
        // If it's yellow, use yellow theme
        if (color.R > 200 && color.G > 200 && color.B < 100)
            return 2; // yellow theme
        
        // If it's orange, use orange theme
        if (color.R > 200 && color.G > 100 && color.G < 200 && color.B < 100)
            return 1; // orange theme
        
        // Default to purple theme
        return 5;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _keyboard.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// Special color mapping for line and volume controls
    /// </summary>
    public record SpecialColorMapping(int LineColor, int VolumeColor);
}
