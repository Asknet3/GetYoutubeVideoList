using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Data;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using System.Collections;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using GetYoutubeVideoList;

namespace GetYoutubeVideoList
{
    public partial class GetList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Task.Run(() => Status.Text= "In esecuzione...");

            //try
            //{
            //    string channelId = "UCZ0KUEiciMan-aG7e1mr4uA";
            //    string apiKey = "AIzaSyA0_amtuRm5Ud4in_QRz6iWsnhllSsTYDk";
            //    List<Video> listVideo = GetVideoList.GetListFromJson(channelId, apiKey);

            //    foreach (Video v in listVideo)
            //    {
            //        AddVideo("", v.title, v.status, v.playlist, v.description, v.tags, v.dataCaricamento, v.duration, v.viewCount, "http://www.youtube.com/watch?v=" + v.videoId, v.channelTitle);
            //    }

            //    Status.Text = "Operazione completata!";
            //}
            //catch(Exception ex)
            //{
            //    Status.Text = "Operazione non riuscita!\n Si è verificato il seguente errore:\n\n" + ex;
            //}

            GetVideosbyPlaylistId("PLfw_isEyHQQHorssZ_dVUiLt-hsiEfNOZ");


            #region GESTIONE VIDEO NON IN PLAYLIST
            string nextPageToken = "start";

            while (!String.IsNullOrEmpty(nextPageToken) || (nextPageToken != null && nextPageToken.Equals("start"))) // ciclo fino a scorrere tutte le pagine
            {

                //YouTubeService yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyA0_amtuRm5Ud4in_QRz6iWsnhllSsTYDk" });
                //var yt = new YouTubeService(new BaseClientService.Initializer() { HttpClientInitializer = (Google.Apis.Http.IConfigurableHttpClientInitializer)credential, ApplicationName = this.GetType().ToString() });

                string[] scopes = new string[] { YouTubeService.Scope.Youtube,  // view and manage your YouTube account
                                             YouTubeService.Scope.YoutubeForceSsl,
                                             YouTubeService.Scope.Youtubepartner,
                                             YouTubeService.Scope.YoutubepartnerChannelAudit,
                                             YouTubeService.Scope.YoutubeReadonly,
                                             YouTubeService.Scope.YoutubeUpload};
                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
                {
                    //ClientId = "222822455460-b6n8rn6ofnjbr1n8quro9n4pv6piit2p.apps.googleusercontent.com",
                    //ClientSecret = "AIzaSyB7X6x0NIYfpf0aXx846CB4BW-aPzwk8tk"

                    ClientId = "668981891376-uq2emjvtpjee272glsdh5h1liuarrr9u.apps.googleusercontent.com",
                    ClientSecret = "1SEzci9hXOdAwhEGVssAWKnl"
                }
                    , scopes
                    , "LavazzaTube"
                    , CancellationToken.None
                    , new FileDataStore("C:\\Users\\Giuseppe\\Desktop\\Videolist")).Result;

                //var yt = new YouTubeService(new BaseClientService.Initializer() { HttpClientInitializer = (Google.Apis.Http.IConfigurableHttpClientInitializer)credential, ApplicationName = this.GetType().ToString() });
                YouTubeService yt = new YouTubeService(new YouTubeService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Alfaromeo",
                    //ApplicationName = "LavazzaTube",
                });

                VideosResource.ListRequest videosListRequest = yt.Videos.List("snippet,statistics,status,contentDetails");
                //VideosResource.ListRequest videosListRequest = yt.Videos.List("snippet");

                var searchListRequest = yt.Search.List("snippet");
                //searchListRequest.Type = "video";
                if (!nextPageToken.Equals("start"))
                    searchListRequest.PageToken = nextPageToken;
                //searchListRequest.Key = "AIzaSyA0_amtuRm5Ud4in_QRz6iWsnhllSsTYDk";
                searchListRequest.MaxResults = 50;

                //searchListRequest.ChannelId = "UCmsyyHg8mmDsnsQVhMlacdQ"; // Channel ID Lavazza
                searchListRequest.ChannelId = "UCZ0KUEiciMan-aG7e1mr4uA";   // Channel ID Alfa Romeo


                searchListRequest.SafeSearch = Google.Apis.YouTube.v3.SearchResource.ListRequest.SafeSearchEnum.None;
                //searchListRequest.OauthToken = "4/8d4XeKfJQqzuB0joJnlDgo1T8F27SIR6bzyJC5pQ8rk";
                //searchListRequest.ForMine = true;


                var searchListResult = searchListRequest.Execute();


                nextPageToken = searchListResult.NextPageToken;

                foreach (var item in searchListResult.Items)
                {
                    string npt;
                    string title;
                    string status;
                    //string playlist;
                    string descrizione;
                    IList<string> tag;
                    string dataCaricamento;
                    string durata;
                    string visualizzazioni;
                    string url;
                    string channel;


                    if (item.Id.Kind.Equals("youtube#searchListResponse"))
                    {
                        item.Kind.Count();
                    }
                    //********* E' un video *********
                    int count = 0;
                    if (item.Id.Kind.Equals("youtube#video"))
                    {
                        videosListRequest.Id = item.Id.VideoId; // prende l'id del video che verrà usato per filtrare.
                        videosListRequest.MaxResults = 50;
                        var videoListResult = videosListRequest.Execute();
                        Google.Apis.YouTube.v3.Data.Video video = videoListResult.Items[0];
                        npt = nextPageToken;
                        title = video.Snippet.Title.Replace("'", "''");
                        status = !String.IsNullOrEmpty(video.Status.PrivacyStatus) ? video.Status.PrivacyStatus : String.Empty;
                        //playlist= 
                        descrizione = video.Snippet.Description.Replace("'", "''");
                        tag = video.Snippet.Tags;
                        dataCaricamento = video.Snippet.PublishedAt.ToString();
                        durata = System.Xml.XmlConvert.ToTimeSpan(video.ContentDetails.Duration).ToString();
                        visualizzazioni = video.Statistics.ViewCount.Value.ToString();
                        url = "http://www.youtube.com/watch?v=" + item.Id.VideoId;
                        channel = video.Snippet.ChannelTitle;

                        AddVideo("[" + item.Id.Kind + "] " + nextPageToken, title, status, null, descrizione, tag, dataCaricamento, durata, visualizzazioni, url, channel);
                    }
                    else
                    { count++; }

                }
            }
            #endregion



            #region GESTIONE PLAYLIST
            nextPageToken = "start";
            IList<String> playListsID = new List<String>();
            //YouTubeService yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyA0_amtuRm5Ud4in_QRz6iWsnhllSsTYDk" });

            while (!String.IsNullOrEmpty(nextPageToken) || (nextPageToken != null && nextPageToken.Equals("start"))) // ciclo fino a scorrere tutte le pagine
            {
                string[] scopes = new string[] { YouTubeService.Scope.Youtube,  // view and manage your YouTube account
                                             YouTubeService.Scope.YoutubeForceSsl,
                                             YouTubeService.Scope.Youtubepartner,
                                             /*YouTubeService.Scope.YoutubepartnerChannelAudit,*/
                                             YouTubeService.Scope.YoutubeReadonly,
                                             /*YouTubeService.Scope.YoutubeUpload*/ };
                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
                {
                    //ClientId = "292824132082.apps.googleusercontent.com",
                    ClientId = "668981891376-uq2emjvtpjee272glsdh5h1liuarrr9u.apps.googleusercontent.com",
                    ClientSecret = "1SEzci9hXOdAwhEGVssAWKnl"
                }
                    , scopes
                    , "Alfaromeo"
                    , CancellationToken.None
                    , new FileDataStore("C:\\Users\\Giuseppe\\Desktop\\Videolist")).Result;

                //var yt = new YouTubeService(new BaseClientService.Initializer() { HttpClientInitializer = (Google.Apis.Http.IConfigurableHttpClientInitializer)credential, ApplicationName = this.GetType().ToString() });
                YouTubeService yt = new YouTubeService(new YouTubeService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Alfaromeo",
                });

                PlaylistItemsResource.ListRequest PlayListRequest = yt.PlaylistItems.List("contentDetails,snippet,status");

                VideosResource.ListRequest videosListRequest = yt.Videos.List("snippet,statistics,status,contentDetails");

                var searchListRequest = yt.Search.List("snippet");
                if (!nextPageToken.Equals("start"))
                    searchListRequest.PageToken = nextPageToken;
                //searchListRequest.Key = "AIzaSyA0_amtuRm5Ud4in_QRz6iWsnhllSsTYDk";
                searchListRequest.MaxResults = 50;
                //searchListRequest.ChannelId = "UCmsyyHg8mmDsnsQVhMlacdQ"; // LAVAZZA
                searchListRequest.ChannelId = "UCZ0KUEiciMan-aG7e1mr4uA";  // Alfaromeo

                searchListRequest.OauthToken = "ya29.Glv6AxgrnuzJkn-PursX1ccm6BrhUs6sDWylFv4Rni-C6JMbL1OnaKw2gxDUMr7akPmKB8XG2BgbficrB6soMOnu8cb_9HuYUKFQpeilIvxfESfjHFN43y3kRZ6B";
                searchListRequest.Type = "playlist";

                var searchListResult = searchListRequest.Execute();

                nextPageToken = searchListResult.NextPageToken;

                string np = "";
                foreach (var item in searchListResult.Items) //per ogni playlist...
                {
                    playListsID.Add(item.Id.PlaylistId);

                    if (item.Id.PlaylistId.Equals("PLfw_isEyHQQHorssZ_dVUiLt-hsiEfNOZ"))
                    {
                    };

                    PlayListRequest.PlaylistId = item.Id.PlaylistId; // id della playlist
                    //PlayListRequest.PlaylistId = "PLfw_isEyHQQHorssZ_dVUiLt-hsiEfNOZ"; // id della playlist
                    PlayListRequest.MaxResults = 50;
                    PlayListRequest.PageToken = np;

                    var PlayListResult = PlayListRequest.Execute();

                    foreach (var playlistItem in PlayListResult.Items) // per ogni video della playlist...
                    {
                        videosListRequest.Id = playlistItem.Snippet.ResourceId.VideoId; // prende l'id del video che verrà usato per filtrare.
                        videosListRequest.MaxResults = 50;
                        var videoListResult = videosListRequest.Execute();

                        try
                        {
                            Google.Apis.YouTube.v3.Data.Video video = videoListResult.Items[0];

                            String npt = np;
                            String channel = video.Snippet.ChannelTitle;
                            String title = video.Snippet.Title.Replace("'", "''");
                            String status = !String.IsNullOrEmpty(video.Status.PrivacyStatus) ? video.Status.PrivacyStatus : String.Empty;
                            String playlist = playlistItem.Snippet.PlaylistId;
                            List<string> playlist_title = new List<string>(new string[] { item.Snippet.Title }); // titolo della playlist
                            String descrizione = video.Snippet.Description.Replace("'", "''");
                            IList<string> tag = video.Snippet.Tags;
                            String dataCaricamento = video.Snippet.PublishedAt.ToString();
                            String durata = System.Xml.XmlConvert.ToTimeSpan(video.ContentDetails.Duration).ToString();
                            String visualizzazioni = video.Statistics.ViewCount.Value.ToString();
                            String url = "http://www.youtube.com/watch?v=" + videosListRequest.Id;

                            AddVideo("[VIDEO] " + npt, title, status, playlist_title, descrizione, tag, dataCaricamento, durata, visualizzazioni, url, channel);
                        }
                        catch(Exception ex)
                        { }
                    }
                }
            }
            #endregion

            // export();
        }


        protected void AddVideo(String nextPageToken, String title, String status, IList<String> playlist, String descrizione, IList<string> tag, String dataCaricamento, String durata, String visualizzazioni, String url, String channel)
        {
            string concatTags = "";
            if (tag != null)
                concatTags = string.Join(", ", tag).Replace("'", "''");

            string concatPlaylist = "";
            if (playlist != null)
                concatPlaylist = string.Join(", ", playlist).Replace("'", "''");

            string connString = "Data Source = DESKTOP-VSA5IJB\\SQLEXPRESS; Initial Catalog = VideoListLavazza; Integrated Security = False; User ID = dbUserVespa; Password = V3sp4.; MultipleActiveResultSets = True";
            SqlConnection connection = GCU.Db.GetConnection(connString);

            //Db.Insert(connection, String.Format("videoList VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}', '{8}')", title, status, "''", descrizione, concatTags, dataCaricamento, durata, "''", url));
            //Db.Insert(connection,"videoList(title) VALUES('" + title +"')");


            DataTable dt= GCU.Db.getDataTableFrom_Select(connection, "url", "videoList", "url='" + url + "'"); 
            if (dt.Rows.Count == 0)// Aggiunge il video solo se non già presente
            {
                SqlCommand command;
                string sqlQuery = "INSERT INTO " + String.Format("videoList VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}', '{8}', '{9}', '{10}')", nextPageToken, title.Replace("'", "''"), status, concatPlaylist.Replace("'", "''"), descrizione.Replace("'", "''"), concatTags.Replace("'", "''"), dataCaricamento, durata, visualizzazioni, url, channel);

                try
                {

                    command = new SqlCommand(sqlQuery, connection);
                    connection.Open();
                    int numberOfRows = command.ExecuteNonQuery();
                }
                catch (Exception ex) { }
                finally { connection.Close(); }
            }
        }




        public void GetVideosbyPlaylistId(string playlist_id)
        {
            string[] scopes = new string[] { YouTubeService.Scope.Youtube,  // view and manage your YouTube account
                                             YouTubeService.Scope.YoutubeForceSsl,
                                             YouTubeService.Scope.Youtubepartner,
                                             YouTubeService.Scope.YoutubepartnerChannelAudit,
                                             YouTubeService.Scope.YoutubeReadonly,
                                             YouTubeService.Scope.YoutubeUpload };

            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                //ClientId = "292824132082.apps.googleusercontent.com",
                ClientId = "668981891376-uq2emjvtpjee272glsdh5h1liuarrr9u.apps.googleusercontent.com",
                ClientSecret = "1SEzci9hXOdAwhEGVssAWKnl"
            }
                   , scopes
                   , "Alfaromeo"
                   , CancellationToken.None
                   , new FileDataStore("C:\\Users\\Giuseppe\\Desktop\\Videolist")).Result;

            YouTubeService yt = new YouTubeService(new YouTubeService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Alfaromeo",
            });


            VideosResource.ListRequest videosListRequest = yt.Videos.List("snippet,statistics,status,contentDetails");
            PlaylistItemsResource.ListRequest PlayListRequest = yt.PlaylistItems.List("contentDetails,snippet,status");

            string nextPageToken = "start";

            while (!String.IsNullOrEmpty(nextPageToken) || (nextPageToken != null && nextPageToken.Equals("start"))) // ciclo fino a scorrere tutte le pagine
            {

                PlayListRequest.PlaylistId = playlist_id;        // id della playlist
                                                                 //PlayListRequest.PlaylistId = "PLfw_isEyHQQHorssZ_dVUiLt-hsiEfNOZ"; // id della playlist
                if (!nextPageToken.Equals("start"))
                    PlayListRequest.PageToken = nextPageToken;

                PlayListRequest.MaxResults = 50;

                var PlayListResult = PlayListRequest.Execute();
                nextPageToken = PlayListResult.NextPageToken;

                string np = "";
                foreach (var playlistItem in PlayListResult.Items) // per ogni video della playlist...
                {
                    videosListRequest.Id = playlistItem.Snippet.ResourceId.VideoId; // prende l'id del video che verrà usato per filtrare.
                    videosListRequest.MaxResults = 50;
                    var videoListResult = videosListRequest.Execute();

                    try
                    {
                        Google.Apis.YouTube.v3.Data.Video video = videoListResult.Items[0];

                        String npt = np;
                        String channel = video.Snippet.ChannelTitle;
                        String title = video.Snippet.Title.Replace("'", "''");
                        String status = !String.IsNullOrEmpty(video.Status.PrivacyStatus) ? video.Status.PrivacyStatus : String.Empty;
                        String playlist = playlistItem.Snippet.PlaylistId;
                        List<string> playlist_title = new List<string>(new string[] { playlistItem.Snippet.Title }); // titolo della playlist
                        String descrizione = video.Snippet.Description.Replace("'", "''");
                        IList<string> tag = video.Snippet.Tags;
                        String dataCaricamento = video.Snippet.PublishedAt.ToString();
                        String durata = System.Xml.XmlConvert.ToTimeSpan(video.ContentDetails.Duration).ToString();
                        String visualizzazioni = video.Statistics.ViewCount.Value.ToString();
                        String url = "http://www.youtube.com/watch?v=" + videosListRequest.Id;

                        AddVideo("[VIDEO] " + npt, title, status, playlist_title, descrizione, tag, dataCaricamento, durata, visualizzazioni, url, channel);
                    }
                    catch (Exception ex)
                    { }
                }
            }
        }



        protected void export()
        {
            //string connString = "Data Source = DESKTOP-VSA5IJB\\SQLEXPRESS; Initial Catalog = VideoListLavazza; Integrated Security = False; User ID = dbUserVespa; Password = V3sp4.; MultipleActiveResultSets = True";
            //SqlConnection connection = new SqlConnection(connString);
            //SqlConnection connection = GCU.Db.GetConnection(connString);
            //DataTable dt = GCU.Db.getDataTableFrom_Select(connection, " * ", "videoList");
            //GCU.Export.ExportToExcel(dt, "C:\\Users\\Giuseppe\\Desktop\\VideolistvideoList.xlsx", "videoList", "VideoList", "Giuseppe Cristaudo", "DLBi");


            SqlConnection connection = GetConnection();
            SqlCommand command;

            string sqlQuery = "SELECT * FROM [videoList] ";

            try
            {
                command = new SqlCommand(sqlQuery, connection);
                
                DataTable dt = new DataTable();
                connection.Open();
                SqlDataReader myreader = command.ExecuteReader();
                dt.Load(myreader);
            }
            finally
            { connection.Close(); 
            }

        }

        private static SqlConnection GetConnection()
        {
            string connString = "Data Source = DESKTOP-VSA5IJB\\SQLEXPRESS; Initial Catalog = VideoListLavazza; Integrated Security = False; User ID = dbUserVespa; Password = V3sp4.; MultipleActiveResultSets = True";
            return new SqlConnection(connString);
        }
    }
}