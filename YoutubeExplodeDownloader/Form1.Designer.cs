namespace YoutubeExplodeDownloader
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            SearchBtn = new Button();
            UrlTxt = new TextBox();
            PathTxt = new TextBox();
            pictureBox1 = new PictureBox();
            DownloadProgress = new ProgressBar();
            PlaylistProgress = new ProgressBar();
            DownloadLbl = new Label();
            PlaylistLbl = new Label();
            UrlLbl = new Label();
            vScrollBar1 = new VScrollBar();
            BatchLbl = new Label();
            label2 = new Label();
            panel1 = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(354, 47);
            label1.Name = "label1";
            label1.Size = new Size(97, 15);
            label1.TabIndex = 0;
            label1.Text = "Download Folder";
            // 
            // SearchBtn
            // 
            SearchBtn.Location = new Point(713, 12);
            SearchBtn.Name = "SearchBtn";
            SearchBtn.Size = new Size(75, 76);
            SearchBtn.TabIndex = 1;
            SearchBtn.Text = "Search";
            SearchBtn.UseVisualStyleBackColor = true;
            SearchBtn.Click += SearchBtn_Click;
            // 
            // UrlTxt
            // 
            UrlTxt.Location = new Point(12, 12);
            UrlTxt.Name = "UrlTxt";
            UrlTxt.Size = new Size(695, 23);
            UrlTxt.TabIndex = 2;
            // 
            // PathTxt
            // 
            PathTxt.Location = new Point(354, 65);
            PathTxt.Name = "PathTxt";
            PathTxt.Size = new Size(353, 23);
            PathTxt.TabIndex = 3;
            PathTxt.Click += PathTxt_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 38);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(336, 279);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // DownloadProgress
            // 
            DownloadProgress.Location = new Point(688, 94);
            DownloadProgress.Name = "DownloadProgress";
            DownloadProgress.Size = new Size(100, 23);
            DownloadProgress.Step = 1;
            DownloadProgress.TabIndex = 5;
            // 
            // PlaylistProgress
            // 
            PlaylistProgress.Location = new Point(354, 123);
            PlaylistProgress.Name = "PlaylistProgress";
            PlaylistProgress.Size = new Size(434, 23);
            PlaylistProgress.Step = 1;
            PlaylistProgress.TabIndex = 6;
            // 
            // DownloadLbl
            // 
            DownloadLbl.AutoSize = true;
            DownloadLbl.Location = new Point(583, 94);
            DownloadLbl.Name = "DownloadLbl";
            DownloadLbl.Size = new Size(99, 15);
            DownloadLbl.TabIndex = 7;
            DownloadLbl.Text = "Downloading -->";
            DownloadLbl.Visible = false;
            // 
            // PlaylistLbl
            // 
            PlaylistLbl.AutoSize = true;
            PlaylistLbl.Location = new Point(606, 109);
            PlaylistLbl.Name = "PlaylistLbl";
            PlaylistLbl.Size = new Size(24, 15);
            PlaylistLbl.TabIndex = 8;
            PlaylistLbl.Text = "0/0";
            // 
            // UrlLbl
            // 
            UrlLbl.AutoSize = true;
            UrlLbl.Font = new Font("Microsoft Sans Serif", 30F, FontStyle.Regular, GraphicsUnit.Point);
            UrlLbl.Location = new Point(397, 200);
            UrlLbl.Name = "UrlLbl";
            UrlLbl.Size = new Size(271, 46);
            UrlLbl.TabIndex = 9;
            UrlLbl.Text = "Checking Link";
            UrlLbl.Visible = false;
            // 
            // vScrollBar1
            // 
            vScrollBar1.Location = new Point(494, 91);
            vScrollBar1.Maximum = 8;
            vScrollBar1.Minimum = -999;
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new Size(22, 26);
            vScrollBar1.TabIndex = 10;
            vScrollBar1.Value = -1;
            vScrollBar1.ValueChanged += vScrollBar1_ValueChanged;
            // 
            // BatchLbl
            // 
            BatchLbl.AutoSize = true;
            BatchLbl.Location = new Point(2, 4);
            BatchLbl.Name = "BatchLbl";
            BatchLbl.Size = new Size(13, 15);
            BatchLbl.TabIndex = 11;
            BatchLbl.Text = "1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(354, 91);
            label2.Name = "label2";
            label2.Size = new Size(141, 15);
            label2.TabIndex = 12;
            label2.Text = "Simultaneous Downloads";
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(BatchLbl);
            panel1.Location = new Point(518, 92);
            panel1.Name = "panel1";
            panel1.Size = new Size(40, 24);
            panel1.TabIndex = 13;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 333);
            Controls.Add(label2);
            Controls.Add(vScrollBar1);
            Controls.Add(UrlLbl);
            Controls.Add(DownloadLbl);
            Controls.Add(PlaylistProgress);
            Controls.Add(DownloadProgress);
            Controls.Add(pictureBox1);
            Controls.Add(PathTxt);
            Controls.Add(UrlTxt);
            Controls.Add(SearchBtn);
            Controls.Add(label1);
            Controls.Add(PlaylistLbl);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button SearchBtn;
        private TextBox UrlTxt;
        private TextBox PathTxt;
        private PictureBox pictureBox1;
        private ProgressBar DownloadProgress;
        private ProgressBar PlaylistProgress;
        private Label DownloadLbl;
        private Label PlaylistLbl;
        private Label UrlLbl;
        private VScrollBar vScrollBar1;
        private Label BatchLbl;
        private Label label2;
        private Panel panel1;
    }
}