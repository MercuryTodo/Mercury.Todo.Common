namespace Common.Consumers
{
    public interface IConsumersConfiguration
    {
        bool? IsMemoryInstance { get; }
        string ServiceQueue { get; }
    }
}