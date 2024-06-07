using TagLib;
using System.Text.RegularExpressions;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;
using YoutubeExplode.Converter;

namespace YoutubeExplodeDownloader
{
    public partial class Form1 : Form
    {

        YoutubeClient youtube;
        //Cookie cookies = new Cookie();

        public Form1()
        {
            InitializeComponent();
            //youtube = new YoutubeClient(cookies);
            youtube = new YoutubeClient();
        }

        private async void GetVideo(string url)
        {
            DownloadProgress.Value = 0;
            DownloadProgress.Refresh();

            StreamManifest streamManifest;
            try
            {
                streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
            }
            catch (Exception)
            {
                try
                {
                    GetPlaylist(url);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                return;
            }
            UrlLbl.Visible = false;
            UrlLbl.Refresh();

            DownloadLbl.Visible = true;
            DownloadLbl.Refresh();

            var video = await youtube.Videos.GetAsync(url);
            var title = Regex.Replace(video.Title, @"[^a-zA-Z0-9\-]", "");
            pictureBox1.ImageLocation = video.Thumbnails.Where(t => t.Url.EndsWith(".jpg")).GetWithHighestResolution().Url;
            pictureBox1.Refresh();

            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            var progress = new Progress<double>(p =>
            {
                DownloadProgress.Value = Convert.ToInt32(p * 100);
            }); 
            if (!Directory.GetFiles(PathTxt.Text).Contains($"{PathTxt.Text}\\{title}.mp3"))
                await youtube.Videos.DownloadAsync(url, $"{PathTxt.Text.ToString()}\\{title}.mp3", progress);
            
            pictureBox1.Refresh();
            AddCover(title, video);

            DownloadLbl.Visible = false;
            DownloadLbl.Refresh();
        }

        private void AddCover(string title, Video video)
        {
            var tfile = TagLib.File.Create($"{PathTxt.Text}\\{title}.mp3");
            tfile.Tag.Pictures = new IPicture[] { new Picture(new ByteVector((byte[])new ImageConverter().ConvertTo(pictureBox1.Image, typeof(byte[])))) };
            tfile.Tag.Title = video.Title;
            tfile.Tag.DateTagged = DateTime.Now;
            tfile.Save();
            tfile.Dispose();
        }
       
        private async void GetPlaylist(string url)
        {
            try
            {
                var playlist = await youtube.Playlists.GetAsync(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            PlaylistProgress.Value = 0;
            PlaylistProgress.Refresh();


            // Get all playlist videos
            var videos = await youtube.Playlists.GetVideosAsync(url);
            PlaylistProgress.Maximum = videos.Count;
            PlaylistLbl.Text = "0/" + videos.Count.ToString();
            PlaylistLbl.Refresh();
            foreach (var video in videos)
            {
                PlaylistLbl.Text = $"{PlaylistProgress.Value}/{PlaylistProgress.Maximum}";
                PlaylistLbl.Refresh();
                PlaylistProgress.PerformStep();
                GetVideo(video.Url);
            }
            PlaylistLbl.Text = $"{PlaylistProgress.Value}/{PlaylistProgress.Maximum}";
            PlaylistLbl.Refresh();
        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            UrlLbl.Visible = true;
            UrlLbl.Refresh();
            if (PathTxt.Text != string.Empty && UrlTxt.Text != string.Empty)
                GetVideo(UrlTxt.Text);
            else
                MessageBox.Show("Please select a directory and provide a link");
        }

        private void PathTxt_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog diag = new FolderBrowserDialog();
            if (diag.ShowDialog() == DialogResult.OK)
            {
                PathTxt.Text = diag.SelectedPath;
                // Focus on the form so you can interract with the other elements without issue.
                Focus();
            }
        }
    }
}
