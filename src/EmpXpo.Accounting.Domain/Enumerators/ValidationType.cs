
namespace EmpXpo.Accounting.Domain.Enumerators
{
    public class ValidatorType
    {
        private readonly string _name;
        public string Name => _name;

        public static readonly ValidatorType Create = new ValidatorType("Create");
        public static readonly ValidatorType Update = new ValidatorType("Update");
        public static readonly ValidatorType Delete = new ValidatorType("Delete");
        public static readonly ValidatorType Get = new ValidatorType("Get");

        public ValidatorType(string name)
        {
            _name = name;
        }

    }
}
