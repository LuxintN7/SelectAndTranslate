//
// Abstract superclass                          
//

using System;
using System.Threading.Tasks;
using System.Web;

namespace SelectAndTranslate
{    
    public abstract class Translator
    {
        public enum Language
        {
            DE,
            EN,
            FR,
            PL,
            RU,
            UK
        }

        public static Language DefaultLanguageFrom = Language.EN;
        public static Language DefaultLanguageTo = Language.UK;

        protected Language LanguageFrom;
        protected Language LanguageTo;

        public Translator()
        {
            LanguageFrom = DefaultLanguageFrom;
            LanguageTo = DefaultLanguageTo;
        }

        public Translator(Language languageFrom, Language languageTo)
        {
            SetLanguages(languageFrom, languageTo);
        }

        public void SetLanguages(Language languageFrom, Language languageTo)
        {
            LanguageFrom = languageFrom;
            LanguageTo = languageTo;
        }

        public abstract string Translate(string text);

        public Task<string> TranslateAsync(string text)
        {
            return Task.Run(() => Translate(text));
        }
    }
}
