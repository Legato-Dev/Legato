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

		private Legato _Accesser { get; set; }

		private void Form1_Load(object sender, EventArgs e)
		{
			_Accesser = new Legato();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			_Accesser = null;
		}

		private void buttonGet_Click(object sender, EventArgs e)
		{
			if (_Accesser != null)
			{
				Console.WriteLine($"State:{_Accesser.State}");
				Console.WriteLine($"IsShuffle:{_Accesser.IsShuffle}");
				Console.WriteLine($"Volume:{_Accesser.Volume}");
				Console.WriteLine($"Position:{_Accesser.Position} - {_Accesser.Duration}");
			}
		}

		private void buttonPlayPause_Click(object sender, EventArgs e)
		{
			if (_Accesser != null)
			{
					if (_Accesser.State == Interop.Aimp.Enum.PlayerState.Playing)
					_Accesser.Pause();
				else
					_Accesser.Play();
			}
		}
	}
}
