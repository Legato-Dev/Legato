using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Legato.AlbumArtExtraction.Flac;

namespace Legato.AlbumArtExtraction
{
	/// <summary>
	/// FLAC形式のファイルからアルバムアートを抽出する機能を表します
	/// </summary>
	public class FlacAlbumArtExtractor : IAlbumArtExtractor
	{
		/// <summary>
		/// メタデータブロックを読み取ります
		/// </summary>
		/// <param name="stream">対象の Stream</param>
		private MetaData _ReadMetaDataBlock(Stream stream)
		{
			var isLastAndMetaDataType = Helper.ReadAsByte(stream);
			var isLast = (isLastAndMetaDataType & 0x80U) != 0;
			var metaDataType = (MetaDataType)(isLastAndMetaDataType & 0x7FU);
			var metaDataLength = Helper.ReadAsUInt(stream, 3);
			if (metaDataLength == 0)
				throw new ApplicationException("this MetaDataBlock is invalid");
			var metaData = Helper.ReadAsByteList(stream, (int)metaDataLength);

			return new MetaData(metaDataType, isLast, metaData);
		}

		/// <summary>
		/// PICTUREタイプのメタデータから Image を取り出します
		/// </summary>
		private Image _ParsePictureMetaData(MetaData pictureMetaData)
		{
			if (pictureMetaData.Type != MetaDataType.PICTURE)
				throw new ArgumentException("このメタデータはPICTUREタイプではありません");

			using (var memory = new MemoryStream())
			{
				memory.Write(pictureMetaData.Data.ToArray(), 0, pictureMetaData.Data.Count);
				memory.Seek(0, SeekOrigin.Begin);

				var mimeTypeLength = Helper.ReadAsUInt(memory, skip: 4);
				var explanationLength = Helper.ReadAsUInt(memory, skip: (int)mimeTypeLength);
				var imageSourceSize = Helper.ReadAsUInt(memory, skip: (int)explanationLength + 4 * 4);
				var imageSource = Helper.ReadAsByteList(memory, (int)imageSourceSize);

				using (var image = new MemoryStream())
				{
					image.Write(imageSource.ToArray(), 0, imageSource.Count);
					return Image.FromStream(image);
				}
			}
		}

		/// <summary>
		/// 対象のファイルが形式と一致しているかを判別します
		/// </summary>
		public bool CheckType(string filePath)
		{
			using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				var fileType = Helper.ReadAsAsciiString(file, 4);

				if (fileType != "fLaC")
					return false;
			}

			try
			{
				if (Extract(filePath) != null)
					return true;
			}
			catch (ApplicationException ex) { }

			return false;
		}

		/// <summary>
		/// アルバムアートを抽出します
		/// </summary>
		public Image Extract(string filePath)
		{
			try
			{
				using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					Helper.Skip(file, 4);

					var metaDataList = new List<MetaData>();
					MetaData metaData = null;
					do
					{
						metaDataList.Add(metaData = _ReadMetaDataBlock(file));
					}
					while (!metaData.IsLast && metaDataList.Count < 64);

					if (metaDataList.Count >= 64)
						throw new InvalidDataException("メタデータの個数が異常です");

					var picture = metaDataList.Find(i => i.Type == MetaDataType.PICTURE);

					return (picture != null) ? _ParsePictureMetaData(picture) : null;
				}
			}
			catch(Exception ex)
			{
				throw new ApplicationException("アルバムアートの抽出に失敗しました", ex);
			}
		}
	}
}
