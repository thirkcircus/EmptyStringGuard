using System;
using EmptyStringGuard;

public class SimpleClass
{
    public SimpleClass()
    {
    }

    public SimpleClass(string nonEmptyArg, [AllowEmpty] string emptyArg)
    {
        Console.WriteLine(nonEmptyArg + " " + emptyArg);
    }

    public void SomeMethod(string nonEmptyArg, [AllowEmpty] string emptyArg)
    {
        Console.WriteLine(nonEmptyArg);
    }

    public string NonEmptyProperty { get; set; }

    [AllowEmpty]
    public string EmptyProperty { get; set; }


    public void PublicWrapperOfPrivateMethod()
    {
        SomePrivateMethod(string.Empty);
    }

    private void SomePrivateMethod(string x)
    {
        Console.WriteLine(x);
    }

    public void MethodWithTwoRefs(ref string first, ref string second)
    {
    }

    public void MethodWithExistingArgumentGuard(string x)
    {
        if (String.IsNullOrEmpty(x))
            throw new ArgumentException("x is null or empty.", "x");

        Console.WriteLine(x);
    }

    public void MethodWithExistingEmptyStringGuard(string x)
    {
        if (string.Empty == x)
            throw new ArgumentException("x");

        Console.WriteLine(x);
    }

    public void MethodWithExistingEmptyStringGuardReversed(string x)
    {
        if (x == string.Empty)
            throw new ArgumentException("x");

        Console.WriteLine(x);
    }

    public void MethodWithExistingEmptyStringGuardWithMessage(string x)
    {
        if (string.Empty == x)
            throw new ArgumentException("x", "x is empty.");

        Console.WriteLine(x);
    }
}