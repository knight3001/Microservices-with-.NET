using System.Threading.Tasks;
using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemCreatedConsumer : IConsumer<CatelogItemCreated>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatelogItemCreated> context)
        {
            var message = context.Message;

            var item = await repository.GetByIdAsync(message.Id);

            if (item != null)
            {
                return;
            }

            item = new CatalogItem
            {
                Id = message.Id,
                Name = message.Name,
                Description = message.Description,
            };

            await repository.CreateAsync(item);
        }
    }
}