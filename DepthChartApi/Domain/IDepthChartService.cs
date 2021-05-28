using System.Collections.Generic;
using System.Threading.Tasks;
using DepthChartApi.Models;

namespace DepthChartApi.Domain
{
    public interface IDepthChartService
    {
        Task<IEnumerable<Player>> GetChart();
        Task<IEnumerable<Player>> GetPlayersUnderPlayer(Player player);
        Task RemovePlayer(Player player);
        Task AddPlayer(Player player);
    }
}