using MultilangApp.Services;
using MultilangApp.Resources.Localization;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MultilangApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Greeting => AppResources.GreetingText;
        public string ChangeLanguageLabel => AppResources.ChangeLanguage;
        public string EnglishButton => AppResources.EnglishButton;
        public string RussianButton => AppResources.RussianButton;
        public string EstonianButton => AppResources.EstonianButton;

        public ICommand SetEnglishCommand { get; }
        public ICommand SetRussianCommand { get; }
        public ICommand SetEstonianCommand { get; }

        public MainViewModel()
        {
            SetEnglishCommand = new Command(() => ChangeLanguage("en"));
            SetRussianCommand = new Command(() => ChangeLanguage("ru"));
            SetEstonianCommand = new Command(() => ChangeLanguage("et"));

            LanguageService.LanguageChanged += OnLanguageChanged;
        }

        private void ChangeLanguage(string code)
        {
            LanguageService.ChangeLanguage(code);
        }

        private void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(Greeting));
            OnPropertyChanged(nameof(ChangeLanguageLabel));
            OnPropertyChanged(nameof(EnglishButton));
            OnPropertyChanged(nameof(RussianButton));
            OnPropertyChanged(nameof(EstonianButton));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
