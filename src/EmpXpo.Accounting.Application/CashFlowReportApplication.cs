using EmpXpo.Accounting.Domain;
using EmpXpo.Accounting.Domain.Abstractions.Application;
using EmpXpo.Accounting.Domain.Abstractions.Repositories;
using EmpXpo.Accounting.Domain.Exceptions;

namespace EmpXpo.Accounting.Application
{
    public class CashFlowReportApplication : ICashFlowReportApplication
    {
        private readonly IRepository<CashFlowReport> _cashFlowReportRepository;

        public CashFlowReportApplication(IRepository<CashFlowReport> cashFlowReportRepository)
        {
            _cashFlowReportRepository = cashFlowReportRepository;
        }

        public async Task<IEnumerable<DateTime>> ListDates()
        {
            return await _cashFlowReportRepository.Query<DateTime>("CashFlowReportDate");
        }

        public async Task<IEnumerable<CashFlowReport>> Report(DateTime model)
        {
            if (model == DateTime.MinValue)
                throw new CashFlowException($"Invalid model, the date is invalid {model}");

            var startDate = new DateTime(model.Year, model.Month, model.Day);
            var endDate = startDate.AddDays(1);

            return (await _cashFlowReportRepository.Query<CashFlowReport>("CashFlowReport", new { startDate, endDate }));
        }
    }
}
