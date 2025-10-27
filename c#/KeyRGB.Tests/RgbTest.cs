using KeyRGB.Library;
using System.Drawing;

namespace KeyRGB.Tests;

internal class RgbTest
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== RGB Test ===");
        Console.WriteLine();
        
        using var controller = new RGBKeyboardController();
        
        if (controller.Connect())
        {
            Console.WriteLine("✅ Klávesnice připojena!");
            Console.WriteLine();
            
            // Test barev
            var colors = new[] 
            {
                (Color.Red, "Červená"),
                (Color.Green, "Zelená"), 
                (Color.Blue, "Modrá"),
                (Color.Purple, "Fialová"),
                (Color.Orange, "Oranžová"),
                (Color.Yellow, "Žlutá")
            };
            
            foreach (var (color, name) in colors)
            {
                Console.WriteLine($"Nastavuji {name}...");
                if (controller.SetStaticColor(color))
                {
                    Console.WriteLine($"✅ {name} nastavena");
                }
                else
                {
                    Console.WriteLine($"❌ Chyba při nastavení {name}");
                }
                
                Thread.Sleep(1500);
            }
            
            Console.WriteLine();
            Console.WriteLine("Test dokončen! Vracím fialovou...");
            controller.SetStaticColor(Color.Purple);
        }
        else
        {
            Console.WriteLine("❌ Nepodařilo se připojit ke klávesnici");
        }
        
        Console.WriteLine();
        Console.WriteLine("Stiskněte Enter pro ukončení...");
        Console.ReadLine();
    }
}