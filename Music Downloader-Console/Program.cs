using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Music_Downloader_Console
{
    class Program
    {
        static List<Json.MusicInfo> SearchResult = new List<Json.MusicInfo>();
        static Json.Setting Setting = new Json.Setting();
        static string ApiUrl = "";
        

        static void Main(string[] args)
        {
            Console.Title = "Music Downloader - Console";
            if (File.Exists("Setting.json"))
            {
                StreamReader sr = new StreamReader("Setting.json");
                Setting = JsonConvert.DeserializeObject<Json.Setting>(sr.ReadToEnd());
                sr.Close();
            }
            else
            {
                Setting = new Json.Setting()
                {
                    DownloadQuality = "999000",
                    Ifdownloadlrc = false,
                    Ifdownloadpic = false,
                    Savenamestyle = 0,
                    SavePath = Environment.CurrentDirectory,
                    Savepathstyle = 0,
                    SearchQuantity = "100"
                };
                string json_ = JsonConvert.SerializeObject(Setting);
                StreamWriter sw_ = new StreamWriter("Setting.json");
                sw_.Write(json_);
                sw_.Flush();
                sw_.Close();
            }

            Console.WriteLine("欢迎使用 Music Downloader - Console");
            Console.WriteLine("本程序使用网易云音乐接口");
            MainPage();
        }
        static void MainPage()
        {
            Console.WriteLine("请选择功能（输入数字）:");
            Console.WriteLine("1.搜索");
            Console.WriteLine("2.导入歌单");
            Console.WriteLine("3.热歌榜");
            Console.WriteLine("4.新歌榜");
            Console.WriteLine("5.设置");
            Console.WriteLine("6.关于 ");
            Console.WriteLine("7.打赏 ");
            string key = Console.ReadLine();
            switch (key)
            {
                case "1":
                    SearchPage();
                    break;

                case "2":
                    Console.Clear();
                    Console.WriteLine("请输入音乐歌单链接或ID");
                    string ml = Console.ReadLine();
                    GetMusicList(ml);
                    break;
                case "3":
                    Console.Clear();
                    GetMusicList("3778678");
                    break;
                case "4":
                    Console.Clear();
                    GetMusicList("3779629");
                    break;
                case "5":
                    Console.Clear();
                    SettingPage();
                    string json_ = JsonConvert.SerializeObject(Setting);
                    StreamWriter sw_ = new StreamWriter("Setting.json");
                    sw_.Write(json_);
                    sw_.Flush();
                    sw_.Close();
                    MainPage();
                    break;
                case "6":
                    Console.WriteLine();
                    Console.WriteLine("本程序使用网易云音乐接口");
                    Console.WriteLine("程序作者：NiTian1207");
                    Console.WriteLine("作者QQ：1024028162");
                    Console.WriteLine("作者博客：www.nitian1207.top");
                    Console.WriteLine("Q群：348846978");
                    Console.WriteLine();
                    MainPage();
                    break;
                case "7":
                    Console.WriteLine();
                    Console.WriteLine("请选择支付方式：");
                    Console.WriteLine("1. 微信支付");
                    Console.WriteLine("1. 支付宝支付");
                    string p = Console.ReadLine();
                    switch (p)
                    {
                        case "1":
                            Process.Start("explorer.exe", "https://www.nitian1207.top/img/wechatpay.png");
                            break;
                        case "2":
                            Process.Start("explorer.exe", "https://www.nitian1207.top/img/alipay.png");
                            break;
                    }
                    MainPage();
                    break;
                default:
                    Console.WriteLine("命令不存在");
                    Console.WriteLine();
                    MainPage();
                    break;

            }
        }

        /// <summary>
        /// 带cookie访问
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static string GetHTML(string url)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.Cookie, cookie);
            Stream s = wc.OpenRead(url);
            StreamReader sr = new StreamReader(s);
            return sr.ReadToEnd();
        }

        static void GetMusicList(string parameter)
        {
            SearchResult.Clear();
            Console.WriteLine("正在获取歌单信息");
            string id = "";
            if (parameter.IndexOf("http") != -1)
            {
                if (parameter.IndexOf("&userid") != -1)
                {
                    id = parameter.Substring(parameter.IndexOf("id=") + 3, parameter.IndexOf("&userid=") - (parameter.IndexOf("id=") + 3));
                }
                else
                {
                    id = parameter.Substring(parameter.IndexOf("id=") + 3);
                }
            }
            else
            {
                id = parameter;
            }

            WebClient wc = new WebClient();
            Json.musiclistjson.Root jmr = new Json.musiclistjson.Root();
            try
            {
                jmr = JsonConvert.DeserializeObject<Json.musiclistjson.Root>(GetHTML(ApiUrl + "playlist/detail?id=" + id));
            }
            catch
            {
                Console.WriteLine("歌单获取失败");
                Console.WriteLine();
                MainPage();
            }

            string ids = "";

            for (int i = 0; i < jmr.playlist.trackIds.Count; i++)
            {
                ids += jmr.playlist.trackIds[i].id.ToString() + ",";
            }
            ids = ids.Substring(0, ids.Length - 1);

            if (jmr.playlist.trackIds.Count > 200)
            {
                string[] _id = ids.Split(',');
                
                int times = jmr.playlist.trackIds.Count / 200;
                int remainder = jmr.playlist.trackIds.Count % 200;
                if (remainder != 0)
                {
                    times++;
                }

                for (int i = 0; i < times; i++)
                {
                    string _ids = "";
                    if (i != times - 1)
                    {
                        for (int x = 0; x < 200; x++)
                        {
                            _ids += _id[i * 200 + x] + ",";
                        }
                    }
                    else
                    {
                        for (int x = 0; x < remainder; x++)
                        {
                            _ids += _id[i * 200 + x] + ",";
                        }
                    }
                    _GetMusicList(_ids.Substring(0, _ids.Length - 1));
                }

            }
            else
            {
                _GetMusicList(ids);
            }

            int flacquantity = 0;
            for (int i = 0; i < SearchResult.Count; i++)
            {
                try
                {
                    if (SearchResult[i].Url == null)
                    {
                        SearchResult[i].Url = "无版权或获取错误";
                    }
                    if (SearchResult[i].Url.IndexOf("flac") != -1)
                    {
                        flacquantity++;
                    }
                }
                catch
                {
                }
            }
            Console.WriteLine("无损总数：" + flacquantity.ToString());
            PrinOutMusicInfo();
        }

        static void _GetMusicList(string ids)
        {
            List<Json.MusicInfo> ret = new List<Json.MusicInfo>();
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string _u = ApiUrl + "song/detail?ids=" + ids;
            string j = GetHTML(_u);
            Json.MusicDetails.Root mdr = JsonConvert.DeserializeObject<Json.MusicDetails.Root>(j);
            string u = ApiUrl + "song/url?id=" + ids + "&br=" + Setting.DownloadQuality;
            Json.GetUrl.Root urls = JsonConvert.DeserializeObject<Json.GetUrl.Root>(GetHTML(u));
            for (int i = 0; i < mdr.songs.Count; i++)
            {
                string singer = "";
                List<string> singerid = new List<string>();
                string _url = "";

                for (int x = 0; x < mdr.songs[i].ar.Count; x++)
                {
                    singer += mdr.songs[i].ar[x].name + "、";
                    singerid.Add(mdr.songs[i].ar[x].id.ToString());
                }

                for (int x = 0; x < urls.data.Count; x++)
                {
                    if (urls.data[x].id == mdr.songs[i].id)
                    {
                        _url = urls.data[x].url;
                    }
                }

                Json.MusicInfo mi = new Json.MusicInfo()
                {
                    Album = mdr.songs[i].al.name,
                    AlbumId = mdr.songs[i].al.id,
                    Id = mdr.songs[i].id,
                    LrcUrl = ApiUrl + "lyric?id=" + mdr.songs[i].id,
                    PicUrl = mdr.songs[i].al.picUrl,
                    Singer = singer.Substring(0, singer.Length - 1),
                    SingerId = singerid,
                    Song = mdr.songs[i].name,
                    Url = _url,
                    MvUrl = ApiUrl + "mv/url?id=" + mdr.songs[i].mv.ToString()
                };
                ret.Add(mi);
            }
            SearchResult.AddRange(ret);
        }

        static void SettingPage()
        {
            string json_ = JsonConvert.SerializeObject(Setting);
            StreamWriter sw_ = new StreamWriter("Setting.json");
            sw_.Write(json_);
            sw_.Flush();
            sw_.Close();
            Console.WriteLine("选择需要设置的项：");
            Console.WriteLine("1. 保存路径（" + Setting.SavePath + "）");
            Console.WriteLine("2. 搜索品质（" + Setting.DownloadQuality + "）");
            Console.WriteLine("3. 是否下载歌词（" + Setting.Ifdownloadlrc.ToString() + "）");
            Console.WriteLine("4. 是否下载专辑图片（" + Setting.Ifdownloadpic.ToString() + "）");
            Console.WriteLine("5. 文件名保存格式（" + Setting.Savenamestyle.ToString() + "）");
            Console.WriteLine("6. 文件路径保存格式（" + Setting.Savepathstyle.ToString() + "）");
            Console.WriteLine("7. 搜索数量（" + Setting.SearchQuantity.ToString() + "）");
            Console.WriteLine("0. 返回主界面");
            string k = Console.ReadLine();
            switch (k)
            {
                case "1":
                    Console.WriteLine("请输入保存路径");
                    string path = Console.ReadLine();
                    if (Directory.Exists(path) == false)
                    {
                        try
                        {
                            Directory.CreateDirectory(path);
                            Setting.SavePath = path;
                            Console.WriteLine("设置成功");
                        }
                        catch
                        {
                            Console.WriteLine("目录创建失败");
                        }
                    }
                    else
                    {
                        Setting.SavePath = path;
                        Console.WriteLine("设置成功");
                    }


                    SettingPage();
                    break;
                case "2":
                    Console.WriteLine("请选择搜索品质：");
                    Console.WriteLine("1. 999000");
                    Console.WriteLine("2. 320000");
                    Console.WriteLine("3. 192000");
                    Console.WriteLine("4. 128000");
                    string q = Console.ReadLine();
                    switch (q)
                    {
                        case "1":
                            Setting.DownloadQuality = "999000";
                            Console.WriteLine("设置成功");
                            break;
                        case "2":
                            Setting.DownloadQuality = "320000";
                            Console.WriteLine("设置成功");
                            break;
                        case "3":
                            Setting.DownloadQuality = "192000";
                            Console.WriteLine("设置成功");
                            break;
                        case "4":
                            Setting.DownloadQuality = "128000";
                            Console.WriteLine("设置成功");
                            break;
                        default:
                            Console.WriteLine("设置错误");
                            break;
                    }
                    SettingPage();
                    break;
                case "3":
                    Console.WriteLine("请选择是否下载歌词：");
                    Console.WriteLine("1. 是");
                    Console.WriteLine("2. 否");
                    string l = Console.ReadLine();
                    switch (l)
                    {
                        case "1":
                            Setting.Ifdownloadlrc = true;
                            Console.WriteLine("设置成功");
                            break;
                        case "2":
                            Setting.Ifdownloadlrc = false;
                            Console.WriteLine("设置成功");
                            break;
                        default:
                            Console.WriteLine("设置错误");
                            break;
                    }
                    SettingPage();
                    break;
                case "4":
                    Console.WriteLine("请选择是否下载专辑图片：");
                    Console.WriteLine("1. 是");
                    Console.WriteLine("2. 否");
                    string p = Console.ReadLine();
                    switch (p)
                    {
                        case "1":
                            Setting.Ifdownloadpic = true;
                            Console.WriteLine("设置成功");
                            break;
                        case "2":
                            Setting.Ifdownloadpic = false;
                            Console.WriteLine("设置成功");
                            break;
                        default:
                            Console.WriteLine("设置错误");
                            break;
                    }
                    SettingPage();
                    break;
                case "5":
                    Console.WriteLine("请选择文件名保存格式：");
                    Console.WriteLine("1. 歌曲名 - 歌手名");
                    Console.WriteLine("2. 歌手名 - 歌曲名");
                    string n = Console.ReadLine();
                    switch (n)
                    {
                        case "1":
                            Setting.Savenamestyle = 0;
                            Console.WriteLine("设置成功");
                            break;
                        case "2":
                            Setting.Savenamestyle = 1;
                            Console.WriteLine("设置成功");
                            break;
                        default:
                            Console.WriteLine("设置错误");
                            break;
                    }
                    SettingPage();
                    break;
                case "6":
                    Console.WriteLine("请选择文件路径保存格式：");
                    Console.WriteLine("1. .../");
                    Console.WriteLine("2. .../歌手/");
                    Console.WriteLine("3. .../歌手/专辑/");
                    string sp = Console.ReadLine();
                    switch (sp)
                    {
                        case "1":
                            Setting.Savepathstyle = 0;
                            Console.WriteLine("设置成功");
                            break;
                        case "2":
                            Setting.Savepathstyle = 1;
                            Console.WriteLine("设置成功");
                            break;
                        case "3":
                            Setting.Savepathstyle = 2;
                            Console.WriteLine("设置成功");
                            break;
                        default:
                            Console.WriteLine("设置错误");
                            break;
                    }
                    SettingPage();
                    break;
                case "0":
                    Console.Clear();
                    return;
                default:
                    Console.WriteLine("输入错误");
                    SettingPage();
                    break;
                case "7":
                    Console.WriteLine("请输入搜索数量（1-300）：");
                    string s = Console.ReadLine();
                    try
                    {
                        if (Int32.Parse(s) > 300 || Int32.Parse(s) <= 0)
                        {
                            int ss = 0;
                            do
                            {
                                Console.WriteLine("请输入1-300之间的数");
                                ss = Int32.Parse(Console.ReadLine());
                            }
                            while (ss > 300 || ss <= 0);
                            Setting.SearchQuantity = ss.ToString();
                            Console.WriteLine("设置成功");
                        }
                        else
                        {
                            Setting.SearchQuantity = s;
                            Console.WriteLine("设置成功");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("请输入数字");
                    }
                    break;
            }
        }

        static void PrinOutMusicInfo()
        {
            if (SearchResult != null)
            {
                for (int i = 0; i < SearchResult.Count; i++)
                {
                    string OutInfo = "";
                    OutInfo = (i + 1).ToString() + ". " + SearchResult[i].Song + " - " + SearchResult[i].Singer + " - " + SearchResult[i].Album;
                    Console.WriteLine(OutInfo);
                }

                Console.WriteLine();
                Console.WriteLine("1. 下载音乐");
                Console.WriteLine("2. 播放MV");
                Console.WriteLine("0. 返回主界面");
                string k = Console.ReadLine();
                switch (k)
                {
                    case "1":
                        Console.WriteLine("下载所有音乐输入-1。");
                        Console.WriteLine("下载指定音乐输入对应序号，多个音乐用 \",\" 隔开");
                        Console.WriteLine("返回输入0");
                        string Index = Console.ReadLine();
                        if (Index == "0")
                        {
                            Console.Clear();
                            PrinOutMusicInfo();
                        }
                        while (Index.Replace(" ", "") == "")
                        {
                            Console.WriteLine("输入不能为空");
                            Index = Console.ReadLine();
                        }
                        if (Index.IndexOf(",") != -1)
                        {
                            List<Json.DownloadList> dl = new List<Json.DownloadList>();
                            string[] index = Index.Split(',');
                            for (int i = 0; i < index.Length; i++)
                            {
                                if (Int32.Parse(index[i]) <= 0 || Int32.Parse(index[i]) > SearchResult.Count + 1)
                                {
                                    continue;
                                }

                                Json.DownloadList d = new Json.DownloadList()
                                {
                                    Album = SearchResult[Int32.Parse(index[i]) - 1].Album,
                                    ID = SearchResult[Int32.Parse(index[i]) - 1].Id.ToString(),
                                    LrcUrl = SearchResult[Int32.Parse(index[i]) - 1].LrcUrl,
                                    Singername = SearchResult[Int32.Parse(index[i]) - 1].Singer,
                                    Songname = SearchResult[Int32.Parse(index[i]) - 1].Song,
                                    Url = SearchResult[Int32.Parse(index[i]) - 1].Url,
                                    PicUrl = SearchResult[Int32.Parse(index[i]) - 1].PicUrl
                                };
                                dl.Add(d);
                            }
                            Thread a = new Thread(new ParameterizedThreadStart(Download));
                            a.Start(dl);
                            Console.ReadLine();
                        }
                        else
                        {
                            List<int> downloadindex = new List<int>();

                            if (Index == "-1")
                            {
                                for (int i = 0; i < SearchResult.Count; i++)
                                {
                                    downloadindex.Add(i);
                                }
                            }
                            else
                            {
                                if (Index.IndexOf("-") != -1 && Index != "-1")
                                {
                                    string[] s = Index.Split('-');
                                    while (Int32.Parse(s[0]) <= 0 || Int32.Parse(s[1]) > SearchResult.Count + 1 || Int32.Parse(s[0]) < Int32.Parse(s[1]) || s.Length != 2)
                                    {
                                        Console.WriteLine("输入有误，重新输入");
                                        s = Console.ReadLine().Split('-');
                                    }
                                    for (int i = 0; i < Int32.Parse(s[1]) - Int32.Parse(s[0]) + 1; i++)
                                    {
                                        downloadindex.Add(Int32.Parse(s[0]) - 1 + i);
                                    }
                                }
                                else
                                {
                                    while (Int32.Parse(Index) < -1 || Int32.Parse(Index) > SearchResult.Count + 1)
                                    {
                                        Console.WriteLine("输入有误，重新输入");
                                        Index = Console.ReadLine();
                                    }
                                    int index = Int32.Parse(Index) - 1;
                                    downloadindex.Add(index);
                                }
                            }

                            List<Json.DownloadList> dl = new List<Json.DownloadList>();
                            for (int i = 0; i < downloadindex.Count; i++)
                            {
                                Json.DownloadList d = new Json.DownloadList()
                                {
                                    Album = SearchResult[downloadindex[i]].Album,
                                    ID = SearchResult[downloadindex[i]].Id.ToString(),
                                    LrcUrl = SearchResult[downloadindex[i]].LrcUrl,
                                    Singername = SearchResult[downloadindex[i]].Singer,
                                    Songname = SearchResult[downloadindex[i]].Song,
                                    Url = SearchResult[downloadindex[i]].Url,
                                    PicUrl = SearchResult[downloadindex[i]].PicUrl
                                };
                                dl.Add(d);
                            }
                            Thread a = new Thread(new ParameterizedThreadStart(Download));
                            a.Start(dl);
                            Console.ReadLine();
                        }
                        break;
                    case "2":
                        Console.WriteLine("请输入歌曲序号");
                        string ind = Console.ReadLine();
                        Process.Start("explorer.exe ", GetMvUrl(SearchResult[Int32.Parse(ind) - 1].MvUrl));
                        PrinOutMusicInfo();
                        break;
                    case "0":
                        Console.Clear();
                        MainPage();
                        break;
                    default:
                        Console.WriteLine("命令不存在");
                        PrinOutMusicInfo();
                        break;
                }
            }
        }

        static string GetMvUrl(string jsonurl)
        {
            WebClient wc = new WebClient();
            Json.mvjson.Root mv = JsonConvert.DeserializeObject<Json.mvjson.Root>(GetHTML(jsonurl));
            return mv.data.url;
        }

        static void SearchPage()
        {
            Console.Clear();
            Console.WriteLine("请输入搜索内容");
            string key = Console.ReadLine();
            if (key == "" || key == null)
            {
                Console.WriteLine("搜索内容不能为空");
                Console.WriteLine();
                MainPage();
            }
            Console.WriteLine("正在搜索，请稍后。");
            SearchResult.Clear();
            Thread t = new Thread(new ParameterizedThreadStart(Search));
            t.Start(new ArrayList { key, Setting.SearchQuantity });
        }

        /// <summary>
        /// Parameter[0]是关键词，Parameter[1]是搜索数量
        /// </summary>
        /// <param name="Parameter"></param>
        static void Search(object Parameter)
        {
            SearchResult.Clear();
            ArrayList par = Parameter as ArrayList;
            string key = par[0].ToString();
            int quantity = Int32.Parse(par[1].ToString());
            int pagequantity = quantity / 100;
            int remainder = quantity % 100;

            if (remainder == 0)
            {
                remainder = 100;
            }
            if (pagequantity == 0)
            {
                pagequantity = 1;
            }

            for (int i = 0; i < pagequantity; i++)
            {
                if (i == pagequantity - 1 && pagequantity >= 1)
                {
                    SearchResult.AddRange(Search(key, i + 1, remainder));
                }
                else
                {
                    SearchResult.AddRange(Search(key, i + 1, 100));
                }
            }
            PrinOutMusicInfo();

        }

        static string NameCheck(string name)
        {
            string re = name.Replace("*", " ");
            re = re.Replace("\\", " ");
            re = re.Replace("\"", " ");
            re = re.Replace("<", " ");
            re = re.Replace(">", " ");
            re = re.Replace("|", " ");
            re = re.Replace("?", " ");
            re = re.Replace("/", ",");
            re = re.Replace(":", "：");
            re = re.Replace("-", "_");
            return re;
        }

        static void Download(object o)
        {
            Console.Clear();
            string songname = "";
            string ID = "";
            string singername = "";
            string downloadpath = "";
            string filename = "";
            List<Json.DownloadList> dl = (List<Json.DownloadList>)o;
            for (int i = 0; i < dl.Count; i++)
            {
                if (dl[i].Url == "无版权或获取错误")
                {
                    Console.WriteLine("无版权或获取错误");
                    Console.WriteLine();
                    continue;
                }

                songname = NameCheck(dl[i].Songname);
                ID = dl[i].ID;
                singername = NameCheck(dl[i].Singername);
                downloadpath = Setting.SavePath;
                if (Setting.Savepathstyle == 1)
                {
                    downloadpath += "\\" + singername;
                }
                if (Setting.Savepathstyle == 2)
                {
                    downloadpath += "\\" + singername + "\\" + NameCheck(dl[i].Album);
                }
                if (!Directory.Exists(downloadpath))
                {
                    Directory.CreateDirectory(downloadpath);
                }
                try
                {
                    if (dl[i].Url.IndexOf(".flac") != -1)
                    {
                        if (Setting.Savenamestyle == 0)
                        {
                            filename = songname + " - " + singername + ".flac";
                        }
                        else
                        {
                            filename = singername + " - " + songname + ".flac";
                        }
                    }
                    else
                    {
                        if (Setting.Savenamestyle == 0)
                        {
                            filename = songname + " - " + singername + ".mp3";
                        }
                        else
                        {
                            filename = singername + " - " + songname + ".mp3";
                        }
                    }
                }
                catch
                {
                    Console.WriteLine(songname + " - " + singername + " 下载错误");
                    Console.WriteLine();
                    continue;
                }
                if (!File.Exists(downloadpath + "\\" + filename))
                {
                    Console.WriteLine("正在下载：" + songname + " - " + singername);

                    try
                    {
                        DownloadFile(dl[i].Url, downloadpath + "\\" + filename);
                        Console.WriteLine();
                    }
                    catch
                    {
                        Console.WriteLine("音乐下载错误");
                    }
                    if (Setting.Ifdownloadlrc)
                    {
                        Console.WriteLine("开始下载歌词");
                        DownloadLrc(dl[i].LrcUrl, downloadpath + "\\" + filename.Replace(".mp3", "").Replace(".flac", "") + ".lrc");

                    }
                    if (Setting.Ifdownloadpic)
                    {
                        try
                        {
                            Console.WriteLine("开始下载专辑图片");
                            WebClient wc = new WebClient();
                            wc.DownloadFile(dl[i].PicUrl, downloadpath + "\\" + filename.Replace(".mp3", "").Replace(".flac", "") + ".jpg");
                        }
                        catch
                        {
                            Console.WriteLine("专辑图片下载错误");
                        }
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine("下载完成，双击回车回到上一界面");
            Console.WriteLine();
            Console.ReadLine();
            Console.Clear();
            PrinOutMusicInfo();
        }

        static void DownloadLrc(string jsonurl, string path)
        {
            StreamWriter sw = new StreamWriter(path);
            try
            {
                WebClient wc = new WebClient();
                Json.Lrcjson.Root l = JsonConvert.DeserializeObject<Json.Lrcjson.Root>(GetHTML(jsonurl));
                sw.Write(l.lrc.lyric);
                sw.Flush();
                sw.Close();
                Console.WriteLine("歌词下载完成");
            }
            catch
            {
                Console.WriteLine("歌词下载错误");
                sw.Close();
            }
        }

        static long GetFileLength(string url)
        {
            long length = 0;
            var req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            req.Method = "HEAD";
            req.Timeout = 5000;
            var res = (HttpWebResponse)req.GetResponse();
            if (res.StatusCode == HttpStatusCode.OK)
            {
                length = res.ContentLength;
            }
            res.Close();
            return length; // (length / 1024)单位 KB
        }

        /// <summary>
        /// Http方式下载文件
        /// </summary>
        /// <param name="url">http地址</param>
        /// <param name="savepath">本地文件</param>
        /// <returns></returns>
        static bool DownloadFile(string url, string savepath)
        {
            bool res = false;
            long startPosition = 0; // 上次下载的文件起始位置
            FileStream writeStream; // 写入本地文件流对象

            // 判断要下载的文件夹是否存在
            if (File.Exists(savepath))
            {
                writeStream = File.OpenWrite(savepath); // 存在则打开要下载的文件
                startPosition = writeStream.Length; // 获取已经下载的长度,可以结合"HTTP_HEAD获取文件长度"这篇文章算出下载进度百分比
                writeStream.Seek(startPosition, SeekOrigin.Current); // 本地文件写入位置定位
            }
            else
            {
                writeStream = new FileStream(savepath, FileMode.Create); // 创建一个文件
            }

            try
            {

                long filelength = GetFileLength(url);
                long havedownloadlength = 0;
                CustomeProgressBar pb = new CustomeProgressBar();
                Console.Write("文件大小：" + (filelength / 1024 / 1024).ToString() + "MB ");

                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(url); // 打开网络连接

                if (startPosition != 0)
                {
                    myRequest.AddRange((int)startPosition); // 设置Range值,与上面的writeStream.Seek用意相同,是为了定义远程文件读取位置
                }

                Stream readStream = myRequest.GetResponse().GetResponseStream(); // 向服务器请求,获得服务器的回应数据流

                byte[] btArray = new byte[512]; // 定义一个字节数据,用来向readStream读取内容和向writeStream写入内容
                int contentSize = readStream.Read(btArray, 0, btArray.Length); // 尝试读取文件

                while (contentSize > 0) // 如果读取长度大于零则续连成功,继续下载
                {
                    havedownloadlength += contentSize;
                    writeStream.Write(btArray, 0, contentSize); // 写入本地文件
                    double d1 = Convert.ToDouble(havedownloadlength);
                    double d2 = Convert.ToDouble(filelength);
                    int value = (int)((d1 / d2) * 50);
                    pb.Display(value);
                    contentSize = readStream.Read(btArray, 0, btArray.Length); // 继续向远程文件读取
                }

                //关闭流
                writeStream.Close();
                readStream.Close();

                res = true; //返回true下载成功
            }
            catch (Exception)
            {
                writeStream.Close();
                res = false; //返回false下载失败
            }

            return res;
        }


        static List<Json.MusicInfo> Search(string Key, int Page = 1, int limit = 100)
        {
            if (Key == null || Key == "")
            {
                return null;
            }
            WebClient wc = new WebClient();
            string offset = ((Page - 1) * 100).ToString();
            string url = ApiUrl + "search?keywords=" + Key + "&limit=" + limit.ToString() + "&offset=" + offset;
            string json = GetHTML(url);
            if (json == null || json == "")
            {
                return null;
            }
            Json.SearchResultJson.Root srj = JsonConvert.DeserializeObject<Json.SearchResultJson.Root>(json);
            List<Json.MusicInfo> ret = new List<Json.MusicInfo>();
            string ids = "";
            for (int i = 0; i < srj.result.songs.Count; i++)
            {
                ids += srj.result.songs[i].id + ",";
            }
            string _u = ApiUrl + "song/detail?ids=" + ids.Substring(0, ids.Length - 1);
            string j = GetHTML(_u);
            Json.MusicDetails.Root mdr = JsonConvert.DeserializeObject<Json.MusicDetails.Root>(j);
            string u = ApiUrl + "song/url?id=" + ids.Substring(0, ids.Length - 1) + "&br=" + Setting.DownloadQuality;
            Json.GetUrl.Root urls = JsonConvert.DeserializeObject<Json.GetUrl.Root>(GetHTML(u));

            for (int i = 0; i < mdr.songs.Count; i++)
            {
                string singer = "";
                List<string> singerid = new List<string>();
                string _url = "";

                for (int x = 0; x < mdr.songs[i].ar.Count; x++)
                {
                    singer += mdr.songs[i].ar[x].name + "、";
                    singerid.Add(mdr.songs[i].ar[x].id.ToString());
                }

                for (int x = 0; x < urls.data.Count; x++)
                {
                    if (urls.data[x].id == mdr.songs[i].id)
                    {
                        _url = urls.data[x].url;
                    }
                }

                Json.MusicInfo mi = new Json.MusicInfo()
                {
                    Album = mdr.songs[i].al.name,
                    AlbumId = mdr.songs[i].al.id,
                    Id = mdr.songs[i].id,
                    LrcUrl = ApiUrl + "lyric?id=" + mdr.songs[i].id,
                    PicUrl = mdr.songs[i].al.picUrl,
                    Singer = singer.Substring(0, singer.Length - 1),
                    SingerId = singerid,
                    Song = mdr.songs[i].name,
                    Url = _url,
                    MvUrl = ApiUrl + "mv/url?id=" + mdr.songs[i].mv.ToString()
                };
                ret.Add(mi);
            }
            return ret;
        }
    }
}
