using System;
using System.Windows.Forms;
using Legato.Interop.Win32.Enum;

namespace Legato.Entities
{
	/// <summary>
	/// AIMP と通信するためのメッセージ専用ウィンドウ
	/// </summary>
	public class MessageReceiver : Form, IWindowMessageReceivable
	{
		public event Action<WindowMessage, IntPtr, IntPtr> MessageReceived;

		public MessageReceiver()
		{
			// メッセージ専用ウインドウに変更
			Interop.Win32.Helper.ChangeMessageOnlyWindow(this);
		}

		protected override void WndProc(ref Message message)
		{
			// MessageReceived を発行
			MessageReceived?.Invoke((WindowMessage)message.Msg, message.WParam, message.LParam);

			base.WndProc(ref message);
		}
	}
}
