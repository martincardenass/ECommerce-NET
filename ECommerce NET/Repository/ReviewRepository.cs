using ECommerce_NET.Data;
using ECommerce_NET.Dto;
using ECommerce_NET.Interfaces;
using ECommerce_NET.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce_NET.Repository
{
    public class ReviewRepository : IReview
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteReview(Review review)
        {
            _context.Remove(review);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DoesReviewExist(int reviewId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.Review_Id.Equals(reviewId));
        }

        public async Task<ICollection<Review>> GetItemReviews(int itemId)
        {
            return await _context.Reviews
                .Where(i => i.Item_Id.Equals(itemId))
                .ToListAsync();
        }

        public async Task<Review> GetReviewById(int reviewId)
        {
            return await _context.Reviews
                .FindAsync(reviewId);
        }

        public async Task<ReviewDto> GetReviewDtoById(int reviewId)
        {
            var review = await _context.Reviews
                .FindAsync(reviewId);

            var reviewDto = new ReviewDto
            {
                Review_Id = review.Review_Id,
                Review_Title = review.Review_Title,
                Review_Content = review.Review_Content,
                Modified = review.Modified
            };

            return reviewDto;
        }

        public async Task<Review> NewReview(Review review, int userId, int itemId)
        {
            var newReview = new Review
            {
                Review_Title = review.Review_Title,
                Review_Content = review.Review_Content,
                Added = DateTimeOffset.UtcNow,
                User_Id = userId,
                Item_Id = itemId
            };

            _context.Add(newReview);

            _ = await _context.SaveChangesAsync();

            return newReview;
        }

        public async Task<ReviewDto> UpdateReview(int reviewId, ReviewDto review)
        {
            var existingReview = await GetReviewById(reviewId);

            if (existingReview is not null)
            {
                var type = typeof(ReviewDto);
                var props = type.GetProperties();

                foreach (var prop in props)
                {
                    // Skip unnecessary int iteration
                    if (prop.PropertyType.Equals(typeof(int)))
                    {
                        continue;
                    }

                    // Get every different property on every iteration
                    var eachProp = existingReview.GetType().GetProperty(prop.Name);

                    var newValue = prop.GetValue(review);
                    // Change the prop value if a value was provided
                    if (newValue is not null)
                    {
                        eachProp.SetValue(existingReview, newValue);
                    }
                }

                // Both of this method works fine. With no difference whatsover in performance. Might try later with way more values.

                // * Iteration
                // First > 0.1967377
                // Second > 0.0122666
                // Third > 0.0060339

                // * Directly updating properties
                // First > 0.1880584
                // Second > 0.107387
                // Third > 0.0052554

                //existingReview.Review_Title = review.Review_Title ?? existingReview.Review_Title;
                //existingReview.Review_Content = review.Review_Content ?? existingReview.Review_Content;
                //existingReview.Modified = DateTimeOffset.UtcNow;

                _context.Update(existingReview);
                await _context.SaveChangesAsync();
            }

            // Mapping all together to see the updated properties + the old ones
            var reviewUpdatedToReturn = new ReviewDto
            {
                Review_Id = existingReview.Review_Id,
                Review_Title = existingReview.Review_Title,
                Review_Content = existingReview.Review_Content,
                Modified = existingReview.Modified,
            };

            return reviewUpdatedToReturn;
        }
    }
}
