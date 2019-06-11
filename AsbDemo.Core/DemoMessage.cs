namespace AsbDemo.Core
{
    public interface IDemoMessage
    {
        string Id { get; set; }
        string Value { get; set; }
    }

    public class DemoMessage : IDemoMessage
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}
