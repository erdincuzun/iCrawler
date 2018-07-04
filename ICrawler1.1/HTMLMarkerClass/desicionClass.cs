using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace HTMLMarkerClass
{
    public static class desicionClass
    {             
        public static ArrayList _list;

        //structures of prediction
        private static bool _Main(HTMLMarkerClass.element _element)
        {
            if (_element.tagName == "DIV" || _element.tagName == "TD")
                if (HTMLMarkerClass.desicionClass.determineClass(_element) == "main" && _element.wordCount_AE >= 2)
                    return true;

            return false;
        }

        private static bool Others(HTMLMarkerClass.element _element)
        {
            if (_element.tagName == "DIV" || _element.tagName == "TD")
                if (_element.elementlinked_id != -1)
                {
                    HTMLMarkerClass.element _parent_element = (HTMLMarkerClass.element)_list[_element.elementlinked_id];
                    if (_parent_element.tagName == "DIV" || _parent_element.tagName == "TD" || _parent_element.tagName == "TR" || _parent_element.tagName == "FORM" || _parent_element.tagName == "BODY")
                        //if (!(HTMLMarkerClass.desicionClass.determineClass(_parent_element) == "links" || HTMLMarkerClass.desicionClass.determineClass(_parent_element) == "menu"))
                        if (HTMLMarkerClass.desicionClass.determineClass(_element) == "others" && _element.wordCount_AE >= 2)
                            return true;
                }

            return false;
        }

        private static bool Links(HTMLMarkerClass.element _element)
        {
            if (_element.tagName == "DIV" || _element.tagName == "TD" || _element.tagName == "UL")
                if (HTMLMarkerClass.desicionClass.determineClass(_element) == "links" && _element.wordCount_AE >= 2)
                    return true;

            return false;
        }

        private static bool Menus(HTMLMarkerClass.element _element)
        {
            if (_element.tagName == "DIV" || _element.tagName == "TD" || _element.tagName == "UL")
                if (HTMLMarkerClass.desicionClass.determineClass(_element) == "menu" && _element.wordCount_AE >= 2)
                    return true;

            return false;
        }

        private static bool Empty(HTMLMarkerClass.element _element)
        {
            if (_element.tagName == "DIV" || _element.tagName == "TD")
                if (HTMLMarkerClass.desicionClass.determineClass(_element) == "empty" && _element.wordCount_AE >= 2)
                    return true;

            return false;
        }

        private static bool Headline(HTMLMarkerClass.element _element)
        {
            if (!(_element.tagName == "DIV" || _element.tagName == "TD"))
                if (_element.elementlinked_id != -1)
                {
                    HTMLMarkerClass.element _parent_element = (HTMLMarkerClass.element)_list[_element.elementlinked_id];
                    if (HTMLMarkerClass.desicionClass._Main(_parent_element) || HTMLMarkerClass.desicionClass.Others(_parent_element))
                        if (HTMLMarkerClass.desicionClass.determineClass(_element) == "headline")
                            return true;
                }

            return false;
        }

        private static bool Summary(HTMLMarkerClass.element _element)
        {
            if (!(_element.tagName == "DIV" || _element.tagName == "TD"))
                if (_element.elementlinked_id != -1)
                {
                    HTMLMarkerClass.element _parent_element = (HTMLMarkerClass.element)_list[_element.elementlinked_id];
                    if (HTMLMarkerClass.desicionClass._Main(_parent_element) || HTMLMarkerClass.desicionClass.Others(_parent_element))
                        if (HTMLMarkerClass.desicionClass.determineClass(_element) == "summary")
                            return true;
                }

            return false;
        }

        public static bool write_or_not(string _class, HTMLMarkerClass.element _element)
        {
            bool yaz = false;
            if (_class == "main") yaz = _Main(_element);
            else if (_class == "others") yaz = Others(_element);
            else if (_class == "headline") yaz = Headline(_element);
            else if (_class == "summary") yaz = Summary(_element);
            else if (_class == "links") yaz = Links(_element);
            else if (_class == "menu") yaz = Menus(_element);
            else if (_class == "empty") yaz = Empty(_element);
            else yaz = false;

            return yaz;
        }


        /* 
           Desicion Tree for finding 7 classes 
           J48 pruned tree
        */
        public static string determineClass(HTMLMarkerClass.element d)
        {
            if (d.meanofWordinLinksAllWords_AE <= 0.419355)
            {
                if (d.DensityinHTML_AE <= 0.063521)
                {
                    if (d.tagName == "DIV")
                    {
                        if (d.wordCount_AE <= 3)
                            if (d.LinkCount_AE <= 0)
                                if (d.sim_bagofword_AE <= 0.00885)
                                    if (d.repeat_tag_count <= 1)
                                        if (d.div_count <= 1)
                                            if (d.span_count_AE <= 1)
                                                if (d.meanofWordinLinks <= 3.5)
                                                    if (d.wordCount_AE <= 0)
                                                        if (d.p_count_AE <= 0)
                                                            if (d.ul_count_AE <= 0) return "empty";
                                                            else return "links";
                                                        else return "empty";
                                                    else
                                                        if (d.span_count <= 0)
                                                            if (d.ul_count <= 1)
                                                                if (d.DensityinHTML_AE <= 0.003376)
                                                                    if (d.meanofWordinLinksAllWords > 0.15)
                                                                        return "empty";
                                                                    else
                                                                        if (d.input_count_AE >= 1)
                                                                            return "empty";
                                                                        else
                                                                            if (d.wordCount_AE > 10)
                                                                                if (d.wordCount_AE > 3)
                                                                                    return "others";
                                                                                else
                                                                                    return "empty";
                                                                            else
                                                                                if (d.input_count >= 1)
                                                                                    return "empty";
                                                                                else
                                                                                    if (d.wordCount_AE > 3)
                                                                                        return "others";
                                                                                    else
                                                                                        return "empty";
                                                                else
                                                                    if (d.wordCount_AE <= 2) return "empty";
                                                                    else
                                                                        if (d.DensityinHTML_AE <= 0.003881)
                                                                            return "empty";
                                                                        else
                                                                            if (d.input_count_AE >= 1)
                                                                                return "empty";
                                                                            else
                                                                                return "others";
                                                            else return "empty";
                                                        else return "empty";
                                                else
                                                    if (d.wordCount <= 76) return "menu";
                                                    else return "links";
                                            else return "menu";
                                        else
                                            if (d.sim_innerHTML_AE <= 0.01793)
                                                if (d.h5_count <= 4)
                                                    if (d.sim_bagofword_AE <= -1)
                                                        if (d.td_count <= 76)
                                                            if (d.p_count <= 25)
                                                                if (d.li_count <= 48)
                                                                    return "empty";
                                                                else
                                                                    if (d.div_count <= 19)
                                                                        if (d.wordCount_AE <= 2) return "menu";
                                                                        else return "empty";
                                                                    else return "empty";
                                                            else
                                                                if (d.LinkCount <= 2) return "empty";
                                                                else return "empty";
                                                        else
                                                            if (d.wordCount <= 120) return "links";
                                                            else return "empty";
                                                    else
                                                        if (d.sim_innerHTML <= 0.24712) return "empty";
                                                        else
                                                            if (d.DensityinHTML <= 0.378215) return "others";
                                                            else
                                                                if (d.h2_count <= 1) return "empty";
                                                                else return "links";
                                                else
                                                    if (d.div_count_AE <= 1) return "links";
                                                    else
                                                        if (d.dot_count_AE <= 1) return "empty";
                                                        else return "menu";
                                            else
                                                if (d.input_count <= 1) return "main";
                                                else return "empty";
                                    else
                                        if (d.span_count_AE <= 0)
                                            if (d.repeat_tag_count <= 38)
                                                if (d.h2_count <= 0)
                                                    if (d.input_count <= 0)
                                                        if (d.ul_count <= 0)
                                                            if (d.p_count <= 1)
                                                                if (d.p_count_AE <= 0)
                                                                    if (d.wordCount_AE <= 2)
                                                                        if (d.h4_count <= 0)
                                                                            if (d.div_count_AE <= 1)
                                                                                if (d.h1_count <= 0)
                                                                                    if (d.repeat_tag_count <= 9)
                                                                                        if (d.wordCountinLink <= 0)
                                                                                            if (d.wordCount_AE <= 1)
                                                                                                if (d.div_count <= 2)
                                                                                                    if (d.div_count <= 1)
                                                                                                        if (d.wordCount_AE <= 0)
                                                                                                            if (d.repeat_tag_count <= 4)
                                                                                                                if (d.repeat_tag_count <= 2) return "empty";
                                                                                                                else return "menu";
                                                                                                            else
                                                                                                                if (d.DensityinHTML_AE <= 0.000429)
                                                                                                                    if (d.repeat_tag_count <= 6) return "empty";
                                                                                                                    else return "menu";
                                                                                                                else return "links";
                                                                                                        else return "menu";
                                                                                                    else return "links";
                                                                                                else
                                                                                                    if (d.div_count <= 6) return "others";
                                                                                                    else return "empty";
                                                                                            else return "";
                                                                                        else return "menu";
                                                                                    else
                                                                                        if (d.DensityinHTML_AE <= 0.002157)
                                                                                            if (d.sim_bagofword <= -1)
                                                                                                if (d.wordCount_AE > 2) return "others";
                                                                                                else return "empty";
                                                                                            else return "empty";
                                                                                        else return "links";
                                                                                else return "empty";
                                                                            else
                                                                                if (d.div_count_AE <= 3) return "links";
                                                                                else return "empty";
                                                                        else return "links";
                                                                    else return "others";
                                                                else return "others";
                                                            else
                                                                if (d.wordCountinLink <= 32)
                                                                    if (d.wordCount_AE >= 3)
                                                                        return "menu";
                                                                    else
                                                                        return "empty";
                                                                else
                                                                    if (d.wordCount_AE >= 3)
                                                                        return "links";
                                                                    else
                                                                        return "empty";
                                                        else
                                                            if (d.div_count_AE <= 2)
                                                                if (d.p_count <= 17) return "links";
                                                                else return "empty";
                                                            else
                                                                if (d.sim_bagofword_AE <= -1)
                                                                    if (d.DensityinHTML_AE <= 0.00466) return "links";
                                                                    else return "empty";
                                                                else return "empty";
                                                    else return "empty";
                                                else
                                                    if (d.meanofWordinLinksAllWords <= 0.849057)
                                                        if (d.h1_count <= 0)
                                                            if (d.wordCount_AE <= 2) return "empty";
                                                            else return "empty";
                                                        else return "empty";
                                                    else return "links";
                                            else
                                                if (d.p_count_AE <= 0)
                                                    if (d.repeat_tag_count <= 45) return "menu";
                                                    else return "empty";
                                                else return "others";
                                        else
                                            if (d.LinkCount <= 0) return "menu";
                                            else return "empty";
                                else
                                    if (d.meanofWordinLinks <= 0.871429)
                                        if (d.repeat_tag_count <= 1)
                                            if (d.sim_innerHTML_AE <= 0.00552) return "menu";
                                            else return "others";
                                        else
                                            if (d.dot_count_AE <= 1)
                                                if (d.span_count_AE <= 0)
                                                    if (d.div_count_AE <= 0)
                                                        if (d.wordCount_AE <= 1) return "menu";
                                                        else return "others";
                                                    else
                                                        if (d.LinkCount <= 4) return "links";
                                                        else return "menu";
                                                else return "menu";
                                            else return "others";
                                    else
                                        if (d.sim_innerHTML <= 0.61664)
                                            if (d.sim_innerHTML_AE <= 0.22513) return "empty";
                                            else
                                                if (d.li_count_AE <= 1) return "links";
                                                else return "empty";
                                        else return "main";
                            else
                                if (d.repeat_tag_count <= 14) return "menu";
                                else return "links";
                        else
                            if (d.meanofWordinLinksAllWords <= 0.658071)
                                if (d.input_count <= 0)
                                    if (d.LinkCount_AE <= 1)
                                        if (d.h6_count_AE <= 0)
                                            if (d.sim_innerHTML_AE <= 0.00079)
                                                if (d.span_count_AE <= 3)
                                                    if (d.sim_bagofword <= -1)
                                                        if (d.h2_count_AE <= 0)
                                                            if (d.p_count <= 0)
                                                                if (d.meanofWordinLinksAllWords_AE <= 0.140187)
                                                                    if (d.dot_count_AE <= 1)
                                                                        if (d.repeat_tag_count <= 5)
                                                                            if (d.tagName == "DIV" || d.tagName == "TD")
                                                                                if (d.meanofWordinLinks < 0.3)
                                                                                    return "others";
                                                                                else
                                                                                    if (d.meanofWordinLinks_AE < 0.2)
                                                                                        return "others";
                                                                                    else
                                                                                        return "";
                                                                            else
                                                                                return "";
                                                                        else
                                                                            if (d.wordCount_AE <= 8)
                                                                                if (d.tagName == "DIV" || d.tagName == "TD")
                                                                                    return "others";
                                                                                else
                                                                                    return "";
                                                                            else if (d.tagName == "DIV" || d.tagName == "TD")
                                                                                return "others";
                                                                            else
                                                                                return "";
                                                                    else
                                                                        if (d.repeat_tag_count <= 1)
                                                                            if (d.img_count_AE <= 0)
                                                                                if (d.br_count_AE <= 0)
                                                                                    if (d.dot_count_AE <= 1)
                                                                                        if (d.meanofWordinLinksAllWords > 0.4)
                                                                                            return "links";
                                                                                        else
                                                                                            if (d.wordCount_AE > 36)
                                                                                                return "main";
                                                                                            else
                                                                                                return "others";
                                                                                    else if (d.meanofWordinLinksAllWords > 0.4)
                                                                                        return "links";
                                                                                    else
                                                                                        if (d.wordCount_AE > 36)
                                                                                            return "main";
                                                                                        else
                                                                                            return "others";
                                                                                else if (d.meanofWordinLinksAllWords > 0.4)
                                                                                    return "links";
                                                                                else
                                                                                    if (d.wordCount_AE > 36)
                                                                                        return "main";
                                                                                    else
                                                                                        return "others";
                                                                            else if (d.meanofWordinLinksAllWords > 0.4)
                                                                                return "links";
                                                                            else
                                                                                if (d.wordCount_AE > 36)
                                                                                    return "main";
                                                                                else
                                                                                    return "others";
                                                                        else if (d.meanofWordinLinksAllWords > 0.4)
                                                                            return "links";
                                                                        else
                                                                            if (d.wordCount_AE > 36)
                                                                                return "main";
                                                                            else
                                                                                return "others";
                                                                else
                                                                    if (d.dot_count_AE <= 1)
                                                                        if (d.meanofWordinLinksAllWords_AE < 0.3)
                                                                            if (d.wordCount_AE > 36)
                                                                                return "main";
                                                                            else
                                                                                return "others";
                                                                        else
                                                                            return "links";
                                                                    else return "empty";
                                                            else
                                                                if (d.img_count_AE <= 0)
                                                                    if (d.ul_count_AE <= 0)
                                                                        if (d.wordCount <= 19)
                                                                            if (d.repeat_tag_count <= 31)
                                                                                if (d.repeat_tag_count <= 12)
                                                                                    if (d.wordCount_AE <= 18)
                                                                                        if (d.tagName == "DIV" || d.tagName == "TD")
                                                                                            if (d.meanofWordinLinksAllWords > 0.2)
                                                                                                return "links";
                                                                                            else
                                                                                                return "others";
                                                                                        else
                                                                                            return "";
                                                                                    else
                                                                                        if (d.meanofWordinLinksAllWords_AE > 0.2)
                                                                                            return "links";
                                                                                        else
                                                                                            return "others";
                                                                                else if (d.meanofWordinLinksAllWords_AE > 0.2)
                                                                                    return "links";
                                                                                else
                                                                                    return "others";
                                                                            else if (d.meanofWordinLinksAllWords_AE > 0.2)
                                                                                return "links";
                                                                            else
                                                                                return "others";
                                                                        else
                                                                            if (d.meanofWordinLinksAllWords_AE > 0.20)
                                                                                if (d.LinkCount_AE < 3)
                                                                                    if (d.meanofWordinLinks_AE >= 5)
                                                                                        if (d.h1_count_AE >= 1)
                                                                                            return "others";
                                                                                        else
                                                                                            return "links";
                                                                                    else
                                                                                        return "others";
                                                                                else
                                                                                    return "links";
                                                                            else
                                                                                if (d.wordCount_AE > 40)
                                                                                    return "main";
                                                                                else
                                                                                    return "others";
                                                                    else
                                                                        if (d.meanofWordinLinksAllWords_AE < 0.2)
                                                                            if (d.wordCount_AE > 37)
                                                                                return "main";
                                                                            else
                                                                                return "others";
                                                                        else
                                                                            return "links";
                                                                else return "others";
                                                        else
                                                            if (d.p_count_AE <= 0)
                                                                if (d.LinkCount > 3)
                                                                    if (d.wordCount_AE > 4)
                                                                        return "others";
                                                                    else
                                                                        return "links";
                                                                else
                                                                    if (d.meanofWordinLinksAllWords_AE > 0.15)
                                                                        if (d.LinkCount_AE > 1)
                                                                            return "links";
                                                                        else
                                                                            return "others";
                                                                    else
                                                                        return "others";
                                                            else
                                                                if (d.LinkCount_AE > 3)
                                                                    return "links";
                                                                else
                                                                {
                                                                    if (d.div_count <= 0)
                                                                        return "others";
                                                                    else
                                                                        if (d.wordCount_AE >= 5)
                                                                            return "others";
                                                                        else
                                                                            return "empty";
                                                                }
                                                    else
                                                        if (d.wordCount <= 24)
                                                            if (d.repeat_tag_count <= 3)
                                                                if (d.LinkCount_AE <= 0)
                                                                    if (d.dot_count_AE <= 2)
                                                                        if (d.p_count_AE <= 1) return "others";
                                                                        else return "empty";
                                                                    else return "empty";
                                                                else
                                                                    if (d.dot_count_AE <= 1) return "links";
                                                                    else return "empty";
                                                            else return "links";
                                                        else
                                                            if (d.LinkCount_AE <= 0)
                                                                if (d.wordCount_AE <= 35) return "summary";
                                                                else return "empty";
                                                            else return "menu";
                                                else
                                                    if (d.dot_count_AE <= 1) return "others";
                                                    else
                                                        if (d.wordCount_AE > 5)
                                                            if (d.meanofWordinLinksAllWords_AE < 0.3)
                                                                return "others";
                                                            else
                                                                return "links";
                                                        else
                                                            return "empty";
                                            else
                                                if (d.wordCountinLink_AE <= 3)
                                                    if (d.sim_innerHTML_AE <= 0.5246) return "others";
                                                    else
                                                        if (d.DensityinHTML_AE <= 0.014323)
                                                            if (d.dot_count_AE <= 0)
                                                                if (d.DensityinHTML_AE <= 0.006863)
                                                                    if (d.h1_count_AE <= 0)
                                                                        if (d.p_count_AE <= 0) return "headline";
                                                                        else return "others";
                                                                    else return "others";
                                                                else return "others";
                                                            else return "others";
                                                        else
                                                            if (d.repeat_tag_count <= 4)
                                                                if (d.repeat_tag_count <= 1)
                                                                    if (d.p_count_AE <= 2)
                                                                        if (d.DensityinHTML_AE <= 0.015284) return "main";
                                                                        else return "others";
                                                                    else return "main";
                                                                else return "main";
                                                            else
                                                                if (d.repeat_tag_count <= 28) return "links";
                                                                else return "others";
                                                else
                                                    if (d.DensityinHTML_AE <= 0.021538) return "others";
                                                    else
                                                        if (d.wordCountinLink_AE <= 8) return "links";
                                                        else return "others";
                                        else
                                            if (d.p_count_AE <= 0) return "others";
                                            else return "links";
                                    else
                                        if (d.meanofWordinLinksAllWords <= 0.417699)
                                            if (d.img_count_AE <= 0)
                                                if (d.meanofWordinLinks_AE <= 3.555556)
                                                    if (d.LinkCount_AE <= 7)
                                                        if (d.p_count_AE <= 0)
                                                            if (d.h1_count_AE <= 0)
                                                                if (d.span_count_AE <= 1)
                                                                    if (d.meanofWordinLinks_AE <= 2.5) return "links";
                                                                    else return "empty";
                                                                else return "others";
                                                            else return "others";
                                                        else
                                                            if (d.meanofWordinLinksAllWords_AE >= 0.23)
                                                                if (d.h1_count_AE >= 1)
                                                                    if (d.meanofWordinLinksAllWords_AE < 0.5)
                                                                        return "others";
                                                                    else
                                                                        return "links";
                                                                else
                                                                    return "links";
                                                            else
                                                                return "others";
                                                    else return "menu";
                                                else
                                                    if (d.LinkCount <= 2)
                                                        if (d.meanofWordinLinksAllWords_AE > 0.23)
                                                            return "links";
                                                        else
                                                            if (d.wordCount_AE < 37)
                                                                return "others";
                                                            else
                                                                return "main";
                                                    else return "links";
                                            else
                                                if (d.p_count <= 4)
                                                    if (d.LinkCount_AE > 4)
                                                        if (d.meanofWordinLinksAllWords_AE < 0.65)
                                                            return "links";
                                                        else
                                                            return "menus";
                                                    else
                                                        if (d.meanofWordinLinksAllWords_AE < 0.15)
                                                            return "others";
                                                        else
                                                            return "links";
                                                else
                                                    if (d.LinkCount_AE > 4)
                                                        if (d.meanofWordinLinksAllWords_AE < 0.65)
                                                            return "links";
                                                        else
                                                            return "menus";
                                                    else
                                                        return "others";
                                        else
                                            if (d.br_count_AE <= 0)
                                                if (d.h2_count_AE <= 1) return "menu";
                                                else return "others";
                                            else return "empty";
                                else
                                    if (d.dot_count <= 12)
                                        if (d.ul_count_AE <= 0)
                                            if (d.img_count_AE <= 0) return "empty";
                                            else return "menu";
                                        else return "menu";
                                    else
                                        if (d.meanofWordinLinksAllWords > 0.4)
                                            if (d.meanofWordinLinksAllWords_AE > 0.23)
                                                return "links";
                                            else
                                                return "others";
                                        else
                                            if (d.input_count_AE >= 2)
                                                return "";
                                            else
                                                return "others";
                            else
                                if (d.wordCount_AE <= 4)
                                    return "empty";
                                else if (d.meanofWordinLinks_AE >= 0.3)
                                    return "links";
                                else
                                    if (d.meanofWordinLinksAllWords >= 0.6)
                                        if (d.wordCount_AE > 4)
                                            if (d.meanofWordinLinks > 2.2)
                                                return "links";
                                            else
                                                if (d.wordCountinLink_AE > 10)
                                                    return "menu";
                                                else
                                                    return "others";
                                        else
                                            return "links";
                                    else
                                        return "others";
                    }
                    if (d.tagName == "TD")
                    {
                        if (d.repeat_tag_count <= 11)
                            if (d.repeat_tag_count <= 6)
                                if (d.meanofWordinLinks <= 0.947368)
                                    if (d.input_count <= 1)
                                        if (d.h2_count_AE <= 0)
                                            if (d.img_count_AE <= 0)
                                                if (d.wordCount_AE > 3)
                                                    return "others";
                                                else
                                                    return "links";
                                            else
                                                if (d.p_count_AE <= 0) return "links";
                                                else return "others";
                                        else return "links";
                                    else return "empty";
                                else
                                    if (d.repeat_tag_count <= 4)
                                        if (d.wordCount_AE <= 1) return "empty";
                                        else return "menu";
                                    else return "menu";
                            else
                                if (d.repeat_tag_count <= 9)
                                    if (d.dot_count <= 1) return "menu";
                                    else
                                        if (d.div_count_AE <= 0) return "links";
                                        else return "menu";
                                else return "links";
                        else
                            if (d.repeat_tag_count <= 29)
                                if (d.repeat_tag_count <= 13) return "others";
                                else
                                    if (d.meanofWordinLinks_AE <= 0.05)
                                        if (d.wordCount_AE < 3)
                                            return "empty";
                                        else
                                            return "others";
                                    else
                                        return "menu";
                            else
                                if (d.wordCount_AE < 3)
                                    return "empty";
                                else
                                    if (d.input_count_AE >= 1)
                                        return "empty";
                                    else
                                        return "others";
                    }
                    if (d.tagName == "H1")
                    {
                        if (d.repeat_tag_count <= 5) return "headline";
                        else return "others";
                    }
                    if (d.tagName == "H2")
                    {
                        if (d.dot_count_AE <= 0)
                            if (d.wordCount_AE <= 12)
                                if (d.repeat_tag_count <= 14)
                                    if (d.wordCount_AE <= 1) return "";
                                    else return "headline";
                                else
                                    if (d.wordCount_AE > 18)
                                        return "summary";
                                    else
                                        if (d.wordCount_AE > 2)
                                            return "headline";
                                        else
                                            return "";
                            else
                                if (d.repeat_tag_count <= 3)
                                    if (d.wordCount_AE > 18)
                                        return "summary";
                                    else
                                        if (d.wordCount_AE > 2)
                                            return "headline";
                                        else
                                            return "";
                                else
                                    if (d.wordCount_AE > 18)
                                        return "summary";
                                    else
                                        if (d.wordCount_AE > 2)
                                            return "headline";
                                        else
                                            return "";
                        else
                            if (d.wordCount_AE > 14)
                                return "summary";
                            else
                                if (d.wordCount_AE > 2)
                                    return "headline";
                                else
                                    return "";
                    }
                    if (d.tagName == "H3")
                    {
                        if (d.repeat_tag_count <= 10)
                            if (d.repeat_tag_count <= 1)
                                if (d.span_count_AE <= 0)
                                    if (d.DensityinHTML_AE <= 0.004484) return "";
                                    else
                                        if (d.wordCount_AE <= 16)
                                            return "headline";
                                        else
                                            return "summary";
                                else
                                    if (d.wordCount_AE > 20)
                                        return "summary";
                                    else
                                        if (d.wordCount_AE > 2)
                                            return "headline";
                                        else
                                            return "";
                            else
                                if (d.wordCount_AE > 20)
                                    return "summary";
                                else
                                    if (d.wordCount_AE > 2)
                                        return "headline";
                                    else
                                        return "";
                        else if (d.wordCount_AE > 20)
                            return "summary";
                        else
                            if (d.wordCount_AE > 2)
                                return "headline";
                            else
                                return "";
                    }
                    if (d.tagName == "H4")
                    {
                        if (d.repeat_tag_count <= 10)
                            if (d.repeat_tag_count <= 1)
                                if (d.span_count_AE <= 0)
                                    if (d.DensityinHTML_AE <= 0.004484) return "";
                                    else
                                        if (d.wordCount_AE <= 16)
                                            return "headline";
                                        else
                                            return "summary";
                                else
                                    if (d.wordCount_AE > 20)
                                        return "summary";
                                    else
                                        if (d.wordCount_AE > 2)
                                            return "headline";
                                        else
                                            return "";
                            else
                                if (d.wordCount_AE > 20)
                                    return "summary";
                                else
                                    if (d.wordCount_AE > 2)
                                        return "headline";
                                    else
                                        return "";
                        else if (d.wordCount_AE > 20)
                            return "summary";
                        else
                            if (d.wordCount_AE > 2)
                                return "headline";
                            else
                                return "";
                    }
                    if (d.tagName == "H5") return "";
                    if (d.tagName == "H6") return "";
                    if (d.tagName == "P") return "";
                    if (d.tagName == "SPAN") return "";
                    if (d.tagName == "FONT")
                        if (d.wordCount_AE <= 6)
                            return "";
                        else
                            if (d.wordCount_AE < 16)
                                return "headline";
                            else
                                return "";
                    if (d.tagName == "UL")
                    {
                        if (d.meanofWordinLinksAllWords_AE < 0.3)
                            if (d.wordCount > 14)
                                return "summary";
                            else
                                return "others";
                        else
                            return "links";
                    }
                    if (d.tagName == "DL") return "";
                    if (d.tagName == "LI") return "";
                    if (d.tagName == "STRONG")
                        if (d.wordCount_AE > 10)
                            return "summary";
                        else
                            return "";
                    if (d.tagName == "B")
                    {
                        if (d.wordCount_AE <= 11) { return "headline"; }
                        else return "summary";
                    }

                    if (d.tagName == "OL") { return "links"; }
                }
                else
                    if (d.wordCount_AE <= 68)
                        if (d.img_count_AE <= 0)
                            if (d.input_count <= 0)
                            {
                                if (d.tagName == "DIV")
                                    if (d.wordCount_AE < 70)
                                        if (d.meanofWordinLinks_AE > 5)
                                            return "links";
                                        else
                                            return "others";
                                    else
                                        return "main";
                                if (d.tagName == "TD")
                                    if (d.wordCount_AE < 50)
                                        return "others";
                                    else
                                        return "main";
                                if (d.tagName == "H1")
                                    if (d.wordCount_AE < 22)
                                        return "headline";
                                    else
                                        return "summary";
                                if (d.tagName == "H2")
                                    if (d.wordCount_AE < 12)
                                        return "headline";
                                    else
                                        return "summary";
                                if (d.tagName == "H3")
                                    if (d.wordCount_AE < 12)
                                        return "headline";
                                    else
                                        return "summary";
                                if (d.tagName == "H4")
                                    if (d.wordCount_AE < 12)
                                        return "headline";
                                    else
                                        return "summary";
                                if (d.tagName == "H5")
                                    if (d.wordCount_AE < 12)
                                        return "";
                                    else
                                        return "summary";
                                if (d.tagName == "H6") return "";
                                if (d.tagName == "P") return "";
                                if (d.tagName == "SPAN") return "";
                                if (d.tagName == "FONT") return "";
                                if (d.tagName == "UL") return "";
                                if (d.tagName == "DL") return "";
                                if (d.tagName == "LI") return "";
                                if (d.tagName == "STRONG") return "";
                                if (d.tagName == "B") return "";
                                if (d.tagName == "OL") return "";
                            }
                            else
                                if (d.meanofWordinLinksAllWords_AE < 0.23)
                                    return "others";
                                else
                                    return "links";
                        else
                            if (d.br_count <= 0) return "links";
                            else return "others";
                    else
                        if (d.dot_count_AE <= 2)
                            if (d.br_count_AE <= 8)
                                if (d.LinkCount_AE <= 2)
                                    if (d.wordCount_AE > 37)
                                        return "main";
                                    else
                                        return "others";
                                else return "links";
                            else return "empty";
                        else
                            if (d.img_count_AE <= 7)
                                if (d.repeat_tag_count <= 3)
                                    if (d.wordCount <= 96)
                                        if (d.wordCount_AE <= 91)
                                            if (d.meanofWordinLinksAllWords_AE > 0.3)
                                                return "links";
                                            else
                                                return "main";
                                        else
                                            if (d.wordCount_AE > 30)
                                                return "main";
                                            else
                                                return "others";
                                    else
                                        if (d.meanofWordinLinksAllWords_AE > 0.23)
                                            if (d.li_count_AE > 10)
                                                return "links";
                                            else
                                                return "main";
                                        else
                                            return "main";
                                else
                                    if (d.repeat_tag_count <= 6)
                                        if (d.tagName == "DIV" || d.tagName == "TD")
                                            if (d.meanofWordinLinksAllWords_AE < 0.23)
                                                return "main";
                                            else
                                                return "links";
                                        else
                                            return "";
                                    else
                                        if (d.tagName == "DIV" || d.tagName == "TD")
                                            if (d.wordCount_AE > 55)
                                                return "main";
                                            else
                                                return "others";
                                        else
                                            return "";
                            else
                            {
                                if (d.tagName == "DIV")
                                    if (d.wordCount_AE <= 803)
                                        if (d.meanofWordinLinksAllWords_AE > 0.2)
                                            return "links";
                                        else
                                            if (d.input_count_AE >= 2)
                                                return "empty";
                                            else
                                                return "others";
                                    else if (d.meanofWordinLinksAllWords_AE > 0.2)
                                        return "links";
                                    else
                                        return "main";
                                if (d.tagName == "TD") return "others";
                                if (d.tagName == "H1") return "others";
                                if (d.tagName == "H2") return "others";
                                if (d.tagName == "H3") return "others";
                                if (d.tagName == "H4") return "others";
                                if (d.tagName == "H5") return "others";
                                if (d.tagName == "H6") return "others";
                                if (d.tagName == "P") return "others";
                                if (d.tagName == "SPAN") return "others";
                                if (d.tagName == "FONT") return "others";
                                if (d.tagName == "UL") return "others";
                                if (d.tagName == "DL") return "others";
                                if (d.tagName == "LI") return "others";
                                if (d.tagName == "STRONG") return "others";
                                if (d.tagName == "B") return "others";
                                if (d.tagName == "OL") return "others";
                            }
            }
            else
            {

                if (d.meanofWordinLinks_AE <= 4)
                    if (d.p_count_AE <= 1)
                        if (d.input_count <= 1)
                        {
                            if (d.tagName == "DIV")
                                if (d.meanofWordinLinks <= 2.421053)
                                    if (d.img_count_AE <= 7)
                                        if (d.h1_count_AE <= 0)
                                            if (d.meanofWordinLinksAllWords_AE <= 0.586207)
                                                if (d.sim_bagofword <= 0.57343)
                                                    if (d.h2_count_AE <= 0)
                                                        if (d.br_count <= 10)
                                                            if (d.h5_count_AE <= 0) 
                                                                 return "menu";
                                                            else return "links";
                                                        else return "links";
                                                    else return "links";
                                                else return "others";
                                            else
                                                if (d.br_count_AE <= 1)
                                                    if (d.dot_count_AE <= 1)
                                                        if (d.meanofWordinLinksAllWords_AE <= 2.602273)
                                                            if (d.h2_count_AE <= 0) return "menu";
                                                            else
                                                                if (d.repeat_tag_count <= 7)
                                                                    if (d.meanofWordinLinks_AE <= 2.3) return "menu";
                                                                    else
                                                                        if (d.wordCount_AE <= 6) return "menu";
                                                                        else return "links";
                                                                else return "links";
                                                        else
                                                            if (d.meanofWordinLinks_AE <= 1.222222)
                                                                if (d.wordCount_AE <= 1) return "menu";
                                                                else return "links";
                                                            else return "menu";
                                                    else
                                                        if (d.DensityinHTML_AE <= 0.000629) return "links";
                                                        else
                                                            if (d.li_count_AE <= 4)
                                                                if (d.wordCountinLink_AE <= 12)
                                                                    if (d.wordCount <= 16) return "menu";
                                                                    else return "links";
                                                                else
                                                                    if (d.span_count_AE <= 0) return "menu";
                                                                    else return "links";
                                                            else return "menu";
                                                else
                                                    if (d.LinkCount_AE <= 5) return "links";
                                                    else
                                                        if (d.meanofWordinLinks_AE <= 1.153846) return "links";
                                                        else return "menu";
                                        else
                                            if (d.DensityinHTML_AE <= 0.002045) return "empty";
                                            else
                                                if (d.meanofWordinLinksAllWords_AE <= 0.738095) return "others";
                                                else return "menu";
                                    else
                                        if (d.LinkCount_AE <= 23) return "menu";
                                        else return "links";
                                else
                                    if (d.wordCountinLink_AE <= 2)
                                        if (d.img_count_AE <= 0)
                                            if (d.h2_count <= 0) return "empty";
                                            else return "menu";
                                        else return "links";
                                    else
                                        if (d.div_count_AE <= 7)
                                            if (d.img_count_AE <= 0)
                                                if (d.sim_innerHTML <= 0.08851)
                                                    if (d.meanofWordinLinksAllWords <= 1.035714)
                                                        if (d.repeat_tag_count <= 9)
                                                            if (d.sim_bagofword_AE <= 0.01501)
                                                                if (d.sim_innerHTML <= 0.00347)
                                                                    if (d.span_count_AE <= 3)
                                                                        if (d.meanofWordinLinks_AE <= 2.891304)
                                                                            if (d.repeat_tag_count <= 7) return "menu";
                                                                            else
                                                                                if (d.wordCount_AE <= 103) return "menu";
                                                                                else return "links";
                                                                        else
                                                                            if (d.br_count_AE <= 3)
                                                                                if (d.span_count <= 1)
                                                                                    if (d.meanofWordinLinks <= 4.541667)
                                                                                        if (d.wordCount_AE <= 15)
                                                                                            if (d.h2_count_AE <= 0)
                                                                                                if (d.wordCount_AE <= 7)
                                                                                                    if (d.span_count_AE <= 0)
                                                                                                        if (d.wordCount_AE <= 4) return "menu";
                                                                                                        else return "links";
                                                                                                    else return "links";
                                                                                                else return "menu";
                                                                                            else return "links";
                                                                                        else return "menu";
                                                                                    else
                                                                                        if (d.meanofWordinLinks_AE <= 3.5) return "links";
                                                                                        else return "empty";
                                                                                else return "menu";
                                                                            else return "links";
                                                                    else
                                                                        if (d.meanofWordinLinks_AE <= 3.555556)
                                                                            if (d.span_count_AE <= 5) 
                                                                                if(d.meanofWordinLinksAllWords_AE >= 0.23)
                                                                                    return "links";
                                                                                else
                                                                                    return "others";
                                                                            else return "menu";
                                                                        else return "links";
                                                                else return "links";
                                                            else
                                                                if (d.sim_innerHTML <= 0.02996) return "others";
                                                                else return "menu";
                                                        else
                                                            if (d.ul_count_AE <= 1)
                                                                if (d.span_count_AE <= 3) return "links";
                                                                else return "others";
                                                            else return "menu";
                                                    else
                                                        if (d.meanofWordinLinks_AE <= 3.222222) return "menu";
                                                        else
                                                            if (d.span_count_AE <= 4)
                                                                if (d.span_count_AE <= 0)
                                                                    if (d.repeat_tag_count <= 22)
                                                                        if (d.repeat_tag_count <= 14)
                                                                            if (d.repeat_tag_count <= 7) return "menu";
                                                                            else
                                                                                if (d.sim_bagofword <= -1) return "links";
                                                                                else return "menu";
                                                                        else return "menu";
                                                                    else return "links";
                                                                else
                                                                    if (d.div_count_AE <= 1)
                                                                        if (d.DensityinHTML_AE <= 0.001299) return "menu";
                                                                        else return "links";
                                                                    else return "menu";
                                                            else return "menu";
                                                else
                                                    if (d.meanofWordinLinks_AE <= 3.25)
                                                        if (d.repeat_tag_count <= 10) return "menu";
                                                        else return "links";
                                                    else return "links";
                                            else
                                                if (d.LinkCount_AE <= 1) return "menu";
                                                else
                                                    if (d.br_count_AE <= 0)
                                                        if (d.meanofWordinLinksAllWords_AE <= 1.007813)
                                                            if (d.meanofWordinLinks_AE <= 3.064516)
                                                                if (d.repeat_tag_count <= 1) return "menu";
                                                                else return "links";
                                                            else return "links";
                                                        else return "links";
                                                    else
                                                        if (d.img_count_AE <= 2) return "menu";
                                                        else return "links";
                                        else
                                            if (d.wordCount_AE <= 74) return "menu";
                                            else
                                                if (d.span_count_AE <= 16) return "links";
                                                else
                                                    if (d.meanofWordinLinksAllWords_AE > 0.23)
                                                        return "links";
                                                    else
                                                        return "others";
                            if (d.tagName == "TD")
                                if (d.repeat_tag_count <= 29)
                                    if (d.span_count_AE <= 0)
                                        if (d.meanofWordinLinksAllWords_AE <= 0.677083)
                                            if (d.repeat_tag_count <= 15)
                                                if (d.br_count_AE <= 0) return "menu";
                                                else return "links";
                                            else return "menu";
                                        else
                                            if (d.ul_count_AE <= 0)
                                                if (d.dot_count_AE <= 3) return "menu";
                                                else
                                                    if (d.wordCount_AE <= 5) return "links";
                                                    else return "menu";
                                            else
                                                if (d.LinkCount_AE <= 12) return "links";
                                                else return "menu";
                                    else
                                        if (d.LinkCount_AE <= 13) return "links";
                                        else return "menu";
                                else
                                    if (d.meanofWordinLinksAllWords_AE > 0.23)
                                        return "links";
                                    else
                                        return "others";
                            if (d.tagName == "H1") return "headline";
                            if (d.tagName == "H2") return "";
                            if (d.tagName == "H3") return "";
                            if (d.tagName == "H4") return "";
                            if (d.tagName == "H5") return "";
                            if (d.tagName == "H6") return "";
                            if (d.tagName == "P") return "";
                            if (d.tagName == "SPAN") return "";
                            if (d.tagName == "FONT") return "";
                            if (d.tagName == "UL")
                                if (d.LinkCount_AE <= 1)
                                    if (d.li_count_AE <= 2) return "menu";
                                    else return "menu";
                                else
                                    if (d.meanofWordinLinks_AE <= 3.7) return "menu";
                                    else
                                        if (d.meanofWordinLinks_AE <= 3.9) return "links";
                                        else return "menu";
                            if (d.tagName == "DL") return "";
                            if (d.tagName == "LI") return "";
                            if (d.tagName == "STRONG") return "";
                            if (d.tagName == "B") return "";
                            if (d.tagName == "OL") return "";
                        }
                        else
                            if (d.br_count_AE <= 2)
                                if (d.wordCountinLink_AE <= 3)
                                    if (d.span_count <= 1)
                                        if (d.wordCount_AE <= 2) return "empty";
                                        else return "menu";
                                    else return "empty";
                                else
                                    if (d.dot_count_AE <= 11) return "menu";
                                    else
                                        if (d.img_count_AE <= 0) return "menu";
                                        else return "links";
                            else 
                                if(d.dot_count > 10)
                                    return "others";
                                else
                                    return "empty";
                    else
                        if (d.span_count <= 14)
                            if (d.meanofWordinLinks_AE <= 2.454545)
                                if (d.meanofWordinLinks_AE <= 1.527273) return "menu";
                                else return "menu";
                            else
                                if (d.br_count_AE <= 0)
                                    if (d.h2_count_AE <= 3)
                                        if (d.img_count_AE <= 0)
                                            if (d.span_count_AE <= 1)
                                                if (d.DensityinHTML <= 0.013597)
                                                    if (d.repeat_tag_count <= 4) return "menu";
                                                    else return "links";
                                                else return "links";
                                            else return "others";
                                        else return "links";
                                    else return "menu";
                                else return "menu";
                        else
                            if (d.h1_count_AE <= 0)
                                return "empty";
                            else
                                if (d.wordCount_AE > 5)
                                    return "others";
                                else
                                    return "empty";
                else
                {
                    if (d.tagName == "DIV")
                        if (d.img_count <= 13)
                            if (d.DensityinHTML_AE <= 0.00249)
                                if (d.repeat_tag_count <= 3) return "menu";
                                else
                                    if (d.meanofWordinLinks <= 6.5)
                                        if (d.DensityinHTML_AE <= 0.001377) return "links";
                                        else return "links";
                                    else return "links";
                            else
                                if (d.wordCountinLink_AE <= 5)
                                    if (d.wordCount_AE <= 8)
                                        if (d.img_count <= 1)
                                            if (d.h1_count_AE <= 0)
                                                if (d.LinkCount <= 4) return "links";
                                                else
                                                    if (d.dot_count <= 8) 
                                                        return "links";
                                                    else
                                                        if(d.meanofWordinLinksAllWords_AE > 0.3)
                                                            return "links";
                                                        else
                                                            return "others";
                                            else return "menu";
                                        else return "menu";
                                    else return "empty";
                                else
                                    if (d.meanofWordinLinks_AE <= 5)
                                        if (d.meanofWordinLinksAllWords <= 0.271429) return "menu";
                                        else
                                            if (d.img_count <= 0)
                                                if (d.DensityinHTML_AE <= 0.012587)
                                                    if (d.ul_count_AE <= 0) return "links";
                                                    else
                                                        if (d.div_count_AE <= 0) return "menu";
                                                        else return "links";
                                                else return "links";
                                            else return "links";
                                    else
                                        if (d.br_count <= 4)
                                            if (d.meanofWordinLinksAllWords_AE <= 0.670732)
                                                if (d.p_count <= 2)
                                                    if (d.DensityinHTML_AE <= 0.004913)
                                                        if (d.meanofWordinLinks_AE <= 2)
                                                            return "others";
                                                        else
                                                            return "links";
                                                    else return "links";
                                                else 
                                                    if(d.LinkCount_AE <= 2)
                                                        if(d.meanofWordinLinks_AE >= 5)
                                                            if(d.h1_count_AE >= 1)
                                                                return "others";
                                                            else
                                                                return "links";
                                                        else
                                                            return "others";
                                                    else                                                             
                                                        return "links";
                                            else
                                                if (d.img_count_AE <= 1)
                                                    return "links";
                                                else
                                                    if (d.sim_bagofword <= 0.94534) return "links";
                                                    else return "menu";
                                        else
                                            if (d.LinkCount_AE <= 9) return "links";
                                            else return "menu";
                        else
                            if (d.input_count_AE <= 1)
                                if (d.ul_count_AE <= 7)
                                    if (d.meanofWordinLinksAllWords_AE > 0.5)
                                        return "links";
                                    else
                                        return "others";
                                    else return "links";
                                else return "menu";
                    if (d.tagName == "TD")
                        if (d.repeat_tag_count <= 28)
                            if (d.dot_count_AE <= 1)
                                if (d.img_count_AE <= 0)
                                    if (d.dot_count_AE <= 0)
                                        if (d.wordCount_AE <= 6) if (d.meanofWordinLinksAllWords_AE > 0.2)
                                                return "links";
                                            else
                                                return "others";
                                        else if (d.meanofWordinLinksAllWords_AE > 0.2)
                                            return "menu";
                                        else
                                            return "others";
                                    else if (d.meanofWordinLinksAllWords_AE > 0.2)
                                        return "menu";
                                    else
                                        return "others";
                                else if (d.meanofWordinLinksAllWords_AE > 0.2)
                                    return "menu";
                                else
                                    return "others";
                            else
                                if (d.img_count_AE <= 0) return "links";
                                else
                                    if (d.wordCount_AE <= 195)
                                        if(d.meanofWordinLinksAllWords_AE > 0.2)
                                            return "links";
                                        else
                                            return "others";
                                    else 
                                        if (d.meanofWordinLinksAllWords_AE > 0.2)
                                        return "links";
                                    else
                                        return "others";
                        else 
                            if(d.meanofWordinLinksAllWords_AE > 0.2)
                                            return "links";
                                        else
                                            return "others";
                    if (d.tagName == "H1") return "headline";
                    if (d.tagName == "H2")
                        if (d.DensityinHTML_AE <= 0.011876) return "links";
                        else return "headline";
                    if (d.tagName == "H3") return "links";
                    if (d.tagName == "H4") return "links";
                    if (d.tagName == "H5") return "links";
                    if (d.tagName == "H6") return "links";
                    if (d.tagName == "P") return "links";
                    if (d.tagName == "SPAN") return "links";
                    if (d.tagName == "FONT") return "links";
                    if (d.tagName == "UL")
                        if (d.wordCount_AE <= 15)
                            if (d.img_count_AE <= 1) return "links";
                            else return "menu";
                        else return "links";
                    if (d.tagName == "DL") return "links";
                    if (d.tagName == "LI") return "links";
                    if (d.tagName == "STRONG") return "links";
                    if (d.tagName == "B") return "links";
                    if (d.tagName == "OL") return "links";
                }
            }

            return "";
        }
    }//class
}//namespace
