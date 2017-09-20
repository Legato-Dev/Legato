using Legato.Interop.AimpRemote.Entities;
using Legato.Interop.AimpRemote.Enum;
using Legato.Interop.Win32.Enum;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace Legato.Interop.AimpRemote
{
	/// <summary>
	/// AIMPから提供されるリモートAPIに関するヘルパーを提供します
	/// </summary>
	public class Helper
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
		/// <param name="stream"></param>
		/// <returns></returns>
		private static uint _ReadToUInt32(Stream stream)
		{
			var buf = new byte[4];
			stream.Read(buf, 0, 4);

			return BitConverter.ToUInt32(buf, 0);
		}

		/// <summary>
		/// 8 byte 単位のメモリ読出し/値変換を行います。
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		private static ulong _ReadToUInt64(Stream stream)
		{
			var buf = new byte[8];
			stream.Read(buf, 0, 8);

			return BitConverter.ToUInt64(buf, 0);
		}

		/// <summary>
		/// メモリより読み出した文字列を文字数分返します。
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="count"></param>
		private static string _Read(StringReader reader, uint count)
		{
			const uint mask = 0x7FFFFFFF;
			var maskedCount = (int)(count & mask);
			var buf = new char[maskedCount];
			reader.Read(buf, 0, maskedCount);

			return new string(buf);
		}

		private static MemoryMappedViewStream _RemoteMmfStream
		{
			get
			{
				var mmf = MemoryMappedFile.OpenExisting(RemoteClassName, MemoryMappedFileRights.ReadWrite, HandleInheritability.Inheritable);

				return mmf.CreateViewStream(0, RemoteMapFileSize);
			}
		}

		public static TrackInfo GetCurrentTrack()
		{
			var trackInfo = new TrackInfo();
			var meta = new TrackMetaInfo();

			using (var memory = _RemoteMmfStream)
			{
				// 数値情報の読み取り
				meta.HeaderSize = _ReadToUInt32(memory);

				trackInfo.IsActive = _ReadToUInt32(memory) != 0;
				trackInfo.BitRate = _ReadToUInt32(memory);
				trackInfo.channelType = (ChannelType)_ReadToUInt32(memory);
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

				using (var reader = new StringReader(trackInfoString))
				{
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
		private static IntPtr _SendMessageBase(WindowMessage windowMessage, IntPtr param, IntPtr value)
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
			return _SendMessageBase((WindowMessage)AimpWindowMessage.Property, new IntPtr((uint)propertyType | (uint)mode), value);
		}

		public static IntPtr SendPropertyMessage(PropertyType propertyType, PropertyAccessMode mode)
		{
			return SendPropertyMessage(propertyType, mode, IntPtr.Zero);
		}

		// send Command Message
		public static IntPtr SendCommandMessage(CommandType commandType, IntPtr value)
		{
			return _SendMessageBase((WindowMessage)AimpWindowMessage.Command, new IntPtr((uint)commandType), value);
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
