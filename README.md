## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

## Nuget 

Nuget package http://nuget.org/packages/EmptyStringGuard.Fody 

To Install from the Nuget Package Manager Console 
    
    PM> Install-Package EmptyStringGuard.Fody

### Your Code

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

### What gets compiled 

    public class SampleOutput
    {

        public string EmptyStringProperty{get;set}
    
        string emptyStringProperty;
        public string EmptyStringProperty
        {
            get
            {
                return emptyStringProperty;
            }
            set
            {
                emptyStringProperty = value;
            }
        }
    
        string someProperty;
        public string SomeProperty
        {
            get
            {
                return someProperty;
            }
            set
            {
                if (string.Empty == value)
                {
                    throw new ArgumentException("value", "Cannot set the value of property 'SomeProperty' to an empty string.");
                }
                someProperty = value;
            }
        }

        public void AnotherMethod(string arg)
        {
        }

        public void SomeMethod(string arg)
        {
            if (string.Empty == arg)
            {
				throw new ArgumentException("arg", "Cannot accept an empty string.");
            }
        }
    }
    
## Attributes

Where and how injection occurs can be controlled via attributes. The EmptyStringGuard.Fody nuget ships with an assembly containing these attributes.

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
        
        /// <summary>
        /// Used by <see cref="EmptyStringGuardAttribute"/> to taget specific categories of members.
        /// </summary>
        [Flags]
        public enum ValidationFlags
        {
            None = 0,
            Properties = 1,
            Arguments = 2,
            OutValues = 4,
            ReturnValues = 8,
            NonPublic = 16,
            Methods = Arguments | OutValues | ReturnValues,
            AllPublicArguments = Properties | Arguments,
            AllPublic = Properties | Methods,
            All = AllPublic | NonPublic
        }
    }


## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)
