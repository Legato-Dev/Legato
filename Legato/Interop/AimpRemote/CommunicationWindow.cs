using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Legato.Interop.AimpRemote.Enum;
using Legato.Interop.Win32.Enum;
using static Legato.Interop.Win32.API;

namespace Legato.Interop.AimpRemote
{
	/// <summary>
	/// Aimpと通信するためのメッセージ専用ウィンドウ
	/// </summary>
	public class CommunicationWindow : Form
	{
		// MessageReceived events
		public event Action<WindowMessage, IntPtr, IntPtr> MessageReceived;
		public event Action<CopyDataStruct> CopyDataMessageReceived;
		public event Action<NotifyType, IntPtr> NotifyMessageReceived;

		// Changed events
		public event Action<PropertyType> PropertyChanged;
		public event Action TrackInfoChanged;
		public event Action CurrentTrackChanged;

		// PropertyChanged events
		public event Action<TimeSpan> DurationPropertyChanged;
		public event Action<bool> IsMutePropertyChanged;
		public event Action<bool> IsRepeatPropertyChanged;
		public event Action<bool> IsShufflePropertyChanged;
		public event Action<int> PositionPropertyChanged;
		public event Action<PlayerState> StatePropertyChanged;
		public event Action<int> VolumePropertyChanged;

		public CommunicationWindow()
		{
			// メッセージ専用ウインドウに変更
			Win32.Helper.ChangeMessageOnlyWindow(this);

			MessageReceived += (message, wParam, lParam) =>
			{
				// CopyDataMessageReceived を発行
				if (message == WindowMessage.COPYDATA)
				{
					var cds = Marshal.PtrToStructure<CopyDataStruct>(lParam);
					CopyDataMessageReceived?.Invoke(cds);
				}

				// NotifyMessageReceived を発行
				else if (message == (WindowMessage)AimpWindowMessage.Notify)
					NotifyMessageReceived?.Invoke((NotifyType)wParam, lParam);
			};

			NotifyMessageReceived += (type, lParam) =>
			{
				if (type != NotifyType.Property)
					Debug.WriteLine($"[NotifyMessage] {type} 0x{lParam.ToString("X")}");

				// PropertyChanged を発行
				if (type == NotifyType.Property)
					PropertyChanged?.Invoke((PropertyType)lParam);

				// CurrentTrackChanged を発行
				else if (type == NotifyType.TrackStart)
					CurrentTrackChanged?.Invoke();

				// TrackInfoChanged を発行
				else if (type == NotifyType.TrackInfo && (int)lParam == 1)
					TrackInfoChanged?.Invoke();

				else if (type != NotifyType.TrackInfo)
					throw new ApplicationException($"NotifyType '{type}' is undefined value");
			};

			PropertyChanged += (type) =>
			{
				var propertyValue = Helper.SendPropertyMessage(type, PropertyAccessMode.Get).ToInt32();

				// DurationPropertyChanged を発行
				if (type == PropertyType.Duration)
					DurationPropertyChanged?.Invoke(TimeSpan.FromMilliseconds(propertyValue));

				// IsMutePropertyChanged を発行
				else if (type == PropertyType.IsMute)
					IsMutePropertyChanged?.Invoke(propertyValue != 0);

				// IsRepeatPropertyChanged を発行
				else if (type == PropertyType.IsRepeat)
					IsRepeatPropertyChanged?.Invoke(propertyValue != 0);

				// IsShufflePropertyChanged を発行
				else if (type == PropertyType.IsShuffle)
					IsShufflePropertyChanged?.Invoke(propertyValue != 0);

				// PositionPropertyChanged を発行
				else if (type == PropertyType.Position)
					PositionPropertyChanged?.Invoke(propertyValue);

				// StatePropertyChanged を発行
				else if (type == PropertyType.State)
					StatePropertyChanged?.Invoke((PlayerState)propertyValue);

				// VolumePropertyChanged を発行
				else if (type == PropertyType.Volume)
					VolumePropertyChanged?.Invoke(propertyValue);

				else
					throw new ApplicationException($"PropertyType '{type}' is undefined value");
			};
		}

		protected override void WndProc(ref Message message)
		{
			// MessageReceived を発行
			MessageReceived?.Invoke((WindowMessage)message.Msg, message.WParam, message.LParam);

			base.WndProc(ref message);
		}
	}
}
