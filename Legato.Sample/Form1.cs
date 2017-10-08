using System;
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

		private Legato _Legato { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Legatoに対するイベントリスナを追加します
		/// </summary>
		private void _AddLegatoEventListeners()
		{
			_Legato.Subscribed += () =>
			{
				Console.WriteLine("接続されました");
			};

			_Legato.Unsubscribed += () =>
			{
				Console.WriteLine("切断されました");
			};

			_Legato.Communicator.CurrentTrackChanged += (track) =>
			{
				// バルーン&トースト通知
				notifyIcon.BalloonTipTitle = $"Legato NowPlaying\r\n{track.Title} - {track.Artist}";
				notifyIcon.BalloonTipText = $"Album : {track.Album}";
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

			_Legato.Communicator.StatePropertyChanged += (state) =>
			{
				Console.WriteLine($"StatePropertyChanged: {state}");
			};

			_Legato.Communicator.PositionPropertyChanged += (position) =>
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
			if (_Legato?.IsRunning ?? false)
			{
				try
				{
					pictureBox1.Image = _Legato.AlbumArt ?? Properties.Resources.logo;
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

			_Legato = new Legato();
			_AddLegatoEventListeners();
			_UpdateAlbumArt();
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			_Legato.Dispose();
		}

		private void buttonPlayPause_Click(object sender, EventArgs e)
		{
			if (_Legato?.IsRunning ?? false)
			{
				_Legato.PlayPause();
			}
		}

		private void buttonNext_Click(object sender, EventArgs e)
		{
			if (_Legato?.IsRunning ?? false)
			{
				_Legato.Next();
			}
		}

		private void buttonPrev_Click(object sender, EventArgs e)
		{
			if (_Legato?.IsRunning ?? false)
			{
				_Legato.Prev();
			}
		}

		private void buttonPlayerInfo_Click(object sender, EventArgs e)
		{
			Console.Write($"IsRunning:{_Legato.IsRunning} ");

			if (_Legato?.IsRunning ?? false)
			{
				Console.Write($"State:{_Legato.State} ");
				Console.Write($"Volume:{_Legato.Volume} ");
				Console.Write($"IsShuffle:{_Legato.IsShuffle} ");
				Console.Write($"IsRepeat:{_Legato.IsRepeat} ");
				Console.Write($"IsMute:{_Legato.IsMute} ");
				Console.Write($"Position:{_Legato.Position} ");
			}
			Console.WriteLine();
		}

		#endregion Procedures
	}
}
