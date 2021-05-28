using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepthChartApi.DataAccess;
using DepthChartApi.Domain;
using DepthChartApi.Exceptions;
using DepthChartApi.Models;
using Moq;
using Xunit;

namespace DepthChartApi.Tests.Unit
{
    public class DepthChartServiceTests
    {
        private readonly Mock<IPlayerRepository> _mockPlayerContext;
        private readonly DepthChartService sut;

        public DepthChartServiceTests()
        {
            _mockPlayerContext = new Mock<IPlayerRepository>();
            sut = new DepthChartService(_mockPlayerContext.Object);
        }

        [Fact]
        [Trait("Method", "AddPlayer")]
        public async Task Given_Depth_Is_Null_When_Adding_Player_Then_Depth_Is_Assigned_IntMax()
        {
            var player = new Player { Depth = null };

            await sut.AddPlayer(player);

            Assert.Equal(int.MaxValue, player.Depth);
            _mockPlayerContext.Verify(x => x.Add(player), Times.Once);
        }

        [Fact]
        [Trait("Method", "GetChart")]
        public async Task Given_There_Are_No_Players_GetChart_Returns_Empty_Result()
        {
            var result = await sut.GetChart();

            Assert.Empty(result);
        }

        [Fact]
        [Trait("Method", "GetChart")]
        public async Task Given_Two_Players_Are_At_Same_Position_GetChart_Bumps_Down_The_First_Player()
        {
            var players = new List<Player>
            {
                new Player(){Id=100, Name="Bob", Position = "WR", GameName="NFL", Depth = 0, AtomicCounter = 1},
                new Player(){Id=200, Name="Alice", Position = "WR", GameName="NFL", Depth = 0, AtomicCounter = 2}
            };
            _mockPlayerContext.Setup(x => x.GetPlayers()).ReturnsAsync(players);

            var result = await sut.GetChart();

            Assert.Equal(200, result.First().Id);
        }

        [Fact]
        [Trait("Method", "GetChart")]
        public async Task Given_Depth_Of_A_Player_Is_IntMax_GetChart_Pushes_The_Player_At_End()
        {
            var players = new List<Player>
            {
                new Player(){Id=100, Name="Bob", Position = "WR", GameName="NFL", Depth = int.MaxValue, AtomicCounter = 1},
                new Player(){Id=200, Name="Alice", Position = "WR", GameName="NFL", Depth = 0, AtomicCounter = 2}
            };
            _mockPlayerContext.Setup(x => x.GetPlayers()).ReturnsAsync(players);

            var result = await sut.GetChart();

            Assert.Equal(200, result.First().Id);
            Assert.Equal(100, result.Last().Id);
        }

        [Fact]
        [Trait("Method", "GetChart")]
        public async Task Given_There_Are_Multiple_Games_GetChart_Groups_The_Results_By_Game()
        {
            const string gameNFL = "NLF"; const string gameMLB = "MLB";

            var players = new List<Player>
            {
                new Player(){Id=100, Name="Bob", Position = "WR", GameName=gameNFL, Depth = int.MaxValue, AtomicCounter = 1},
                new Player(){Id=200, Name="Alice", Position = "WR", GameName=gameNFL, Depth = 0, AtomicCounter = 2},
                new Player(){Id=100, Name="Bob", Position = "WR", GameName=gameMLB, Depth = int.MaxValue, AtomicCounter = 1},
                new Player(){Id=200, Name="Alice", Position = "WR", GameName=gameMLB, Depth = 0, AtomicCounter = 2}
            };
            _mockPlayerContext.Setup(x => x.GetPlayers()).ReturnsAsync(players);

            var result = await sut.GetChart();
            var resultsList = result.ToArray();

            Assert.Equal(gameNFL, resultsList[0].GameName);
            Assert.Equal(gameNFL, resultsList[1].GameName);
            Assert.Equal(gameMLB, resultsList[2].GameName);
            Assert.Equal(gameMLB, resultsList[3].GameName);
        }

        [Fact]
        [Trait("Method", "GetChart")]
        public async Task Given_Players_Are_At_Same_Depth_Then_GetChart_Gives_Last_Player_Priority()
        {
            var players = new List<Player>
            {
                new Player(){Id=100, Name="Bob", Position = "WR", GameName= "NFL", Depth = 3, AtomicCounter = 1},
                new Player(){Id=200, Name="Alice", Position = "WR", GameName= "NFL", Depth = 3, AtomicCounter = 2},
                new Player(){Id=300, Name="John", Position = "WR", GameName= "NFL", Depth = 3, AtomicCounter = 3}
            };
            _mockPlayerContext.Setup(x => x.GetPlayers()).ReturnsAsync(players);

            var result = await sut.GetChart();

            Assert.Equal(300, result.First().Id);
            Assert.Equal(100, result.Last().Id);
        }

        [Fact]
        [Trait("Method", "GetChart")]
        public async Task JustOneBigExampleWithMultipleScenarios()
        {
            List<Player> players = new List<Player>
            {
                new Player(){Id=100, Name="Bob", Position = "WR", GameName="NFL", Depth = 0, AtomicCounter = 1},
                new Player(){Id=200, Name="Alice", Position = "WR", GameName="NFL", Depth = 0, AtomicCounter = 2},
                new Player(){Id=300, Name="Charlie", Position = "WR", GameName="NFL",Depth = 2, AtomicCounter = 3},
                new Player(){Id=100, Name="Bob", Position = "KR", GameName="NFL", Depth = int.MaxValue, AtomicCounter = 4},
                new Player(){Id=300, Name="Charlie", Position = "KR", GameName="NFL",Depth = 1, AtomicCounter = 5},
                new Player(){Id=400, Name="Tobey", Position = "WR", GameName="NFL", Depth = int.MaxValue, AtomicCounter = 6}
            };

            _mockPlayerContext.Setup(x => x.GetPlayers()).ReturnsAsync(players);

            var result = await sut.GetChart();
            var resultsList = result.ToList();

            Assert.Equal(200, resultsList[0].Id);
            Assert.Equal(100, resultsList[1].Id);
            Assert.Equal(300, resultsList[2].Id);
            Assert.Equal(400, resultsList[3].Id);
            Assert.Equal(300, resultsList[4].Id);
            Assert.Equal(100, resultsList[5].Id);
        }

        [Fact]
        [Trait("Method", "GetPlayersUnderPlayer")]
        public async Task Given_The_Player_Is_Not_In_The_Chart_Then_GetPlayersUnderPlayer_Throws_PlayerNotFoundException()
        {
            List<Player> players = new List<Player>
            {
                new Player(){Id=100, Name="Bob", Position = "WR", GameName="NFL", Depth = 0, AtomicCounter = 1},
            };
            _mockPlayerContext.Setup(x => x.GetPlayers()).ReturnsAsync(players);

            var playerUnderMe = new Player { Id = 500, Name = "Not A Player", Position = "KR" };

            await Assert.ThrowsAsync<PlayerNotFoundException>(() => sut.GetPlayersUnderPlayer(playerUnderMe));
        }

        [Fact]
        [Trait("Method", "GetPlayersUnderPlayer")]
        public async Task Given_Multiple_Players_With_Different_Postions_When_GetPlayersUnderPlayer_Then_Players_With_Same_Postion_Are_Returned()
        {
            List<Player> players = new List<Player>
            {
                new Player(){Id=100, Name="Bob", Position = "WR", GameName="NFL", Depth = 0, AtomicCounter = 1},
                new Player(){Id=200, Name="Alice", Position = "WR", GameName="NFL", Depth = 0, AtomicCounter = 2},
                new Player(){Id=300, Name="Charlie", Position = "WR", GameName="NFL",Depth = 2, AtomicCounter = 3},
                new Player(){Id=100, Name="Bob", Position = "KR", GameName="NFL", Depth = int.MaxValue, AtomicCounter = 4},
                new Player(){Id=300, Name="Charlie", Position = "KR", GameName="NFL",Depth = 1, AtomicCounter = 5},
                new Player(){Id=400, Name="Tobey", Position = "WR", GameName="NFL", Depth = int.MaxValue, AtomicCounter = 6}
            };
            _mockPlayerContext.Setup(x => x.GetPlayers()).ReturnsAsync(players);

            var playerUnderMe = new Player() { Id = 200, Position = "WR", GameName = "NFL" };
            var result = await sut.GetPlayersUnderPlayer(playerUnderMe);

            var isThereAPostionOtherThanWR = result.Any(x => x.Position.Equals("KR"));
            Assert.False(isThereAPostionOtherThanWR);
        }

        [Fact]
        [Trait("Method", "GetPlayersUnderPlayer")]
        public async Task Given_There_Are_Players_Above_When_GetPlayersUnderPlayer_Then_Players_Above_Are_Skipped()
        {
            List<Player> players = new List<Player>
            {
                new Player(){Id=100, Name="Bob", Position = "WR", GameName="NFL", Depth = 1, AtomicCounter = 1},
                new Player(){Id=200, Name="Alice", Position = "WR", GameName="NFL", Depth = 2, AtomicCounter = 2},
                new Player(){Id=300, Name="Bob", Position = "WR", GameName="NFL", Depth = 3, AtomicCounter = 3},
            };
            _mockPlayerContext.Setup(x => x.GetPlayers()).ReturnsAsync(players);

            var playerUnderMe = new Player() { Id = 200, Position = "WR", GameName = "NFL" };
            var result = await sut.GetPlayersUnderPlayer(playerUnderMe);

            var arePlayersAboveSkipped = result.Any(x => x.Id == 100);
            Assert.False(arePlayersAboveSkipped);
        }

        [Fact]
        [Trait("Method", "GetPlayersUnderPlayer")]
        public async Task Given_There_Are_Players_Above_When_GetPlayersUnderPlayer_Then_Player_ThemSelf_Are_Skipped()
        {
            List<Player> players = new List<Player>
            {
                new Player() {Id=100, Name="Bob", Position = "WR", GameName="NFL", Depth = 1, AtomicCounter = 1},
                new Player() {Id=200, Name="Alice", Position = "WR", GameName="NFL", Depth = 2, AtomicCounter = 2},
                new Player() {Id=300, Name="Bob", Position = "WR", GameName="NFL", Depth = 3, AtomicCounter = 3},
            };
            _mockPlayerContext.Setup(x => x.GetPlayers()).ReturnsAsync(players);

            var playerUnderMe = new Player() { Id = 200, Position = "WR", GameName = "NFL" };
            var result = await sut.GetPlayersUnderPlayer(playerUnderMe);

            var arePlayersAboveSkipped = result.Any(x => x.Id == 100 || x.Id == 200);
            Assert.False(arePlayersAboveSkipped);
            Assert.Single(result);
            Assert.Equal(300, result.First().Id);
        }

        [Fact]
        [Trait("Method", "RemovePlayer")]
        public async Task Given_Player_Does_Not_Exist_When_Removing_Then_Remove_Method_Throws_Exception()
        {
            List<Player> players = new List<Player>
            {
                new Player() {Id=100, Name="Bob", Position = "WR", GameName="NFL", Depth = 1, AtomicCounter = 1},
            };
            _mockPlayerContext.Setup(x => x.GetPlayers()).ReturnsAsync(players);

            var playerToRemove = new Player() { Id = 500, Position = "WR", GameName = "NFL" };

            await Assert.ThrowsAsync<PlayerNotFoundException>(() => sut.RemovePlayer(playerToRemove));
        }

        [Fact]
        [Trait("Method", "RemovePlayer")]
        public async Task Given_Player_Exist_With_Given_Position_When_Removing_Then_Remove_Method_Is_Called_With_The_Player()
        {
            List<Player> players = new List<Player>
            {
                new Player() {Id=100, Name="Bob", Position = "WR", GameName="NFL", Depth = 1}
            };
            _mockPlayerContext.Setup(x => x.GetPlayers()).ReturnsAsync(players);

            var playerToRemove = new Player() { Id = 100, Position = "WR", GameName = "NFL" };
            await sut.RemovePlayer(playerToRemove);

            _mockPlayerContext.Verify(x => x.Remove(players.First()), Times.Once);
        }
    }
}