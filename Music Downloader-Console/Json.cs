using System.Collections.Generic;

namespace Music_Downloader_Console
{
    class Json
    {
        public class musiclistjson
        {
            public class Root
            {
                public Playlist playlist { get; set; }
            }
            public class Playlist
            {
                public List<TrackIdsItem> trackIds { get; set; }
            }
            public class TrackIdsItem
            {
                public long id { get; set; }
            }
        }

        public class mvjson
        {
            public class Data
            {
                /// <summary>
                /// Id
                /// </summary>
                public int id { get; set; }
                /// <summary>
                /// http://vodkgeyttp8.vod.126.net/cloudmusic/JDAyMDg1YTAhJCAkJDAhMA==/mv/5302271/f2599cfd252980d86743e25e158c9ee2.mp4?wsSecret=62af60b71b70626791836ae4a8b70899&wsTime=1568649435
                /// </summary>
                public string url { get; set; }
            }

            public class Root
            {
                /// <summary>
                /// Data
                /// </summary>
                public Data data { get; set; }
            }

        }

        public class Lrcjson
        {
            public class Lrc
            {
                /// <summary>
                /// Version
                /// </summary>
                public int version { get; set; }

                /// </summary>
                public string lyric { get; set; }
            }

            public class Klyric
            {
                /// <summary>
                /// Version
                /// </summary>
                public int version { get; set; }
                /// <summary>
                /// Lyric
                /// </summary>
                public string lyric { get; set; }
            }

            public class Tlyric
            {
                /// <summary>
                /// Version
                /// </summary>
                public int version { get; set; }
                /// <summary>
                /// Lyric
                /// </summary>
                public string lyric { get; set; }
            }

            public class Root
            {
                public Lrc lrc { get; set; }
                /// <summary>
                /// Klyric
                /// </summary>
                public Klyric klyric { get; set; }
                /// <summary>
                /// Tlyric
                /// </summary>
                public Tlyric tlyric { get; set; }
                /// <summary>
                /// Code
                /// </summary>
                public int code { get; set; }
            }


        }

        public class GetUrl
        {
            public class Data
            {
                /// <summary>
                /// Id
                /// </summary>
                public long id { get; set; }
                /// <summary>
                /// http://m8.music.126.net/20190912230430/712bef7a551e78ec25ebef60253543dc/ymusic/7f59/3fb1/2416/00bf36a7fbd8eb8e8a2ed762e39a4cfd.flac
                /// </summary>
                public string url { get; set; }
            }

            public class Root
            {
                /// <summary>
                /// Data
                /// </summary>
                public List<Data> data { get; set; }
            }

        }

        public class DownloadList
        {
            public string Songname { get; set; }
            public string Singername { get; set; }
            public string Url { get; set; }
            public string ID { get; set; }
            public string LrcUrl { set; get; }
            public string Album { get; set; }
            public string PicUrl { get; set; }
        }

        public class Setting
        {
            public string SavePath { get; set; }
            public string DownloadQuality { get; set; }
            public bool Ifdownloadpic { get; set; }
            public bool Ifdownloadlrc { get; set; }
            public int Savenamestyle { get; set; }
            public int Savepathstyle { get; set; }
            public string SearchQuantity { get; set; }
        }

        public class MusicDetails
        {
            public class Ar
            {
                /// <summary>
                /// Id
                /// </summary>
                public long id { get; set; }
                /// <summary>
                /// 薛之谦
                /// </summary>
                public string name { get; set; }
            }

            public class Al
            {
                /// <summary>
                /// Id
                /// </summary>
                public long id { get; set; }
                /// <summary>
                /// 尘
                /// </summary>
                public string name { get; set; }
                /// <summary>
                /// https://p1.music.126.net/JL_id1CFwNJpzgrXwemh4Q==/109951164172892390.jpg
                /// </summary>
                public string picUrl { get; set; }
            }

            public class Songs
            {
                /// <summary>
                /// 笑场
                /// </summary>
                public string name { get; set; }
                /// <summary>
                /// Id
                /// </summary>
                public long id { get; set; }
                /// <summary>
                /// Ar
                /// </summary>
                public List<Ar> ar { get; set; }
                /// <summary>
                /// Al
                /// </summary>
                public Al al { get; set; }
                /// <summary>
                /// Mv
                /// </summary>
                public long mv { get; set; }
            }

            public class Root
            {
                /// <summary>
                /// Songs
                /// </summary>
                public List<Songs> songs { get; set; }
            }

        }

        public class MusicInfo
        {
            public string Song { get; set; }
            public string Singer { get; set; }
            public List<string> SingerId { get; set; }
            public string Album { get; set; }
            public long AlbumId { get; set; }
            public long Id { get; set; }
            public string Url { get; set; }
            public string LrcUrl { get; set; }
            public string PicUrl { get; set; }
            public string MvUrl { get; set; }
        }

        public class SearchResultJson
        {

            public class Songs
            {
                /// <summary>
                /// Id
                /// </summary>
                public long id { get; set; }
            }

            public class Result
            {
                /// <summary>
                /// Songs
                /// </summary>
                public List<Songs> songs { get; set; }
            }

            public class Root
            {
                /// <summary>
                /// Result
                /// </summary>
                public Result result { get; set; }
            }
        }
    }
}
