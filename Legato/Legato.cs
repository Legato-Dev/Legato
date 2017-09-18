using Legato.Interop.Aimp;
using Legato.Interop.Aimp.Enum;
using System;
using System.Runtime.InteropServices;

namespace Legato
{
	public class Legato : IDisposable
	{
		public Legato()
		{
			_Communicator = new CommunicationWindow();
			_Communicator.CopyDataMessageReceived += (senderWindowHandle, copyData) =>
			{
				// AlbumArtの更新
				if (copyData.dwData == new IntPtr(RemoteHelper.CopyDataIdArtWork))
				{
					var dataLength = (int)copyData.cbData;
					_AlbumArt = new byte[dataLength];
					Marshal.Copy(copyData.lpData, _AlbumArt, 0, dataLength);
				}
			};
		}

		private CommunicationWindow _Communicator { get; set; }

		/// <summary>
		/// AIMPが起動しているかどうかを示す値を取得します
		/// </summary>
		public bool IsRunning
		{
			get
			{
				try { var remote = RemoteHelper.AimpRemoteWindowHandle; return true; }
				catch { return false; }
			}
		}

		/// <summary>
		/// AIMPの再生状態を示す値を取得します
		/// </summary>
		public PlayerState State => (PlayerState)RemoteHelper.SendPropertyMessage(PropertyType.State, PropertyAccessMode.Get).ToInt32();

		/// <summary>
		/// 曲の長さを示す値を取得します(単位は[ms]です)
		/// </summary>
		public int Duration => RemoteHelper.SendPropertyMessage(PropertyType.Duration, PropertyAccessMode.Get).ToInt32();

		/// <summary>
		/// 曲の再生位置を取得または設定します(単位は[ms]です)
		/// </summary>
		public int Position
		{
			get { return RemoteHelper.SendPropertyMessage(PropertyType.Position, PropertyAccessMode.Get).ToInt32(); }
			set { RemoteHelper.SendPropertyMessage(PropertyType.Position, PropertyAccessMode.Set, new IntPtr(value)); }
		}

		/// <summary>
		/// 曲の再生音量を取得または設定します(単位は[ms]です)
		/// </summary>
		public int Volume
		{
			get { return RemoteHelper.SendPropertyMessage(PropertyType.Volume, PropertyAccessMode.Get).ToInt32(); }
			set
			{
				var volume = Math.Max(0, Math.Min(value, 100));
				var result = RemoteHelper.SendPropertyMessage(PropertyType.Volume, PropertyAccessMode.Set, new IntPtr(volume));

				if (result == IntPtr.Zero)
					throw new Exception("AimpのVolumeプロパティの設定に失敗しました。");
			}
		}

		/// <summary>
		/// ミュート状態であるかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsMute
		{
			get { return RemoteHelper.SendPropertyMessage(PropertyType.IsMute, PropertyAccessMode.Get) != IntPtr.Zero; }
			set { RemoteHelper.SendPropertyMessage(PropertyType.IsMute, PropertyAccessMode.Set, new IntPtr(value ? 1 : 0)); }
		}

		/// <summary>
		/// リピート再生中であるかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsRepeat
		{
			get { return RemoteHelper.SendPropertyMessage(PropertyType.IsRepeat, PropertyAccessMode.Get) != IntPtr.Zero; }
			set { RemoteHelper.SendPropertyMessage(PropertyType.IsRepeat, PropertyAccessMode.Set, new IntPtr(value ? 1 : 0)); }
		}

		/// <summary>
		/// ランダム再生中であるかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsShuffle
		{
			get { return RemoteHelper.SendPropertyMessage(PropertyType.IsShuffle, PropertyAccessMode.Get) != IntPtr.Zero; }
			set { RemoteHelper.SendPropertyMessage(PropertyType.IsShuffle, PropertyAccessMode.Set, new IntPtr(value ? 1 : 0)); }
		}

		/* TODO
		public TrackInfo CurrentTrack
		{
			get;
		}
		*/

		/// <summary>
		/// 再生中のアルバムアートを取得します。
		/// </summary>
		public byte[] AlbumArt
		{
			get
			{
				if (!RemoteHelper.RequestArtWork(_Communicator))
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
			RemoteHelper.SendCommandMessage(CommandType.Playing);
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
			RemoteHelper.SendCommandMessage(CommandType.PlayPause);
		}

		/// <summary>
		/// 再生中の曲を一時停止します。
		/// </summary>
		public void Pause()
		{
			RemoteHelper.SendCommandMessage(CommandType.Pausing);
		}

		/// <summary>
		/// 再生中の曲を停止します。
		/// </summary>
		public void Stop()
		{
			RemoteHelper.SendCommandMessage(CommandType.Stopped);
		}

		/// <summary>
		/// 次の曲へ移動します。
		/// </summary>
		public void Next()
		{
			RemoteHelper.SendCommandMessage(CommandType.Next);
		}

		/// <summary>
		/// 前の曲へ移動します。
		/// </summary>
		public void Prev()
		{
			RemoteHelper.SendCommandMessage(CommandType.Previous);
		}

		/// <summary>
		/// AIMP を終了します。
		/// </summary>
		public void Close()
		{
			RemoteHelper.SendCommandMessage(CommandType.Quit);
		}

		#endregion

		public void Dispose()
		{

		}
	}
}
