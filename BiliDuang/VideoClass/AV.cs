﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace BiliDuang.VideoClass
{
    public class VideoQuality
    {
        public static int Q360P = 16;
        public static int Q480P = 32;
        public static int Q720P = 64;
        //以下要大会员
        public static int Q720P60 = 74;
        public static int Q1080P = 80;
        public static int Q1080PP = 112;
        public static int Q1080P60 = 116;

        public static int Int(string q)
        {
            switch (q)
            {
                case "1080P60":
                    return 116;
                case "1080P+":
                    return 112;
                case "1080P":
                    return 80;
                case "720P60":
                    return 74;
                case "720P":
                    return 64;
                case "480P":
                    return 32;
                case "320P":
                    return 16;
                default:
                    return 116;

            }
        }

        public static string Name(int quality)
        {
            switch (quality)
            {
                case 16:
                    return "流畅 360P";
                case 32:
                    return "清晰 480P";
                case 64:
                    return "高清 720P";
                case 74:
                    return "高清 720P60";
                case 80:
                    return "超清 1080P";
                case 112:
                    return "超清 1080P+";
                case 116:
                    return "超清 1080P60";
                default:
                    return "未知清晰度";
            }
        }
    }

    public class episode
    {
        public string aid;
        public string cid;
        public string avname;
        public string pic
        {
            get
            {
                return _pic;
            }
            set
            {
                if (value.Contains("http"))
                {


                    string deerory = Environment.CurrentDirectory + "/temp/";

                    string fileName = string.Format("{0}-{1}.jpg", aid, cid);
                    if (!File.Exists(deerory + fileName))
                    {
                        WebRequest imgRequest = WebRequest.Create(value);

                        HttpWebResponse res;
                        try
                        {
                            res = (HttpWebResponse)imgRequest.GetResponse();
                        }
                        catch (WebException ex)
                        {

                            res = (HttpWebResponse)ex.Response;
                        }

                        if (res.StatusCode.ToString() == "OK")
                        {
                            System.Drawing.Image downImage = System.Drawing.Image.FromStream(imgRequest.GetResponse().GetResponseStream());




                            if (!System.IO.Directory.Exists(deerory))
                            {
                                System.IO.Directory.CreateDirectory(deerory);
                            }

                            downImage.Save(deerory + fileName);

                            downImage.Dispose();
                            _pic = deerory + fileName;
                        }
                    }
                    else
                    {
                        _pic = deerory + fileName;
                    }
                }
                else
                {
                    if (File.Exists(value))
                    {
                        _pic = value;
                    }
                }
            }
        }
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                value = value.Replace("\\", " ");
                value = value.Replace("/", " ");
                _name = value;
            }
        }
        private string _pic;
        public string savedir;
        public string missonname;
        public int quality;
        private string saveto;

        public void Download(string saveto)
        {
            this.saveto = saveto;
            DownloadObject dobject = new DownloadObject(aid, cid, quality, saveto, name,avname);
            int index = DownloadQueue.AddDownload(dobject);
            //DownloadQueue.objs[index].Start();
        }

        public void Download(bool reald = true)
        {
            DownloadObject dobject = new DownloadObject(aid, cid, quality, saveto, name,avname);
            int index = DownloadQueue.AddDownload(dobject);

        }

        public void Download(string saveto, int quality)
        {
            this.saveto = saveto;
            this.quality = quality;
            DownloadObject dobject = new DownloadObject(aid, cid, quality, saveto, name,avname);
            int index = DownloadQueue.AddDownload(dobject);

        }

        public void DownloadDanmaku(string saveto)
        {
            //todo
            //savedir+".ass"
            //"https://api.bilibili.com/x/v1/dm/list.so?oid="+ cid
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            string content = Other.GetHtml("https://api.bilibili.com/x/v1/dm/list.so?oid=" + cid);
            Byte[] pageData = MyWebClient.DownloadData("https://api.bilibili.com/x/v1/dm/list.so?oid=" + cid); //从指定网站下载数据
            string xmlname = saveto + "\\" + name + ".xml";
            File.WriteAllText(xmlname, content);
            string back = Other.HTTPGetXML("https://api.bilibili.com/x/v1/dm/list.so?oid=" + cid);
            XDocument xml = XDocument.Parse(back);
            //Console.WriteLine(xml.Element("i").Element("chatserver").Value);
            //xml.Element("i").Elements("d");
            return;
            //TODO
        }

    }

    public class AV
    {
        public readonly bool status;
        public readonly string aid;
        public readonly bool isbangumi;
        public readonly string bangumiurl;
        public readonly string name;
        public readonly string des;
        public readonly List<episode> episodes = new List<episode>();
        public readonly bool vip;
        public readonly UP up = new UP();
        private string _pic;
        public readonly string cid;
        public string imgurl
        {
            get
            {
                return _pic;
            }
            set
            {
                string deerory = Environment.CurrentDirectory + "/temp/";

                string fileName = string.Format("{0}.jpg", aid);
                if (!File.Exists(deerory + fileName))
                {
                    WebRequest imgRequest = WebRequest.Create(value);

                    HttpWebResponse res;
                    try
                    {
                        res = (HttpWebResponse)imgRequest.GetResponse();
                    }
                    catch (WebException ex)
                    {

                        res = (HttpWebResponse)ex.Response;
                    }

                    if (res.StatusCode.ToString() == "OK")
                    {
                        System.Drawing.Image downImage = System.Drawing.Image.FromStream(imgRequest.GetResponse().GetResponseStream());




                        if (!System.IO.Directory.Exists(deerory))
                        {
                            System.IO.Directory.CreateDirectory(deerory);
                        }

                        downImage.Save(deerory + fileName);

                        downImage.Dispose();
                        _pic = deerory + fileName;
                    }
                }
                else
                {
                    _pic = deerory + fileName;
                }
            }
        }

        private JSONCallback.AV.AV av;

        public AV(string aid, bool nonotice = false)
        {
            this.aid = aid;
            //https://api.bilibili.com/x/web-interface/view/detail?aid=81012897&jsonp=json

            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            string callback = Encoding.UTF8.GetString(MyWebClient.DownloadData("https://api.bilibili.com/x/web-interface/view/detail?aid=" + aid + "&jsonp=json")); //如果获取网站页面采用的是UTF-8，则使用这句
            av = JsonConvert.DeserializeObject<JSONCallback.AV.AV>(callback);
            MyWebClient.Dispose();
            if (av.code != 0)
            {
                if (!nonotice)
                {
                    Dialog.Show(av.message, "获取错误");
                }
                name = av.message;
                status = false;
                return;
            }
            if (av.data.View.redirect_url != null)
            {
                if (av.data.View.redirect_url.Contains("ep"))
                {
                    isbangumi = true;
                    bangumiurl = av.data.View.redirect_url;
                    return;
                }
            }
            status = true;
            cid = av.data.View.cid;
            name = av.data.View.title;
            des = av.data.View.desc;
            up.id = av.data.Card.card.mid;
            up.name = av.data.Card.card.name;
            up.imgurl = av.data.Card.card.face;
            imgurl = av.data.View.pic;
            foreach (JSONCallback.AV.PagesItem page in av.data.View.pages)
            {
                episode episode = new episode();
                episode.cid = page.cid;
                episode.pic = _pic;
                episode.name = page.part;
                episode.aid = aid;
                episode.avname = this.name;
                episodes.Add(episode);
            }

        }

    }
}