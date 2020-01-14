namespace DnDGen.Infrastructure.Generators
{
    public interface JustInTimeFactory
    {
        T Build<T>();
        T Build<T>(string name);
    }
}
