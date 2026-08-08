namespace Catalog.Api.Books.DeleteBook;

public record DeleteBookCommand(Guid Id) : ICommand<DeleteBookResult>;

public class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
{
    public DeleteBookCommandValidator()
    {
        RuleFor(item => item.Id).NotEmpty().WithMessage("Id не может быть пустым");
    }
}

public record DeleteBookResult(bool IsSuccess);

public class DeleteBookCommandHandler(IDocumentSession session)
    : ICommandHandler<DeleteBookCommand, DeleteBookResult>
{
    public async Task<DeleteBookResult> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        var book = await session.LoadAsync<Book>(command.Id, cancellationToken);

        if (book is null)
        {
            // throw new BookNotFoundException(command.Id);
            return new DeleteBookResult(false);
        }

        // command.Adapt(book.Id);
        session.Delete<Book>(command.Id);
        await session.SaveChangesAsync(cancellationToken);

        return new DeleteBookResult(true);
    }
}