using EmptyStringGuard;

internal interface InterfaceBadAttributes
{
    void MethodWithEmptyStringCheckOnParam([AllowEmpty] string arg);
}