using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace HTMLMarkerClass
{
    public class xmlfile
    {
        public static ArrayList _xml_list;
        public static string _destdir;

        public static void Read_XMLFile()
        {
             _xml_list = new ArrayList();       

            string line = "";
            if (File.Exists(_destdir + "template.xml"))
            {
                StreamReader file = new StreamReader(_destdir + "template.xml");
                while ((line = file.ReadLine()) != null)
                {                  
                    _xml_list.Add(line);
                }

                file.Close();
            }
            else if (File.Exists(_destdir + "rule.xml"))
            {
                StreamReader file = new StreamReader(_destdir + "rule.xml");
                while ((line = file.ReadLine()) != null)
                {
                    _xml_list.Add(line);
                }

                file.Close();
            }
        }

        public static void Read_XMLFile(string filename)
        {
            _xml_list = new ArrayList();

            string line = "";
            if (File.Exists(filename))
            {
                StreamReader file = new StreamReader(filename);
                while ((line = file.ReadLine()) != null)
                {
                    _xml_list.Add(line);
                }

                file.Close();
            }
        }

        public static void Save_XML_File()
        {
            StreamWriter sw = File.CreateText(_destdir + "template.xml");
            foreach (string _veri in _xml_list)
                if (_veri.Trim() != "")
                    sw.WriteLine(_veri);
            sw.Close();
        }

        public static void _xml_list_add(string _case_Name, string index, ArrayList _list)
        {
            int id = Convert.ToInt32(index);
            HTMLMarkerClass.element _element = (HTMLMarkerClass.element)_list[id];
            HTMLMarkerClass.element _elementust = new HTMLMarkerClass.element();
            if (_element.elementlinked_id >= 0)
                _elementust = (HTMLMarkerClass.element)_list[_element.elementlinked_id];

            string _str = "<" + _case_Name + " ";
            _str = _str + "tagname=\"" + _element.elementName + "\" ";
            _str = _str + "parent_tagname=\"" + _elementust.elementName + "\" ";
            _str = _str + "html_text=\"<" + _element.innerHTML_AE + ">\" ";
            _str = _str + "_count=\"<100>\" ";
            
            //_str = _str + "tagname=\"" + _element.tagName + "\" ";
            //_str = _str + "tagclassName=\"" + _element.tag_class_Name + "\" ";
            //_str = _str + "tagidName=\"" + _element.tag_id_Name + "\" ";
            //_str = _str + "parent_tagname=\"" + _elementust.tagName + "\" ";
            //_str = _str + "parent_tagclassName=\"" + _elementust.tag_class_Name + "\" ";
            //_str = _str + "parent_tagidName=\"" + _elementust.tag_id_Name + "\" ";
            _str = _str + "/>";

            if (!_xml_list.Contains(_str))
                _xml_list.Add(_str);

            Save_XML_File();
            Read_XMLFile();
        }

        public static void _xml_list_change(string find_str, string case_str, string new_str)
        {
            for (int i = 0; i < _xml_list.Count; i++)
            {
                if ((string)_xml_list[i] == find_str)
                {
                    _xml_list[i] = _xml_list[i].ToString().Replace("<" + case_str + " ", "<" + new_str + " ");
                    Save_XML_File();
                    Read_XMLFile();               
                }
            }
        }        

        public static void _xml_list_delete(string find_str)
        {
            for (int i = 0; i < xmlfile._xml_list.Count; i++)
            {
                if ((string)xmlfile._xml_list[i] == find_str)
                {
                    xmlfile._xml_list.RemoveAt(i);
                    Save_XML_File();
                    Read_XMLFile();
                    break;
                }
            }
        }

        //string tagName = findElementName(line, "tagname=\"<(.*?)>\"");
        //string parent_tagName = findElementName(line, "parent_tagname=\"<(.*?)>\"");

        //find patterns in html
        public static string findElementName(string tagname, string pattern)
        {
            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matchList = exp.Matches(tagname);

            Match match = matchList[0];
            string _str = match.Groups[1].Value;

            return _str;
        }

        public static string find_ClassName(string line)
        {
            return line.Substring(1, line.IndexOf(" ") - 1);
        }
    }
}
