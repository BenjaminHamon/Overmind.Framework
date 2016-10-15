using System;

namespace Overmind.Core.Extensions
{
	/// <summary>Extension methods for bytes.</summary>
	public static class ByteExtensions
	{
		/// <summary>Converts a byte array to its hexadecimal string representation.</summary>
		/// <remarks>The returned string is in uppercase and without separators.</remarks>
		/// <exception cref="ArgumentNullException">Thrown if the byte array is null.</exception>
		public static string ToHexString(byte[] byteArray)
		{
			return BitConverter.ToString(byteArray).Replace("-", "");
		}

		/// <summary>Converts a hexadecimal string to a byte arrray.</summary>
		/// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
		/// <exception cref="FormatException">Thrown if the value is not a hexadecimal string.</exception>
		public static byte[] FromHexString(string hexString)
		{
			if (hexString.Length % 2 != 0)
				hexString = "0" + hexString;

			int arrayLength = hexString.Length / 2;
			byte[] byteArray = new byte[arrayLength];
			for (int byteIndex = 0; byteIndex < arrayLength; byteIndex++)
				byteArray[byteIndex] = Convert.ToByte(hexString.Substring(byteIndex * 2, 2), 16);
			return byteArray;
		}
	}
}