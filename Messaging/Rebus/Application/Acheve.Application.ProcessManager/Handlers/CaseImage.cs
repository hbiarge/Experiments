using System;

namespace Acheve.Application.ProcessManager.Handlers
{
    public class CaseImage
    {
        public const int MaxProcessedWaits = 5;
        public static readonly TimeSpan ProcessedWaitTime = TimeSpan.FromSeconds(20);

        public int Id { get; set; }

        public string Url { get; set; }

        public string ImageTicket { get; set; }
        
        public string DownloadError { get; set; }
        
        // An image is considered downloaded if
        // we have a ticket for the image (where the image is stored)
        // or if we couldn't download the image because an error 
        public bool Downloaded => string.IsNullOrEmpty(ImageTicket) == false 
                                  || string.IsNullOrEmpty(DownloadError) == false;

        public string MetadataTicket { get; set; }
        
        public string MetadataError { get; set; }

        public int ProcessedWaits { get; set; }

        // An image is considered processed if
        // we couldn't download the image because an error (so we didn't send the image to process)
        // we have a ticket for the metadata (where the metadata is stored)
        // or if we couldn't process the image because an error 
        public bool Processed => string.IsNullOrEmpty(DownloadError) == false
                                 || string.IsNullOrEmpty(MetadataTicket) == false
                                 || string.IsNullOrEmpty(MetadataError) == false;
    }
}