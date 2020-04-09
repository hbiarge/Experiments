using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Messages;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.DataBus;
using Rebus.Handlers;
using Shared;

namespace ExternalImageProcessor.Handlers
{
    public class ImageReadyHandler : IHandleMessages<ImageReady>
    {
        private readonly IBus _bus;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ImageReadyHandler> _logger;

        public ImageReadyHandler(IBus bus, IHttpClientFactory httpClientFactory, ILogger<ImageReadyHandler> logger)
        {
            _bus = bus;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Handle(ImageReady message)
        {
            _logger.LogInformation(
                "Receiving image to process for case number {caseNumber}. ImageId: {id}, ImageTicket {ticket}",
                message.CaseNumber,
                message.ImageId,
                message.ImageTicket);

            var client = _httpClientFactory.CreateClient("process");

            try
            {
                var content = new MultipartFormDataContent
                {
                    {new StringContent(message.CaseNumber.ToString("D")), "CaseNumber"},
                    {new StringContent(message.ImageId.ToString("G")), "ImageId"},
                    {new StringContent($"{Constants.KnownUris.ApiBaseUri}/ExternalImageProcess/{message.CaseNumber:D}/images/{message.ImageId:G}"), "CallbackUrl"},
                    {new StreamContent(await DataBusAttachment.OpenRead(message.ImageTicket)), "image"}
                };

                await client.PostAsync(
                    new Uri($"{Constants.KnownUris.ImageProcessBaseUri}/ImageProcess"),
                    content);

                _logger.LogInformation(
                    "Image {id} for case number {caseNumber} sent to process successfully.",
                    message.ImageId,
                    message.CaseNumber);
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    "Image {id} for case number {caseNumber} process error. {imageProcessError}",
                    message.CaseNumber,
                    message.ImageId,
                    e.Message);

                await _bus.Send(new UnableToProcessImage
                {
                    CaseNumber = message.CaseNumber,
                    ImageId = message.ImageId,
                    Error = e.Message
                });
            }
        }
    }
}
