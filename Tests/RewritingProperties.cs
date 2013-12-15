using System;
using NUnit.Framework;

[TestFixture]
public class RewritingProperties
{
    private Type sampleClassType;
    private Type classWithPrivateMethodType;

    public RewritingProperties()
    {
        sampleClassType = AssemblyWeaver.Assembly.GetType("SimpleClass");
        classWithPrivateMethodType = AssemblyWeaver.Assembly.GetType("ClassWithPrivateMethod");
    }

    [Test]
    public void PropertySetterRequiresNonEmptyArgument()
    {
        AssemblyWeaver.TestListener.Reset();
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        var exception = Assert.Throws<ArgumentException>(() => { sample.NonEmptyProperty = string.Empty; });
        Assert.AreEqual("value", exception.ParamName);
        Assert.AreEqual("[EmptyStringGuard] Cannot set the value of property 'System.String SimpleClass::NonEmptyProperty()' to an empty string.\r\nParameter name: value", exception.Message);
    }

    [Test]
    public void DoesNotRequireNonEmptyStringSetterWhenPropertiesExcludedByAttribute()
    {
        var sample = (dynamic)Activator.CreateInstance(classWithPrivateMethodType);
        sample.SomeProperty = string.Empty;
    }
}