using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace HTMLMarkerClass
{
    public class domsim
    {
        //ref'lerle gönderilen değerleri dom update edecek
        public static void prepareDOMSim(ref DOM _dom1, ref DOM _dom2)
        {
            for (int i = 0; i < _dom1._list.Count; i++)
			{
                for (int j = i; j < _dom2._list.Count; j++)
                {
                    element _element1 = (element)_dom1._list[i];
                    element _element2 = (element)_dom2._list[j];

                    if (_element1.tagName == "DIV" && _element2.tagName == "DIV")
                    {
                        element _element_sub1 = new element();
                        element _element_sub2 = new element();
                        if (_element1.elementlinked_id >= 0 && _element2.elementlinked_id >= 0)
                        {
                            _element_sub1 = (element)_dom1._list[_element1.elementlinked_id];
                            _element_sub2 = (element)_dom2._list[_element2.elementlinked_id];
                        }

                        if (_element1.tag_class_Name != "" || _element1.tag_id_Name != "")
                            if (_element1.tagName == _element2.tagName
                                && _element1.tag_id_Name == _element2.tag_id_Name
                                && _element1.tag_class_Name == _element2.tag_class_Name
                                && _element_sub1.tagName == _element_sub2.tagName
                                && _element_sub1.tag_id_Name == _element_sub2.tag_id_Name
                                && _element_sub1.tag_class_Name == _element_sub2.tag_class_Name)
                            {
                                _element1.sim_bagofword = Math.Round(similarity.Cossine_Similarity(_element1.BagofWords, _element2.BagofWords), 5);
                                _element1.sim_bagofword_AE = Math.Round(similarity.Cossine_Similarity(_element1.BagofWords_AE, _element2.BagofWords_AE), 5);
                                _element1.sim_innerHTML = Math.Round(similarity.Cossine_Similarity(_element1.innerHTML, _element2.innerHTML), 5);
                                _element1.sim_innerHTML_AE = Math.Round(similarity.Cossine_Similarity(_element1.innerHTML_AE, _element2.innerHTML_AE), 5);

                                _element2.sim_bagofword = _element1.sim_bagofword;
                                _element2.sim_bagofword_AE = _element1.sim_bagofword_AE;
                                _element2.sim_innerHTML = _element1.sim_innerHTML;
                                _element2.sim_innerHTML_AE = _element1.sim_innerHTML_AE;

                                _dom1._list[i] = _element1;
                                _dom2._list[j] = _element2;
                                break;
                            }
                    }//if div
                }//for2
			}//for1
        }

    }
}
