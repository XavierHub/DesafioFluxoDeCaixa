namespace EmpXpo.Accounting.Domain.Abstractions.Services
{
    public interface IClientApiApplicationService
    {
        public Task<T> Get<T>(string api, IDictionary<string, string>? param = null);
        public Task<T> Post<T>(string api, T param);
    }
}
