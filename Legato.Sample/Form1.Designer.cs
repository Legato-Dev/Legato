namespace Legato.Sample
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
			this.buttonPlayerInfo = new System.Windows.Forms.Button();
			this.buttonPlayPause = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.buttonNext = new System.Windows.Forms.Button();
			this.buttonPrev = new System.Windows.Forms.Button();
			this.CurrentPos = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonPlayerInfo
			// 
			this.buttonPlayerInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonPlayerInfo.Location = new System.Drawing.Point(12, 373);
			this.buttonPlayerInfo.Name = "buttonPlayerInfo";
			this.buttonPlayerInfo.Size = new System.Drawing.Size(80, 25);
			this.buttonPlayerInfo.TabIndex = 2;
			this.buttonPlayerInfo.Text = "PlayerInfo";
			this.buttonPlayerInfo.UseVisualStyleBackColor = true;
			this.buttonPlayerInfo.Click += new System.EventHandler(this.buttonPlayerInfo_Click);
			// 
			// buttonPlayPause
			// 
			this.buttonPlayPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonPlayPause.Location = new System.Drawing.Point(267, 373);
			this.buttonPlayPause.Name = "buttonPlayPause";
			this.buttonPlayPause.Size = new System.Drawing.Size(83, 25);
			this.buttonPlayPause.TabIndex = 3;
			this.buttonPlayPause.Text = "Play / Pause";
			this.buttonPlayPause.UseVisualStyleBackColor = true;
			this.buttonPlayPause.Click += new System.EventHandler(this.buttonPlayPause_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(391, 349);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			// 
			// buttonNext
			// 
			this.buttonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonNext.Location = new System.Drawing.Point(356, 373);
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.Size = new System.Drawing.Size(47, 25);
			this.buttonNext.TabIndex = 5;
			this.buttonNext.Text = "Next";
			this.buttonNext.UseVisualStyleBackColor = true;
			this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
			// 
			// buttonPrev
			// 
			this.buttonPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonPrev.Location = new System.Drawing.Point(214, 373);
			this.buttonPrev.Name = "buttonPrev";
			this.buttonPrev.Size = new System.Drawing.Size(47, 25);
			this.buttonPrev.TabIndex = 6;
			this.buttonPrev.Text = "Prev";
			this.buttonPrev.UseVisualStyleBackColor = true;
			this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
			// 
			// CurrentPos
			// 
			this.CurrentPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.CurrentPos.AutoSize = true;
			this.CurrentPos.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.CurrentPos.Location = new System.Drawing.Point(98, 379);
			this.CurrentPos.Name = "CurrentPos";
			this.CurrentPos.Size = new System.Drawing.Size(31, 12);
			this.CurrentPos.TabIndex = 7;
			this.CurrentPos.Text = "--:--";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(415, 410);
			this.Controls.Add(this.CurrentPos);
			this.Controls.Add(this.buttonPrev);
			this.Controls.Add(this.buttonNext);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.buttonPlayPause);
			this.Controls.Add(this.buttonPlayerInfo);
			this.MinimumSize = new System.Drawing.Size(353, 85);
			this.Name = "Form1";
			this.Text = "Legato.Sample";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button buttonPlayerInfo;
		private System.Windows.Forms.Button buttonPlayPause;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button buttonNext;
		private System.Windows.Forms.Button buttonPrev;
		private System.Windows.Forms.Label CurrentPos;
	}
}

