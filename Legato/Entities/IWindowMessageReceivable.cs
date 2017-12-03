using System;
using Legato.Interop.Win32.Enum;

namespace Legato.Entities
{
	public interface IWindowMessageReceivable
	{
		event Action<WindowMessage, IntPtr, IntPtr> MessageReceived;
	}
}
