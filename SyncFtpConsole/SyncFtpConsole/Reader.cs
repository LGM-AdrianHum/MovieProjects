using System;
using System.Threading;

namespace SyncFtpConsole
{
    public class Reader
    {
        private static readonly AutoResetEvent GetInput;
        private static readonly AutoResetEvent GotInput;
        private static string _input;

        static Reader()
        {
            GetInput = new AutoResetEvent(false);
            GotInput = new AutoResetEvent(false);
            var inputThread = new Thread(reader) {IsBackground = true};
            inputThread.Start();
        }

        // ReSharper disable once InconsistentNaming
        private static void reader()
        {
            while (true)
            {
                GetInput.WaitOne();
                _input = Console.ReadLine();
                GotInput.Set();
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public static string ReadLine(int timeOutMillisecs)
        {
            GetInput.Set();
            var success = GotInput.WaitOne(timeOutMillisecs);
            if (success)
                return _input;
            throw new TimeoutException("User did not provide input within the timelimit.");
        }
    }
}