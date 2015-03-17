using System;
using System.Diagnostics;
using System.Net;
using System.Web;

namespace SelectAndTranslate
{
    public class GoogleTranslator : Translator
    {     
        public GoogleTranslator()
            : base() { } 

        public GoogleTranslator(Language languageFrom, Language languageTo)
            : base(languageFrom, languageTo) { }

        protected Uri BuildRequestURI(string text)
        {
            return new Uri(String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}|{2}",
                HttpUtility.UrlEncode(text), LanguageFrom, LanguageTo));
        }

        string GetResultPage(string text)
        {
            WebClient client = new WebClient();

            try
            {
                return client.DownloadString(BuildRequestURI(text));
            }
            finally
            {
                client.Dispose();
            }
        }

        
        public override string Translate(string text)
        {
            string result = "nothing yet...";
            string resultPage = GetResultPage(text);

            const string anchor = "TRANSLATED_TEXT='";

            try
            {
                result = resultPage.Substring(resultPage.IndexOf(anchor) + anchor.Length);
                result = result.Substring(0, result.IndexOf("';INPUT_TOOL_PATH="));
                result = result.Replace(@"\x26quot;", "\"") // "
                    .Replace(@"\x26#39;", "'") // '
                    .Replace(@"\x26amp;", "&") // &
                    .Replace(@"\x26gt;", ">")  // >
                    .Replace(@"\x26lt;", "<")  // <
                    .Replace(@"\x3d", "=")     // =
                    .Replace(@"\r\x3cbr\x3e", "\r"); // new line
            }
            catch (Exception e)
            {
                result = e.Message;
            }

            return result;
        }
    }
}
