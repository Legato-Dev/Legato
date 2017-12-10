using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Legato.AlbumArtExtraction;
using static Legato.Interop.Win32.API;

namespace Legato {
	public class AlbumArtManager {

		private AimpProperties _Properties { get; set; }

		private AimpObserver _Observer { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="observer">FetchAlbumArtAsync を利用する場合は設定してください</param>
		public AlbumArtManager(AimpProperties properties, AimpObserver observer = null) {
			_Properties = properties;
			_Observer = observer;
		}

		/// <summary>
		/// アルバムアートを抽出します
		/// </summary>
		/// <exception cref="FileNotFoundException">ファイルパスが不正である時にスローされます</exception>
		/// <exception cref="NotSupportedException">ファイル形式が未サポートの時にスローされます</exception>
		public Image ExtractAlbumArt() {
			// 利用可能な extractor を自動選択
			var extractor = new Selector().SelectAlbumArtExtractor(_Properties.CurrentTrack.FilePath);
			Debug.WriteLine($"{extractor.ToString()} が選択されました");

			return extractor.Extract(_Properties.CurrentTrack.FilePath);
		}

		/// <summary>
		/// <para>AIMP Remote API のメモリ読出しにてアルバムアートを取得します。利用には Observer の設定が必要です。</para>
		/// <para>この操作は正確なデータが取得できない可能性があります。可能であれば、代わりに AlbumArtManager.ExtractAlbumArt() を利用してください。</para>
		/// </summary>
		/// <param name="token">未実装</param>
		public Task<Image> FetchAlbumArtAsync(CancellationToken? token = null) {
			if (_Observer == null)
				throw new NullReferenceException("Observer がインスタンスに設定されていません。");

			if (Interop.AimpRemote.Helper.AimpRemoteWindowHandle == IntPtr.Zero)
				throw new ApplicationException("AlbumArtの取得に失敗しました。AIMPが起動されているかを確認してください。");

			// AlbumArt 受信イベントをトリガーとする TaskCompletionSource
			var tcs = new TaskCompletionSource<Image>();

			Action<CopyDataStruct> handle = null;
			handle = (copyData) => {
				// AlbumArtの更新
				if (copyData.dwData == new IntPtr(Interop.AimpRemote.Helper.CopyDataIdArtWork)) {
					_Observer.CopyDataMessageReceived -= handle;

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

			_Observer.CopyDataMessageReceived += handle;
			if (!Interop.AimpRemote.Helper.RequestAlbumArt(_Observer.Receiver)) {
				tcs.SetException(new Exception("AlbumArt のリクエストに失敗しました"));
			}

			return tcs.Task;
		}
	}
}
