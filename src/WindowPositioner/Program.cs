using System;
using System.Linq;
using System.Threading.Tasks;
using WindowPositioner.Settings;

namespace WindowPositioner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Window Positioner";

            Console.WriteLine("Selected screen: " + WindowManager.SelectedScreen.DeviceName);
            Console.WriteLine(
                $"Screen is {WindowManager.ScreenWidth} in width and {WindowManager.ScreenHeight} in height.");

            if (!GlobalSettings.Instance.ProcessNames.Any())
            {
                Console.WriteLine("Could not find processes to automatically structure windows for. Make sure GlobalSettings is properly configured!");
                Console.ReadLine();
            }

            Console.WriteLine("Automatically repositioning windows for processes:");

            foreach (var s in GlobalSettings.Instance.ProcessNames)
                Console.WriteLine($"[+] {s}");

            Console.WriteLine("Every 10 seconds.");

            Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);

            Console.ReadLine();
        }

        private static async Task Run()
        {
            while (true)
            {
                var wnds = WindowManager.CalculateMatrix().ToList();

                var dupes = wnds.GroupBy(w => w.X + w.Y).Count(g => g.Count() > 1);

                if (dupes > 0)
                    Console.WriteLine("Calculated {0} duplicates.. this could mean overlap!", dupes);

                WindowManager.RepositionWindows(wnds);

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}