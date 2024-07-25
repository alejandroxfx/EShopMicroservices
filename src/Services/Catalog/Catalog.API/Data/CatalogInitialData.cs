using Marten.Schema;
using System.Collections.Generic;

namespace Catalog.API.Data
{
    public class CatalogInitialData : IInitialData
    {
        public async Task Populate(IDocumentStore store, CancellationToken cancellation)
        {
            using var session = store.LightweightSession();
            
            if (await session.Query<Product>().AnyAsync())
                return;

            session.Store<Product>(GetPreconfiguredProducts());
            await session.SaveChangesAsync();

        }

        private static IEnumerable<Product> GetPreconfiguredProducts()
        {
            List<Product> seedData = new List<Product>();
            for (int i = 1; i <= 10; i++) {
                seedData.Add(new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = $"Iphone {i}",
                    Description = $"The version {i} is awesome!!!",
                    ImageFile = $"product-{i}.png",
                    Price = i+000,
                    Category = ["SmartPhone", "Apples"]
                });
            }
            for (int i = 1; i <= 10; i++)
            {
                seedData.Add(new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = $"Samsung {i}",
                    Description = $"The version {i} is great!!!",
                    ImageFile = $"product-{i}.png",
                    Price = i + 000,
                    Category = ["SmartPhone", "Samsung"]
                });
            }
            for (int i = 1; i <= 10; i++)
            {
                seedData.Add(new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = $"Xiaomi {i}",
                    Description = $"The version {i} is powerful!!!",
                    ImageFile = $"product-{i}.png",
                    Price = i + 000,
                    Category = ["SmartPhone", "Xiaomi"]
                });
            }
            
            return seedData;
        }
    }
}
