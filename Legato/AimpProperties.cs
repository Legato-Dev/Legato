using System;
using Legato.Interop.AimpRemote;
using Legato.Interop.AimpRemote.Entities;
using Legato.Interop.AimpRemote.Enum;

namespace Legato {
	/// <summary>
	/// AIMPからのプロパティデータの取得をサポートします
	/// </summary>
	public class AimpProperties {

		/// <summary>
		/// AIMP が起動しているかどうかを示す値を取得します
		/// </summary>
		public bool IsRunning => Helper.AimpRemoteWindowHandle != IntPtr.Zero;

		/// <summary>
		/// AIMP の実行ファイルのパスを取得します
		/// </summary>
		/// <exception cref="ApplicationException" />
		/// <exception cref="FileNotFoundException" />
		public string AimpProcessPath => Helper.AimpProcessPath;

		/// <summary>
		/// AIMP の再生状態を示す値を取得します
		/// </summary>
		public PlayerState State => (PlayerState)Helper.SendPropertyMessage(PlayerProperty.State, PropertyAccessMode.Get).ToInt32();

		/// <summary>
		/// 曲の再生位置を取得または設定します(単位は[ms]です)
		/// </summary>
		public int Position
		{
			get { return Helper.SendPropertyMessage(PlayerProperty.Position, PropertyAccessMode.Get).ToInt32(); }
			set { Helper.SendPropertyMessage(PlayerProperty.Position, PropertyAccessMode.Set, new IntPtr(value)); }
		}

		/// <summary>
		/// 曲の再生音量を取得または設定します(単位は[ms]です)
		/// </summary>
		public int Volume
		{
			get { return Helper.SendPropertyMessage(PlayerProperty.Volume, PropertyAccessMode.Get).ToInt32(); }
			set {
				var volume = Math.Max(0, Math.Min(value, 100));
				var result = Helper.SendPropertyMessage(PlayerProperty.Volume, PropertyAccessMode.Set, new IntPtr(volume));

				if (result == IntPtr.Zero)
					throw new ApplicationException("AimpのVolumeプロパティの設定に失敗しました。");
			}
		}

		/// <summary>
		/// ミュート状態であるかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsMute
		{
			get { return Helper.SendPropertyMessage(PlayerProperty.IsMute, PropertyAccessMode.Get) != IntPtr.Zero; }
			set { Helper.SendPropertyMessage(PlayerProperty.IsMute, PropertyAccessMode.Set, new IntPtr(value ? 1 : 0)); }
		}

		/// <summary>
		/// 一曲リピート再生中であるかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsRepeat
		{
			get { return Helper.SendPropertyMessage(PlayerProperty.IsRepeat, PropertyAccessMode.Get) != IntPtr.Zero; }
			set { Helper.SendPropertyMessage(PlayerProperty.IsRepeat, PropertyAccessMode.Set, new IntPtr(value ? 1 : 0)); }
		}

		/// <summary>
		/// ランダム再生中であるかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsShuffle
		{
			get { return Helper.SendPropertyMessage(PlayerProperty.IsShuffle, PropertyAccessMode.Get) != IntPtr.Zero; }
			set { Helper.SendPropertyMessage(PlayerProperty.IsShuffle, PropertyAccessMode.Set, new IntPtr(value ? 1 : 0)); }
		}

		/// <summary>
		/// 再生中の曲の情報を取得します
		/// </summary>
		public TrackInfo CurrentTrack => Helper.ReadTrackInfo();
	}
}
