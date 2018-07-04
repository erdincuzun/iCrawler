using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//additional packages
using System.Text.RegularExpressions;
using System.Collections;
using System.Net;
using System.IO;

using HTMLMarkerClass;

//windows form
using System.ComponentModel;

namespace Crawler
{
    public class kayit
    {
        public string URL;
        public string HTML_Content;
        public string XML_Content;
        public int URL_Count;
    }

    public class Crawler
    {   //base url
        public string default_url;
        //encoding sık kontrolü sorun yaratır.
        //en son encoding bilgisini kullan
        public string last_error;
        private Encoding last_encoding;
        private int error_cnt;

        //Hashtable link eklerken var olan linkleri eklememek için bellekte kontrolünde kullanıldı.
        //veritabanının yükü hafifletildi.
        public Hashtable all_links;
        private ArrayList ignore_List;

        //bazı linkler id, article_id ve özel bir numara içeriyor. Bunların kontrolü için kullanılacak.
        private ArrayList kayitlar;

        //for progressbar
        private BackgroundWorker _bg;
        private DoWorkEventArgs _e;
        

        public bool crawl(string baslangic_url, string directoryname)
        {
            all_links = new Hashtable();
            kayitlar = new ArrayList();

            string robots = download(baslangic_url + "robots.txt");
            Prepare_Ignore_Links(robots);

            download_given_links_secim(baslangic_url, directoryname, 0, 1, false, false);
            saveAllFiles(directoryname);

            return true;
        }

        public bool crawl(string baslangic_url, string directoryname, int count, int secim, BackgroundWorker _bg_form, DoWorkEventArgs e, bool one_page, bool allpages)
        {
            _bg = _bg_form;
            _e = e;

            all_links = new Hashtable();
            kayitlar = new ArrayList();

            string robots = download(baslangic_url + "robots.txt");
            Prepare_Ignore_Links(robots);

            download_given_links_secim(baslangic_url, directoryname, count, secim, one_page, allpages);
            //saveAllFiles(directoryname);

            return true;
        }

        //preapere testing data for training dataset
        public bool crawl_for_testing(string baslangic_url, string directoryname, int count)
        {
            all_links = new Hashtable();
            kayitlar = new ArrayList();

            download_given_links_for_Testing(baslangic_url, count);
            //saveAllFiles(directoryname);

            return true;
        }

        private void SaveLogFile(string directoryname, string url, string filename)
        {
            StreamWriter file = new StreamWriter(directoryname + "\\log.txt", true, Encoding.Unicode);
            file.WriteLine(filename + ";" + url);
            file.Close();
        }

        private void SaveHTMLFile(string directoryname, string html, string filename)
        {
            StreamWriter file = new StreamWriter(directoryname + "\\HTML\\" + filename, false, Encoding.Unicode);
            file.WriteLine(html);
            file.Close();
        }

        private void SaveXMLFile(string directoryname, ArrayList main, ArrayList headline, ArrayList additional, ArrayList summary, string filename)
        {
            StreamWriter file = new StreamWriter(filename, false, Encoding.Unicode);
            file.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
            file.WriteLine("<page>\r\n");
            foreach (string _m in main)
                file.WriteLine("<main>\r\n" + clear_illegal_characters_for_XML(stripHtml(_m)) + "\r\n</main>\r\n");
            foreach (string _h in headline)
                file.WriteLine("<headline>\r\n" + clear_illegal_characters_for_XML(stripHtml(_h)) + "\r\n</headline>\r\n");
            foreach (string _s in summary)
                file.WriteLine("<summary>\r\n" + clear_illegal_characters_for_XML(stripHtml(_s)) + "\r\n</summary>\r\n");
            foreach (string _a in additional)
                file.WriteLine("<additional>\r\n" + clear_illegal_characters_for_XML(stripHtml(_a)) + "\r\n</additional>\r\n");
            file.WriteLine("</page>\r\n");
            file.Close();
        }


        private void SaveRuleFile(string directoryname, Hashtable _rules)
        {
            Rules_Process.Save_Rules(_rules, directoryname + "\\rule.xml");
        }

        public string extract_links_by_ML(ArrayList _list)
        {
            string sonuc_AE = "";
            if (_list != null)
            {
                HTMLMarkerClass.desicionClass._list = _list; //_list gönder

                for (int i = 0; i < _list.Count; i++)
                {
                    HTMLMarkerClass.element _element = (HTMLMarkerClass.element)_list[i];

                    if (HTMLMarkerClass.desicionClass.write_or_not("links", _element))
                    {
                        sonuc_AE = sonuc_AE + " " + _element.outerHTML_AE;
                    }
                }
            }
            return sonuc_AE;
        }

        public string extract_links_by_Rules(string htmlcontent, Hashtable _ht_rules)
        {
            string sonuc_AE = "";
            ArrayList _links_list = efficientextraction.extractRules(_ht_rules, htmlcontent, "links");


            for (int i = 0; i < _links_list.Count; i++)
            {
                sonuc_AE = sonuc_AE + " " + (string)_links_list[i];
            }

            return sonuc_AE;
        }

        public string clear_illegal_characters_for_XML(string strOutput)
        {
            strOutput = strOutput.Replace("<", " ");
            strOutput = strOutput.Replace(">", " ");
            strOutput = strOutput.Replace("\"", " ");
            strOutput = strOutput.Replace("&", " ");
            strOutput = strOutput.Replace("€", " ");
            strOutput = strOutput.Replace("�", " ");
            strOutput = strOutput.Replace("|", " ");

            return strOutput;
        }

        public static string stripHtml(string strOutput)
        {
            //Strips the HTML tags from strHTML
            if (strOutput != null)
            {
                string temp = HTML.stripHtml(HTML.trimScript(strOutput));
                if (temp.Trim() != "")
                    strOutput = temp;

                temp = HTML.stripHtml(HTML.trimDIV(strOutput));
                if (temp.Trim() != "")
                    strOutput = temp;

                temp = HTML.stripHtml(HTML.trimTD(strOutput));
                if (temp.Trim() != "")
                    strOutput = temp;

                System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex("<(.|\n)+?>");
                strOutput = _regex.Replace(strOutput, " ");
            }
            return strOutput;
        }

        private void CreateDOM(ref DOM _dom_temp, string _htmldoc)
        {
            _htmldoc = _htmldoc.Replace("\r\n", "");
            _htmldoc = _htmldoc.Replace("\r", "");
            _htmldoc = _htmldoc.Replace("\n", "");
            if (_htmldoc.Trim() != "")
                _dom_temp.prepareDOM(_htmldoc);
        }

        //dom to xml main, headline, summary, additional
        private ArrayList PrepareContent(string _class, ArrayList _list)
        {
            ArrayList _al = new ArrayList();
            if (_list != null)
            {
                HTMLMarkerClass.desicionClass._list = _list; //_list gönder

                for (int i = 0; i < _list.Count; i++)
                {
                    HTMLMarkerClass.element _element = (HTMLMarkerClass.element)_list[i];

                    if (HTMLMarkerClass.desicionClass.write_or_not(_class, _element))
                    {
                        _al.Add(_element.outerHTML_AE);
                    }
                }
            }

            return _al;
        }

        // Display results to a text
        public int download_given_links_secim(string baslangic_url, string DirectoryName, int count, int secim, bool one_page, bool allpages)
        {
            string dirinfo = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + "." + DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second;

            default_url = baslangic_url;
            default_url = baslangic_url.ToString().Substring(0, baslangic_url.ToString().LastIndexOf("/") + 1);

            kayit _k = new kayit();

            string html_content = download(baslangic_url);
            _k.URL = baslangic_url;
            _k.HTML_Content = html_content;

            //dom
            ML_Rules _mr = new ML_Rules(); //rules
            if (secim == 1 || secim == 2)
                _mr._ht_rules = Rules_Process.Load_Rules(DirectoryName + "\\rule.xml"); 

            DOM _dom = new DOM();
            if ((secim == 1 || secim == 3) && one_page == false)
            {
                CreateDOM(ref _dom, html_content);
                _mr = new ML_Rules(_dom._list, _mr._ht_rules);
                if (html_content != "")
                    _k.URL_Count = Add_links_to_Hashtable(html_content, baslangic_url);
            }

            if ((secim == 1 || secim == 3) && one_page == true)
            {
                CreateDOM(ref _dom, html_content);
                _mr = new ML_Rules(_dom._list, _mr._ht_rules);
                if (html_content != "")
                {
                    _k.URL_Count = Add_links_to_Hashtable(extract_links_by_ML(_dom._list), baslangic_url);
                    count = _k.URL_Count;
                }
            }

            if ((secim == 2) && one_page == true)
            {
                if (html_content != "")
                {
                    _k.URL_Count = Add_links_to_Hashtable(extract_links_by_Rules(html_content, _mr._ht_rules), baslangic_url);
                    count = _k.URL_Count;
                }
            }

            if ((secim == 2) && one_page == false)
            {
                if (html_content != "")
                    _k.URL_Count = Add_links_to_Hashtable(html_content, baslangic_url);
            }

            if (one_page == false & allpages == true)
                count = _k.URL_Count;


            kayitlar.Add(_k);
            
            int i = 1;

            int cnt = all_links.Count;//all records, changeable

            if (count > 0)
                cnt = count;

            if (!Directory.Exists(DirectoryName))
                Directory.CreateDirectory(DirectoryName);
            if (!Directory.Exists(DirectoryName + "\\" + dirinfo))
                Directory.CreateDirectory(DirectoryName + "\\" + dirinfo);
            if (!Directory.Exists(DirectoryName + "\\" + dirinfo + "\\XML"))
                Directory.CreateDirectory(DirectoryName + "\\" + dirinfo + "\\XML");
            if (!Directory.Exists(DirectoryName + "\\" + dirinfo + "\\HTML"))
                Directory.CreateDirectory(DirectoryName + "\\" + dirinfo + "\\HTML");
            //save files
            SaveHTMLFile(DirectoryName + "\\" + dirinfo, _k.URL, "0.html");
            SaveLogFile(DirectoryName + "\\" + dirinfo, _k.URL, DirectoryName + "\\" + dirinfo + "\\HTML\\" + "0.txt");

            if(all_links.Count > 0)
            {
                int docno = 1;
                while (i < cnt)
                {
                    if (i >= all_links.Count)
                        break;

                    _k = new kayit();
                    html_content = download(all_links[i - 1].ToString());

                    if (html_content != "")
                    {
                        _k.URL = all_links[i - 1].ToString();
                        _k.HTML_Content = html_content;
                        default_url = all_links[i - 1].ToString().Substring(0, all_links[i - 1].ToString().LastIndexOf("/") + 1);                       

                        //save files
                        SaveHTMLFile(DirectoryName + "\\" + dirinfo, _k.HTML_Content, i.ToString() + ".html");
                        SaveLogFile(DirectoryName + "\\" + dirinfo, _k.URL, DirectoryName + "\\" + dirinfo + "\\HTML\\" + i.ToString() + ".txt");

                        ArrayList _main_list = efficientextraction.extractRules(_mr._ht_rules, _k.HTML_Content, "main");
                        if (_main_list.Count > 0)
                        {
                            ArrayList _headline_list = efficientextraction.extractRules(_mr._ht_rules, _k.HTML_Content, "headline");
                            ArrayList _summary_list = efficientextraction.extractRules(_mr._ht_rules, _k.HTML_Content, "summary");
                            ArrayList _additional_list = efficientextraction.extractRules(_mr._ht_rules, _k.HTML_Content, "additional");
                            //Remove Repetative Parts
                            Rules_Process.RemoveRepetativeParts(ref _main_list, ref _additional_list, ref _summary_list, ref _additional_list, _mr._ht_rules);

                            SaveXMLFile(DirectoryName, _main_list, _headline_list, _additional_list, _summary_list, DirectoryName + "\\" + dirinfo + "\\XML\\" + i.ToString() + ".xml");
                        }
                        else
                        {
                            if (secim == 1 || secim == 3)
                            {
                                _dom = new DOM();
                                CreateDOM(ref _dom, html_content);
                                ArrayList _main_list_ML = PrepareContent("main", _dom._list);
                                if (_main_list_ML.Count > 0)
                                {
                                    ArrayList _headline_list_ML = PrepareContent("headline", _dom._list);
                                    ArrayList _summary_list_ML = PrepareContent("summary", _dom._list);
                                    ArrayList _additional_list_ML = PrepareContent("additional", _dom._list);
                                    //Remove Repetative Parts
                                    Rules_Process.RemoveRepetativeParts(ref _main_list_ML, ref _additional_list_ML, ref _summary_list_ML, ref _additional_list_ML, _mr._ht_rules);

                                    SaveXMLFile(DirectoryName, _main_list_ML, _headline_list_ML, _additional_list_ML, _summary_list_ML, DirectoryName + "\\" + dirinfo + "\\XML\\" + i.ToString() + ".xml");
                                }
                                else
                                {
                                    if (one_page == false)
                                    {
                                        _k.URL_Count = Add_links_to_Hashtable(html_content, baslangic_url);//all a href add because of not contain main 
                                        if (allpages == true)
                                            count = count + _k.URL_Count;
                                    }
                                }
                                //rule'lar kullanılacak önceki ile kontrol et
                                _mr = new ML_Rules(_dom._list, _mr._ht_rules);
                            }
                        }

                        kayitlar.Add(_k);
                        docno++;
                    }
                    else
                        cnt++;

                    int durum = (int)((double)i / count * 100);
                    _bg.ReportProgress(durum);

                    if (_bg.CancellationPending)
                    {
                        _e.Cancel = true;
                        break;
                    }

                    i++;
                }//while i    

                if (secim == 1 || secim == 3)
                    SaveRuleFile(DirectoryName, _mr._ht_rules);
            }

            _bg.ReportProgress(100);

            return 0;
        }

        // Display results to a text
        public int download_given_links_for_Testing(string baslangic_url, int count)
        {
            default_url = baslangic_url;
            default_url = baslangic_url.ToString().Substring(0, baslangic_url.ToString().LastIndexOf("/") + 1);

            kayit _k = new kayit();

            string html_content = download(baslangic_url);
            _k.URL = baslangic_url;
            _k.HTML_Content = html_content;
            if (html_content != "")
                _k.URL_Count = Add_links_to_Hashtable(html_content, baslangic_url);

            kayitlar.Add(_k);

            int i = 1;

            int cnt = all_links.Count;//all records, changeable

            if (count > 0)
                cnt = count;

            while (i < cnt)
            {
                _k = new kayit();
                html_content = download(all_links[i - 1].ToString());

                if (html_content != "")
                {
                    _k.URL = all_links[i - 1].ToString();
                    _k.HTML_Content = html_content;
                    default_url = all_links[i - 1].ToString().Substring(0, all_links[i - 1].ToString().LastIndexOf("/") + 1);

                    _k.URL_Count = Add_links_to_Hashtable(html_content, baslangic_url);

                    kayitlar.Add(_k);
                }
                i++;
            }

            return 0;
        }

        public bool saveAllFiles(string directoryname)
        {
            if (!Directory.Exists(directoryname))
            {
                Directory.CreateDirectory(directoryname);
            }

            StreamWriter file = new StreamWriter(directoryname + "\\0000.txt", false, Encoding.Unicode);
            foreach (kayit _k in kayitlar)
            { 
                file.WriteLine(_k.URL + "," + _k.URL_Count.ToString() + "," + _k.HTML_Content.Length.ToString());
            }

            file.Close();

            int i = 1;
            foreach (kayit _k in kayitlar)
            {
                StreamWriter file2;                
                if(i.ToString().Length == 1)
                    file2 = new StreamWriter(directoryname + "\\000" + i.ToString() + ".html", false, Encoding.Unicode);
                else if (i.ToString().Length == 2)
                    file2 = new StreamWriter(directoryname + "\\00" + i.ToString() + ".html", false, Encoding.Unicode);
                else if (i.ToString().Length == 3)
                    file2 = new StreamWriter(directoryname + "\\0" + i.ToString() + ".html", false, Encoding.Unicode);
                else
                    file2 = new StreamWriter(directoryname + "\\" + i.ToString() + ".html", false, Encoding.Unicode);

                file2.WriteLine(_k.HTML_Content);
                file2.Close();
                i++;
            }

            return true;
        }

        // Display results to a text
        public string download(string URL)
        {
            WebResponse response = GetResponse(URL);

            if (response != null)
            {
                if (last_encoding == null)
                    last_encoding = Encoding.Default;

                StreamReader reader = new StreamReader(response.GetResponseStream(), last_encoding);

                string content = reader.ReadToEnd();

                if (error_cnt == 2)
                {
                    last_error = "2 kere aynı sayfa istendi...";
                    return "";
                }

                if (content.Contains("�"))
                {
                    error_cnt++;

                    last_encoding = Encoding.Default;
                    content = download(URL);
                }

                if (content.Contains("Ã") || content.Contains("Ä"))
                {
                    error_cnt++;

                    last_encoding = Encoding.UTF8;
                    content = download(URL);
                }

                error_cnt = 0;


                //Eğer sayfa bulunamadı türünden hata var ise içeri temizle
                //Milliyet için
                //<strong>SAYFA BULUNAMADI  !!!</strong>
                //Hürriyet için
                //Aradığınız sayfa <b>http://www.hurriyet.com.tr</b> üzerinde bulunamadı.
                //Sabah için
                //Sayfa Bulunamadı!</font>

                content = RepeairTurkishCharacter(content);


                return content;
            }

            else return "";
        }

        // Display results to a webpage
        private WebResponse GetResponse(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.KeepAlive = false;
                request.Method = "GET";
                request.AllowAutoRedirect = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.MediaType = "text/html;charset=utf-8";
                request.UserAgent = "MSIE 7.0";

                //if (ozeldurum == true)
                //{
                 /*   request.ContentType = "application/x-www-form-urlencoded";
                    request.MediaType = "text/html;charset=utf-8";
                    request.UserAgent = "MSIE 7.0";*/
                //}
                //request.Proxy = WebProxy.GetDefaultProxy();
                //request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                return request.GetResponse();
            }

            catch (WebException e)
            {
                last_error = "Web sayfası açılamadı: " + e;
                return null;
            }

            catch (IOException e)
            {
                last_error = "Dosya Yaratılamadı: " + e;
                return null;
            }
        }

        //REGULAR EXPRESSIONS for extracting links
        public int Add_links_to_Hashtable(string html_content, string baslangic_url)
        {
            int cnt = all_links.Count;
            int total_links_in_a_web_page = 0;
            //links in javascript //for Milliyet
            string pattern = @"openWindow\('(.*?)'\)";
            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matchList = exp.Matches(html_content);

            total_links_in_a_web_page = total_links_in_a_web_page + matchList.Count;

            for (int i = 0; i < matchList.Count; i++)
            {
                Match match = matchList[i];
                if (match.Value.Length > 0)
                {
                    string web_page = match.Groups[1].Value;

                    web_page = web_page.ToLower();

                    if (web_page.Length != 0)
                    {
                        web_page = repair_link(web_page, baslangic_url);
                        if (web_page != "")
                        {
                            //primary key                           
                            if (!all_links.ContainsValue(web_page))
                            {
                                all_links.Add(cnt++, web_page);
                            }
                        }
                    }
                }
            }

            //for Sabah 2002-2003
            pattern = @"popup\('(.*?)'";
            exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            matchList = exp.Matches(html_content);

            total_links_in_a_web_page = total_links_in_a_web_page + matchList.Count;

            for (int i = 0; i < matchList.Count; i++)
            {
                Match match = matchList[i];
                if (match.Value.Length > 0)
                {
                    string web_page = match.Groups[1].Value;

                    web_page = web_page.ToLower();

                    if (web_page.Length != 0)
                    {
                        web_page = repair_link(web_page, baslangic_url);
                        if (web_page != "")
                        {
                            all_links.Add(cnt++, web_page);
                        }
                    }
                }
            }


            //for a href
            pattern = "href=\"(.*?)\"";            
            exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            matchList = exp.Matches(html_content);

            total_links_in_a_web_page = total_links_in_a_web_page + matchList.Count;

            for (int i = 0; i < matchList.Count; i++)
            {
                Match match = matchList[i];
                if (match.Value.Length > 0)
                {
                    string web_page = match.Groups[1].Value;
                    if (web_page.Length != 0)
                    {
                        web_page = repair_link(web_page, baslangic_url);
                        if (web_page != "")
                        {
                            //primary key                           
                            if (!all_links.ContainsValue(web_page))
                            {
                                all_links.Add(cnt++, web_page);
                            }
                        }
                    }
                }
            }

            //for a href2
            pattern = "href='(.*?)'";
            exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            matchList = exp.Matches(html_content);

            total_links_in_a_web_page = total_links_in_a_web_page + matchList.Count;

            for (int i = 0; i < matchList.Count; i++)
            {
                Match match = matchList[i];
                if (match.Value.Length > 0)
                {
                    string web_page = match.Groups[1].Value;
                    if (web_page.Length != 0)
                    {
                        web_page = repair_link(web_page, baslangic_url);
                        if (web_page != "")
                        {
                            //primary key                           
                            if (!all_links.ContainsValue(web_page))
                            {
                                all_links.Add(cnt++, web_page);
                            }
                        }
                    }
                }
            }

            //for xml
            pattern = "<link>(.*?)</link>";
            exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            matchList = exp.Matches(html_content);

            total_links_in_a_web_page = total_links_in_a_web_page + matchList.Count;

            for (int i = 0; i < matchList.Count; i++)
            {
                Match match = matchList[i];
                if (match.Value.Length > 0)
                {
                    string web_page = match.Groups[1].Value;
                    if (web_page.Length != 0)
                    {
                        web_page = repair_link(web_page, baslangic_url);
                        if (web_page != "")
                        {
                            //primary key                           
                            if (!all_links.ContainsValue(web_page))
                            {
                                all_links.Add(cnt++, web_page);
                            }
                        }
                    }
                }
            }

            
            
            return total_links_in_a_web_page;
        }

        public void Prepare_Ignore_Links(string html_content)
        {
            ignore_List = new ArrayList();
            string pattern = @"(?<Permission>(?:disallow))(?:\s*:\s*)(?<Url>[/0-9_a-z.]*)";
            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matchList = exp.Matches(html_content);

            for (int i = 0; i < matchList.Count; i++)
            {
                Match match = matchList[i];
                if (match.Value.Length > 0)
                {
                    string ignore_url = match.Groups[2].Value;
                    if(ignore_url != "/")
                    ignore_List.Add(ignore_url);
                }
            }
        }

        //linkleri kontrol etmeliyiz o web sitesi dışında bir durum varsa eklememeliyiz.
        public string repair_link(string web_page, string baslangic_url)
        {

            web_page = web_page.Replace("amp;", "");
            string base_url = FindBaseUrl(baslangic_url);

            web_page = web_page.Trim();

            string[] ignore_Keywords = { ".xml", "ajax", "webservice", "javascript", ".ico", ".ICO", ".jpg", ".JPG", 
                                         ".png", "gif", "GIF", "mailto", "rss", ".css", "#"};

            if (web_page.Length > 0)
            {

                foreach (string _cntrl in ignore_Keywords)
                    if (web_page.Contains(_cntrl))
                        return "";

                //robots.txt
                foreach (string _ignore_url in ignore_List)
                    if (web_page.Contains(_ignore_url))
                        return "";

                if (web_page.Length > 1)
                {
                    if (web_page == base_url || web_page == base_url.Substring(0, base_url.Length - 1))
                        return "";//kendisi
                    else if (web_page.Length == 1 && web_page[0] == '/')
                        return "";
                    else if (web_page.Substring(0,2) == "/?")
                        return "";
                }

                if (web_page[0] == '/')
                    web_page = base_url.Substring(0, base_url.Length - 1) + web_page;
                else if (web_page[0] == '?')
                    return "";                 
            }
            else
                return "";

            //.xxx.com formatında kontrol için
            string base_url2 = base_url.Replace("http://", "");
            base_url2 = base_url2.Replace("/", "");
            base_url2 = base_url2.Replace("www.", "");

            if (!web_page.Contains(base_url2))
                return "";

            return web_page;
        }

        //İşlem yapılan URL bulunması için
        public string FindBaseUrl(string url)
        {
            string result = "";
            string pattern = @"http://.*?/";
            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matchList = exp.Matches(url);

            if (matchList.Count > 0)
            {
                Match match = matchList[0];
                result = match.Groups[0].Value;
            }           

            return result;
        }

        private string RepeairTurkishCharacter(string content)
        {
            content = content.Replace("&#252;", "ü");
            content = content.Replace("&#220;", "Ü");
            content = content.Replace("&#231;", "ç");
            content = content.Replace("&#199;", "Ç");
            content = content.Replace("&#351;", "ş");
            content = content.Replace("&#350;", "Ş");
            content = content.Replace("&#286;", "Ğ");
            content = content.Replace("&#287;", "ğ");
            content = content.Replace("&#305;", "ı");
            content = content.Replace("&#304;", "İ");
            content = content.Replace("&#246;", "ö");
            content = content.Replace("&#214;", "Ö");
            return content;
        }

        //http://arsiv.sabah.com.tr/arsiv/2003/01/02/s02.html
        //http://webarsiv.hurriyet.com.tr/2003/12/31/hurriyetim.asp
        //http://arsiv.sabah.com.tr/2004/01/02/yaz27-50-105-20040101.html
        //http://www.sabah.com.tr/Yasam/2010/03/14/mucevher_fuarinda_hirsizlik
        //http://www.milliyet.com.tr/hakkari-de-askerlere-ates-acildi/siyaset/sondakika/14.03.2010/1211267/default.htm?ver=32
        //http://hurarsiv.hurriyet.com.tr/goster/haberler.aspx?id=2065&tarih=2008-03-02
        //http://www.milliyet.com.tr/2006/01/01/

        private static bool IsInteger(string theValue)
        {
            try
            {
                Convert.ToInt32(theValue);
                return true;
            }
            catch
            {
                return false;
            }
        } //IsInteger
       
    }
}