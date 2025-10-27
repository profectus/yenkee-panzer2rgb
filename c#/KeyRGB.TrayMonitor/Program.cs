namespace KeyRGB.TrayMonitor;

internal static class Program
{
    /// <summary>
    /// Hlavní vstupní bod aplikace
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Nastavíme aplikaci pro vysoké DPI
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Kontrola, zda už aplikace neběží
        bool createdNew;
        using var mutex = new Mutex(true, "KeyRGB.TrayMonitor.Mutex", out createdNew);
        
        if (!createdNew)
        {
            MessageBox.Show("KeyRGB Tray Monitor již běží!", "Aplikace je spuštěna", 
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            // Spustíme tray aplikaci
            using var trayApp = new TrayApplication();
            Application.Run();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Nepodařilo se spustit aplikaci: {ex.Message}", "Chyba", 
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}