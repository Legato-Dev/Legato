using Legato.Interop.Aimp.Enum;
using Legato.Interop.Win32.Enum;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Legato.Interop.Aimp
{
	/// <summary>
	/// AIMPから提供されるリモートウィンドウを操作する
	/// </summary>
	public class RemoteHelper
	{
		public static readonly string RemoteClassName = "AIMP2_RemoteInfo";
		public static readonly int RemoteMapFileSize = 2048;

		/// <summary>
		/// アートワークのCopyDataIdを示します
		/// </summary>
		public static readonly uint CopyDataIdArtWork = 0x41495043;

		public static IntPtr AimpRemoteWindowHandle
		{
			get
			{
				var handle = Win32.API.FindWindow(RemoteClassName, null);

				if (handle == IntPtr.Zero)
					throw new Exception("remote window not found");

				return handle;
			}
		}

		/// <summary>
		/// 4 byte 単位のメモリ読出し/値変換を行います。
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static uint ReadToUint32(byte[] buf, Stream stream)
		{
			stream.Read(buf, 0, 4);
			return BitConverter.ToUInt32(buf, 0);
		}

		/// <summary>
		/// 8 byte 単位のメモリ読出し/値変換を行います。
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static ulong ReadToUint64(byte[] buf, Stream stream)
		{
			stream.Read(buf, 0, 8);
			return BitConverter.ToUInt64(buf, 0);
		}

		/// <summary>
		/// メモリより読み出した文字列を文字数分返します。
		/// </summary>
		/// <param name="len"></param>
		/// <param name="buf"></param>
		/// <param name="sr"></param>
		/// <returns></returns>
		public static string ReadToString(int len, char[] buf, StringReader sr)
		{
			buf = new char[len];
			sr.Read(buf, 0, len);

			return new string(buf);
		}

		public static MemoryMappedViewStream RemoteMmfStream
		{
			get
			{
				var mmf = MemoryMappedFile.OpenExisting(RemoteClassName, MemoryMappedFileRights.ReadWrite, HandleInheritability.Inheritable);
				return mmf.CreateViewStream(0, RemoteMapFileSize);
			}
		}

		// send Message (base)
		private static IntPtr SendMessageBase(WindowMessage windowMessage, IntPtr param, IntPtr value)
		{
			IntPtr output;
			var result = Win32.API.SendMessageTimeout(AimpRemoteWindowHandle, windowMessage, param, value, SendMessageTimeoutType.NORMAL, 1000, out output);

			if (result == IntPtr.Zero)
			{
				// TODO 例外処理
				throw new Exception("on SendMessageBase");
			}

			return output;
		}

		// send Property Message
		public static IntPtr SendPropertyMessage(PropertyType propertyType, PropertyAccessMode mode, IntPtr value)
		{
			return SendMessageBase((WindowMessage)AimpWindowMessage.Property, new IntPtr((uint)propertyType | (uint)mode), value);
		}

		public static IntPtr SendPropertyMessage(PropertyType propertyType, PropertyAccessMode mode)
		{
			return SendPropertyMessage(propertyType, mode, IntPtr.Zero);
		}

		// send Command Message
		public static IntPtr SendCommandMessage(CommandType commandType, IntPtr value)
		{
			return SendMessageBase((WindowMessage)AimpWindowMessage.Command, new IntPtr((uint)commandType), value);
		}

		public static IntPtr SendCommandMessage(CommandType commandType)
		{
			return SendCommandMessage(commandType, IntPtr.Zero);
		}

		/// <summary>
		/// アルバムアートをリクエストします
		/// </summary>
		/// <param name="communicationWindow">ArtWorkを受け取る通信ウィンドウ</param>
		public static bool RequestAlbumArt(CommunicationWindow communicationWindow)
		{
			return SendCommandMessage(CommandType.RequestAlbumArt, communicationWindow.Handle) != IntPtr.Zero;
		}

		public static bool RegisterNotify(CommunicationWindow communicationWindow)
		{
			return SendCommandMessage(CommandType.RegisterNotify, communicationWindow.Handle) != IntPtr.Zero;
		}

		public static bool UnregisterNotify(CommunicationWindow communicationWindow)
		{
			return SendCommandMessage(CommandType.UnregisterNotify, communicationWindow.Handle) != IntPtr.Zero;
		}
	}
}
