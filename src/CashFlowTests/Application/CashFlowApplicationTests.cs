using EmpXpo.Accounting.Application;
using EmpXpo.Accounting.Domain;
using EmpXpo.Accounting.Domain.Abstractions.Application;
using EmpXpo.Accounting.Domain.Abstractions.Repositories;
using EmpXpo.Accounting.Domain.Enumerators;
using EmpXpo.Accounting.Domain.Exceptions;
using Moq;

namespace CashFlowApiTests.Application
{
    public class CashFlowApplicationTest
    {
        private readonly Mock<IRepository<CashFlow>> _cashFlowRepository;
        private readonly ICashFlowApplication _cashFlowApplication;

        public CashFlowApplicationTest()
        {
            _cashFlowRepository = new Mock<IRepository<CashFlow>>();
            _cashFlowApplication = new CashFlowApplication(_cashFlowRepository.Object);
            _cashFlowRepository.Setup(x => x.Insert(It.IsAny<CashFlow>()))
                               .Returns(() => Task.Run(() => (object)10));
        }

        [Theory]
        [InlineData(CashFlowType.Debit)]
        [InlineData(CashFlowType.Credit)]
        public async Task WhenRunCreate_ShouldCreateCashFlow(CashFlowType type)
        {
            var model = new CashFlow { Amount = 100, Description = "Importante", Type = type };
            var payload = await _cashFlowApplication.Create(model);
            var value = Assert.IsType<CashFlow>(payload);

            Assert.NotNull(value);
            Assert.True(value.Id > 0);
            Assert.NotEqual(DateTime.MinValue, value.CreatedOn);
        }

        [Fact]
        public async Task WhenRunCreateWithInvalidValues_ShouldThrowException()
        {
            var model = new CashFlow();
            await Assert.ThrowsAsync<CashFlowException>(async () => await _cashFlowApplication.Create(model));
        }

        [Theory]
        [InlineData(CashFlowType.Debit)]
        [InlineData(CashFlowType.Credit)]
        public async Task WhenRunCreateWithId_ShouldCreateCashFlow(CashFlowType type)
        {
            var model = new CashFlow { Id = 10, Amount = 100, Type = type };
            var payload = await _cashFlowApplication.Create(model);
            var value = Assert.IsType<CashFlow>(payload);

            Assert.True(value.Id > 0);
            Assert.NotEqual(DateTime.MinValue, value.CreatedOn);
        }

        [Fact]
        public async Task WhenRunCreateWithNullViewModel_ShouldThrowException()
        {
            await Assert.ThrowsAsync<CashFlowException>(async () => await _cashFlowApplication.Create(null));
        }

        [Fact]
        public async Task WhenRunCreateWithZeroAumont_ShouldThrowException()
        {
            var model = new CashFlow { Amount = 0, Description = "Muito Importante", Type = CashFlowType.Debit };
            await Assert.ThrowsAsync<CashFlowException>(async () => await _cashFlowApplication.Create(model));
        }

        [Fact]
        public async Task WhenRunCreateWithHugeDescription_ShouldThrowException()
        {
            _cashFlowRepository.Setup(x => x.Insert(It.IsAny<CashFlow>()))
                               .Throws<Exception>();

            var model = new CashFlow { Amount = 200, Description = new string('A', 500), Type = CashFlowType.Debit };
            await Assert.ThrowsAsync<Exception>(async () => await _cashFlowApplication.Create(model));
        }
    }
}