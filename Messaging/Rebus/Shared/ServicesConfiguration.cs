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
            public string Protocol { get; set; } = "http";

            public string Host { get; set; }

            public string Port { get; set; }

            public string BaseUrl => $"{Protocol}{Uri.SchemeDelimiter}{Host}:{Port}";

            public static HostInfo Create(string url)
            {
                var uri = new Uri(url);

                return new HostInfo
                {
                    Protocol = uri.Scheme,
                    Host = uri.Host,
                    Port = uri.Port.ToString("D")
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
                options.Api = ServicesConfiguration.HostInfo.Create("http://localhost:5000");
            }

            if (options.Images is null)
            {
                options.Images = ServicesConfiguration.HostInfo.Create("http://localhost:5001");
            }

            if (options.ImageProcess is null)
            {
                options.ImageProcess = ServicesConfiguration.HostInfo.Create("http://localhost:5002");
            }

            if (options.Estimations is null)
            {
                options.Estimations = ServicesConfiguration.HostInfo.Create("http://localhost:5003");
            }

            if (options.Notifications is null)
            {
                options.Notifications = ServicesConfiguration.HostInfo.Create("http://localhost:5004");
            }

            if (options.StateHolder is null)
            {
                options.StateHolder = ServicesConfiguration.HostInfo.Create("https://localhost:5005");
            }
        }
    }
}