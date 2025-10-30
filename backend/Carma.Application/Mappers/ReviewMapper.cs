using Carma.Application.DTOs.Review;
using Carma.Domain.Entities;

namespace Carma.Application.Mappers;

public static class ReviewMapper
{
    public static Review MapToReview(ReviewCreateDto reviewCreateDto)
    {
        return new Review
        {
            RideId = reviewCreateDto.RideId,
            Karma = reviewCreateDto.Karma,
            Text = reviewCreateDto.Text,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static ReviewGetDto MapToGetDto(Review review)
    {
        return new ReviewGetDto(
            UserMapper.MapToUserSummaryDto(review.Reviewer),
            review.RideId,
            review.Karma,
            review.Text
            );
    }
}