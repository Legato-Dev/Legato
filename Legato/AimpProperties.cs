using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Legato.Interop.AimpRemote;
using Legato.Interop.AimpRemote.Entities;
using Legato.Interop.AimpRemote.Enum;

namespace Legato {
	public class AimpProperties : IDisposable {

		private AlbumArtManager _AlbumArtManager { get; set; }

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

		/// <summary>
		/// 再生中のアルバムアートを取得します。
		/// </summary>
		/// <exception cref="ApplicationException" />
		public Task<Image> AlbumArt
		{
			get {
				if (!IsRunning)
					throw new ApplicationException("AlbumArtの取得に失敗しました。AIMPが起動されているかを確認してください。");

				try {
					// throw new NotSupportedException(); // ← 強制的に ♰最後の砦♰ を使うときはこちらを有効にしてください(非推奨)

					var albumArt = _AlbumArtManager.ExtractAlbumArt();
					return Task.FromResult(albumArt);
				}
				catch (NotSupportedException) {
					Debug.WriteLine("利用可能な AlbumArtExtractor はありませんでした");

					// 利用可能な extractor が無かったときの ♰最後の砦♰
					// Remote API のメモリ読出しにて AlbumArt を取得
					return _AlbumArtManager.FetchAlbumArtAsync();
				}
				catch (FileNotFoundException) {
					// CurrentTrack.FilePath からURL等を渡された可能性がある
					return Task.FromResult((Image)null);
				}
			}
		}

		public AimpProperties() {
			_AlbumArtManager = new AlbumArtManager();
		}

		public void Dispose() {
			_AlbumArtManager.Dispose();
		}
	}
}
