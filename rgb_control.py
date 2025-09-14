#!/usr/bin/env python3
"""
🎨 SONiX RGB Keyboard Controller
================================

Complete RGB lighting control for SONiX USB keyboards (VID: 0x0c45, PID: 0x8508)
NOTE: White colors are NOT supported by hardware - only available colors are used.

Usage: python rgb_control.py [key_color] [key_intensity] [special_color]

Examples:
  python rgb_control.py "#ff0000" 100 8    # Red keys, red line & volume
  python rgb_control.py blue 50 1          # Blue keys, orange line & volume  
  python rgb_control.py "255,0,0" 75 7     # Red keys, dark blue line & volume

Hardware reverse-engineered packet structure:
- Position 14-16: RGB values for key lighting
- Position 28: Line color control (NO WHITE available)
- Position 30: Volume button color control (NO WHITE available)
"""

import sys
from rgb_effects import SonixKeyboard

class RGBKeyboardController:
    def __init__(self):
        self.keyboard = SonixKeyboard()
        
        # Base packet template for white keys
        self.base_packet = "043b04062200000000060301fa00ffffff0000000000000000000004060008000000000000000000000000000000000000000000000000000000000000000000"
        
        # FINAL color mapping - 8 LOGICAL combinations (0-7)
        # Each number sets unified colors for both line and volume
        self.special_colors = {
            0: {"line": 8, "volume": 9},     # off
            1: {"line": 1, "volume": 3},     # orange theme
            2: {"line": 2, "volume": 4},     # yellow theme  
            3: {"line": 3, "volume": 5},     # green theme
            4: {"line": 4, "volume": 6},     # light blue theme
            5: {"line": 6, "volume": 8},     # purple theme (default)
            6: {"line": 10, "volume": 2},    # red theme (line=10 discovered!)
            7: {"line": 5, "volume": 7},     # dark blue theme
        }

    def parse_color(self, color_input):
        """Parse RGB color - hex, RGB tuple, or color name"""
        if not color_input:
            return None
            
        color_str = str(color_input).strip().lower()
        
        # Hex format (#ff0000)
        if color_str.startswith('#') and len(color_str) == 7:
            try:
                r = int(color_str[1:3], 16)
                g = int(color_str[3:5], 16)
                b = int(color_str[5:7], 16)
                return (r, g, b)
            except ValueError:
                return None
        
        # RGB format (255,0,0)
        if ',' in color_str:
            try:
                parts = [int(x.strip()) for x in color_str.split(',')]
                if len(parts) == 3 and all(0 <= x <= 255 for x in parts):
                    return tuple(parts)
            except (ValueError, TypeError):
                return None
        
        # Color names
        color_names = {
            'red': (255, 0, 0), 'green': (0, 255, 0), 'blue': (0, 0, 255),
            'white': (255, 255, 255), 'black': (0, 0, 0), 'yellow': (255, 255, 0),
            'cyan': (0, 255, 255), 'magenta': (255, 0, 255), 'orange': (255, 165, 0),
            'purple': (128, 0, 128), 'pink': (255, 192, 203), 'off': (0, 0, 0)
        }
        
        return color_names.get(color_str, None)

    def parse_special_color(self, color_input):
        """Parse special color index (0-7)"""
        color_str = str(color_input).strip()
        
        if color_str.isdigit():
            val = int(color_str)
            if 0 <= val <= 7:
                return val
        
        return 5  # default purple

    def connect(self):
        return self.keyboard.connect()

    def disconnect(self):
        return self.keyboard.disconnect()

    def apply_rgb_lighting(self, key_color="white", key_intensity=100, special_color=5):
        """Apply complete RGB lighting - ALL IN ONE PACKET"""
        
        # Parse key RGB color
        rgb = self.parse_color(key_color)
        if not rgb:
            rgb = (255, 255, 255)  # default white
            
        # Apply intensity to RGB
        intensity = max(0, min(100, int(key_intensity))) / 100.0
        rgb = tuple(int(c * intensity) for c in rgb)
        
        # Parse special colors (line and volume together)
        special_val = self.parse_special_color(special_color)
        colors = self.special_colors[special_val]
        line_val = colors["line"] 
        volume_val = colors["volume"]

        r, g, b = rgb
        print(f"🎨 Setting RGB: keys=({r}, {g}, {b}), special={special_val} (line={line_val}, volume={volume_val})")
        
        # Create modified packet
        data_bytes = bytes.fromhex(self.base_packet)
        data_list = list(data_bytes)
        
        # RGB colors for keys (positions 14-16)
        data_list[14] = r  # Red
        data_list[15] = g  # Green  
        data_list[16] = b  # Blue
        
        # Position 28 = line color
        data_list[28] = line_val
        
        # Position 30 = volume color
        data_list[30] = volume_val
        
        data = bytes(data_list)
        print(f"🔧 Packet: {data.hex()[:50]}...")
        success = self.keyboard._send_packet(data)
        
        if success:
            print(f"✅ Complete setup: RGB=#{r:02x}{g:02x}{b:02x}, special={special_val}")
        else:
            print("❌ Error setting colors")
            
        return success

def print_help():
    """Print usage help"""
    print("""
🎨 SONiX RGB Keyboard Controller
===============================

Usage:
python rgb_control.py [key_color] [key_intensity] [special_color]

Parameters:
  key_color     - RGB key color (hex, RGB, or name) - ANY COLOR  
  key_intensity - RGB key intensity (0-100)
  special_color - Color index for line & volume (0-9)

Key Color Formats:
  🎨 HEX:   #ff0000, #00ff00, #0000ff
  🎨 RGB:   255,0,0   0,255,0   0,0,255  
  🎨 Names: red, blue, green, purple, white, yellow, orange, pink, cyan, off

Special Colors (0-9):
  0 = off           5 = purple
  1 = orange        6 = line=white, volume=red  
  2 = yellow        7 = dark blue
  3 = green         8 = RED (COMPLETE RED!)
  4 = light blue    9 = off (backup)

Examples:
python rgb_control.py "#ff0000" 100 8       # Red keys + red line & volume
python rgb_control.py "0,255,0" 75 1        # Green keys + orange line & volume  
python rgb_control.py blue 50 3             # Blue keys + green line & volume
python rgb_control.py white 25 7            # White keys + dark blue line & volume
python rgb_control.py off 0 0               # Everything off

Hardware Info:
- Device: SONiX USB Keyboard (VID: 0x0c45, PID: 0x8508)
- Packet: 043b type, 64 bytes
- RGB positions: 14-16 (R,G,B)
- Line position: 28 (values: 1,2,3,4,5,6,7,8,10)
- Volume position: 30 (values: 1-9)
""")

def main():
    if len(sys.argv) == 1 or sys.argv[1] in ["-h", "--help", "help"]:
        print_help()
        return 0
    
    # Default values
    key_color = "white"
    key_intensity = "100" 
    special_color = "5"
    
    # Parse arguments
    if len(sys.argv) > 1:
        key_color = sys.argv[1]
    if len(sys.argv) > 2:
        key_intensity = sys.argv[2]
    if len(sys.argv) > 3:
        special_color = sys.argv[3]
    
    print(f"🎯 RGB Keyboard Control:")
    print(f"   Keys: {key_color} ({key_intensity}%)")
    print(f"   Special: {special_color}")
    print()
    
    # Connect and apply
    controller = RGBKeyboardController()
    
    if not controller.connect():
        print("❌ Failed to connect to keyboard!")
        print("   Check that SONiX keyboard is connected and recognized")
        return 1
    
    try:
        controller.apply_rgb_lighting(key_color, key_intensity, special_color)
        print("\n🎉 RGB setup complete!")
        
    except Exception as e:
        print(f"❌ Error: {e}")
        return 1
        
    finally:
        controller.disconnect()
    
    return 0

if __name__ == "__main__":
    sys.exit(main())
