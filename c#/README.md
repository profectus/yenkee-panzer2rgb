# KeyRGB - .NET RGB Keyboard Control Library

🎨 Complete RGB lighting control for SONiX USB keyboards (VID: 0x0c45, PID: 0x8508) in .NET

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![C#](https://img.shields.io/badge/C%23-12-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## 🚀 Overview

This project is a .NET conversion of the original Python RGB keyboard controller. It provides a NuGet library for easy integration into C# applications and includes a console application for testing.

**Note**: White colors are NOT supported by the hardware - only available colors are used.

## 📦 Projects

- **KeyRGB.Library** - Class library for NuGet package containing RGB keyboard control functionality
- **KeyRGB.Console** - Simple console application demonstrating usage of the library

## 🔧 Installation

### Using NuGet Package Manager
```bash
Install-Package KeyRGB.Library
```

### Using .NET CLI
```bash
dotnet add package KeyRGB.Library
```

## 💻 Usage

### Console Application
```bash
# Basic usage with default values
KeyRGB.Console.exe

# Custom key color, intensity, and special effects
KeyRGB.Console.exe "#ff0000" 100 6    # Red keys, red line & volume
KeyRGB.Console.exe blue 50 1          # Blue keys, orange line & volume  
KeyRGB.Console.exe "255,0,0" 75 7     # Red keys, dark blue line & volume
```

### C# Library Usage
```csharp
using KeyRGB.Library;

// Create controller instance
using var controller = new RGBKeyboardController();

// Connect to keyboard
if (controller.Connect())
{
    // Apply RGB lighting
    controller.ApplyRgbLighting("red", 100, 6);
    
    // Or use different color formats
    controller.ApplyRgbLighting("#00ff00", 75, 3);   // Hex format
    controller.ApplyRgbLighting("0,0,255", 50, 4);   // RGB format
    controller.ApplyRgbLighting("purple", 80, 5);    // Color name
}
```

## 🎨 Color Formats

### Key Colors
- **Hex format**: `#ff0000` (red)
- **RGB format**: `255,0,0` (red)  
- **Color names**: `red`, `green`, `blue`, `white`, `black`, `yellow`, `cyan`, `magenta`, `orange`, `purple`, `pink`, `off`

### Special Colors (0-7)
Controls line and volume button colors:
- **0**: Off
- **1**: Orange theme  
- **2**: Yellow theme
- **3**: Green theme
- **4**: Light Blue theme
- **5**: Purple theme (default)
- **6**: Red theme
- **7**: Dark Blue theme

## 🔧 Building from Source

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code with C# extension

### Build Steps
```bash
git clone https://github.com/profectus/yenkee-panzer2rgb.git
cd yenkee-panzer2rgb/c#
dotnet restore
dotnet build
```

### Running Tests
```bash
dotnet run --project KeyRGB.Console
```

## 📋 Requirements

- Windows (HidSharp dependency)
- SONiX USB keyboard (VID: 0x0c45, PID: 0x8508)
- Administrator privileges (for USB HID access)
- No other RGB software using the keyboard

## 🛠️ Technical Details

### Hardware Reverse-Engineered Packet Structure
- **Position 14-16**: RGB values for key lighting
- **Position 28**: Line color control (NO WHITE available)
- **Position 30**: Volume button color control (NO WHITE available)

### Dependencies
- **HidSharp**: USB HID device communication
- **System.Drawing**: Color parsing and manipulation

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 🐛 Troubleshooting

### "Failed to connect" error
- Ensure keyboard is connected and detected by Windows
- Run application as Administrator
- Close any other RGB software (like manufacturer's software)
- Check Device Manager for proper keyboard recognition

### "Failed to apply lighting" error
- Keyboard may be in use by another application
- Try reconnecting the keyboard
- Restart the application

## 🔄 Migration from Python Version

This .NET version provides the same functionality as the original Python version with these improvements:
- Better performance and memory management
- Native Windows integration
- NuGet package distribution
- Strong typing and IntelliSense support
- Easier deployment (no Python runtime required)

## 📊 Version History

- **1.0.0**: Initial .NET conversion with full Python feature parity
