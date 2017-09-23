namespace Legato.TwitterSample
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.labelTrackNumber = new System.Windows.Forms.Label();
			this.labelTitle = new System.Windows.Forms.Label();
			this.labelArtist = new System.Windows.Forms.Label();
			this.labelAlbum = new System.Windows.Forms.Label();
			this.checkBoxNeedAlbumArt = new System.Windows.Forms.CheckBox();
			this.checkBoxAutoPosting = new System.Windows.Forms.CheckBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonShowSettingWindow = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button1.Location = new System.Drawing.Point(169, 138);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(107, 25);
			this.button1.TabIndex = 0;
			this.button1.Text = "NowPlaying";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(151, 151);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// labelTrackNumber
			// 
			this.labelTrackNumber.AutoSize = true;
			this.labelTrackNumber.Font = new System.Drawing.Font("メイリオ", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelTrackNumber.Location = new System.Drawing.Point(169, 12);
			this.labelTrackNumber.Name = "labelTrackNumber";
			this.labelTrackNumber.Size = new System.Drawing.Size(29, 23);
			this.labelTrackNumber.TabIndex = 2;
			this.labelTrackNumber.Text = "--.";
			// 
			// labelTitle
			// 
			this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTitle.AutoEllipsis = true;
			this.labelTitle.Font = new System.Drawing.Font("メイリオ", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelTitle.Location = new System.Drawing.Point(194, 12);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(153, 23);
			this.labelTitle.TabIndex = 3;
			this.labelTitle.Text = "title";
			// 
			// labelArtist
			// 
			this.labelArtist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelArtist.AutoEllipsis = true;
			this.labelArtist.Font = new System.Drawing.Font("メイリオ", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelArtist.Location = new System.Drawing.Point(169, 35);
			this.labelArtist.Name = "labelArtist";
			this.labelArtist.Size = new System.Drawing.Size(178, 23);
			this.labelArtist.TabIndex = 4;
			this.labelArtist.Text = "artist";
			// 
			// labelAlbum
			// 
			this.labelAlbum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelAlbum.AutoEllipsis = true;
			this.labelAlbum.Font = new System.Drawing.Font("メイリオ", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelAlbum.Location = new System.Drawing.Point(169, 58);
			this.labelAlbum.Name = "labelAlbum";
			this.labelAlbum.Size = new System.Drawing.Size(178, 23);
			this.labelAlbum.TabIndex = 5;
			this.labelAlbum.Text = "album";
			// 
			// checkBoxNeedAlbumArt
			// 
			this.checkBoxNeedAlbumArt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxNeedAlbumArt.AutoSize = true;
			this.checkBoxNeedAlbumArt.Location = new System.Drawing.Point(173, 116);
			this.checkBoxNeedAlbumArt.Name = "checkBoxNeedAlbumArt";
			this.checkBoxNeedAlbumArt.Size = new System.Drawing.Size(123, 16);
			this.checkBoxNeedAlbumArt.TabIndex = 6;
			this.checkBoxNeedAlbumArt.Text = "アルバムアートも投稿";
			this.checkBoxNeedAlbumArt.UseVisualStyleBackColor = true;
			// 
			// checkBoxAutoPosting
			// 
			this.checkBoxAutoPosting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxAutoPosting.AutoSize = true;
			this.checkBoxAutoPosting.Location = new System.Drawing.Point(173, 94);
			this.checkBoxAutoPosting.Name = "checkBoxAutoPosting";
			this.checkBoxAutoPosting.Size = new System.Drawing.Size(72, 16);
			this.checkBoxAutoPosting.TabIndex = 7;
			this.checkBoxAutoPosting.Text = "自動投稿";
			this.checkBoxAutoPosting.UseVisualStyleBackColor = true;
			this.checkBoxAutoPosting.CheckedChanged += new System.EventHandler(this.checkBoxAutoPosting_CheckedChanged);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Cursor = System.Windows.Forms.Cursors.Default;
			this.panel1.Location = new System.Drawing.Point(174, 87);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(174, 1);
			this.panel1.TabIndex = 8;
			// 
			// buttonShowSettingWindow
			// 
			this.buttonShowSettingWindow.Location = new System.Drawing.Point(282, 138);
			this.buttonShowSettingWindow.Name = "buttonShowSettingWindow";
			this.buttonShowSettingWindow.Size = new System.Drawing.Size(66, 25);
			this.buttonShowSettingWindow.TabIndex = 9;
			this.buttonShowSettingWindow.Text = "Settings...";
			this.buttonShowSettingWindow.UseVisualStyleBackColor = true;
			this.buttonShowSettingWindow.Click += new System.EventHandler(this.buttonShowSettingWindow_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(359, 172);
			this.Controls.Add(this.buttonShowSettingWindow);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.checkBoxAutoPosting);
			this.Controls.Add(this.checkBoxNeedAlbumArt);
			this.Controls.Add(this.labelAlbum);
			this.Controls.Add(this.labelArtist);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.labelTrackNumber);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.button1);
			this.MinimumSize = new System.Drawing.Size(375, 210);
			this.Name = "Form1";
			this.Text = "AIMP NowPlaying";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label labelTrackNumber;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.Label labelArtist;
		private System.Windows.Forms.Label labelAlbum;
		private System.Windows.Forms.CheckBox checkBoxNeedAlbumArt;
		private System.Windows.Forms.CheckBox checkBoxAutoPosting;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonShowSettingWindow;
	}
}

