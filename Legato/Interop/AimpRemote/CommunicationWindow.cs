using Legato.Interop.AimpRemote.Enum;
using Legato.Interop.Win32.Enum;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Legato.Interop.Win32.API;

namespace Legato.Interop.AimpRemote
{
	/// <summary>
	/// Aimpと通信するためのメッセージ専用ウィンドウ
	/// </summary>
	public class CommunicationWindow : Form
	{
		public delegate void MessageReceivedHandler(WindowMessage windowMessage, IntPtr wParam, IntPtr lParam);
		public event MessageReceivedHandler MessageReceived;

		public delegate void CopyDataMessageReceivedHandler(CopyDataStruct copyData);
		public event CopyDataMessageReceivedHandler CopyDataMessageReceived;

		public delegate void NotifyMessageReceivedHandler(NotifyType notifyType, IntPtr value);
		public event NotifyMessageReceivedHandler NotifyMessageReceived;

		public delegate void PropertyNotifyReceivedHandler(PropertyType type);
		public event PropertyNotifyReceivedHandler PropertyNotifyReceived;

		public delegate void TrackInfoNotifyHandler();
		public event TrackInfoNotifyHandler TrackInfoNotify;

		public delegate void TrackStartNotifyHandler();
		public event TrackStartNotifyHandler TrackStartNotify;

		public CommunicationWindow()
		{
			// メッセージ専用ウインドウに変更
			Win32.Helper.ChangeMessageOnlyWindow(this);
		}

		protected override void WndProc(ref Message message)
		{
			// Message
			MessageReceived?.Invoke((WindowMessage)message.Msg, message.WParam, message.LParam);

			// CopyDataMessage
			if ((WindowMessage)message.Msg == WindowMessage.COPYDATA)
			{
				var cds = Marshal.PtrToStructure<CopyDataStruct>(message.LParam);
				CopyDataMessageReceived?.Invoke(cds);
			}

			// NotifyMessage
			if ((AimpWindowMessage)message.Msg == AimpWindowMessage.Notify)
			{
				var type = (NotifyType)message.WParam;
				NotifyMessageReceived?.Invoke(type, message.LParam);

				if (type == NotifyType.Property)
					PropertyNotifyReceived?.Invoke((PropertyType)message.LParam);
				else if (type == NotifyType.TrackInfo && (int)message.LParam == 1)
					TrackInfoNotify?.Invoke();
				else if (type == NotifyType.TrackStart)
					TrackStartNotify?.Invoke();
				else if (type != NotifyType.TrackInfo)
					throw new ApplicationException("NotifyType is unknown value.");
			}

			base.WndProc(ref message);
		}
	}
}
