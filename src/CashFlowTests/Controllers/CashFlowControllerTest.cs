using AutoMapper;
using EmpXpo.Accounting.CashFlowApi.Controllers;
using EmpXpo.Accounting.CashFlowApi.Middlewares;
using EmpXpo.Accounting.Domain;
using EmpXpo.Accounting.Domain.Abstractions.Application;
using EmpXpo.Accounting.Domain.Enumerators;
using EmpXpo.Accounting.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Net;

namespace CashFlowApiTests.Controllers
{
    public class CashFlowControllerTest
    {

        private readonly CashFlowController _cashFlowController;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ICashFlowApplication> _cashFlowApplication;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<HttpResponse> _httpResponseMock;
        private readonly Mock<ILogger<CashFlowExceptionHandlingMiddleware>> _logger;
        Func<Task>? _callbackMethod = null;

        public CashFlowControllerTest()
        {
            _mapper = new Mock<IMapper>();
            _cashFlowApplication = new Mock<ICashFlowApplication>();
            _cashFlowController = new CashFlowController(_mapper.Object, _cashFlowApplication.Object);

            _httpContextMock = new Mock<HttpContext>();
            _httpResponseMock = new Mock<HttpResponse>();
            _logger = new Mock<ILogger<CashFlowExceptionHandlingMiddleware>>();

            _httpResponseMock.Setup(x => x.OnStarting(It.IsAny<Func<Task>>()))
                             .Callback<Func<Task>>(m => _callbackMethod = m);

            _httpContextMock.SetupGet(x => x.Response)
                            .Returns(_httpResponseMock.Object);

            _httpContextMock.SetupGet(x => x.Response.Headers)
                            .Returns(new HeaderDictionary() {
                                                                new KeyValuePair<string, StringValues>("statusCode", new StringValues("200"))
                                                            }
                                    );
        }

        [Fact]
        public async Task WhenRunCreate_ShouldCreateCashFlow()
        {
            var entity = new CashFlow { Id = 1, Amount = 100, Description = "Importante", Type = CashFlowType.Debit, CreatedOn = DateTime.Now };
            _cashFlowApplication.Setup(x => x.Create(It.IsAny<CashFlow>()))
                                .Returns(Task.Run(() => entity));

            var viewModel = new CashFlowViewModel { Amount = 100, Description = "Importante", Type = CashFlowType.Debit };

            var payload = await _cashFlowController.Create(viewModel);
            var result = Assert.IsType<OkObjectResult>(payload);
            var value = Assert.IsType<CashFlow>(result.Value);

            Assert.True(result.StatusCode == (int)HttpStatusCode.OK);
            Assert.NotNull(value);
            Assert.Equal(entity.Id, value.Id);
            Assert.Equal(entity.Amount, value.Amount);
            Assert.Equal(entity.Description, value.Description);
            Assert.Equal(entity.CreatedOn, value.CreatedOn);
            Assert.Equal(entity.Type, value.Type);
        }

        [Fact]
        public async Task WhenRunCreateWithInvalidValues_ShouldReturnBadRequestWithMsgAlerts()
        {
            var viewModel = new CashFlowViewModel();

            var payload = await _cashFlowController.Create(viewModel);
            var result = Assert.IsType<BadRequestObjectResult>(payload);
            var value = Assert.IsType<List<string>>(result.Value);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.True(value.Count > 0);
        }

        [Fact]
        public async Task WhenRunCreateWithId_ShouldReturnBadRequestWithMsgAlerts()
        {
            var viewModel = new CashFlowViewModel { Id = 10 };

            var payload = await _cashFlowController.Create(viewModel);
            var result = Assert.IsType<BadRequestObjectResult>(payload);
            var value = Assert.IsType<List<string>>(result.Value);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.True(value.Count > 0);
        }

        [Fact]
        public async Task WhenRunCreateWithNullViewModel_ShouldReturnBadRequest()
        {
            var payload = await _cashFlowController.Create(null);
            var result = Assert.IsType<BadRequestObjectResult>(payload);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task WhenRunCreateWithZeroAumont_ShouldReturnBadRequestWithMsgAlerts()
        {
            var viewModel = new CashFlowViewModel { Amount = 0, Description = "Muito Importante", Type = CashFlowType.Debit };

            var payload = await _cashFlowController.Create(viewModel);
            var result = Assert.IsType<BadRequestObjectResult>(payload);
            var value = Assert.IsType<List<string>>(result.Value);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.True(value.Count > 0);
        }

        [Fact]
        public async Task WhenRunCreateWithHugeDescription_ShouldReturnBadRequestWithMsgAlerts()
        {
            var viewModel = new CashFlowViewModel { Amount = 100, Description = new string('A', 500), Type = CashFlowType.Debit };

            var payload = await _cashFlowController.Create(viewModel);
            var result = Assert.IsType<BadRequestObjectResult>(payload);
            var value = Assert.IsType<List<string>>(result.Value);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.True(value.Count > 0);
        }



        [Fact]
        public async Task WhenCallApi_ShouldMiddlewareManagementTheUnhandledExceptions()
        {
            var isNextDelegateCalled = false;
            var requestDelegate = new RequestDelegate(async (innerContext) =>
            {
                isNextDelegateCalled = true;

                if (_callbackMethod != null)
                {
                    await _callbackMethod.Invoke();
                }
                else
                {
                    await Task.CompletedTask;
                }
            });

            var middelware = new CashFlowExceptionHandlingMiddleware(requestDelegate, _logger.Object);
            await middelware.InvokeAsync(_httpContextMock.Object);

            Assert.True(isNextDelegateCalled);
            Assert.True((_httpContextMock.Object.Response.Headers.TryGetValue("statusCode", out var value)));
            Assert.Equal((int)HttpStatusCode.OK, int.Parse(value.FirstOrDefault() ?? ""));
        }
    }
}