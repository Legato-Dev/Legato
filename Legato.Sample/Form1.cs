using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Legato.Entities;

namespace Legato.Sample
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		#region Properties

		private NotifyIcon _NotifyIcon { get; set; }

		private AimpCommands _Commands { get; set; }
		private AimpProperties _Properties { get; set; }
		private AimpObserver _Observer { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Legatoに対するイベントリスナを追加します
		/// </summary>
		private void _AddLegatoEventListeners()
		{
			_Observer.Subscribed += () =>
			{
				Console.WriteLine("接続されました");
			};

			_Observer.Unsubscribed += () =>
			{
				Console.WriteLine("切断されました");
			};

			_Observer.CurrentTrackChanged += async (track) =>
			{
				var os = Environment.OSVersion;

				// トースト通知
				if (os.Version.Major >= 6 && os.Version.Minor >= 2)
				{
					_NotifyIcon.BalloonTipTitle = $"Legato NowPlaying\r\n{track.Title} - {track.Artist}";
					_NotifyIcon.BalloonTipText = $"Album : {track.Album}";
					Debug.WriteLine("トースト通知が表示されました。");
				}
				// バルーン通知
				else
				{
					_NotifyIcon.BalloonTipTitle = $"Legato NowPlaying";
					_NotifyIcon.BalloonTipText = $"{track.Title} - {track.Artist}\r\nAlbum : {track.Album}";
					Debug.WriteLine("バルーン通知が表示されました。");
				}
				_NotifyIcon.ShowBalloonTip(10000);

				Console.WriteLine("CurrentTrackChanged:");
				Console.Write($"Title:{track.Title} ");
				Console.Write($"Artist:{track.Artist} ");
				Console.Write($"Album:{track.Album} ");
				Console.Write($"Genre:{track.Genre} ");
				Console.Write($"Duration:{track.Duration} ");
				Console.Write($"TrackNumber:{track.TrackNumber} ");
				Console.Write($"Year:{track.Year} ");
				Console.Write($"ChannelType:{track.ChannelType} ");
				Console.Write($"BitRate:{track.BitRate} ");
				Console.Write($"SampleRate:{track.SampleRate} ");
				Console.WriteLine();

				await _UpdateAlbumArt();
			};

			_Observer.StatePropertyChanged += (state) =>
			{
				Console.WriteLine($"StatePropertyChanged: {state}");
			};

			_Observer.PositionPropertyChanged += (position) =>
			{
				var totalSec = position / 1000;
				var min = totalSec / 60;
				var sec = totalSec % 60;

				CurrentPos.Text = $"{min:D2}:{sec:D2}";
			};
		}

		/// <summary>
		/// フォームに表示されているアルバムアートを更新します
		/// </summary>
		private async Task _UpdateAlbumArt()
		{
			if (_Properties?.IsRunning ?? false)
			{
				try
				{
					pictureBox1.Image = (await _Properties.AlbumArt) ?? Properties.Resources.logo;
				}
				catch (Exception ex) when (ex is ApplicationException || ex is NotSupportedException)
				{
					Console.WriteLine(ex.Message);
				}
				catch (Exception ex)
				{
					Console.WriteLine("unknown exception: " + ex.Message);
				}
			}
			else
			{
				pictureBox1.Image = Properties.Resources.logo;
			}
		}

		#endregion Methods

		#region Procedures

		private async void Form1_Load(object sender, EventArgs e)
		{
			Icon = Properties.Resources.legato;

			_NotifyIcon = new NotifyIcon();
			_NotifyIcon.Icon = Properties.Resources.legato;

			_Properties = new AimpProperties();
			_Commands = new AimpCommands();
			_Observer = new AimpObserver();

			_AddLegatoEventListeners();
			await _UpdateAlbumArt();
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			_Observer.Dispose();
		}

		private void buttonPlayPause_Click(object sender, EventArgs e)
		{
			if (_Properties?.IsRunning ?? false)
			{
				_Commands.PlayPause();
			}
		}

		private void buttonNext_Click(object sender, EventArgs e)
		{
			if (_Properties?.IsRunning ?? false)
			{
				_Commands.Next();
			}
		}

		private void buttonPrev_Click(object sender, EventArgs e)
		{
			if (_Properties?.IsRunning ?? false)
			{
				_Commands.Prev();
			}
		}

		private void buttonPlayerInfo_Click(object sender, EventArgs e)
		{
			Console.Write($"IsRunning:{_Properties.IsRunning} ");

			if (_Properties?.IsRunning ?? false)
			{
				Console.Write($"State:{_Properties.State} ");
				Console.Write($"Volume:{_Properties.Volume} ");
				Console.Write($"IsShuffle:{_Properties.IsShuffle} ");
				Console.Write($"IsRepeat:{_Properties.IsRepeat} ");
				Console.Write($"IsMute:{_Properties.IsMute} ");
				Console.Write($"Position:{_Properties.Position} ");
			}
			Console.WriteLine();
		}

		#endregion Procedures
	}
}
