using System;
using Microsoft.Extensions.Logging;

namespace GenericHost.Services
{
    public sealed class ScopedDependency : IScopedDependency, IDisposable
    {
        private readonly ILogger<ScopedDependency> _logger;

        public ScopedDependency(ILogger<ScopedDependency> logger)
        {
            _logger = logger;
        }

        public int GetId()
        {
            return GetHashCode();
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing ScopedDependency instance {id}", GetHashCode());
        }
    }
}