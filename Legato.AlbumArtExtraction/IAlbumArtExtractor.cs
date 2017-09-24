using System.Drawing;

namespace Legato.AlbumArtExtraction
{
	/// <summary>
	/// アルバムアートを抽出するために必要となるメンバを公開します
	/// </summary>
	public interface IAlbumArtExtractor
	{
		Image Extract(string filePath);
	}
}
