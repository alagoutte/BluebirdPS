using System;

namespace BluebirdPS.Models.APIV1
{
    public class Media : TwitterObject
    {
        public string Id { get; set; }
        public Uri MediaUrl { get; set; }
        public Uri Url { get; set; }
        public string DisplayUrl { get; set; }
        public Uri ExpandedUrl { get; set; }
        public string Type { get; set; }
        //public PSObject Sizes { get; set; }

        public Media(dynamic input)
        {
            try
            {
                Id = input.id_str;
                MediaUrl = new Uri(input.media_url_https);
                Url = new Uri(input.url);
                DisplayUrl = input.display_url;
                ExpandedUrl = new Uri(input.expanded_url);
                Type = Core.Helpers.ToTitleCase(input.type);

                //Sizes = input.sizes;
            }
            catch
            {

            }
        }

        public override string ToString()
        {
            return $"Id: {Id}, Type: {Type}, DisplayUrl: {DisplayUrl}";
        }
        //sizes
        //    large
        //    small
        //    medium
        //    thumb
        //        w
        //        h
        //        resize
    }

}
