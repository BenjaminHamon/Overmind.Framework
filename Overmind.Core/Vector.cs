using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Overmind.Core
{
	public class Vector : IReadOnlyList<double>
	{
		#region Constructors

		public Vector(params double[] coordinates)
			: this((IEnumerable<double>)coordinates)
		{ }

		public Vector(IEnumerable<double> coordinates)
		{
			this.Coordinates = coordinates.ToImmutableList();
		}

		#endregion

		private readonly ImmutableList<double> Coordinates;

		public override string ToString()
		{
			return Coordinates.ToCollectionString();
		}

		#region IReadOnlyList

		public double this[int index] { get { return Coordinates[index]; } }
		public int Count { get { return Coordinates.Count; } }
		public IEnumerator<double> GetEnumerator() { return Coordinates.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return Coordinates.GetEnumerator(); }

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			Vector other = obj as Vector;
			if (other == null)
				return false;

			return this.SequenceEqual(other);
		}

		public double Norm()
		{
			return Math.Sqrt(Coordinates.Aggregate<double, double>(0, (accumulator, next) => accumulator + next * next));
		}

		/// <summary>
		/// Creates a normalized vector from this vector.
		/// </summary>
		/// <returns></returns>
		public Vector Normalize()
		{
			return this / Norm();
		}

		#region Operators

		public static bool operator==(Vector firstVector, Vector secondVector)
		{
			if (Object.ReferenceEquals(firstVector, secondVector))
				return true;

			if ((object)firstVector == null)
				return false;

			return firstVector.Equals(secondVector);
		}

		public static bool operator!=(Vector firstVector, Vector secondVector)
		{
			return (firstVector == secondVector) == false;
		}

		public static Vector operator+(Vector firstVector, Vector secondVector)
		{
			if (firstVector.Count != secondVector.Count)
				throw new InvalidOperationException("Vector dimensions do not match");

			return new Vector(firstVector.Zip(secondVector, (first, second) => first + second));
		}

		public static Vector operator-(Vector vector)
		{
			return new Vector(vector.Select(x => -x));
		}

		public static Vector operator-(Vector firstVector, Vector secondVector)
		{
			if (firstVector.Count != secondVector.Count)
				throw new InvalidOperationException("Vector dimensions do not match");

			return new Vector(firstVector.Zip(secondVector, (first, second) => first - second));
		}

		public static Vector operator*(Vector vector, double multiplier)
		{
			return new Vector(vector.Select(x => x * multiplier));
		}

		public static Vector operator/(Vector vector, double divisor)
		{
			return new Vector(vector.Select(x => x / divisor));
		}

		#endregion
	}
}
