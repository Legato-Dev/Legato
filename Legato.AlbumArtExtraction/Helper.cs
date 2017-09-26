using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Legato.AlbumArtExtraction
{
	public static class Helper
	{
		/// <summary>
		/// ASCII文字列として指定されたカウント数だけ読み取ります
		/// </summary>
		public static string _ReadAsAsciiString(Stream stream, int count, int offset = 0)
		{
			var buf = new byte[count];
			stream.Read(buf, offset, count);

			return new string(Encoding.ASCII.GetChars(buf));
		}

		/// <summary>
		/// 指定した長さのデータを List<byte> として読み取ります
		/// </summary>
		/// <param name="stream">対象の Stream</param>
		/// <param name="count">読み取るデータの長さ(バイト数)</param>
		/// <param name="offset">読み飛ばす長さ(バイト数)</param>
		public static List<byte> _ReadAsByteList(Stream stream, int count, int offset = 0)
		{
			var buf = new byte[count];
			stream.Read(buf, offset, count);

			return new List<byte>(buf);
		}

		/// <summary>
		/// 指定した長さのデータを uint として読み取ります(ビッグエンディアン)
		/// </summary>
		/// <param name="stream">対象の Stream</param>
		/// <param name="count">読み取るデータの長さ(バイト数)</param>
		/// <param name="offset">読み飛ばす長さ(バイト数)</param>
		public static uint _ReadAsUInt(Stream stream, int count = 4, int offset = 0)
		{
			if (count > 4)
				throw new ArgumentOutOfRangeException("count");

			var buf = _ReadAsByteList(stream, count, offset);
			foreach (var i in Enumerable.Range(0, 4 - count))
				buf.Insert(0, 0);
			buf.Reverse();

			return BitConverter.ToUInt32(buf.ToArray(), 0);
		}
	}
}
