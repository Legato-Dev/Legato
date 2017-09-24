using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreTweet;
using JsonFx.Json;

namespace Legato.TwitterSample
{
	public partial class Form1 : Form
	{
		#region secretInfos
		private static readonly string _ConsumerKey = "your ck";
		private static readonly string _ConsumerSecret = "your cs";
		private static readonly string _AccessToken = "your at";
		private static readonly string _AccessTokenSecret = "your ats";
		#endregion secretInfos

		private Legato _Legato { get; set; }
		private Tokens _Twitter { get; set; }
		private string PostingFormat { get; set; } = "";

		public Form1()
		{
			InitializeComponent();

			_Legato = new Legato();
			_Twitter = Tokens.Create(_ConsumerKey, _ConsumerSecret, _AccessToken, _AccessTokenSecret);
			pictureBox1.Image = _Legato.AlbumArt;

			_Legato.Communicator.CurrentTrackChanged += async (track) =>
			{
				labelTrackNumber.Text = $"{track.TrackNumber:D2}.";
				labelTitle.Text = track.Title;
				labelArtist.Text = track.Artist;
				labelAlbum.Text = track.Album;

				pictureBox1.Image = _Legato.AlbumArt ?? Properties.Resources.logo;

				if (checkBoxAutoPosting.Checked)
				{
					await _PostAsync();
				}
			};

			{
				if (_Legato?.IsRunning ?? false)
				{
					var track = _Legato.CurrentTrack;
					labelTrackNumber.Text = $"{track.TrackNumber:D2}.";
					labelTitle.Text = track.Title;
					labelArtist.Text = track.Artist;
					labelAlbum.Text = track.Album;
				}

				if (_Legato?.IsRunning ?? false)
				{
					if (_Legato.AlbumArt == null)
					{
						pictureBox1.Image = Properties.Resources.logo;

						var defaultArt = Properties.Resources.logo;
						defaultArt.Save("tempDefault.png", ImageFormat.Png);
					}
					else
						pictureBox1.Image = _Legato.AlbumArt;
				}
			}
		}

		private async Task _PostAsync()
		{
			try
			{
				var track = _Legato.CurrentTrack;

				// 投稿内容を構築
				var stringBuilder = new StringBuilder(PostingFormat);
				stringBuilder = stringBuilder.Replace("{Title}", "{0}");
				stringBuilder = stringBuilder.Replace("{Artist}", "{1}");
				stringBuilder = stringBuilder.Replace("{Album}", "{2}");
				stringBuilder = stringBuilder.Replace("{TrackNum}", "{3:D2}");
				var text = string.Format(stringBuilder.ToString(), track.Title, track.Artist, track.Album, track.TrackNumber);

				if (checkBoxNeedAlbumArt.Checked && _Legato.AlbumArt == null)
				{
					await _Twitter.Statuses.UpdateWithMediaAsync(status: text, media: new FileInfo("tempDefault.png"));
				}
				else if (checkBoxNeedAlbumArt.Checked && _Legato.AlbumArt != null)
				{
					using (var memory = new MemoryStream())
						_Legato.AlbumArt.Save("temp.png", ImageFormat.Png);

					await _Twitter.Statuses.UpdateWithMediaAsync(text, new FileInfo("temp.png"));
				}
				else
				{
					_Twitter.Statuses.Update(text);
				}

				Console.WriteLine("Twitter への投稿が完了しました");
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
			}
		}

		private async void Form1_Load(object sender, EventArgs e)
		{
			var defaultFormat = "{TrackNum}. {Title}\r\nArtist: {Artist}\r\nAlbum: {Album}\r\n#nowplaying";

			try
			{
				string jsonString = null;

				Icon = Properties.Resources.legato;

				using (var streamReader = new StreamReader("settings.json", Encoding.UTF8))
					jsonString = await streamReader.ReadToEndAsync();

				var jReader = new JsonReader();
				dynamic json = jReader.Read(jsonString);
				PostingFormat = json.format ?? defaultFormat;
			}
			catch (FileNotFoundException)
			{
				PostingFormat = defaultFormat;
			}
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			var jsonWriter = new JsonWriter();
			var jsonString = jsonWriter.Write(new { format = PostingFormat });

			using (var streamWriter = new StreamWriter("settings.json", false, Encoding.UTF8))
				streamWriter.WriteAsync(jsonString);
		}

		private async void nowPlaying_Click(object sender, EventArgs e)
		{
			await _PostAsync();
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			if (_Legato.AlbumArt == null)
				Process.Start("tempDefault.png");
			else
			{
				_Legato.AlbumArt.Save("temp.png", ImageFormat.Png);
				Process.Start("temp.png");
			}
		}

		private void checkBoxAutoPosting_CheckedChanged(object sender, EventArgs e)
		{
			nowPlaying.Enabled = !checkBoxAutoPosting.Checked;
		}

		private void buttonShowSettingWindow_Click(object sender, EventArgs e)
		{
			var settingWindow = new SettingWindow(PostingFormat);
			if (settingWindow.ShowDialog() == DialogResult.OK)
			{
				PostingFormat = settingWindow.PostingFormat;
			}
		}
	}
}
