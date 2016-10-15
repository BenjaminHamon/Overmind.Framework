using Overmind.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Overmind.Core.Reflection
{
	/// <summary>
	/// <para>Compares objects using reflection.</para>
	/// <para>
	/// Objects are compared recursively, checking all their properties using reflection.
	/// The recursion stops on basic types which can be compared directly using Object.Equals.
	/// Collections are compared using IEnumerable{}.SequenceEqual.
	/// </para>
	/// </summary>
	public class ReflectionComparer : IEqualityComparer<object>
	{
		/// <summary></summary>
		/// <param name="checkTypeEquality">If true, object types must be the same to be equal.</param>
		public ReflectionComparer(bool checkTypeEquality = true)
		{
			this.checkTypeEquality = checkTypeEquality;
		}

		private readonly bool checkTypeEquality;

		/// <summary>Determines whether the specified objects are equal.</summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns>True if the specified objects are equal; otherwise, false.</returns>
		bool IEqualityComparer<object>.Equals(object first, object second)
		{
			if ((first == null) || (second == null))
				return first == second;

			Type type = first.GetType();
			if (checkTypeEquality && (type != second.GetType()))
				return false;

			if (ShouldUseObjectMethods(first))
				return first.Equals(second);

			if (first is IEnumerable)
			{
				IEnumerable<object> firstAsEnumerable = ((IEnumerable)first).Cast<object>();
				IEnumerable<object> secondAsEnumerable = ((IEnumerable)second).Cast<object>();
				return firstAsEnumerable.SequenceEqual(secondAsEnumerable, this);
			}

			foreach (PropertyInfo property in type.GetProperties())
			{
				object firstValue = property.GetValue(first, null);
				object secondValue = checkTypeEquality ? property.GetValue(second, null)
					: second.GetType().GetProperty(property.Name).GetValue(second, null);

				if (((IEqualityComparer<object>)this).Equals(firstValue, secondValue) == false)
					return false;
			}

			return true;
		}

		/// <summary>Returns a hash code for the specified object.</summary>
		/// <param name="obj">The System.Object for which a hash code is to be returned.</param>
		/// <returns>A hash code for the specified object.</returns>
		int IEqualityComparer<object>.GetHashCode(object obj)
		{
			return obj.GetHashCode();

			// Commented because not tested and not used.

			//if (obj == null)
			//    return 0;

			//if (ShouldUseObjectMethods(obj))
			//    return obj.GetHashCode();

			//unchecked
			//{
			//    int hash = 17;
			//    foreach (PropertyInfo property in obj.GetType().GetProperties())
			//    {
			//        object value = property.GetValue(obj, null);
			//        if (value != null)
			//            hash = hash * 23 + ((IEqualityComparer<object>)this).GetHashCode(value);
			//    }
			//    return hash;
			//}
		}

		/// <summary>Gets an object complete representation as a string, with all its properties.</summary>
		/// <param name="obj">The object to describe.</param>
		/// <param name="byteArrayEncoding">Optional encoding to decode byte arrays, if null write them using hexadecimal.</param>
		/// <param name="typeFullName">If true, uses the fully qualified name for types, otherwise just the name.</param>
		/// <returns>The object represented as a string.</returns>
		public string ToFullString(object obj, Encoding byteArrayEncoding = null, bool typeFullName = false)
		{
			if (obj == null)
				return "";

			if (obj is DateTime)
				return ((DateTime)obj).ToString("o");

			if (ShouldUseObjectMethods(obj))
				return obj.ToString();

			if (obj is byte[])
			{
				if (byteArrayEncoding == null)
					return ByteExtensions.ToHexString((byte[])obj);
				return byteArrayEncoding.GetString((byte[])obj);
			}

			if (obj is IEnumerable)
				return String.Format("[{0}]", String.Join(", ", ((IEnumerable)obj).Cast<object>()
					.Select<object, string>(child => ToFullString(child, byteArrayEncoding, typeFullName)).ToArray()));

			Type type = obj.GetType();
			IEnumerable<string> properties = type.GetProperties().Select(property => String.Format("{0}: {1}",
				property.Name, ToFullString(property.GetValue(obj, null), byteArrayEncoding, typeFullName)));
			return String.Format("{0} ({1})", typeFullName ? type.FullName : type.Name, String.Join(", ", properties.ToArray()));
		}

		private bool ShouldUseObjectMethods(object obj)
		{
			Type type = obj.GetType();
			return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(DateTime);
		}
	}
}