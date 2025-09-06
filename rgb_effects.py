# Skript pro ovládání RGB podsvícení klávesnice SONiX s různými efekty
# Před použitím: pip install hidapi
import hid
import time
from typing import Tuple, List
from enum import Enum, auto

class Effect(Enum):
    """Dostupné světelné efekty"""
    SOLID = auto()    # Plná barva
    RAIN = auto()     # Efekt deště
    # Další efekty budou přidány podle zachycených paketů

class VolumeColor(Enum):
    """Přednastavené barvy pro volume tlačítko"""
    COLOR_1 = 1    # První přednastavená barva (01)
    COLOR_2 = 2    # Druhá přednastavená barva (02)
    COLOR_3 = 3    # Třetí přednastavená barva (25)

class SonixKeyboard:
    def __init__(self):
        self.vendor_id = 0x0c45
        self.product_id = 0x8508
        self.device = None
        self.target_usage = 146
        self.target_usage_page = 65308

    def connect(self):
        """Připojení ke správnému rozhraní klávesnice"""
        for device_dict in hid.enumerate(self.vendor_id, self.product_id):
            if (device_dict.get('usage') == self.target_usage and 
                device_dict.get('usage_page') == self.target_usage_page):
                try:
                    self.device = hid.device()
                    self.device.open_path(device_dict['path'])
                    print(f"Připojeno k zařízení:")
                    print(f"Výrobce: {self.device.get_manufacturer_string()}")
                    print(f"Produkt: {self.device.get_product_string()}")
                    return True
                except IOError as e:
                    print(f"Chyba při připojování ke konkrétnímu rozhraní: {e}")
                    continue
        
        print("Nenalezeno správné rozhraní klávesnice!")
        return False

    def _send_packet(self, data: bytes) -> bool:
        """Odešle paket do klávesnice a přečte odpověď"""
        try:
            result = self.device.write(data)
            if result == 64:
                # Přečti odpověď pro kontrolu
                response = self.device.read(64, timeout_ms=100)
                if response:
                    print(f"Odpověď: {bytes(response).hex()}")
                return True
            return False
        except IOError as e:
            print(f"Chyba při odesílání paketu: {e}")
            return False

    def set_effect_rain(self, r: int, g: int, b: int):
        """Nastaví efekt deště s danou barvou"""
        data = bytes([
            0x04, 0xce, 0x01, 0x06, 0x22, 0x00, 0x00, 0x00,
            0x00, 0x0f, 0x04, 0x01, b, r, g, 0x00,  # Opravené pořadí na BGR
            0x8a, 0x08] + [0x00] * 46)
        
        if self._send_packet(data):
            print(f"Efekt deště nastaven s barvou: #{r:02x}{g:02x}{b:02x}")
            return True
        return False

    def set_effect_solid(self, r: int, g: int, b: int, brightness: float = 1.0):
        """Nastaví plnou barvu podsvícení
        
        Args:
            r (int): Červená složka (0-255)
            g (int): Zelená složka (0-255)
            b (int): Modrá složka (0-255)
            brightness (float): Jas v rozsahu 0.0-1.0 (výchozí: 1.0 = 100%)
        """
        # Převeď jas na hodnoty pro paket
        brightness = max(0.0, min(1.0, brightness))  # Omez na rozsah 0-1
        
        # Přesné hodnoty podle zachycených paketů
        if brightness > 0.75:  # Super vysoký jas (>75%)
            brightness_byte1 = 0x11
            brightness_byte2 = 0x03
        elif brightness > 0.25:  # Normální jas (26-75%)
            brightness_byte1 = 0x10
            brightness_byte2 = 0x02
        else:  # Nízký jas (0-25%)
            brightness_byte1 = 0x0f
            brightness_byte2 = 0x01
            
        data = bytes([
            0x04, brightness_byte1, 0x02, 0x06, 0x22, 0x00, 0x00, 0x00,
            0x00, 0x06, brightness_byte2, 0x01, 0x00, 0x00, r, g, b, 0x00,
            0x00, 0x00] + [0x00] * 44)
        
        if self._send_packet(data):
            print(f"Stálá barva nastavena na: #{r:02x}{g:02x}{b:02x} (jas: {brightness*100:.0f}%)")
            return True
        return False

    def set_effect(self, effect: Effect, r: int, g: int, b: int, brightness: float = 1.0):
        """Nastaví požadovaný efekt s danou barvou a jasem
        
        Args:
            effect (Effect): Typ efektu (SOLID, RAIN)
            r (int): Červená složka (0-255)
            g (int): Zelená složka (0-255)
            b (int): Modrá složka (0-255)
            brightness (float): Jas v rozsahu 0.0-1.0 (výchozí: 1.0 = 100%)
        """
        if effect == Effect.RAIN:
            return self.set_effect_rain(r, g, b)  # Rain efekt zatím nepodporuje nastavení jasu
        elif effect == Effect.SOLID:
            return self.set_effect_solid(r, g, b, brightness)
        else:
            print(f"Neznámý efekt: {effect}")
            return False

    def demo_effects(self):
        """Předvede různé efekty"""
        colors = [
            (255, 0, 0),    # Červená
            (0, 255, 0),    # Zelená
            (0, 0, 255),    # Modrá
            (255, 255, 0),  # Žlutá
            (255, 0, 255),  # Purpurová
            (0, 255, 255),  # Azurová
            (255, 255, 255) # Bílá
        ]

        print("Demo efektu deště:")
        for r, g, b in colors:
            self.set_effect(Effect.RAIN, r, g, b)
            time.sleep(1)

    def set_volume_color(self, color: VolumeColor) -> bool:
        """Nastaví barvu volume tlačítka na jednu z přednastavených hodnot
        
        Args:
            color (VolumeColor): Přednastavená barva z výčtu VolumeColor
        
        Returns:
            bool: True pokud se nastavení povedlo
        """
        # Namapování hodnot pro třetí barvu
        value = 0x25 if color == VolumeColor.COLOR_3 else color.value
        
        # První packet - nastavení barvy
        data = bytes([
            0x1c, 0x00, 0x10, 0x40, 0xbf, 0xf2, 0x0a, 0x9a,
            0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x1b, 0x00,
            0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x02, 0x48,
            0x00, 0x00, 0x00, 0x00, 0x21, 0x09, 0x04, 0x02,
            0x01, 0x00, 0x40, 0x00, 0x04, value, 0x00, color.value if color != VolumeColor.COLOR_3 else 0x03,
            0x22, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])
        
        if self._send_packet(data):
            print(f"Volume tlačítko nastaveno na barvu {color.name}")
            return True
        return False
        
        if self._send_packet(data1) and self._send_packet(data2):
            print(f"Volume tlačítko nastaveno na barvu {color.name}")
            return True
        return False

    def close(self):
        """Uzavře spojení s klávesnicí"""
        if self.device:
            self.device.close()

def main():
    # Příklad použití
    kb = SonixKeyboard()
    
    try:
        if not kb.connect():
            return

        # Spusť demo
        kb.demo_effects()

    except Exception as e:
        print(f"Chyba: {e}")
    finally:
        kb.close()

if __name__ == "__main__":
    main()
