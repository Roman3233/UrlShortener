namespace UrlShortener.Core.Exceptions;

public class DuplicateUrlException : Exception
{
    public DuplicateUrlException(string originalUrl)
        : base($"URL '{originalUrl}' has already been shortened.") { }
}

public class ShortUrlNotFoundException : Exception
{
    public ShortUrlNotFoundException(int id)
        : base($"ShortUrl with id {id} was not found.") { }
}

public class ForbiddenDeleteException : Exception
{
    public ForbiddenDeleteException()
        : base("You do not have permission to delete this URL.") { }
}