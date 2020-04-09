namespace Messages
{
    public enum EstimationStates
    {
        New = 0,
        AllImagesDownloaded,
        AllImagesProcessed,
        EstimationReady,
        EstimationError,
        ClientNotified,
        NotificationError
    }
}