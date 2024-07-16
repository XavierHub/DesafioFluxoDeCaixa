using AutoMapper;
using EmpXpo.Accounting.Domain;
using EmpXpo.Accounting.Domain.Abstractions.Application;
using EmpXpo.Accounting.Domain.Enumerators;
using EmpXpo.Accounting.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmpXpo.Accounting.CashFlowApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CashFlowController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICashFlowApplication _cashFlowApplication;

        public CashFlowController(IMapper mapper,
                                  ICashFlowApplication cashFlowApplication
                                 )
        {
            _mapper = mapper;
            _cashFlowApplication = cashFlowApplication;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CashFlowViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (model == null || !model.IsValid(ValidatorType.Create))
                return BadRequest(model?.Errors());

            return Ok(await _cashFlowApplication.Create(_mapper.Map<CashFlow>(model)));
        }

        [HttpGet]
        [Route("{date}")]
        public async Task<IActionResult> Get(DateTime date)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (date == null || DateTime.MinValue == date)
                return BadRequest();

            return Ok(await _cashFlowApplication.Get(date));
        }
    }
}
