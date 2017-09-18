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
			this.buttonGet = new System.Windows.Forms.Button();
			this.buttonPlayPause = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonGet
			// 
			this.buttonGet.Location = new System.Drawing.Point(12, 12);
			this.buttonGet.Name = "buttonGet";
			this.buttonGet.Size = new System.Drawing.Size(97, 25);
			this.buttonGet.TabIndex = 2;
			this.buttonGet.Text = "Get player info";
			this.buttonGet.UseVisualStyleBackColor = true;
			this.buttonGet.Click += new System.EventHandler(this.buttonGet_Click);
			// 
			// buttonPlayPause
			// 
			this.buttonPlayPause.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonPlayPause.Location = new System.Drawing.Point(113, 117);
			this.buttonPlayPause.Name = "buttonPlayPause";
			this.buttonPlayPause.Size = new System.Drawing.Size(92, 25);
			this.buttonPlayPause.TabIndex = 3;
			this.buttonPlayPause.Text = "Play / Pause";
			this.buttonPlayPause.UseVisualStyleBackColor = true;
			this.buttonPlayPause.Click += new System.EventHandler(this.buttonPlayPause_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(318, 154);
			this.Controls.Add(this.buttonPlayPause);
			this.Controls.Add(this.buttonGet);
			this.Name = "Form1";
			this.Text = "Legato.Sample.Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button buttonGet;
		private System.Windows.Forms.Button buttonPlayPause;
	}
}

