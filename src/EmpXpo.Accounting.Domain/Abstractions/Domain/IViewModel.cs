
using EmpXpo.Accounting.Domain.Enumerators;

namespace EmpXpo.Accounting.Domain.Abstractions.Domain
{
    public interface IViewModel : IEntity
    {
        public bool IsValid(ValidatorType validator);
        public bool HasErrors();
        public List<string> Errors();
    }
}
