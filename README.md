# PytonSharpTest

Тестовий проєкт для демонстрації вирішення задачі виклику простого callback'у з DLL на С# до основного модуля на Python, за умови використання основного модуля у вигляді .exe, збудованного pyinstaller'ом.

##  Спочатку

pip install pythonnet

## Потім

### Зі сторони C#

 - Відкрити солюшн SharpPythonLib\SharpPythonLib.sln
 - У диспечері конфігурацій додати "x64" та видалити "Any CPU"
 - Зібрати "Release | x64"
 
### Зі сторони Python 

Запустити 

TestCS\build_exe.cmd 

 - щоб зібрати .exe'шник та скопіювати до dist\ файли бібліотеки (шоб не ускладнювати життя тим, у кого нема VisualStidio - файли додані у репозиторій у SharpPythonLib\SharpPythonLib\bin\x64\Release\net6.0\)
 

## Щоб запускати main.py з IDE
 
 файли бібліотеки
 
 - SharpPythonLib.deps.json 
 - SharpPythonLib.dll 
 
 мають лежати рядом у одному каталозі з TestCS\main.py (просто скопіюйте ручками)