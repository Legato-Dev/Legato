using System;
using System.Windows.Forms;
using static System.Console;

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
			_Legato.Connected += () =>
			{
				WriteLine("接続されました");
			};

			_Legato.Disconnected += () =>
			{
				WriteLine("切断されました");
			};

			_Legato.Communicator.CurrentTrackChanged += (track) =>
			{
				WriteLine("CurrentTrackChanged:");
				Write($"Title:{track.Title} ");
				Write($"Artist:{track.Artist} ");
				Write($"Album:{track.Album} ");
				Write($"Genre:{track.Genre} ");
				Write($"Duration:{track.Duration} ");
				Write($"TrackNumber:{track.TrackNumber} ");
				Write($"Year:{track.Year} ");
				Write($"ChannelType:{track.ChannelType} ");
				Write($"BitRate:{track.BitRate} ");
				Write($"SampleRate:{track.SampleRate} ");
				WriteLine();

				_UpdateAlbumArt();
			};

			_Legato.Communicator.StatePropertyChanged += (state) =>
			{
				WriteLine($"StatePropertyChanged: {state}");
				_UpdateAlbumArt();
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
				pictureBox1.Image = _Legato.AlbumArt ?? Properties.Resources.logo;
			else
				pictureBox1.Image = Properties.Resources.logo;
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
			Write($"IsRunning:{_Legato.IsRunning} ");

			if (_Legato?.IsRunning ?? false)
			{
				Write($"State:{_Legato.State} ");
				Write($"Volume:{_Legato.Volume} ");
				Write($"IsShuffle:{_Legato.IsShuffle} ");
				Write($"IsRepeat:{_Legato.IsRepeat} ");
				Write($"IsMute:{_Legato.IsMute} ");
				Write($"Position:{_Legato.Position} ");
			}
			WriteLine();
		}

		#endregion Procedures
	}
}
