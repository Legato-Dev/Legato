using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Legato.Sample
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		#region Properties

		private Aimp _Aimp { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Legatoに対するイベントリスナを追加します
		/// </summary>
		private void _AddLegatoEventListeners()
		{
			_Aimp.Subscribed += () =>
			{
				Console.WriteLine("接続されました");
			};

			_Aimp.Unsubscribed += () =>
			{
				Console.WriteLine("切断されました");
			};

			_Aimp.Communicator.CurrentTrackChanged += (track) =>
			{
				
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

				_UpdateAlbumArt();
			};

			_Aimp.Communicator.StatePropertyChanged += (state) =>
			{
				Console.WriteLine($"StatePropertyChanged: {state}");
			};

			_Aimp.Communicator.PositionPropertyChanged += (position) =>
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
		private void _UpdateAlbumArt()
		{
			if (_Aimp?.IsRunning ?? false)
			{
				try
				{
					pictureBox1.Image = _Aimp.AlbumArt ?? Properties.Resources.logo;
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

		private void Form1_Load(object sender, EventArgs e)
		{
			Icon = Properties.Resources.legato;

			_Aimp = new Aimp();
			_AddLegatoEventListeners();
			_UpdateAlbumArt();
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			_Aimp.Dispose();
		}

		private void buttonPlayPause_Click(object sender, EventArgs e)
		{
			if (_Aimp?.IsRunning ?? false)
			{
				_Aimp.PlayPause();
			}
		}

		private void buttonNext_Click(object sender, EventArgs e)
		{
			if (_Aimp?.IsRunning ?? false)
			{
				_Aimp.Next();
			}
		}

		private void buttonPrev_Click(object sender, EventArgs e)
		{
			if (_Aimp?.IsRunning ?? false)
			{
				_Aimp.Prev();
			}
		}

		private void buttonPlayerInfo_Click(object sender, EventArgs e)
		{
			Console.Write($"IsRunning:{_Aimp.IsRunning} ");

			if (_Aimp?.IsRunning ?? false)
			{
				Console.Write($"State:{_Aimp.State} ");
				Console.Write($"Volume:{_Aimp.Volume} ");
				Console.Write($"IsShuffle:{_Aimp.IsShuffle} ");
				Console.Write($"IsRepeat:{_Aimp.IsRepeat} ");
				Console.Write($"IsMute:{_Aimp.IsMute} ");
				Console.Write($"Position:{_Aimp.Position} ");
			}
			Console.WriteLine();
		}

		#endregion Procedures
	}
}
