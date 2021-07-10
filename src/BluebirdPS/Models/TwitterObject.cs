namespace BluebirdPS.Models
{
    public class TwitterObject
    {
        internal object OriginalObject { get; set; }

        public object GetOriginalObject()
        {
            return OriginalObject;
        }
    }
}
