using EmpXpo.Accounting.Domain.Enumerators;

namespace EmpXpo.Accounting.Domain
{
    public class CashFlow : IdentifierBase
    {
        public CashFlowType Type { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
