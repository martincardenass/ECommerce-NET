using ECommerce_NET.Dto;
using ECommerce_NET.Interfaces;
using ECommerce_NET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce_NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReview _reviewService;
        private readonly IItem _itemService;

        public ReviewController(IReview reviewService, IItem itemService)
        {
            _reviewService = reviewService;
            _itemService = itemService;
        }

        [Authorize(Roles = "user, admin")]
        [HttpPost("new")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> NewReview([FromForm] Review review, int itemId)
        {
            bool itemExists = await _itemService.DoesItemExist(itemId);

            if(!itemExists)
            {
                return NotFound();
            }

            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _ = int.TryParse(userIdClaim, out int userId);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var newReview = await _reviewService.NewReview(review, userId, itemId);

            return Ok(newReview);
        }

        [Authorize(Roles = "user, admin")]
        [HttpPatch("{reviewId}/update")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromForm] ReviewDto review)
        {
            bool reviewExists = await _reviewService.DoesReviewExist(reviewId);

            if(!reviewExists)
            {
                return NotFound();
            }

            if(!TryValidateModel(review))
            {
                return BadRequest(ModelState);
            }

            var updatedReview = await _reviewService.UpdateReview(reviewId, review);

            return Ok(updatedReview);
        }
    }
}
