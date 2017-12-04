using System;
using System.Drawing;
using System.IO;
using Legato.Interop.AimpRemote.Entities;
using Legato.Interop.AimpRemote.Enum;
using System.Diagnostics;
using Legato.AlbumArtExtraction;
using Legato.Entities;
using Legato.Interop.AimpRemote;
using System.Threading.Tasks;

namespace Legato
{
	public class Aimp : IDisposable
	{
		public Aimp(int pollingIntervalMilliseconds = 100, bool isAutoSubscribing = true)
		{
			_Initialize(pollingIntervalMilliseconds, isAutoSubscribing);
		}

		public Aimp(TimeSpan pollingInterval, bool isAutoSubscribing = true)
		{
			if (pollingInterval.TotalMilliseconds > int.MaxValue)
				throw new ArgumentOutOfRangeException("pollingInterval");

			_Initialize((int)pollingInterval.TotalMilliseconds, isAutoSubscribing);
		}

		#region Properties

		public AimpObserver AimpObserver { get; set; }

		private AlbumArtManager _AlbumArtManager { get; set; }

		/// <summary>
		/// ポーリングの間隔を取得または設定します
		/// <para>この値が大きいと、ポーリングを伴う処理が遅延する可能性があります</para>
		/// </summary>
		public int PollingIntervalMilliseconds { get; set; }

		private System.Timers.Timer _Polling { get; set; }

		/// <summary>
		/// AIMP のイベント通知の購読を自動的に行うかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsAutoSubscribing { get; set; }

		/// <summary>
		/// AIMP が起動しているかどうかを示す値を取得します
		/// </summary>
		public bool IsRunning => Interop.AimpRemote.Helper.AimpRemoteWindowHandle != IntPtr.Zero;

		/// <summary>
		/// AIMP の実行ファイルのパスを取得します
		/// </summary>
		/// <exception cref="ApplicationException" />
		/// <exception cref="FileNotFoundException" />
		public string AimpProcessPath
		{
			get
			{
				var processPath = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\Media\AIMP\shell\open\command")?.GetValue(null)?.ToString();
				if (processPath == null)
					throw new ApplicationException("AIMP4.exeが見つかりませんでした。インストールされていない可能性があります。");

				var fileInfo = new FileInfo(processPath);
				if (!fileInfo.Exists)
					throw new FileNotFoundException("AIMP4.exeが見つかりませんでした。ファイルが移動したか、削除された可能性があります");

				return processPath;
			}
		}

		/// <summary>
		/// AIMP を起動します
		/// </summary>
		public Process StartAimp() => Process.Start(AimpProcessPath);

		/// <summary>
		/// AIMP の再生状態を示す値を取得します
		/// </summary>
		public PlayerState State => (PlayerState)Interop.AimpRemote.Helper.SendPropertyMessage(PlayerProperty.State, PropertyAccessMode.Get).ToInt32();

		/// <summary>
		/// 曲の再生位置を取得または設定します(単位は[ms]です)
		/// </summary>
		public int Position
		{
			get { return Interop.AimpRemote.Helper.SendPropertyMessage(PlayerProperty.Position, PropertyAccessMode.Get).ToInt32(); }
			set { Interop.AimpRemote.Helper.SendPropertyMessage(PlayerProperty.Position, PropertyAccessMode.Set, new IntPtr(value)); }
		}

		/// <summary>
		/// 曲の再生音量を取得または設定します(単位は[ms]です)
		/// </summary>
		public int Volume
		{
			get { return Interop.AimpRemote.Helper.SendPropertyMessage(PlayerProperty.Volume, PropertyAccessMode.Get).ToInt32(); }
			set
			{
				var volume = Math.Max(0, Math.Min(value, 100));
				var result = Interop.AimpRemote.Helper.SendPropertyMessage(PlayerProperty.Volume, PropertyAccessMode.Set, new IntPtr(volume));

				if (result == IntPtr.Zero)
					throw new ApplicationException("AimpのVolumeプロパティの設定に失敗しました。");
			}
		}

		/// <summary>
		/// ミュート状態であるかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsMute
		{
			get { return Interop.AimpRemote.Helper.SendPropertyMessage(PlayerProperty.IsMute, PropertyAccessMode.Get) != IntPtr.Zero; }
			set { Interop.AimpRemote.Helper.SendPropertyMessage(PlayerProperty.IsMute, PropertyAccessMode.Set, new IntPtr(value ? 1 : 0)); }
		}

		/// <summary>
		/// 一曲リピート再生中であるかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsRepeat
		{
			get { return Interop.AimpRemote.Helper.SendPropertyMessage(PlayerProperty.IsRepeat, PropertyAccessMode.Get) != IntPtr.Zero; }
			set { Interop.AimpRemote.Helper.SendPropertyMessage(PlayerProperty.IsRepeat, PropertyAccessMode.Set, new IntPtr(value ? 1 : 0)); }
		}

		/// <summary>
		/// ランダム再生中であるかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsShuffle
		{
			get { return Interop.AimpRemote.Helper.SendPropertyMessage(PlayerProperty.IsShuffle, PropertyAccessMode.Get) != IntPtr.Zero; }
			set { Interop.AimpRemote.Helper.SendPropertyMessage(PlayerProperty.IsShuffle, PropertyAccessMode.Set, new IntPtr(value ? 1 : 0)); }
		}

		/// <summary>
		/// 再生中の曲の情報を取得します
		/// </summary>
		public TrackInfo CurrentTrack => Interop.AimpRemote.Helper.CurrentTrack;

		/// <summary>
		/// 再生中のアルバムアートを取得します
		/// </summary>
		/// <exception cref="ApplicationException" />
		public Task<Image> AlbumArt
		{
			get
			{
				if (!IsRunning)
					throw new ApplicationException("AlbumArtの取得に失敗しました。AIMPが起動されているかを確認してください。");

				try
				{
					// throw new NotSupportedException(); // ← 強制的に ♰最後の砦♰ を使うときはこちらを有効にしてください(非推奨)

					var filePath = CurrentTrack.FilePath;

					// 利用可能な extractor を自動選択
					var extractor = new Selector().SelectAlbumArtExtractor(filePath);
					Debug.WriteLine(extractor.ToString());

					return Task.FromResult(extractor.Extract(filePath));
				}
				catch (NotSupportedException)
				{
					Debug.WriteLine("利用可能な AlbumArtExtractor はありませんでした");

					// 利用可能な extractor が無かったときの ♰最後の砦♰
					// Remote API のメモリ読出しにて AlbumArt を取得
					return _AlbumArtManager.FetchAlbumArtAsync();
				}
				catch (FileNotFoundException)
				{
					// noop: CurrentTrack.FilePathからURL等を渡された可能性がある
				}

				return Task.FromResult((Image)null);
			}
		}

		#endregion Properties

		#region Methods

		private void _Initialize(int pollingIntervalMilliseconds, bool isAutoSubscribing)
		{
			_Polling = new System.Timers.Timer(pollingIntervalMilliseconds);
			IsAutoSubscribing = isAutoSubscribing;

			var receiver = new MessageReceiver();
			AimpObserver = new AimpObserver(receiver);
			_AlbumArtManager = new AlbumArtManager(AimpObserver);

			// ポーリング
			_Polling.Elapsed += (s, e) =>
			{
				// 通知が購読されていない
				if (!AimpObserver.IsSubscribed)
				{
					if (IsRunning && IsAutoSubscribing)
					{
						// 通知を購読
						AimpObserver.Subscribe();
					}
				}

				// 通知が購読されている
				else
				{
					if (IsRunning)
					{
						// PositionProperty
						// _AimpObserver.OnPositionPropertyChanged(Position); // TODO: ここで_AimpObserverのPosition変更通知を発火したい
					}
					else
					{
						// AIMPが終了した
						AimpObserver.Unsubscribe();
					}
				}
			};

			_Polling.Start();
		}

		public void Dispose()
		{
			_Polling.Stop();

			AimpObserver.Dispose();
		}

		#region AIMPCommands

		/// <summary>
		/// 曲を再生します
		/// </summary>
		public void Play() => Interop.AimpRemote.Helper.SendCommandMessage(CommandType.Playing);

		/// <summary>
		/// 再生中の曲の再生状態を切り替えます
		/// </summary>
		public void PlayPause() => Interop.AimpRemote.Helper.SendCommandMessage(CommandType.PlayPause);

		/// <summary>
		/// 再生中の曲を一時停止します
		/// </summary>
		public void Pause() => Interop.AimpRemote.Helper.SendCommandMessage(CommandType.Pausing);

		/// <summary>
		/// 再生中の曲を停止します
		/// </summary>
		public void Stop() => Interop.AimpRemote.Helper.SendCommandMessage(CommandType.Stopped);

		/// <summary>
		/// 次の曲へ移動します
		/// </summary>
		public void Next() => Interop.AimpRemote.Helper.SendCommandMessage(CommandType.Next);

		/// <summary>
		/// 前の曲へ移動します
		/// </summary>
		public void Prev() => Interop.AimpRemote.Helper.SendCommandMessage(CommandType.Previous);

		/// <summary>
		/// AIMP を終了します
		/// </summary>
		public void Close() => Interop.AimpRemote.Helper.SendCommandMessage(CommandType.Quit);

		#endregion AIMPCommands

		#endregion Methods
	}
}
