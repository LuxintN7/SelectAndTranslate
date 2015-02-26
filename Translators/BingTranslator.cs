using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace SelectAndTranslate
{
    public partial class BingTranslator : WebTranslator
    {
        public BingTranslator()
            : base() { }

        public BingTranslator(Languages languageFrom, Languages languageTo)
            : base(languageFrom, languageTo) { }

        protected override Uri BuildRequestURI()
        {
            return new Uri(String.Format("http://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&from={1}&to={2}",
                GetEncodedText(), LanguageFrom, LanguageTo));
        }

        private WebResponse getResponse()
        {   
            //  SOURCE: http://msdn.microsoft.com/en-us/library/ff512387.aspx  
 
            MicrosoftTranslatorSdk.HttpSamples.AdmAuthentication admAuthentification
                = new MicrosoftTranslatorSdk.HttpSamples.AdmAuthentication("SelectAndTranslate", "CoYgzppR7c0Ta4V0JStc5Ag1vAyM4inhS2t/yRE3KeY=");

            MicrosoftTranslatorSdk.HttpSamples.AdmAccessToken admAccessToken = admAuthentification.GetAccessToken();

            string authToken = "Bearer" + " " + admAccessToken.access_token; 

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BuildRequestURI());
            request.Headers.Add("Authorization", authToken);           

            return request.GetResponse();
        }

        public override string Translate(string text) 
        {
            this.Text = text;
            
            string translation = "nothing yet...";

            try
            {
                using (Stream stream = getResponse().GetResponseStream())
                {
                    System.Runtime.Serialization.DataContractSerializer dataContractSerializer
                        = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    translation = (string)dataContractSerializer.ReadObject(stream);
                }

            }
            catch (WebException e)
            {
                translation = e.Message;
            }
            return translation;
        }
    }
}
