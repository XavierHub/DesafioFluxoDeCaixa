using EmpXpo.Accounting.Application;
using EmpXpo.Accounting.Domain;
using EmpXpo.Accounting.Domain.Abstractions.Application;
using EmpXpo.Accounting.Domain.Abstractions.Repositories;
using EmpXpo.Accounting.Domain.Exceptions;
using Moq;

namespace CashFlowReportApiTests.Controllers
{
    public class CashFlowApplicationTest
    {
        private readonly Mock<IRepository<CashFlowReport>> _cashFlowReportRepository;
        private readonly ICashFlowReportApplication _cashFlowReportApplication;

        public CashFlowApplicationTest()
        {
            _cashFlowReportRepository = new Mock<IRepository<CashFlowReport>>();
            _cashFlowReportApplication = new CashFlowReportApplication(_cashFlowReportRepository.Object);

            _cashFlowReportRepository.Setup(x => x.Query<DateTime>("CashFlowReportDate", null))
                                     .Returns(Task.FromResult<IEnumerable<DateTime>>(new List<DateTime> { DateTime.Now }));

            _cashFlowReportRepository.Setup(x => x.Query<CashFlowReport>("CashFlowReport", It.IsAny<object>()))
                                     .Returns(() =>
                                                  Task.Run<IEnumerable<CashFlowReport>>(() =>
                                                               new List<CashFlowReport> {
                                                                   new CashFlowReport { Credit = 100, Debit = 50, Total = 50 }
                                                               }
                                                          )
                                             );
        }

        [Fact]
        public async Task WhenRunGet_ShouldReturnListDates()
        {
            var payload = await _cashFlowReportApplication.ListDates();
            var value = Assert.IsAssignableFrom<IEnumerable<DateTime>>(payload);

            Assert.NotNull(value);
            Assert.True(value.Count() > 0);
            Assert.DoesNotContain(DateTime.MinValue, value);
        }

        [Fact]
        public async Task WhenRunGetWithoutDatabaseDate_ShouldReturnEmptyListDates()
        {
            _cashFlowReportRepository.Setup(x => x.Query<DateTime>("CashFlowReportDate", null))
                                     .Returns(Task.FromResult<IEnumerable<DateTime>>(new List<DateTime> { DateTime.Now }));

            var payload = await _cashFlowReportApplication.ListDates();
            var value = Assert.IsAssignableFrom<IEnumerable<DateTime>>(payload);

            Assert.NotNull(value);
            Assert.True(value.Count() > 0);
            Assert.DoesNotContain(DateTime.MinValue, value);
        }

        [Fact]
        public async Task WhenRunGetWithDate_ShouldReturnReport()
        {
            var viewModel = DateTime.Now;
            var payload = await _cashFlowReportApplication.Report(viewModel);
            var value = Assert.IsAssignableFrom<IEnumerable<CashFlowReport>>(payload);

            Assert.NotNull(value);
            Assert.True(value.Count() > 0);
        }

        [Fact]
        public async Task WhenRunGetWithoutDatabaseDate_ShouldReturnReportWithZeroValues()
        {
            _cashFlowReportRepository.Setup(x => x.Query<CashFlowReport>("CashFlowReport", It.IsAny<object>()))
                                     .Returns(() =>
                                                  Task.Run<IEnumerable<CashFlowReport>>(() =>
                                                               new List<CashFlowReport> {
                                                                   new CashFlowReport { Credit = 0, Debit = 0, Total = 0 }
                                                               }
                                                          )
                                             );

            var viewModel = DateTime.Now;
            var payload = await _cashFlowReportApplication.Report(viewModel);
            var values = Assert.IsAssignableFrom<IEnumerable<CashFlowReport>>(payload);

            Assert.NotNull(values);
            Assert.All(values, x =>
            {
                Assert.Equal(0, x.Credit);
                Assert.Equal(0, x.Debit);
                Assert.Equal(0, x.Total);
            });
        }

        [Fact]
        public async Task WhenRunCreateWithInvalidDate_ShouldReturnException()
        {
            var viewModel = DateTime.MinValue;
            await Assert.ThrowsAsync<CashFlowException>(async () => await _cashFlowReportApplication.Report(viewModel));
        }
    }
}