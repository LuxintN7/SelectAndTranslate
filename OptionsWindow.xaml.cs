using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SelectAndTranslate
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private static WebTranslator.Languages languageFrom, languageTo;
        private static bool googleIsChosen, bingIsChosen, yandexIsChosen;

        static OptionsWindow()
        {
            languageFrom = WebTranslator.DefaultLanguageFrom;
            languageTo = WebTranslator.DefaultLanguageTo;           

            googleIsChosen = bingIsChosen = yandexIsChosen = true;
            
            initializeWebTranslators();
        }

        public OptionsWindow()
        {
            InitializeComponent();            
            
            setAvailableLanguages();
            checkChosenTranslators();
            btnApply.IsEnabled = false;
        }

        private void setAvailableLanguages()
        {
            cmbLanguagesFrom.ItemsSource = Enum.GetValues(typeof(WebTranslator.Languages));                            
            cmbLanguagesFrom.SelectedItem = languageFrom;

            cmbLanguagesTo.ItemsSource = Enum.GetValues(typeof(WebTranslator.Languages));                
            cmbLanguagesTo.SelectedItem = languageTo;
        }
        
        private static void initializeWebTranslators()
        {
            var translators = new List<WebTranslator>();

            if (googleIsChosen) translators.Add(new GoogleTranslator());
            if (bingIsChosen) translators.Add(new BingTranslator());
            if (yandexIsChosen) translators.Add(new YandexTranslator());

            foreach (var translator in translators)
                translator.SetLanguages(languageFrom, languageTo);

            TranslationWindow.Translators = translators;
        }

        private void setLanguages()
        {
            languageFrom = (WebTranslator.Languages)cmbLanguagesFrom.SelectedItem;
            languageTo = (WebTranslator.Languages)cmbLanguagesTo.SelectedItem;
        }

        private void checkChosenTranslators()
        {
            chbGoogleTranslator.IsChecked = googleIsChosen;
            chbBingTranslator.IsChecked = bingIsChosen;
            chbYandexTranslator.IsChecked = yandexIsChosen;
        }

        private void setChosenTranslators()
        {
            googleIsChosen = (bool)chbGoogleTranslator.IsChecked;
            bingIsChosen = (bool)chbBingTranslator.IsChecked;
            yandexIsChosen = (bool)chbYandexTranslator.IsChecked;
        }

        private void applyOptions()
        {
            setLanguages();
            setChosenTranslators(); 
            initializeWebTranslators();           
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            applyOptions();
            (sender as Button).IsEnabled = false;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChangesPerformed(object sender, EventArgs e)
        {
            btnApply.IsEnabled = true;
        }


    }
}
