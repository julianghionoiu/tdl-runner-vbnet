using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BeFaster.Runner
{
    public class WindowsConsoleSupport
    {

        private const int STD_INPUT_HANDLE = -10;

        private const int STD_OUTPUT_HANDLE = -11;

        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;

        // ReSharper restore InconsistentNaming

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        public static void EnableColours()
        {
            try
            {
                GetLastError();
            }
            catch (Exception)
            {
                // Do nothing if no DLL present, at least we tried
                return;
            }

            var iStdIn = GetStdHandle(STD_INPUT_HANDLE);
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);

            if (!GetConsoleMode(iStdIn, out uint inConsoleMode))
            {
                Console.WriteLine("failed to get input console mode");
                return;
            }
            if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
            {
                Console.WriteLine("failed to get output console mode");
                return;
            }

            inConsoleMode |= ENABLE_VIRTUAL_TERMINAL_INPUT;
            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;

            if (!SetConsoleMode(iStdIn, inConsoleMode))
            {
                Console.WriteLine($"failed to set input console mode, error code: {GetLastError()}");
                return;
            }
            if (!SetConsoleMode(iStdOut, outConsoleMode))
            {
                Console.WriteLine($"failed to set output console mode, error code: {GetLastError()}");
                return;
            }
        }
    }
}
