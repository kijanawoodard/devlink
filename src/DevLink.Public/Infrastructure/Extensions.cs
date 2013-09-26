using System;

namespace DevLink.Public.Infrastructure
{
	public static class Extensions
	{
		public static TResult DefaultIfNull<T, TResult>(this T foo, Func<T, TResult> func, TResult defaultValue)
			where T : class
		{
			return foo == null ? defaultValue : func(foo);
		}
	}
}