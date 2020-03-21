using System;
using Microsoft.Extensions.Options;

namespace Shared
{
    public class ServicesConfiguration
    {
        public HostInfo Api { get; set; }
        public HostInfo Images { get; set; }
        public HostInfo ImageProcess { get; set; }
        public HostInfo Estimations { get; set; }
        public HostInfo Notifications { get; set; }
        public HostInfo StateHolder { get; set; }

        public class HostInfo
        {
            public string Port { get; set; }
            
            public string Host { get; set; }
            
            public string BaseUrl => $"http://{Host}:{Port}";
            
            public static HostInfo Create(string url)
            {
                var parts = url.Split(':', StringSplitOptions.RemoveEmptyEntries);

                return new HostInfo
                {
                    Host = parts[0],
                    Port = parts[1]
                };
            }
        }
    }

    public class ServicesPostConfiguration : IPostConfigureOptions<ServicesConfiguration>
    {
        private const string Localhost = "localhost";

        public void PostConfigure(string name, ServicesConfiguration options)
        {
            if (options.Api is null)
            {
                options.Api = ServicesConfiguration.HostInfo.Create("localhost:5000");
            }

            if (options.Images is null)
            {
                options.Images = ServicesConfiguration.HostInfo.Create("localhost:5001");
            }

            if (options.ImageProcess is null)
            {
                options.ImageProcess = ServicesConfiguration.HostInfo.Create("localhost:5002");
            }

            if (options.Estimations is null)
            {
                options.Estimations = ServicesConfiguration.HostInfo.Create("localhost:5003");
            }

            if (options.Notifications is null)
            {
                options.Notifications = ServicesConfiguration.HostInfo.Create("localhost:5004");
            }

            if (options.StateHolder is null)
            {
                options.StateHolder = ServicesConfiguration.HostInfo.Create("localhost:5005");
            }
        }
    }
}