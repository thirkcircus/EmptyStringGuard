using EmptyStringGuard;

[EmptyStringGuard(ValidationFlags.NonPublic | ValidationFlags.Methods)]
public class ClassWithPrivateMethod
{
    public void PublicWrapperOfPrivateMethod()
    {
        SomePrivateMethod(string.Empty);
    }

    // ReSharper disable UnusedParameter.Local
    private void SomePrivateMethod(string x)
    // ReSharper restore UnusedParameter.Local
    {
    }

    public string SomeProperty { get; set; }
}