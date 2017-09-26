using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Legato.AlbumArtExtraction
{
	public static class Helper
	{
		public static void Skip(Stream stream, int skip)
		{
			if (skip > 0)
				stream.Seek(skip, SeekOrigin.Current);
		}

		/// <summary>
		/// ASCII文字列として指定されたカウント数だけ読み取ります
		/// </summary>
		public static string ReadAsAsciiString(Stream stream, int count, int skip = 0)
		{
			Skip(stream, skip);
			var buf = new byte[count];
			stream.Read(buf, 0, count);

			return new string(Encoding.ASCII.GetChars(buf));
		}

		/// <summary>
		/// 指定した長さのデータを List<byte> として読み取ります
		/// </summary>
		/// <param name="stream">対象の Stream</param>
		/// <param name="count">読み取るデータの長さ(バイト数)</param>
		/// <param name="skip">読み飛ばす長さ(バイト数)</param>
		public static List<byte> ReadAsByteList(Stream stream, int count, int skip = 0)
		{
			Skip(stream, skip);
			var buf = new byte[count];
			stream.Read(buf, 0, count);

			return new List<byte>(buf);
		}

		/// <summary>
		/// 指定した長さのデータを uint として読み取ります(ビッグエンディアン)
		/// </summary>
		/// <param name="stream">対象の Stream</param>
		/// <param name="count">読み取るデータの長さ(バイト数)</param>
		/// <param name="skip">読み飛ばす長さ(バイト数)</param>
		public static uint ReadAsUInt(Stream stream, int count = 4, int skip = 0)
		{
			if (count > 4)
				throw new ArgumentOutOfRangeException("count");

			var buf = ReadAsByteList(stream, count, skip);
			foreach (var i in Enumerable.Range(0, 4 - count))
				buf.Insert(0, 0);
			buf.Reverse();

			return BitConverter.ToUInt32(buf.ToArray(), 0);
		}
	}
}
