using System.Collections.Generic;
using System.Threading.Tasks;
using DepthChartApi.Models;

namespace DepthChartApi.DataAccess
{
    public interface IPlayerRepository
    {
        Task Add(Player player);
        Task Remove(Player player);
        Task<IEnumerable<Player>> GetPlayers();
    }
}