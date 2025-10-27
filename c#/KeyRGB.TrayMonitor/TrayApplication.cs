using System.ComponentModel;
using KeyRGB.Library;

namespace KeyRGB.TrayMonitor;

/// <summary>
/// Hlavní tray aplikace pro monitorování aktivního okna a ovládání RGB klávesnice
/// </summary>
public partial class TrayApplication : Form
{
    private NotifyIcon? _trayIcon;
    private System.Windows.Forms.Timer? _windowMonitorTimer;
    private RGBKeyboardController? _keyboardController;
    private string _lastActiveProcess = string.Empty;
    private readonly object _lockObject = new();

    // Special colors pro různé aplikace (pouze podsvícení)
    private const int VSCodeSpecialColor = 1;        // Oranžové podsvícení - VS Code, PhpStorm, Rider
    private const int EdgeSpecialColor = 7;          // Modré podsvícení - Microsoft Edge  
    private const int WordSpecialColor = 3;          // Zelené podsvícení - Microsoft Word
    private const int YellowSpecialColor = 2;        // Žluté podsvícení - nahrada za červenou
    private const int DefaultSpecialColor = 5;       // Fialové podsvícení - Ostatní aplikace

    public TrayApplication()
    {
        InitializeComponent();
        InitializeTrayIcon();
        InitializeKeyboardController();
        InitializeWindowMonitoring();
        
        // Skryjeme hlavní okno - aplikace běží pouze v tray
        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
        this.Visible = false;
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        
        // Form
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(0, 0);
        this.FormBorderStyle = FormBorderStyle.None;
        this.Name = "TrayApplication";
        this.Text = "KeyRGB Tray Monitor";
        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
        
        this.ResumeLayout(false);
    }

    private void InitializeTrayIcon()
    {
        _trayIcon = new NotifyIcon
        {
            Text = "KeyRGB Monitor - Oranžová IDE, Modrá Edge, Zelená Office, Fialová ostatní",
            Visible = true
        };

        // Vytvoříme jednoduchý context menu
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Zobrazit stav", null, ShowStatus);
        contextMenu.Items.Add("-");
        contextMenu.Items.Add("Ukončit", null, ExitApplication);
        
        _trayIcon.ContextMenuStrip = contextMenu;
        _trayIcon.DoubleClick += ShowStatus;

        // Nastavíme ikonu (můžeme použít systémovou ikonu)
        _trayIcon.Icon = SystemIcons.Application;
    }

    private void InitializeKeyboardController()
    {
        try
        {
            _keyboardController = new RGBKeyboardController();
            
            // Pokusíme se připojit
            if (!_keyboardController.Connect())
            {
                var errorMsg = "Nepodařilo se najít RGB klávesnici!\n\n" +
                              "Možná řešení:\n" +
                              "• Zkontrolujte připojení klávesnice\n" +
                              "• Spusťte aplikaci jako administrátor\n" +
                              "• Zkontrolujte Device Manager";
                
                ShowBalloonTip("RGB Klávesnice nenalezena", errorMsg, ToolTipIcon.Warning);
                
                // Aktualizujeme tooltip
                if (_trayIcon != null)
                {
                    _trayIcon.Text = "KeyRGB - Klávesnice nenalezena";
                }
            }
            else
            {
                ShowBalloonTip("KeyRGB Monitor", "RGB klávesnice úspěšně připojena!", ToolTipIcon.Info);
            }
        }
        catch (Exception ex)
        {
            ShowBalloonTip("Chyba", $"Nepodařilo se inicializovat ovladač klávesnice: {ex.Message}", ToolTipIcon.Error);
            
            // Aktualizujeme tooltip
            if (_trayIcon != null)
            {
                _trayIcon.Text = $"KeyRGB - Chyba: {ex.Message}";
            }
        }
    }

    private void InitializeWindowMonitoring()
    {
        _windowMonitorTimer = new System.Windows.Forms.Timer
        {
            Interval = 500 // Kontrolujeme každých 500ms
        };
        _windowMonitorTimer.Tick += OnWindowMonitorTick;
        _windowMonitorTimer.Start();
    }

    private void OnWindowMonitorTick(object? sender, EventArgs e)
    {
        lock (_lockObject)
        {
            try
            {
                var currentProcess = WindowMonitor.GetActiveWindowProcessName();
                
                // Kontrolujeme pouze pokud se proces změnil
                if (currentProcess != _lastActiveProcess)
                {
                    _lastActiveProcess = currentProcess;
                    UpdateKeyboardColor();
                }
            }
            catch (Exception ex)
            {
                // Logujeme chybu, ale nepřerušujeme monitoring
                System.Diagnostics.Debug.WriteLine($"Chyba při monitorování okna: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Určí special color a název podle aktivní aplikace
    /// </summary>
    /// <param name="processName">Název procesu</param>
    /// <returns>Tuple s special color indexem a názvem typu aplikace</returns>
    private (int specialColor, string appType) GetSpecialColorForActiveApp(string processName)
    {
        if (string.IsNullOrEmpty(processName))
            return (DefaultSpecialColor, "Neznámá");

        var process = processName.ToLowerInvariant();

        // VS Code, PhpStorm, Rider - Oranžové podsvícení
        if (process.Contains("code") || 
            process.Contains("phpstorm") || 
            process.Contains("rider") ||
            process.Contains("webstorm") ||
            process.Contains("intellij") ||
            process.Contains("pycharm") ||
            process.Contains("clion") ||
            process.Contains("datagrip") ||
            process.Contains("goland") ||
            process.Contains("rubymine"))
        {
            return (VSCodeSpecialColor, "Vývojářské IDE - Oranžové podsvícení");
        }

        // Microsoft Edge - Modré podsvícení
        if (process.Contains("msedge") || process.Contains("edge"))
        {
            return (EdgeSpecialColor, "Edge - Modré podsvícení");
        }

        // Microsoft Word - Zelené podsvícení
        if (process.Contains("winword") || 
            process.Contains("word") ||
            process.Contains("excel") ||
            process.Contains("powerpoint") ||
            process.Contains("outlook"))
        {
            return (WordSpecialColor, "Office - Zelené podsvícení");
        }

        // Ostatní aplikace - Fialové podsvícení
        return (DefaultSpecialColor, "Ostatní - Fialové podsvícení");
    }

    private void UpdateKeyboardColor()
    {
        if (_keyboardController == null) return;

        try
        {
            // Zkontrolujeme, zda je klávesnice stále připojena
            if (!_keyboardController.IsConnected)
            {
                // Pokusíme se znovu připojit
                if (!_keyboardController.Connect())
                {
                    if (_trayIcon != null)
                    {
                        _trayIcon.Text = "KeyRGB - Klávesnice odpojena";
                    }
                    return;
                }
            }

            var (specialColor, appType) = GetSpecialColorForActiveApp(_lastActiveProcess);
            var appName = string.IsNullOrEmpty(_lastActiveProcess) ? "Unknown" : _lastActiveProcess;

            // Použijeme bílé klávesy na 75% intenzitu a pouze změníme podsvícení
            if (_keyboardController.ApplyRgbLighting("white", 75, specialColor))
            {
                // Aktualizujeme tooltip při úspěchu
                if (_trayIcon != null)
                {
                    _trayIcon.Text = $"KeyRGB - Aktivní: {appName} ({appType})";
                }
            }
            else
            {
                // Chyba při nastavování barvy
                if (_trayIcon != null)
                {
                    _trayIcon.Text = "KeyRGB - Chyba při nastavování barvy";
                }
            }
        }
        catch (Exception ex)
        {
            ShowBalloonTip("Chyba", $"Nepodařilo se nastavit barvu klávesnice: {ex.Message}", ToolTipIcon.Warning);
            
            if (_trayIcon != null)
            {
                _trayIcon.Text = $"KeyRGB - Chyba: {ex.Message}";
            }
        }
    }

    private void ShowStatus(object? sender, EventArgs e)
    {
        var windowTitle = WindowMonitor.GetActiveWindowTitle();
        var processName = WindowMonitor.GetActiveWindowProcessName();
        var (specialColor, appType) = GetSpecialColorForActiveApp(processName);

        var keyboardStatus = _keyboardController?.IsConnected == true ? "Připojena" : "Odpojena";

        var message = $"Aktivní okno: {windowTitle}\n" +
                     $"Proces: {processName}\n" +
                     $"Typ aplikace: {appType}\n" +
                     $"Special Color: {specialColor}\n" +
                     $"Stav klávesnice: {keyboardStatus}";

        ShowBalloonTip("Stav KeyRGB Monitor", message, ToolTipIcon.Info);
    }

    private void ShowBalloonTip(string title, string text, ToolTipIcon icon)
    {
        _trayIcon?.ShowBalloonTip(3000, title, text, icon);
    }

    private void ExitApplication(object? sender, EventArgs e)
    {
        _windowMonitorTimer?.Stop();
        _trayIcon?.Dispose();
        _keyboardController?.Dispose();
        Application.Exit();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _windowMonitorTimer?.Dispose();
            _trayIcon?.Dispose();
            _keyboardController?.Dispose();
        }
        base.Dispose(disposing);
    }

    protected override void SetVisibleCore(bool value)
    {
        // Zajistíme, že okno nebude nikdy viditelné
        base.SetVisibleCore(false);
    }
}