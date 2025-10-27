using KeyRGB.Library;
using System.Drawing;

namespace KeyRGB.Tests.SpecialColors;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Test bílých kláves s barevným podsvícením ===");
        Console.WriteLine();
        
        using var controller = new RGBKeyboardController();
        
        if (controller.Connect())
        {
            Console.WriteLine("✅ Klávesnice připojena!");
            Console.WriteLine();
            
            // Test special colors s bílými klávesami
            var specialColors = new[] 
            {
                (1, "ORANŽOVÉ podsvícení - VS Code, PhpStorm, Rider"),
                (5, "FIALOVÉ podsvícení - Microsoft Edge"), 
                (3, "ZELENÉ podsvícení - Microsoft Word, Office"),
                (2, "ŽLUTÉ podsvícení - nahrada za červenou"),
                (7, "MODRÉ podsvícení - Ostatní aplikace")
            };
            
            foreach (var (specialColor, description) in specialColors)
            {
                Console.WriteLine($"Nastavuji bílé klávesy s {description}...");
                if (controller.ApplyRgbLighting("white", 100, specialColor))
                {
                    Console.WriteLine($"✅ {description} nastaveno");
                }
                else
                {
                    Console.WriteLine($"❌ Chyba při nastavení {description}");
                }
                
                Console.WriteLine("Stiskněte Enter pro pokračování...");
                Console.ReadLine();
            }
            
            Console.WriteLine();
            Console.WriteLine("Test dokončen! Vracím výchozí fialové podsvícení...");
            controller.ApplyRgbLighting("white", 100, 5);
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