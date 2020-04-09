using System;
using System.Collections.Generic;

namespace Acheve.Common.Messages 
{
    public class AllImagesProcessed
    {
        public Guid CaseNumber { get; set; }

        public ICollection<ImageMetadata> Metadata { get; set; }
    }

    public class ImageMetadata
    {
        public int ImageId { get; set; }

        public string Metadata { get; set; }
    }
}