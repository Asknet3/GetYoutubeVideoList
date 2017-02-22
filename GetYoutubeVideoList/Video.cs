using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetYoutubeVideoList
{
    public class Video
    {
        public string title { get; set; }
        public string description { get; set; }
        public string kind { get; set; }
        public string videoId { get; set; }
        public string channelTitle { get; set; }
        public string dataCaricamento { get; set; }
        public string duration { get; set; }
        public string viewCount { get; set; }
        public string status { get; set; }
        public IList<string> tags { get; set; }
        public IList<string> playlist { get; set; }
    }
}