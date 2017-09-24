using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Legato.AlbumArtExtraction
{
	/// <summary>
	/// FLAC形式のファイルからアルバムアートを抽出する機能を表します
	/// </summary>
	public class FlacAlbumArtExtractor : IAlbumArtExtractor
	{
		public FlacAlbumArtExtractor(string filePath)
		{
			FilePath = filePath;
		}

		public string FilePath { get; set; }
		private Stream _Stream { get; set; }

		/// <summary>
		/// ASCII文字列として指定されたカウント数だけ読み取ります
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="count"></param>
		private string _ReadAsAsciiString(int count)
		{
			var buf = new byte[count];
			_Stream.Read(buf, 0, count);

			return new string(Encoding.ASCII.GetChars(buf));
		}

		private uint _ReadAsUInt()
		{
			var buf = new byte[4];
			_Stream.Read(buf, 0, 4);

			return BitConverter.ToUInt32(buf, 0);
		}

		private List<byte> _ReadAsByteList(int count)
		{
			var buf = new byte[count];
			_Stream.Read(buf, 0, count);

			return new List<byte>(buf);
		}

		public enum FlacMetaDataType
		{
			STREAMINFO = 0,
			PADDING = 1,
			APPLICATION = 2,
			SEEKTABLE = 3,
			VORBIS_COMMENT = 4,
			CUESHEET = 5,
			PICTURE = 6
		}

		public class FlacMetaData
		{
			public FlacMetaData(FlacMetaDataType type, bool isLast, List<byte> data)
			{
				Type = type;
				IsLast = isLast;
				Data = data;
			}

			public FlacMetaDataType Type { get; set; }
			public bool IsLast { get; set; }
			public List<byte> Data { get; set; }

			public override string ToString() => $"FlacMetaData {{ Type = {Type}, IsLast = {IsLast}, DataSize = {Data.Count} }}";
		}

		private FlacMetaData _ReadMetaDataBlock()
		{
			var isLastAndMetaDataType = _ReadAsByteList(1)[0];
			var isLast = (isLastAndMetaDataType & 0x01U) == 1;
			var metaDataType = (FlacMetaDataType)(isLastAndMetaDataType & 0x7FU);
			var metaDataLengthSource = _ReadAsByteList(3);
			metaDataLengthSource.Insert(0, 0);
			metaDataLengthSource.Reverse();
			var metaDataLength = BitConverter.ToUInt32(metaDataLengthSource.ToArray(), 0);
			var metaData = _ReadAsByteList((int)metaDataLength);

			var flacMetaData = new FlacMetaData(metaDataType, isLast, metaData);

			return flacMetaData;
		}

		public Image Extract()
		{
			using (_Stream = new FileStream(FilePath, FileMode.Open))
			{
				var fileType = _ReadAsAsciiString(4);

				if (fileType != "fLaC")
					throw new InvalidDataException("このファイルはFLAC形式ではありません");

				var metaDataList = new List<FlacMetaData>();
				FlacMetaData metaData = null;
				do
				{
					metaDataList.Add(metaData = _ReadMetaDataBlock());
					Console.WriteLine(metaData);
				} while (!metaData.IsLast);
			}

			return null;
		}
	}
}
