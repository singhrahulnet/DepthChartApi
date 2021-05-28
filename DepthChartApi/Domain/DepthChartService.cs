using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepthChartApi.DataAccess;
using DepthChartApi.Exceptions;
using DepthChartApi.Models;

namespace DepthChartApi.Domain
{
    public class DepthChartService : IDepthChartService
    {
        private readonly IPlayerRepository _playerRepository;

        public DepthChartService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task AddPlayer(Player player)
        {
            if (player.Depth == null) player.Depth = int.MaxValue;

            await _playerRepository.Add(player);
        }

        public async Task<IEnumerable<Player>> GetChart()
        {
            var allPlayers = await _playerRepository.GetPlayers();

            var sortedPlayers = allPlayers.GroupBy(x => new { x.GameName, x.Position })
                                    .SelectMany(p => p.OrderBy(x => x.Depth)
                                                    .ThenByDescending(c => c.AtomicCounter));

            return sortedPlayers;
        }

        public async Task<IEnumerable<Player>> GetPlayersUnderPlayer(Player player)
        {
            var chart = await GetChart();

            var isPlayerInTheChart = chart.Any(PlayerAtTheSpecifiedPosition(player));

            if (isPlayerInTheChart == false) throw new PlayerNotFoundException(nameof(RemovePlayer));

            var allPlayersWithSamePosition = chart.Where(PlayersWithSamePositionInTheGame(player));

            var playersUnder = allPlayersWithSamePosition.SkipWhile(IsNotThisPlayer(player));

            //Skip the passed in player
            return playersUnder.Skip(1);
        }

        private Func<Player, bool> PlayersWithSamePositionInTheGame(Player player)
        {
            return x => x.Position == player.Position
                        && x.GameName == player.GameName;
        }

        private Func<Player, bool> IsNotThisPlayer(Player player)
        {
            return x => x.Id != player.Id;
        }

        public async Task RemovePlayer(Player player)
        {
            var players = await _playerRepository.GetPlayers();
            var playerToRemove = players.FirstOrDefault(PlayerAtTheSpecifiedPosition(player));

            if (playerToRemove == null) throw new PlayerNotFoundException(nameof(RemovePlayer));

            await _playerRepository.Remove(playerToRemove);
        }

        private Func<Player, bool> PlayerAtTheSpecifiedPosition(Player player)
        {
            return x => x.Id == player.Id
                        && x.Position == player.Position
                        && x.GameName == player.GameName;
        }
    }
}
