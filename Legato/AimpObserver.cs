using System;
using Legato.Entities;
using Legato.Interop.AimpRemote;
using Legato.Interop.AimpRemote.Entities;
using Legato.Interop.AimpRemote.Enum;
using Legato.Interop.Win32.Enum;

namespace Legato
{
	/// <summary>
	/// AIMPからのイベント受信をサポートします
	/// </summary>
	public class AimpObserver : IDisposable
	{
		private System.Timers.Timer _Polling { get; set; }

		private MessageReceiver _Receiver { get; set; }

		/// <summary>
		/// AIMP のイベント通知を購読しているかどうか(受信可能であるかどうか)を示す値を取得します
		/// </summary>	
		public bool IsSubscribed { get; private set; } = false;

		/// <summary>
		/// AIMP のイベント通知の購読を自動的に行うかどうかを示す値を取得または設定します
		/// </summary>
		public bool IsAutoSubscribing { get; set; }

		/// <summary>
		/// AIMP の通知を購読した時に発生します
		/// </summary>
		public event Action Subscribed;

		/// <summary>
		/// AIMP の通知を購読解除した時(または AIMP が終了した時)に発生します
		/// </summary>
		public event Action Unsubscribed;

		#region Notification events

		/// <summary>
		/// 何らかの通知が発生した時に発生します
		/// </summary>
		public event Action<NotifyType, IntPtr> NotifyMessageReceived;

		/// <summary>
		/// 何らかのプロパティが変更された時に発生します
		/// </summary>
		public event Action<PlayerProperty> PropertyChanged;

		/// <summary>
		/// CurrentTrack プロパティが変更された時に発生します
		/// </summary>
		public event Action<TrackInfo> CurrentTrackChanged;

		/// <summary>
		/// Duration プロパティが変更された時に発生します
		/// </summary>
		public event Action<TimeSpan> DurationPropertyChanged;

		/// <summary>
		/// IsMute プロパティが変更された時に発生します
		/// </summary>
		public event Action<bool> IsMutePropertyChanged;

		/// <summary>
		/// IsRepeat プロパティが変更された時に発生します
		/// </summary>
		public event Action<bool> IsRepeatPropertyChanged;

		/// <summary>
		/// IsShuffle プロパティが変更された時に発生します
		/// </summary>
		public event Action<bool> IsShufflePropertyChanged;

		/// <summary>
		/// Position プロパティが変更された時に発生します
		/// </summary>
		public event Action<int> PositionPropertyChanged;

		/// <summary>
		/// State プロパティが変更された時に発生します
		/// </summary>
		public event Action<PlayerState> StatePropertyChanged;

		/// <summary>
		/// Volume プロパティが変更された時に発生します
		/// </summary>
		public event Action<int> VolumePropertyChanged;

		#endregion Notification events

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pollingIntervalMilliseconds"></param>
		/// <param name="isAutoSubscribing"></param>
		public AimpObserver(int pollingIntervalMilliseconds = 100, bool isAutoSubscribing = true) =>
			_Initialize(isAutoSubscribing, pollingIntervalMilliseconds);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pollingInterval"></param>
		/// <param name="isAutoSubscribing"></param>
		public AimpObserver(TimeSpan pollingInterval, bool isAutoSubscribing = true)
		{
			if (pollingInterval.TotalMilliseconds > int.MaxValue)
				throw new ArgumentOutOfRangeException("pollingInterval");

			_Initialize(isAutoSubscribing, (int)pollingInterval.TotalMilliseconds);
		}

		private void _Initialize(bool isAutoSubscribing, int pollingIntervalMilliseconds)
		{
			IsAutoSubscribing = isAutoSubscribing;

			// ポーリング
			_Polling = new System.Timers.Timer(pollingIntervalMilliseconds);
			_Polling.Elapsed += (s, e) =>
			{
				var isRunning = Helper.AimpRemoteWindowHandle != IntPtr.Zero;

				// 通知が購読されていない
				if (!IsSubscribed)
				{
					if (isRunning && IsAutoSubscribing)
					{
						// 通知を購読
						Subscribe();
					}
				}

				// 通知が購読されている
				else
				{
					if (isRunning)
					{
						// PositionProperty
						var position = Helper.SendPropertyMessage(PlayerProperty.Position, PropertyAccessMode.Get).ToInt32();
						PositionPropertyChanged?.Invoke(position);
					}
					else
					{
						// AIMPが終了した
						Unsubscribe();
					}
				}
			};
			_Polling.Start();

			_Receiver = new MessageReceiver();
			_Receiver.MessageReceived += (message, wParam, lParam) =>
			{
				// NotifyMessageReceived を発行
				if (message == (WindowMessage)AimpWindowMessage.Notify)
					NotifyMessageReceived?.Invoke((NotifyType)wParam, lParam);
			};

			NotifyMessageReceived += (type, lParam) =>
			{
				// PropertyChanged を発行
				if (type == NotifyType.Property)
					PropertyChanged?.Invoke((PlayerProperty)lParam);

				// CurrentTrackChanged を発行
				else if (type == NotifyType.TrackStart)
					CurrentTrackChanged?.Invoke(Helper.ReadTrackInfo());

				else if (type == NotifyType.TrackInfo) { }

				else
					throw new ApplicationException($"NotifyType '{type}' is undefined value");
			};

			PropertyChanged += (type) =>
			{
				var propertyValue = Helper.SendPropertyMessage(type, PropertyAccessMode.Get).ToInt32();

				// DurationPropertyChanged を発行
				if (type == PlayerProperty.Duration)
					DurationPropertyChanged?.Invoke(TimeSpan.FromMilliseconds(propertyValue));

				// IsMutePropertyChanged を発行
				else if (type == PlayerProperty.IsMute)
					IsMutePropertyChanged?.Invoke(propertyValue != 0);

				// IsRepeatPropertyChanged を発行
				else if (type == PlayerProperty.IsRepeat)
					IsRepeatPropertyChanged?.Invoke(propertyValue != 0);

				// IsShufflePropertyChanged を発行
				else if (type == PlayerProperty.IsShuffle)
					IsShufflePropertyChanged?.Invoke(propertyValue != 0);

				// PositionPropertyChanged を発行
				else if (type == PlayerProperty.Position)
					PositionPropertyChanged?.Invoke(propertyValue);

				// StatePropertyChanged を発行
				else if (type == PlayerProperty.State)
					StatePropertyChanged?.Invoke((PlayerState)propertyValue);

				// VolumePropertyChanged を発行
				else if (type == PlayerProperty.Volume)
					VolumePropertyChanged?.Invoke(propertyValue);

				else
					throw new ApplicationException($"PropertyType '{type}' is undefined value");
			};
		}

		/// <summary>
		/// AIMP のイベント通知を購読します
		/// </summary>
		/// <exception cref="ApplicationException" />
		public void Subscribe()
		{
			if (IsSubscribed)
				throw new ApplicationException("既に通知を購読しています");

			IsSubscribed = true;

			var isRunning = Helper.AimpRemoteWindowHandle != IntPtr.Zero;

			if (!isRunning)
				throw new ApplicationException("AIMPが起動していないため、購読に失敗しました");

			Helper.RegisterNotify(_Receiver);
			Subscribed?.Invoke();
		}

		/// <summary>
		/// AIMP のイベント通知の購読を解除します
		/// </summary>
		/// <exception cref="ApplicationException" />
		public void Unsubscribe()
		{
			if (!IsSubscribed)
				throw new ApplicationException("通知を購読していません");

			var isRunning = Helper.AimpRemoteWindowHandle != IntPtr.Zero;

			if (isRunning)
			{
				Helper.UnregisterNotify(_Receiver);
			}

			IsSubscribed = false;
			Unsubscribed?.Invoke();
		}

		/// <summary>
		/// 解放します
		/// </summary>
		public void Dispose()
		{
			// 通知の購読解除
			if (IsSubscribed)
				Unsubscribe();

			_Polling.Stop();
		}
	}
}
