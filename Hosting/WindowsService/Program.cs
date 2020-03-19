using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace WindowsService
{
    class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss zzz} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            var (isService, contentRoot) = GetExecutionMode(args);   

            var webHostArgs = args.Where(arg => arg != "--console").ToArray();

            var host = WebHost.CreateDefaultBuilder(webHostArgs)
                .UseContentRoot(contentRoot)
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddSerilog();
                })
                .UseStartup<Startup>()
                .Build();

            if (isService)
            {
                host.RunAsCustomService();
            }
            else
            {
                host.Run();
            }
        }

        private static (bool isService, string contentRoot) GetExecutionMode(string[] args)
        {
            var notIsService = Debugger.IsAttached || args.Contains("--console");

            if (notIsService)
            {
                return (false, Directory.GetCurrentDirectory());
            }

            var pathToExe = Process.GetCurrentProcess().MainModule?.FileName;
            return (true, Path.GetDirectoryName(pathToExe));
        }
    }
}
