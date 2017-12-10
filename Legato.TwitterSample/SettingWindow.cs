using System;
using System.Windows.Forms;

namespace Legato.TwitterSample {
	public partial class SettingWindow : Form {
		public string PostingFormat { get; set; }

		public SettingWindow(string postingFormat) {
			InitializeComponent();

			PostingFormat = postingFormat;
		}

		private void SettingWindow_Load(object sender, EventArgs e) {
			textBoxPostingFormat.Text = PostingFormat;
		}

		private void buttonOk_Click(object sender, EventArgs e) {
			PostingFormat = textBoxPostingFormat.Text;

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
