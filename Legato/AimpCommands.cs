using System.Diagnostics;
using Legato.Interop.AimpRemote;
using Legato.Interop.AimpRemote.Enum;

namespace Legato {
	/// <summary>
	/// AIMPに対するコマンド送信をサポートします
	/// </summary>
	public class AimpCommands {

		/// <summary>
		/// AIMP を起動します
		/// </summary>
		public Process StartAimp() => Process.Start(Helper.AimpProcessPath);

		/// <summary>
		/// 曲を再生します
		/// </summary>
		public void Play() => Helper.SendCommandMessage(CommandType.Playing);

		/// <summary>
		/// 再生中の曲の再生状態を切り替えます
		/// </summary>
		public void PlayPause() => Helper.SendCommandMessage(CommandType.PlayPause);

		/// <summary>
		/// 再生中の曲を一時停止します
		/// </summary>
		public void Pause() => Helper.SendCommandMessage(CommandType.Pausing);

		/// <summary>
		/// 再生中の曲を停止します
		/// </summary>
		public void Stop() => Helper.SendCommandMessage(CommandType.Stopped);

		/// <summary>
		/// 次の曲へ移動します
		/// </summary>
		public void Next() => Helper.SendCommandMessage(CommandType.Next);

		/// <summary>
		/// 前の曲へ移動します
		/// </summary>
		public void Prev() => Helper.SendCommandMessage(CommandType.Previous);

		/// <summary>
		/// AIMP を終了します
		/// </summary>
		public void Close() => Helper.SendCommandMessage(CommandType.Quit);
	}
}
