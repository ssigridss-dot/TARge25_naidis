
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GemApp.GemLogic;
using GemApp.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;

namespace GemApp;

public partial class MainPage : ContentPage
{
    private Game game = new();

    private readonly LeaderboardService leaderboardService = new();

    private readonly Dictionary<Gem, Button> gemButtons = new();

    private (int Row, int Col)? selectedGem;

    private bool isProcessingMove;
    private bool hasSavedResult;
    private bool hasGameStarted;

    private const int GameDurationSeconds = 60;

    private readonly Stopwatch gameStopwatch = new();

    private IDispatcherTimer? gameTimer;

    private int remainingSeconds = GameDurationSeconds;

    public MainPage()
    {
        InitializeComponent();

        ApplyTheme(Theme.Light);

        BuildBoard();
        UpdateStatistics();

        // Taimer luuakse, aga mäng ei käivitu enne nupu vajutamist.
        gameTimer = Dispatcher.CreateTimer();
        gameTimer.Interval = TimeSpan.FromSeconds(1);
        gameTimer.IsRepeating = true;
        gameTimer.Tick += OnGameTimerTick;

        remainingSeconds = GameDurationSeconds;
        TimerLabel.Text = $"{GameDurationSeconds} s";

        hasGameStarted = false;
        StatusLabel.Text = "Vajuta „Uus mäng”, et alustada!";
    }

    // TAIMER
    private void StartTimer()
    {
        gameTimer?.Stop();
        gameStopwatch.Reset();

        remainingSeconds = GameDurationSeconds;
        TimerLabel.Text = $"{remainingSeconds} s";

        hasSavedResult = false;
        hasGameStarted = true;

        gameStopwatch.Start();
        gameTimer?.Start();
    }

    private async void OnGameTimerTick(object? sender, EventArgs e)
    {
        if (!hasGameStarted || game.GameOver)
            return;

        remainingSeconds = Math.Max(
            0,
            GameDurationSeconds -
            (int)gameStopwatch.Elapsed.TotalSeconds);

        TimerLabel.Text = $"{remainingSeconds} s";

        if (remainingSeconds <= 0)
        {
            await FinishGameAsync();
        }
    }

    private async Task FinishGameAsync()
    {
        if (hasSavedResult)
            return;

        hasSavedResult = true;
        hasGameStarted = false;

        gameTimer?.Stop();
        gameStopwatch.Stop();

        game.EndGame();
        selectedGem = null;

        StatusLabel.Text = "Aeg sai läbi! 🏁";

        string? enteredName = await DisplayPromptAsync(
            "Mäng läbi!",
            $"Sinu tulemus: {game.Score} punkti.\n" +
            "Sisesta nimi edetabeli jaoks:",
            accept: "Salvesta tulemus",
            cancel: "Jäta nimi vahele",
            placeholder: "Mängija",
            maxLength: 25,
            initialValue: "Mängija");

        string playerName = string.IsNullOrWhiteSpace(enteredName)
            ? "Mängija"
            : enteredName.Trim();

        try
        {
            await leaderboardService.SaveAsync(
                new LeaderboardEntry
                {
                    PlayerName = playerName,
                    Score = game.Score,
                    PlayedAt = DateTime.Now
                });

            await DisplayAlert(
                "Mäng läbi!",
                $"{playerName}, sinu tulemus on {game.Score} punkti.\n" +
                "Tulemus on edetabelisse salvestatud.",
                "OK");
        }
        catch (IOException)
        {
            await DisplayAlert(
                "Salvestamise viga",
                $"Sinu tulemus oli {game.Score} punkti, " +
                "kuid selle salvestamine ebaõnnestus.",
                "OK");
        }
        catch (UnauthorizedAccessException)
        {
            await DisplayAlert(
                "Salvestamise viga",
                "Rakendusel pole õigust edetabelit salvestada.",
                "OK");
        }
    }

    // KALLISKIVI VÄLIMUS
    private static string GetGemSymbol(GemType type)
    {
        return type switch
        {
            GemType.Blue => "🔷",
            GemType.Green => "💚",
            GemType.Red => "🔴",
            GemType.Yellow => "🟡",
            GemType.Orange => "🟠",
            GemType.Purple => "💜",
            GemType.Black => "⚫",
            _ => "💎"
        };
    }

    private static Color GetGemColor(GemType type)
    {
        return type switch
        {
            GemType.Blue => Color.FromArgb("#2878D0"),
            GemType.Green => Color.FromArgb("#269B62"),
            GemType.Red => Color.FromArgb("#D93655"),
            GemType.Yellow => Color.FromArgb("#E0AF24"),
            GemType.Orange => Color.FromArgb("#E87924"),
            GemType.Purple => Color.FromArgb("#9255C7"),
            GemType.Black => Color.FromArgb("#414653"),
            _ => Color.FromArgb("#777777")
        };
    }

    private Button CreateGemButton(Gem gem)
    {
        var button = new Button
        {
            Text = GetGemSymbol(gem.Type),
            FontSize = 21,
            Padding = 0,
            Margin = 0,
            CornerRadius = 8,
            BackgroundColor = GetGemColor(gem.Type),
            TextColor = Colors.White,
            MinimumWidthRequest = 0,
            MinimumHeightRequest = 0,
            BorderWidth = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        button.Clicked += async (sender, args) =>
        {
            if (sender is Button clickedButton)
            {
                await OnGemClicked(
                    Grid.GetRow(clickedButton),
                    Grid.GetColumn(clickedButton));
            }
        };

        return button;
    }

    // MÄNGULAUA JOONISTAMINE
    private void BuildBoard()
    {
        GameBoard.Children.Clear();
        GameBoard.RowDefinitions.Clear();
        GameBoard.ColumnDefinitions.Clear();
        gemButtons.Clear();

        for (int row = 0; row < Board.Rows; row++)
        {
            GameBoard.RowDefinitions.Add(
                new RowDefinition { Height = GridLength.Star });
        }

        for (int col = 0; col < Board.Cols; col++)
        {
            GameBoard.ColumnDefinitions.Add(
                new ColumnDefinition { Width = GridLength.Star });
        }

        for (int row = 0; row < Board.Rows; row++)
        {
            for (int col = 0; col < Board.Cols; col++)
            {
                Gem gem = game.Board.Cells[row, col];
                Button button = CreateGemButton(gem);

                GameBoard.Add(button, col, row);
                gemButtons[gem] = button;
            }
        }
    }

    // KALLISKIVIDE ASUKOHAD
    private Dictionary<Gem, (int Row, int Col)> CaptureGemPositions()
    {
        var positions = new Dictionary<Gem, (int Row, int Col)>();

        for (int row = 0; row < Board.Rows; row++)
        {
            for (int col = 0; col < Board.Cols; col++)
            {
                Gem gem = game.Board.Cells[row, col];
                positions[gem] = (row, col);
            }
        }

        return positions;
    }

    // KUKKUMISANIMATSIOON
    private async Task AnimateBoardTransitionAsync(
        Dictionary<Gem, (int Row, int Col)> oldPositions)
    {
        var currentGems = game.Board.Cells
            .Cast<Gem>()
            .ToHashSet();

        var removedButtons = gemButtons
            .Where(pair => !currentGems.Contains(pair.Key))
            .ToList();

        await Task.WhenAll(
            removedButtons.Select(
                pair => pair.Value.FadeTo(0, 150)));

        foreach (var pair in removedButtons)
        {
            GameBoard.Children.Remove(pair.Value);
            gemButtons.Remove(pair.Key);
        }

        double cellHeight = GameBoard.Height > 0
            ? (GameBoard.Height -
               (Board.Rows - 1) * GameBoard.RowSpacing) / Board.Rows
            : 40;

        double cellWidth = GameBoard.Width > 0
            ? (GameBoard.Width -
               (Board.Cols - 1) * GameBoard.ColumnSpacing) / Board.Cols
            : 40;

        var animations = new List<Task>();

        for (int row = 0; row < Board.Rows; row++)
        {
            for (int col = 0; col < Board.Cols; col++)
            {
                Gem gem = game.Board.Cells[row, col];

                bool isNewGem = !gemButtons.TryGetValue(
                    gem,
                    out Button? button);

                if (isNewGem)
                {
                    button = CreateGemButton(gem);
                    gemButtons[gem] = button;

                    GameBoard.Add(button, col, row);

                    button.TranslationY = -cellHeight * (row + 1);
                    button.Opacity = 0;
                }
                else
                {
                    var oldPosition = oldPositions[gem];

                    double offsetX =
                        (oldPosition.Col - col) * cellWidth;

                    double offsetY =
                        (oldPosition.Row - row) * cellHeight;

                    Grid.SetRow(button!, row);
                    Grid.SetColumn(button!, col);

                    button!.TranslationX = offsetX;
                    button.TranslationY = offsetY;
                    button.Opacity = 1;
                }

                button!.Text = GetGemSymbol(gem.Type);
                button.BackgroundColor = GetGemColor(gem.Type);

                animations.Add(AnimateGemAsync(button));
            }
        }

        await Task.WhenAll(animations);
    }

    private static async Task AnimateGemAsync(Button button)
    {
        await Task.WhenAll(
            button.TranslateTo(0, 0, 280, Easing.CubicOut),
            button.FadeTo(1, 180, Easing.CubicOut));
    }

    // KALLISKIVIDE VALIMINE JA VAHETAMINE
    private async Task OnGemClicked(int row, int col)
    {
        if (!hasGameStarted)
        {
            StatusLabel.Text = "Vajuta „Uus mäng”, et alustada!";
            return;
        }

        if (isProcessingMove || game.GameOver)
            return;

        if (selectedGem == null)
        {
            selectedGem = (row, col);
            HighlightGem(row, col, true);

            StatusLabel.Text = "Vali nüüd kõrvalasuv kalliskivi!";
            return;
        }

        var first = selectedGem.Value;

        if (first.Row == row && first.Col == col)
        {
            HighlightGem(row, col, false);
            selectedGem = null;

            StatusLabel.Text = "Vali esimene kalliskivi.";
            return;
        }

        bool adjacent =
            Math.Abs(first.Row - row) +
            Math.Abs(first.Col - col) == 1;

        if (!adjacent)
        {
            HighlightGem(first.Row, first.Col, false);

            selectedGem = (row, col);
            HighlightGem(row, col, true);

            StatusLabel.Text =
                "Vali horisontaalselt või vertikaalselt kõrvuti kivi!";

            return;
        }

        HighlightGem(first.Row, first.Col, false);
        selectedGem = null;

        isProcessingMove = true;

        try
        {
            var oldPositions = CaptureGemPositions();

            bool validMove = game.MakeMove(
                first.Row,
                first.Col,
                row,
                col);

            if (!validMove)
            {
                StatusLabel.Text =
                    "See vahetus ei tekita kolmikut. Proovi uuesti!";
                return;
            }

            await AnimateBoardTransitionAsync(oldPositions);
            UpdateStatistics();

            if (game.GameOver)
                return;

            StatusLabel.Text = "Tubli! Kogu veel punkte! 💎";
        }
        finally
        {
            isProcessingMove = false;
        }
    }

    private void HighlightGem(int row, int col, bool selected)
    {
        foreach (var button in GameBoard.Children.OfType<Button>())
        {
            if (Grid.GetRow(button) == row &&
                Grid.GetColumn(button) == col)
            {
                button.BorderWidth = selected ? 4 : 0;
                button.BorderColor = Colors.White;
                break;
            }
        }
    }

    // PUNKTID
    private void UpdateStatistics()
    {
        ScoreLabel.Text = game.Score.ToString();

        ScoreProgress.Progress = Math.Clamp(
            (double)game.Score / game.TargetScore,
            0.0,
            1.0);
    }

    // EDETABEL
    private async void OnLeaderboardClicked(object sender, EventArgs e)
    {
        try
        {
            var entries = await leaderboardService.LoadAsync();

            LeaderboardList.ItemsSource = entries;

            LeaderboardPopupBackdrop.IsVisible = true;
            LeaderboardPopup.IsVisible = true;
        }
        catch (IOException)
        {
            await DisplayAlert(
                "Viga",
                "Edetabelit ei saanud lugeda.",
                "OK");
        }
        catch (UnauthorizedAccessException)
        {
            await DisplayAlert(
                "Viga",
                "Rakendusel pole õigust edetabelit lugeda.",
                "OK");
        }
    }

    private void OnCloseLeaderboardClicked(object sender, EventArgs e)
    {
        CloseLeaderboard();
    }

    private void CloseLeaderboard()
    {
        LeaderboardPopup.IsVisible = false;
        LeaderboardPopupBackdrop.IsVisible = false;
    }

    private async void OnResetLeaderboardClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Nulli edetabel",
            "Kas soovid kustutada kõik edetabeli tulemused?\n" +
            "Seda tegevust ei saa tagasi võtta.",
            "Jah, kustuta",
            "Tühista");

        if (!confirm)
            return;

        try
        {
            await leaderboardService.ClearAsync();

            // Värskenda avatud edetabel kohe tühjaks.
            LeaderboardList.ItemsSource =
                await leaderboardService.LoadAsync();

            await DisplayAlert(
                "Edetabel nullitud",
                "Kõik edetabeli tulemused on kustutatud.",
                "OK");
        }
        catch (IOException)
        {
            await DisplayAlert(
                "Viga",
                "Edetabelit ei saanud kustutada.",
                "OK");
        }
        catch (UnauthorizedAccessException)
        {
            await DisplayAlert(
                "Viga",
                "Rakendusel pole õigust edetabelit kustutada.",
                "OK");
        }
    }

    // TEEMAD
    private void OnLightThemeClicked(object sender, EventArgs e)
    {
        ApplyTheme(Theme.Light);
        CloseThemeMenu();
    }

    private void OnDarkThemeClicked(object sender, EventArgs e)
    {
        ApplyTheme(Theme.Dark);
        CloseThemeMenu();
    }

    private void OnHalloweenThemeClicked(object sender, EventArgs e)
    {
        ApplyTheme(Theme.Halloween);
        CloseThemeMenu();
    }

    private void ApplyTheme(Theme theme)
    {
        theme.Apply(this);

        ThemeOverlay.BackgroundColor = theme.BackgroundColor;

        ThemeBackground.Source =
            ImageSource.FromFile(theme.BackgroundImageFile);

        ThemeBackground.Opacity =
            theme.Name == "Lilleaed" ? 0.72 :
            theme.Name == "Öine meri" ? 0.62 :
            0.58;

        ThemeOverlay.Opacity =
            theme.Name == "Lilleaed" ? 0.15 :
            theme.Name == "Öine meri" ? 0.48 :
            0.42;

        if (hasGameStarted)
            StatusLabel.Text = $"Valitud kujundus: {theme.Name}";
    }

    // UUS MÄNG
    private async void OnRestartClicked(object sender, EventArgs e)
    {
        if (isProcessingMove)
            return;

        gameTimer?.Stop();
        gameStopwatch.Stop();

        selectedGem = null;
        hasGameStarted = false;
        hasSavedResult = false;

        game.Restart();

        BuildBoard();
        UpdateStatistics();

        StartTimer();

        StatusLabel.Text = "Mäng algas! Kogu 60 sekundiga punkte! 💎";

        await GameBoard.FadeTo(0.4, 100);
        await GameBoard.FadeTo(1.0, 150);
    }

    // TEEMA HÜPIKAKEN
    private void OnThemeMenuClicked(object sender, EventArgs e)
    {
        ThemePopupBackdrop.IsVisible = true;
        ThemePopup.IsVisible = true;
    }

    private void OnCloseThemeMenuClicked(object sender, EventArgs e)
    {
        CloseThemeMenu();
    }

    private void OnCloseThemeMenuTapped(object sender, TappedEventArgs e)
    {
        CloseThemeMenu();
    }

    private void CloseThemeMenu()
    {
        ThemePopup.IsVisible = false;
        ThemePopupBackdrop.IsVisible = false;
    }
}
