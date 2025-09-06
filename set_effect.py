# Jednoduchý ovladač efektů
from rgb_effects import SonixKeyboard, Effect, VolumeColor
import time

def main():
    kb = SonixKeyboard()
    
    try:
        if not kb.connect():
            return
        
        # Nastavení červené barvy pro hlavní podsvícení        
        kb.set_effect(Effect.SOLID, 255, 255,255, 0.5)
        #time.sleep(1)
        
        #print("Nastavuji první barvu volume tlačítka")
        #kb.set_volume_color(VolumeColor.COLOR_1)
        #time.sleep(2)
        
    except Exception as e:
        print(f"Chyba: {e}")
    finally:
        kb.close()

if __name__ == "__main__":
    main()
