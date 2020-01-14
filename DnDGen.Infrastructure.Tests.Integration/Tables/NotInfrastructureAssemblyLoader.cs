using DnDGen.Infrastructure.Other;
using DnDGen.Infrastructure.Tables;
using System.Reflection;

namespace DnDGen.Infrastructure.Tests.Integration.Tables
{
    internal class NotInfrastructureAssemblyLoader : AssemblyLoader
    {
        public Assembly GetRunningAssembly()
        {
            var type = typeof(OtherCollectionsCaller);
            return type.Assembly;
        }
    }
}
