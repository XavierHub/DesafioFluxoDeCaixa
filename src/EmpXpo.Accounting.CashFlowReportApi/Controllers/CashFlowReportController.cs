using AutoMapper;
using EmpXpo.Accounting.Domain.Abstractions.Application;
using Microsoft.AspNetCore.Mvc;

namespace EmpXpo.Accounting.CashFlowApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CashFlowReportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICashFlowReportApplication _cashFlowReportApplication;

        public CashFlowReportController(IMapper mapper,
                                        ICashFlowReportApplication cashFlowReportApplication
                                       )
        {
            _mapper = mapper;
            _cashFlowReportApplication = cashFlowReportApplication;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(await _cashFlowReportApplication.ListDates());
        }

        [HttpGet]
        [Route("{date}")]
        public async Task<IActionResult> Get(DateTime date)
        {
            if (!ModelState.IsValid || date == DateTime.MinValue)
                return BadRequest();

            return Ok(await _cashFlowReportApplication.Report(date));
        }
    }
}
