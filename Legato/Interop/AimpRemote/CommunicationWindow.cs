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

		public delegate void NotifyMessageReceivedHandler(NotifyType notifyType);
		public event NotifyMessageReceivedHandler NotifyMessageReceived;

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
				NotifyMessageReceived?.Invoke((NotifyType)message.WParam);
			}

			base.WndProc(ref message);
		}
	}
}
