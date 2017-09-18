using Legato.Interop.Win32.Enum;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Legato.Interop.Win32.API;

namespace Legato.Interop.Aimp
{
	/// <summary>
	/// Aimpと通信するためのメッセージ専用ウィンドウ
	/// </summary>
	public class CommunicationWindow : Form
	{
		public delegate void MessageReceivedHandler(WindowMessage windowMessage, IntPtr wParam, IntPtr lParam);
		public event MessageReceivedHandler MessageReceived;

		public delegate void CopyDataMessageReceivedHandler(IntPtr senderWindowHandle, CopyDataStruct copyData);
		public event CopyDataMessageReceivedHandler CopyDataMessageReceived;

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
				CopyDataMessageReceived?.Invoke(message.WParam, cds);
			}

			base.WndProc(ref message);
		}
	}
}
