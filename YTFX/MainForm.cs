using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Web;
using Google.GData.YouTube;
using Google.GData.Client;
using Google.YouTube;

namespace YTFX
{
    public partial class MainForm : Form
    {

        private const string applicationName = "NotifierYT";
        private const string developerKey = "AI39si6_TfYPjjqjCOtTQviaRIHdVYQn1so3ktPqru4XlQlUauXRv1zP4uGa3LcDt0n9gEB8XmTU0e9JroACCiPZBQ00dRpibw";

        public MainForm()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {


            if (String.IsNullOrEmpty(userBox.Text) || String.IsNullOrEmpty(passBox.Text))
            {
                MessageBox.Show("Invalid username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool success = false;

            int totalVideos = 0;

            try
            {

                mainPanel.Enabled = false;

                statusLabel.Text = "Export in progress";

                Application.DoEvents();

                YouTubeRequestSettings settings = new YouTubeRequestSettings(applicationName, developerKey, userBox.Text, passBox.Text);

                settings.AutoPaging = true;

                YouTubeRequest request = new YouTubeRequest(settings);

                Feed<Video> favs = request.GetFavoriteFeed(userBox.Text);

                string path = String.Format("{0}\\youtube-export.html", Path.GetTempPath());

                StreamWriter writer = new StreamWriter(path);

                using (writer)
                {

                    writer.WriteLine("<!doctype NETSCAPE-Bookmark-file-1>");
                    writer.WriteLine("<!-- File generated automatically by YouTube Favorite Exporter. -->");
                    writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">");
                    writer.WriteLine("<title>Exported Youtube Favorites</title>");
                    writer.WriteLine("<h1>Exported Youtube Favorites</h1>");
                    writer.WriteLine("<dl>");

                    foreach (Video a in favs.Entries)
                    {
                        totalVideos++;

                        writer.WriteLine(String.Format("\t<dt><a href=\"http://www.youtube.com/watch?v={0}\" add_date=\"{1}\">{2}</a><dt>", a.VideoId, ToTimestamp(a.Updated), HttpUtility.HtmlEncode(a.Title)));

                        statusLabel.Text = String.Format("Export in progress ({0} so far)", totalVideos);

                        Application.DoEvents();

                    }

                    writer.WriteLine("</dl>");

                }

                success = true;

                if (success) Process.Start(path);

            }
            catch (GDataRequestException ex)
            {
                MessageBox.Show(ex.ResponseString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                mainPanel.Enabled = true;

                if (success)
                    statusLabel.Text = String.Format("Export complete ({0} in total)", totalVideos);
                else
                    statusLabel.Text = "Export failed";

            }


        }

        long ToTimestamp(DateTime dt)
        {
            TimeSpan ux = (dt - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)ux.TotalSeconds;
        }

        private void passBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode != Keys.Enter) return;

            startButton_Click(null, EventArgs.Empty);

        }


    }
}
