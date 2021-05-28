using System.Collections.Generic;
using System.Threading.Tasks;
using DepthChartApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DepthChartApi.DataAccess
{
    public class PlayerRepository : DbContext, IPlayerRepository
    {
        private DbSet<Player> _players { get; set; }

        public PlayerRepository() { }

        public PlayerRepository(DbContextOptions<PlayerRepository> options)
          : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseInMemoryDatabase("ChartOfPlayersDB");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.AtomicCounter);
            entity.HasIndex(p => new { p.Id, p.Position, p.GameName }).IsUnique();
        });

        public async Task Add(Player player)
        {
            _players.Add(player);
            await SaveChangesAsync();
        }
        public async Task Remove(Player player)
        {
            _players.Remove(player);
            await SaveChangesAsync();
        }
        public async Task<IEnumerable<Player>> GetPlayers()
        {
            return await _players.ToListAsync();
        }
    }
}
