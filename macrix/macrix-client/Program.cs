using macrix_client.Controllers;
using macrix_client.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;

namespace macrix_client
{
    class Program
    {
        //this is needed to resize the terminal window in Windows 11, necessary to avoid manually changing the host to Windows Console Host in Windows Developer Settings
        //this is also not needed in Widows 10 and prior versions
        //thanks to Node defender - https://stackoverflow.com/questions/77482431/c-sharp-cannot-maximize-console-from-within-the-code
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);
        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);
        private const int SW_RESTORE = 9;

        static void Main(string[] args)
        {
            // More Windows 11 terminal stuff
            // Constants for the ShowWindow function
            const int SW_MAXIMIZE = 3;
            // Get the handle of the console window
            IntPtr consoleWindowHandle = GetForegroundWindow();
            // Restore the window if it's minimized
            ShowWindow(consoleWindowHandle, SW_RESTORE);
            // Maximize the console window
            ShowWindow(consoleWindowHandle, SW_MAXIMIZE);
            // Get the screen size
            Rect screenRect;
            GetWindowRect(consoleWindowHandle, out screenRect);
            // Resize and reposition the console window to fill the screen
            var width = screenRect.Right - screenRect.Left;
            var height = screenRect.Bottom - screenRect.Top;
            MoveWindow(consoleWindowHandle, (int)screenRect.Left, (int)screenRect.Top, (int)width, (int)height, true);

            var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddHttpClient()
            .AddSingleton<IActionsService, ActionsService>()
            .AddSingleton<IMacrixAPIService, MacrixAPIService>()
            .BuildServiceProvider();

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogDebug("Starting application");
            MainController Program = ActivatorUtilities.CreateInstance<MainController>(serviceProvider);
            Program.Start();
        }

    }
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

}
