using AutoMapper;
using EmpXpo.Accounting.Domain.Abstractions.Services;
using EmpXpo.Accounting.Repository;
using EmpXpo.Accounting.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;


namespace EmpXpo.Accounting.WebApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IClientApiApplicationService _clientApiApplicationService;
        private readonly WebAppOptions _options;

        public ReportController(ILogger<ReportController> logger,
                                IClientApiApplicationService clientApiApplicationService,
                                IOptions<WebAppOptions> options
                               )
        {
            _logger = logger;
            _options = options.Value;
            _clientApiApplicationService = clientApiApplicationService;
        }

        
        public async Task<IActionResult> Index()
        {
            await GetCashFlowReport(DateTime.Now);
            await CashFlowReportReferences();            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(CashFlowEntryReportViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await GetCashFlowReport(viewModel.Data);
            }

            await CashFlowReportReferences();
            return View("Index");
        }

        private async Task GetCashFlowReport(DateTime date)
        {
            var param = new Dictionary<string, string>() { { "date", date.ToString("yyyy-MM-dd") } };
            ViewBag.CashFlowsReport = await _clientApiApplicationService.Get<List<CashFlowEntryReportViewModel>>(_options.CashFlowReportApi, param);
        }

        private async Task CashFlowReportReferences()
        {
            var references = await _clientApiApplicationService.Get<List<DateTime>>(_options.CashFlowReportApi);
            ViewBag.CashFlowsReferences = references.Select((item, index) => new SelectListItem
            {
                Selected = index == 0,
                Text = item.ToString("dd-MM-yyyy"),
                Value = item.ToString("dd-MM-yyyy")
            }).ToList();
        }
    }
}
