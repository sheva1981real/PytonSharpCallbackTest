import os
import clr
from System import Action
import time

lib_name = "SharpPythonLib"

curr_dir = os.getcwd()
ref_path = curr_dir + "\\" + lib_name

print("ref_path: ",ref_path, "\n")
clr.AddReference(ref_path)


from SharpPythonLib import SharpPythonCallbackTester

def joystick_callback(yaw :int, pitch :int, fire :bool, black_button :bool, red_button :bool, slowmo:int, slider :int):
    print("JOYSTICK: ", yaw, pitch, fire, black_button, red_button, slowmo, slider)
    
if __name__ == "__main__":
    obj = SharpPythonCallbackTester()
    a = obj.call_me_from_py()
    print("a: ", a)
    obj.set_callback(Action[int, int, bool, bool, bool, int, int](joystick_callback))
try:
    while True:
        time.sleep(20)
except KeyboardInterrupt:
    obj.Detouch()
    pass
