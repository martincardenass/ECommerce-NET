using ECommerce_NET.Dto;
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

        [Authorize(Roles = "user, admin")]
        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Item>))]
        public async Task<IActionResult> GetItems()
        {
            var items = await _itemService.GetItems();

            return Ok(items);
        }

        [Authorize(Roles = "user, admin")]
        [HttpGet("{itemId}")]
        [ProducesResponseType(200, Type = typeof(Item))]
        public async Task<IActionResult> GetItemById(int itemId)
        {
            var item = await _itemService.GetItemById(itemId);

            return Ok(item);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("new")]
        [ProducesResponseType(200, Type = typeof(Item))]
        public async Task<IActionResult> CreateNewItem([FromForm] Item item, [FromForm] List<IFormFile?> images, [FromForm] List<ItemVariant?> variants)
        {
            var (newItem, imageCollection, variantsCollection) = await _itemService.NewItem(item, images, variants);

            var itemDto = new NewItemDto
            {
                Item_Id = newItem.Item_Id,
                Item_Name = newItem.Item_Name,
                Item_Description = newItem.Item_Description,
                Added = newItem.Added,
                Category_Id = newItem.Category_Id,
                Images = imageCollection,
                Variants = variantsCollection
            };

            return Ok(itemDto);
        }
    }
}
