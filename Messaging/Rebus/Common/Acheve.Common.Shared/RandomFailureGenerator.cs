using System;
using Microsoft.Extensions.Logging;

namespace Acheve.Common.Shared
{
    public static class RandomFailureGenerator
    {
        public static void RandomFail(double failThreshold, string url, ILogger logger)
        {
            var random = new Random();
            var value = random.NextDouble();
            var overThreshold = value >= failThreshold;

            logger.LogInformation(
                "Random failure value is {randomValue} and threshold is {failureThreshold} so value is over threshold: {overThreshold}", 
                value.ToString("F"), 
                failThreshold.ToString("F"), 
                overThreshold);

            if (overThreshold)
            {
                var ex = new InvalidOperationException("Bad luck, just a random fail :(");

                logger.LogError("Ohh an error happened processing the request {url}", url);

                throw ex;
            }
        }
    }
}