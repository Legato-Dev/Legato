namespace Legato.Interop.Aimp.Enum
{
	/// <summary>
	/// AIMP 上の使用コマンドを定義します
	/// </summary>
	public enum CommandType
	{
		/// <summary>
		/// 再生を行うことを示すコマンドです
		/// </summary>
		Playing = 13,

		/// <summary>
		/// 再生と一時停止の切り替えを行うことを示すコマンドです
		/// </summary>
		PlayPause = 14,

		/// <summary>
		/// 一時停止を行うことを示すコマンドです
		/// </summary>
		Pausing = 15,

		/// <summary>
		/// 停止を行うことを示すコマンドです
		/// </summary>
		Stopped = 16,

		/// <summary>
		/// 次の曲へ移動を行うことを示すコマンドです
		/// </summary>
		Next = 17,

		/// <summary>
		/// 前の曲へ移動を行うことを示すコマンドです
		/// </summary>
		Previous = 18,

		/// <summary>
		/// AIMPの終了を行うことを示すコマンドです
		/// </summary>
		Quit = 21,

		/// <summary>
		/// アルバムアートを取得することを示すコマンドです(このコマンドは32bitアプリケーションでのみ動作します)
		/// </summary>
		GetArtwork = 29,
	}
}
