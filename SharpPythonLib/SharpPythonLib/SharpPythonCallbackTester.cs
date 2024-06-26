﻿using System.Runtime.InteropServices;
using System.Threading;

using SharpDX.DirectInput;

namespace SharpPythonLib
{
    public class SharpPythonCallbackTester {

        private DirectInput directInput;
        private Joystick? joystick;

        private int callCount;
        private Delegate? callback;

        private const int JOY_VALUE_HI = 65535;
        private const int JOY_VALUE_LOW = 0;
        private const int JOY_VALUE_POV_NONE = -1;
        private const int JOY_VALUE_MIDDLE = 32767;
        private const int JOY_ON = 128;
        private const int JOY_OFF = 0;
        private const int JOY_XY_TRESHOLD = 100;

        // ГН (лево 0, середина 32767, право 65535)
        private int yawPrev = JOY_VALUE_MIDDLE;
        private int yawNew = JOY_VALUE_MIDDLE;
        // ВН (верх [от себя] 0, середина 32767, низ [на себя] 65535)
        private int pitchPrev = JOY_VALUE_MIDDLE;
        private int pitchNew = JOY_VALUE_MIDDLE;
        //Спуск (вЫкл:0, ВКЛ:128)
        private int buttons0Prev = JOY_OFF;
        private int buttons0New = JOY_OFF;

        // Маленький джойстик вверху (по часовой 12:0 3:9000 6:18000 9:27000 12:35999; ifu 4500)
        private int povPrev = JOY_VALUE_POV_NONE;
        private int povNew = JOY_VALUE_POV_NONE;

        // Черная Кнопка слева (вЫкл:0, ВКЛ:128)
        private int buttons2Prev = JOY_OFF;
        private int buttons2New = JOY_OFF;
        // Красная Кнопка справа (вЫкл:0, ВКЛ:128)
        private int buttons3Prev = JOY_OFF;
        private int buttons3New = JOY_OFF;

        // Слайдер (верх [от себя]:0 низ [на себя]:65535)
        private int sliders0Prev = JOY_VALUE_LOW;
        private int sliders0New = JOY_VALUE_LOW;

        void ProcessSingleState(SharpDX.DirectInput.JoystickUpdate state)
        {
            switch (state.Offset)
            {
                case JoystickOffset.X:
                    yawNew = (int)state.Value;
                    break;

                case JoystickOffset.Y:
                    pitchNew = (int)state.Value;
                    break;

                case JoystickOffset.Buttons0:
                    buttons0New = (int)state.Value;
                    break;

                case JoystickOffset.Buttons2:
                    buttons2New = (int)state.Value;
                    break;

                case JoystickOffset.Buttons3:
                    buttons3New = (int)state.Value;
                    break;

                case JoystickOffset.PointOfViewControllers0:
                    povNew = (int)state.Value;
                    break;

                case JoystickOffset.Sliders0:
                    sliders0New = (int)state.Value;
                    break;

            }
        }

        int Abs(int val)
        {
            return (val >= 0) ? val : -(val);
        }
        void ProcessStates()
        {
            bool bYaw = (yawNew == 32676 && pitchNew == 32676) || (Abs(yawNew - yawPrev) > JOY_XY_TRESHOLD);            
            bool bPitch = Abs(pitchNew - pitchPrev) > JOY_XY_TRESHOLD;
            bool bFire = Abs(buttons0New - buttons0Prev) > 0;
            bool bBlackBnt = Abs(buttons2New - buttons2Prev) > 0;
            bool bRedBnt = Abs(buttons3New - buttons3Prev) > 0;
            bool bPov = Abs(povNew - povPrev) > 0;
            bool bSlider = Abs(sliders0New - sliders0Prev) > JOY_XY_TRESHOLD;

            if (bYaw || bPitch || bFire || bBlackBnt || bRedBnt || bPov || bSlider)
            {                
                yawPrev = yawNew;
                pitchPrev = pitchNew;
                buttons0Prev = buttons0New;
                buttons2Prev = buttons2New;
                buttons3Prev = buttons3New;
                povPrev = povNew;
                sliders0Prev = sliders0New;
                call_it_back();
            }
        }


        bool FindJoyStick()
        {
            directInput = new DirectInput();

            // Find a Joystick Guid
            var joystickGuid = Guid.Empty;

            // If Gamepad not found, look for a Joystick
            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;

            // If Joystick not found, throws an error
            if (joystickGuid == Guid.Empty)
            {
                return false;
            }

            joystick = new Joystick(directInput, joystickGuid);

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 128;

            // Acquire the joystick
            joystick.Acquire();

            Console.WriteLine("Found Joystick GUID: " + joystickGuid);
            Console.WriteLine("AxeCount: " + joystick.Capabilities.AxeCount);
            Console.WriteLine("ForceFeedbackMinimumTimeResolution: " + joystick.Capabilities.ForceFeedbackMinimumTimeResolution);
            Console.WriteLine("ForceFeedbackSamplePeriod: " + joystick.Capabilities.ForceFeedbackSamplePeriod);
            Console.WriteLine("ButtonCount: " + joystick.Capabilities.ButtonCount);
            Console.WriteLine("PovCount: " + joystick.Capabilities.PovCount);

            return true;
        }


        private void JoyStickLoop()
        {
            try
            {
                while (true && joystick != null)
                {
                    joystick.Poll();
                    var datas = joystick.GetBufferedData();
                    foreach (var state in datas)
                    {
                        ProcessSingleState(state);
                    }
                    ProcessStates();

                    Thread.Sleep(5);
                }
            }
            catch (SharpDX.SharpDXException edx)
            {
                if (edx.Descriptor.ApiCode == "InputLost")
                {
                    joystick = null;
                    Console.WriteLine(edx);
                }
            }
        }

        SharpPythonCallbackTester()
        {
            callback = null;
            callCount = 0;
            StartThread();
        }


        public void ThreadMethod()
        {
            while (true)
            {
                if (FindJoyStick())
                {
                    JoyStickLoop();
                }
                Thread.Sleep(50);
            }
        }
        private void StartThread()
        {
            Thread callbackThread = new Thread(
                        new ThreadStart(this.ThreadMethod));
            callbackThread.Start();
            Console.WriteLine("ThreadStarted");

        }

        private void call_it_back()
        {   if(callback != null)
            {
                object[] paramsToPass = new object[7];
                paramsToPass[0] = (int)yawNew;
                paramsToPass[1] = (int)pitchNew;
                paramsToPass[2] = (bool)(buttons0New != 0);
                paramsToPass[3] = (bool)(buttons2New != 0);
                paramsToPass[4] = (bool)(buttons3New != 0);
                paramsToPass[5] = (int)povNew;
                paramsToPass[6] = (int)sliders0New;
                callback.DynamicInvoke(paramsToPass);
                callCount++;
            }
        }
        public void set_callback(Delegate f)
        {
            callback = f;
            Console.WriteLine("callback setted");
        }

        public int call_me_from_py()
        {
            return (Int32)777;
        }
    }
}
