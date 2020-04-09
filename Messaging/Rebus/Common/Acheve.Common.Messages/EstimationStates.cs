namespace Acheve.Common.Messages
{
    public enum EstimationStates
    {
        New = 0,
        AllImagesDownloaded,
        AllImagesProcessed,
        StuckWaitingForExternalImagesToBeProcessed,
        EstimationReady,
        StuckWaitingForExternalEstimation,
        EstimationError,
        ClientNotified,
        NotificationError
    }
}