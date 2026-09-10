using UrlShortener.Core.Services;
using Xunit;

namespace UrlShortener.Tests;

public class Base62EncoderTests
{
    private readonly Base62Encoder _sut = new();

    [Fact]
    public void Encode_ZeroId_ReturnsFirstAlphabetCharacter()
    {
        var result = _sut.Encode(0);
        Assert.Equal("0", result);
    }

    [Fact]
    public void Decode_KnownCode_ReturnsExpectedId()
    {
        var result = _sut.Decode("3d7");
        Assert.Equal(12345, result);
    }

    [Fact]
    public void EncodeDecode_Roundtrip_PreservesValue()
    {
        var code = _sut.Encode(12345);
        var result = _sut.Decode(code);
        Assert.Equal(12345, result);
    }

    [Fact]
    public void Decode_InvalidCharacter_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => _sut.Decode("abc!"));
        Assert.Contains("!", exception.Message);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(1, "1")]
    [InlineData(61, "Z")]
    [InlineData(62, "10")]
    [InlineData(12345, "3d7")]
    public void Encode_MultipleIds_ReturnsExpectedCodes(int id, string expectedCode)
    {
        var result = _sut.Encode(id);
        Assert.Equal(expectedCode, result);
    }
}