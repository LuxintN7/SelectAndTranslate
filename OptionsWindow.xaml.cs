using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace SelectAndTranslate
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private static Translator.Language languageFrom, languageTo;
        private static bool googleIsChosen, bingIsChosen, yandexIsChosen;
        public bool IsClosed = false;

        static OptionsWindow()
        {
            languageFrom = Translator.DefaultLanguageFrom;
            languageTo = Translator.DefaultLanguageTo;           

            googleIsChosen = bingIsChosen = yandexIsChosen = true;
            
            InitializeWebTranslators();
        }

        public OptionsWindow()
        {
            InitializeComponent();            

            SetAvailableLanguages();
            CheckChosenTranslators();
            Apply.IsEnabled = false;
        }

        private void SetAvailableLanguages()
        {
            LanguagesFrom.ItemsSource = Enum.GetValues(typeof(Translator.Language));                            
            LanguagesFrom.SelectedItem = languageFrom;

            LanguagesTo.ItemsSource = Enum.GetValues(typeof(Translator.Language));                
            LanguagesTo.SelectedItem = languageTo;
        }
        
        private static void InitializeWebTranslators()
        {
            var translators = new List<Translator>();

            if (googleIsChosen) translators.Add(new GoogleTranslator());
            if (bingIsChosen) translators.Add(new BingTranslator());
            if (yandexIsChosen) translators.Add(new YandexTranslator());

            foreach (var translator in translators)
                translator.SetLanguages(languageFrom, languageTo);

            TranslationWindow.Translators = translators;
        }

        private void SetLanguages()
        {
            languageFrom = (Translator.Language)LanguagesFrom.SelectedItem;
            languageTo = (Translator.Language)LanguagesTo.SelectedItem;
        }

        private void CheckChosenTranslators()
        {
            GoogleCheckBox.IsChecked = googleIsChosen;
            BingCheckBox.IsChecked = bingIsChosen;
            YandexCheckBox.IsChecked = yandexIsChosen;
        }

        private void SetChosenTranslators()
        {
            googleIsChosen = (bool)GoogleCheckBox.IsChecked;
            bingIsChosen = (bool)BingCheckBox.IsChecked;
            yandexIsChosen = (bool)YandexCheckBox.IsChecked;
        }

        private void ApplyOptions()
        {
            SetLanguages();
            SetChosenTranslators(); 
            InitializeWebTranslators();           
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            ApplyOptions();
            ((Button)sender).IsEnabled = false;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsClosed = true;
            this.Close();
        }

        private void ChangesPerformed(object sender, EventArgs e)
        {
            Apply.IsEnabled = true;
        }
    }
}
