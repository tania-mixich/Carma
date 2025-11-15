using Carma.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Carma.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result);
        }

        return result.ErrorType switch
        {
            ErrorType.Validation => new BadRequestObjectResult(result.Error),
            ErrorType.NotFound => new NotFoundObjectResult(result.Error),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(result.Error),
            ErrorType.Conflict => new ConflictObjectResult(result.Error),
            _ => new BadRequestObjectResult(result.Error)
        };
    }
    
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }
        
        return result.ErrorType switch
        {
            ErrorType.Validation => new BadRequestObjectResult(result.Error),
            ErrorType.NotFound => new NotFoundObjectResult(result.Error),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(result.Error),
            ErrorType.Conflict => new ConflictObjectResult(result.Error),
            _ => new BadRequestObjectResult(result.Error)
        };
    }
}