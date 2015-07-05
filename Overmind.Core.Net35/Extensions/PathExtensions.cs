using System;
using System.IO;
using System.Linq;

namespace Overmind.Core.Extensions
{
	public static class PathExtensions
	{
		public static string Combine(params string[] pathCollection)
		{
			return pathCollection.Aggregate((firstPath, secondPath) => Path.Combine(firstPath, secondPath));
		}
	}
}
