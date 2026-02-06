namespace Settlr.Common.Helper;

public static class ValidationConstants
{
    public const int NameMaxLength = 150;
    public const int EmailMaxLength = 256;
    public const int PasswordMinLength = 6;
    public const int PasswordMaxLength = 100;
    public const int GroupNameMaxLength = 150;
    public const int ExpenseDescriptionMaxLength = 500;

    public const string MinAmount = "0.01";
    public const string MaxAmount = "1000000000";
}
