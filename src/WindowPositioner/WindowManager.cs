using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WindowPositioner.Settings;

namespace WindowPositioner
{
    internal static class WindowManager
    {
        public static Screen SelectedScreen => Screen.AllScreens[GlobalSettings.Instance.ScreenIndex];

        public static int ScreenWidth => SelectedScreen.WorkingArea.Width;

        public static int ScreenHeight => SelectedScreen.WorkingArea.Height;

        public static bool FixedSizes { get; set; }

        private static IEnumerable<Process> GameProcesses
            => GlobalSettings.Instance.ProcessNames.Distinct().SelectMany(Process.GetProcessesByName);

        public static IEnumerable<ManagedWindow> CalculateMatrix()
        {
            var ret = new List<ManagedWindow>();

            var procs = GameProcesses.ToList();
            var dims = Math.Ceiling(Math.Sqrt(procs.Count));

            Console.WriteLine("Screen Resolution {0}x{1} - {2} running processes.", ScreenWidth, ScreenHeight,
                procs.Count);
            Console.WriteLine("Creating matrix of {0} by {1}.", dims, dims);

            var windowWidth = FixedSizes ? 100 : ScreenWidth/dims;
            var windowHeight = FixedSizes ? 100 : ScreenHeight/dims;

            Console.WriteLine("Each game window will be {0} in width and {1} in height.", windowWidth, windowHeight);

            var cur = 0;
            var row = 0;
            var column = 0;

            foreach (var p in procs)
            {
                if (cur.IsMultipleOf((int) dims))
                    row++;

                if (column >= dims)
                    column = 0;

                ret.Add(new ManagedWindow
                {
                    Process = p,
                    Width = (int) windowWidth,
                    Height = (int) windowHeight,
                    X = (int) windowWidth*row + SelectedScreen.WorkingArea.X,
                    Y = (int) windowHeight*column
                });

                cur++;
                column++;
            }

            return ret;
        }

        public static void RepositionWindows(IEnumerable<ManagedWindow> windows)
        {
            foreach (var w in windows)
            {
                MoveWindow(w.Process.MainWindowHandle, w.X, w.Y, w.Height, w.Width);
                Thread.Sleep(100);
            }
        }

        private static bool IsMultipleOf(this int left, int right)
        {
            // Theoretically these are multiples of, but we don't count them as such in our specific case.
            if (left == 0 || right == 0)
                return false;

            var isMultiple = left%right == 0;
            return isMultiple;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
            uint uFlags);

        private static void MoveWindow(IntPtr hWnd, int x, int y, int height, int width)
        {
            const short SWP_NOZORDER = 0X4;
            const int SWP_SHOWWINDOW = 0x0040;

            SetWindowPos(hWnd, IntPtr.Zero, x, y, width, height, SWP_NOZORDER | SWP_SHOWWINDOW);
        }
    }

    public struct ManagedWindow
    {
        public Process Process;
        public int Width;
        public int Height;
        public int X;
        public int Y;
    }
}