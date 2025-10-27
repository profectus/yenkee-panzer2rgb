using System.Runtime.InteropServices;

namespace KeyRGB.TrayMonitor;

/// <summary>
/// Třída pro monitorování aktivního okna pomocí Windows API
/// </summary>
public static class WindowMonitor
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

    [DllImport("user32.dll")]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    /// <summary>
    /// Získá název aktivního okna
    /// </summary>
    /// <returns>Název aktivního okna</returns>
    public static string GetActiveWindowTitle()
    {
        const int nChars = 256;
        IntPtr handle = GetForegroundWindow();
        
        System.Text.StringBuilder sb = new(nChars);
        if (GetWindowText(handle, sb, nChars) > 0)
        {
            return sb.ToString();
        }
        return string.Empty;
    }

    /// <summary>
    /// Získá název procesu aktivního okna
    /// </summary>
    /// <returns>Název procesu aktivního okna</returns>
    public static string GetActiveWindowProcessName()
    {
        try
        {
            IntPtr handle = GetForegroundWindow();
            GetWindowThreadProcessId(handle, out uint processId);
            
            using var process = System.Diagnostics.Process.GetProcessById((int)processId);
            return process.ProcessName;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Kontroluje, zda je aktivní okno Microsoft Edge
    /// </summary>
    /// <returns>True pokud je aktivní Edge</returns>
    public static bool IsEdgeActive()
    {
        var processName = GetActiveWindowProcessName().ToLowerInvariant();
        return processName.Contains("msedge") || processName.Contains("edge");
    }
}