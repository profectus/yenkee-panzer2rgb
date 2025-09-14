#!/usr/bin/env python3
"""
SONiX Keyboard HID Communication Library

Low-level USB HID communication for SONiX keyboards (VID: 0x0c45, PID: 0x8508).
Provides direct packet sending capabilities for RGB lighting control.
"""

import hid

class SonixKeyboard:
    """SONiX USB Keyboard HID Interface"""
    
    def __init__(self):
        self.vendor_id = 0x0c45
        self.product_id = 0x8508
        self.device = None
        self.target_usage = 146
        self.target_usage_page = 65308

    def connect(self):
        """Connect to the correct keyboard interface"""
        for device_dict in hid.enumerate(self.vendor_id, self.product_id):
            if (device_dict.get('usage') == self.target_usage and 
                device_dict.get('usage_page') == self.target_usage_page):
                try:
                    self.device = hid.device()
                    self.device.open_path(device_dict['path'])
                    print(f"Connected to device:")
                    print(f"Manufacturer: {self.device.get_manufacturer_string()}")
                    print(f"Product: {self.device.get_product_string()}")
                    return True
                except IOError as e:
                    print(f"Error connecting to interface: {e}")
                    continue
        
        print("Correct keyboard interface not found!")
        return False

    def _send_packet(self, data: bytes) -> bool:
        """Send packet to keyboard and read response"""
        try:
            result = self.device.write(data)
            if result == 64:
                # Read response for verification
                response = self.device.read(64, timeout_ms=100)
                if response:
                    return bytes(response).hex()
                return True
            return False
        except IOError as e:
            print(f"Error sending packet: {e}")
            return False

    def disconnect(self):
        """Disconnect from keyboard"""
        if self.device:
            self.device.close()
            self.device = None
