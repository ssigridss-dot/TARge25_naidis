using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace TARge25_naidis
{
    public partial class ValgusfoorPage : ContentPage
    {
        // Valgusfoori tuled
        private BoxView punane;
        private BoxView kollane;
        private BoxView roheline;

        // Lehe pealkiri
        private Label pealkiri;

        // Kas foor on sisse lülitatud?
        private bool foorOn = false;

        public ValgusfoorPage()
        {
            Title = "Valgusfoor";
            Label tiitliTekst = new Label
            {
                Text = "Valgusfoor",
                TextColor = Colors.Black,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };

            NavigationPage.SetTitleView(this, tiitliTekst);

            // TAUSTAPILT

            Image taust = new Image
            {
                Source = "linn.webp",
                Aspect = Aspect.AspectFill
            };

            // PEALKIRI

            pealkiri = new Label
            {
                Text = "Vali valgus",
                FontSize = 28,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 15)
            };

            // PUNANE TULI

            punane = new BoxView
            {
                WidthRequest = 90,
                HeightRequest = 90,
                CornerRadius = 45,
                Color = Colors.Gray,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            // KOLLANE TULI

            kollane = new BoxView
            {
                WidthRequest = 90,
                HeightRequest = 90,
                CornerRadius = 45,
                Color = Colors.Gray,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            // ROHELINE TULI

            roheline = new BoxView
            {
                WidthRequest = 90,
                HeightRequest = 90,
                CornerRadius = 45,
                Color = Colors.Gray,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            // FOORI KORPUS

            Border fooriKorpus = new Border
            {
                BackgroundColor = Colors.Black,
                Stroke = Colors.DarkGray,
                StrokeThickness = 4,
                Padding = new Thickness(20),
                HorizontalOptions = LayoutOptions.Center,

                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(30)
                }
            };

            // FOORI KORPUSE SISU

            VerticalStackLayout tuled = new VerticalStackLayout
            {
                Spacing = 18,
                HorizontalOptions = LayoutOptions.Center
            };

            tuled.Children.Add(punane);
            tuled.Children.Add(kollane);
            tuled.Children.Add(roheline);

            fooriKorpus.Content = tuled;

            // FOORI POST

            BoxView post = new BoxView
            {
                WidthRequest = 25,
                HeightRequest = 150,
                Color = Colors.Black,
                HorizontalOptions = LayoutOptions.Center
            };

            // POSTI ALUS

            BoxView postiAlus = new BoxView
            {
                WidthRequest = 130,
                HeightRequest = 20,
                Color = Colors.Black,
                HorizontalOptions = LayoutOptions.Center
            };

            // SISSE NUPP

            Button sisseNupp = new Button
            {
                Text = "Sisse",
                FontSize = 20,
                BackgroundColor = Colors.Green,
                TextColor = Colors.White,
                CornerRadius = 10,
                WidthRequest = 130
            };

            sisseNupp.Clicked += (sender, e) =>
            {
                FoorSisse();
            };

            // VÄLJA NUPP

            Button väljaNupp = new Button
            {
                Text = "Välja",
                FontSize = 20,
                BackgroundColor = Colors.Red,
                TextColor = Colors.White,
                CornerRadius = 10,
                WidthRequest = 130
            };

            väljaNupp.Clicked += (sender, e) =>
            {
                FoorVälja();
            };

            // NUPPUDE PAIGUTUS

            HorizontalStackLayout nupud = new HorizontalStackLayout
            {
                Spacing = 20,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 15, 0, 20)
            };

            nupud.Children.Add(sisseNupp);
            nupud.Children.Add(väljaNupp);

            // KOGU FOORI PAIGUTUS

            VerticalStackLayout fooriPaigutus = new VerticalStackLayout
            {
                Spacing = 5,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            fooriPaigutus.Children.Add(pealkiri);
            fooriPaigutus.Children.Add(fooriKorpus);
            fooriPaigutus.Children.Add(post);
            fooriPaigutus.Children.Add(postiAlus);
            fooriPaigutus.Children.Add(nupud);

            // GRID

            Grid ekraan = new Grid();

            // Esimene kiht = taust
            ekraan.Children.Add(taust);

            // Teine kiht = valgusfoor
            ekraan.Children.Add(fooriPaigutus);


            // Grid ekraanil
            Content = ekraan;

            // TULEDE KLIKKIMINE

            LisaPunaseKlõps();
            LisaKollaseKlõps();
            LisaRoheliseKlõps();
        }


        // FOORI SISSELÜLITAMINE

        private void FoorSisse()
        {
            foorOn = true;

            // Õiged värvid
            punane.Color = Colors.Red;
            kollane.Color = Colors.Yellow;
            roheline.Color = Colors.Green;

            pealkiri.Text = "Vali valgus";
        }

        // FOORI VÄLJALÜLITAMINE

        private void FoorVälja()
        {
            foorOn = false;

            // Kõik tuled muutuvad halliks
            punane.Color = Colors.Gray;
            kollane.Color = Colors.Gray;
            roheline.Color = Colors.Gray;

            pealkiri.Text = "Lülita foor sisse";
        }

        // PUNASE TULE KLIKK

        private void LisaPunaseKlõps()
        {
            TapGestureRecognizer klõps = new TapGestureRecognizer();

            klõps.Tapped += async (sender, e) =>
            {
                if (foorOn)
                {
                    pealkiri.Text = "Seisa";

                    await punane.ScaleTo(1.2, 150);
                    await punane.ScaleTo(1.0, 150);
                }
            };

            punane.GestureRecognizers.Add(klõps);
        }

        // KOLLASE TULE KLIKK

        private void LisaKollaseKlõps()
        {
            TapGestureRecognizer klõps = new TapGestureRecognizer();

            klõps.Tapped += async (sender, e) =>
            {
                if (foorOn)
                {
                    pealkiri.Text = "Valmistu";

                    await kollane.ScaleTo(1.2, 150);
                    await kollane.ScaleTo(1.0, 150);
                }
            };

            kollane.GestureRecognizers.Add(klõps);
        }

        // ROHELISE TULE KLIKK

        private void LisaRoheliseKlõps()
        {
            TapGestureRecognizer klõps = new TapGestureRecognizer();

            klõps.Tapped += async (sender, e) =>
            {
                if (foorOn)
                {
                    pealkiri.Text = "Sõida";

                    await roheline.ScaleTo(1.2, 150);
                    await roheline.ScaleTo(1.0, 150);
                }
            };

            roheline.GestureRecognizers.Add(klõps);
        }
    }
}