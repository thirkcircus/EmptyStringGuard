using System;
using NUnit.Framework;

[TestFixture]
public class RewritingConstructors
{
    Type sampleClassType;

    public RewritingConstructors()
    {
        sampleClassType = AssemblyWeaver.Assembly.GetType("SimpleClass");
    }

    [Test]
    public void RequiresNonEmptyArgument()
    {
        AssemblyWeaver.TestListener.Reset();
        Assert.That(new TestDelegate(() => Activator.CreateInstance(sampleClassType, string.Empty, string.Empty)),
            Throws.TargetInvocationException
                .With.InnerException.TypeOf<ArgumentException>()
                .And.InnerException.Property("ParamName").EqualTo("nonEmptyArg"));
    }

    [Test]
    public void AllowsEmptyStringWhenAttributeApplied()
    {
        Activator.CreateInstance(sampleClassType, "foobar", string.Empty);
    }
}