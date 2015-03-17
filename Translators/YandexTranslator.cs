using System;
using System.Text;
using System.Net;
using System.Xml;

namespace SelectAndTranslate
{
    public class YandexTranslator : Translator
    {
        private const string Key = "trnsl.1.1.20141229T202549Z.5f61901044d9ab3e.4d5c2d268897918f1adbfa15eb58b66d970ecbef";

        public YandexTranslator()
            : base() { }

        public YandexTranslator(Language languageFrom, Language languageTo)
            : base(languageFrom, languageTo) { }

        protected Uri BuildRequestURI(string text)
        {
            return new Uri(String.Format("https://translate.yandex.net/api/v1.5/tr/translate?key={0}&lang={1}-{2}&text={3}",
                Key, LanguageFrom, LanguageTo, text));            
        }

        private string DownloadResult(string text)
        {
            try
            {
                return (new WebClient()).DownloadString(BuildRequestURI(text));
            }
            catch (WebException e)
            {
                return e.Message;
            }
        }

        public override string Translate(string text)
        {         
            string translation = "nothing yet...";
            XmlDocument xmlDocumet = new XmlDocument();

            string y = DownloadResult(text);
            byte[] bytes = Encoding.Default.GetBytes(y);
            translation = Encoding.UTF8.GetString(bytes);
            xmlDocumet.LoadXml(translation);
            translation = xmlDocumet.GetElementsByTagName("text")[0].InnerText;

            return translation;
        }
    }
}
