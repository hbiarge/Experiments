using System;
using System.Net.Http;
using System.Threading.Tasks;
using Acheve.Common.Messages;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;

namespace Acheve.Application.ImageDownloads.Handlers
{
    public class ImageUrlReceivedHandler : IHandleMessages<ImageUrlReceived>
    {
        private readonly IBus _bus;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ImageUrlReceivedHandler> _logger;

        public ImageUrlReceivedHandler(IBus bus, IHttpClientFactory httpClientFactory, ILogger<ImageUrlReceivedHandler> logger)
        {
            _bus = bus;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Handle(ImageUrlReceived message)
        {
            _logger.LogInformation(
                "New request to download image: case number {caseNumber}. ImageId: {imageId}, ImageUrl {url}",
                message.CaseNumber,
                message.ImageId,
                message.ImageUrl);

            var client = _httpClientFactory.CreateClient("images");

            try
            {
                var stream = await client.GetStreamAsync(message.ImageUrl);

                _logger.LogInformation(
                    "Download successfully for image {imageId} for case number {caseNumber}.",
                    message.ImageId,
                    message.CaseNumber);

                var attachment = await _bus.Advanced.DataBus.CreateAttachment(stream);

                await _bus.Send(new ImageDownloaded
                {
                    CaseNumber = message.CaseNumber,
                    ImageId = message.ImageId,
                    ImageTicket = attachment.Id
                });
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    "Download error for image {imageId} for case number {caseNumber}. {downloadError}",
                    message.ImageId,
                    message.CaseNumber,
                    e.Message);

                await _bus.Send(new UnableToDownloadImage
                {
                    CaseNumber = message.CaseNumber,
                    ImageId = message.ImageId,
                    Error = e.Message
                });
            }
        }
    }
}
