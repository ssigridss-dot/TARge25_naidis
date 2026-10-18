using System;
using System.Linq;
using System.Threading.Tasks;
using GemApp.GemLogic;
using GemApp.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace GemApp;

public partial class MainPage : ContentPage
{
    private Game game = new();
    private (int Row, int Col)? selectedGem;

    private readonly Color[] gemColors =
    {
        Color.FromArgb("#E85D75"),
        Color.FromArgb("#42A879"),
        Color.FromArgb("#4C91E8"),
        Color.FromArgb("#9B72CF"),
        Color.FromArgb("#E8B547"),
        Color.FromArgb("#55C6C8")
    };

    private readonly string[] gemTypes =
    {
        "Ruby",
        "Emerald",
        "Sapphire",
        "Amethyst",
        "Topaz",
        "Diamond"
    };

    public MainPage()
    {
        InitializeComponent();

        ApplyTheme(Theme.Light);
        BuildBoard();
        UpdateStatistics();
    }

    private void ApplyTheme(Theme theme)
    {
        theme.Apply(this);

        ThemeOverlay.BackgroundColor = theme.BackgroundColor;

        // Taustapilt peab asuma kaustas Resources/Images.
        ThemeBackground.Source =
            ImageSource.FromFile(theme.BackgroundImageFile);

        // Taustapildi tugevus sõltub valitud teemast.
        ThemeBackground.Opacity =
            theme.Name == "Lilleaed" ? 0.72 :
            theme.Name == "Öine meri" ? 0.62 :
            0.58;

        ThemeOverlay.Opacity =
            theme.Name == "Lilleaed" ? 0.15 :
            theme.Name == "Öine meri" ? 0.48 :
            0.42;

        StatusLabel.Text = $"Valitud kujundus: {theme.Name}";
    }

    private void BuildBoard()
    {
        GameBoard.Children.Clear();
        GameBoard.RowDefinitions.Clear();
        GameBoard.ColumnDefinitions.Clear();

        for (int row = 0; row < game.Board.Rows; row++)
        {
            GameBoard.RowDefinitions.Add(
                new RowDefinition { Height = GridLength.Star });
        }

        for (int col = 0; col < game.Board.Columns; col++)
        {
            GameBoard.ColumnDefinitions.Add(
                new ColumnDefinition { Width = GridLength.Star });
        }

        for (int row = 0; row < game.Board.Rows; row++)
        {
            for (int col = 0; col < game.Board.Columns; col++)
            {
                int gemRow = row;
                int gemCol = col;

                Gem gem = game.Board.Gems[gemRow, gemCol]!;

                int colorIndex = Array.IndexOf(gemTypes, gem.Type);

                var button = new Button
                {
                    Text = gem.Symbol,
                    FontSize = 21,
                    Padding = 0,
                    Margin = 0,
                    CornerRadius = 8,
                    BackgroundColor = gemColors[colorIndex],
                    TextColor = Colors.White,
                    MinimumWidthRequest = 0,
                    MinimumHeightRequest = 0,
                    BorderWidth = 0,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                button.Clicked += async (sender, args) =>
                {
                    await OnGemClicked(gemRow, gemCol);
                };

                GameBoard.Add(button, gemCol, gemRow);
            }
        }
    }

    private async Task OnGemClicked(int row, int col)
    {
        if (game.IsGameOver)
            return;

        if (selectedGem == null)
        {
            selectedGem = (row, col);
            HighlightGem(row, col, true);

            StatusLabel.Text =
                "Valisid kalliskivi. Vali nüüd kõrvalasuv kivi!";

            return;
        }

        var first = selectedGem.Value;

        HighlightGem(first.Row, first.Col, false);
        selectedGem = null;

        if (first.Row == row && first.Col == col)
        {
            StatusLabel.Text = "Vali järgmine kalliskivi.";
            return;
        }

        bool validSwap = game.TrySwap(
            first.Row, first.Col, row, col);

        if (!validSwap)
        {
            StatusLabel.Text =
                "Vaheta kõrvuti kive nii, et tekiks vähemalt kolm ühesugust!";
            return;
        }

        await GameBoard.ScaleTo(1.03, 80);
        await GameBoard.ScaleTo(1.0, 100);

        BuildBoard();
        UpdateStatistics();

        if (game.IsGameOver)
        {
            StatusLabel.Text = "Sa võitsid! 🎉";

            await DisplayAlert(
                "Palju õnne!",
                $"Said {game.Player.Score} punkti ja tegid {game.Player.Moves} käiku!",
                "Vahva!");

            return;
        }

        StatusLabel.Text = "Tubli! Vali järgmised kalliskivid.";
    }

    private void HighlightGem(
        int row,
        int col,
        bool selected)
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

    private void UpdateStatistics()
    {
        ScoreLabel.Text =
            $"{game.Player.Score} / {game.TargetScore}";

        MovesLabel.Text =
            $"KÄIGUD: {game.Player.Moves}";

        ScoreProgress.Progress = Math.Min(
            1.0,
            (double)game.Player.Score / game.TargetScore);
    }

    private void OnLightThemeClicked(
        object sender,
        EventArgs e)
    {
        ApplyTheme(Theme.Light);
    }

    private void OnDarkThemeClicked(
        object sender,
        EventArgs e)
    {
        ApplyTheme(Theme.Dark);
    }

    private void OnHalloweenThemeClicked(
        object sender,
        EventArgs e)
    {
        ApplyTheme(Theme.Halloween);
    }

    private async void OnRestartClicked(
        object sender,
        EventArgs e)
    {
        game.Restart();
        selectedGem = null;

        BuildBoard();
        UpdateStatistics();

        StatusLabel.Text = "Uus mäng on alanud!";

        await GameBoard.FadeTo(0.4, 100);
        await GameBoard.FadeTo(1.0, 150);
    }
}