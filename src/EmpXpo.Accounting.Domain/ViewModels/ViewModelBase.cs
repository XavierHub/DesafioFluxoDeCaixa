using EmpXpo.Accounting.Domain.Abstractions.Domain;
using EmpXpo.Accounting.Domain.Enumerators;
using FluentValidation.Results;

namespace EmpXpo.Accounting.Domain.ViewModels
{
    public abstract class ViewModelBase<T> : IViewModel
    {
        protected List<string> _errors = new List<string>();

        public bool HasErrors()
        {
            return _errors.Count > 0;
        }
        public virtual bool IsValid(ValidatorType validator)
        {
            return default;
        }
        protected void AddErrors(ValidationResult validate)
        {
            for (int i = 0; i < validate.Errors.Count; i++)
            {
                _errors.Add($"{typeof(T).Name}, {validate.Errors[i].ErrorMessage}");
            }
        }
        public List<string> Errors()
        {
            return _errors;
        }
    }
}
