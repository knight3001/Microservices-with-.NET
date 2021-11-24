using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemsRepository;
        private readonly CatalogClient catalogoClient;

        public ItemsController(IRepository<InventoryItem> itemsRepository, CatalogClient catalogoClient)
        {
            this.itemsRepository = itemsRepository;
            this.catalogoClient = catalogoClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsyn(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var catalogItems = await catalogoClient.GetCatalogItemAsnc();
            var inventoryItemEntities = await itemsRepository.GetAllAsync(item => item.UserId == userId);

            var inventoryItemsDtos = inventoryItemEntities.Select(item => {
                var catalogItem = catalogItems.Single(c => c.Id == item.CatelogItemId);
                return item.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemsDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await itemsRepository.GetByIdAsync(
                item => item.UserId == grantItemsDto.UserId && item.CatelogItemId == grantItemsDto.CatalogItemId
            );

            if(inventoryItem == null)
            {
                inventoryItem = new InventoryItem{
                    CatelogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await itemsRepository.CreateAsync(inventoryItem);
            }
            else{
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await itemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }

    }
}