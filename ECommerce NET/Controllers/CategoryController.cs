using ECommerce_NET.Interfaces;
using ECommerce_NET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategory _categoryService;

        public CategoryController(ICategory categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("new")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateNewCategory([FromForm] Category category)
        {
            int newCategory = await _categoryService.NewCategory(category);

            if(newCategory > 0)
            {
                return Ok(newCategory);
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("/delete/{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var category = await _categoryService.GetCategoryById(categoryId);

            bool result = await _categoryService.DeleteCategory(category);

            if(result)
            {
                return NoContent();
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
        }
    }
}
