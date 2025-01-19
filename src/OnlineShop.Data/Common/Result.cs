using System.Text;

namespace OnlineShop.Data.Common;

public class Result<V, E> : IEquatable<Result<V, E>>
    where V : notnull
    where E : ExceptionBase
{
    private readonly V? _value;
    private readonly E[]? _exceptions;

    public V Value {
        get {
            if (_value is null)
                throw new InvalidOperationException($"Can't access '{nameof(Value)}' when {nameof(IsSuccess)} is false");

            return _value!;
        }
        init {
            _value = value;
        }
    }

    public E[] Exceptions {
        get {
            if (_exceptions is null)
                throw new InvalidOperationException($"Can't access '{nameof(Exceptions)}' when {nameof(IsSuccess)} is true");

            return _exceptions!;
        }
        init {
            _exceptions = value;
        }
    }

    public Result(V value) => Value = value;
    public Result(params E[] exceptions) => Exceptions = exceptions;

    public static implicit operator Result<V, E>(V value) {
        return new Result<V, E>(value);
    }

    public static implicit operator Result<V, E>(E[] exceptions) {
        return new Result<V, E>(exceptions);
    }

    public static implicit operator Result<V, E>(E exception) {
        return new Result<V, E>([exception]);
    }

    public override bool Equals(object? obj) {
        if (obj is Result<V, E> result) {
            bool valueEquality = (_value is null && result._value is null)
                || (result._value is not null
                    && _value?.Equals(result._value) == true);
            bool exceptionsEquality = (_exceptions is null && result._exceptions is null)
                || (result._exceptions is not null
                    && _exceptions?.SequenceEqual(result._exceptions) == true);

            return valueEquality && exceptionsEquality;
        }

        return false;
    }

    public bool Equals(Result<V, E>? other) => other is not null && Equals(other as object);

    public static bool operator ==(Result<V, E> a, Result<V, E> b) => a.Equals(b);
    public static bool operator !=(Result<V, E> a, Result<V, E> b) => !a.Equals(b);

    public static bool operator ==(Result<V, E> a, bool b) => a.IsSuccess == b;
    public static bool operator !=(Result<V, E> a, bool b) => a.IsSuccess != b;

    public static explicit operator bool(Result<V, E> a) => a.IsSuccess;

    public override int GetHashCode() => base.GetHashCode() ^ 11;

    public bool IsSuccess => _value is not null && _exceptions is null;

    public override string? ToString()
    {
        if (IsSuccess) {
            return Value!.ToString();
        } 

        var builder = new StringBuilder();

        builder.AppendLine("List of Exceptions:");

        foreach (var exception in Exceptions) {
            builder.Append(exception.ToString()).AppendLine();
        }

        return builder.ToString();
    }
}

public class Result<V>
    : Result<V, ExceptionBase>
        where V : notnull
{
    public Result(V value) : base(value) { }
    public Result(params ExceptionBase[] exceptions) : base(exceptions) { }

    public static implicit operator Result<V>(V value) {
        return new Result<V>(value);
    }

    public static implicit operator Result<V>(ExceptionBase[] exceptions) {
        return new Result<V>(exceptions);
    }

    public static implicit operator Result<V>(ExceptionBase exception) {
        return new Result<V>([exception]);
    }
}