using EmpXpo.Accounting.Domain;
using EmpXpo.Accounting.Domain.Abstractions.Application;
using EmpXpo.Accounting.Domain.Abstractions.Repositories;
using EmpXpo.Accounting.Domain.Enumerators;
using EmpXpo.Accounting.Domain.Exceptions;

namespace EmpXpo.Accounting.Application
{
    public class CashFlowApplication : ApplicationBase<CashFlow>, ICashFlowApplication
    {
        private readonly IRepository<CashFlow> _cashFlowRepository;

        public CashFlowApplication(IRepository<CashFlow> cashFlowRepository) : base(cashFlowRepository)
        {
            _cashFlowRepository = cashFlowRepository;
        }

        public override async Task<CashFlow> Create(CashFlow model)
        {
            if (model == null)
            {
                throw new CashFlowException($"Invalid model, the model cannot be null");
            }
            if (model.Amount <= 0)
            {
                throw new CashFlowException($"Invalid Amount {model.Amount}");
            }

            model.CreatedOn = DateTime.Now;
            model.Amount = model.Type == CashFlowType.Credit ? Math.Abs(model.Amount) : -Math.Abs(model.Amount);
            model.Id = Convert.ToInt32(await _cashFlowRepository.Insert(model));

            return model;
        }

        public async Task<IEnumerable<CashFlow>> Get(DateTime model)
        {
            if (model == DateTime.MinValue)
                throw new CashFlowException($"Invalid model, the date is invalid {model}");

            var startDate = model.Date;
            var endDate = model.Date.AddDays(1);

            var entities = await _cashFlowRepository.Query(x => x.CreatedOn >= startDate && x.CreatedOn < endDate);

            return entities;
        }
    }
}
