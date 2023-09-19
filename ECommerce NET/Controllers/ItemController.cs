using ECommerce_NET.Interfaces;
using ECommerce_NET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItem _itemService;

        public ItemController(IItem itemService)
        {
            _itemService = itemService;
        }

        [Authorize(Roles = "user")]
        [HttpGet("items")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Item>))]
        public async Task<IActionResult> GetItems()
        {
            var items = await _itemService.GetItems();

            return Ok(items);
        }
    }
}
