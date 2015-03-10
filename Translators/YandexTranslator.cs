using System;
using System.Text;
using System.Net;
using System.Xml;

namespace SelectAndTranslate
{
    public class YandexTranslator : WebTranslator
    {
        string key = "trnsl.1.1.20141229T202549Z.5f61901044d9ab3e.4d5c2d268897918f1adbfa15eb58b66d970ecbef";

        public YandexTranslator()
            : base() { }

        public YandexTranslator(string key)
            : base() { SetKey(key); }

        public YandexTranslator(Languages languageFrom, Languages languageTo)
            : base(languageFrom, languageTo) { }
        
        public YandexTranslator(string key, Languages languageFrom, Languages languageTo)
            : base(languageFrom, languageTo) { SetKey(key); }

        public void SetKey(string key)
        {
            this.key = key;
        }

        protected override Uri BuildRequestURI()
        {
            return new Uri(String.Format("https://translate.yandex.net/api/v1.5/tr/translate?key={0}&lang={1}-{2}&text={3}",
                key, LanguageFrom, LanguageTo, Text));            
        }

        string downloadResult()
        {
            try
            {
                return (new WebClient()).DownloadString(BuildRequestURI());
            }
            catch (WebException e)
            {
                return e.Message;
            }
        }

        public override string Translate(string text)
        {         
            string translation = "nothing yet...";
            WebClient webClient = new WebClient();            
            XmlDocument xmlDocumet = new XmlDocument();

            this.Text = text;

            string y = downloadResult();
            byte[] bytes = Encoding.Default.GetBytes(y);
            translation = Encoding.UTF8.GetString(bytes);
            xmlDocumet.LoadXml(translation);
            translation = xmlDocumet.GetElementsByTagName("text")[0].InnerText;

            return translation;
        }
    }
}
