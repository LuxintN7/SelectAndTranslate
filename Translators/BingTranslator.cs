using System;
using System.Net;
using System.IO;
using System.Web;

namespace SelectAndTranslate
{
    public class BingTranslator : Translator
    {
        private const string ClientSecret = "CoYgzppR7c0Ta4V0JStc5Ag1vAyM4inhS2t/yRE3KeY=";

        public BingTranslator()
            : base() { }

        public BingTranslator(Language languageFrom, Language languageTo)
            : base(languageFrom, languageTo) { }

        protected Uri BuildRequestURI(string text)
        {
            return new Uri(String.Format("http://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&from={1}&to={2}",
                HttpUtility.UrlEncode(text), LanguageFrom, LanguageTo));
        }

        private WebResponse GetResponse(string text)
        {   
            //  SOURCE: http://msdn.microsoft.com/en-us/library/ff512387.aspx  
 
            MicrosoftTranslatorSdk.HttpSamples.AdmAuthentication admAuthentification
                = new MicrosoftTranslatorSdk.HttpSamples.AdmAuthentication("SelectAndTranslate", ClientSecret);

            MicrosoftTranslatorSdk.HttpSamples.AdmAccessToken admAccessToken = admAuthentification.GetAccessToken();

            string authToken = "Bearer" + " " + admAccessToken.access_token; 

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BuildRequestURI(text));
            request.Headers.Add("Authorization", authToken);           

            return request.GetResponse();
        }

        public override string Translate(string text) 
        {        
            string result = "nothing yet...";

            Stream stream = null;

            try
            {
                using (stream = GetResponse(text).GetResponseStream())
                {
                    var dataContractSerializer 
                        = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    result = (string) dataContractSerializer.ReadObject(stream);
                }
            }
            catch (WebException e)
            {
                result = e.Message;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            return result;
        }
    }
}
