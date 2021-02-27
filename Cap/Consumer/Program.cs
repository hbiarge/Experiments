using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddCap(options =>
                    {
                        options.UseRabbitMQ(setup =>
                        {
                            setup.HostName = "rabbit";
                            setup.Port = 5672;
                            setup.UserName = "guest";
                            setup.Password = "guest";
                        });

                        options.UseSqlServer(setup =>
                        {
                            setup.ConnectionString = @"Server=database;User Id=sa;Password=P@ssw0rd;Initial Catalog=cap.sample.consumer;MultipleActiveResultSets=true;";
                            setup.Schema = "consumer";
                        });
                    });

                    services.AddTransient<Handler>();
                    services.AddHostedService<Worker>();
                });
    }
}
