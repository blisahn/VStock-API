using Microsoft.AspNetCore.Mvc;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Presentation.Models;

namespace VBorsa_API.Presentation.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result) =>
            result.Succeeded
                ? new OkObjectResult(new ApiResponse<T>
                    { Succeeded = true, Message = result.Message, Data = result.Data })
                : new BadRequestObjectResult(new ApiResponse<T> { Succeeded = false, Errors = result.Errors });

        public static IActionResult ToActionResult(this Result result) =>
            result.Succeeded
                ? new OkObjectResult(new ApiResponse<object> { Succeeded = true, Message = result.Message })
                : new BadRequestObjectResult(new ApiResponse<object> { Succeeded = false, Errors = result.Errors });
    }
}