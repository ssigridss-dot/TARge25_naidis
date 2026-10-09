using System.Collections.ObjectModel;
using ItalyFoodApp.Models;

namespace ItalyFoodApp;

public partial class MainPage : ContentPage
{
    public ObservableCollection<Dish> Dishes { get; set; }

    private bool isEstonian = false;

    public MainPage()
    {
        InitializeComponent();

        Dishes = new ObservableCollection<Dish>
        {
            new Dish
            {
                Name = "Pasta Carbonara",
                Description = "Classic Roman pasta dish",
                ImageUrl = "carbonara.jpg",
                Ingredients = "Spaghetti, eggs, Pecorino Romano, guanciale and black pepper.",
                PreparationTime = "About 25 minutes",

                NameEt = "Pasta Carbonara",
                DescriptionEt = "Klassikaline Rooma pastaroog",
                IngredientsEt = "Spagetid, munad, Pecorino Romano, guanciale ja must pipar.",
                PreparationTimeEt = "Umbes 25 minutit"
            },

            new Dish
            {
                Name = "Pizza Margherita",
                Description = "Simple and delicious Italian pizza",
                ImageUrl = "pizza.jpg",
                Ingredients = "Pizza dough, tomato sauce, mozzarella, basil and olive oil.",
                PreparationTime = "About 30 minutes",

                NameEt = "Pizza Margherita",
                DescriptionEt = "Lihtne ja maitsev Itaalia pitsa",
                IngredientsEt = "Pitsapõhi, tomatikaste, mozzarella, basiilik ja oliiviõli.",
                PreparationTimeEt = "Umbes 30 minutit"
            },

            new Dish
            {
                Name = "Tiramisu",
                Description = "Popular coffee-flavored Italian dessert",
                ImageUrl = "tiramisu.jpg",
                Ingredients = "Ladyfingers, coffee, mascarpone, eggs, sugar and cocoa powder.",
                PreparationTime = "About 30 minutes + chilling",

                NameEt = "Tiramisu",
                DescriptionEt = "Populaarne kohvimaitseline Itaalia magustoit",
                IngredientsEt = "Küpsised, kohv, mascarpone, munad, suhkur ja kakaopulber.",
                PreparationTimeEt = "Umbes 30 minutit + jahutamine"
            },

            new Dish
            {
                Name = "Risotto alla Milanese",
                Description = "Creamy saffron rice from Milan",
                ImageUrl = "risotto.jpg",
                Ingredients = "Arborio rice, saffron, Parmesan, onion, butter and white wine.",
                PreparationTime = "About 35 minutes",

                NameEt = "Risotto alla Milanese",
                DescriptionEt = "Kreemine safraniriis Milanost",
                IngredientsEt = "Arborio riis, safran, Parmesan, sibul, või ja valge vein.",
                PreparationTimeEt = "Umbes 35 minutit"
            },

            new Dish
            {
                Name = "Lasagne",
                Description = "Layers of pasta, meat and cheese",
                ImageUrl = "lasagne.jpg",
                Ingredients = "Pasta sheets, minced meat, tomato sauce, béchamel and Parmesan.",
                PreparationTime = "About 1 hour",

                NameEt = "Lasagne",
                DescriptionEt = "Kihiline pasta liha ja juustuga",
                IngredientsEt = "Pastalehed, hakkliha, tomatikaste, béchamel-kaste ja Parmesan.",
                PreparationTimeEt = "Umbes 1 tund"
            }
        };

        BindingContext = this;

        DishCarousel.IndicatorView = DishIndicator;

        StartAutomaticCarousel();
    }


    // ==========================================
    // AUTOMAATNE KERIMINE
    // ==========================================

    private async void StartAutomaticCarousel()
    {
        while (true)
        {
            await Task.Delay(4000);

            if (Dishes.Count == 0)
                continue;

            int nextPosition = DishCarousel.Position + 1;

            if (nextPosition >= Dishes.Count)
                nextPosition = 0;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                DishCarousel.ScrollTo(
                    nextPosition,
                    position: ScrollToPosition.Center,
                    animate: true);
            });
        }
    }


    // ==========================================
    // KAARDI / INFO NUPU VAJUTAMINE
    // ==========================================

    private async void DishButton_Clicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.BindingContext is not Dish dish)
            return;

        string title;
        string message;

        if (isEstonian)
        {
            title = dish.NameEt;

            message =
                $"Peamised koostisosad:\n\n" +
                $"{dish.IngredientsEt}\n\n" +
                $"Valmistamisaeg: {dish.PreparationTimeEt}";
        }
        else
        {
            title = dish.Name;

            message =
                $"Main ingredients:\n\n" +
                $"{dish.Ingredients}\n\n" +
                $"Preparation time: {dish.PreparationTime}";
        }

        await DisplayAlert(title, message, "OK");
    }


    // ==========================================
    // EESTI KEEL
    // ==========================================

    private void EstonianButton_Clicked(object sender, EventArgs e)
    {
        isEstonian = true;

        UpdateLanguage();

        EstonianButton.BackgroundColor = Color.FromArgb("#8B0000");
        EnglishButton.BackgroundColor = Color.FromArgb("#555555");
    }


    // ==========================================
    // INGLISE KEEL
    // ==========================================

    private void EnglishButton_Clicked(object sender, EventArgs e)
    {
        isEstonian = false;

        UpdateLanguage();

        EnglishButton.BackgroundColor = Color.FromArgb("#8B0000");
        EstonianButton.BackgroundColor = Color.FromArgb("#555555");
    }


    // ==========================================
    // TEKSTIDE UUENDAMINE
    // ==========================================

    private void UpdateLanguage()
    {
        foreach (Dish dish in Dishes)
        {
            if (isEstonian)
            {
                // Eesti tekstide jaoks kasutame
                // eraldi omadusi.
            }
        }

        DishCarousel.ItemsSource = null;
        DishCarousel.ItemsSource = Dishes;

        if (isEstonian)
        {
            TitleLabel.Text = "Itaalia köök";
            SubtitleLabel.Text = "Avasta Itaalia parimad maitsed";
            SwipeLabel.Text = "Libista, et avastada Itaalia roogasid";

            foreach (Dish dish in Dishes)
            {
                dish.Name = dish.NameEt;
                dish.Description = dish.DescriptionEt;
            }
        }
        else
        {
            TitleLabel.Text = "Italian Kitchen";
            SubtitleLabel.Text = "Taste the best of Italy";
            SwipeLabel.Text = "Swipe to explore Italian dishes";

            Dishes[0].Name = "Pasta Carbonara";
            Dishes[0].Description = "Classic Roman pasta dish";

            Dishes[1].Name = "Pizza Margherita";
            Dishes[1].Description = "Simple and delicious Italian pizza";

            Dishes[2].Name = "Tiramisu";
            Dishes[2].Description = "Popular coffee-flavored Italian dessert";

            Dishes[3].Name = "Risotto alla Milanese";
            Dishes[3].Description = "Creamy saffron rice from Milan";

            Dishes[4].Name = "Lasagne";
            Dishes[4].Description = "Layers of pasta, meat and cheese";
        }

        DishCarousel.ItemsSource = null;
        DishCarousel.ItemsSource = Dishes;
    }


    // ==========================================
    // KAARDI VAHETUMISE ANIMATSIOON
    // ==========================================

    private async void DishCarousel_PositionChanged(
        object sender,
        PositionChangedEventArgs e)
    {
        if (DishCarousel.CurrentItem is not null)
        {
            await DishCarousel.FadeTo(0.85, 100);
            await DishCarousel.FadeTo(1.0, 150);
        }
    }
}