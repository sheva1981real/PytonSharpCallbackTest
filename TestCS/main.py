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

def callback(a, b, c):
    print("CALLBACK", a, b, c)

if __name__ == "__main__":
    obj = SharpPythonCallbackTester()
    a = obj.call_me_from_py()
    print("a: ", a)
    obj.set_callback(Action[int, int, int](callback))
try:
    while True:
        time.sleep(20)
except KeyboardInterrupt:
    pass
