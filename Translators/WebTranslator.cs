//
// Abstract superclass                          
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SelectAndTranslate
{    
    public abstract class WebTranslator
    {
        public enum Languages
        {
            DE,
            EN,
            FR,
            PL,
            RU,
            UK
        }

        public static Languages DefaultLanguageFrom = Languages.EN;
        public static Languages DefaultLanguageTo = Languages.UK;

        protected string Text;
        protected Languages LanguageFrom;
        protected Languages LanguageTo;


        public WebTranslator()
        {
            LanguageFrom = DefaultLanguageFrom;
            LanguageTo = DefaultLanguageTo;
        }

        public WebTranslator(Languages langFrom, Languages langTo)
        {
            SetLanguages(langFrom, langTo);
        }

        public void SetLanguages(Languages langFrom, Languages langTo)
        {
            LanguageFrom = langFrom;
            LanguageTo = langTo;
        }

        public string GetEncodedText(string text)
        {
            return HttpUtility.UrlEncode(text);
        }

        protected string GetEncodedText()
        {
            return GetEncodedText(this.Text);
        }

        public override string ToString()
        {
            return base.ToString().Substring("SelectAndTrnslate.".Length + 1);
        }

        public abstract string Translate(string text);
        protected abstract Uri BuildRequestURI();        
    }
}
