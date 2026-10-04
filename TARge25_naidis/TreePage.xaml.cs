using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace TARge25_naidis
{
    public partial class TreePage : ContentPage
    {
        // Näitab, kas puu on juba langetatud.
        private bool puuLangetatud = false;

        public TreePage()
        {
            InitializeComponent();

            // Algne kuupäev.
            kuupaevPicker.Date = DateTime.Today;

            // Algne kellaaeg.
            kellaaegPicker.Time = new TimeSpan(12, 0, 0);

            // Näitame algset kiirust.
            kiiruseLabel.Text = $"{kiiruseStepper.Value:0} ms";

            // ALGUS: roheline puu ilma õunte ja õiteta
            PeidaÕunad();
            PeidaÕied();
        }

        // TEGEVUSE KÄIVITAMINE

        private async void OnActionClicked(object sender, EventArgs e)
        {
            if (tegevusePicker.SelectedIndex == -1)
            {
                infoLabel.Text = "Palun vali tegevus!";
                return;
            }

            if (puuLangetatud)
            {
                infoLabel.Text =
                    "Puu on juba langetatud. Vajuta Lähtesta.";
                return;
            }

            string tegevus = tegevusePicker.SelectedItem?.ToString();

            switch (tegevus)
            {
                case "Kasva":
                    await Kasva();
                    break;

                case "Õitse":
                    await Õitse();
                    break;

                case "Värise":
                    await Värise();
                    break;

                case "Langeta":
                    await Langeta();
                    break;
            }
        }

        // KASVA

        private async Task Kasva()
        {
            infoLabel.Text = "Puu kasvab... 🌱";

            // Kasvamise ajal ei ole õunu ega õisi.
            PeidaÕunad();
            PeidaÕied();

            uint aeg = (uint)kiiruseStepper.Value;

            // Puu muutub suuremaks.
            await puu.ScaleTo(1.25, aeg);

            infoLabel.Text =
                "Puu kasvas suuremaks! 🌳";
        }

        // ÕITSE

        private async Task Õitse()
        {
            infoLabel.Text = "Puu hakkab õitsema... 🌸";

            // Õunu ei ole.
            PeidaÕunad();

            // Õied tulevad nähtavale.
            NäitaÕisi();

            uint aeg = (uint)kiiruseStepper.Value;

            // Õitsemine.
            await puu.ScaleTo(1.1, aeg / 2);
            await puu.ScaleTo(1.0, aeg / 2);

            infoLabel.Text =
                "Puu on õites! 🌸";
        }

        // VÄRISE

        private async Task Värise()
        {
            infoLabel.Text =
                "Tuul puhub ja õunad kukuvad! 🍃🍎";

            // Õisi ei ole.
            PeidaÕied();

            // Õunad tulevad nähtavale.
            NäitaÕunu();

            uint aeg = (uint)kiiruseStepper.Value;

            // Puu värisemine ja õunte kukkumine.
            Task puuVärin = VärisePuu(aeg);
            Task õunad = KukutaÕunad();

            await Task.WhenAll(puuVärin, õunad);

            infoLabel.Text =
                "Õunad kukkusid puu otsast alla! 🍎";
        }

        // PUU VÄRIN

        private async Task VärisePuu(uint aeg)
        {
            await puu.TranslateTo(-15, 0, aeg / 4);
            await puu.TranslateTo(15, 0, aeg / 4);
            await puu.TranslateTo(-12, 0, aeg / 4);
            await puu.TranslateTo(12, 0, aeg / 4);
            await puu.TranslateTo(0, 0, aeg / 4);
        }

        // ÕUNAD KUKUVAD

        private async Task KukutaÕunad()
        {
            Task õun1 = KukutaÕun(oun1, 130);
            Task õun2 = KukutaÕun(oun2, 180);
            Task õun3 = KukutaÕun(oun3, 150);

            await Task.WhenAll(õun1, õun2, õun3);
        }

        private async Task KukutaÕun(
            BoxView õun,
            double kaugus)
        {
            // Õun kukub alla.
            await õun.TranslateTo(0, kaugus, 700);

            // Õun muutub nähtamatuks.
            await õun.FadeTo(0, 300);

            õun.IsVisible = false;
        }

        // PUU LANGETAMINE

        private async Task Langeta()
        {
            // DatePicker võib anda nullable väärtuse.
            DateTime kuupaev =
                kuupaevPicker.Date ?? DateTime.Today;

            // TimePicker võib anda nullable väärtuse.
            TimeSpan kellaaeg =
                kellaaegPicker.Time ?? new TimeSpan(12, 0, 0);

            // Kontrollime talve.
            bool onTalv =
                kuupaev.Month == 12 ||
                kuupaev.Month == 1 ||
                kuupaev.Month == 2;

            // Kontrollime valget aega.
            bool onValgeAeg =
                kellaaeg >= new TimeSpan(8, 0, 0) &&
                kellaaeg < new TimeSpan(17, 0, 0);

            // Kui pole talv või pole valge aeg, siis puud ei langetata.
            if (!onTalv || !onValgeAeg)
            {
                infoLabel.Text =
                    "Puu langetamine on lubatud ainult talvel ja valgel ajal (08:00–17:00). ❌";

                return;
            }

            infoLabel.Text =
                "Puu langetatakse... 🪓";

            // Õunad ja õied lähevad ära.
            PeidaÕunad();
            PeidaÕied();

            uint aeg = (uint)kiiruseStepper.Value;

            // Puu langeb külili.
            await puu.RotateTo(90, aeg);

            puuLangetatud = true;

            infoLabel.Text =
                "Puu on langenud! 🌳";
        }

        // ÕUNAD

        private void NäitaÕunu()
        {
            oun1.IsVisible = true;
            oun2.IsVisible = true;
            oun3.IsVisible = true;

            oun1.Opacity = 1;
            oun2.Opacity = 1;
            oun3.Opacity = 1;

            oun1.TranslationY = 0;
            oun2.TranslationY = 0;
            oun3.TranslationY = 0;
        }

        private void PeidaÕunad()
        {
            oun1.IsVisible = false;
            oun2.IsVisible = false;
            oun3.IsVisible = false;

            oun1.Opacity = 1;
            oun2.Opacity = 1;
            oun3.Opacity = 1;

            oun1.TranslationY = 0;
            oun2.TranslationY = 0;
            oun3.TranslationY = 0;
        }

        // ÕIED

        private void NäitaÕisi()
        {
            õis1.IsVisible = true;
            õis2.IsVisible = true;
            õis3.IsVisible = true;
            õis4.IsVisible = true;

            õis1.Opacity = 1;
            õis2.Opacity = 1;
            õis3.Opacity = 1;
            õis4.Opacity = 1;
        }

        private void PeidaÕied()
        {
            õis1.IsVisible = false;
            õis2.IsVisible = false;
            õis3.IsVisible = false;
            õis4.IsVisible = false;
        }

        // SLIDER

        private void OnSliderChanged(
            object sender,
            ValueChangedEventArgs e)
        {
            võra1.Opacity = e.NewValue;
            võra2.Opacity = e.NewValue;
            võra3.Opacity = e.NewValue;
        }

        // STEPPER

        private void OnStepperChanged(
            object sender,
            ValueChangedEventArgs e)
        {
            kiiruseLabel.Text =
                $"{e.NewValue:0} ms";
        }

        // RESET

        private async void OnResetClicked(
            object sender,
            EventArgs e)
        {
            // Peatame animatsioonid.
            puu.CancelAnimations();

            oun1.CancelAnimations();
            oun2.CancelAnimations();
            oun3.CancelAnimations();

            // Puu tagasi algasendisse.
            puu.Rotation = 0;
            puu.Scale = 1;
            puu.TranslationX = 0;
            puu.TranslationY = 0;

            // Puu ei ole enam langetatud.
            puuLangetatud = false;

            // Taastame läbipaistvuse.
            võra1.Opacity = 1;
            võra2.Opacity = 1;
            võra3.Opacity = 1;

            lehistikuSlider.Value = 1;

            // Alguses ei ole õunu.
            PeidaÕunad();

            // Alguses ei ole õisi.
            PeidaÕied();

            // Tegevuse valik tühjaks.
            tegevusePicker.SelectedIndex = -1;

            infoLabel.Text =
                "Puu on lähtestatud! 🌳";

            await Task.Delay(100);
        }

        // LEHELT LAHKUMINE

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            puu.CancelAnimations();

            oun1.CancelAnimations();
            oun2.CancelAnimations();
            oun3.CancelAnimations();
        }
    }
}