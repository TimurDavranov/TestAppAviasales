using Dapper;
using System.Data;

namespace SA.UnitTests.Base
{
    public interface IDapper
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    }

    public class DapperWrapper : IDapper
    {
        private readonly IDbConnection _connection;

        public DapperWrapper(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await _connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }
    }
}
