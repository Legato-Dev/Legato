using Legato.Interop.AimpRemote.Entities;
using Legato.Interop.AimpRemote.Enum;
using Legato.Interop.Win32.Enum;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using Legato.Entities;
using Microsoft.Win32;
using System.ComponentModel;

namespace Legato.Interop.AimpRemote {
	/// <summary>
	/// AIMP のリモート API に関するヘルパーを提供します
	/// </summary>
	public class Helper {
		public static readonly string RemoteClassName = "AIMP2_RemoteInfo";
		public static readonly int RemoteMapFileSize = 2048;
		public static IntPtr AimpRemoteWindowHandle => Win32.API.FindWindow(RemoteClassName, null);

		/// <exception cref="ApplicationException" />
		/// <exception cref="FileNotFoundException" />
		public static string AimpProcessPath
		{
			get {
				var processPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\Media\AIMP\shell\open\command")?.GetValue(null)?.ToString();
				if (processPath == null)
					throw new ApplicationException("AIMP4.exeが見つかりませんでした。インストールされていない可能性があります。");

				var fileInfo = new FileInfo(processPath);
				if (!fileInfo.Exists)
					throw new FileNotFoundException("AIMP4.exeが見つかりませんでした。ファイルが移動したか、削除された可能性があります");

				return processPath;
			}
		}

		/// <summary>
		/// 4 byte 単位のメモリ読出し/値変換を行います。
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		private static uint _ReadToUInt32(Stream stream) {
			var buf = new byte[4];
			stream.Read(buf, 0, 4);

			return BitConverter.ToUInt32(buf, 0);
		}

		/// <summary>
		/// 8 byte 単位のメモリ読出し/値変換を行います。
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		private static ulong _ReadToUInt64(Stream stream) {
			var buf = new byte[8];
			stream.Read(buf, 0, 8);

			return BitConverter.ToUInt64(buf, 0);
		}

		/// <summary>
		/// メモリより読み出した文字列を文字数分返します。
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="count"></param>
		private static string _Read(StringReader reader, uint count) {
			const uint mask = 0x7FFFFFFF;
			var maskedCount = (int)(count & mask);
			var buf = new char[maskedCount];
			reader.Read(buf, 0, maskedCount);

			return new string(buf);
		}

		public static TrackInfo ReadTrackInfo() {
			var trackInfo = new TrackInfo();
			var meta = new TrackMetaInfo();

			MemoryMappedFile mmf = null;
			try {
				mmf = MemoryMappedFile.OpenExisting(RemoteClassName, MemoryMappedFileRights.ReadWrite, HandleInheritability.Inheritable);
			}
			catch (FileNotFoundException) {
				throw new ApplicationException("CurrentTrackの取得に失敗しました。AIMPが起動されているかを確認してください。");
			}

			using (var memory = mmf.CreateViewStream(0, RemoteMapFileSize)) {
				// 数値情報の読み取り
				meta.HeaderSize = _ReadToUInt32(memory);

				_ReadToUInt32(memory);
				trackInfo.BitRate = _ReadToUInt32(memory);
				trackInfo.ChannelType = (ChannelType)_ReadToUInt32(memory);
				trackInfo.Duration = _ReadToUInt32(memory);
				trackInfo.FileSize = _ReadToUInt64(memory);

				meta.Mask = _ReadToUInt32(memory);

				trackInfo.SampleRate = _ReadToUInt32(memory);
				trackInfo.TrackNumber = _ReadToUInt32(memory);

				meta.AlbumStringLength = _ReadToUInt32(memory);
				meta.ArtistStringLength = _ReadToUInt32(memory);
				meta.YearStringLength = _ReadToUInt32(memory);
				meta.FilePathStringLength = _ReadToUInt32(memory);
				meta.GenreStringLength = _ReadToUInt32(memory);
				meta.TitleStringLength = _ReadToUInt32(memory);

				// ヘッダの終端まで移動
				memory.Position = meta.HeaderSize;

				// 文字列の読み取り
				var buffer = new byte[RemoteMapFileSize - meta.HeaderSize];
				memory.Read(buffer, 0, buffer.Length);
				var trackInfoString = Encoding.Unicode.GetString(buffer);

				using (var reader = new StringReader(trackInfoString)) {
					trackInfo.Album = _Read(reader, meta.AlbumStringLength);
					trackInfo.Artist = _Read(reader, meta.ArtistStringLength);
					trackInfo.Year = _Read(reader, meta.YearStringLength);
					trackInfo.FilePath = _Read(reader, meta.FilePathStringLength);
					trackInfo.Genre = _Read(reader, meta.GenreStringLength);
					trackInfo.Title = _Read(reader, meta.TitleStringLength);
				}
			}

			return trackInfo;
		}

		// send Message (base)

		private static IntPtr _SendMessageBase(WindowMessage windowMessage, IntPtr param, IntPtr value) {
			IntPtr output;
			var result = Win32.API.SendMessageTimeout(AimpRemoteWindowHandle, windowMessage, param, value, SendMessageTimeoutType.NORMAL, 1000, out output);

			if (result == IntPtr.Zero) {
				var errorCode = Marshal.GetLastWin32Error();
				var error = new Win32Exception(errorCode);

				throw new ApplicationException($"AIMPとの通信に失敗しました。(errorCode={errorCode}, message=\"{error.Message}\")", error);
			}

			return output;
		}

		// send Property Message

		public static IntPtr SendPropertyMessage(PlayerProperty property, PropertyAccessMode mode, IntPtr value) =>
			_SendMessageBase((WindowMessage)AimpWindowMessage.Property, new IntPtr((uint)property | (uint)mode), value);

		public static IntPtr SendPropertyMessage(PlayerProperty property, PropertyAccessMode mode) =>
			SendPropertyMessage(property, mode, IntPtr.Zero);

		// send Command Message

		public static IntPtr SendCommandMessage(CommandType commandType, IntPtr value) =>
			_SendMessageBase((WindowMessage)AimpWindowMessage.Command, new IntPtr((uint)commandType), value);

		public static IntPtr SendCommandMessage(CommandType commandType) =>
			SendCommandMessage(commandType, IntPtr.Zero);

		/// <summary>
		/// イベント通知を行うウィンドウを AIMP に登録します
		/// <para>この操作はスレッドセーフです</para>
		/// </summary>
		/// <param name="communicationWindow">イベント通知を受け取る通信ウィンドウ</param>
		public static bool RegisterNotify(MessageReceiver messageReceiver) {
			bool result = false;
			messageReceiver.Invoke((Action)(() => {
				result = SendCommandMessage(CommandType.RegisterNotify, messageReceiver.Handle) != IntPtr.Zero;
			}));

			return result;
		}

		/// <summary>
		/// イベント通知を行うウィンドウを AIMP から登録解除します
		/// <para>この操作はスレッドセーフです</para>
		/// </summary>
		/// <param name="communicationWindow">イベント通知を受け取る通信ウィンドウ</param>
		public static bool UnregisterNotify(MessageReceiver messageReceiver) {
			bool result = false;
			messageReceiver.Invoke((Action)(() => {
				result = SendCommandMessage(CommandType.UnregisterNotify, messageReceiver.Handle) != IntPtr.Zero;
			}));

			return result;
		}
	}
}
