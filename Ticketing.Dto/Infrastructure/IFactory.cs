namespace Ticketing.Dto.Infrastructure
{
    public interface IFactory<out T>
    {
        T Create();
    }
}
