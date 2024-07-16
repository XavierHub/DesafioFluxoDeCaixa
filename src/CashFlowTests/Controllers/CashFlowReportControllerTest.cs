using AutoMapper;
using EmpXpo.Accounting.CashFlowApi.Controllers;
using EmpXpo.Accounting.CashFlowApi.Middlewares;
using EmpXpo.Accounting.Domain;
using EmpXpo.Accounting.Domain.Abstractions.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Net;

namespace CashFlowReportApiTests.Controllers
{
    public class CashFlowReportControllerTest
    {

        private readonly CashFlowReportController _cashFlowReportController;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ICashFlowReportApplication> _cashFlowReportApplication;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<HttpResponse> _httpResponseMock;
        private readonly Mock<ILogger<CashFlowReportExceptionHandlingMiddleware>> _logger;
        Func<Task>? _callbackMethod = null;

        public CashFlowReportControllerTest()
        {
            _mapper = new Mock<IMapper>();
            _cashFlowReportApplication = new Mock<ICashFlowReportApplication>();

            _cashFlowReportController = new CashFlowReportController(_mapper.Object, _cashFlowReportApplication.Object);

            _cashFlowReportApplication.Setup(x => x.ListDates())
                                      .Returns(Task.FromResult<IEnumerable<DateTime>>(new List<DateTime> { DateTime.Now }));

            _cashFlowReportApplication.Setup(x => x.Report(It.IsAny<DateTime>()))
                                      .Returns(Task.FromResult<IEnumerable<CashFlowReport>>(new List<CashFlowReport> {
                                          new CashFlowReport{ Credit = 1000, Debit = 300, Total = 700 }
                                      }));



            _httpContextMock = new Mock<HttpContext>();
            _httpResponseMock = new Mock<HttpResponse>();
            _logger = new Mock<ILogger<CashFlowReportExceptionHandlingMiddleware>>();

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
        public async Task WhenRunGet_ShouldReturnListDates()
        {
            var payload = await _cashFlowReportController.Get();
            var result = Assert.IsType<OkObjectResult>(payload);
            var value = Assert.IsAssignableFrom<IEnumerable<DateTime>>(result.Value);

            Assert.True(result.StatusCode == (int)HttpStatusCode.OK);
            Assert.NotNull(value);
            Assert.True(value.Count() > 0);
        }

        [Fact]
        public async Task WhenRunGetWithoutDatabaseDate_ShouldReturnEmptyListDates()
        {
            _cashFlowReportApplication.Setup(x => x.ListDates())
                                      .Returns(Task.FromResult<IEnumerable<DateTime>>(new List<DateTime> { }));

            var payload = await _cashFlowReportController.Get();
            var result = Assert.IsType<OkObjectResult>(payload);
            var value = Assert.IsAssignableFrom<IEnumerable<DateTime>>(result.Value);

            Assert.True(result.StatusCode == (int)HttpStatusCode.OK);
            Assert.NotNull(value);
            Assert.True(value.Count() == 0);
        }

        [Fact]
        public async Task WhenRunGetWithDate_ShouldReturnReport()
        {
            var viewModel = DateTime.Now;
            var payload = await _cashFlowReportController.Get(viewModel);
            var result = Assert.IsType<OkObjectResult>(payload);
            var value = Assert.IsAssignableFrom<IEnumerable<CashFlowReport>>(result.Value);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(value);
            Assert.True(value.Count()>0);
        }

        [Fact]
        public async Task WhenRunGetWithoutDatabaseDate_ShouldReturnReportWithZeroValues()
        {
            _cashFlowReportApplication.Setup(x => x.Report(It.IsAny<DateTime>()))
                                      .Returns(Task.FromResult<IEnumerable<CashFlowReport>>(new List<CashFlowReport> {
                                          new CashFlowReport{ Credit = 0, Debit = 0, Total = 0 }
                                      }));


            var viewModel = DateTime.Now;
            var payload = await _cashFlowReportController.Get(viewModel);
            var result = Assert.IsType<OkObjectResult>(payload);
            var values = Assert.IsAssignableFrom<IEnumerable<CashFlowReport>>(result.Value);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(values);
            Assert.All(values, x =>
            {
                Assert.Equal(0, x.Credit);
                Assert.Equal(0, x.Debit);
                Assert.Equal(0, x.Total);
            });
        }

        [Fact]
        public async Task WhenRunCreateWithInvalidDate_ShouldReturnBadRequest()
        {
            var viewModel = DateTime.MinValue;
            var payload = await _cashFlowReportController.Get(viewModel);
            var result = Assert.IsType<BadRequestResult>(payload);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
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

            var middelware = new CashFlowReportExceptionHandlingMiddleware(requestDelegate, _logger.Object);
            await middelware.InvokeAsync(_httpContextMock.Object);

            Assert.True(isNextDelegateCalled);
            Assert.True((_httpContextMock.Object.Response.Headers.TryGetValue("statusCode", out var value)));
            Assert.Equal((int)HttpStatusCode.OK, int.Parse(value.FirstOrDefault() ?? ""));
        }

    }
}