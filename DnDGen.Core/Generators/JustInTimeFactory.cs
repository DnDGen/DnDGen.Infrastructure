namespace DnDGen.Core.Generators
{
    public interface JustInTimeFactory
    {
        T Build<T>();
        T Build<T>(string name);
    }
}
