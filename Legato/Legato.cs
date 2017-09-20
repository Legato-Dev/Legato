using Legato.Interop.AimpRemote;
using Legato.Interop.AimpRemote.Entities;
using Legato.Interop.AimpRemote.Enum;
using System;
using System.Runtime.InteropServices;

namespace Legato
{
	public class Legato : IDisposable
	{
		public Legato()
		{
			_Communicator = new CommunicationWindow();

			_Communicator.CopyDataMessageReceived += (copyData) =>
			{
				// AlbumArtの更新
				if (copyData.dwData == new IntPtr(Helper.CopyDataIdArtWork))
				{
					var dataLength = (int)copyData.cbData;
					_AlbumArt = new byte[dataLength];
					Marshal.Copy(copyData.lpData, _AlbumArt, 0, dataLength);
				}
			};

			_Communicator.NotifyMessageReceived += (notifyType) =>
			{
				// TODO: 通知
			};

			if (IsRunning)
				Helper.RegisterNotify(_Communicator);
		}

		private CommunicationWindow _Communicator { get; set; }

		/// <summary>
		/// AIMPが起動しているかどうかを示す値を取得します
		/// </summary>
		public bool IsRunning
		{
			get
			{
				try { var remote = Helper.AimpRemoteWindowHandle; return true; }
				catch { return false; }
			}
		}

		/// <summary>
		/// AIMPの再生状態を示す値を取得します
		/// </summary>
		public PlayerState State => (PlayerState)Helper.SendPropertyMessage(PropertyType.State, PropertyAccessMode.Get).ToInt32();

		/// <summary>
		/// 曲の長さを示す値を取得します(単位は[ms]です)
		/// </summary>
		public int Duration => Helper.SendPropertyMessage(PropertyType.Duration, PropertyAccessMode.Get).ToInt32();

		/// <summary>
		/// 曲の再生位置を取得または設定します(単位は[ms]です)
		/// </summary>
		public int Position
		{
			get { return Helper.SendPropertyMessage(PropertyType.Position, PropertyAccessMode.Get).ToInt32(); }
			set { Helper.SendPropertyMessage(PropertyType.Position, PropertyAccessMode.Set, new IntPtr(value)); }
		}

		/// <summary>
		/// 曲の再生音量を取得または設定します(単位は[ms]です)
		/// </summary>
		public int Volume
		{
			get { return Helper.SendPropertyMessage(PropertyType.Volume, PropertyAccessMode.Get).ToInt32(); }
			set
			{
				var volume = Math.Max(0, Math.Min(value, 100));
				var result = Helper.SendPropertyMessage(PropertyType.Volume, PropertyAccessMode.Set, new IntPtr(volume));

				if (result == IntPtr.Zero)
					throw new ApplicationException("AimpのVolumeプロパティの設定に失敗しました。");
			}
		}

		/// <summary>
		/// ミュート状態であるかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsMute
		{
			get { return Helper.SendPropertyMessage(PropertyType.IsMute, PropertyAccessMode.Get) != IntPtr.Zero; }
			set { Helper.SendPropertyMessage(PropertyType.IsMute, PropertyAccessMode.Set, new IntPtr(value ? 1 : 0)); }
		}

		/// <summary>
		/// リピート再生中であるかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsRepeat
		{
			get { return Helper.SendPropertyMessage(PropertyType.IsRepeat, PropertyAccessMode.Get) != IntPtr.Zero; }
			set { Helper.SendPropertyMessage(PropertyType.IsRepeat, PropertyAccessMode.Set, new IntPtr(value ? 1 : 0)); }
		}

		/// <summary>
		/// ランダム再生中であるかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsShuffle
		{
			get { return Helper.SendPropertyMessage(PropertyType.IsShuffle, PropertyAccessMode.Get) != IntPtr.Zero; }
			set { Helper.SendPropertyMessage(PropertyType.IsShuffle, PropertyAccessMode.Set, new IntPtr(value ? 1 : 0)); }
		}

		/// <summary>
		/// 再生中の曲の情報を取得します
		/// </summary>
		public TrackInfo CurrentTrack => Helper.GetCurrentTrack();

		/// <summary>
		/// 再生中のアルバムアートを取得します。
		/// </summary>
		public byte[] AlbumArt
		{
			get
			{
				if (!Helper.RequestAlbumArt(_Communicator))
					return null;

				return _AlbumArt;
			}
		}
		private byte[] _AlbumArt { get; set; }

		#region Commands

		/// <summary>
		/// 曲を再生します。
		/// </summary>
		public void Play()
		{
			Helper.SendCommandMessage(CommandType.Playing);
		}

		/// <summary>
		/// <para>再生中の曲の再生状態を切り替えます。</para>
		/// <para>【現状態】⇒【次状態】</para>
		/// <para>再生中⇒一時停止</para>
		/// <para>一時停止中⇒再生</para>
		/// <para>停止中⇒再生</para>
		/// </summary>
		public void PlayPause()
		{
			Helper.SendCommandMessage(CommandType.PlayPause);
		}

		/// <summary>
		/// 再生中の曲を一時停止します。
		/// </summary>
		public void Pause()
		{
			Helper.SendCommandMessage(CommandType.Pausing);
		}

		/// <summary>
		/// 再生中の曲を停止します。
		/// </summary>
		public void Stop()
		{
			Helper.SendCommandMessage(CommandType.Stopped);
		}

		/// <summary>
		/// 次の曲へ移動します。
		/// </summary>
		public void Next()
		{
			Helper.SendCommandMessage(CommandType.Next);
		}

		/// <summary>
		/// 前の曲へ移動します。
		/// </summary>
		public void Prev()
		{
			Helper.SendCommandMessage(CommandType.Previous);
		}

		/// <summary>
		/// AIMP を終了します。
		/// </summary>
		public void Close()
		{
			Helper.SendCommandMessage(CommandType.Quit);
		}

		#endregion

		public void Dispose()
		{

		}
	}
}
