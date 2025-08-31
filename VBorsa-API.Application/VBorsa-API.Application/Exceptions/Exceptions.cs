namespace VBorsa_API.Application.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string? message) : base(message)
    {
    }
}

public class LoginAuthenticationException : Exception
{
    public LoginAuthenticationException(string? message) : base(message)
    {
    }
}

public class RegisterException : Exception
{
    public RegisterException(string? message) : base(message)
    {
    }
}

public class SymbolAlreadyExistsException : Exception
{
    public SymbolAlreadyExistsException(string? message) : base(message)
    {
    }
}

public class SymbolCreationFailedExpceion : Exception
{
    public SymbolCreationFailedExpceion(string? message) : base(message)
    {
    }
}

public class MarketListFailedException : Exception
{
    public MarketListFailedException(string? message) : base(message)
    {
    }
}

public class SymbolNotExistsException : Exception
{
    public SymbolNotExistsException(string? message) : base(message)
    {
    }
}

public class ValidationException : Exception
{
    public ValidationException(string? message, IDictionary<string, string[]> errors) : base(message)
    {
        ValidationErrors = errors;
    }

    public IDictionary<string, string[]> ValidationErrors { get; }
}

public class AssetCreationExceptıon : Exception
{
    public AssetCreationExceptıon(string? message) : base(message)
    {
    }
}

public class TransactionException : Exception
{
    public TransactionException(string? message) : base(message)
    {
    }
}

public class RefreshTokenNotValidException : Exception
{
    public RefreshTokenNotValidException(string? message) : base(message)
    {
    }
}

public class BinanceSocketException : Exception
{
    public BinanceSocketException(string? message) : base(message)
    {
    }
}

public class TransactionCreationFailedException : Exception
{
    public TransactionCreationFailedException(string? message) : base(message)
    {
    }
}

public class AuthenticationException : Exception
{
    public AuthenticationException(string? message) : base(message)
    {
    }
}

public class UpdateException : Exception
{
    public UpdateException(string? message) : base(message)
    {
    }
}

public class AssignRoleException : Exception
{
    public AssignRoleException(string? message) : base(message)
    {
    }
}

public class UserDeleteException : Exception
{
    public UserDeleteException(string? message) : base(message)
    {
    }
}

public class UserCreationFailedException : Exception
{
    public UserCreationFailedException(string? message) : base(message)
    {
    }
}