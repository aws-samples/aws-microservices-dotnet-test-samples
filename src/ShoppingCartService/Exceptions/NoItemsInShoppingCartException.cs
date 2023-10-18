namespace ShoppingCartService.Exceptions;

public class NoItemsInShoppingCartException : Exception
{
    public NoItemsInShoppingCartException()
    {
    }

    public NoItemsInShoppingCartException(string message) : base(message)
    {
    }

    public NoItemsInShoppingCartException(string message, Exception inner) : base(message, inner)
    {
    }
}