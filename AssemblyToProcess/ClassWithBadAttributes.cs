using EmptyStringGuard;

public abstract class ClassWithBadAttributes
{
    public abstract void MethodWithEmptyStringCheckOnParam([AllowEmpty] string arg);
}