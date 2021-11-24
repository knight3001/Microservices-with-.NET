using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> _itemRepository;
        private static int requestCount = 0;

        public ItemsController(IRepository<Item> itemRepository)
        {
            this._itemRepository = itemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            requestCount++;
            Console.WriteLine($"Request {requestCount}: Starting...");

            if(requestCount <= 2){
                 Console.WriteLine($"Request {requestCount}: Delaying...");
                 await Task.Delay(TimeSpan.FromSeconds(10));
            }

            if(requestCount <= 4){
                 Console.WriteLine($"Request {requestCount}: 500 (Internal Server Error)...");
                 return StatusCode(500);
            }

            var items = (await _itemRepository.GetAllAsync())
                        .Select(item => item.AsDto());
            Console.WriteLine($"Request {requestCount}: 200 (OK).");            
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await _itemRepository.GetByIdAsync(id);

            if (item == null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto CreateItemDto)
        {
            var item = new Item
            {
                Name = CreateItemDto.Name,
                Description = CreateItemDto.Description,
                Price = CreateItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await _itemRepository.CreateAsync(item);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdateItemDto updateItemDto)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            item.Name = updateItemDto.Name;
            item.Description = updateItemDto.Description;
            item.Price = updateItemDto.Price;

            await _itemRepository.UpdateAsync(item);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _itemRepository.DeleteAsync(item.Id);
            return NoContent();
        }

    }
}