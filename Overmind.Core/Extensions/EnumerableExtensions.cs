using System;
using System.Collections.Generic;
using System.Linq;

namespace Overmind.Core.Extensions
{
	public static class EnumerableExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">Collection is null.</exception>
		public static string ToCollectionString<T>(this IEnumerable<T> collection)
		{
			return "{" + String.Join(", ", collection.Select(item => item.ToString()).ToArray()) + "}";
		}
	}
}
