using System;

namespace EmptyStringGuard
{
    /// <summary>
    /// Prevents the injection of empty string checking.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class AllowEmptyAttribute : Attribute
    {
    }

    /// <summary>
    /// Prevents the injection of empty string checking.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class AllowEmptyStringAttribute : Attribute
    {
    }
}