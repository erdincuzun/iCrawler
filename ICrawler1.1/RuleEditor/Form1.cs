using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using mshtml;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

using HTMLMarkerClass;


namespace WebMarker
{
    public partial class Form1 : Form
    {   
        HTMLMarkerClass.DOM _dom;
        HTMLMarkerClass.DOM _dom2;        


        ArrayList directory;

        ArrayList _list;

        private TreeNode _treeNode;

        private string _savehtmldoc;

        Hashtable _ht = new Hashtable();//for ruleset

        private void FillTree(int key, TreeNode _tnchild)
        {
            ArrayList _htmain = _dom.FindChilds(_list, key);
            HTMLMarkerClass.element _element = (HTMLMarkerClass.element)_list[key];

            foreach (HTMLMarkerClass.element d in _htmain)
            {
                TreeNode _tn = _tnchild.Nodes.Add(d.id.ToString(), d.elementName);
                FillTree(d.id, _tn);
            }

        }

        private void PrepareDOMTreeView(string _htmldoc)
        {
            _list = _dom.prepareDOM(_htmldoc);

            TreeNode _tn = DOMtreeView.Nodes.Add("0", "<html>");

            ArrayList _listchild0 = _dom.FindChilds(_list, 0);

            foreach (HTMLMarkerClass.element d in _listchild0)
            {               
                _treeNode = _tn.Nodes.Add(d.id.ToString(), d.elementName);

                FillTree(d.id, _treeNode);
            } 
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void BTN_Parse_Click(object sender, EventArgs e)
        {            
            string _htmldoc = "";
            OpenFileDialog opd = new OpenFileDialog();
            opd.InitialDirectory = Application.StartupPath + "\\" + CMBOX_Directory.Text;
            opd.Filter = "Web Pages | *.html;*.htm";
            opd.Title = "Bir dosya seçimi yapın.";
            if (opd.ShowDialog() == DialogResult.OK && opd.FileName.Length > 3)
            {
                string filename =  opd.FileName;
                if(filename.Contains(CMBOX_Directory.Text))
                    filename = "\\" + filename.Substring(filename.IndexOf(CMBOX_Directory.Text));
                LBL_html_filename.Text = "HTML FileName: " + filename;

                TextReader textReader = new StreamReader(opd.FileName);
                _htmldoc = textReader.ReadToEnd();
                textReader.Close();

                DOMtreeView.Nodes.Clear();
                _dom = new HTMLMarkerClass.DOM();
                DateTime _start = DateTime.Now;
                _htmldoc = _htmldoc.Replace("\r\n", "");

                webBrowser1.ScriptErrorsSuppressed = true;

                PrepareDOMTreeView(_htmldoc);
                _savehtmldoc = _dom.savehtmlContent;//for prepearing a new file

                TimeSpan _t = DateTime.Now.Subtract(_start);
                LBL_Time.Text = "Prepearing Time: " + _t.TotalMilliseconds.ToString() + " ms";


                groupBox2.Visible = true;
                groupBox5.Visible = true;
                groupBox6.Visible = false;
                panel1.Size = new Size(593, 270);

                string baslangic = "<p><span class=\"auto-style1\" style=\"mso-fareast-font-family: SimSun; mso-font-kerning: 1.0pt; mso-ansi-language: EN-US; mso-fareast-language: BO; mso-bidi-language: BO\">After extraction (AE) of child <i style=\"mso-bidi-font-style:normal\">DIV </i>/<i style=\"mso-bidi-font-style:normal\">TD</i> tags</span></p>";
                webBrowser1.DocumentText = baslangic;
                webBrowser3.DocumentText = baslangic;
            }                   
        }

        private void DOMtreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            groupBox6.Visible = true;
            panel1.Size = new Size(593, 355);
            groupBox6.Text = "Selected Node: " + DOMtreeView.SelectedNode.Text;

            int id = Convert.ToInt32(DOMtreeView.SelectedNode.Name);
            HTMLMarkerClass.element _element = (HTMLMarkerClass.element)_list[id];
            HTMLMarkerClass.element _elementust = new HTMLMarkerClass.element();
            if (_element.elementlinked_id >= 0)
            _elementust = (HTMLMarkerClass.element)_list[_element.elementlinked_id];

            webBrowser1.DocumentText = _element.outerHTML;
            webBrowser3.DocumentText = _element.outerHTML_AE;           

            listBox_Information.Items.Clear();
            listBox_Information.Items.Add("id : " + _element.id.ToString() + " : " + _element.elementName);
            listBox_Information.Items.Add("Tag : " + _element.tagName);
            if (_element.elementlinked_id >= 0)
            listBox_Information.Items.Add("linked id : " + _element.elementlinked_id.ToString() + " : " + _elementust.elementName);
            listBox_Information.Items.Add("WordsCount : " + _element.wordCount.ToString());
            listBox_Information.Items.Add("DensityInHTML : " + _element.DensityinHTML.ToString());
            listBox_Information.Items.Add("LinkCount : " + _element.LinkCount);
            listBox_Information.Items.Add("wordCountinLink : " + _element.wordCountinLink);
            listBox_Information.Items.Add("meanofWordinLinks : " + _element.meanofWordinLinks.ToString());
            listBox_Information.Items.Add("meanofWordinLinksinAllWords : " + _element.meanofWordinLinksAllWords.ToString());
            listBox_Information.Items.Add("WordsCount_AE : " + _element.wordCount_AE.ToString());
            listBox_Information.Items.Add("DensityInHTML_AE : " + _element.DensityinHTML_AE.ToString());
            listBox_Information.Items.Add("LinkCount_AE : " + _element.LinkCount_AE);
            listBox_Information.Items.Add("wordCountinLink_AE : " + _element.wordCountinLink_AE);
            listBox_Information.Items.Add("meanofWordinLinks_AE : " + _element.meanofWordinLinks_AE.ToString());
            listBox_Information.Items.Add("meanofWordinLinksinAllWords_AE : " + _element.meanofWordinLinksAllWords_AE.ToString());

            listBox_Information2.Items.Clear();
            listBox_Information2.Items.Add("Tag number information");
            listBox_Information2.Items.Add("DIV - AE: " + _element.div_count.ToString() + " - " + _element.div_count_AE.ToString());
            listBox_Information2.Items.Add("TD - AE: " + _element.td_count.ToString() + " - " + _element.td_count_AE.ToString());
            listBox_Information2.Items.Add("UL - AE: " + _element.ul_count.ToString() + " - " + _element.ul_count_AE.ToString());
            listBox_Information2.Items.Add("H1 - AE: " + _element.h1_count.ToString() + " - " + _element.h1_count_AE.ToString());
            listBox_Information2.Items.Add("H2 - AE: " + _element.h2_count.ToString() + " - " + _element.h2_count_AE.ToString());
            listBox_Information2.Items.Add("P - AE: " + _element.p_count.ToString() + " - " + _element.p_count_AE.ToString());
            listBox_Information2.Items.Add("SPAN - AE: " + _element.span_count.ToString() + " - " + _element.span_count_AE.ToString());
            listBox_Information2.Items.Add("BR - AE: " + _element.br_count.ToString() + " - " + _element.br_count_AE.ToString());
            listBox_Information2.Items.Add("H3 - AE: " + _element.h3_count.ToString() + " - " + _element.h3_count_AE.ToString());
            listBox_Information2.Items.Add("H4 - AE: " + _element.h4_count.ToString() + " - " + _element.h4_count_AE.ToString());
            listBox_Information2.Items.Add("H5 - AE: " + _element.h5_count.ToString() + " - " + _element.h5_count_AE.ToString());
            listBox_Information2.Items.Add("H6 - AE: " + _element.h6_count.ToString() + " - " + _element.h6_count_AE.ToString());
            listBox_Information2.Items.Add("IMG - AE: " + _element.img_count.ToString() + " - " + _element.img_count_AE.ToString());
            listBox_Information2.Items.Add("INPUT - AE: " + _element.input_count.ToString() + " - " + _element.input_count_AE.ToString());
            listBox_Information2.Items.Add("LI - AE: " + _element.li_count.ToString() + " - " + _element.li_count_AE.ToString());
            listBox_Information2.Items.Add("OBJECT - AE: " + _element.object_count.ToString() + " - " + _element.object_count_AE.ToString());
            listBox_Information2.Items.Add("DOT(.) - AE: " + _element.dot_count.ToString() + " - " + _element.dot_count_AE.ToString());

            string class_sonuc = HTMLMarkerClass.desicionClass.determineClass(_element);
            listBox_Information.Items.Add("Relevant Prediction : " + class_sonuc);
            LBL_Time3.Text = class_sonuc;

            string rule_sonuc = "";
            LBL_Information.Text = "";
            foreach (string line in xmlfile._xml_list)
            {
                if (line.Contains(" tagname=\"" + _element.elementName + "\"") && line.Contains(" parent_tagname=\"" + _element.parent_elementName + "\""))
                {
                    if (line.Length > 2)
                    {
                        LBL_Information.Text = line.Substring(1, line.IndexOf(" ") - 1);
                        rule_sonuc = line.Substring(1, line.IndexOf(" ") - 1);
                    }
                }
            }

            if (rule_sonuc == "")
                LBL_Tahmin.Text = "";
            else
            {
                if (rule_sonuc != class_sonuc)
                {
                    LBL_Tahmin.Text = "Error";
                    LBL_Tahmin.ForeColor = Color.Red;
                }
                else
                {
                    LBL_Tahmin.Text = "OK";
                    LBL_Tahmin.ForeColor = Color.Green; 
                }
            }
        }

        private string[] findTagANDOtherPart(string html)
        {
            string _starttag = html;
            int _start = _starttag.IndexOf('<');
            int _end = _starttag.IndexOf('>');
            _starttag = _starttag.Substring(_start, _end - _start + 1);
            string otherparts = html.Substring(_end - _start + 1, html.Length - (_end - _start + 1));

            string[] _res = new string[2];
            _res[0] = _starttag;
            _res[1] = otherparts;

            return  _res;
        }


        private void linksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
                xmlfile._xml_list_add("advertisement", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("menu", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void linksRelatedWithArticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("links", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void headLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("headline", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void mainofArticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("main", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void summaryofArticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("summary", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void commentsofArticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("comments", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void tagsofArticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("tags", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void bannerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("banner", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("image", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("comment_sub", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void linksRelatedWithArticleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("menu", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void copyrightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("copyright", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void ınformationAboutArticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("additional", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void mixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LBL_Information.Text == "")
            xmlfile._xml_list_add("empty", DOMtreeView.SelectedNode.Name, _list);
            else
                MessageBox.Show("Daha önceden işaretlenmiş");
        }

        private void sAVEFILEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //xmlfile.Save_XML_File();            
        }       


        private void Form1_Load(object sender, EventArgs e)
        {
            directory = new ArrayList();
            DirectoryInfo di = new DirectoryInfo(Application.StartupPath);
            directory.AddRange(di.GetDirectories());

            CMBOX_Directory.Items.Clear();
            for (int i = 0; i < directory.Count; i++)
                CMBOX_Directory.Items.Add(directory[i] + "\\");


            groupBox2.Visible = false;
            groupBox5.Visible = false;
            groupBox6.Visible = false;
            panel1.Size = new Size(593, 82);

            string baslangic = "<p><span class=\"auto-style1\" style=\"mso-fareast-font-family: SimSun; mso-font-kerning: 1.0pt; mso-ansi-language: EN-US; mso-fareast-language: BO; mso-bidi-language: BO\">After extraction (AE) of child <i style=\"mso-bidi-font-style:normal\">DIV </i>/<i style=\"mso-bidi-font-style:normal\">TD</i> tags</span></p>";
            webBrowser1.DocumentText = baslangic;
            webBrowser3.DocumentText = baslangic;

            if (CMBOX_Directory.Items.Count > 0)
            {
                CMBOX_Directory.Text = CMBOX_Directory.Items[0].ToString();
                LBL_html_filename.Text = "";
                LBL_Information.Text = "";
                LBL_Time3.Text = "";
                LBL_Tahmin.Text = "";
            }
            else
            {
                MessageBox.Show("No Any Directory!!!");                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }        

        private void TXT_No_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }


        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
                          
        }

        private void button5_Click(object sender, EventArgs e)
        {
               
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            
        }

        private string Fast_Retrieve(string xml, string _htmldoc, string xmltagname)
        {
            HTMLMarkerClass.DOM _dom = new HTMLMarkerClass.DOM();


            string p_ename = "";// _xml.parent_elementName;
            string ename = "";// _xml.elementName;

            if (p_ename.Contains(","))
            {
                string[] s = p_ename.Split(',');
                p_ename = s[0];              
            }

            p_ename = p_ename.Replace(" ", ".");
            p_ename = p_ename.Replace(":", ".");
            p_ename = p_ename.Replace("(", ".");
            p_ename = p_ename.Replace(")", ".");
            p_ename = p_ename.Replace("?", ".");
            p_ename = p_ename.Replace("*", ".");

            if (ename.Contains(","))
            {
                string[] s = ename.Split(',');
                ename = s[0];               
            }

            ename = ename.Replace(" ", ".");
            ename = ename.Replace(":", ".");
            ename = ename.Replace("(", ".");
            ename = ename.Replace(")", ".");
            ename = ename.Replace("?", ".");
            ename = ename.Replace("*", ".");

            if (p_ename != "")
            {
                if (!_ht.ContainsKey(p_ename))
                {
                    string[] htmlcon = HTMLMarkerClass.webfilter.Contents_of_givenLayout_Tags_TESTER(_htmldoc, p_ename, true);
                    string[] htmlcon2 = new string[2];

                    _ht.Add(p_ename, htmlcon);
                    if (htmlcon != null)
                    if (htmlcon.Length == 1)
                        if (!_ht.ContainsKey(ename))
                        {
                            htmlcon2 = HTMLMarkerClass.webfilter.Contents_of_givenLayout_Tags_TESTER(htmlcon[0], ename, true);
                            _ht.Add(ename, htmlcon2);
                        }
                        else
                            htmlcon2 = (string[])_ht[ename];


                    if (htmlcon2 != null)
                    if (htmlcon2.Length == 1)
                    {
                        htmlcon2[0] = HTMLMarkerClass.HTML.stripHtml(htmlcon2[0]);
                        xml = xml + "<" + xmltagname + " tag=\"" + _dom.clear_illegal_characters_for_XML(ename) + "\" parenttag=\"" + _dom.clear_illegal_characters_for_XML(p_ename) + "\"  >\r\n" + HTMLMarkerClass.HTML.stripHtml(htmlcon2[0]) + "</" + xmltagname + ">\r\n";
                    }
                    else
                        xml = xml + "<" + xmltagname + " tag=\"" + _dom.clear_illegal_characters_for_XML(ename) + "\" parenttag=\"" + _dom.clear_illegal_characters_for_XML(p_ename) + "\"  >\r\n ... ERROR ... </" + xmltagname + ">\r\n";
                    
                }
                else
                {
                    string[] htmlcon = (string[])_ht[p_ename];
                    string[] htmlcon2 = new string[2];

                    if (htmlcon != null)
                    if (htmlcon.Length == 1)
                        if (!_ht.ContainsKey(ename))
                        {
                            htmlcon2 = HTMLMarkerClass.webfilter.Contents_of_givenLayout_Tags_TESTER(htmlcon[0], ename, true);
                            _ht.Add(ename, htmlcon2);
                        }
                        else
                            htmlcon2 = (string[])_ht[ename];

                    if(htmlcon2 != null)
                    if (htmlcon2.Length == 1)
                    {
                        htmlcon2[0] = HTMLMarkerClass.HTML.stripHtml(htmlcon2[0]);
                        xml = xml + "<" + xmltagname + " tag=\"" + _dom.clear_illegal_characters_for_XML(ename) + "\" parenttag=\"" + _dom.clear_illegal_characters_for_XML(p_ename) + "\"  >\r\n" + HTMLMarkerClass.HTML.stripHtml(htmlcon2[0]) + "</" + xmltagname + ">\r\n";
                    }
                    else
                        xml = xml + "<" + xmltagname + " tag=\"" + _dom.clear_illegal_characters_for_XML(ename) + "\" parenttag=\"" + _dom.clear_illegal_characters_for_XML(p_ename) + "\"  >\r\n ... ERROR ... </" + xmltagname + ">\r\n";


                }
            }
            else //parent boş ise direkt köke bak
            {
                if (!_ht.ContainsKey(ename))
                {
                    string[] htmlcon = HTMLMarkerClass.webfilter.Contents_of_givenLayout_Tags_TESTER(_htmldoc, ename, true);

                    if (htmlcon != null)
                    {
                        _ht.Add(ename, htmlcon);

                        if (htmlcon != null)
                        if (htmlcon.Length == 1)
                        {
                            htmlcon[0] = HTMLMarkerClass.HTML.stripHtml(htmlcon[0]);
                            xml = xml + "<" + xmltagname + " tag=\"" + _dom.clear_illegal_characters_for_XML(ename) + "\" parenttag=\"" + _dom.clear_illegal_characters_for_XML(p_ename) + "\"  >\r\n" + HTMLMarkerClass.HTML.stripHtml(htmlcon[0]) + "</" + xmltagname + ">\r\n";
                        }
                        else
                            xml = xml + "<" + xmltagname + " tag=\"" + _dom.clear_illegal_characters_for_XML(ename) + "\" parenttag=\"" + _dom.clear_illegal_characters_for_XML(p_ename) + "\"  >\r\n ... ERROR ... </" + xmltagname + ">\r\n";
                    }
                }
                else
                {
                    string[] htmlcon = (string[])_ht[ename];
                    if (htmlcon != null)
                    if (htmlcon.Length == 1)
                    {
                        htmlcon[0] = HTMLMarkerClass.HTML.stripHtml(htmlcon[0]);
                        xml = xml + "<" + xmltagname + " tag=\"" + _dom.clear_illegal_characters_for_XML(ename) + "\" parenttag=\"" + _dom.clear_illegal_characters_for_XML(p_ename) + "\"  >\r\n" + HTMLMarkerClass.HTML.stripHtml(htmlcon[0]) + "</" + xmltagname + ">\r\n";
                    }
                    else
                        xml = xml + "<" + xmltagname + " tag=\"" + _dom.clear_illegal_characters_for_XML(ename) + "\" parenttag=\"" + _dom.clear_illegal_characters_for_XML(p_ename) + "\"  >\r\n ... ERROR ... </" + xmltagname + ">\r\n";
                }

            }

            return xml;
        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
           
        }

        private void button4_Click_2(object sender, EventArgs e)
        {
          
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
           
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
           
        }

        private void button13_Click(object sender, EventArgs e)
        {
           
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
           
        }

        private void DOMtreeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
        }

        private void button5_Click_2(object sender, EventArgs e)
        {
            domsim.prepareDOMSim(ref _dom, ref _dom2);
        }

        private void CMBOX_Directory_SelectedIndexChanged(object sender, EventArgs e)
        {
            xmlfile._destdir = Application.StartupPath + "\\" + CMBOX_Directory.Text;
            xmlfile.Read_XMLFile();

            if (File.Exists(CMBOX_Directory.Text + "rule.xml"))
                LBL_RuleFileName.Text = "Rule FileName: \\" + CMBOX_Directory.Text + "rule.xml";
            else
                LBL_RuleFileName.Text = "No Rule File";

            CMBOX_Cases.Text = "";
            CMBOX_XML_List.Text = "";
            LBL_Time.Text = "";
        }

        private void CMBOX_Directory_Sub_SelectedIndexChanged(object sender, EventArgs e)
        {

        }       

        private void CMBOX_Cases_SelectedIndexChanged(object sender, EventArgs e)
        {
            CMBOX_XML_List.Items.Clear();
            foreach (string _xml_e in xmlfile._xml_list)
                if (_xml_e.Contains("<" + CMBOX_Cases.Text + " "))
                    CMBOX_XML_List.Items.Add(_xml_e);            
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (CMBOX_XML_List.Text != "")
            {
                xmlfile._xml_list_change(CMBOX_XML_List.Text, CMBOX_Cases.Text, CMBOX_NewCase.Text);

                CMBOX_XML_List.Text = "";
                CMBOX_XML_List.Items.Clear();
                foreach (string _xml_e in xmlfile._xml_list)
                    if (_xml_e.Contains("<" + CMBOX_Cases.Text + " "))
                        CMBOX_XML_List.Items.Add(_xml_e);                 
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (CMBOX_XML_List.Text != "")
            {
                xmlfile._xml_list_delete(CMBOX_XML_List.Text);                    
                CMBOX_XML_List.Text = "";

                CMBOX_XML_List.Items.Clear();
                foreach (string _xml_e in xmlfile._xml_list)
                    if (_xml_e.Contains("<" + CMBOX_Cases.Text + " "))
                        CMBOX_XML_List.Items.Add(_xml_e);  
            }
        }        

        private void CMBOX_XML_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            string all_blocks = "";
            string all_blocks_AE = "";
            if (_dom != null)
            {
                webBrowser1.DocumentText = "";
                int i = 1;
                foreach (HTMLMarkerClass.element _element in _dom._list)
                {
                    string tagName = "<"+xmlfile.findElementName(CMBOX_XML_List.Text, "tagname=\"<(.*?)>\"")+">";
                    string parent_tagName = "<" + xmlfile.findElementName(CMBOX_XML_List.Text, "parent_tagname=\"<(.*?)>\"") + ">";

                    if (_element.elementName == tagName && _element.parent_elementName == parent_tagName)
                    {
                        all_blocks = all_blocks + "<b>" + i.ToString() + "</b><br /><br />" + _element.outerHTML;
                        all_blocks_AE = all_blocks_AE + "<b>" + i.ToString() + "</b><br /><br />" + _element.outerHTML_AE;
                        i++;
                    }
                    else
                    {
                        //parent çok değişken olduğu durumlar var bunun için!!!
                        /*if (CMBOX_Cases.Text=="main" && _element.elementName == tagName)
                        {
                            all_blocks = all_blocks + "<b>" + i.ToString() + "</b><br /><br />" + _element.outerHTML;
                            all_blocks_AE = all_blocks_AE + "<b>" + i.ToString() + "</b><br /><br />" + _element.outerHTML_AE;
                            i++;
                        } */
                    }
                }

                /*if(i==1)
                    lbl_dENae.Text = "Nothing";
                else if(i==2)
                    lbl_dENae.Text = "unique";
                else
                    lbl_dENae.Text = "repetitive";*/
                
                
            webBrowser1.DocumentText = all_blocks;
            webBrowser3.DocumentText = all_blocks_AE;
            }           
        }

        TextWriter tw;

        private void CreateARFF_File(string FileName, int islem, string className, HTMLMarkerClass.element _e, int secim)
        {   
            //başlangıç ayarları
            if(islem == 0)
            {
            tw = new StreamWriter(FileName);
            tw.WriteLine("@relation class_relation");
            tw.WriteLine("@attribute 'tagName' {DIV, TD, H1, H2, H3, H4, H5, H6, P, SPAN, FONT, UL}");
            tw.WriteLine("@attribute 'wordCount_AE' real");
            tw.WriteLine("@attribute 'densityinHTML_AE' real");
            tw.WriteLine("@attribute 'LinkCount_AE' real");
            tw.WriteLine("@attribute 'wordCountinLink_AE' real");
            tw.WriteLine("@attribute 'meanofWordinLinks_AE' real");
            tw.WriteLine("@attribute 'meanofWordinLinksAllWords_AE' real");
            tw.WriteLine("@attribute 'wordCount' real");
            tw.WriteLine("@attribute 'DensityinHTML' real");
            tw.WriteLine("@attribute 'LinkCount' real");
            tw.WriteLine("@attribute 'wordCountinLink' real");
            tw.WriteLine("@attribute 'meanofWordinLinks' real");
            tw.WriteLine("@attribute 'meanofWordinLinksAllWords' real");
            tw.WriteLine("@attribute 'sim_bagofword' real");
            tw.WriteLine("@attribute 'sim_bagofword_AE' real");
            tw.WriteLine("@attribute 'sim_innerHTML' real");
            tw.WriteLine("@attribute 'sim_innerHTML_AE' real");
            tw.WriteLine("@attribute 'dot_count_AE' real");
            tw.WriteLine("@attribute 'h1_count_AE' real");
            tw.WriteLine("@attribute 'h2_count_AE' real");
            tw.WriteLine("@attribute 'h3_count_AE' real");
            tw.WriteLine("@attribute 'h4_count_AE' real");
            tw.WriteLine("@attribute 'h5_count_AE' real");
            tw.WriteLine("@attribute 'h6_count_AE' real");
            tw.WriteLine("@attribute 'img_count_AE' real");
            tw.WriteLine("@attribute 'p_count_AE' real");
            tw.WriteLine("@attribute 'br_count_AE' real");
            tw.WriteLine("@attribute 'span_count_AE' real");
            tw.WriteLine("@attribute 'object_count_AE' real");
            tw.WriteLine("@attribute 'ul_count_AE' real");
            tw.WriteLine("@attribute 'li_count_AE' real");
            tw.WriteLine("@attribute 'input_count_AE' real");
            tw.WriteLine("@attribute 'div_count_AE' real");
            tw.WriteLine("@attribute 'td_count_AE' real");
            tw.WriteLine("@attribute 'dot_count' real");
            tw.WriteLine("@attribute 'h1_count' real");
            tw.WriteLine("@attribute 'h2_count' real");
            tw.WriteLine("@attribute 'h3_count' real");
            tw.WriteLine("@attribute 'h4_count' real");
            tw.WriteLine("@attribute 'h5_count' real");
            tw.WriteLine("@attribute 'h6_count' real");
            tw.WriteLine("@attribute 'img_count' real");
            tw.WriteLine("@attribute 'p_count' real");
            tw.WriteLine("@attribute 'br_count' real");
            tw.WriteLine("@attribute 'span_count' real");
            tw.WriteLine("@attribute 'object_count' real");
            tw.WriteLine("@attribute 'ul_count' real");
            tw.WriteLine("@attribute 'li_count' real");
            tw.WriteLine("@attribute 'input_count' real");
            tw.WriteLine("@attribute 'div_count' real");
            tw.WriteLine("@attribute 'td_count' real");
            tw.WriteLine("@attribute 'repeat_tag_count' real");


            if (secim == 1)
                tw.WriteLine("@attribute 'class' {relevant_block, irrelevant_block, main_block}");
            if (secim == 2)
                tw.WriteLine("@attribute 'class' {menu, links, empty}");
            if (secim == 3)
                tw.WriteLine("@attribute 'class' {headline, summary, others}");
            if (secim == 4)
                tw.WriteLine("@attribute 'class' {headline, summary, others, menu, links, empty, main}");

            tw.WriteLine("@data");
            }
            //veri ekle
            if(islem == 1)
            {
                tw.WriteLine(_e.tagName + ","
                                        + _e.wordCount_AE + "," + _e.DensityinHTML_AE + "," + _e.LinkCount_AE + ","
                                        + _e.wordCountinLink_AE + "," + _e.meanofWordinLinks_AE + "," + _e.meanofWordinLinksAllWords_AE + ","
                                        + _e.wordCount + "," + _e.DensityinHTML + "," + _e.LinkCount + ","
                                        + _e.wordCountinLink + "," + _e.meanofWordinLinks + "," + _e.meanofWordinLinksAllWords + ","
                                        + _e.sim_bagofword + "," + _e.sim_bagofword_AE + "," + _e.sim_innerHTML + "," + _e.sim_innerHTML_AE + ","
                                        + _e.dot_count_AE + "," + _e.h1_count_AE + "," + _e.h2_count_AE + ","
                                        + _e.h2_count_AE + "," + _e.h4_count_AE + "," + _e.h5_count_AE + ","
                                        + _e.h6_count_AE + "," + _e.img_count_AE + "," + _e.p_count_AE + ","
                                        + _e.br_count_AE + "," + _e.span_count_AE + "," + _e.object_count_AE + ","
                                        + _e.ul_count_AE + "," + _e.li_count_AE + "," + _e.input_count_AE + ","
                                        + _e.div_count_AE + "," + _e.td_count_AE + "," 
                                        + _e.dot_count + "," + _e.h1_count + "," + _e.h2_count + ","
                                        + _e.h2_count + "," + _e.h4_count + "," + _e.h5_count + ","
                                        + _e.h6_count + "," + _e.img_count + "," + _e.p_count + ","
                                        + _e.br_count + "," + _e.span_count + "," + _e.object_count + ","
                                        + _e.ul_count + "," + _e.li_count + "," + _e.input_count + ","
                                        + _e.div_count + "," + _e.td_count + "," + _e.repeat_tag_count + ","
                                        + className);
            }
            //dosyayı kapat
            if(islem == 2)
            {
                tw.Close();
            }

        }

        private void prepeareARFF_File_for_Block_Detection(DOM _tempDOM, string line, int secim)
        {
            foreach (HTMLMarkerClass.element _element in _tempDOM._list)
            {
                //parent yok!!!
                string _ename = "<" + xmlfile.findElementName(line, "[^_]tagname=\"<(.*?)>\"") + ">";
                string parent_tagName = "<" + xmlfile.findElementName(line, "parent_tagname=\"<(.*?)>\"") + ">";

                if (_element.elementName == _ename && _element.parent_elementName == parent_tagName)
                {
                    if(_element.BagofWords_AE != null)
                    if(_element.BagofWords_AE.Trim() != "")//boşsa hata var demek
                        if (_element.tagName == "DIV" || _element.tagName == "TD" || _element.tagName == "UL")
                        {
                        string className = "";
                        if (line.Length > 2)
                            className = line.Substring(1, line.IndexOf(" ") - 1);

                        if (className != "image")
                        {
                            if (className == "main")
                                className = "main_block";
                            else if (className == "additional" || className == "headline"
                                    || className == "summary"
                                    || className == "comments" || className == "comment_sub")
                                    className = "relevant_block";
                            else
                                    className = "irrelevant_block";

                            //|| className == "tags"|| className == "image"

                            //benzer etiket içermeyen bölümler ignore ediliyor
                            bool yazma = false;
                            if (className == "main_block" && _element.repeat_tag_count > 6)
                                yazma = true;

                            //&& _element.sim_bagofword_AE != -1
                            if (!yazma)
                            if (className == "irrelevant_block")   
                                    CreateARFF_File("", 1, className, _element, secim);
                            else if (_element.wordCount_AE != 0)
                                if (_element.tagName != "UL")
                                    CreateARFF_File("", 1, className, _element, secim);
                            }
                        
                    }
                    //relevant_block, irrelevant_block, main_block                                   
                    //arff dosyasına bir veri ekle
                }//if there is _elementName??? 
            }//all dom
        }

        private void prepeareARFF_File_for_Unnecessary_Block_Detection(DOM _tempDOM, string line, int secim)
        {
            foreach (HTMLMarkerClass.element _element in _tempDOM._list)
            {
                //parent yok!!!
                string _ename = "<" + xmlfile.findElementName(line, "[^_]tagname=\"<(.*?)>\"") + ">";
                string parent_tagName = "<" + xmlfile.findElementName(line, "parent_tagname=\"<(.*?)>\"") + ">";

                if (_element.elementName == _ename && _element.parent_elementName == parent_tagName)
                {                  
                        string className = "";
                        if (line.Length > 2)
                            className = line.Substring(1, line.IndexOf(" ") - 1);

                        if (className == "menu")
                            className = "menu";
                        else if (className == "links")
                            className = "links";
                        else if (className == "empty")
                            className = "empty";
                        else
                            className = "";

                        if (_element.wordCount_AE != 0 && className != "")
                            CreateARFF_File("", 1, className, _element, secim);                 
                    //relevant_block, irrelevant_block, main_block                                   
                    //arff dosyasına bir veri ekle
                }//if there is _elementName??? 
            }//all dom
        }

        private void prepeareARFF_File_for_ALL(DOM _tempDOM, string line, int secim)
        {
            foreach (HTMLMarkerClass.element _element in _tempDOM._list)
            {
                //parent yok!!!
                string _ename = "<" + xmlfile.findElementName(line, "[^_]tagname=\"<(.*?)>\"") + ">";
                string parent_tagName = "<" + xmlfile.findElementName(line, "parent_tagname=\"<(.*?)>\"") + ">";

                if (_element.elementName == _ename && _element.parent_elementName == parent_tagName)
                {
                    string className = "";
                    if (line.Length > 2)
                        className = line.Substring(1, line.IndexOf(" ") - 1);

                    if (className == "image" || className == "tags")
                        className = "";

                        if (className == "main")
                            className = "main";
                        else if (className == "menu")
                            className = "menu";
                        else if (className == "links")
                            className = "links";
                        else if (className == "empty")
                            className = "empty";
                        else if (className == "headline")
                            className = "headline";
                        else if (className == "summary")
                            className = "summary";
                        else if (className == "comment_sub"  || className == "additional")
                            className = "others";
                        else
                            className = "";

                        //benzer etiket içermeyen bölümler ignore ediliyor
                        bool yazma = false;
                        if (className == "main" && _element.repeat_tag_count > 6)
                            yazma = true;

                        //&& _element.sim_bagofword_AE != -1
                        if(className != "")
                            if (!yazma)
                                CreateARFF_File("", 1, className, _element, secim);
                    
                }//if there is _elementName??? 
            }//all dom
        }

        private void prepeareARFF_File_for_Necessary_Block_Detection(DOM _tempDOM, string line, int secim)
        {
            foreach (HTMLMarkerClass.element _element in _tempDOM._list)
            {
                //parent yok!!!
                string _ename = "<" + xmlfile.findElementName(line, "[^_]tagname=\"<(.*?)>\"") + ">";
                string parent_tagName = "<" + xmlfile.findElementName(line, "parent_tagname=\"<(.*?)>\"") + ">";

                if (_element.elementName == _ename && _element.parent_elementName == parent_tagName)
                {
                    string className = "";
                    if (line.Length > 2)
                        className = line.Substring(1, line.IndexOf(" ") - 1);
                    
                    if (className == "headline")
                        className = "headline";
                    /*else if (className == "main")
                        className = "main";*/
                    else if (className == "summary")
                        className = "summary";
                    /*else if (className == "tags")
                        className = "tags";*/
                    else if (className == "comment_sub" || (_element.tagName != "DIV" && className == "additional"))
                        className = "others";
                    else
                        className = "";

                    if (_element.wordCount_AE != 0 && className != "")
                        CreateARFF_File("", 1, className, _element, secim);
                    //relevant_block, irrelevant_block, main_block                                   
                    //arff dosyasına bir veri ekle
                }//if there is _elementName??? 
            }//all dom
        }

        private void prepareARFF_File(int secim, string filename)
        {
            
        }

        private void BTN_TrainingSet_Click(object sender, EventArgs e)
        {
            DateTime _start = DateTime.Now;
            prepareARFF_File(1, "block_prediction.arff");
            TimeSpan _t = DateTime.Now.Subtract(_start);
            LBL_Time3.Text = _t.ToString();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            LBL_Information.Text = webfilter.CountStringOccurrences("deneme de DE", "de").ToString();
        }

        private void BTN_TrainingSet2_Click(object sender, EventArgs e)
        {
            prepareARFF_File(2, "unnecessary_block_prediction.arff");
        }

        private void BTN_TrainingSet3_Click(object sender, EventArgs e)
        {
            prepareARFF_File(3, "necessary_block_prediction.arff");
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click_3(object sender, EventArgs e)
        {
            prepareARFF_File(4, "all_in_one.arff");
        }

        private void ShowContent(string _class)
        {
            listBox_Information.Items.Clear();
            string sonuc = "";
            string sonuc_AE = "";
            int no = 1;
            if (_list != null)
            {
                HTMLMarkerClass.desicionClass._list = _list; //_list gönder

                for (int i = 0; i < _list.Count; i++)
                {
                    HTMLMarkerClass.element _element = (HTMLMarkerClass.element)_list[i];

                    if (HTMLMarkerClass.desicionClass.write_or_not(_class, _element))
                    {
                        sonuc = sonuc + "<br /><h2>" + _class + " " + no.ToString() + "</h2><br />" + _element.outerHTML;
                        sonuc_AE = sonuc_AE + "<br /><h2>" + _class + " " + no.ToString() + "</h2><br />" + _element.outerHTML_AE;
                        listBox_Information.Items.Add("tn:" + _element.elementName);
                        listBox_Information.Items.Add("ptn:" + _element.parent_elementName);
                        no++;
                    }
                }
            }

            webBrowser1.DocumentText = sonuc;
            webBrowser3.DocumentText = sonuc_AE; 
        }

        private void button7_Click_2(object sender, EventArgs e)
        {
            ShowContent("main");
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            ShowContent("headline");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            ShowContent("summary");
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            ShowContent("others");
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            ShowContent("links");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            ShowContent("menu");
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            ShowContent("empty");
        }

        private void button18_Click(object sender, EventArgs e)
        {
            ML_Rules _mr = new ML_Rules(_dom._list, null);
            file_Rules _fr = new file_Rules("Dataset\\" + CMBOX_Directory.Text, "");

            Rules_Process.Compare_Two_Ht(_mr._ht_rules, _fr._ht_rules, _dom._list);

            listBox_Information.Items.Clear();
            for (int i = 0; i < Rules_Process._CN.Length; i++)
            {
                listBox_Information.Items.Add(Rules_Process._CN[i] + ": " + Rules_Process._CN_count_correct[i].ToString() + " - " + Rules_Process._CN_count_false[i].ToString());                
            }
        }

        private TimeSpan _gecensure;
        private long filesize;
        private void CreateDOM(ref DOM _dom_temp, string file)
        {
            DateTime _start = DateTime.Now;

            TextReader textReader = new StreamReader(file);
            string _htmldoc = textReader.ReadToEnd();
            textReader.Close();
            _htmldoc = _htmldoc.Replace("\r\n", "");
            if (_htmldoc.Trim() != "")
                _dom_temp.prepareDOM(_htmldoc);

            FileInfo f = new FileInfo(file);
            filesize = f.Length;

            _gecensure = DateTime.Now.Subtract(_start);
        }

        private void PredictionWriteaFile(ArrayList _l, string _d, string m, string webdomain, string subdomain_file)
        {
            ML_Rules _mr = new ML_Rules(_l, null);
            file_Rules _fr = new file_Rules(_d, "");

            Rules_Process.Compare_Two_Ht(_mr._ht_rules, _fr._ht_rules, _l);

            string temp_str = m; 
            for (int i = 0; i < Rules_Process._CN.Length; i++)
                    temp_str = temp_str + ";" + Rules_Process._CN_count_correct[i].ToString() + ";" + Rules_Process._CN_count_false[i].ToString();
            temp_str = temp_str + ";" + webdomain + ";" + subdomain_file + ";" +_gecensure.TotalMilliseconds.ToString() + ";" + filesize.ToString() + ";" + _l.Count.ToString();
            using (StreamWriter sw = File.AppendText("ML_Results.txt"))
                sw.WriteLine(temp_str);
        }

        private void button19_Click(object sender, EventArgs e)
        {
           
        }

        private string ReadFile(string file)
        {
            TextReader textReader = new StreamReader(file);
            string _htmldoc = textReader.ReadToEnd();
            textReader.Close();

            FileInfo f = new FileInfo(file);
            filesize = f.Length;

            _htmldoc = _htmldoc.Replace("\r\n", "");
            return _htmldoc;
        }

        private void EE_WriteaFile(string temp_str)
        {
            using (StreamWriter sw = File.AppendText("EE_Results.txt"))
                sw.WriteLine(temp_str);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            
           
        }

        private void button21_Click(object sender, EventArgs e)
        {
            file_Rules _fr = new file_Rules("Dataset\\" + CMBOX_Directory.Text, "");

            TextReader textReader = new StreamReader(LBL_html_filename.Text);
            string _htmldoc = textReader.ReadToEnd();
            textReader.Close();

            ArrayList _al = efficientextraction.extractRules(_fr._ht_rules, _htmldoc, "main");
            string bos = "<h1>main results test</h2><br>";
            int i = 1;
            foreach (string item in _al)
            {
                if(!bos.Contains(item))
                bos = bos + "<br> <h2>result " + i + "</h2> <br>" + item;
                i++;
            }

            webBrowser1.DocumentText = bos;
            webBrowser3.DocumentText = bos; 
            //<DIV id="blog_maintext" style="line-height:20px;">
            //<DIV.style=.?LINE.HEIGHT..20px.?.id=.?blog_maintext.?>
        }

        private void button22_Click(object sender, EventArgs e)
        {

        }

        private void button23_Click(object sender, EventArgs e)
        {
            file_Rules _fr1 = new file_Rules("Dataset\\" + CMBOX_Directory.Text, "");
            file_Rules _fr2 = new file_Rules("Dataset\\" + CMBOX_Directory.Text, "auto_template.xml");

            Rules_Process.Compare_Two_Ht(_fr1._ht_rules, _fr2._ht_rules, _dom._list);

            listBox_Information.Items.Clear();
            for (int i = 0; i < Rules_Process._CN.Length; i++)
            {
                listBox_Information.Items.Add(Rules_Process._CN[i] + ": " + Rules_Process._CN_count_correct[i].ToString() + " - " + Rules_Process._CN_count_false[i].ToString());
            }
        }
    }  
}