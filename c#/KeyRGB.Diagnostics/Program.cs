using KeyRGB.Library;

namespace KeyRGB.Diagnostics;

/// <summary>
/// Jednoduchý diagnostický program pro testování připojení klávesnice
/// </summary>
internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("KeyRGB Diagnostika");
        Console.WriteLine("==================");
        Console.WriteLine();

        // Test 1: Základní diagnostika
        Console.WriteLine("1. Spouštím diagnostiku...");
        try
        {
            KeyboardDiagnostics.RunCompleteDiagnostics();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chyba při diagnostice: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("2. Test připojení klávesnice...");

        // Test 2: Pokus o připojení
        try
        {
            using var controller = new RGBKeyboardController();
            
            Console.WriteLine("Pokouším se připojit ke klávesnici...");
            
            if (controller.Connect())
            {
                Console.WriteLine("✅ Připojení úspěšné!");
                
                // Test nastavení barvy
                Console.WriteLine("Testuji nastavení červené barvy...");
                if (controller.SetStaticColor(System.Drawing.Color.Red))
                {
                    Console.WriteLine("✅ Červená barva nastavena!");
                }
                else
                {
                    Console.WriteLine("❌ Nepodařilo se nastavit barvu");
                }
                
                Thread.Sleep(2000);
                
                Console.WriteLine("Testuji nastavení zelené barvy...");
                if (controller.SetStaticColor(System.Drawing.Color.Green))
                {
                    Console.WriteLine("✅ Zelená barva nastavena!");
                }
                else
                {
                    Console.WriteLine("❌ Nepodařilo se nastavit barvu");
                }
                
                Thread.Sleep(2000);
                
                Console.WriteLine("Vracím fialovou barvu...");
                controller.SetStaticColor(System.Drawing.Color.Purple);
            }
            else
            {
                Console.WriteLine("❌ Nepodařilo se připojit ke klávesnici!");
                Console.WriteLine();
                Console.WriteLine("Možné řešení:");
                Console.WriteLine("1. Zkontrolujte, že je klávesnice připojena");
                Console.WriteLine("2. Spusťte aplikaci jako administrátor");
                Console.WriteLine("3. Zkontrolujte VID/PID v Device Manageru");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Chyba: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        Console.WriteLine();
        Console.WriteLine("Stiskněte Enter pro ukončení...");
        Console.ReadLine();
    }
}