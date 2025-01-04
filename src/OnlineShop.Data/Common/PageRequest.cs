namespace OnlineShop.Data.Common;

public record PageRequest(int Size, int Number, bool Desc = true);