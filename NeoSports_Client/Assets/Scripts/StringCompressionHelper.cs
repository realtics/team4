using System;
using System.IO;
using System.IO.Compression;
using System.Text;

public static class StringCompressionHelper
{
	/// <summary>
	/// 압축하기
	/// </summary>
	/// <param name="source">소스 문자열</param>
	/// <returns>압축 문자열</returns>

	public static string Compress(string source)
	{
		byte[] sourceArray = Encoding.UTF8.GetBytes(source);

		MemoryStream memoryStream = new MemoryStream();

		using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
		{
			gZipStream.Write(sourceArray, 0, sourceArray.Length);
		}

		memoryStream.Position = 0;

		byte[] temporaryArray = new byte[memoryStream.Length];
		memoryStream.Read(temporaryArray, 0, temporaryArray.Length);

		byte[] targetArray = new byte[temporaryArray.Length + 4];

		Buffer.BlockCopy(temporaryArray, 0, targetArray, 4, temporaryArray.Length);
		Buffer.BlockCopy(BitConverter.GetBytes(sourceArray.Length), 0, targetArray, 0, 4);

		return Convert.ToBase64String(targetArray);
	}

	/// <summary>
	/// 압축 해제하기
	/// </summary>
	/// <param name="source">소스 문자열</param>
	/// <returns>압축 해제 문자열</returns>
	public static string Decompress(string source)
	{
		byte[] sourceArray = Convert.FromBase64String(source);

		using (MemoryStream memoryStream = new MemoryStream())
		{
			int dataLength = BitConverter.ToInt32(sourceArray, 0);
			memoryStream.Write(sourceArray, 4, sourceArray.Length - 4);

			byte[] targetArray = new byte[dataLength];
			memoryStream.Position = 0;

			using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
			{
				gZipStream.Read(targetArray, 0, targetArray.Length);
			}

			return Encoding.UTF8.GetString(targetArray);
		}
	}

}
