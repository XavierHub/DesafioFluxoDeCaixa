using AutoMapper;
using EmpXpo.Accounting.Domain.Abstractions.Services;
using EmpXpo.Accounting.Domain.Enumerators;
using EmpXpo.Accounting.Repository;
using EmpXpo.Accounting.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace EmpXpo.Accounting.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IClientApiApplicationService _clientApiApplicationService;
        private readonly WebAppOptions _options;

        public HomeController(ILogger<HomeController> logger,
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
            await GetCashFlowData();
            CashFlowTypes();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(CashFlowEntryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await _clientApiApplicationService.Post(_options.CashFlowApi, viewModel);
            }

            await GetCashFlowData();
            CashFlowTypes();
            return View("Index");
        }

        private async Task GetCashFlowData()
        {
            var param = new Dictionary<string, string>() { { "date", DateTime.Now.ToString("yyyy-MM-dd") } };
            ViewBag.CashFlows = await _clientApiApplicationService.Get<List<CashFlowEntryViewModel>>(_options.CashFlowApi, param);
        }

        private void CashFlowTypes()
        {
            ViewBag.CashFlowTypes = Enum.GetValues(typeof(CashFlowType)).Cast<CashFlowType>().Select(x => new SelectListItem
            {
                Selected = x == CashFlowType.Debit,
                Text = x.ToString(),
                Value = ((int)x).ToString()
            }).ToList();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError($"An unhandled exception has occurred|RequestId:{ Activity.Current?.Id ?? HttpContext.TraceIdentifier}");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
