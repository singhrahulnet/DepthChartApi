using System;
using System.Collections.Generic;
using DepthChartApi.Models;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace DepthChartApi.Domain
{
    public class ValidationService : AbstractValidator<Player>, IValidationService
    {
        private static List<Game> _supportedGames = new List<Game>();

        public ValidationService(IConfiguration configuration)
        {
            configuration.GetSection("Games").Bind(_supportedGames);
            SetupTheRules();
        }

        private void SetupTheRules()
        {
            RuleFor(player => player.Id).NotNull().GreaterThan(0);
            RuleFor(player => player.Name).NotEmpty();
            RuleFor(player => player.Position).NotEmpty();
            RuleFor(player => player.GameName).NotEmpty();

            RuleFor(player => player).Must(PositionBeSupportedForTheGame())
                                     .WithMessage(x => $"Either the Game or the position {x.Position} is not suppoted for {x.GameName}");
        }

        private Func<Player, bool> PositionBeSupportedForTheGame()
        {
            return player =>
            {
                var game = _supportedGames.Find(game => game.Name.Equals(player.GameName));

                return game != null && game.Positions.Any(p => p.Equals(player.Position));
            };
        }
    }
}
