using System;

namespace ExternalPropertyAttributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ShowNativePropertyAttribute : SpecialCaseDrawerAttribute
	{
	}
}
