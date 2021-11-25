using System.Threading.Tasks;
using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemDeletedConsumer : IConsumer<CatelogItemDeleted>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatelogItemDeleted> context)
        {
            var message = context.Message;

            var item = await repository.GetByIdAsync(message.Id);

            if (item == null)
            {
                return;
            }

            await repository.DeleteAsync(message.Id);
        }
    }
}