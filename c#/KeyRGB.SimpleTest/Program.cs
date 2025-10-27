using HidSharp;

namespace KeyRGB.SimpleTest;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== KeyRGB Jednoduchý Test ===");
        Console.WriteLine();
        
        Console.WriteLine("Hledám všechna HID zařízení...");
        var allDevices = DeviceList.Local.GetHidDevices();
        Console.WriteLine($"Nalezeno {allDevices.Count()} HID zařízení");
        Console.WriteLine();
        
        Console.WriteLine("HID zařízení:");
        foreach (var device in allDevices)
        {
            try
            {
                Console.WriteLine($"VID: 0x{device.VendorID:X4}, PID: 0x{device.ProductID:X4} - {device.GetProductName() ?? "Unknown"}");
            }
            catch
            {
                Console.WriteLine($"VID: 0x{device.VendorID:X4}, PID: 0x{device.ProductID:X4} - Chyba při čtení názvu");
            }
        }
        
        Console.WriteLine();
        Console.WriteLine("Hledám SONiX klávesnici (VID: 0x0C45, PID: 0x8508)...");
        
        var sonixDevices = DeviceList.Local.GetHidDevices(0x0c45, 0x8508);
        
        if (sonixDevices.Any())
        {
            Console.WriteLine($"✅ Nalezeno {sonixDevices.Count()} SONiX zařízení!");
            
            foreach (var device in sonixDevices)
            {
                try
                {
                    Console.WriteLine($"Zařízení: {device.GetProductName()}");
                    Console.WriteLine($"Cesta: {device.DevicePath}");
                    
                    Console.WriteLine("Zkouším otevřít zařízení...");
                    using var stream = device.Open();
                    Console.WriteLine("✅ Úspěšně otevřeno!");
                    
                    Console.WriteLine("✅ Klávesnice je připravena k použití!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Chyba při otevírání: {ex.Message}");
                    Console.WriteLine("💡 Zkuste spustit jako administrátor");
                }
            }
        }
        else
        {
            Console.WriteLine("❌ SONiX klávesnice nebyla nalezena!");
            Console.WriteLine();
            Console.WriteLine("Možné příčiny:");
            Console.WriteLine("1. Klávesnice není připojena");
            Console.WriteLine("2. Jiné VID/PID - zkontrolujte Device Manager");
            Console.WriteLine("3. Potřebujete spustit jako administrátor");
            Console.WriteLine();
            
            Console.WriteLine("Všechna nalezená zařízení s podobným VID:");
            var similarDevices = allDevices.Where(d => d.VendorID == 0x0c45);
            foreach (var device in similarDevices)
            {
                try
                {
                    Console.WriteLine($"  VID: 0x{device.VendorID:X4}, PID: 0x{device.ProductID:X4} - {device.GetProductName() ?? "Unknown"}");
                }
                catch
                {
                    Console.WriteLine($"  VID: 0x{device.VendorID:X4}, PID: 0x{device.ProductID:X4} - Chyba při čtení");
                }
            }
        }
        
        Console.WriteLine();
        Console.WriteLine("Stiskněte Enter pro ukončení...");
        Console.ReadLine();
    }
}