# KeyRGB Tray Monitor

Aplikace pro Windows, která běží v systémovém trayi a automaticky mění barvu RGB klávesnice podle aktivní aplikace.

## Funkce

- **Automatické sledování aktivního okna** - aplikace každých 500ms kontroluje, která aplikace je aktivní
- **Barevné schéma**:
  - 🟢 **Zelená** - když je aktivní Microsoft Edge
  - 🟣 **Fialová** - pro všechny ostatní aplikace
- **Systémový tray** - aplikace běží v pozadí bez rušení
- **Notifikace** - informace o změnách a stavu aplikace
- **Jednotná instance** - zabraňuje spuštění více kopií aplikace současně

## Požadavky

- Windows 10/11
- .NET 8 Runtime
- SONiX RGB klávesnice (VID: 0x0c45, PID: 0x8508)
- Oprávnění správce (pro přístup k USB zařízení)

## Instalace a spuštění

1. Sestavte projekt v Release módu:
   ```bash
   dotnet build -c Release
   ```

2. Spusťte aplikaci jako správce:
   ```bash
   cd KeyRGB.TrayMonitor\bin\Release\net8.0-windows
   KeyRGB.TrayMonitor.exe
   ```

3. Aplikace se spustí v systémovém trayi (pravý dolní roh)

## Ovládání

### Tray menu
- **Zobrazit stav** - ukáže informace o aktuálním stavu
- **Ukončit** - zavře aplikaci

### Klávesové zkratky
- Dvojklik na ikonu v trayi - zobrazí stav aplikace

## Technické detaily

### Monitorování oken
- Používá Windows API (`GetForegroundWindow`, `GetWindowText`, `GetWindowThreadProcessId`)
- Kontroluje každých 500ms aktivní okno
- Detekuje procesy `msedge` a `edge` jako Microsoft Edge

### RGB ovládání
- Používá knihovnu `KeyRGB.Library`
- Zelená barva: RGB(0, 255, 0) s green theme
- Fialová barva: RGB(128, 0, 128) s purple theme
- Ovládá jak klávesnici, tak volume tlačítka

### Bezpečnost
- Mutex zabraňuje spuštění více instancí
- Graceful shutdown s uvolněním zdrojů
- Exception handling pro stabilní běh

## Řešení problémů

### Aplikace se nespustí
- Zkontrolujte oprávnění správce
- Ověřte, že je klávesnice připojená
- Zkontrolujte .NET 8 runtime

### Barvy se nemění
- Restartujte aplikaci
- Zkontrolujte připojení klávesnice
- Ověřte v Device Manager přítomnost SONiX zařízení

### Aplikace "zmizí"
- Hledejte ikonu v systémovém trayi
- Pokud není vidět, možná je skrytá - klikněte na šipku v trayi

## Struktura projektu

```
KeyRGB.TrayMonitor/
├── Program.cs              # Vstupní bod aplikace
├── TrayApplication.cs      # Hlavní tray aplikace
├── WindowMonitor.cs        # Sledování aktivních oken
└── KeyRGB.TrayMonitor.csproj
```

## Rozšíření

Aplikaci lze snadno rozšířit o:
- Více aplikací s různými barvami
- Konfigurační soubor pro barvy
- Efekty místo statických barev
- Profily pro různé uživatele
- Plánované změny barev

## Licence

Tento projekt je součástí KeyRGB solution a používá stejnou licenci.