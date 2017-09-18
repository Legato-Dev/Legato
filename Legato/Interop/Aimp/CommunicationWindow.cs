using Legato.Interop.Win32.Enum;
using System;
using System.Windows.Forms;

namespace Legato.Interop.Aimp
{
	/// <summary>
	/// Aimpと通信するためのメッセージ専用ウィンドウ
	/// </summary>
	public class CommunicationWindow : Form
	{
		public delegate void MessageReceivedHandler(WindowMessage windowMessage, IntPtr wParam, IntPtr lParam);
		public event MessageReceivedHandler MessageReceived;

		public CommunicationWindow()
		{
			// メッセージ専用ウインドウに変更
			Win32.Helper.ChangeMessageOnlyWindow(this);
		}

		protected override void WndProc(ref Message message)
		{
			MessageReceived?.Invoke((WindowMessage)message.Msg, message.WParam, message.LParam);
			base.WndProc(ref message);
		}
	}
}
