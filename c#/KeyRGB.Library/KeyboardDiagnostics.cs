using HidSharp;
using System.Text;

namespace KeyRGB.Library;

/// <summary>
/// Diagnostické nástroje pro ladění připojení klávesnice
/// </summary>
public static class KeyboardDiagnostics
{
    private const int VendorId = 0x0c45;
    private const int ProductId = 0x8508;

    /// <summary>
    /// Zobrazí všechna dostupná HID zařízení
    /// </summary>
    public static void ListAllHidDevices()
    {
        Console.WriteLine("=== Všechna HID zařízení ===");
        var devices = DeviceList.Local.GetHidDevices();
        
        foreach (var device in devices)
        {
            try
            {
                Console.WriteLine($"VID: 0x{device.VendorID:X4}, PID: 0x{device.ProductID:X4}");
                Console.WriteLine($"  Manufacturer: {device.GetManufacturer() ?? "N/A"}");
                Console.WriteLine($"  Product: {device.GetProductName() ?? "N/A"}");
                Console.WriteLine($"  Serial: {device.GetSerialNumber() ?? "N/A"}");
                Console.WriteLine($"  Path: {device.DevicePath}");
                Console.WriteLine($"  Max Input Length: {device.GetMaxInputReportLength()}");
                Console.WriteLine($"  Max Output Length: {device.GetMaxOutputReportLength()}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error reading device info: {ex.Message}");
                Console.WriteLine();
            }
        }
    }

    /// <summary>
    /// Hledá SONiX klávesnice
    /// </summary>
    public static void FindSonixKeyboards()
    {
        Console.WriteLine("=== Hledání SONiX klávesnic ===");
        Console.WriteLine($"Hledám VID: 0x{VendorId:X4}, PID: 0x{ProductId:X4}");
        Console.WriteLine();

        var devices = DeviceList.Local.GetHidDevices(VendorId, ProductId);
        
        if (!devices.Any())
        {
            Console.WriteLine("❌ Žádná SONiX klávesnice nebyla nalezena!");
            Console.WriteLine();
            Console.WriteLine("Možné příčiny:");
            Console.WriteLine("1. Klávesnice není připojena");
            Console.WriteLine("2. Jiné VID/PID (zkontrolujte Device Manager)");
            Console.WriteLine("3. Potřebujete spustit jako administrátor");
            Console.WriteLine("4. Chybí ovladače");
            return;
        }

        Console.WriteLine($"✅ Nalezeno {devices.Count()} zařízení:");
        
        foreach (var device in devices)
        {
            try
            {
                Console.WriteLine($"Zařízení: {device.GetProductName() ?? "Unknown"}");
                Console.WriteLine($"  Path: {device.DevicePath}");
                Console.WriteLine($"  Manufacturer: {device.GetManufacturer() ?? "N/A"}");
                
                Console.WriteLine($"  Max Input Length: {device.GetMaxInputReportLength()}");
                Console.WriteLine($"  Max Output Length: {device.GetMaxOutputReportLength()}");
                Console.WriteLine($"  Max Feature Length: {device.GetMaxFeatureReportLength()}");

                // Pokusíme se připojit
                Console.WriteLine("  Zkouším připojení...");
                try
                {
                    using var stream = device.Open();
                    Console.WriteLine("  ✅ Připojení úspěšné!");
                    
                    // Test zápisu
                    Console.WriteLine("  Testuju zápis...");
                    var testPacket = new byte[64]; // Prázdný packet
                    testPacket[0] = 0x04; // Report ID
                    
                    stream.Write(testPacket);
                    Console.WriteLine("  ✅ Zápis úspěšný!");
                    
                    // Test čtení
                    Console.WriteLine("  Testuju čtení...");
                    var buffer = new byte[64];
                    var bytesRead = stream.Read(buffer, 0, buffer.Length);
                    Console.WriteLine($"  ✅ Přečteno {bytesRead} bytů");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ❌ Chyba připojení: {ex.Message}");
                }
                
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Chyba při zpracování zařízení: {ex.Message}");
                Console.WriteLine();
            }
        }
    }

    /// <summary>
    /// Kontroluje oprávnění a požadavky systému
    /// </summary>
    public static void CheckSystemRequirements()
    {
        Console.WriteLine("=== Kontrola systémových požadavků ===");
        
        // Kontrola OS
        Console.WriteLine($"OS: {Environment.OSVersion}");
        Console.WriteLine($"Architektura: {Environment.Is64BitOperatingSystem} ({(Environment.Is64BitProcess ? "64-bit" : "32-bit")} proces)");
        
        // Kontrola HidSharp
        Console.WriteLine($"HidSharp verze: {typeof(HidDevice).Assembly.GetName().Version}");
        
        Console.WriteLine("💡 Tip: Pokud klávesnice nefunguje, zkuste spustit jako administrátor");
        Console.WriteLine();
    }

    /// <summary>
    /// Spustí kompletní diagnostiku
    /// </summary>
    public static void RunCompleteDiagnostics()
    {
        Console.WriteLine("KeyRGB Diagnostika");
        Console.WriteLine("==================");
        Console.WriteLine();
        
        CheckSystemRequirements();
        FindSonixKeyboards();
        
        Console.WriteLine("=== Seznam všech HID zařízení ===");
        ListAllHidDevices();
        
        Console.WriteLine("Diagnostika dokončena!");
    }
}