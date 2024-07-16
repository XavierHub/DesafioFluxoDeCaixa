namespace EmpXpo.Accounting.Domain.Abstractions.Application
{
    public interface ICashFlowReportApplication
    {
        public Task<IEnumerable<DateTime>> ListDates();
        public Task<IEnumerable<CashFlowReport>> Report(DateTime date);
    }
}
