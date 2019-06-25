namespace AsbDemo.Core
{
    public interface IMessageBase
    {
        string Id { get; set; }
    }

    public interface IDemoMessage : IMessageBase
    {
        string Value { get; set; }
    }

    public interface IDemoMessage2 : IMessageBase
    {
        string Name { get; set; }
        string LastName { get; set; }
    }
}
