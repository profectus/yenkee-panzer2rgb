# KeyRGB TrayMonitor - Návod k použití

## Co aplikace dělá

TrayMonitor je aplikace, která běží v systémovém tray (vedle hodin) a automaticky mění barvu RGB klávesnice podle aktivní aplikace:

- **Zelená barva** - když je aktivní Microsoft Edge
- **Fialová barva** - když je aktivní jakákoliv jiná aplikace

## Jak spustit aplikace

1. **Sestavení aplikace:**
   ```powershell
   dotnet build KeyRGB.TrayMonitor\KeyRGB.TrayMonitor.csproj --configuration Release
   ```

2. **Spuštění aplikace:**
   ```powershell
   .\KeyRGB.TrayMonitor\bin\Release\net8.0-windows\KeyRGB.TrayMonitor.exe
   ```

## Použití aplikace

### Automatické spuštění s Windows
Pro automatické spuštění při startu Windows:
1. Zkopírujte aplikaci do složky: `%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup`
2. Nebo přidejte do registrů Windows (doporučeno pro pokročilé uživatele)

### Ovládání z tray

Aplikace se po spuštění skryje do systémového tray. Najdete ji vedle hodin:

- **Ikona aplikace** - zobrazuje aktuální stav
- **Kliknutí pravým tlačítkem** - otevře context menu:
  - *Zobrazit stav* - ukáže informace o aktivním okně a barvě
  - *Ukončit* - ukončí aplikace

- **Dvojklik na ikonu** - rychle zobrazí aktuální stav

### Tooltip informace
Při najetí myší na ikonu v tray se zobrazí tooltip s aktuálními informacemi:
- Název aktivní aplikace
- Aktuální barva klávesnice

## Podporované aplikace

### Microsoft Edge
Aplikace rozpoznává tyto procesy jako Edge:
- `msedge.exe` (standardní Edge)
- `edge.exe` (starší verze)

### Ostatní aplikace
Všechny ostatní aplikace (Chrome, Firefox, VS Code, atd.) aktivují fialovou barvu.

## Přizpůsobení barev

Pokud chcete změnit barvy, upravte konstanty v souboru `TrayApplication.cs`:

```csharp
// Barvy pro různé stavy
private static readonly Color EdgeColor = Color.Green;      // Barva pro Edge
private static readonly Color OtherColor = Color.Purple;    // Barva pro ostatní
```

Dostupné barvy můžete najít v dokumentaci .NET `System.Drawing.Color` nebo použít vlastní RGB hodnoty:
```csharp
Color.FromArgb(255, 0, 0)  // Červená
Color.FromArgb(0, 255, 0)  // Zelená  
Color.FromArgb(0, 0, 255)  // Modrá
```

## Rozšíření funkcionalita

### Přidání dalších aplikací

Pro rozpoznávání dalších aplikací upravte metodu v `WindowMonitor.cs`:

```csharp
public static bool IsSpecificApp()
{
    var processName = GetActiveWindowProcessName().ToLowerInvariant();
    return processName.Contains("chrome") || 
           processName.Contains("firefox") ||
           processName.Contains("váš-proces");
}
```

### Více barev pro různé aplikace

Můžete rozšířit logiku v `TrayApplication.cs` pro podporu více aplikací:

```csharp
private Color GetColorForProcess(string processName)
{
    return processName.ToLowerInvariant() switch
    {
        var name when name.Contains("msedge") => Color.Green,
        var name when name.Contains("chrome") => Color.Blue,
        var name when name.Contains("firefox") => Color.Orange,
        var name when name.Contains("code") => Color.Red,
        _ => Color.Purple
    };
}
```

## Troubleshooting

### Aplikace se nespustila
- Zkontrolujte, zda máte připojenou kompatibilní RGB klávesnici
- Ověřte, že máte nainstalovaný .NET 8 Runtime
- Spusťte jako administrátor (některé klávesnice to vyžadují)

### Klávesnice nemění barvu
- Zkontrolujte, zda je klávesnice správně rozpoznána systémem
- Restartujte aplikaci
- Zkontrolujte Device Manager pro USB HID zařízení

### Aplikace běží vícekrát
Aplikace má ochranu proti vícenásobnému spuštění pomocí Mutex. Pokud se pokusíte spustit druhou instanci, zobrazí se upozornění.

## Systémové požadavky

- **OS:** Windows 10/11
- **Framework:** .NET 8 Runtime
- **Klávesnica:** SONiX USB RGB keyboard (VID: 0x0c45, PID: 0x8508)
- **Oprávnění:** Aplikace potřebuje přístup k USB zařízením

## Vývojářské informace

- **Jazyk:** C# 12
- **Framework:** .NET 8, WinForms
- **Architektura:** Single-threaded UI s timerem pro monitoring
- **Interval monitorování:** 500ms (konfigurovatelné)
- **Memory footprint:** ~15-20 MB

---

Pro další informace nebo hlášení chyb kontaktujte vývojáře projektu.