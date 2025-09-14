using KeyRGB.Library;

namespace KeyRGB.Console;

/// <summary>
/// Simple console application demonstrating KeyRGB.Library usage
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine("🎨 SONiX RGB Keyboard Controller - .NET Edition");
        System.Console.WriteLine("================================================");
        System.Console.WriteLine();

        using var controller = new RGBKeyboardController();

        // Parse command line arguments
        var keyColor = args.Length > 0 ? args[0] : "white";
        var keyIntensity = args.Length > 1 && int.TryParse(args[1], out var intensity) ? intensity : 100;
        var specialColor = args.Length > 2 && int.TryParse(args[2], out var special) ? special : 5;

        System.Console.WriteLine($"Parameters:");
        System.Console.WriteLine($"  Key Color: {keyColor}");
        System.Console.WriteLine($"  Key Intensity: {keyIntensity}%");
        System.Console.WriteLine($"  Special Color: {specialColor}");
        System.Console.WriteLine();

        // Connect to keyboard
        System.Console.Write("Connecting to keyboard... ");
        if (!controller.Connect())
        {
            System.Console.WriteLine("❌ Failed to connect!");
            System.Console.WriteLine();
            System.Console.WriteLine("Make sure:");
            System.Console.WriteLine("- SONiX keyboard is connected (VID: 0x0c45, PID: 0x8508)");
            System.Console.WriteLine("- Application is running with administrator privileges");
            System.Console.WriteLine("- No other RGB software is using the keyboard");
            return;
        }
        System.Console.WriteLine("✅ Connected!");
        System.Console.WriteLine();

        // Apply RGB lighting
        System.Console.Write("Applying RGB lighting... ");
        if (controller.ApplyRgbLighting(keyColor, keyIntensity, specialColor))
        {
            System.Console.WriteLine("✅ Success!");
            System.Console.WriteLine();
            System.Console.WriteLine("RGB lighting has been applied to your keyboard!");
        }
        else
        {
            System.Console.WriteLine("❌ Failed to apply lighting!");
        }

        System.Console.WriteLine();
        System.Console.WriteLine("Examples:");
        System.Console.WriteLine("  KeyRGB.Console.exe \"#ff0000\" 100 8    # Red keys, red line & volume");
        System.Console.WriteLine("  KeyRGB.Console.exe blue 50 1          # Blue keys, orange line & volume");
        System.Console.WriteLine("  KeyRGB.Console.exe \"255,0,0\" 75 7     # Red keys, dark blue line & volume");
        System.Console.WriteLine();
        System.Console.WriteLine("Available special colors (0-7):");
        System.Console.WriteLine("  0: Off, 1: Orange, 2: Yellow, 3: Green");
        System.Console.WriteLine("  4: Light Blue, 5: Purple (default), 6: Red, 7: Dark Blue");
        System.Console.WriteLine();
        System.Console.WriteLine("Press any key to exit...");
        System.Console.ReadKey();
    }
}
