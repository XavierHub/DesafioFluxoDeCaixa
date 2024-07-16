using EmpXpo.Accounting.Domain.Enumerators;
using EmpXpo.Accounting.Domain.ViewModels;

namespace CashFlowApiTests.Controllers
{
    public class DomainTest
    {
        [Theory]

        [InlineData(CashFlowType.Debit)]
        [InlineData(CashFlowType.Credit)]
        public void WhenValidateViewModel_ShouldReturnValid(CashFlowType type)
        {
            var viewModel = new CashFlowViewModel();
            viewModel.Amount = 100;
            viewModel.Description = "Importante";
            viewModel.Type = type;

            var result = viewModel.IsValid(ValidatorType.Create);

            Assert.True(result);
            Assert.False(viewModel.HasErrors());
            Assert.Empty(viewModel.Errors());
            Assert.True(viewModel.Amount > 0);
            Assert.True(viewModel.Description.Length > 0);
            Assert.Equal(type, viewModel.Type);
        }

        [Theory]

        [InlineData(CashFlowType.Debit)]
        [InlineData(CashFlowType.Credit)]
        public void WhenValidateViewModelWithId_ShouldReturnInValid(CashFlowType type)
        {
            var model = new CashFlowViewModel() { Id = 10, Amount = 100, Description = "Importante", Type = type };
            var result = model.IsValid(ValidatorType.Create);

            Assert.False(result);
            Assert.True(model.HasErrors());
            Assert.NotEmpty(model.Errors());
        }

        [Theory]

        [InlineData(CashFlowType.Debit)]
        [InlineData(CashFlowType.Credit)]
        public void WhenValidateViewModelWithHugeDescription_ShouldReturnInValid(CashFlowType type)
        {
            var model = new CashFlowViewModel() { Id = 10, Amount = 100, Description = new string('S', 300), Type = type };
            var result = model.IsValid(ValidatorType.Create);

            Assert.False(result);
            Assert.True(model.HasErrors());
            Assert.NotEmpty(model.Errors());
        }

        [Theory]

        [InlineData(CashFlowType.Debit)]
        [InlineData(CashFlowType.Credit)]
        public void WhenValidateViewModelWithZeroAmount_ShouldReturnInValid(CashFlowType type)
        {
            var model = new CashFlowViewModel() { Amount = 0, Type = type };
            var result = model.IsValid(ValidatorType.Create);

            Assert.False(result);
            Assert.True(model.HasErrors());
            Assert.NotEmpty(model.Errors());
        }
    }
}