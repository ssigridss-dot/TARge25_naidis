
using System.Collections.ObjectModel;
using System.Globalization;
using ItalyFoodApp.Models;
using ItalyFoodApp.Resources.Languages;

namespace ItalyFoodApp;

public partial class MainPage : ContentPage
{
    public ObservableCollection<Dish> Dishes { get; set; } = new();

    private bool isEstonian = false;
    private CancellationTokenSource? carouselCancellation;

    // Hetkel avatud roa andmed
    private Dish? selectedDish;

    public MainPage()
    {
        InitializeComponent();

        CreateDishes();

        BindingContext = this;
        DishCarousel.IndicatorView = DishIndicator;

        UpdateLanguage();
    }

    // TOIDUD
    private void CreateDishes()
    {
        Dishes.Add(new Dish
        {
            Name = "Pasta Carbonara",
            Description = "Classic Roman pasta dish",
            ImageUrl = "pasta_carbonara.png",
            Ingredients = "Spaghetti, eggs, Pecorino Romano, guanciale and black pepper.",
            PreparationTime = "About 25 minutes",
            NameEt = "Pasta Carbonara",
            DescriptionEt = "Klassikaline Rooma pastaroog",
            IngredientsEt = "Spagetid, munad, Pecorino Romano, guanciale ja must pipar.",
            PreparationTimeEt = "Umbes 25 minutit"
        });

        Dishes.Add(new Dish
        {
            Name = "Pizza Margherita",
            Description = "Simple and delicious Italian pizza",
            ImageUrl = "pizza_margherita.png",
            Ingredients = "Pizza dough, tomato sauce, mozzarella, basil and olive oil.",
            PreparationTime = "About 30 minutes",
            NameEt = "Pizza Margherita",
            DescriptionEt = "Lihtne ja maitsev Itaalia pitsa",
            IngredientsEt = "Pitsapõhi, tomatikaste, mozzarella, basiilik ja oliiviõli.",
            PreparationTimeEt = "Umbes 30 minutit"
        });

        Dishes.Add(new Dish
        {
            Name = "Tiramisu",
            Description = "Popular coffee-flavored Italian dessert",
            ImageUrl = "tiramisu.png",
            Ingredients = "Ladyfingers, coffee, mascarpone, eggs, sugar and cocoa powder.",
            PreparationTime = "About 30 minutes + chilling",
            NameEt = "Tiramisu",
            DescriptionEt = "Populaarne kohvimaitseline Itaalia magustoit",
            IngredientsEt = "Küpsised, kohv, mascarpone, munad, suhkur ja kakaopulber.",
            PreparationTimeEt = "Umbes 30 minutit + jahutamine"
        });

        Dishes.Add(new Dish
        {
            Name = "Risotto alla Milanese",
            Description = "Creamy saffron rice from Milan",
            ImageUrl = "risotto_alla_milanese.png",
            Ingredients = "Arborio rice, saffron, Parmesan, onion, butter and white wine.",
            PreparationTime = "About 35 minutes",
            NameEt = "Risotto alla Milanese",
            DescriptionEt = "Kreemine safraniriis Milanost",
            IngredientsEt = "Arborio riis, safran, parmesan, sibul, või ja valge vein.",
            PreparationTimeEt = "Umbes 35 minutit"
        });

        Dishes.Add(new Dish
        {
            Name = "Lasagne",
            Description = "Layers of pasta, meat and cheese",
            ImageUrl = "lasagne.png",
            Ingredients = "Pasta sheets, minced meat, tomato sauce, béchamel and Parmesan.",
            PreparationTime = "About 1 hour",
            NameEt = "Lasagne",
            DescriptionEt = "Kihiline pasta liha ja juustuga",
            IngredientsEt = "Pastalehed, hakkliha, tomatikaste, béchamel-kaste ja parmesan.",
            PreparationTimeEt = "Umbes 1 tund"
        });
    }

    // KEELEVALIK
    private void EstonianButton_Clicked(object sender, EventArgs e)
    {
        isEstonian = true;
        UpdateLanguage();
    }

    private void EnglishButton_Clicked(object sender, EventArgs e)
    {
        isEstonian = false;
        UpdateLanguage();
    }

    // KEELE VAHETAMINE
    private void UpdateLanguage()
    {
        AppResources.Culture = new CultureInfo(
            isEstonian ? "et" : "en");

        TitleLabel.Text = AppResources.AppTitle;
        Title = isEstonian ? "Itaalia toidud" : "Italian Food";
        SubtitleLabel.Text = AppResources.Subtitle;
        
        EstonianButton.Text = "🇪🇪 Eesti";
        EnglishButton.Text = "🇬🇧 English";

        EstonianButton.BackgroundColor = isEstonian
            ? Color.FromArgb("#8B0000")
            : Color.FromArgb("#555555");

        EnglishButton.BackgroundColor = isEstonian
            ? Color.FromArgb("#555555")
            : Color.FromArgb("#8B0000");

        foreach (Dish dish in Dishes)
        {
            dish.DisplayName = isEstonian
                ? dish.NameEt
                : dish.Name;

            dish.DisplayDescription = GetDescription(dish);
            dish.DisplayMoreInfo = AppResources.MoreInformation;
        }

        // Kui detailinfo on avatud, uuendame ka selle keelt.
        if (selectedDish != null && DetailsOverlay.IsVisible)
        {
            RefreshDishDetails(selectedDish);
        }
    }

    private string GetDescription(Dish dish)
    {
        if (dish.Name == "Pasta Carbonara")
            return AppResources.CarbonaraDescription;

        if (dish.Name == "Pizza Margherita")
            return AppResources.PizzaDescription;

        if (dish.Name == "Tiramisu")
            return AppResources.TiramisuDescription;

        if (dish.Name == "Risotto alla Milanese")
            return AppResources.RisottoDescription;

        if (dish.Name == "Lasagne")
            return AppResources.LasagneDescription;

        return isEstonian
            ? dish.DescriptionEt
            : dish.Description;
    }

    // DETAILINFO AVAMINE
    private void DishButton_Clicked(object sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.BindingContext is not Dish dish)
        {
            return;
        }

        // Peatame automaatse kerimise.
        carouselCancellation?.Cancel();
        carouselCancellation?.Dispose();
        carouselCancellation = null;

        selectedDish = dish;

        RefreshDishDetails(dish);

        // Näitame kujundatud hüpikakent.
        DetailsOverlay.IsVisible = true;
    }

    // DETAILINFO SISU JA KEELE UUENDAMINE
    private void RefreshDishDetails(Dish dish)
    {
        DetailTitleLabel.Text = isEstonian
            ? dish.NameEt
            : dish.Name;

        IngredientsHeadingLabel.Text = AppResources.Ingredients;
        PreparationHeadingLabel.Text = AppResources.PreparationTime;
        CloseDetailsButton.Text = AppResources.Ok;

        IngredientsValueLabel.Text = isEstonian
            ? dish.IngredientsEt
            : dish.Ingredients;

        PreparationValueLabel.Text = isEstonian
            ? dish.PreparationTimeEt
            : dish.PreparationTime;
    }

    // DETAILINFO SULGEMINE
    private void CloseDetailsButton_Clicked(object sender, EventArgs e)
    {
        DetailsOverlay.IsVisible = false;
        selectedDish = null;

        // Jätkame automaatset kerimist.
        if (IsVisible)
        {
            StartAutomaticCarousel();
        }
    }

    // AUTOMAATNE KERIMINE IGA 4 SEKUNDI JÄREL
    protected override void OnAppearing()
    {
        base.OnAppearing();

        StartAutomaticCarousel();
    }

    private void StartAutomaticCarousel()
    {
        if (carouselCancellation != null)
            return;

        carouselCancellation = new CancellationTokenSource();
        CancellationToken token = carouselCancellation.Token;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(4000, token);

                    if (Dishes.Count == 0 ||
                        DetailsOverlay.IsVisible)
                    {
                        continue;
                    }

                    int nextPosition =
                        (DishCarousel.Position + 1) % Dishes.Count;

                    DishCarousel.ScrollTo(
                        nextPosition,
                        position: ScrollToPosition.Center,
                        animate: true);
                }
            }
            catch (OperationCanceledException)
            {
                // Kerimine peatati.
            }
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        carouselCancellation?.Cancel();
        carouselCancellation?.Dispose();
        carouselCancellation = null;
    }

    // ANIMATSIOON KAARDI VAHETAMISEL
    private async void DishCarousel_PositionChanged(
        object sender,
        PositionChangedEventArgs e)
    {
        await DishCarousel.FadeToAsync(0.9, 100);
        await DishCarousel.FadeToAsync(1.0, 150);
    }
}
