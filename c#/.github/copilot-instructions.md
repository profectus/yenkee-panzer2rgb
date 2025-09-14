# .NET RGB Keyboard Control Library

This workspace contains a .NET solution for RGB keyboard control functionality, originally converted from Python.

## Project Structure
- **KeyRGB.Library** - Class library for NuGet package containing RGB keyboard control functionality
- **KeyRGB.Console** - Simple console application demonstrating usage of the library

## Development Guidelines
- Use C# 12 and .NET 8
- Follow Microsoft coding standards
- Include XML documentation for public APIs
- Use dependency injection where appropriate
- Implement proper error handling and logging

## RGB Keyboard Functionality
- USB HID communication for keyboard control
- RGB effect implementations (static colors, breathing, wave effects)
- Hardware abstraction for different keyboard models
- Thread-safe operations for real-time effects

## Testing
- Unit tests for core functionality
- Integration tests with mock hardware
- Console application for manual testing
