using Dapper;
using Dommel;
using EmpXpo.Accounting.Domain.Abstractions.Repositories;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace EmpXpo.Accounting.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly CashFlowRepositoryOptions _options;

        public Repository(IOptions<CashFlowRepositoryOptions> options)
        {
            _options = options.Value;

            if (string.IsNullOrWhiteSpace(_options.ConnectionString))
                throw new Exception("Connection string is empty");
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            return await connection.GetAllAsync<T>();
        }


        public async Task<T> GetById(object parm)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            return await connection.GetAsync<T>(parm) ?? Activator.CreateInstance<T>();
        }
        public async Task<bool> Delete(T entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            return await connection.DeleteAsync(entity);
        }

        public async Task<bool> Update(T entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            return await connection.UpdateAsync<T>(entity);
        }

        public async Task<object> Insert(T entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            return await connection.InsertAsync(entity);
        }

        public async Task<IEnumerable<T>> Query(Expression<Func<T, bool>> expression)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            return await connection.SelectAsync(expression);
        }

        public async Task<IEnumerable<U>> Query<U>(string spName, object? parm = null)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            return await connection.QueryAsync<U>(spName,
                                                  param: parm,
                                                  commandType: CommandType.StoredProcedure);
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_options.ConnectionString);
        }
    }
}
