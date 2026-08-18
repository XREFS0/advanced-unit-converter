using System.Globalization;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;
using AdvancedUnitConverter.Infrastructure.Database;
using Dapper;

namespace AdvancedUnitConverter.Infrastructure.Repositories
{
    public class FavoritesRepository : IFavoritesRepository
    {
        private readonly DatabaseContext _dbContext;

        public FavoritesRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<FavoriteConversion>> GetAllAsync()
        {
            using var connection = _dbContext.CreateConnection();
            string sql = "SELECT * FROM Favorites ORDER BY CreatedAt DESC;";
            var entities = await connection.QueryAsync<FavoriteEntity>(sql);
            return entities.Select(MapToDomain);
        }

        public async Task<long> AddAsync(FavoriteConversion favorite)
        {
            using var connection = _dbContext.CreateConnection();
            string sql = @"
                INSERT OR IGNORE INTO Favorites (Category, FromUnitId, FromUnitName, FromUnitSymbol, ToUnitId, ToUnitName, ToUnitSymbol, CreatedAt)
                VALUES (@Category, @FromUnitId, @FromUnitName, @FromUnitSymbol, @ToUnitId, @ToUnitName, @ToUnitSymbol, @CreatedAt);
                SELECT last_insert_rowid();";

            var entity = MapToEntity(favorite);
            return await connection.ExecuteScalarAsync<long>(sql, entity);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            string sql = "DELETE FROM Favorites WHERE Id = @Id;";
            int affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        public async Task<bool> ExistsAsync(UnitCategoryType category, string fromUnitId, string toUnitId)
        {
            using var connection = _dbContext.CreateConnection();
            string sql = "SELECT COUNT(1) FROM Favorites WHERE Category = @Category AND FromUnitId = @FromUnitId AND ToUnitId = @ToUnitId;";
            int count = await connection.ExecuteScalarAsync<int>(sql, new
            {
                Category = (int)category,
                FromUnitId = fromUnitId,
                ToUnitId = toUnitId
            });
            return count > 0;
        }

        private static FavoriteConversion MapToDomain(FavoriteEntity entity) => new()
        {
            Id = entity.Id,
            Category = (UnitCategoryType)entity.Category,
            FromUnitId = entity.FromUnitId,
            FromUnitName = entity.FromUnitName,
            FromUnitSymbol = entity.FromUnitSymbol,
            ToUnitId = entity.ToUnitId,
            ToUnitName = entity.ToUnitName,
            ToUnitSymbol = entity.ToUnitSymbol,
            CreatedAt = DateTime.TryParse(entity.CreatedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt) ? dt : DateTime.UtcNow
        };

        private static FavoriteEntity MapToEntity(FavoriteConversion model) => new()
        {
            Id = model.Id,
            Category = (int)model.Category,
            FromUnitId = model.FromUnitId,
            FromUnitName = model.FromUnitName,
            FromUnitSymbol = model.FromUnitSymbol,
            ToUnitId = model.ToUnitId,
            ToUnitName = model.ToUnitName,
            ToUnitSymbol = model.ToUnitSymbol,
            CreatedAt = model.CreatedAt.ToString("o", CultureInfo.InvariantCulture)
        };

        private class FavoriteEntity
        {
            public long Id { get; set; }
            public int Category { get; set; }
            public string FromUnitId { get; set; } = string.Empty;
            public string FromUnitName { get; set; } = string.Empty;
            public string FromUnitSymbol { get; set; } = string.Empty;
            public string ToUnitId { get; set; } = string.Empty;
            public string ToUnitName { get; set; } = string.Empty;
            public string ToUnitSymbol { get; set; } = string.Empty;
            public string CreatedAt { get; set; } = string.Empty;
        }
    }
}
