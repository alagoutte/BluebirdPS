namespace BluebirdPS.Models.APIV2.TweetInfo.Context
{
    public class ContextAnnotation : TwitterObject
    {
        public Domain Domain { get; set; }
        public Entity Entity { get; set; }

        ContextAnnotation() { }
        ContextAnnotation(dynamic input)
        {
            OriginalObject = input;
            if (Core.Helpers.HasProperty(input, "domain"))
            {
                Domain = new Domain(input.domain);
            }
            if (Core.Helpers.HasProperty(input, "entity"))
            {
                Entity = new Entity(input.entity);
            }
        }
    }

    public class Domain : TwitterObject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Domain() { }
        public Domain(dynamic input)
        {
            Id = input.id;
            Name = input.name;
            Description = input.description;
            OriginalObject = input;
            OriginalObject = input;
        }
    }

    public class Entity : TwitterObject
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Entity() { }
        public Entity(dynamic input)
        {
            Id = input.id;
            Name = input.name;
            OriginalObject = input;
        }
    }
}
