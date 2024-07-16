using EmpXpo.Accounting.Domain.Enumerators;
using FluentValidation;

namespace EmpXpo.Accounting.Domain.ViewModels
{
    public class CashFlowViewModel : ViewModelBase<CashFlowViewModel>
    {
        public int? Id { get; set; }
        public CashFlowType Type { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; } = string.Empty;

        public override bool IsValid(ValidatorType validatorType)
        {
            var validator = new InlineValidator<CashFlowViewModel>();

            if (validatorType == ValidatorType.Create)
            {
                validator.RuleFor(x => x.Id).Null();
                validator.RuleFor(x => x.Type).IsInEnum();
                validator.RuleFor(x => x.Description).NotNull().MaximumLength(100);
                validator.RuleFor(x => x.Amount).NotNull().GreaterThan(0);
            }

            AddErrors(validator.Validate(this));
            return !base.HasErrors();
        }
    }
}
