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
    public class ReportControllerTest
    {
        private readonly ReportController _reportController;
        
        private readonly Mock<IClientApiApplicationService> _clientApiApplicationService;
        private readonly Mock<IOptions<WebAppOptions>> _options;        
        private readonly Mock<ILogger<ReportController>> _logger;
        private const string CASH_FLOW_REPORT_API = "http://CashFlowReportApi";
        private const string CASH_FLOW_API = "http://CashFlowApi.com";

        public ReportControllerTest()
        {
            _logger = new Mock<ILogger<ReportController>>();
            _options = new Mock<IOptions<WebAppOptions>>();
            _clientApiApplicationService = new Mock<IClientApiApplicationService>();

            _options.Setup(x => x.Value)
                    .Returns(new WebAppOptions()
                    {
                        CashFlowApi = CASH_FLOW_API,
                        CashFlowReportApi = CASH_FLOW_REPORT_API
                    });

            _reportController = new ReportController(_logger.Object, _clientApiApplicationService.Object, _options.Object);


            _clientApiApplicationService.Setup(x => x.Get<List<CashFlowEntryReportViewModel>>(CASH_FLOW_REPORT_API, It.IsAny<Dictionary<string, string>>()))
                                        .Returns(Task.Run(() => 
                                                    new List<CashFlowEntryReportViewModel>() { new CashFlowEntryReportViewModel() { Debit=100, Credit=200, Total=100 } }
                                                )
                                        );

            _clientApiApplicationService.Setup(x => x.Get<List<DateTime>>(CASH_FLOW_REPORT_API, null))
                                        .Returns(Task.Run(() =>
                                                    new List<DateTime>() { DateTime.Now }
                                                )
                                        );
        }

        [Fact]
        public async Task WhenRunIndex_ShouldReturnViewWithViewBagData()
        {
            var payload = await _reportController.Index();
            var result = Assert.IsType<ViewResult>(payload);
            var cashFlowsReferences = Assert.IsAssignableFrom<List<SelectListItem>>(result.ViewData["CashFlowsReferences"]);            
            var cashFlows = Assert.IsAssignableFrom<List<CashFlowEntryReportViewModel>>(result.ViewData["CashFlowsReport"]);            

            Assert.NotNull(cashFlowsReferences);            
            Assert.True(cashFlowsReferences.Count() > 0);

            Assert.NotNull(cashFlows);
            Assert.True(cashFlows.Count() > 0);
        }


        [Fact]
        public async Task WhenRunIndexWithCashFlowData_ShouldReturnViewWithViewBagDataCashFlowEmpytList()
        {
            _clientApiApplicationService.Setup(x => x.Get<List<CashFlowEntryReportViewModel>>(CASH_FLOW_REPORT_API, It.IsAny<Dictionary<string, string>>()))
                                        .Returns(Task.Run(() =>
                                                    new List<CashFlowEntryReportViewModel>() { }
                                                )
                                        );

            var payload = await _reportController.Index();
            var result = Assert.IsType<ViewResult>(payload);
            var cashFlowsReferences = Assert.IsAssignableFrom<List<SelectListItem>>(result.ViewData["CashFlowsReferences"]);
            var cashFlows = Assert.IsAssignableFrom<List<CashFlowEntryReportViewModel>>(result.ViewData["CashFlowsReport"]);

            Assert.NotNull(cashFlowsReferences);
            Assert.True(cashFlowsReferences.Count() > 0);

            Assert.NotNull(cashFlows);
            Assert.True(cashFlows.Count() == 0);
        }

        [Fact]
        public async Task WhenRunIndexWithParameters_ShouldReturnViewWithViewBagData()
        {
            var viewModel = new CashFlowEntryReportViewModel { Data= DateTime.Now  };
            var payload = await _reportController.Index(viewModel);
            var result = Assert.IsType<ViewResult>(payload);
            var CashFlowsReferences = Assert.IsAssignableFrom<List<SelectListItem>>(result.ViewData["CashFlowsReferences"]);
            var cashFlows = Assert.IsAssignableFrom<List<CashFlowEntryReportViewModel>>(result.ViewData["CashFlowsReport"]);

            Assert.NotNull(CashFlowsReferences);
            Assert.True(CashFlowsReferences.Count() > 0);

            Assert.NotNull(cashFlows);
            Assert.True(cashFlows.Count() > 0);
        }
    }
}