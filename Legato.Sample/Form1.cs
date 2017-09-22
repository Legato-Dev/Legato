using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Console;

namespace Legato.Sample
{
	public partial class Form1 : Form
	{
		#region Field

		private Legato _Legato { get; set; }
		private int _SongPosition;

		#endregion Field

		public Form1()
		{
			InitializeComponent();

			_Legato = new Legato();
			_SongPosition = 0;

			Icon = Properties.Resources.legato;

			_UpdateAlbumArt();

			_Legato.Communicator.CurrentTrackChanged += () =>
			{
				_UpdateAlbumArt();
			};

			_Legato.Communicator.StatePropertyChanged += (state) =>
			{
				_UpdateAlbumArt();
			};

			_Legato.Communicator.PositionPropertyChanged += (position) => {
				_SongPosition = _Legato.Position;
				_UpdateSongPosition();
			};
		}

		/// <summary>
		/// 再生時間の表示を更新します
		/// </summary>
		private void _UpdateSongPosition()
		{
			var totalSec = _SongPosition / 1000;
			var min = totalSec / 60;
			var sec = totalSec % 60;

			CurrentPos.Text = $"{min:D2}:{sec:D2}";
		}

		/// <summary>
		/// フォームに表示されているアルバムアートを更新します
		/// </summary>
		private void _UpdateAlbumArt()
		{
			pictureBox1.Image = _Legato.AlbumArt ?? Properties.Resources.logo;
		}

		#region Procedures

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

		private void buttonFetch_Click(object sender, EventArgs e)
		{
			WriteLine($"IsRunning:{_Legato.IsRunning}");

			if (_Legato?.IsRunning ?? false)
			{
				WriteLine($"State:{_Legato.State}");
				WriteLine($"IsShuffle:{_Legato.IsShuffle}");
				WriteLine($"Volume:{_Legato.Volume}");
				WriteLine($"Position:{_Legato.Position} - {_Legato.Duration}");

				var track = _Legato.CurrentTrack;
				WriteLine($"Title:{track.Title}");
				WriteLine($"Artist:{track.Artist}");
				WriteLine($"Album:{track.Album}");

				pictureBox1.Image = _Legato.AlbumArt ?? Properties.Resources.logo;
			}
		}

		#endregion Procedures
	}
}
