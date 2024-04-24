pyinstaller.exe --onefile main.py
copy ..\SharpPythonLib\SharpPythonLib\bin\x64\Release\net6.0\SharpPythonLib.dll dist\SharpPythonLib.dll
 dist\SharpPythonLib.deps.json
copy ..\SharpPythonLib\SharpPythonLib\bin\x64\Release\net6.0\SharpDX.DirectInput.dll dist\SharpDX.DirectInput.dll
copy ..\SharpPythonLib\SharpPythonLib\bin\x64\Release\net6.0\SharpDX.dll dist\SharpDX.dll