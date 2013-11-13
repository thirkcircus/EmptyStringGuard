using NUnit.Framework;

[TestFixture]
public class ErrorsTests
{
    [Test]
    public void ErrorsForAbstract()
    {
        Assert.Contains("Method 'System.Void ClassWithBadAttributes::MethodWithEmptyStringCheckOnParam(System.String)' is abstract but has an [AllowEmptyAttribute]. Remove this attribute.", AssemblyWeaver.Errors);
        Assert.Contains("Method 'System.Void InterfaceBadAttributes::MethodWithEmptyStringCheckOnParam(System.String)' is abstract but has an [AllowEmptyAttribute]. Remove this attribute.", AssemblyWeaver.Errors);
    }
}