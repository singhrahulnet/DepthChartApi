using System.Collections.Generic;
using System.Threading.Tasks;
using DepthChartApi.Controllers;
using DepthChartApi.Domain;
using DepthChartApi.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DepthChartApi.Tests.Unit
{
    public class DepthChartControllerTests
    {
        private readonly Mock<IDepthChartService> _mockDepthChartService;
        private readonly Mock<IValidationService> _mockValidationService;
        private readonly Mock<ValidationResult> _mockValidatioResult;
        private readonly DepthChartController sut;

        public DepthChartControllerTests()
        {
            _mockDepthChartService = new Mock<IDepthChartService>();
            _mockValidationService = new Mock<IValidationService>();
            _mockValidatioResult = new Mock<ValidationResult>();
            sut = new DepthChartController(_mockDepthChartService.Object
                                            , _mockValidationService.Object);
        }

        [Fact]
        [Trait("Method", "Add")]
        public async Task Given_Model_Is_Invalid_Then_Add_Method_Returns_400()
        {
            _mockValidatioResult.Setup(x => x.IsValid).Returns(false);
            _mockValidationService.Setup(x => x.Validate(null)).Returns(_mockValidatioResult.Object);

            var result = await sut.Add(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        [Trait("Method", "Add")]
        public async Task Given_Model_Is_Valid_Then_Add_Method_Returns_201()
        {
            var playerToAdd = new Player { Id = 100 };
            _mockValidatioResult.Setup(x => x.IsValid).Returns(true);
            _mockValidationService.Setup(x => x.Validate(playerToAdd)).Returns(_mockValidatioResult.Object);

            var result = await sut.Add(playerToAdd);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        [Trait("Method", "Get")]
        public async Task Given_There_Is_Players_Chart_GET_Returns_Them()
        {
            var playerChart = new List<Player> { new Player { Id = 100 } };
            _mockDepthChartService.Setup(x => x.GetChart()).ReturnsAsync(playerChart);

            var result = await sut.Get();
            var actual = (List<Player>)result;

            Assert.IsAssignableFrom<IEnumerable<Player>>(result);
            Assert.Equal(playerChart, actual);
        }

        [Fact]
        [Trait("Method", "GetPlayerUnder")]
        public async Task Given_There_Are_Players_Under_Chart_GetPlayerUnder_Returns_Them()
        {
            var playerChart = new List<Player> { new Player { Id = 100 } };
            _mockDepthChartService.Setup(x => x.GetPlayersUnderPlayer(It.IsAny<Player>()))
                                .ReturnsAsync(playerChart);

            var result = await sut.GetPlayerUnder("NFL", "WR", 2);
            var actual = (List<Player>)result;

            Assert.IsAssignableFrom<IEnumerable<Player>>(result);
            Assert.Equal(playerChart, actual);
        }

        [Fact]
        [Trait("Method", "Remove")]
        public async Task Given_Model_Is_Invalid_Then_Remove_Method_Returns_400()
        {
            _mockValidatioResult.Setup(x => x.IsValid).Returns(false);
            _mockValidationService.Setup(x => x.Validate(null)).Returns(_mockValidatioResult.Object);

            var result = await sut.Remove(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        [Trait("Method", "Remove")]
        public async Task Given_Model_Is_Valid_Then_Remove_Method_Returns_204()
        {
            var playerToRemove = new Player { Id = 100 };
            _mockValidatioResult.Setup(x => x.IsValid).Returns(true);
            _mockValidationService.Setup(x => x.Validate(playerToRemove)).Returns(_mockValidatioResult.Object);

            var result = await sut.Remove(playerToRemove);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
