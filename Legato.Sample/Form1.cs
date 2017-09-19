using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using static System.Console;
using System.Threading.Tasks;

namespace Legato.Sample
{
	public partial class Form1 : Form
	{
		#region Field

		private System.Timers.Timer _timer;
		private uint TimerCounter;
		private uint MinuteCounter;
		private uint SecondCounter;
		private bool OnlyTimerStartFlg;

		#endregion

		#region Constants

		const uint msConvertSec = 1000;
		const uint betweenMin = 59;

		#endregion

		public Form1()
		{
			InitializeComponent();
			Initialize();
		}

		public void Initialize()
		{
			_timer = new System.Timers.Timer();
			TimerCounter = 0;
			MinuteCounter = 0;
			SecondCounter = 0;
			OnlyTimerStartFlg = false;
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

		private void DisSigneture()
		{
			TimerCounter = 0;
			MinuteCounter = 0;
			SecondCounter = 0;
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

				var albumArt = _Legato.AlbumArt;
				WriteLine($"AlbumArt: {albumArt != null} {albumArt.Length}");

				WriteLine($"Information:{_Legato.CurrentTrack.Title}");
				WriteLine($"Information:{_Legato.CurrentTrack.Artist}");
				WriteLine($"Information:{_Legato.CurrentTrack.Album}");

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
				{
					_Legato.Pause();
					_timer.Stop();
				}
				else
				{
					Task.Run(async () =>
					{
						_Legato.Play();

						var time = _Legato.Position / msConvertSec;
						var customize = _Legato.Position - (uint)time * msConvertSec;

						await Task.Delay(new TimeSpan(customize));
						TimerCounter = (uint)time;

						CurrentPos.Invoke((Action)(() =>
						{
							CurrentPos.Text = $"Duration = {MinuteCounter} : {SecondCounter}";
						}));

						if (OnlyTimerStartFlg == false)
						{
							_timer.Elapsed += (s, v) =>
							{
								++TimerCounter;
								CurrentPos.Invoke((Action)(() =>
								{
									if (SecondCounter == betweenMin)
									{
										++MinuteCounter;
										SecondCounter = 0;
									}
									else
										++SecondCounter;

									CurrentPos.Text = $"Duration = {MinuteCounter} : {string.Format("{0:D2}", SecondCounter)}";
								}));
							};

							_timer.Interval = msConvertSec;
							OnlyTimerStartFlg = true;
						}

						_timer.Start();
					});
				}
			}
		}

		private void buttonNext_Click(object sender, EventArgs e)
		{
			if (_Legato?.IsRunning ?? false)
			{
				_Legato.Next();
				DisSigneture();
			}
		}

		private void buttonPrev_Click(object sender, EventArgs e)
		{
			if (_Legato?.IsRunning ?? false)
			{
				_Legato.Prev();
				DisSigneture();
			}
		}
	}
}