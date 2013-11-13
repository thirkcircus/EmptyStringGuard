using System;

namespace EmptyStringGuard
{
    /// <summary>
    /// Used by <see cref="EmptyStringGuardAttribute"/> to target specific categories of members.
    /// </summary>
    [Flags]
    public enum ValidationFlags
    {
        /// <summary>
        /// Don't process anything.
        /// </summary>
        None = 0,

        /// <summary>
        /// Process properties.
        /// </summary>
        Properties = 1,

        /// <summary>
        /// Process arguments of methods.
        /// </summary>
        Methods = 2,

        /// <summary>
        /// Process non-public members.
        /// </summary>
        NonPublic = 4,

        /// <summary>
        /// Process public properties, and arguments of public methods.
        /// </summary>
        AllPublic = Properties | Methods,

        /// <summary>
        /// Process all properties, and arguments of all methods.
        /// </summary>
        All = AllPublic | NonPublic
    }
}