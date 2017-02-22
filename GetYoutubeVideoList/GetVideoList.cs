using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using Google.Apis.YouTube.v3.Data;

namespace GetYoutubeVideoList
{
    public class GetVideoList
    {
        //static string channelId = "UCZ0KUEiciMan-aG7e1mr4uA";
        //static string apiKey = "AIzaSyA0_amtuRm5Ud4in_QRz6iWsnhllSsTYDk";
        static string type = "video";
        static string order = "date";
        static string maxResults = "50";

        static List<Video> listaVideo = new List<Video>();

        /// <summary>
        /// Restituisce la lista dei Video contenuti in un canale YouTube
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="apiKey"></param>
        /// <returns>Lista di Video</returns>
        public static List<Video> GetListFromJson(string channelId, string apiKey)
        {
            string pageToken = "first";
            bool existsNext = true;


            while ( (!String.IsNullOrEmpty(pageToken) || (pageToken != null && pageToken.Equals("first"))) && existsNext ) // ciclo fino a scorrere tutte le pagine
            {
                string jsonUrl = @"https://www.googleapis.com/youtube/v3/search?order=" + order + "&part=snippet&channelId=" + channelId + "&maxResults=" + maxResults + "&key=" + apiKey + "&type=" + type + "&pageToken=" + (pageToken.Equals("first") || pageToken.Equals("last") ? "" : pageToken) ;

            
                using (WebClient wc = new WebClient())
                {
                    var json = wc.DownloadString(jsonUrl);


                    JObject jres = JObject.Parse(json);

                    if (jres["nextPageToken"] == null)  
                    {
                        existsNext = false; // Ultima pagina 
                    }
                    else
                    {
                        pageToken = jres["nextPageToken"].ToString(); // Aggiorno il pageToken
                    }


                   

                    IList<JToken> results = jres["items"].Children().ToList();

                    // serialize JSON results into.NET objects
                    IList<SearchResult> searchResults = new List<SearchResult>();
                    foreach (JToken result in results)
                    {
                        SearchResult searchResult = JsonConvert.DeserializeObject<SearchResult>(result.ToString());
                        searchResults.Add(searchResult);
                    }


                    // Estraggo tutte le playlist del canale (max 50 per come attualmente implementato)
                    string playlistUrl = "https://www.googleapis.com/youtube/v3/playlists?part=snippet,contentDetails&maxResults=50&key=" + apiKey + "&channelId=" + channelId;
                    var jsonPlaylist = wc.DownloadString(playlistUrl);
                    JObject jsonObjPlaylist = JObject.Parse(jsonPlaylist);
                    IList<JToken> resultsPlaylist = jsonObjPlaylist["items"].Children().ToList();

                    // Per ogni playlist estratta, estraggo tutti i video ad essa associati 
                    List<Tuple<string, string>> playlist_video = new List<Tuple<string, string>>();  // Tupls che conterrà il TITOLO DELLA PLAYLIST e l'ID DEL VIDEO
                    foreach(JToken playlist in resultsPlaylist)
                    {
                        string pl_id = playlist["id"].ToString(); // id della playlist

                        // Estraggo tutti i video associati alla playlist (max 50 per come attualmente implementato)
                        string playlistVideoUrl = "https://www.googleapis.com/youtube/v3/playlistItems?part=contentDetails,snippet&maxResults=50&playlistId=" + pl_id + "&key=" + apiKey;
                        var jsonPlaylistItem = wc.DownloadString(playlistVideoUrl);
                        JObject jsonObjPlaylistItem = JObject.Parse(jsonPlaylistItem);
                        IList<JToken> resultsPlaylistItem = jsonObjPlaylistItem["items"].Children().ToList();
                        foreach (JToken jt in resultsPlaylistItem)
                        {
                            Tuple<string, string> tupla = new Tuple<string, string>(playlist["snippet"]["title"].ToString(), jt["contentDetails"]["videoId"].ToString());
                            playlist_video.Add(tupla); // aggiungo la tupla alla lista
                        }
                    }




                    // Estraggo le informazioni aggiuntive per ogni singolo video
                    foreach (var item in searchResults)
                    {
                        string videoUrl = "https://www.googleapis.com/youtube/v3/videos?part=contentDetails,statistics,status,snippet&key=" + apiKey + "&id=" + (item.Id).VideoId;
                        var jsonVideo = wc.DownloadString(videoUrl);
                        JObject jsonObjVideo = JObject.Parse(jsonVideo);
                        IList<JToken> resultsVideo = jsonObjVideo["items"].Children().ToList();

                        Video video = new Video();

                        video.kind = item.Kind;
                        video.videoId = (item.Id).VideoId;
                        video.title = item.Snippet.Title;
                        video.description = item.Snippet.Description;
                        video.channelTitle = item.Snippet.ChannelTitle;
                        video.dataCaricamento = item.Snippet.PublishedAt.ToString();
                        video.duration = System.Xml.XmlConvert.ToTimeSpan(resultsVideo[0]["contentDetails"]["duration"].ToString()).ToString();
                        video.viewCount = resultsVideo[0]["statistics"]["viewCount"].ToString();
                        video.status = resultsVideo[0]["status"]["privacyStatus"].ToString();
                        video.playlist = new List<string>();

                        // Aggiungo le playlist aasociate al video
                        List<string> myplaylists = new List<string>();
                        foreach (Tuple<string, string> pv in playlist_video)
                        {
                            if (((item.Id).VideoId).Equals(pv.Item2))
                            {
                                myplaylists.Add(pv.Item1);
                                video.playlist.Add(pv.Item1);
                            }
                        }
                        
                        
                        // Aggiungo i tag associati al video
                        List<JToken> t = resultsVideo[0]["snippet"]["tags"] != null ? resultsVideo[0]["snippet"]["tags"].ToList() : null;
                        List<string> mytags = new List<string>();
                        if (t != null)
                        {
                            foreach (JToken jt in t)
                            {
                                mytags.Add(jt != null ? jt.ToString() : "");
                            }
                        }
                        video.tags= mytags;


                        listaVideo.Add(video); // Aggiungo il video alla lista
                    }
                }
            }

            return listaVideo;
        }
    }
}