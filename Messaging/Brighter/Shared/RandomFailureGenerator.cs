using System;
using Microsoft.Extensions.Logging;

namespace Shared
{
    public static class RandomFailureGenerator
    {
        public static void RandomFail(ILogger logger)
        {
            var random = new Random();

            if (random.NextDouble() >= 0.5)
            {
                var ex = new InvalidOperationException("Bad luck, just a random fail :(");

                logger.LogError("Ohh an error happened: {error}", ex);

                throw ex;
            }
        }
    }
}