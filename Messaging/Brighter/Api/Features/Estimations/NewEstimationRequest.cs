using System.ComponentModel.DataAnnotations;

namespace Api.Features.Estimations
{
    public class NewEstimationRequest
    {
        [Required]
        [Url]
        public string CallBackUri { get; set; }

        [Required]
        [MaxLength(10)]
        public string[] ImageUrls { get; set; }
    }
}