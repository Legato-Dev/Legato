using Legato.Interop.Aimp;
using Legato.Interop.Aimp.Enum;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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
				if (copyData.dwData == new IntPtr(RemoteHelper.CopyDataIdArtWork))
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
				RemoteHelper.RegisterNotify(_Communicator);
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

		public TrackInfo CurrentTrack
		{
			get
			{
				var trackInfo = new TrackInfo();
				var meta = new TrackMetaInfo();

				using (var memory = RemoteHelper.RemoteMmfStream)
				{
					// 数値情報の読み取り
					meta.HeaderSize = RemoteHelper.ReadToUInt32(memory);

					trackInfo.IsActive = RemoteHelper.ReadToUInt32(memory) != 0;
					trackInfo.BitRate = RemoteHelper.ReadToUInt32(memory);
					trackInfo.channelType = (ChannelType)RemoteHelper.ReadToUInt32(memory);
					trackInfo.Duration = RemoteHelper.ReadToUInt32(memory);
					trackInfo.FileSize = RemoteHelper.ReadToUInt64(memory);

					meta.Mask = RemoteHelper.ReadToUInt32(memory);

					trackInfo.SampleRate = RemoteHelper.ReadToUInt32(memory);
					trackInfo.TrackNumber = RemoteHelper.ReadToUInt32(memory);

					meta.AlbumStringLength = RemoteHelper.ReadToUInt32(memory);
					meta.ArtistStringLength = RemoteHelper.ReadToUInt32(memory);
					meta.YearStringLength = RemoteHelper.ReadToUInt32(memory);
					meta.FilePathStringLength = RemoteHelper.ReadToUInt32(memory);
					meta.GenreStringLength = RemoteHelper.ReadToUInt32(memory);
					meta.TitleStringLength = RemoteHelper.ReadToUInt32(memory);

					// ヘッダの終端まで移動
					memory.Position = meta.HeaderSize;

					// 文字列の読み取り
					var buffer = new byte[RemoteHelper.RemoteMapFileSize - meta.HeaderSize];
					memory.Read(buffer, 0, buffer.Length);
					var trackInfoString = Encoding.Unicode.GetString(buffer);

					using (var reader = new StringReader(trackInfoString))
					{
						trackInfo.Album = RemoteHelper.Read(reader, meta.AlbumStringLength);
						trackInfo.Artist = RemoteHelper.Read(reader, meta.ArtistStringLength);
						trackInfo.Year = RemoteHelper.Read(reader, meta.YearStringLength);
						trackInfo.FilePath = RemoteHelper.Read(reader, meta.FilePathStringLength);
						trackInfo.Genre = RemoteHelper.Read(reader, meta.GenreStringLength);
						trackInfo.Title = RemoteHelper.Read(reader, meta.TitleStringLength);
					}
				}

				return trackInfo;
			}
		}

		/// <summary>
		/// 再生中のアルバムアートを取得します。
		/// </summary>
		public byte[] AlbumArt
		{
			get
			{
				if (!RemoteHelper.RequestAlbumArt(_Communicator))
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
