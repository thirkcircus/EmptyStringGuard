using System;
using NUnit.Framework;

[TestFixture]
public class RewritingMethods
{
    private Type sampleClassType;
    private Type classWithPrivateMethodType;
    private Type specialClassType;

    public RewritingMethods()
    {
        sampleClassType = AssemblyWeaver.Assembly.GetType("SimpleClass");
        classWithPrivateMethodType = AssemblyWeaver.Assembly.GetType("ClassWithPrivateMethod");
        specialClassType = AssemblyWeaver.Assembly.GetType("SpecialClass");
    }

    [Test]
    public void RequiresNonEmptyArgument()
    {
        AssemblyWeaver.TestListener.Reset();
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        var exception = Assert.Throws<ArgumentException>(() => sample.SomeMethod(string.Empty, string.Empty));
        Assert.AreEqual("nonEmptyArg", exception.ParamName);
    }

    [Test]
    public void AllowsEmptyStringWhenAttributeApplied()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        sample.SomeMethod("foobar", string.Empty);
    }

    [Test]
    public void DoesNotRequireNonEmptyForNonPublicMethod()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        sample.PublicWrapperOfPrivateMethod();
    }

    [Test]
    public void RequiresNonEmptyForNonPublicMethodWhenAttributeSpecifiesNonPublic()
    {
        AssemblyWeaver.TestListener.Reset();
        var sample = (dynamic)Activator.CreateInstance(classWithPrivateMethodType);
        Assert.Throws<ArgumentException>(() => sample.PublicWrapperOfPrivateMethod());
    }

#if (DEBUG)

    [Test]
    public void RequiresNonEmptyArgumentAsync()
    {
        AssemblyWeaver.TestListener.Reset();
        var sample = (dynamic)Activator.CreateInstance(specialClassType);
        var exception = Assert.Throws<ArgumentException>(() => sample.SomeMethodAsync(string.Empty, string.Empty));
        Assert.AreEqual("nonEmptyArg", exception.ParamName);
        Assert.AreEqual("[EmptyStringGuard] nonEmptyArg is an empty string.\r\nParameter name: nonEmptyArg", exception.Message);
    }

    [Test]
    public void AllowsEmptyStringWhenAttributeAppliedAsync()
    {
        var sample = (dynamic)Activator.CreateInstance(specialClassType);
        sample.SomeMethodAsync("foobar", string.Empty);
    }

#endif
}