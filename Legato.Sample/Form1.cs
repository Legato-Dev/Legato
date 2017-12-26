using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Legato.Sample {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}

		#region Properties

		private NotifyIcon _NotifyIcon { get; set; } = new NotifyIcon { Visible = true, Icon = Properties.Resources.legato };

		private AimpCommands _Commands { get; set; } = new AimpCommands();
		private AimpProperties _Properties { get; set; } = new AimpProperties();
		private AimpObserver _Observer { get; set; } = new AimpObserver();

		#endregion Properties

		#region Methods

		private void Form1_Load(object sender, EventArgs e) {
			Icon = Properties.Resources.legato;

			// 各種イベントハンドラの登録
			_Observer.Subscribed += _Subscribed;
			_Observer.Unsubscribed += _Unsubscribed;
			_Observer.CurrentTrackChanged += _CurrentTrackChanged;
			_Observer.StatePropertyChanged += _StatePropertyChanged;
			_Observer.PositionPropertyChanged += _PositionPropertyChanged;

			// 直接イベントハンドラを呼び出して画面を初期化
			_CurrentTrackChanged(_Properties.CurrentTrack);
			_StatePropertyChanged(_Properties.State);
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
			_Observer.Dispose();
		}

		private void buttonPlayPause_Click(object sender, EventArgs e) {
			if (_Properties.IsRunning) {
				_Commands.PlayPause();
			}
		}

		private void buttonNext_Click(object sender, EventArgs e) {
			if (_Properties.IsRunning) {
				_Commands.Next();
			}
		}

		private void buttonPrev_Click(object sender, EventArgs e) {
			if (_Properties.IsRunning) {
				_Commands.Prev();
			}
		}

		private void _Subscribed() {
			Console.WriteLine("接続されました");
		}

		private void _Unsubscribed() {
			Console.WriteLine("切断されました");
		}

		private void _CurrentTrackChanged(Interop.AimpRemote.Entities.TrackInfo track) {
			var os = Environment.OSVersion;

			// トースト通知
			if (os.Version.Major >= 6 && os.Version.Minor >= 2) {
				_NotifyIcon.BalloonTipTitle = $"Legato\r\n{track.Title} - {track.Artist}";
				_NotifyIcon.BalloonTipText = $"Album : {track.Album}";
				Debug.WriteLine("トースト通知");
			}
			// バルーン通知
			else {
				_NotifyIcon.BalloonTipTitle = $"Legato";
				_NotifyIcon.BalloonTipText = $"{track.Title} - {track.Artist}\r\nAlbum : {track.Album}";
				Debug.WriteLine("バルーン通知");
			}
			_NotifyIcon.ShowBalloonTip(1000);

			// 曲情報ラベルのTextを更新
			titleLabel.Text = track.Title;
			artistLabel.Text = track.Artist;
			albumLabel.Text = track.Album;
		}

		private void _StatePropertyChanged(Interop.AimpRemote.Enum.PlayerState state) {
			// 再生・停止ボタンのTextを更新
			if (state == Interop.AimpRemote.Enum.PlayerState.Playing) {
				buttonPlayPause.Text = "Play";
			}
			else if (state == Interop.AimpRemote.Enum.PlayerState.Stopped) {
				buttonPlayPause.Text = "Pause";
			}
			else {
				buttonPlayPause.Text = "Play / Pause";
			}
		}

		private void _PositionPropertyChanged(int position) {
			var totalSec = position / 1000;
			var min = totalSec / 60;
			var sec = totalSec % 60;

			// 再生時間ラベルのTextを更新
			CurrentPos.Text = $"{min:D2}:{sec:D2}";
		}

		#endregion Methods
	}
}
