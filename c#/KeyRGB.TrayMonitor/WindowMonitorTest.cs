using KeyRGB.TrayMonitor;

namespace KeyRGB.Tests;

/// <summary>
/// Rychlý test pro ověření funkcionality WindowMonitor
/// </summary>
internal class WindowMonitorTest
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== KeyRGB TrayMonitor Test ===");
        Console.WriteLine();
        
        Console.WriteLine("Test 1: Získání aktivního okna");
        var windowTitle = WindowMonitor.GetActiveWindowTitle();
        var processName = WindowMonitor.GetActiveWindowProcessName();
        var isEdge = WindowMonitor.IsEdgeActive();
        
        Console.WriteLine($"Aktivní okno: {windowTitle}");
        Console.WriteLine($"Proces: {processName}");
        Console.WriteLine($"Je Edge: {isEdge}");
        Console.WriteLine($"Barva: {(isEdge ? "Zelená" : "Fialová")}");
        Console.WriteLine();
        
        Console.WriteLine("Test 2: Kontinuální monitoring (10 sekund)");
        Console.WriteLine("Zkuste přepínat mezi aplikacemi...");
        Console.WriteLine();
        
        string lastProcess = "";
        var endTime = DateTime.Now.AddSeconds(10);
        
        while (DateTime.Now < endTime)
        {
            var currentProcess = WindowMonitor.GetActiveWindowProcessName();
            var currentIsEdge = WindowMonitor.IsEdgeActive();
            
            if (currentProcess != lastProcess)
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss");
                var color = currentIsEdge ? "ZELENÁ" : "FIALOVÁ";
                Console.WriteLine($"[{timestamp}] {currentProcess} -> {color}");
                lastProcess = currentProcess;
            }
            
            Thread.Sleep(100);
        }
        
        Console.WriteLine();
        Console.WriteLine("Test dokončen!");
        Console.WriteLine("Stiskněte Enter pro ukončení...");
        Console.ReadLine();
    }
}