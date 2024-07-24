namespace Catalog.API.Products.CreateProduct
{

    public record CreateProductCommand(string Name, 
                                       string[] Category,
                                       string Description,
                                       string ImageFile,
                                       decimal Price) : ICommand<CreateProductResult>;

    public record CreateProductResult(Guid Id);

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(m => m.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(m => m.Category).NotEmpty().WithMessage("Category is required");
            RuleFor(m => m.ImageFile).NotEmpty().WithMessage("ImageFile is required");
            RuleFor(m => m.Price).GreaterThan(0).WithMessage("Price is required");
        }
    }

    internal class CreateProductCommandHandler(IDocumentSession session)
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancelationToken)
        {
            //Create object
            var product = new Product
            {
                Name = command.Name,
                Category = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price
            };

            //Save into DB
            session.Store(product);
            await session.SaveChangesAsync(cancelationToken);

            return new CreateProductResult(product.Id);
        }
    }
}