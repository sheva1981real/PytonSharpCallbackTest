using System.Runtime.InteropServices;
using System.Threading;

namespace SharpPythonLib
{
    public class SharpPythonCallbackTester {

        private int v1;
        private int v2;
        private int v3;
        private int callCount;
        private Delegate? callback;

        SharpPythonCallbackTester()
        {
            callback  = null;
            callCount = 0;
            v1 = 999999000;
            v2 = 888888000;
            v3 = 777777000;
            setThred();
        }


        public void ThreadMethod()
        {
            Console.WriteLine("ThreadMethod");
            while (true)
            {
                call_it_back();
                Thread.Sleep(2000);
            }
        }
        private void setThred()
        {
            Thread callbackThread = new Thread(
                        new ThreadStart(this.ThreadMethod));
            callbackThread.Start();
            Console.WriteLine("ThreadStarted");

        }

        private void call_it_back()
        {   if(callback != null)
            {
                object[] paramsToPass = new object[3];
                paramsToPass[0] = (Int32)v1 + callCount;
                paramsToPass[1] = (Int32)v2 + callCount;
                paramsToPass[2] = (Int32)v3 + callCount;
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
