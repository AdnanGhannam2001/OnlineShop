namespace OnlineShop.Data.Common;

public record Page<T>(int Size, int Total, IEnumerable<T> Items);