using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using mshtml;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace HTMLMarkerClass
{
    //features that is used in machine learning
    public class element
    {
        public int id;
        public int elementlinked_id;

        public string elementName;
        public string parent_elementName;
        public string tagName;

        public string tag_id_Name;
        public string tag_class_Name;
        public int tag_id;
        public int tag_class;
        public int tag_idORclass;
        

        public string outerHTML;
        public string innerHTML;

        public string BagofWords;

        public int wordCount;
        public double DensityinHTML;
        public int LinkCount;
        public int wordCountinLink;
        public double meanofWordinLinks;
        public double meanofWordinLinksAllWords;
        public int dot_count;
        public int h1_count;
        public int h2_count;
        public int h3_count;
        public int h4_count;
        public int h5_count;
        public int h6_count;
        public int img_count;
        public int p_count;
        public int br_count;
        public int span_count;
        public int object_count;
        public int ul_count;
        public int li_count;
        public int input_count;
        public int div_count;
        public int td_count;

        //etiketin tüm body içinde tekrar sayısı
        public int repeat_tag_count;

        //wordCountinLink/LinkCount

        public string outerHTML_AE;//After Extraction(AE) nested tags
        public string innerHTML_AE;
        public string BagofWords_AE;

        public int wordCount_AE;
        public double DensityinHTML_AE;
        public int LinkCount_AE;
        public int wordCountinLink_AE;
        public double meanofWordinLinks_AE;
        public double meanofWordinLinksAllWords_AE;
        public int dot_count_AE;
        public int h1_count_AE;
        public int h2_count_AE;
        public int h3_count_AE;
        public int h4_count_AE;
        public int h5_count_AE;
        public int h6_count_AE;
        public int img_count_AE;
        public int p_count_AE;
        public int br_count_AE;
        public int span_count_AE;
        public int object_count_AE;
        public int ul_count_AE;
        public int li_count_AE;
        public int input_count_AE;
        public int div_count_AE;
        public int td_count_AE;       

        //similarity between layouts
        public double sim_bagofword;
        public double sim_bagofword_AE;
        public double sim_innerHTML;
        public double sim_innerHTML_AE;
    }

    //DOM and feature extraction
    public class DOM
    {
        public string domhtmlContent;
        public string savehtmlContent;
        public string resulthmtlContent;
        public string all_words;
        public Hashtable _ht;
        public ArrayList _list;
        public ArrayList _xmllist;

        public ArrayList prepareDOM(string htmlContent2)
        {
            string htmlContent = htmlContent2;
            htmlContent = HTML.trim_commenttags(htmlContent);
            htmlContent = HTML.trimOptions(htmlContent);
            htmlContent = HTML.trimScript(htmlContent);
            htmlContent = HTML.trim_HREF_SCR(htmlContent);
            htmlContent = HTML.trim_some_cases(htmlContent);
            //for fast processing otherwise image, link, javascript loading...

            IHTMLDocument2 htmlDocument = new mshtml.HTMLDocumentClass();
            
            htmlDocument.write(htmlContent);

            IHTMLElementCollection allElements = htmlDocument.all;
            _ht = new Hashtable();
            _list = new ArrayList();
            _xmllist = new ArrayList();

            string _tempinner_text = "";
            if (htmlDocument.body != null)
                if (htmlDocument.body.innerText != null)
                {
                    _tempinner_text = htmlDocument.body.innerText.Replace("\r\n", "");
                    domhtmlContent = htmlDocument.body.outerHTML.Replace("\r\n", "");
                }

            element _firstelement = AnalyzeGivenHTML(htmlDocument.body.innerHTML, _tempinner_text);
            all_words = _firstelement.BagofWords;
            int i = 0;
            foreach (IHTMLElement htmlelement in allElements)
            {
               
                if (htmlelement.outerHTML != null)
                {
                    element _element = new element();
                    _element.id = i;
                    _element.outerHTML = htmlelement.outerHTML;     
                    _element.outerHTML = _element.outerHTML.Replace("\r\n", "");
                    if (htmlelement.innerHTML != null)
                    {
                        _element.innerHTML = htmlelement.innerHTML;
                        _element.innerHTML = _element.innerHTML.Replace("\r\n", "");
                    }
                    else
                        _element.innerHTML = "";                    

                    if (_element.id == 0)
                    {
                        _element.elementlinked_id = -1;//root
                        savehtmlContent = _element.outerHTML;
                        resulthmtlContent = _element.outerHTML;
                    }
                    else
                        _element.elementlinked_id = 0;

                    if (htmlelement.tagName == "HTML")
                    {//html bazen geç geliyor...
                        savehtmlContent = _element.outerHTML;
                        resulthmtlContent = _element.outerHTML;
                    }

                    string _str = _element.outerHTML;
                    int _start = _str.IndexOf('<');
                    int _end = _str.IndexOf('>');
                    _element.elementName = _str.Substring(_start, _end - _start + 1);

                    _element.tagName = htmlelement.tagName;
                    _element.tag_id_Name = "";
                    _element.tag_class_Name = "";
                    if (htmlelement.id != null)
                    {
                        _element.tag_id = 1;
                        _element.tag_id_Name = htmlelement.id;
                    }

                    if (htmlelement.className != null)
                    {
                        _element.tag_class = 1;
                        _element.tag_class_Name = htmlelement.className;
                    }

                    if (_element.tag_id != 1 || _element.tag_class != 1)
                        _element.tag_idORclass = 1;

                    string tempinner_text = htmlelement.innerText;
                    if (tempinner_text != null)
                    {
                        tempinner_text = tempinner_text.Replace("\r\n", " ");
                        tempinner_text = tempinner_text.Trim();
                    }
                    else
                        tempinner_text = "";

                        element _tempelement = AnalyzeGivenHTML(htmlelement.outerHTML, tempinner_text);
                        _element.BagofWords = _tempelement.BagofWords;
                        _element.wordCount = _tempelement.wordCount;
                        _element.DensityinHTML = (double)_element.wordCount / _firstelement.wordCount;
                        _element.LinkCount = _tempelement.LinkCount;
                        _element.wordCountinLink = _tempelement.wordCountinLink;
                        _element.meanofWordinLinks = _tempelement.meanofWordinLinks;
                        _element.meanofWordinLinksAllWords = _tempelement.meanofWordinLinksAllWords;
                        string temp_innerhtml_ = _element.innerHTML.ToUpper(new CultureInfo("en-US", false));//for english words thus html tags
                        _element.dot_count = webfilter.CountStringOccurrences(temp_innerhtml_, ".");
                        _element.h1_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<H1");
                        _element.h2_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<H2");
                        _element.h3_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<H3");
                        _element.h4_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<H4");
                        _element.h5_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<H5");
                        _element.h6_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<H6");
                        _element.img_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<IMG");
                        _element.p_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<P");
                        _element.br_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<BR");
                        _element.span_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<SPAN");
                        _element.object_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<OBJECT");
                        _element.ul_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<UL");
                        _element.li_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<LI");
                        _element.input_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<INPUT")
                            + webfilter.CountStringOccurrences(temp_innerhtml_, "<BUTTON") 
                            + webfilter.CountStringOccurrences(temp_innerhtml_, "<LABEL");                    
                        _element.div_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<DIV");
                        _element.td_count = webfilter.CountStringOccurrences(temp_innerhtml_, "<TD");

                        _element.parent_elementName = "";

                        //sim control
                        //-1 : not available for sim control
                        //0  : similar
                        //0..1: similarity degree
                        //1  : not similar 
                        _element.sim_bagofword = -1;
                        _element.sim_bagofword_AE = -1;
                        _element.sim_innerHTML = -1;
                        _element.sim_innerHTML_AE = -1;
                    
                    
                    _list.Add(_element);

                    int key = (int)htmlelement.sourceIndex;//for fast searching
                    _ht.Add(key, i);
                    i++;
                }
            }

            foreach (IHTMLElement htmlelement in allElements)
            {
                if (htmlelement.outerHTML != null)
                {
                    string[] _sonuclar = ExtractionofSubLayouts(htmlelement);
                    string tempinner_text = _sonuclar[1];
                    string tempOuterHTML = _sonuclar[2];
                    string tempinnerHTML = _sonuclar[3];
                    string str_i = _sonuclar[0];

                    i = Convert.ToInt32(str_i);

                    //After Extraction
                    element _element = (element)_list[i];                    
                    element _tempelement = AnalyzeGivenHTML_AE(tempOuterHTML, tempinner_text);

                    if (_element.elementlinked_id > 0)
                    {
                        element _p_element = (element)_list[_element.elementlinked_id];
                        _element.parent_elementName = _p_element.elementName;
                    }

                    if (_element.tagName == "DIV" || _element.tagName == "TD" || _element.tagName == "UL"
                        || _element.tagName == "H1" || _element.tagName == "H2" || _element.tagName == "H3"
                        || _element.tagName == "H4" || _element.tagName == "H5" || _element.tagName == "H6"
                        || _element.tagName == "SPAN" || _element.tagName == "B" || _element.tagName == "STRONG"
                        || _element.tagName == "P")
                    {
                        _element.outerHTML_AE = tempOuterHTML;
                        _element.innerHTML_AE = tempinnerHTML;
                        _element.BagofWords_AE = _tempelement.BagofWords_AE;
                        _element.wordCount_AE = _tempelement.wordCount_AE;
                        _element.DensityinHTML_AE = (double)_element.wordCount_AE / _firstelement.wordCount;
                        _element.LinkCount_AE = _tempelement.LinkCount_AE;
                        _element.wordCountinLink_AE = _tempelement.wordCountinLink_AE;
                        _element.meanofWordinLinks_AE = _tempelement.meanofWordinLinks_AE;
                        _element.meanofWordinLinksAllWords_AE = _tempelement.meanofWordinLinksAllWords_AE;
                        string temp_innerhtml_AE = _element.innerHTML_AE.ToUpper(new CultureInfo("en-US", false));//for english words thus html tags
                        _element.dot_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, ".");
                        _element.h1_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<H1");
                        _element.h2_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<H2");
                        _element.h3_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<H3");
                        _element.h4_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<H4");
                        _element.h5_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<H5");
                        _element.h6_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<H6");
                        _element.img_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<IMG");
                        _element.p_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<P");
                        _element.br_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<BR");
                        _element.span_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<SPAN");
                        _element.object_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<OBJECT");
                        _element.ul_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<UL");
                        _element.li_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<LI");
                        _element.input_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<INPUT")
                            + webfilter.CountStringOccurrences(temp_innerhtml_AE, "<BUTTON")
                            + webfilter.CountStringOccurrences(temp_innerhtml_AE, "<LABEL");
                        _element.div_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<DIV");
                        _element.td_count_AE = webfilter.CountStringOccurrences(temp_innerhtml_AE, "<TD");

                        if (_element.wordCount_AE > _element.wordCount)
                            _element.wordCount_AE = _element.wordCount; //istisnayi durum scriptler sorun olduğu için nadir bir durum...
                    
                    //etiketin tekrar sayısı
                    //_element.repeat_tag_count = webfilter.CountStringOccurrences(htmlDocument.body.innerHTML, _element.elementName);
                        int benzertagsayisi = 0;
                        for (int k = 0; k < _list.Count; k++)
                        {
                            element _e1 = (element)_list[k];
                            if (_element.elementName == _e1.elementName)
                                benzertagsayisi++;
                        }
                        _element.repeat_tag_count = benzertagsayisi;
                    }
                        _list[i] = _element;
                }// if not null
            }//for each            

            return _list;
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

        public string[] ExtractionofSubLayouts(IHTMLElement htmlelement)
        {
            int key = (int)htmlelement.sourceIndex;
            int i = (int)_ht[key];

            string[] _sonuclar = new string[4];

            string tempOuterHTML = htmlelement.outerHTML;
            string tempinner_text = htmlelement.innerText;
            string tempinnerHTML = htmlelement.innerHTML;

            tempOuterHTML = tempOuterHTML.Replace("\r\n", "");

            if (tempinnerHTML != null)
            {
                tempinnerHTML = tempinnerHTML.Replace("\r\n", "");
            }
            else
                tempinnerHTML = "";


            if (tempinner_text != null)
            {
                tempinner_text = tempinner_text.Replace("\r\n", " ");
                tempinner_text = tempinner_text.Trim();
            }
            else
                tempinner_text = "";

            foreach (IHTMLElement htmlchild in (IHTMLElementCollection)htmlelement.children)
            {
                if (htmlchild.outerHTML != null)
                {
                    int keychild = (int)htmlchild.sourceIndex;
                    int ic = (int)_ht[keychild];
                    element _e = (element)_list[ic];
                    _e.elementlinked_id = i;
                    _list[ic] = _e;

                    if(_e.BagofWords != null)//bazı durumlar hiç hesaplanmayıor. bu durumları null gördüğü için
                    if (_e.tagName == "DIV"
                        || _e.tagName == "TABLE" || _e.tagName == "TBODY" || _e.tagName == "TR" || _e.tagName == "TD"
                        || _e.tagName == "FORM" || _e.tagName == "CENTER" || _e.tagName == "UL" | _e.tagName == "OL")//
                    {
                        if (tempOuterHTML != "")
                        {
                            //Clear child tags from bag of words
                            //Replace function clear all possible words so we write this algorithm
                            
                            tempinner_text = StripOnlyFirstData(tempinner_text, _e.BagofWords);
                            //Clear child tags from outer html
                            //tempOuterHTML = tempOuterHTML.Replace(_e.outerHTML, "");                                    
                            tempOuterHTML = StripOnlyFirstData(tempOuterHTML, _e.outerHTML);
                            tempinnerHTML = StripOnlyFirstData(tempinnerHTML, _e.innerHTML);
                        }//
                    }//IF DIV TABLE ...
                }
            }//childrens

            _sonuclar[0] = i.ToString();
            _sonuclar[1] = tempinner_text;
            _sonuclar[2] = tempOuterHTML;
            _sonuclar[3] = tempinnerHTML;

            return _sonuclar;
        }

        public ArrayList FindChilds(ArrayList _list, int key)
        {
            ArrayList _listchild = new ArrayList();

            foreach (element d in _list)
            {                
                if (d.elementlinked_id == key)
                    if(d.outerHTML != null)
                        _listchild.Add(d);               
            }

            return _listchild;
        }
        
        //prepare information for a given hmtl
        public element AnalyzeGivenHTML(string html_content, string inner_text)
        {
            //html_content = RemoveScripts(html_content);
            element _element = new element();
            _element.BagofWords = inner_text;
            _element.wordCount = HTML.WordsCountGivenText(_element.BagofWords);
      

            string pattern = "href=.*?>(.*?)</a";
            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matchList = exp.Matches(html_content);
            string[] _list = new string[matchList.Count];
            string URL_INNER = "";
            for (int i = 0; i < matchList.Count; i++)
            {
                Match match = matchList[i];
                if (match.Value.Length > 0)
                {                    
                    URL_INNER = URL_INNER + " " + HTML.stripHtml(match.Groups[1].Value);
                }
            }

            int count_link = webfilter.CountStringOccurrences(html_content, "<a ");
            count_link = count_link + webfilter.CountStringOccurrences(html_content, "<A ");
            count_link = count_link + webfilter.CountStringOccurrences(html_content, "onclick="); //interesting javascript with link

            _element.LinkCount = count_link;
            _element.wordCountinLink = HTML.WordsCountGivenText(URL_INNER);
            if (_element.LinkCount != 0)
                _element.meanofWordinLinks = (double)_element.wordCountinLink / _element.LinkCount;
            else
                _element.meanofWordinLinks = 0;

            if (_element.wordCount != 0)
                _element.meanofWordinLinksAllWords = (double)_element.wordCountinLink / _element.wordCount;
            else
                _element.meanofWordinLinksAllWords = 0;

            return _element;
        }

        //prepare information for a given hmtl
        public element AnalyzeGivenHTML_AE(string html_content, string inner_text)
        {
            //html_content = RemoveScripts(html_content);
            element _element = new element();
            _element.BagofWords_AE = inner_text;
            _element.wordCount_AE = HTML.WordsCountGivenText(_element.BagofWords_AE);

            string pattern = "href=.*?>(.*?)</a";
            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matchList = exp.Matches(html_content);
            string[] _list = new string[matchList.Count];
            string URL_INNER = "";
            for (int i = 0; i < matchList.Count; i++)
            {
                Match match = matchList[i];
                if (match.Value.Length > 0)
                {
                    URL_INNER = URL_INNER + " " + HTML.stripHtml(match.Groups[1].Value);
                }
            }

            _element.LinkCount_AE = matchList.Count;
            _element.wordCountinLink_AE = HTML.WordsCountGivenText(URL_INNER);
            if (_element.LinkCount_AE != 0)
                _element.meanofWordinLinks_AE = (double)_element.wordCountinLink_AE / _element.LinkCount_AE;
            else
                _element.meanofWordinLinks_AE = 0;

            if (_element.wordCount_AE != 0)
                _element.meanofWordinLinksAllWords_AE = (double)_element.wordCountinLink_AE / _element.wordCount_AE;
            else
                _element.meanofWordinLinksAllWords = 0;

            return _element;
        }

        private string StripOnlyFirstData(string _htmlcontent, string extracted_data) 
        {
            int pos = _htmlcontent.IndexOf(extracted_data);

            if (pos >= 0)
            {
                string baslangic = "";
                string son = "";
                if (pos != 0)
                {
                    baslangic = _htmlcontent.Substring(0, pos);
                }

                if (extracted_data.Length + pos <= _htmlcontent.Length)
                    son = _htmlcontent.Substring(extracted_data.Length + pos, _htmlcontent.Length - extracted_data.Length - pos);

              _htmlcontent = baslangic + " " + son;
            }

            return _htmlcontent;
        }


        public ArrayList fingTAGibHTML(String htmlContent, String tagName, string filename)
        {
            DateTime _now = DateTime.Now;

            string id_name = "";
            if (tagName.Contains("id="))
                id_name = findElementName(tagName, "id=\"(.*?)\"");
            if (tagName.Contains("ID="))
                id_name = findElementName(tagName, "ID=\"(.*?)\"");
            string class_name = "";
            if (tagName.Contains("class="))
                class_name = findElementName(tagName, "class=\"(.*?)\"");

            // Obtain the document interface
            //IHTMLDocument2 htmlDocument = (IHTMLDocument2)new mshtml.HTMLDocument();
            IHTMLDocument2 htmlDocument = new mshtml.HTMLDocumentClass();
            // Construct the document
            htmlDocument.write(htmlContent);
            // Extract all image elements
            // IHTMLElementCollection imgElements = htmlDocument.images;
            IHTMLElementCollection allElements = htmlDocument.all;
            ArrayList sonuc = new ArrayList();
            // Iterate all the elements and display tag names

            int elementsize = 0;
            int elementcnt = 0;
            foreach (IHTMLElement element in allElements)
            {
                string cn = "";
                if (element.className != null)
                    cn = element.className;
                string id = "";
                if (element.id != null)
                    id = element.id;

                if (element.tagName == "DIV" && cn == class_name && id == id_name)
                    sonuc.Add(element.innerText);

                if (element.innerHTML != null)
                    elementsize += element.innerHTML.Length;


                elementcnt++;
            }

            return sonuc;
        }

        //find patterns in html
        private string findElementName(string tagname, string pattern)
        {
            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matchList = exp.Matches(tagname);

            Match match = matchList[0];
            string _str = match.Groups[1].Value;

            return _str;
        }
        
    }//class DOM

    //HTML Processing
    public class HTML
    {
        //Words count in given text
        public static int WordsCountGivenText(string words)
        {
            words = words.Replace(":", " ");
            words = words.Replace("(", " ");
            words = words.Replace(")", " ");
            words = words.Replace("?", " ");
            words = words.Replace("*", " ");
            words = words.Replace("-", " ");
            words = words.Replace("/", " ");
            words = words.Replace("!", " ");
            words = words.Replace(".", " ");
            // COMPRESS ALL WHITESPACE into a single space, seperating words
            if (words != null)
            {
                if (words.Length > 0)
                {
                    Regex r = new Regex(@"\s+");            //remove all whitespace
                    string compressed = r.Replace(words, " ");
                    return compressed.Split(' ').Length;
                }
                else
                {
                    return 0;
                }
            }
            else
                return 0;
        }
        //trim javascript
        public static string trimScript(string htmlDocText)
        {
            string bodyText = "";

            //garip bir durum. javascript içinde javascript var write komutu içinde... o yüzden bu satır...
            string trimJavascript = "document.write(.*?)";
            Regex regexTrimJs = new Regex(trimJavascript, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            bodyText = regexTrimJs.Replace(htmlDocText, "");

            trimJavascript = @"<SCR.PT(?:\s+[^>]*)?>.*?</SCR.PT\s*>";
            regexTrimJs = new Regex(trimJavascript, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            bodyText = regexTrimJs.Replace(htmlDocText, "");
            return bodyText;
        }
        //trim javascript
        public static string trim_HREF_SCR(string htmlDocText)
        {
            string bodyText = "";
            string trim_str = "href=\".*?\"";
            Regex regexTrimJs = new Regex(trim_str, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            bodyText = regexTrimJs.Replace(htmlDocText, "href=\"\"");

            trim_str = "src=\".*?\"";
            regexTrimJs = new Regex(trim_str, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            bodyText = regexTrimJs.Replace(bodyText, "src=\"\"");

            return bodyText;
        }

        public static string trim_commenttags(string htmlDocText)
        {
            string trim_str = "<!--.*?-->";
            Regex regexTrimJs = new Regex(trim_str, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            htmlDocText = regexTrimJs.Replace(htmlDocText, "");

            return htmlDocText;
        }

                    

        //trim javascript
        public static string trim_some_cases(string htmlDocText)
        {
            string bodyText = "";
            string trim_str = "onload=\".*?\"";
            Regex regexTrimJs = new Regex(trim_str, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            bodyText = regexTrimJs.Replace(htmlDocText, "");

            trim_str = "onerror=\".*?\"";
            regexTrimJs = new Regex(trim_str, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            bodyText = regexTrimJs.Replace(bodyText, "");

            trim_str = @"<.FRAME(?:\s+[^>]*)?>.*?<.*?/.FRAME\s*>";
            regexTrimJs = new Regex(trim_str, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            bodyText = regexTrimJs.Replace(bodyText, "");

            //walesonline.co.uknews özel bir durum
            trim_str = "<tm:contentobject.*?>";
            regexTrimJs = new Regex(trim_str, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            bodyText = regexTrimJs.Replace(bodyText, "");

            trim_str = "<.tm:contentobject.*?>";
            regexTrimJs = new Regex(trim_str, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            bodyText = regexTrimJs.Replace(bodyText, "");

            return bodyText;
        }

        //trim td
        public static string trimTD(string htmlDocText)
        {
            //nested 
            ArrayList _divlist = findgiventag(htmlDocText, "<TD.*?>");
            foreach (string item in _divlist)
            {
                string tag = item;
                tag = tag.Replace(":", ".");
                tag = tag.Replace("(", ".");
                tag = tag.Replace(")", ".");
                tag = tag.Replace("?", ".");
                tag = tag.Replace("*", ".");
                tag = tag.Replace("-", ".");
                tag = tag.Replace("/", ".");
                tag = tag.Replace("!", ".");
                tag = tag.Replace(" ", ".?");
                tag = tag.Replace(";", ".");
                if (htmlDocText.Contains(item))
                {
                    string[] _divstr = webfilter.Contents_of_givenLayout_Tags_TESTER(htmlDocText, tag, false);
                    if (_divstr != null)
                        foreach (string _d in _divstr)
                        {
                            if(_d != null)
                                if(_d != "")
                            htmlDocText = htmlDocText.Replace(_d, "");
                        }
                }

            }

            return htmlDocText;
        } 
        //trim javascript
        public static string trimDIV(string htmlDocText)
        {
            //nested 
            ArrayList _divlist = findgiventag(htmlDocText, "<D[I,i]V.*?>");
            foreach (string item in _divlist)
            {
               string tag = item;
               tag = tag.Replace(":", ".");
               tag = tag.Replace("(", ".");
               tag = tag.Replace(")", ".");
               tag = tag.Replace("?", ".");
               tag = tag.Replace("*", ".");
               tag = tag.Replace("-", ".");
               tag = tag.Replace("/", ".");
               tag = tag.Replace("!", ".");
               tag = tag.Replace(" ", ".?");
               tag = tag.Replace(";", ".");
               if (htmlDocText.Contains(item))
               {
                   string[] _divstr = webfilter.Contents_of_givenLayout_Tags_TESTER(htmlDocText, tag, false);
                   if (_divstr != null)
                       foreach (string _d in _divstr)
                       {
                           if (_d != null)
                               if (_d != "")
                           htmlDocText = htmlDocText.Replace(_d,"");
                       }
               }
                                
            }

            return htmlDocText;
        }

        public static ArrayList findgiventag(string html_content, string pattern)
        {
            ArrayList _al = new ArrayList();
            if (html_content != null)
            {
                Regex exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

                MatchCollection matchList = exp.Matches(html_content);

                for (int i = 0; i < matchList.Count; i++)
                {
                    Match match = matchList[i];
                    string _str = match.Groups[0].Value;
                    _al.Add(_str);
                }

            }
            return _al;
        }
        //trim options because it is negative effects on calculation of count
        public static string trimOptions(string words)
        {
            // COMPRESS ALL WHITESPACE into a single space, seperating words
            if (words != null)
            {
                if (words.Length > 0)
                {
                    Regex r = new Regex(@"<STYLE(?:\s+[^>]*)?>.*?</STYLE\s*>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);                 //style
                    string compressed = r.Replace(words, " ");
                    r = new Regex(@"<select(?:\s+[^>]*)?>.*?</select\s*>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);                 //select
                    compressed = r.Replace(compressed, " ");
                    return compressed;
                }
                else
                {
                    return "";
                }
            }
            else
                return "";
        }
        //remove html tags
        public static string stripHtml(string strOutput)
        {
            //Strips the HTML tags from strHTML
            if (strOutput != null)
            {
                /*if (strOutput.Contains("<script") || strOutput.Contains("<SCRIPT") || strOutput.Contains("<Script"))
                    strOutput = "xxx";*/
                strOutput = trimScript(strOutput);
                strOutput = trimDIV(strOutput);
                strOutput = trimTD(strOutput);

                System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex("<(.|\n)+?>");
                strOutput = _regex.Replace(strOutput, " ");
                strOutput = clear_illegal_characters_for_HTML(strOutput);
            }
            return strOutput;
        }
        //clear illegal characters
        public static string clear_illegal_characters_for_HTML(string strOutput)
        {
            strOutput = strOutput.Replace("&quot;", " ");
            strOutput = strOutput.Replace("&#39;", " ");
            strOutput = strOutput.Replace("\n", " ");
            strOutput = strOutput.Replace("\r", " ");
            strOutput = strOutput.Replace("\t", " ");
            strOutput = strOutput.Replace("&nbsp;", " ");
            strOutput = strOutput.Replace("\"", " ");
            strOutput = strOutput.Replace("\\", " ");
            strOutput = strOutput.Replace("`", "");
            strOutput = strOutput.Replace("’", "");
            strOutput = strOutput.Replace("<", " ");
            strOutput = strOutput.Replace(">", " ");
            strOutput = strOutput.Replace("|", " ");
            strOutput = strOutput.Replace("'", "");
            strOutput = strOutput.Replace(",", " ");
            strOutput = strOutput.Replace("?", " ");
            strOutput = strOutput.Replace("!", " ");
            strOutput = strOutput.Replace(".", " ");
            strOutput = strOutput.Replace("*", " ");
            strOutput = strOutput.Replace("-", " ");
            strOutput = strOutput.Replace("•", " ");
            strOutput = strOutput.Replace(":", " ");
            strOutput = strOutput.Replace("/", " ");
            strOutput = strOutput.Replace(";", " ");
            strOutput = strOutput.Replace("#", " ");
            strOutput = strOutput.Replace("(", " ");
            strOutput = strOutput.Replace(")", " ");
            strOutput = strOutput.Replace("$", " ");
            strOutput = strOutput.Replace("%", " ");
            strOutput = strOutput.Replace("&", " ");
            strOutput = strOutput.Replace("{", " ");
            strOutput = strOutput.Replace("}", " ");
            strOutput = strOutput.Replace("=", " ");
            strOutput = strOutput.Replace("]", " ");
            strOutput = strOutput.Replace("[", " ");
            strOutput = strOutput.Replace("*", " ");
            strOutput = strOutput.Replace("_", " ");
            strOutput = strOutput.Replace("-", " ");
            strOutput = strOutput.Replace("£", " ");
            strOutput = strOutput.Replace("é", " ");
            strOutput = strOutput.Replace("½", " ");
            strOutput = strOutput.Replace("~", " ");
            strOutput = strOutput.Replace("“", " ");
            strOutput = strOutput.Replace("»", " ");
            strOutput = strOutput.Replace("+", " ");
            strOutput = strOutput.Replace("‘", " ");
            strOutput = strOutput.Replace("@", " ");

            Regex _regex = new Regex(@"\s+", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

            strOutput = _regex.Replace(strOutput, " ");

            return strOutput;
        }
    }//html class

    //efficient content extractiom module
    public class webfilter
    {
        //word count for a given word in a given text
        private static int CountofStartingTag(string Text, string tag)
        {
            int words;
            Regex reg = new Regex(@tag);
            MatchCollection mc = reg.Matches(Text);
            if (mc.Count > 0)
                words = mc.Count;
            else
                words = 0;

            return words;
        }

        //end tag for a given tag
        private static string find_EndTag(string _tag)
        {
            int end = _tag.IndexOf(" ");
            if(end==-1)
                end = _tag.IndexOf(">");

            string _result = _tag;
            if (end != -1)
            {
                _result = _tag.Substring(0, end) + ">";
                _result = _result.Replace("<", "</");
            }

            return _result;
        }

        //end tag for a given tag
        private static string find_StartTag(string _tag)
        {
            int end = _tag.IndexOf(" ");
            if (end == -1)
                end = _tag.IndexOf(">");

            string _result = _tag;
            if (end != -1)
            {
                _result = _tag.Substring(0, end);
            }

            return _result;
        }

        //Tag'a ait bilgileri getiren fonksiyon
        //birden fazla sonuç varsa gösterebiliyor
        //nested özelliği regular expression sağlanamıyor. Bu fonksiyon sayesinde nested tapıda çözümleniyor.
        private static string[] GrabbingofHTMLTags(string _text, string _tag, int countofTag)
        {
            string[] _resultArray = new string[countofTag];

            string _endtag = find_EndTag(_tag);
            string _starttag = find_StartTag(_tag);

            int k = 0;
            for (int i = 0; i < countofTag; i++)
            {
                k = _text.IndexOf(_tag, k);
                if (k == -1) break;
                string str1 = "";
                if (k != -1)
                {
                    string str2 = _text.Substring(k);
                    str1 = _text.Substring(k);

                    k = k + _tag.Length;
                    //div içini ararken en son nerede kaldığımızı tutan etiket.
                    int l = str2.IndexOf(_endtag);
                    if (l != -1)
                        str1 = str2.Substring(0, l + _endtag.Length);
                    else
                        str1 = str2;

                    int start_position = l + _endtag.Length;
                    while (CountofStartingTag(str1, _starttag) != CountofStartingTag(str1, _endtag))
                    {
                        l = str2.IndexOf(_endtag, start_position);

                        if (l == -1)
                            break;

                        str1 = str2.Substring(0, l + _endtag.Length);

                        start_position = l + _endtag.Length;
                    }
                }//if 1
                else
                    str1 = "";

                //extract only content
                if (str1 != "")
                {
                    if (str1.Length - _tag.Length > 0)
                    {
                        str1 = str1.Substring(_tag.Length, str1.Length - _tag.Length - _endtag.Length);
                        str1 = str1.Trim();
                        _resultArray[i] = str1;
                    }
                }
            }//for

            return _resultArray;
        }//end function

        //find patterns in html
        public static Hashtable filteringHTMLtags(string html_content, string pattern, Hashtable _tags)
        {
            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matchList = exp.Matches(html_content);

            for (int i = 0; i < matchList.Count; i++)
            {
                Match match = matchList[i];
                string _str = match.Groups[0].Value;
                if (_str.Length > pattern.Length - 3)
                {
                    if (!_tags.ContainsKey(_str))
                        _tags.Add(_str, 1);
                    else
                        _tags[_str] = (int)_tags[_str] + 1;
                }
            }

            return _tags;
        }

        //find patterns in html
        private static Hashtable filteringHTMLtags_TESTER(string html_content, string pattern, Hashtable _tags)
        {
            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matchList = exp.Matches(html_content);

            for (int i = 0; i < matchList.Count; i++)
            {
                Match match = matchList[i];

                if (!_tags.ContainsKey(matchList[i].ToString()))
                    _tags.Add(matchList[i].ToString(), 1);
                else
                    _tags[matchList[i].ToString()] = (int)_tags[matchList[i].ToString()] + 1;
            }

            return _tags;
        }

        //for a given tag
        private static Hashtable filtergivenHTMLtag_TESTER(string html_content, string pattern)
        {
            Hashtable _tags = new Hashtable();

            _tags = filteringHTMLtags_TESTER(html_content, pattern, _tags);

            return _tags;
        }

        //for test finding operation
        public static string[] Contents_of_givenLayout_Tags_TESTER(string html_content, string pattern, bool cut_sub_blocks)
        {
            Hashtable _tags_in_HTML = filtergivenHTMLtag_TESTER(html_content, pattern);
            string[] _content = null;

            int elementsize = 0;
            foreach (DictionaryEntry d in _tags_in_HTML)
            {
                string _tag = (string)d.Key;
                int _cnt = (int)d.Value;

                _content = GrabbingofHTMLTags(html_content, _tag, _cnt);
                string temp = "";
                for (int i = 0; i < _content.Length; i++)
                {
                    string t_content = _content[i];
                    if (cut_sub_blocks)
                    {
                        t_content = HTML.trimDIV(t_content);
                        t_content = HTML.trimTD(t_content);
                        _content[i] = t_content;
                    }

                    temp = temp + t_content;
                }

                elementsize = elementsize + temp.Length;
            }   

            return _content;
        }

        public static int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }
    }//html class
}//namespace
