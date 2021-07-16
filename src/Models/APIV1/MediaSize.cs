namespace BluebirdPS.Models.APIV1
{
    public class MediaSize : TwitterObject
    {
        public string Size { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Resize { get; set; }

        public MediaSize(dynamic input)
        {
            Size = input;
            Width = input.w;
            Height = input.h;
            Resize = Core.Helpers.ToTitleCase(input.resize);
        }
    }

}
