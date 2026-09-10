namespace UrlShortener.Core.Interfaces;

public interface IShortCodeGenerator
{
    string Encode(int id);
    int Decode(string code);
}