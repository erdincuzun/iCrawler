using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTMLMarkerClass
{
    public class similarity
    {
        //1. method: cossine similarity
        public static double Cossine_Similarity(string str1, string str2)
        {
            str1 = str1.Trim();
            str2 = str2.Trim();
            str1 = RemoveUnnecassaryCharacters(str1);
            str2 = RemoveUnnecassaryCharacters(str2);
            string[] words1 = str1.Split(' ');
            string[] words2 = str2.Split(' ');
            int count_sim_words = CountofSimilarWords(words1, words2);
            int xj = words1.Length;
            int xi = count_sim_words;
            int yi = words2.Length;
            int yj = count_sim_words;
            double _r = (xi * xj + yi * yj) / (Math.Sqrt(xi * xi + yi * yi) * Math.Sqrt(xj * xj + yj * yj));
            _r = 1 - _r;
            if (_r < 0)
                _r = 0;
            return (double)_r;
        }

        //2. method: jaccard similarity
        public static double Jaccard_Similarity(string str1, string str2)
        {
            str1 = RemoveUnnecassaryCharacters(str1);
            str2 = RemoveUnnecassaryCharacters(str2);
            string[] words1 = str1.Split(' ');
            string[] words2 = str2.Split(' ');
            int count_sim_words = CountofSimilarWords(words1, words2);
            double _r = (double)count_sim_words / (words1.Length + words2.Length - count_sim_words);
            _r = 1 - _r;
            if (_r < 0)
                _r = 0;
            return _r;
        }

        //3. method: dice similarity
        public static double Dice_Similarity(string str1, string str2)
        {
            str1 = RemoveUnnecassaryCharacters(str1);
            str2 = RemoveUnnecassaryCharacters(str2);
            string[] words1 = str1.Split(' ');
            string[] words2 = str2.Split(' ');
            int count_sim_words = CountofSimilarWords(words1, words2);
            double _r = (double)(2 * count_sim_words) / (words1.Length + words2.Length);
            _r = 1 - _r;
            if (_r < 0)
                _r = 0;
            return _r;
        }



        //count of similar words
        private static int CountofSimilarWords(string[] words1, string[] words2)
        {
            int count_sim_word = 0;
            for (int i = 0; i < words1.Length; i++)
            {
                for (int j = 0; j < words2.Length; j++)
                {
                    if (words1[i] == words2[j])
                    {
                        words1[i] = "-";
                        words2[j] = "-";
                        count_sim_word++;
                        break;
                    }
                }
            }

            return count_sim_word;
        }

        //Remove Special Characters
        private static string RemoveUnnecassaryCharacters(string strOutput)
        {
            strOutput = strOutput.Replace("\n", " ");
            strOutput = strOutput.Replace("\r", " ");
            strOutput = strOutput.Replace("&nbsp;", " ");
            strOutput = strOutput.Replace("\"", " ");
            strOutput = strOutput.Replace("\\", " ");
            strOutput = strOutput.Replace("`", " ");
            strOutput = strOutput.Replace("’", " ");

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
            strOutput = strOutput.Replace("š", " ");


            System.Text.RegularExpressions.Regex objRegExp2 = new System.Text.RegularExpressions.Regex(@"\s+");

            strOutput = objRegExp2.Replace(strOutput, " ");

            return strOutput;
        }
    }
}
