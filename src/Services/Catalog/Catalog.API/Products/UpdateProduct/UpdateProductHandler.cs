
namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommand(Guid Id,
                                       string Name,
                                       string[] Category,
                                       string Description,
                                       string ImageFile,
                                       decimal Price)
        : ICommand<UpdateProductResult>;

    public record UpdateProductResult(bool IsSuccess);

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator() 
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage("Product ID is required");

            RuleFor(m => m.Name).NotEmpty().WithMessage("Name is required")
                                .Length(2,150).WithMessage("Name must be between 2-150 characters");

            RuleFor(m => m.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }

    internal class UpdateProductCommandHandler
        (IDocumentSession session)
        : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var prooduct = await session.LoadAsync<Product>(command.Id, cancellationToken);
            if (prooduct is null)
            {
                throw new ProductNotFoundException(command.Id);
            }

            prooduct.Name = command.Name;
            prooduct.Category = command.Category;
            prooduct.Description = command.Description;
            prooduct.ImageFile = command.ImageFile;
            prooduct.Price = command.Price;

            session.Update(prooduct);
            await session.SaveChangesAsync(cancellationToken);

            return new UpdateProductResult(true);
        }
    }
}
