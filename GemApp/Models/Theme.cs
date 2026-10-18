using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace GemApp.Models;

public class Theme
{
    public string Name { get; }
    public Color BackgroundColor { get; }
    public Color TextColor { get; }
    public Color CardColor { get; }
    public string FontFamily { get; }
    public string BackgroundImageFile { get; }

    public Theme(
        string name,
        Color backgroundColor,
        Color textColor,
        Color cardColor,
        string fontFamily,
        string backgroundImageFile)
    {
        Name = name;
        BackgroundColor = backgroundColor;
        TextColor = textColor;
        CardColor = cardColor;
        FontFamily = fontFamily;
        BackgroundImageFile = backgroundImageFile;
    }

    public void Apply(ContentPage page)
    {
        page.BackgroundColor = BackgroundColor;

        page.Resources["BackgroundColor"] = BackgroundColor;
        page.Resources["TextColor"] = TextColor;
        page.Resources["CardColor"] = CardColor;
        page.Resources["GameFont"] = FontFamily;
    }

    public static Theme Light => new(
        "Lilleaed",
        Color.FromArgb("#FFF0F5"),
        Color.FromArgb("#49304F"),
        Color.FromArgb("#EFFFF5"),
        "OpenSansRegular",
        "flower_garden.jpeg");

    public static Theme Dark => new(
        "Öine meri",
        Color.FromArgb("#07182D"),
        Colors.White,
        Color.FromArgb("#18334D"),
        "OpenSansRegular",
        "night_sea.jpg");

    public static Theme Halloween => new(
        "Halloween",
        Color.FromArgb("#21132D"),
        Color.FromArgb("#FFF0D0"),
        Color.FromArgb("#492344"),
        "OpenSansRegular",
        "halloween.jpeg");
}