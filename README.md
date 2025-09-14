# SONiX RGB Keyboard Controller

CompleteExamples:
```bash
# Red keys with red line & volume
python rgb_control.py "#ff0000" 100 6

# Blue keys with orange line & volume  
python rgb_control.py blue 50 1

# Green keys with dark blue line & volume
python rgb_control.py "0,255,0" 75 7

# Everything off
python rgb_control.py off 0 0
```control for SONiX USB keyboards through reverse-engineered USB HID packets.

## 🎯 Overview

This project provides full RGB control for SONiX keyboards (VID: `0x0c45`, PID: `0x8508`) by sending custom USB HID packets directly to the hardware. All color mappings and packet structures were discovered through systematic reverse engineering using Wireshark USB packet capture.

## 🚀 Features

- **Ready-to-use EXE**: No Python installation required! 
- **Complete RGB Key Control**: Any color, any intensity (0-100%)
- **Line Color Control**: 7 different colors + off (NO WHITE)
- **Volume Button Control**: 8 different colors + off (NO WHITE)
- **Unified Color System**: Single parameter (0-7) controls both line and volume
- **Multiple Input Formats**: Hex (`#ff0000`), RGB (`255,0,0`), color names (`red`)
- **Cross-platform**: Windows EXE + Python source for Linux/macOS

## � Download

### Latest Release
- **Windows EXE**: [`rgb_keyboard.exe`](../../releases/latest) (~8.2MB)
- **Source Code**: Available in this repository

### Requirements
- **EXE**: Windows 64-bit, SONiX keyboard connected
- **Python**: Python 3.x + `hidapi` library

## 📋 Requirements

- SONiX USB Keyboard (VID: 0x0c45, PID: 0x8508)
- Windows (EXE) or Python 3.x with `hidapi` (source)
- Administrator rights (for USB device access)

## 🛠️ Installation & Usage

### Option 1: Ready-to-use EXE (Recommended)
**No Python installation required!**

1. Download `rgb_keyboard.exe` from releases
2. Connect your SONiX keyboard
3. Run directly:
   ```bash
   rgb_keyboard.exe red 100 6
   ```

### Option 2: Python Script
1. Install Python 3.x
2. Install dependencies:
   ```bash
   pip install hidapi
   ```
3. Run the script:
   ```bash
   python rgb_control.py --help
   ```

## 📖 Usage

### Basic Syntax
```bash
# Using EXE (no Python needed)
rgb_keyboard.exe [key_color] [key_intensity] [special_color]

# Using Python script  
python rgb_control.py [key_color] [key_intensity] [special_color]
```

### Examples
```bash
# Red keys with red line & volume (EXE)
rgb_keyboard.exe "#ff0000" 100 6

# Blue keys with orange line & volume (Python)
python rgb_control.py blue 50 1

# Green keys with dark blue line & volume (EXE)
rgb_keyboard.exe "0,255,0" 75 7

# Everything off
rgb_keyboard.exe off 0 0
```

### Key Color Formats

| Format | Example | Description |
|--------|---------|-------------|
| Hex | `#ff0000` | Standard hex color code |
| RGB | `255,0,0` | Comma-separated RGB values |
| Names | `red`, `blue`, `green` | Predefined color names |

### Special Color Index (0-7) - 8 Logical Themes

| Index | Line Color | Volume Color | Description |
|-------|------------|--------------|-------------|
| 0 | off | off | Everything disabled |
| 1 | orange | orange | Orange theme |
| 2 | yellow | yellow | Yellow theme |
| 3 | green | green | Green theme |
| 4 | light blue | light blue | Light blue theme |
| 5 | purple | purple | Purple theme (default) |
| 6 | **red** | red | **Complete red theme** |
| 7 | dark blue | dark blue | Dark blue theme |

## 🔬 Hardware Reverse Engineering

### USB Packet Structure

The keyboard uses **043b** packet type with 64-byte payloads:

```
Packet Structure (hex):
043b 0406 2200 0000 0006 0301 fa00 RRGGBB 0000 0000 0000 0000 0004 LL00 VV00 ...
                                     ^^^^^^                           ^^   ^^
                                     RGB Values                     Line Volume
                                   (Positions 14-16)               (28) (30)
```

### Key Positions
- **Position 14**: Red value (0-255)
- **Position 15**: Green value (0-255)  
- **Position 16**: Blue value (0-255)
- **Position 28**: Line color index
- **Position 30**: Volume color index

### Discovered Color Values

#### Line Colors (Position 28) - NO WHITE
| Value | Color |
|-------|-------|
| 1 | Orange |
| 2 | Yellow |
| 3 | Green |
| 4 | Light Blue |
| 5 | Dark Blue |
| 6 | Purple |
| 8 | Off |
| 10 | **Red** (newly discovered!) |

#### Volume Colors (Position 30) - NO WHITE
| Value | Color |
|-------|-------|
| 1 | Transition/Rainbow |
| 2 | Red |
| 3 | Orange |
| 4 | Yellow |
| 5 | Green |
| 6 | Light Blue |
| 7 | Dark Blue |
| 8 | Purple |
| 9 | Off |

## 🧪 Reverse Engineering Process

This project was developed through systematic USB packet analysis:

1. **Wireshark Capture**: Captured USB traffic from official software
2. **Pattern Analysis**: Identified packet types and structure
3. **Position Mapping**: Located RGB and special color positions
4. **Systematic Testing**: Tested all possible values for complete mapping
5. **Discovery**: Found hidden red line color (value 10) not in official software

### Original Analysis Files
- `wireshark/`: USB packet captures
- `keyboard_settings/`: Original JSON configuration files

## 🎨 Color Combinations

### Popular Themes
```bash
# Gaming setup - red everything (EXE)
rgb_keyboard.exe red 100 6

# Ocean theme - blues (Python)
python rgb_control.py "#0066cc" 75 7

# Sunset theme - orange/yellow (EXE)
rgb_keyboard.exe orange 80 1

# Nature theme - greens (Python)
python rgb_control.py green 90 3

# Stealth mode - minimal lighting (EXE)
rgb_keyboard.exe "#333333" 25 0
```

## 📁 Project Structure

```
├── rgb_keyboard.exe    # Ready-to-use executable (Windows)
├── rgb_control.py      # Main controller script
├── rgb_effects.py      # Low-level HID communication
├── README.md          # This file
└── .gitignore         # Git ignore rules
```

## 🤝 Contributing

If you discover new features or support additional SONiX keyboard models:

1. Use Wireshark to capture USB traffic
2. Analyze packet patterns
3. Test systematically
4. Submit findings with documentation

## ⚠️ Disclaimer

This software directly communicates with USB hardware through reverse-engineered protocols. Use at your own risk. The authors are not responsible for any hardware damage.

## 🏆 Credits

Developed through collaborative reverse engineering and systematic hardware analysis. Special thanks to the open-source community for USB analysis tools.

## 📄 License

MIT License - Feel free to use, modify, and distribute.

---

**Happy RGB lighting!** 🌈✨
