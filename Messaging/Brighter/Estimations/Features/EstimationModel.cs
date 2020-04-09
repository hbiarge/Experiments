using System.Collections.Generic;

namespace Estimations.Features
{
    public class EstimationModel
    {
        public string CaseNumber { get; set; }

        public string CallbackUrl { get; set; }

        public ICollection<ImageMetadata> Type { get; set; }
    }

    public class ImageMetadata
    {
        public int ImageId { get; set; }

        public string Metadata { get; set; }
    }
}