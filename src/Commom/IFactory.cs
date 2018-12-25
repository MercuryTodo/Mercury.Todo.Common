namespace Common
{
    public interface IFactory<T>
    {
        T Create();
    }
}