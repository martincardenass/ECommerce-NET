using ECommerce_NET.Dto;
using ECommerce_NET.Models;

namespace ECommerce_NET.Interfaces
{
    public interface IReview
    {
        Task<Review> NewReview(Review review, int userId, int itemId);
        Task<ICollection<Review>> GetItemReviews(int itemId);
        Task<Review> GetReviewById(int reviewId);
        Task<ReviewDto> GetReviewDtoById(int reviewId);
        Task<ReviewDto> UpdateReview(int reviewId, ReviewDto review);
        Task<bool> DeleteReview(Review review);
        Task<bool> DoesReviewExist(int reviewId);
    }
}
