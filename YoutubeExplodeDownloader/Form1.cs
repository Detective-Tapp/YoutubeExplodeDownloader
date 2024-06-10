using TagLib;
using System.Text.RegularExpressions;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;
using YoutubeExplode.Converter;
using System.Linq;

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

        private async void GetVideo()
        {
            var url = UrlTxt.Text;

            DownloadProgress.Maximum = 100;
            DownloadProgress.Value = 0;
            DownloadProgress.Refresh();

            Video video;
            try
            {
                video = await youtube.Videos.GetAsync(url);
            }
            catch (Exception)
            {
                try
                {
                    await Task.Run(() => GetPlaylist(url));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                return;
            }

            UrlLbl.Hide();
            DownloadLbl.Show();

            // get the title and adjust the string so that it becomes a valid file name.
            var title = Regex.Replace(video.Title, @"[^a-zA-Z0-9\-]", "");
            pictureBox1.ImageLocation = video.Thumbnails.Where(t => t.Url.EndsWith(".jpg")).GetWithHighestResolution().Url;
            pictureBox1.Refresh();

            // Make a dynamic value for the progress bar.
            var progress = new Progress<double>(p =>
            {
                DownloadProgress.Value = Convert.ToInt32(p * 100);
            });
            if (!Directory.GetFiles(PathTxt.Text).Contains($"{PathTxt.Text}\\{title}.mp3"))
            {
                await youtube.Videos.DownloadAsync(url, $"{PathTxt.Text}\\{title}.mp3", progress);
                AddCover(title, video);
            }
            DownloadLbl.Hide();
        }
        private async Task GetVideo(string url)
        { 
            var video = await youtube.Videos.GetAsync(url);
            var title = Regex.Replace(video.Title, @"[^a-zA-Z0-9\-]", "");
            if (!Directory.GetFiles(PathTxt.Text).Contains($"{PathTxt.Text}\\{title}.mp3"))
            {
                await youtube.Videos.DownloadAsync(url, $"{PathTxt.Text}\\{title}.mp3");
                // This is really stupid, but it is the only way I know to covert from the thumbnail to a System.Image......
                var pb = new PictureBox();
                pb.ImageLocation = video.Thumbnails.Where(t => t.Url.EndsWith(".jpg")).GetWithHighestResolution().Url;
                AddCover(title, video, pb.Image);
                pb.Dispose();
            }
        }
        private void AddCover(string title, Video video, Image image)
        { // add a cover and title in the .mp3 files metadata using TagLib.
            var tfile = TagLib.File.Create($"{PathTxt.Text}\\{title}.mp3");
            tfile.Tag.Pictures = new IPicture[] { new Picture(new ByteVector((byte[])new ImageConverter().ConvertTo(image, typeof(byte[])))) };
            tfile.Tag.Title = video.Title;
            tfile.Tag.DateTagged = DateTime.Now;
            tfile.Save();
            tfile.Dispose();
        }

        private void AddCover(string title, Video video)
        { // add a cover and title in the .mp3 files metadata using TagLib.
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
            
            // Get all playlist videos
            var videos = await youtube.Playlists.GetVideosAsync(url);


            PlaylistProgress.Invoke(() => {
                PlaylistProgress.Maximum = videos.Count;
                PlaylistProgress.Value = 0;
                PlaylistProgress.Refresh();
            });

            PlaylistLbl.Invoke(() =>
            {
                PlaylistLbl.Text = $"{PlaylistProgress.Value}/{PlaylistProgress.Maximum}";
                PlaylistLbl.Text = "0/" + videos.Count.ToString();
                PlaylistLbl.Refresh();
            });

            UrlLbl.Invoke(() => { UrlLbl.Hide(); });

            Task[] batch = new Task[int.Parse(BatchLbl.Text)];

            while (PlaylistProgress.Value < PlaylistProgress.Maximum)
            { // Show a label giving user feedback.
                DownloadLbl.Invoke(() => { DownloadLbl.Show(); });

                if (batch.Length <= PlaylistProgress.Maximum - PlaylistProgress.Value)
                {
                    for (int i = 0; i < batch.Length; i++)
                    {
                        batch[i] = Task.Run(() => GetVideo(videos[PlaylistProgress.Value + i].Url));
                        await Task.Delay(1);
                    }
                } // if the batch size is bigger than the remaining videos, tailor the batch size to the amount of videos left.
                else
                {
                    batch = new Task[(PlaylistProgress.Maximum - PlaylistProgress.Value)];
                    
                    for (int i = 0; i < batch.Length; i++)
                    {
                        batch[i] = Task.Run(() => GetVideo(videos[PlaylistProgress.Value + i].Url));
                        await Task.Delay(1);
                    }
                }
                // Reset a simple download progress indicator and redraw it. And give it a base value so that it does not look empty :)
                DownloadProgress.Invoke(() => {
                    DownloadProgress.Maximum = batch.Length + 1;
                    DownloadProgress.Value = 0;
                    DownloadProgress.PerformStep();
                    DownloadProgress.Refresh();
                });
                
                // Step over batchdownload progress for each completed download.
                foreach (var item in batch)
                {
                    DownloadProgress.Invoke(() =>
                    {
                        DownloadProgress.PerformStep();
                        DownloadProgress.Refresh();
                    });

                    Task.WaitAny(batch);
                }

                // Should not be needed cuses of the foreach loop above, but just in case. Can't hurt anyways.
                Task.WaitAll(batch);

                // Give the user feedback of the playlists download progress.
                PlaylistProgress.Invoke(() => {
                    PlaylistProgress.Value += batch.Length;
                    PlaylistProgress.Refresh();
                    PlaylistLbl.Text = $"{PlaylistProgress.Value}/{PlaylistProgress.Maximum}";
                    PlaylistLbl.Refresh();
                });

                // Hide the element cause done downloading
                DownloadLbl.Invoke(() => { DownloadLbl.Hide(); });
            }
        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            UrlLbl.Show();
            if (PathTxt.Text != string.Empty && UrlTxt.Text != string.Empty)
                GetVideo();
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

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            BatchLbl.Text = (vScrollBar1.Value * -1).ToString();
        }
    }
}
