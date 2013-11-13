using System;

namespace EmptyStringGuard
{
    /// <summary>
    /// Allow specific categories of members to be targeted for injection. <seealso cref="ValidationFlags"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
    public class EmptyStringGuardAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyStringGuardAttribute"/> with a <see cref="ValidationFlags"/>.
        /// </summary>
        /// <param name="flags">The <see cref="ValidationFlags"/> to use for the target this attribute is being applied to.</param>
        public EmptyStringGuardAttribute(ValidationFlags flags)
        {
        }
    }
}