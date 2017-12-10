using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Legato.AlbumArtExtraction;
using Legato.Entities;
using Legato.Interop.Win32.Enum;
using static Legato.Interop.Win32.API;

namespace Legato {
	public class AlbumArtManager : IDisposable {

		private MessageReceiver _Receiver { get; set; }

		private event Action<CopyDataStruct> _CopyDataMessageReceived;

		/// <summary>
		/// 
		/// </summary>
		public AlbumArtManager() {
			_Receiver = new MessageReceiver();
			_Receiver.MessageReceived += (message, wParam, lParam) => {
				// CopyDataMessageReceived を発行
				if (message == WindowMessage.COPYDATA) {
					var cds = Marshal.PtrToStructure<CopyDataStruct>(lParam);
					_CopyDataMessageReceived?.Invoke(cds);
				}
			};
			Interop.AimpRemote.Helper.RegisterNotify(_Receiver);
		}

		/// <summary>
		/// アルバムアートを抽出します
		/// </summary>
		/// <exception cref="FileNotFoundException">ファイルパスが不正である時にスローされます</exception>
		/// <exception cref="NotSupportedException">ファイル形式が未サポートの時にスローされます</exception>
		public Image ExtractAlbumArt() {
			var track = Interop.AimpRemote.Helper.ReadTrackInfo();

			// 利用可能な extractor を自動選択
			var extractor = new Selector().SelectAlbumArtExtractor(track.FilePath);
			Debug.WriteLine($"{extractor.ToString()} が選択されました");

			return extractor.Extract(track.FilePath);
		}

		/// <summary>
		/// <para>AIMP Remote API のメモリ読出しにてアルバムアートを取得します。</para>
		/// <para>この操作は正確なデータが取得できない可能性があります。可能であれば、代わりに AlbumArtManager.ExtractAlbumArt() を利用してください。</para>
		/// </summary>
		/// <param name="token">未実装</param>
		/// <exception cref="ApplicationException" />
		public Task<Image> FetchAlbumArtAsync(CancellationToken? token = null) {

			if (Interop.AimpRemote.Helper.AimpRemoteWindowHandle == IntPtr.Zero)
				throw new ApplicationException("AlbumArtの取得に失敗しました。AIMPが起動されているかを確認してください。");

			// AlbumArt 受信イベントをトリガーとする TaskCompletionSource
			var tcs = new TaskCompletionSource<Image>();

			Action<CopyDataStruct> handle = null;
			handle = (copyData) => {
				// AlbumArtの更新
				if (copyData.dwData == new IntPtr(Interop.AimpRemote.Helper.CopyDataIdArtWork)) {
					_CopyDataMessageReceived -= handle;

					var dataLength = (int)copyData.cbData;
					var albumArtSource = new byte[dataLength];
					Marshal.Copy(copyData.lpData, albumArtSource, 0, dataLength);

					using (var memory = new MemoryStream()) {
						memory.Write(albumArtSource, 0, albumArtSource.Length);

						using (var image = Image.FromStream(memory)) {
							tcs.SetResult(new Bitmap(image));
						}
					}
				}
			};
			_CopyDataMessageReceived += handle;

			if (!Interop.AimpRemote.Helper.RequestAlbumArt(_Receiver)) {
				_CopyDataMessageReceived -= handle;
				tcs.SetException(new ApplicationException("AlbumArt のリクエストに失敗しました。アルバムアートが設定されていない可能性があります。"));
			}

			return tcs.Task;
		}

		public void Dispose() {
			Interop.AimpRemote.Helper.UnregisterNotify(_Receiver);
		}
	}
}
