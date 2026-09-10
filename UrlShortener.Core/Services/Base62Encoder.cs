using UrlShortener.Core.Interfaces;

namespace UrlShortener.Core.Services;

public class Base62Encoder : IShortCodeGenerator
{
    private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const int Base = 62;

    public string Encode(int id)
    {
        if (id < 0)
            throw new ArgumentException("Id must be a positive integer.", nameof(id));
        if (id == 0)
            return Alphabet[0].ToString();

        var chars = new Stack<char>();

        while (id > 0)
        {
            chars.Push(Alphabet[id % Base]);
            id /= Base;
        }

        return new string(chars.ToArray());
    }

    public int Decode(string code)
    {
        if (string.IsNullOrEmpty(code))
            throw new ArgumentException("Code cannot be null or empty.", nameof(code));

        var result = 0;

        foreach (var c in code)
        {
            var digit = Alphabet.IndexOf(c);
            if (digit < 0)
                throw new ArgumentException($"Invalid character '{c}' in short code.", nameof(code));

            result = result * Base + digit;
        }

        return result;
    }
}