using System.IO;

namespace DnDGen.Core.Tables
{
    internal interface StreamLoader
    {
        Stream LoadFor(string filename);
    }
}