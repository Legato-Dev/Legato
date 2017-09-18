using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Legato.Sample
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private Legato _Legato { get; set; }

		private void Form1_Load(object sender, EventArgs e)
		{
			_Legato = new Legato();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			_Legato = null;
		}

		private void buttonGet_Click(object sender, EventArgs e)
		{
			Console.WriteLine($"IsRunning:{_Legato.IsRunning}");

			if (_Legato?.IsRunning ?? false)
			{
				Console.WriteLine($"State:{_Legato.State}");
				Console.WriteLine($"IsShuffle:{_Legato.IsShuffle}");
				Console.WriteLine($"Volume:{_Legato.Volume}");
				Console.WriteLine($"Position:{_Legato.Position} - {_Legato.Duration}");

				var albumArt = _Legato.AlbumArt;
				Console.WriteLine($"AlbumArt: {albumArt != null} {albumArt.Length}");

				if ((albumArt?.Length ?? 0) != 0)
				{
					using (var memory = new MemoryStream())
					{
						memory.Write(albumArt, 0, albumArt.Length);
						pictureBox1.Image = Image.FromStream(memory);
					}
				}
			}
		}

		private void buttonPlayPause_Click(object sender, EventArgs e)
		{
			if (_Legato?.IsRunning ?? false)
			{
				if (_Legato.State == Interop.Aimp.Enum.PlayerState.Playing)
					_Legato.Pause();
				else
					_Legato.Play();
			}
		}
	}
}
