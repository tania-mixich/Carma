using Carma.API.Extensions;
using Carma.Application.DTOs.Review;
using Carma.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carma.API.Controllers;

[ApiController]
[Route("users/{userId}/reviews")]
[Authorize]
public class ReviewController : ControllerBase
{
    private readonly ReviewService _reviewService;

    public ReviewController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReviewsForUser(Guid userId)
    {
        var result = await _reviewService.GetAllForUserAsync(userId);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> AddReview(Guid userId, ReviewCreateDto reviewCreateDto)
    {
        var result = await _reviewService.AddReviewAsync(userId, reviewCreateDto);
        return result.ToActionResult();
    }
}