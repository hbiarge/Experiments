namespace Acheve.Common.Shared
{
    public static class Constants
    {
        public const string ApplicationInsightsInstrumentationKey = "AI instrumentation key";

        public static class Azure
        {
            public static class ServiceBus
            {
                // RootManageSharedAccessKey for the demo purposes. Then you can tell rebus to use access policies without management permissions
                // so Rebus does not try to create the queues
                public const string ConnectionString = "ServiceBus connection string";
            }

            public static class Storage
            {
                public const string ConnectionString = "Storage connection string";
                public const string DataBusContainer = "databus";
            }
        }

        public static class Services
        {
            public const string Api = "Api";
            public const string ProcessManagerService = "ProcessManagerService";
            public const string ImageDownloadService = "ImageDownloadService";
            public const string ImageProcessService = "ImageProcessService";
            public const string EstimationService = "EstimationService";
            public const string NotificationService = "NotificationService";
            public const string StateHolder = "StateHolder";

            public static class External
            {
                public const string Image = "ExternalImages";
                public const string ImageProcess = "ExternalImageProcess";
                public const string Estimations = "ExternalEstimations";
                public const string Notification = "ExternalNotifications";
            }
        }

        public static class Queues
        {
            public const string ProcessManager = "processmanager";
            public const string ImageDownloads = "imagedownloads";
            public const string ImageProcess = "imageprocess";
            public const string ExternalEstimations = "externalestimations";
            public const string CallbackNotifications = "callbacknotifications";
            public const string StateHolder = "stateholder";
        }

        public static class FileSystem
        {
            public const string SagasPath = "rebus/sagas";
        }

        public static class FailureThresholds
        {
            // Services with thresholds near 0 are flaky and have more probabilities to throw exceptions
            // Services with thresholds near 1 are stable and have less probabilities to throw exceptions

            public const double ApplicationReceivingExternalImages = 1;
            public const double ApplicationReceivingExternalEstimations = 0;

            public const double ExternalImagesProcessing = 1;
            public const double ExternalEstimationsProcessing = 1;
            public const double ExternalNotificationsProcessing = 1;
        }
    }
}
