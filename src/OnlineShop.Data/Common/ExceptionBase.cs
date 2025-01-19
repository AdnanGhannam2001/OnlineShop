using System.Text;

namespace OnlineShop.Data.Common;

public class ExceptionBase : Exception
{
    public ExceptionBase(string propertyName, string errorMessage, string? errorCode = null) : base(errorMessage) {
        PropertyName = propertyName;
        ErrorCode = errorCode;
    }

    public string PropertyName { get; init; }
    public string? ErrorCode { get; init; }

    public override string ToString() {
        var builder = new StringBuilder();

        if (ErrorCode is not null) {
            builder.Append($"Error Code: {ErrorCode}, ");
        }

        builder.Append($"Property: {PropertyName}, ");
        builder.Append($"Error Message: {Message}");

        return builder.ToString();
    }
}