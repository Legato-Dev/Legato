using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Legato.AlbumArtExtraction.Flac;

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

		private Stream _FileStream { get; set; }

		/// <summary>
		/// ASCII文字列として指定されたカウント数だけ読み取ります
		/// </summary>
		private string _ReadAsAsciiString(Stream stream, int count)
		{
			var buf = new byte[count];
			stream.Read(buf, 0, count);

			return new string(Encoding.ASCII.GetChars(buf));
		}

		private List<byte> _ReadAsByteList(Stream stream, int count)
		{
			var buf = new byte[count];
			stream.Read(buf, 0, count);

			return new List<byte>(buf);
		}

		private uint _ReadAsUInt(Stream stream)
		{
			var buf = new byte[4];
			stream.Read(buf, 0, 4);

			return BitConverter.ToUInt32(buf, 0);
		}

		private MetaData _ReadMetaDataBlock()
		{
			var isLastAndMetaDataType = _ReadAsByteList(_FileStream, 1)[0];
			var isLast = (isLastAndMetaDataType & 0x01U) == 1;
			var metaDataType = (MetaDataType)(isLastAndMetaDataType & 0x7FU);
			var metaDataLengthSource = _ReadAsByteList(_FileStream, 3);
			metaDataLengthSource.Insert(0, 0);
			metaDataLengthSource.Reverse();
			var metaDataLength = BitConverter.ToUInt32(metaDataLengthSource.ToArray(), 0);
			var metaData = _ReadAsByteList(_FileStream, (int)metaDataLength);

			return new MetaData(metaDataType, isLast, metaData);
		}

		private Image _ParsePictureMetaData(MetaData pictureMetaData)
		{
			Image result = null;

			using (var image = new MemoryStream())
			using (var memory = new MemoryStream())
			{
				var initialPos = memory.Position;
				memory.Write(pictureMetaData.Data.ToArray(), 0, pictureMetaData.Data.Count);
				memory.Position = initialPos;

				var imageType = _ReadAsByteList(memory, 4);
				var mimeTypeLengthSource = _ReadAsByteList(memory, 4);
				mimeTypeLengthSource.Reverse();
				var mimeTypeLength = BitConverter.ToUInt32(mimeTypeLengthSource.ToArray(), 0);
				var mimeType = _ReadAsAsciiString(memory, (int)mimeTypeLength);
				var explanationLengthSource = _ReadAsByteList(memory, 4);
				explanationLengthSource.Reverse();
				var explanationLength = BitConverter.ToUInt32(explanationLengthSource.ToArray(), 0);
				_ReadAsByteList(memory, (int)explanationLength);
				var a = _ReadAsByteList(memory, 4);
				var b = _ReadAsByteList(memory, 4);
				var c = _ReadAsByteList(memory, 4);
				var d = _ReadAsByteList(memory, 4);
				var sizeSource = _ReadAsByteList(memory, 4);
				sizeSource.Reverse();
				var size = BitConverter.ToUInt32(sizeSource.ToArray(), 0);

				var buf = new byte[size];
				memory.Read(buf, 0, (int)size);
				image.Write(buf, 0, buf.Length);

				result = Image.FromStream(image);
			}

			return result;
		}

		public Image Extract()
		{
			using (_FileStream = new FileStream(FilePath, FileMode.Open))
			{
				var fileType = _ReadAsAsciiString(_FileStream, 4);

				if (fileType != "fLaC")
					throw new InvalidDataException("このファイルはFLAC形式ではありません");

				var metaDataList = new List<MetaData>();
				MetaData metaData = null;
				do
				{
					metaDataList.Add(metaData = _ReadMetaDataBlock());
				} while (!metaData.IsLast);

				var picture = metaDataList.Find(i => i.Type == MetaDataType.PICTURE);

				if (picture == null)
					return null;

				return _ParsePictureMetaData(picture);
			}
		}
	}
}
