using DepthChartApi.Models;
using FluentValidation.Results;

namespace DepthChartApi.Domain
{
    public interface IValidationService
    {
        ValidationResult Validate(Player player);
    }
}
