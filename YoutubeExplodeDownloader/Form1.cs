using TagLib;
using System.Text.RegularExpressions;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;
using YoutubeExplode.Converter;
using AngleSharp.Common;

namespace YoutubeExplodeDownloader
{
    public partial class Form1 : Form
    {
        private static HttpClient _httpClient = new();
        YoutubeClient youtube;
        //Cookie cookies = new Cookie();

        List<string> log = new List<string>();

        public Form1()
        {
            InitializeComponent();

            // Saves the files as ID3V2.3 instead of ID3V2.4 cause 2.4 does not show the cover image in mediaplayer.
            TagLib.Id3v2.Tag.DefaultVersion = 3;
            TagLib.Id3v2.Tag.ForceDefaultVersion = true;

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
            if (FileTypeSelect.Invoke(() => { return FileTypeSelect.SelectedIndex; }) == 0) {
                if (!Directory.GetFiles(PathTxt.Text).Contains($"{PathTxt.Text}\\{title}.mp3"))
                {
                    await youtube.Videos.DownloadAsync(url, $"{PathTxt.Text}\\{title}.mp3", progress);
                    AddCover(title, video);
                }
            }
            else
            {
                if (!Directory.GetFiles(PathTxt.Text).Contains($"{PathTxt.Text}\\{title}.mp4"))
                {
                    await youtube.Videos.DownloadAsync(url, $"{PathTxt.Text}\\{title}.mp4", progress);
                    AddCover(title, video);
                }
            }
            DownloadLbl.Hide();
        }
        private async Task GetVideo(string url)
        {
            var video = await youtube.Videos.GetAsync(url);
            var title = Regex.Replace(video.Title, @"[^a-zA-Z0-9\-]", "");
            /*if (!Directory.GetFiles(PathTxt.Text).Contains($"{PathTxt.Text}\\{title}.mp3"))
            {
                await youtube.Videos.DownloadAsync(url, $"{PathTxt.Text}\\{title}.mp3");
                // Bit hard to read but it, gets the highest resolution jpeg from youtube, gets the stream,
                // then converts the stream into a System.Drawing.Image type, and feeds that into Addcover.
                AddCover(title, video, Image.FromStream(await _httpClient.GetStreamAsync(video.Thumbnails.Where(t => t.Url.EndsWith(".jpg")).GetWithHighestResolution().Url)));
            }*/
            if (FileTypeSelect.Invoke(() => { return FileTypeSelect.SelectedIndex; }) == 0)
            {
                if (!Directory.GetFiles(PathTxt.Text).Contains($"{PathTxt.Text}\\{title}.mp3"))
                {
                    await youtube.Videos.DownloadAsync(url, $"{PathTxt.Text}\\{title}.mp3");
                    // Bit hard to read but it, gets the highest resolution jpeg from youtube, gets the stream,
                    // then converts the stream into a System.Drawing.Image type, and feeds that into Addcover.
                    AddCover(title, video, Image.FromStream(await _httpClient.GetStreamAsync(video.Thumbnails.Where(t => t.Url.EndsWith(".jpg")).GetWithHighestResolution().Url)));
                }
            }
            else
            {
                if (!Directory.GetFiles(PathTxt.Text).Contains($"{PathTxt.Text}\\{title}.mp4"))
                {
                    await youtube.Videos.DownloadAsync(url, $"{PathTxt.Text}\\{title}.mp4");
                    // Bit hard to read but it, gets the highest resolution jpeg from youtube, gets the stream,
                    // then converts the stream into a System.Drawing.Image type, and feeds that into Addcover.
                    AddCover(title, video, Image.FromStream(await _httpClient.GetStreamAsync(video.Thumbnails.Where(t => t.Url.EndsWith(".jpg")).GetWithHighestResolution().Url)));
                }
            }
        }

        private void AddCover(string title, Video video, Image image)
        { // add a cover and title in the .mp3 files metadata using TagLib.
            TagLib.File tfile;
            if (FileTypeSelect.Invoke(() => { return FileTypeSelect.SelectedIndex; }) == 0)
            {
                 tfile = TagLib.File.Create($"{PathTxt.Text}\\{title}.mp3");
            }
            else
            {
                 tfile = TagLib.File.Create($"{PathTxt.Text}\\{title}.mp4");
            }

            tfile.Tag.Pictures = new IPicture[] { new Picture(new ByteVector((byte[])new ImageConverter().ConvertTo(image, typeof(byte[])))) };
            tfile.Tag.Title = video.Title;
            tfile.Tag.DateTagged = DateTime.Now;
            tfile.Save();
            tfile.Dispose();
        }

        private void AddCover(string title, Video video)
        { // add a cover and title in the .mp3 files metadata using TagLib.
            TagLib.File tfile;
            
            if (FileTypeSelect.Invoke(() => { return FileTypeSelect.SelectedIndex; }) == 0)
            {
                tfile = TagLib.File.Create($"{PathTxt.Text}\\{title}.mp3");
            }
            else
            {
                tfile = TagLib.File.Create($"{PathTxt.Text}\\{title}.mp4");
            }
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

            // Show a label giving user feedback.
            DownloadLbl.Invoke(() => { DownloadLbl.Show(); });

            while (PlaylistProgress.Value < PlaylistProgress.Maximum)
            { 

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
                DownloadProgress.Invoke(() =>
                {
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
            
                try { Task.WaitAll(batch); }
                catch (Exception ex)
                {
                    int count = 0;
                    foreach (var item in batch)
                    {
                        if(item.IsFaulted)
                        {
                            log.Add($"Song: {videos[count + PlaylistProgress.Value].Title}\nUrl: " +
                            $"{videos[count + PlaylistProgress.Value].Url}\n" +
                            $"{ex.Message}\n");
                        }
                        count++;
                    }
                }

                // Give the user feedback of the playlists download progress.
                PlaylistProgress.Invoke(() =>
                {
                    PlaylistProgress.Value += batch.Length;
                    PlaylistProgress.Refresh();
                    PlaylistLbl.Text = $"{PlaylistProgress.Value}/{PlaylistProgress.Maximum}";
                    PlaylistLbl.Refresh();
                });
            }
            // Hide the element cause done downloading
            DownloadLbl.Invoke(() => { DownloadLbl.Hide(); });

            if (log.Any())
            {
                MessageBox.Show($"{log.Count} songs failed to download, see all failed files \n@{PathTxt.Text}\\FailedDownloads.txt");
                System.IO.File.WriteAllLines($"{PathTxt.Text}\\FailedDownloads.txt", log.ToArray());
            }
            else
            {
                MessageBox.Show("Finished downloading playlist!");
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
