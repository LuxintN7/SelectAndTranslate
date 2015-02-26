using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace SelectAndTranslate
{
    public class GoogleTranslator : WebTranslator
    {     
        public GoogleTranslator()
            : base() { } 

        public GoogleTranslator(Languages languageFrom, Languages languageTo)
            : base(languageFrom, languageTo) { }

        protected override Uri BuildRequestURI()
        {
            return new Uri(String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}|{2}", 
                GetEncodedText(), LanguageFrom, LanguageTo));
        }

        string getResultPage()
        {
            try
            {
                return (new WebClient()).DownloadString(BuildRequestURI());
            }
            catch (Exception e)
            {      
                Debug.WriteLine("---Ex:" + e.Message);
                return "EXEPTION";
            }
        }

        
        public override string Translate(string text)
        {
            this.Text = text;

            string translation = "...nothing...";
            string resultPage = getResultPage();

            string anchor = "TRANSLATED_TEXT='";

            try
            {
                translation = resultPage.Substring(resultPage.IndexOf(anchor) + anchor.Length);
                translation = translation.Substring(0, translation.IndexOf("';INPUT_TOOL_PATH="));
                translation = translation.Replace(@"\x26quot;", "\"") // "
                    .Replace(@"\x26#39;", "'") // '
                    .Replace(@"\x26amp;", "&") // &
                    .Replace(@"\x26gt;", ">")  // >
                    .Replace(@"\x26lt;", "<")  // <
                    .Replace(@"\x3d", "=")     // =
                    .Replace(@"\r\x3cbr\x3e", "\r"); // new line
            }
            catch (Exception e)
            {
                translation = e.Message;
            }

            return translation;
        }
    }
}
