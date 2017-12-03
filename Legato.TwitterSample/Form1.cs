using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreTweet;
using Legato.Interop.AimpRemote.Entities;
using Newtonsoft.Json;

namespace Legato.TwitterSample
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		#region Constants

		private static readonly string _DefaultPostingFormat = "{TrackNum}. {Title}\r\nArtist: {Artist}\r\nAlbum: {Album}\r\n#nowplaying";
		private static readonly string _DefaultTokensKey = "please paste your token infos";

		#endregion Constants

		#region Properties

		private Aimp _Aimp { get; set; }
		private Tokens _Twitter { get; set; }
		private string _PostingFormat { get; set; }

		#endregion Properties

		#region Methods

		private async Task _PostAsync()
		{
			try
			{
				var track = _Aimp.CurrentTrack;

				// 投稿内容を構築
				var stringBuilder = new StringBuilder(_PostingFormat);
				stringBuilder = stringBuilder.Replace("{Title}", "{0}");
				stringBuilder = stringBuilder.Replace("{Artist}", "{1}");
				stringBuilder = stringBuilder.Replace("{Album}", "{2}");
				stringBuilder = stringBuilder.Replace("{TrackNum}", "{3:D2}");
				var text = string.Format(stringBuilder.ToString(), track.Title, track.Artist, track.Album, track.TrackNumber);

				if (checkBoxNeedAlbumArt.Checked && _Aimp.AlbumArt != null)
				{
					using (var memory = new MemoryStream())
						_Aimp.AlbumArt.Save("temp.png", ImageFormat.Png);

					await _Twitter.Statuses.UpdateWithMediaAsync(text, new FileInfo("temp.png"));
				}
				else
					await _Twitter.Statuses.UpdateAsync(text);

				Console.WriteLine("Twitter への投稿が完了しました");
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// フォームに表示されているアルバムアートを更新します
		/// </summary>
		private void _UpdateAlbumArt()
		{
			if (_Aimp?.IsRunning ?? false)
				pictureBoxAlbumArt.Image = _Aimp.AlbumArt ?? Properties.Resources.logo;
			else
				pictureBoxAlbumArt.Image = Properties.Resources.logo;
		}

		private void _UpdateFormTrackInfo(TrackInfo track)
		{
			labelTrackNumber.Text = $"{track.TrackNumber:D2}.";
			labelTitle.Text = track.Title;
			labelArtist.Text = track.Artist;
			labelAlbum.Text = track.Album;

			var os = Environment.OSVersion;
			notifyIcon.Icon = Properties.Resources.legato;

			// トースト通知
			if (os.Version.Major >= 6 && os.Version.Minor >= 2)
			{
				notifyIcon.BalloonTipTitle = $"Legato NowPlaying\r\n{track.Title} - {track.Artist}";
				notifyIcon.BalloonTipText = $"Album : {track.Album}";
				Debug.WriteLine("トースト通知が表示されました。");
			}
			// バルーン通知
			else
			{
				notifyIcon.BalloonTipTitle = $"Legato NowPlaying";
				notifyIcon.BalloonTipText = $"{track.Title} - {track.Artist}\r\nAlbum : {track.Album}";
				Debug.WriteLine("バルーン通知が表示されました。");
			}
			notifyIcon.ShowBalloonTip(10000);
		}

		#region File IO Methods

		/// <summary>
		/// settings.json から設定を読み込みます
		/// <para>settings.json が存在しないときは新規に生成します</para>
		/// </summary>
		private async Task _LoadSettingsAsync()
		{
			try
			{
				string jsonString = null;
				using (var reader = new StreamReader("settings.json", Encoding.UTF8))
					jsonString = await reader.ReadToEndAsync();

				dynamic json = JsonConvert.DeserializeObject(jsonString);

				_PostingFormat = json.format ?? _DefaultPostingFormat;
			}
			catch
			{
				_PostingFormat = _DefaultPostingFormat;
				await _SaveSettingsAsync();
			}
		}

		/// <summary>
		/// settings.json に設定を保存します
		/// </summary>
		private async Task _SaveSettingsAsync()
		{
			// 設定ファイルに保存すべき情報がある場合
			if (_PostingFormat != null)
			{
				var data = new { format = _PostingFormat };

				var jsonString = JsonConvert.SerializeObject(data, new JsonSerializerSettings
				{
					StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
				});

				using (var writer = new StreamWriter("settings.json", false, Encoding.UTF8))
					await writer.WriteAsync(jsonString);
			}
		}

		/// <summary>
		/// tokens.json からアカウント情報を読み込みます
		/// </summary>
		private async Task _LoadTokensFileAsync()
		{
			try
			{
				string jsonString = null;
				using (var reader = new StreamReader("tokens.json", Encoding.UTF8))
					jsonString = await reader.ReadToEndAsync();

				dynamic json = JsonConvert.DeserializeObject(jsonString);

				var ck = (string)json.ConsumerKey;
				var cs = (string)json.ConsumerSecret;
				var at = (string)json.AccessToken;
				var ats = (string)json.AccessTokenSecret;

				var isNotDefaultTokens = ck != _DefaultTokensKey && cs != _DefaultTokensKey && at != _DefaultTokensKey && ats != _DefaultTokensKey;
				if (isNotDefaultTokens)
				{
					var tokens = Tokens.Create(ck, cs, at, ats);

					// トークンの有効性を検証
					try
					{
						var account = await tokens.Account.VerifyCredentialsAsync(include_entities: false, skip_status: true);
						_Twitter = tokens;
					}
					catch { }
				}
			}
			catch
			{
				// JSONの構造が間違っている、もしくは存在しなかった場合
				await _CreateTokensFileAsync();
			}
		}

		/// <summary>
		/// tokens.json を生成します
		/// </summary>
		private async Task _CreateTokensFileAsync()
		{
			var data = new
			{
				ConsumerKey = _DefaultTokensKey,
				ConsumerSecret = _DefaultTokensKey,
				AccessToken = _DefaultTokensKey,
				AccessTokenSecret = _DefaultTokensKey
			};

			var jsonString = JsonConvert.SerializeObject(data, new JsonSerializerSettings
			{
				StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
			});

			using (var writer = new StreamWriter("tokens.json", false, Encoding.UTF8))
				await writer.WriteAsync(jsonString);
		}

		#endregion File IO Methods

		#endregion Methods

		#region Procedures

		private async void Form1_Load(object sender, EventArgs e)
		{
			Icon = Properties.Resources.legato;

			await _LoadTokensFileAsync();

			if (_Twitter == null)
			{
				MessageBox.Show(
					"有効なTwitterのトークン情報の設定が必要です。tokens.jsonの中身を編集してからアプリケーションを再実行してください。",
					"情報",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);

				Close();
				return;
			}

			await _LoadSettingsAsync();

			_Aimp = new Aimp();

			_Aimp.Communicator.CurrentTrackChanged += async (track) =>
			{
				_UpdateFormTrackInfo(track);
				_UpdateAlbumArt();

				// auto posting
				if (checkBoxAutoPosting.Checked)
					await _PostAsync();
			};

			if (_Aimp.IsRunning)
			{
				_UpdateFormTrackInfo(_Aimp.CurrentTrack);
				_UpdateAlbumArt();
			}
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			_Aimp?.Dispose();
		}

		private async void buttonPostNowPlaying_Click(object sender, EventArgs e)
		{
			await _PostAsync();
		}

		private void pictureBoxAlbumArt_Click(object sender, EventArgs e)
		{
			if (_Aimp.AlbumArt != null)
			{
				_Aimp.AlbumArt.Save("temp.png", ImageFormat.Png);
				Process.Start("temp.png");
			}
		}

		private void checkBoxAutoPosting_CheckedChanged(object sender, EventArgs e)
		{
			buttonPostNowPlaying.Enabled = !checkBoxAutoPosting.Checked;
		}

		private async void buttonShowSettingWindow_Click(object sender, EventArgs e)
		{
			var settingWindow = new SettingWindow(_PostingFormat);

			if (settingWindow.ShowDialog() == DialogResult.OK)
			{
				_PostingFormat = settingWindow.PostingFormat;
				await _SaveSettingsAsync();
			}
		}

		#endregion Procedures
	}
}
