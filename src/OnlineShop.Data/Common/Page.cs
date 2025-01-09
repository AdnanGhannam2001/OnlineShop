namespace OnlineShop.Data.Common;

public record Page<T>(int Total, IEnumerable<T> Items);