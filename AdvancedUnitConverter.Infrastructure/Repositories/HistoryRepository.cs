using System.Data;
using System.Globalization;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;
using AdvancedUnitConverter.Infrastructure.Database;
using Dapper;

namespace AdvancedUnitConverter.Infrastructure.Repositories
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly DatabaseContext _dbContext;

        public HistoryRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ConversionRecord>> GetAllAsync()
        {
            using var connection = _dbContext.CreateConnection();
            string sql = "SELECT * FROM ConversionHistory ORDER BY Timestamp DESC LIMIT 500;";
            var entities = await connection.QueryAsync<HistoryEntity>(sql);
            return entities.Select(MapToDomain);
        }

        public async Task<IEnumerable<ConversionRecord>> GetByCategoryAsync(UnitCategoryType category)
        {
            using var connection = _dbContext.CreateConnection();
            string sql = "SELECT * FROM ConversionHistory WHERE Category = @Category ORDER BY Timestamp DESC LIMIT 500;";
            var entities = await connection.QueryAsync<HistoryEntity>(sql, new { Category = (int)category });
            return entities.Select(MapToDomain);
        }

        public async Task<IEnumerable<ConversionRecord>> SearchAsync(string query)
        {
            using var connection = _dbContext.CreateConnection();
            string sql = @"
                SELECT * FROM ConversionHistory 
                WHERE FromUnitName LIKE @Q OR ToUnitName LIKE @Q OR FromUnitSymbol LIKE @Q OR ToUnitSymbol LIKE @Q OR FormattedResult LIKE @Q
                ORDER BY Timestamp DESC LIMIT 200;";
            var entities = await connection.QueryAsync<HistoryEntity>(sql, new { Q = $"%{query}%" });
            return entities.Select(MapToDomain);
        }

        public async Task<long> AddAsync(ConversionRecord record)
        {
            using var connection = _dbContext.CreateConnection();
            string sql = @"
                INSERT INTO ConversionHistory (Category, FromUnitId, FromUnitName, FromUnitSymbol, ToUnitId, ToUnitName, ToUnitSymbol, InputValue, ResultValue, FormattedResult, Timestamp)
                VALUES (@Category, @FromUnitId, @FromUnitName, @FromUnitSymbol, @ToUnitId, @ToUnitName, @ToUnitSymbol, @InputValue, @ResultValue, @FormattedResult, @Timestamp);
                SELECT last_insert_rowid();";

            var entity = MapToEntity(record);
            return await connection.ExecuteScalarAsync<long>(sql, entity);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            string sql = "DELETE FROM ConversionHistory WHERE Id = @Id;";
            int affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        public async Task<bool> DeleteBatchAsync(IEnumerable<long> ids)
        {
            using var connection = _dbContext.CreateConnection();
            string sql = "DELETE FROM ConversionHistory WHERE Id IN @Ids;";
            int affected = await connection.ExecuteAsync(sql, new { Ids = ids });
            return affected > 0;
        }

        public async Task<bool> ClearAllAsync()
        {
            using var connection = _dbContext.CreateConnection();
            string sql = "DELETE FROM ConversionHistory;";
            int affected = await connection.ExecuteAsync(sql);
            return affected >= 0;
        }

        private static ConversionRecord MapToDomain(HistoryEntity entity) => new()
        {
            Id = entity.Id,
            Category = (UnitCategoryType)entity.Category,
            FromUnitId = entity.FromUnitId,
            FromUnitName = entity.FromUnitName,
            FromUnitSymbol = entity.FromUnitSymbol,
            ToUnitId = entity.ToUnitId,
            ToUnitName = entity.ToUnitName,
            ToUnitSymbol = entity.ToUnitSymbol,
            InputValue = entity.InputValue,
            ResultValue = entity.ResultValue,
            FormattedResult = entity.FormattedResult,
            Timestamp = DateTime.TryParse(entity.Timestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt) ? dt : DateTime.UtcNow
        };

        private static HistoryEntity MapToEntity(ConversionRecord record) => new()
        {
            Id = record.Id,
            Category = (int)record.Category,
            FromUnitId = record.FromUnitId,
            FromUnitName = record.FromUnitName,
            FromUnitSymbol = record.FromUnitSymbol,
            ToUnitId = record.ToUnitId,
            ToUnitName = record.ToUnitName,
            ToUnitSymbol = record.ToUnitSymbol,
            InputValue = record.InputValue,
            ResultValue = record.ResultValue,
            FormattedResult = record.FormattedResult,
            Timestamp = record.Timestamp.ToString("o", CultureInfo.InvariantCulture)
        };

        private class HistoryEntity
        {
            public long Id { get; set; }
            public int Category { get; set; }
            public string FromUnitId { get; set; } = string.Empty;
            public string FromUnitName { get; set; } = string.Empty;
            public string FromUnitSymbol { get; set; } = string.Empty;
            public string ToUnitId { get; set; } = string.Empty;
            public string ToUnitName { get; set; } = string.Empty;
            public string ToUnitSymbol { get; set; } = string.Empty;
            public double InputValue { get; set; }
            public double ResultValue { get; set; }
            public string FormattedResult { get; set; } = string.Empty;
            public string Timestamp { get; set; } = string.Empty;
        }
    }
}
