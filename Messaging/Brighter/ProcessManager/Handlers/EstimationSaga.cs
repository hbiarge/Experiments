﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using Messages.Events;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;

namespace ProcessManager.Handlers
{
    public class EstimationSaga :
        Saga<EstimationState>,
        IAmInitiatedBy<EstimationRequestedEvent>,
        IHandleMessages<ImageDownloaded>,
        IHandleMessages<UnableToDownloadImage>,
        IHandleMessages<ImageProcessedEvent>,
        IHandleMessages<UnableToProcessImage>,
        IHandleMessages<EstimationCompletedEvent>,
        IHandleMessages<UnableToEstimate>,
        IHandleMessages<NotificationCompleted>,
        IHandleMessages<UnableToNotify>
    {
        private readonly IBus _bus;
        private readonly ILogger<EstimationSaga> _logger;

        public EstimationSaga(IBus bus, ILogger<EstimationSaga> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        protected override void CorrelateMessages(ICorrelationConfig<EstimationState> config)
        {
            // events of interest
            config.Correlate<EstimationRequestedEvent>(m => m.CaseNumber, d => d.CaseNumber);
            config.Correlate<ImageDownloaded>(m => m.CaseNumber, d => d.CaseNumber);
            config.Correlate<UnableToDownloadImage>(m => m.CaseNumber, d => d.CaseNumber);
            config.Correlate<ImageProcessedEvent>(m => m.CaseNumber, d => d.CaseNumber);
            config.Correlate<UnableToProcessImage>(m => m.CaseNumber, d => d.CaseNumber);
            config.Correlate<EstimationCompletedEvent>(m => m.CaseNumber, d => d.CaseNumber);
            config.Correlate<UnableToEstimate>(m => m.CaseNumber, d => d.CaseNumber);
            config.Correlate<NotificationCompleted>(m => m.CaseNumber, d => d.CaseNumber);
            config.Correlate<UnableToNotify>(m => m.CaseNumber, d => d.CaseNumber);
        }

        public async Task Handle(EstimationRequestedEvent message)
        {
            _logger.LogInformation(
                "Receiving new case number {caseNumber} with {imageNumber} image(s)",
                message.CaseNumber,
                message.ImageUrls.Count);

            Data.CaseNumber = message.CaseNumber;
            Data.ClientId = message.ClientId;
            Data.CallbackUrl = message.CallbackUri;
            Data.State = EstimationStates.New;
            Data.Images = message.ImageUrls.Select((url, index) => new CaseImage
            {
                Id = index,
                Url = url
            }).ToArray();

            await _bus.Send(new EstimationStateChanged
            {
                CaseNumber = message.CaseNumber,
                State = Data.State
            });

            foreach (var image in Data.Images)
            {
                await _bus.Send(new ImageUrlReceived
                {
                    CaseNumber = message.CaseNumber,
                    ImageId = image.Id,
                    ImageUrl = image.Url
                });
            }
        }

        public async Task Handle(ImageDownloaded message)
        {
            _logger.LogInformation(
                "Receiving downloaded image for case number {caseNumber}. ImageId: {id}",
                message.CaseNumber,
                message.ImageId);

            var currentImage = Data.Images.Single(x => x.Id == message.ImageId);
            currentImage.ImageTicket = message.ImageTicket;

            await _bus.Send(new ImageReady
            {
                CaseNumber = Data.CaseNumber,
                ImageId = currentImage.Id,
                ImageTicket = currentImage.ImageTicket
            });

            await VerifyIfAllImagesDownloaded();
        }

        public async Task Handle(UnableToDownloadImage message)
        {
            _logger.LogWarning(
                "Unable to download image for case number {caseNumber}. ImageId: {id}",
                message.CaseNumber,
                message.ImageId);

            var currentImage = Data.Images.Single(x => x.Id == message.ImageId);

            // At this point we have tried at least 3 times to download
            // the image with retries at the httpClient level
            // We can also reschedule the message to be processed later
            //await _bus.Defer(TimeSpan.FromSeconds(30), new ImageUrlReceived
            //{
            //    CaseNumber = message.CaseNumber,
            //    ImageId = message.ImageId,
            //    ImageUrl = currentImage.Url
            //});

            currentImage.DownloadError = message.Error;

            await VerifyIfAllImagesDownloaded();

            // We should verify also if all images are processed
            // because in case we can not download the last image
            // all images are processed and we can proceed to estimate
            await VerifyIfAllImagesProcessed();
        }

        private async Task VerifyIfAllImagesDownloaded()
        {
            var allImagesDownloaded = Data.Images.All(x => x.Downloaded);

            if (allImagesDownloaded)
            {
                Data.State = EstimationStates.AllImagesDownloaded;

                await _bus.Send(new EstimationStateChanged
                {
                    CaseNumber = Data.CaseNumber,
                    State = Data.State
                });
            }
        }

        public async Task Handle(ImageProcessedEvent message)
        {
            _logger.LogInformation(
                "Receiving processed image for case number {caseNumber}. ImageId: {id}",
                message.CaseNumber,
                message.ImageId);

            var currentImage = Data.Images.Single(x => x.Id == message.ImageId);
            currentImage.MetadataTicket = message.MetadataTicket;

            await VerifyIfAllImagesProcessed();
        }

        public async Task Handle(UnableToProcessImage message)
        {
            _logger.LogWarning(
                "Unable to receive processed image for case number {caseNumber}. ImageId: {id}",
                message.CaseNumber,
                message.ImageId);

            var currentImage = Data.Images.Single(x => x.Id == message.ImageId);
            currentImage.MetadataError = message.Error;

            await VerifyIfAllImagesProcessed();
        }

        private async Task VerifyIfAllImagesProcessed()
        {
            var allImagesProcessed = Data.Images.All(x => x.Processed);

            if (allImagesProcessed)
            {
                Data.State = EstimationStates.AllImagesProcessed;

                await _bus.Send(new EstimationStateChanged
                {
                    CaseNumber = Data.CaseNumber,
                    State = Data.State
                });

                // Do we have at least one image processed with metadata?
                // If so, send the allImagesProcessed to get an external estimation
                // If not, just send a notification error
                var atLeastOneImageProcessed = Data.Images.Any(x => string.IsNullOrEmpty(x.MetadataTicket) == false);

                if (atLeastOneImageProcessed)
                {
                    await _bus.Send(new AllImagesProcessed
                    {
                        CaseNumber = Data.CaseNumber,
                        Metadata = Data.Images.Select(image => new ImageMetadata
                        {
                            ImageId = image.Id,
                            Metadata = image.MetadataTicket
                        }).ToArray()
                    });
                }
                else
                {
                    Data.State = EstimationStates.EstimationError;

                    await _bus.Send(new EstimationStateChanged
                    {
                        CaseNumber = Data.CaseNumber,
                        State = Data.State
                    });

                    await _bus.Send(new EstimationError
                    {
                        CaseNumber = Data.CaseNumber,
                        CallbackUri = Data.CallbackUrl,
                        Reason = "We can not download any provided images"
                    });
                }
            }
        }

        public async Task Handle(EstimationCompletedEvent message)
        {
            _logger.LogInformation(
                "Receiving estimation complete for case number {caseNumber}.",
                message.CaseNumber);

            Data.EstimationTicket = message.EstimationTicket;
            Data.State = EstimationStates.EstimationReady;

            await _bus.Send(new EstimationStateChanged
            {
                CaseNumber = Data.CaseNumber,
                State = Data.State
            });

            await _bus.Send(new EstimationReady
            {
                CaseNumber = Data.CaseNumber,
                CallbackUri = Data.CallbackUrl,
                EstimationId = Data.EstimationTicket
            });
        }

        public async Task Handle(UnableToEstimate message)
        {
            _logger.LogWarning(
                "Unable to receive estimation for case number {caseNumber}.",
                message.CaseNumber);

            Data.EstimationError = message.Error;
            Data.State = EstimationStates.EstimationError;

            await _bus.Send(new EstimationStateChanged
            {
                CaseNumber = Data.CaseNumber,
                State = Data.State
            });

            await _bus.Send(new EstimationError
            {
                CaseNumber = Data.CaseNumber,
                CallbackUri = Data.CallbackUrl,
                Reason = message.Error
            });
        }

        public async Task Handle(NotificationCompleted message)
        {
            _logger.LogInformation(
                "Callback notification complete for case number {caseNumber}.",
                message.CaseNumber);

            Data.State = EstimationStates.ClientNotified;

            await _bus.Send(new EstimationStateChanged
            {
                CaseNumber = Data.CaseNumber,
                State = Data.State
            });

            MarkAsComplete();
            await CleanupDataBus();
        }

        public async Task Handle(UnableToNotify message)
        {
            _logger.LogWarning(
                "Callback notification error for case number {caseNumber}.",
                message.CaseNumber);

            Data.State = EstimationStates.NotificationError;

            await _bus.Send(new EstimationStateChanged
            {
                CaseNumber = Data.CaseNumber,
                State = Data.State
            });

            MarkAsComplete();
            await CleanupDataBus();
        }

        private async Task CleanupDataBus()
        {
            var tickets = new List<string>();

            if (string.IsNullOrEmpty(Data.EstimationTicket) == false)
            {
                tickets.Add(Data.EstimationTicket);
            }

            var imageTickets = Data.Images
                .SelectMany(x => new[] { x.MetadataTicket, x.ImageTicket })
                .Where(x => string.IsNullOrEmpty(x) == false);

            tickets.AddRange(imageTickets);

            foreach (var ticket in tickets)
            {
                await _bus.Advanced.DataBus.Delete(ticket);
            }
        }
    }
}