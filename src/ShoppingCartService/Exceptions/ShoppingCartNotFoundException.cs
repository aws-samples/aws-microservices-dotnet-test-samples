namespace ShoppingCartService.Exceptions;

public class ShoppingCartNotFoundException : Exception
{
    public ShoppingCartNotFoundException()
    {
    }

    public ShoppingCartNotFoundException(string message) : base(message)
    {
    }

    public ShoppingCartNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }
}