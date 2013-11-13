using EmptyStringGuard;

public class Sample
{
    public void SomeMethod(string arg)
    {
        // throws ArgumentException if arg is empty string.
    }

    public void AnotherMethod([AllowEmpty] string arg)
    {
        // arg may be empty string here
    }

    // Empty string checking works for automatic properties too.
    public string SomeProperty { get; set; }

    // can be applied to a whole property
    [AllowEmpty] 
    public string EmptyStringProperty { get; set; }
}