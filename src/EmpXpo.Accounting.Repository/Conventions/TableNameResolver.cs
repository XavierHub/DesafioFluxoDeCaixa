using Dommel;

namespace EmpXpo.Accounting.Repository.Conventions
{
    public class TableNameResolver : ITableNameResolver
    {
        public string ResolveTableName(Type type)
        {
            return $"{type.Name}";
        }
    }
}
