using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace HTMLMarkerClass
{
    //key part of hashtable
    /*public class rule_part
    {
        public string _tag;
        public string _parent_tag;
    }*/
    //value part of hashtable
    public class rule_information
    {
        public string _Classname;
        public string _tag;
        public string _parent_tag;
        public int _count;
        public string _htmlText;
        public bool _repetive;

        public rule_information(string _c, string _t, string _p, string _txt, int _cnt, bool _rep)
        {
            _Classname = _c;
            _tag = _t;
            _parent_tag = _p;
            _count = _cnt;
            _htmlText = _txt;
            _repetive = _rep;
        }
    }

    //automatically obtained rules
    public class ML_Rules
    {
        public Hashtable _ht_rules;

        private string[] _CN = new string[] { "main", "additional", "headline", "summary", "links", "menu", "empty" };

        public ML_Rules()
        {
            _ht_rules = new Hashtable();
        }

        public ML_Rules(ArrayList _list, Hashtable old_ht)
        {
            if (old_ht == null)
                _ht_rules = new Hashtable();
            else
                _ht_rules = old_ht;

            prepareRules(_list);
        }

        private void add_rule(string key, rule_information _ri)
        {
            if (HTML.stripHtml(_ri._htmlText) != "")
            {
                if (!_ht_rules.ContainsKey(key))
                {
                    _ht_rules.Add(key, _ri); //new rule
                }
                else //rule ok
                {
                    rule_information _ri_temp = (rule_information)_ht_rules[key];

                    if (_ri_temp._Classname == _ri._Classname)
                    {//prediction ok
                        _ri_temp._count++;
                        if (HTML.stripHtml(_ri._htmlText).Trim() != "")
                        {
                            if (_ri_temp._htmlText == _ri._htmlText)
                            {
                                _ri_temp._repetive = true;
                                _ht_rules[key] = _ri_temp;
                            }
                            else
                            {
                                _ri_temp._repetive = true;
                                _ht_rules[key] = _ri_temp;
                            }
                        }
                        else
                        {
                            _ri_temp._htmlText = _ri._htmlText;
                            _ht_rules[key] = _ri_temp;
                        }
                    }
                    else
                    {//prediction error
                        if (_ri_temp._count < 3)//maybe mistake so delete
                        {
                            _ht_rules.Remove(key);
                        }
                    }
                } //rule ok else
            }
        }

        private void prepareRules(ArrayList _list)
        {
            if (_list != null)
            {
                HTMLMarkerClass.desicionClass._list = _list; //_list gönder

                for (int i = 0; i < _list.Count; i++)
                {
                    HTMLMarkerClass.element _element = (HTMLMarkerClass.element)_list[i];
                    if(_element.wordCount_AE > 2)
                    foreach (string item_cn in _CN)
                    {
                        string i_cn = item_cn;
                        if (i_cn == "additional")
                            i_cn = "others";

                        if (HTMLMarkerClass.desicionClass.write_or_not(i_cn, _element))
                        {
                            string _parent_tag;
                            if (_element.elementlinked_id != -1)
                            {
                                HTMLMarkerClass.element _parent_element = (HTMLMarkerClass.element)_list[_element.elementlinked_id];
                                _parent_tag = _parent_element.elementName;
                            }
                            else
                                _parent_tag = "";

                            string key = _element.elementName + ", " + _parent_tag;
                            /*rule_part _rp = new rule_part();
                            _rp._tag = ;
                            _rp._parent_tag = _parent_tag;*/
                            rule_information _ri = new rule_information(item_cn, _element.elementName, _parent_tag, _element.innerHTML_AE, 1, false);
                            add_rule(key, _ri);
                        }//yaz = true
                    } //for each
                }//for i
            } 
        }
    }

    //manual rules in files
    public class file_Rules 
    {
        public Hashtable _ht_rules;

        public file_Rules(string _dir, string filename)
        {
            xmlfile._destdir = _dir;
            if (filename == "")
                xmlfile.Read_XMLFile(); //read template.xml file in _dir
            else
                xmlfile.Read_XMLFile(_dir + filename);
            //results in xmlfile._xml_list, now read this arraylist
            _ht_rules = new Hashtable();
            foreach (string line in xmlfile._xml_list)
            {
                string tagName = "<" + xmlfile.findElementName(line, "tagname=\"<(.*?)>\"") + ">";
                string parent_tagName = "<" + xmlfile.findElementName(line, "parent_tagname=\"<(.*?)>\"") + ">";
                string className = xmlfile.find_ClassName(line);
                if (className == "others")
                    className = "additional";

                /*rule_part _rp = new rule_part();
                _rp._tag = tagName;
                _rp._parent_tag = parent_tagName;*/
                string key = tagName + ", " + parent_tagName;
                rule_information _ri = new rule_information(className, tagName, parent_tagName, "", 100, false);
                _ht_rules.Add(key, _ri);
            }
        }      
    }

    //for tests
    public static class Rules_Process
    {
        public static string[] _CN = new string[] { "main", "additional", "headline", "summary", "links", "menu", "empty" };
        public static int[] _CN_count_correct = new int[7];
        public static int[] _CN_count_false = new int[7];

        public static void Compare_Two_Ht(Hashtable _ht01, Hashtable _ht02, ArrayList _list)
        {
            for (int i = 0; i < _CN_count_correct.Length; i++)
            {
                _CN_count_correct[i] = 0;
                _CN_count_false[i] = 0;
            }

            foreach (DictionaryEntry d2 in _ht02)
            {
                rule_information _ri2 = (rule_information)d2.Value;
                string _cn = _ri2._Classname;
                int _id = id_CN(_cn);

                bool rule_var = false;
                bool element_predict = false;               
                
                for (int i = 0; i < _list.Count; i++)
                {
                    HTMLMarkerClass.element _element = (HTMLMarkerClass.element)_list[i];
                    if(_element.elementName == _ri2._tag && _element.parent_elementName == _ri2._parent_tag && _element.wordCount_AE > 2)
                    {
                        rule_var = true;
                        foreach (DictionaryEntry d1 in _ht01)
                        {
                            rule_information _ri1 = (rule_information)d1.Value;
                            if (_ri1._tag == _ri2._tag && _ri1._parent_tag == _ri2._parent_tag)
                            {              
                                if (_ri1._Classname == _cn)
                                {
                                    if (_id != -1)
                                        _CN_count_correct[_id]++;
                                }
                                else
                                    if (_id != -1)
                                        _CN_count_false[_id]++;//error 1

                                element_predict = true;
                                break;
                            }
                        }//foreach 2   
                        if (rule_var)
                            break;
                    }
                }//for

                //rule in list but no prediction
                if(rule_var && !element_predict)
                if (_id != -1)
                    _CN_count_false[_id]++;//error 2
            }//foreach 1*/
        }

        public static void Save_Rules(Hashtable _ht, string file) 
        {
            StreamWriter sw = File.CreateText(file);
            foreach (DictionaryEntry d in _ht)
            {
                //rule_part _rp = (rule_part)d.Key;
                rule_information _ri = (rule_information)d.Value;
                //<menu tagname="<DIV class="cabeceratop clearfix">" parent_tagname="<DIV class=headertop>" />
                sw.WriteLine("<" + _ri._Classname + " tagname=\"" + _ri._tag + "\" parent_tagname=\"" + _ri._parent_tag + "\" html_text=\"<" + _ri._htmlText + ">\" _count=\"<" + _ri._count + ">\"/>");
            }    
            sw.Close();
        }

        public static Hashtable Load_Rules(string filename)
        {
            Hashtable _ht = new Hashtable();
            if (File.Exists(filename))
            {
                StreamReader file = new StreamReader(filename);
                string line = "";
                while ((line = file.ReadLine()) != null)
                {
                    string tagName = "<" + xmlfile.findElementName(line, "tagname=\"<(.*?)>\"") + ">";
                    string parent_tagName = "<" + xmlfile.findElementName(line, "parent_tagname=\"<(.*?)>\"") + ">";
                    string html_text= "<" + xmlfile.findElementName(line, "html_text=\"<(.*?)>\"") + ">";
                    string count = "<" + xmlfile.findElementName(line, "_count=\"<(.*?)>\"") + ">";
                    string className = xmlfile.find_ClassName(line);
                    string key = tagName + ", " + parent_tagName;
                    if (className == "others")
                        className = "additional";

                    rule_information _ri = new rule_information(className, tagName, parent_tagName, html_text, 1, false);
                    _ht.Add(key, _ri);
                }
                file.Close();
            }
            return _ht;
        }

        private static int id_CN(string _ClassName)
        {
            for (int i = 0; i < _CN.Length; i++)
            {                
                if (_CN[i] == _ClassName)
                    return i;
            }
            return -1;
        }

        private static void RemovefromList(ref ArrayList _list, string inner_html)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (HTML.stripHtml((string)_list[i]) == HTML.stripHtml(inner_html))
                {
                    _list.RemoveAt(i);
                    i--;
                }
                
            }
        }

        public static void RemoveRepetativeParts(ref ArrayList _main_list, ref ArrayList _headline_list, ref ArrayList _summary_list, ref ArrayList _additional_list, Hashtable _ht)
        {
            foreach (DictionaryEntry d in _ht)
            {
                rule_information _ri = (rule_information)d.Value;
                if(_ri._Classname == "main")
                    RemovefromList(ref _main_list, _ri._htmlText);
                if (_ri._Classname == "headline")
                    RemovefromList(ref _headline_list, _ri._htmlText);
                if (_ri._Classname == "summary")
                    RemovefromList(ref _summary_list, _ri._htmlText);
                if (_ri._Classname == "additional")
                    RemovefromList(ref _additional_list, _ri._htmlText);
            }
        }
    }

    //extraction extraction with a given rules (ML_Rules or file_rules)
    public static class efficientextraction
    {
        private static string[] _CN = new string[] { "main", "additional", "headline", "summary", "links", "menu", "empty" };
        //
        public static ArrayList extractRules(Hashtable _ht_rules, string awebpage, string _r_className)
        {
            ArrayList _al = new ArrayList();
            awebpage = uppercaseonlytags(awebpage);
            awebpage = HTML.trim_commenttags(awebpage);
            awebpage = HTML.trimScript(awebpage);            
            awebpage = HTML.trim_some_cases(awebpage);

            foreach (DictionaryEntry a_rule in _ht_rules)
            {
                rule_information _ri = (rule_information)a_rule.Value;
                if (_ri._Classname == _r_className)
                {
                    string a_pattern_for_parent_tag = prepare_a_pattern(_ri._parent_tag);
                    string[] parentcont = HTMLMarkerClass.webfilter.Contents_of_givenLayout_Tags_TESTER(awebpage, a_pattern_for_parent_tag, false);

                    if((parentcont == null && !(_ri._parent_tag.Contains("<DIV") || _ri._parent_tag.Contains("<div")))
                        || (parentcont == null && !(_ri._parent_tag.Contains("<TD") || _ri._parent_tag.Contains("<td")))
                        || (parentcont != null && (_ri._parent_tag.Contains("<tr") || _ri._parent_tag.Contains("<TR"))))
                    {
                        parentcont = new string[1];
                        parentcont[0] = awebpage;
                    }

                    if (parentcont != null)
                    foreach (string _str in parentcont)
                    {
                        string a_pattern_for_tag = prepare_a_pattern(_ri._tag);
                        string[] tagcont = HTMLMarkerClass.webfilter.Contents_of_givenLayout_Tags_TESTER(_str, a_pattern_for_tag, false);

                        if (tagcont != null)
                            foreach (string item in tagcont)
                            {
                                if(item != null)
                                    if (HTML.stripHtml(item).Trim() != "" && _ri._htmlText != item)
                                        if(!same_content(_al, item))
                                        _al.Add(item);
                            }
                    }                    
                }
            }
            
            return _al;
        }

        private static bool same_content(ArrayList _al, string _content)
        {
            foreach (string item in _al)
            {
                if (item == _content)
                    return true;
            }
            return false;
        }

        private static string uppercaseonlytags(string awebpage)
        {
            awebpage = HTML.trimOptions(awebpage);
            awebpage = HTML.trimScript(awebpage);
            awebpage = awebpage.Replace("\r", " ");
            awebpage = awebpage.Replace("\n", " ");
            awebpage = awebpage.Replace("\t", " ");
            awebpage = awebpage.Replace("style=\"\"", "");
            awebpage = awebpage.Replace("  ", " ");//whitespace problem in regex so...
            awebpage = awebpage.Replace("   ", " ");
            awebpage = awebpage.Replace("  ", " ");
            //uppercase problem turkish encoding???
            //starting tags
            awebpage = awebpage.Replace("<div", "<DIV");
            awebpage = awebpage.Replace("<td", "<TD");
            awebpage = awebpage.Replace("<h1", "<H1");
            awebpage = awebpage.Replace("<h2", "<H2");
            awebpage = awebpage.Replace("<h3", "<H3");
            awebpage = awebpage.Replace("<h4", "<H4");
            awebpage = awebpage.Replace("<h5", "<H5");
            awebpage = awebpage.Replace("<h6", "<H6");
            awebpage = awebpage.Replace("<span", "<SPAN");
            awebpage = awebpage.Replace("<font", "<FONT");
            awebpage = awebpage.Replace("<ul", "<UL");
            awebpage = awebpage.Replace("<li", "<LI");
            awebpage = awebpage.Replace("<b", "<b");
            awebpage = awebpage.Replace("<object", "<OBJECT");
            awebpage = awebpage.Replace("<button", "<BUTTON");
            awebpage = awebpage.Replace("<input", "<INPUT");
            awebpage = awebpage.Replace("<img", "<IMG");
            awebpage = awebpage.Replace("<br", "<BR");

            //ending tags
            awebpage = awebpage.Replace("</div", "</DIV");
            awebpage = awebpage.Replace("</td", "</TD");
            awebpage = awebpage.Replace("</h1", "</H1");
            awebpage = awebpage.Replace("</h2", "</H2");
            awebpage = awebpage.Replace("</h3", "</H3");
            awebpage = awebpage.Replace("</h4", "</H4");
            awebpage = awebpage.Replace("</h5", "</H5");
            awebpage = awebpage.Replace("</h6", "</H6");
            awebpage = awebpage.Replace("</span", "</SPAN");
            awebpage = awebpage.Replace("</font", "</FONT");
            awebpage = awebpage.Replace("</ul", "</UL");
            awebpage = awebpage.Replace("</li", "</LI");
            awebpage = awebpage.Replace("</b", "</b");
            awebpage = awebpage.Replace("</object", "</OBJECT");
            awebpage = awebpage.Replace("</button", "</BUTTON");
            awebpage = awebpage.Replace("</input", "</INPUT");
            awebpage = awebpage.Replace("</img", "</IMG");
            awebpage = awebpage.Replace("</br", "</BR");

            return awebpage;
        }

        public static string prepare_a_pattern(string tag)
        {
            tag = uppercaseonlytags(tag);
            string _pattern = "";
            if (tag.Contains(" "))
            {
                string tagName = tag.Substring(1, tag.IndexOf(" ") - 1);
                string[] _temp = findAttritubes(tag);
                string[] _sonuc = new string[1];
                if(_temp.Length>1)
                    _sonuc = new string[_temp.Length];

                int i = 0;
                int degiskensayi = 0;
                foreach (string _t in _temp)
                {
                    if (tag.Contains(_t))
                    {
                        if (tag.Contains(_t + "=\""))
                        {
                            string _tt = _t + "=\"";
                            int start = tag.IndexOf(_tt) + _tt.Length;
                            int end = tag.IndexOf("\"", start);
                            try
                            {
                                _sonuc[i] = tag.Substring(start, end - start);
                                _sonuc[i] = regex_rules(_sonuc[i]);
                            }
                            catch (Exception)
                            {
                                _sonuc[i] = "";//ender durum
                            }
                        }
                        else
                        {
                            string _tt = _t + "=";
                            int start = tag.IndexOf(_tt) + _tt.Length;
                            int end = tag.IndexOf(" ", start);
                            if (end == -1)
                                end = tag.IndexOf(">", start);
                            try
                            {
                                _sonuc[i] = tag.Substring(start, end - start);
                                _sonuc[i] = regex_rules(_sonuc[i]);
                            }
                            catch (Exception)
                            {
                                _sonuc[i] = "";
                            }
                        }
                        if(_sonuc[i]!="")
                        _sonuc[i] = _temp[i] + "=.?.?" + _sonuc[i] + ".?.?";
                        degiskensayi++;
                    }
                    i++;
                }

                if (degiskensayi == 0)
                    _pattern = regex_rules(tag);
                if (degiskensayi == 1)
                {
                    for (int m = 0; m < _temp.Length; m++)
                        if (_sonuc[m] != "")
                            _pattern = "<" + tagName + "." + _sonuc[m].Trim() + ".?>";
                }
                else if (degiskensayi == 2)
                {
                    string degisken1 = "", degisken2 = "";
                    for (int m = 0; m < _temp.Length; m++)
                        if (_sonuc[m] != "")
                            if (degisken1 == "")
                                degisken1 = _sonuc[m].Trim();
                            else
                                degisken2 = _sonuc[m].Trim();

                    _pattern = "<" + tagName + "." + degisken1 + "." + degisken2 + ".?>";
                    _pattern = _pattern + "|<" + tagName + "." + degisken2 + "." + degisken1 + ".?>";
                }
                else if (degiskensayi == 3)
                {
                    string degisken1 = "", degisken2 = "", degisken3 = "";
                    for (int m = 0; m < _temp.Length; m++)
                        if (_sonuc[m] != "")
                            if (degisken1 == "")
                                degisken1 = _sonuc[m].Trim();
                            else if (degisken2 == "")
                                degisken2 = _sonuc[m].Trim();
                            else
                                degisken3 = _sonuc[m].Trim();

                    _pattern = "<" + tagName + "." + degisken1 + "." + degisken2 + "." + degisken3 + ".?>";
                    _pattern = _pattern + "|<" + tagName + "." + degisken1 + "." + degisken3 + "." + degisken2 + ".?>";
                    _pattern = _pattern + "|<" + tagName + "." + degisken2 + "." + degisken1 + "." + degisken3 + ".?>";
                    _pattern = _pattern + "|<" + tagName + "." + degisken2 + "." + degisken3 + "." + degisken1 + ".?>";
                    _pattern = _pattern + "|<" + tagName + "." + degisken3 + "." + degisken1 + "." + degisken2 + ".?>";
                    _pattern = _pattern + "|<" + tagName + "." + degisken3 + "." + degisken2 + "." + degisken1 + ".?>";
                }
                else
                {
                    string degisken1 = "", degisken2 = "", degisken3 = "";
                    for (int m = 0; m < _temp.Length; m++)
                        if (_sonuc[m] != "")
                            if (degisken1 == "")
                                degisken1 = _sonuc[m].Trim();
                            else if (degisken2 == "")
                                degisken2 = _sonuc[m].Trim();
                            else
                                degisken3 = _sonuc[m].Trim();

                    _pattern = "<" + tagName + "." + degisken1 + "." + degisken2 + "." + degisken3 + ".?>";
                    _pattern = _pattern + "|<" + tagName + "." + degisken1 + "." + degisken3 + "." + degisken2 + ".*?>";
                    _pattern = _pattern + "|<" + tagName + "." + degisken2 + "." + degisken1 + "." + degisken3 + ".*?>";
                    _pattern = _pattern + "|<" + tagName + "." + degisken2 + "." + degisken3 + "." + degisken1 + ".*?>";
                    _pattern = _pattern + "|<" + tagName + "." + degisken3 + "." + degisken1 + "." + degisken2 + ".*?>";
                    _pattern = _pattern + "|<" + tagName + "." + degisken3 + "." + degisken2 + "." + degisken1 + ".*?>";
                }
            }
            else
                _pattern = regex_rules(tag);

            
            return _pattern;
        }

        private static string regex_rules(string tag)
        {
            tag = tag.Replace(":", ".");
            tag = tag.Replace("(", ".");
            tag = tag.Replace(")", ".");
            tag = tag.Replace("?", ".");
            tag = tag.Replace("*", ".");
            tag = tag.Replace("-", ".");
            tag = tag.Replace("/", ".");
            tag = tag.Replace("!", ".");
            tag = tag.Replace(" ", ".?");
            //tag = tag.Replace("i", ".");
            tag = tag.Replace("I", ".");
            tag = tag.Replace(";", ".");

            return tag;
        }

        private static string[] findAttritubes(string tag)
        {
            Regex exp = new Regex("\\s.*?=", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matchList = exp.Matches(tag);
            string[] _sonuc = new string[matchList.Count];

            for (int i = 0; i < matchList.Count; i++)
            {
                Match match = matchList[i];
                string _str = match.Groups[0].Value;

                char[] temp = _str.ToCharArray();
                Array.Reverse(temp);
                _str = new string(temp);

                _str = _str.Substring(_str.IndexOf("=") + 1, _str.IndexOf(" ") - _str.IndexOf("="));
                temp = _str.ToCharArray();
                Array.Reverse(temp);
                _str = new string(temp);
                _sonuc[i] = _str;
            }

            return _sonuc;
        }

        private static string findClassIDName(string tagname, string pattern)
        {
            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matchList = exp.Matches(tagname);

            Match match = matchList[0];
            string _str = match.Groups[1].Value;

            return _str;
        }
    }

}
