using EmpXpo.Accounting.Domain.Abstractions.Services;
using EmpXpo.Accounting.Repository;
using EmpXpo.Accounting.WebApp.Controllers;
using EmpXpo.Accounting.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CashFlowReportApiTests.Controllers
{
    public class HomeControllerTest
    {
        private readonly HomeController _homeController;
        
        private readonly Mock<IClientApiApplicationService> _clientApiApplicationService;
        private readonly Mock<IOptions<WebAppOptions>> _options;
        private readonly Mock<ILogger<HomeController>> _logger;
        private const string CASH_FLOW_REPORT_API = "http://CashFlowReportApi";
        private const string CASH_FLOW_API = "http://CashFlowApi.com";        

        public HomeControllerTest()
        {
            _logger = new Mock<ILogger<HomeController>>();
            _options = new Mock<IOptions<WebAppOptions>>();
            _clientApiApplicationService = new Mock<IClientApiApplicationService>();

            _options.Setup(x => x.Value)
                    .Returns(new WebAppOptions()
                    {
                        CashFlowApi = CASH_FLOW_API,
                        CashFlowReportApi = CASH_FLOW_REPORT_API
                    });

            _homeController = new HomeController(_logger.Object, _clientApiApplicationService.Object, _options.Object);


            _clientApiApplicationService.Setup(x => x.Get<List<CashFlowEntryViewModel>>(CASH_FLOW_API, It.IsAny<Dictionary<string, string>>()))
                                        .Returns(Task.Run(() => 
                                                    new List<CashFlowEntryViewModel>() { new CashFlowEntryViewModel() { Amount = 100, CreatedOn=DateTime.Now, Description="novo" } }
                                                )
                                        );
        }

        [Fact]
        public async Task WhenRunIndex_ShouldReturnViewWithViewBagData()
        {
            var payload = await _homeController.Index();
            var result = Assert.IsType<ViewResult>(payload);
            var cashFlowTypes = Assert.IsAssignableFrom<List<SelectListItem>>(result.ViewData["CashFlowTypes"]);            
            var cashFlows = Assert.IsAssignableFrom<List<CashFlowEntryViewModel>>(result.ViewData["CashFlows"]);            

            Assert.NotNull(cashFlowTypes);            
            Assert.True(cashFlowTypes.Count() > 0);

            Assert.NotNull(cashFlows);
            Assert.True(cashFlows.Count() > 0);
        }


        [Fact]
        public async Task WhenRunIndexWithCashFlowData_ShouldReturnViewWithViewBagDataCashFlowEmpytList()
        {
            _clientApiApplicationService.Setup(x => x.Get<List<CashFlowEntryViewModel>>(CASH_FLOW_API, It.IsAny<Dictionary<string, string>>()))
                                        .Returns(Task.Run(() =>
                                                    new List<CashFlowEntryViewModel>()
                                                )
                                        );

            var payload = await _homeController.Index();
            var result = Assert.IsType<ViewResult>(payload);
            var cashFlowTypes = Assert.IsAssignableFrom<List<SelectListItem>>(result.ViewData["CashFlowTypes"]);
            var cashFlows = Assert.IsAssignableFrom<List<CashFlowEntryViewModel>>(result.ViewData["CashFlows"]);

            Assert.NotNull(cashFlowTypes);
            Assert.True(cashFlowTypes.Count() > 0);

            Assert.NotNull(cashFlows);
            Assert.True(cashFlows.Count() == 0);
        }

        [Fact]
        public async Task WhenRunIndexWithParameters_ShouldReturnViewWithViewBagData()
        {
            var viewModel = new CashFlowEntryViewModel { Amount=100, Description="sadfdasdf" };
            var payload = await _homeController.Index(viewModel);
            var result = Assert.IsType<ViewResult>(payload);
            var cashFlowTypes = Assert.IsAssignableFrom<List<SelectListItem>>(result.ViewData["CashFlowTypes"]);
            var cashFlows = Assert.IsAssignableFrom<List<CashFlowEntryViewModel>>(result.ViewData["CashFlows"]);

            Assert.NotNull(cashFlowTypes);
            Assert.True(cashFlowTypes.Count() > 0);

            Assert.NotNull(cashFlows);
            Assert.True(cashFlows.Count() > 0);
        }
    }
}