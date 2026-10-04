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

        // Kas automaatrežiim töötab?
        private bool automaatrežiim = false;

        // Automaatrežiimi peatamiseks
        private CancellationTokenSource automaatTühistus;

        public ValgusfoorPage()
        {
            Title = "Valgusfoor";

            // ÜLEMINE LEHE PEALKIRI

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
                WidthRequest = 120
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
                WidthRequest = 120
            };

            väljaNupp.Clicked += (sender, e) =>
            {
                FoorVälja();
            };

            // AUTOMAATREŽIIMI NUPP

            Button automaatNupp = new Button
            {
                Text = "Automaatrežiim",
                FontSize = 18,
                BackgroundColor = Colors.DarkOrange,
                TextColor = Colors.White,
                CornerRadius = 10,
                WidthRequest = 260,
                HeightRequest = 55,
                HorizontalOptions = LayoutOptions.Center
            };

            automaatNupp.Clicked += async (sender, e) =>
            {
                await AlustaAutomaatrežiimi();
            };

            // SISSE/VÄLJA NUPPUDE PAIGUTUS

            HorizontalStackLayout nupud = new HorizontalStackLayout
            {
                Spacing = 15,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 15, 0, 5)
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
            fooriPaigutus.Children.Add(automaatNupp);

            // GRID

            Grid ekraan = new Grid();

            // Esimene kiht = taust
            ekraan.Children.Add(taust);

            // Teine kiht = valgusfoor
            ekraan.Children.Add(fooriPaigutus);

            // Kuvame ekraanil
            Content = ekraan;

            // TULEDE KLIKKIMINE

            LisaPunaseKlõps();
            LisaKollaseKlõps();
            LisaRoheliseKlõps();
        }

        // FOORI SISSELÜLITAMINE

        private void FoorSisse()
        {
            // Kui automaatrežiim töötab, peatame selle
            if (automaatrežiim)
            {
                automaatTühistus?.Cancel();
                automaatrežiim = false;
            }

            foorOn = true;

            punane.Color = Colors.Red;
            kollane.Color = Colors.Yellow;
            roheline.Color = Colors.Green;

            pealkiri.Text = "Vali valgus";
        }

        // FOORI VÄLJALÜLITAMINE

        private void FoorVälja()
        {
            // Peatame automaatrežiimi
            automaatTühistus?.Cancel();

            automaatrežiim = false;
            foorOn = false;

            // Kõik tuled halliks
            punane.Color = Colors.Gray;
            kollane.Color = Colors.Gray;
            roheline.Color = Colors.Gray;

            // Näitame tulesid uuesti
            punane.Opacity = 1;
            kollane.Opacity = 1;
            roheline.Opacity = 1;

            pealkiri.Text = "Lülita foor sisse";
        }

        // AUTOMAATREŽIIM

        private async Task AlustaAutomaatrežiimi()
        {
            // Kui automaatrežiim juba töötab, siis ei käivitata teist tsüklit
            if (automaatrežiim)
            {
                return;
            }

            foorOn = true;
            automaatrežiim = true;

            automaatTühistus = new CancellationTokenSource();

            CancellationToken token = automaatTühistus.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    // PÄEVAREŽIIM

                    pealkiri.Text = "Päevarežiim";

                    // PUNANE
                    punane.Color = Colors.Red;
                    kollane.Color = Colors.Gray;
                    roheline.Color = Colors.Gray;

                    await punane.ScaleTo(1.2, 300);
                    await punane.ScaleTo(1.0, 300);

                    await Task.Delay(1400, token);

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    // KOLLANE
                    punane.Color = Colors.Gray;
                    kollane.Color = Colors.Yellow;
                    roheline.Color = Colors.Gray;

                    await kollane.ScaleTo(1.2, 300);
                    await kollane.ScaleTo(1.0, 300);

                    await Task.Delay(1400, token);

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    // ROHELINE
                    punane.Color = Colors.Gray;
                    kollane.Color = Colors.Gray;
                    roheline.Color = Colors.Green;

                    await roheline.ScaleTo(1.2, 300);
                    await roheline.ScaleTo(1.0, 300);

                    await Task.Delay(1400, token);

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    // ÖÖREŽIIM

                    pealkiri.Text = "Öörežiim";

                    // Kõik peale kollase välja
                    punane.Color = Colors.Gray;
                    roheline.Color = Colors.Gray;
                    kollane.Color = Colors.Yellow;

                    // Kollane hakkab vilkuma(u 4sek)

                    for (int i = 0; i < 4; i++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        // Kollane nähtavaks
                        await kollane.FadeTo(1, 300);

                        await kollane.ScaleTo(1.15, 200);
                        await kollane.ScaleTo(1.0, 200);

                        await Task.Delay(500, token);

                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        // Kollane tumedaks
                        await kollane.FadeTo(0.2, 300);

                        await Task.Delay(500, token);
                    }

                    // Pärast öörežiimi muutub kollane jälle nähtavaks
                    kollane.Opacity = 1;

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    // Tsükli järgmine ring: päev, öö, päev
                }
            }
            catch (TaskCanceledException)
            {
                // Automaatrežiim peatati nupuga "Sisse" või "Välja".
            }
            finally
            {
                automaatrežiim = false;
            }
        }

        // PUNASE TULE KLIKK

        private void LisaPunaseKlõps()
        {
            TapGestureRecognizer klõps = new TapGestureRecognizer();

            klõps.Tapped += async (sender, e) =>
            {
                if (foorOn && !automaatrežiim)
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
                if (foorOn && !automaatrežiim)
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
                if (foorOn && !automaatrežiim)
                {
                    pealkiri.Text = "Sõida";

                    await roheline.ScaleTo(1.2, 150);
                    await roheline.ScaleTo(1.0, 150);
                }
            };

            roheline.GestureRecognizers.Add(klõps);
        }

        // LEHELT LAHKUMISEL PEATUB AUTOMAATREŽIIM

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            automaatTühistus?.Cancel();
            automaatrežiim = false;
        }
    }
}