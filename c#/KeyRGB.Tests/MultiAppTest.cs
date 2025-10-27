using KeyRGB.Library;
using System.Drawing;

namespace KeyRGB.Tests.MultiApp;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Test barev pro různé aplikace ===");
        Console.WriteLine();
        
        using var controller = new RGBKeyboardController();
        
        if (controller.Connect())
        {
            Console.WriteLine("✅ Klávesnice připojena!");
            Console.WriteLine();
            
            // Test aplikačních barev
            var appColors = new[] 
            {
                (Color.Orange, "ORANŽOVÁ - VS Code, PhpStorm, Rider"),
                (Color.Purple, "FIALOVÁ - Microsoft Edge"), 
                (Color.Green, "ZELENÁ - Microsoft Word, Office"),
                (Color.Blue, "MODRÁ - Ostatní aplikace")
            };
            
            foreach (var (color, description) in appColors)
            {
                Console.WriteLine($"Nastavuji {description}...");
                if (controller.SetStaticColor(color))
                {
                    Console.WriteLine($"✅ {description} nastavena");
                }
                else
                {
                    Console.WriteLine($"❌ Chyba při nastavení {description}");
                }
                
                Console.WriteLine("Stiskněte Enter pro pokračování...");
                Console.ReadLine();
            }
            
            Console.WriteLine();
            Console.WriteLine("Test dokončen!");
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