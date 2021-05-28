using System.Collections.Generic;
using System.Threading.Tasks;
using DepthChartApi.Domain;
using DepthChartApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DepthChartApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepthChartController : ControllerBase
    {
        private readonly IDepthChartService _depthChartService;
        private readonly IValidationService _validationService;

        public DepthChartController(IDepthChartService depthChartService
                                    , IValidationService validationService)
        {
            _depthChartService = depthChartService;
            _validationService = validationService;
        }

        /// <summary>
        /// Add a new Player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add(Player player)
        {
            var validationResult = _validationService.Validate(player);
            if (validationResult.IsValid == false) return BadRequest(validationResult.Errors);

            await _depthChartService.AddPlayer(player);

            return CreatedAtAction(nameof(Get), player);
        }

        /// <summary>
        /// Get the depth chart of players with their positions for all supported games
        /// </summary>
        /// <returns>List of Players arranged in a depth chart</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Player>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<Player>> Get()
        {
            return await _depthChartService.GetChart();
        }

        /// <summary>
        /// Get all the players under the passed in player for a specific position in a game
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="position"></param>
        /// <param name="playerId"></param>
        /// <returns>List of players</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Player>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/{gameName}/{position}/{playerId}")]
        public async Task<IEnumerable<Player>> GetPlayerUnder(string gameName, string position, int playerId)
        {
            var player = new Player { GameName = gameName, Position = position, Id = playerId };
            return await _depthChartService.GetPlayersUnderPlayer(player);
        }

        /// <summary>
        /// Remove a player from a position for a given game
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/player")]
        public async Task<IActionResult> Remove(Player player)
        {
            var validationResult = _validationService.Validate(player);
            if (validationResult.IsValid == false) return BadRequest(validationResult.Errors);

            await _depthChartService.RemovePlayer(player);

            return NoContent();
        }
    }
}