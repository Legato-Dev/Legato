using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Legato.Entities;
using static Legato.Interop.Win32.API;

namespace Legato.Interop.AimpRemote
{
	public class AlbumArtManager
	{
		private AimpObserver _Observer { get; set; }

		public AlbumArtManager(AimpObserver observer)
		{
			_Observer = observer;
		}

		public Task<Image> FetchAlbumArtAsync(CancellationToken? token = null)
		{
			// AlbumArt 受信イベントをトリガーとする TaskCompletionSource
			var tcs = new TaskCompletionSource<Image>();

			Action<CopyDataStruct> handle = null;
			handle = (copyData) =>
			{
				// AlbumArtの更新
				if (copyData.dwData == new IntPtr(Helper.CopyDataIdArtWork))
				{
					_Observer.CopyDataMessageReceived -= handle;

					var dataLength = (int)copyData.cbData;
					var albumArtSource = new byte[dataLength];
					Marshal.Copy(copyData.lpData, albumArtSource, 0, dataLength);

					using (var memory = new MemoryStream())
					{
						memory.Write(albumArtSource, 0, albumArtSource.Length);

						using (var image = Image.FromStream(memory))
						{
							tcs.SetResult(new Bitmap(image));
						}
					}
				}
			};

			_Observer.CopyDataMessageReceived += handle;
			if (!Helper.RequestAlbumArt(_Observer.Receiver))
			{
				tcs.SetException(new Exception("AlbumArt のリクエストに失敗しました"));
			}

			return tcs.Task;
		}
	}
}
