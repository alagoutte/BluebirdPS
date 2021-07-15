using System;
using BluebirdPS.Models.APIV2.Metrics.Media;

namespace BluebirdPS.Models.APIV2
{
    public enum MediaType
    {
        AnimatedGif,
        Photo,
        Video
    }

    public class Media : TwitterObject
    {
        public string MediaKey { get; set; }
        public MediaType Type { get; set; }
        public int? Duration { get; set; }
        public long Height { get; set; }
        public Public PublicMetrics { get; set; }
        public NonPublic NonPublicMetrics { get; set; }
        public Organic OrganicMetrics { get; set; }
        public Promoted PromotedMetrics { get; set; }
        public long Width { get; set; }
        public Uri PreviewImageUrl { get; set; }

        public Media() { }
        public Media(dynamic input)
        {
            try
            {
                OriginalObject = input;

                Enum.TryParse(input.type, true, out MediaType mediaType);

                MediaKey = input.media_key;
                Type = mediaType;
                Height = input.height;
                Width = input.width;
                if (Core.Helpers.HasProperty(input, "duration_ms"))
                {
                    Duration = input.duration_ms;
                }
                if (Core.Helpers.HasProperty(input, "preview_image_url"))
                {
                    PreviewImageUrl = new Uri(input.preview_image_url);
                }

                if (Core.Helpers.HasProperty(input, "non_public_metrics"))
                {
                    NonPublicMetrics = new NonPublic(input.non_public_metrics);
                }
                if (Core.Helpers.HasProperty(input, "organic_metrics"))
                {
                    OrganicMetrics = new Organic(input.organic_metrics);
                }
                if (Core.Helpers.HasProperty(input, "promoted_metrics"))
                {
                    PromotedMetrics = new Promoted(input.promoted_metrics);
                }
            }
            catch
            {
                // any missing properties that are not on the input object
                // and not caught with Helpers.HasProperty if statement,
                // do not fail (for now)
            }
        }

    }

}
