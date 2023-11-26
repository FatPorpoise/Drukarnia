using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Linq;

namespace Drukarnia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] strings = new string[10];
        private int quantityNum = 1;
        private float formatCost = 0.5f, paperColorMul = 1f, grammageMul = 2f, colorMul = 1f, twoSideMul = 1f, UVMul = 1f, expressCost = 0f, discount = 0f, costBefore = 0f, costAfter = 0f;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            strings = new string[] { "Przedmiot zamówienia: ", "1 szt.", FormatValue(), "", "", Grammage(), PrintoutOptions(), "" };
            UpdateSummary();
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                var text = "";
                if (quantity.Text.Length == 0)
                    quantity.Text = "1";
                else
                {
                    foreach (char c in quantity.Text)
                    {
                        if (char.IsDigit(c))
                        {
                            text += c;
                        }
                    }
                    var number = int.Parse(text);
                    if (number == 0)
                    {
                        text = "1";
                        quantityNum = 1;
                    }
                    else
                    {
                        text = number.ToString();
                        quantityNum = number;
                        discount = 0.01f * (quantityNum / 100);
                        if (discount > 0.1f)
                            discount = 0.1f;
                    }
                    quantity.Text = text;
                }
                strings[1] = quantity.Text + " szt.";
                UpdateSummary();
            }
        }
        private void FormatValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (formatLabel.IsLoaded)
            {
                switch (e.NewValue)
                {
                    case 1:
                        formatLabel.Content = "A5 — cena 20gr/szt.";
                        formatCost = 0.2f;
                        break;
                    case 2:
                        formatLabel.Content = "A4 — cena 50gr/szt.";
                        formatCost = 0.5f;
                        break;
                    case 3:
                        formatLabel.Content = "A3 — cena 1,25zł/szt.";
                        formatCost = 1.25f;
                        break;
                    case 4:
                        formatLabel.Content = "A2 — cena 3,13zł/szt.";
                        formatCost = 3.13f;
                        break;
                    case 5:
                        formatLabel.Content = "A1 — cena 7,83zł/szt.";
                        formatCost = 7.83f;
                        break;
                    case 6:
                        formatLabel.Content = "A0 — cena 19,58zł/szt.";
                        formatCost = 19.58f;
                        break;
                }
                strings[2] = FormatValue();
                UpdateSummary();
            }
        }
        private void ifColored_Click(object sender, RoutedEventArgs e)
        {
            if (ifColored.IsChecked == true)
            {
                paperColorMul = 1.5f;
                color.IsEnabled = true;
            }
            else
            {
                paperColorMul = 1f;
                color.IsEnabled = false;
            }
        }
        private void color_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (color.IsLoaded)
            {
                if (color.IsEnabled == true)
                {
                    color.SelectedItem = yellow;
                    strings[3] = ", kolorowy papier";
                }
                else
                {
                    color.SelectedItem = null;
                    strings[3] = "";
                    strings[4] = "";
                }
                UpdateSummary();
            }
        }
        private void color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (color.SelectedItem != null)
            {
                strings[4] = " (" + ((ComboBoxItem)color.SelectedItem).Content.ToString() + ")";
            }
            UpdateSummary();
        }
        private void GrammageClick(object sender, RoutedEventArgs e)
        {
            strings[5] = Grammage();
            UpdateSummary();
        }
        private void PrintoutOptionsClick(object sender, RoutedEventArgs e)
        {
            strings[6] = PrintoutOptions();
            UpdateSummary();
        }
        private void RealizationTimeClick(object sender, RoutedEventArgs e)
        {
            if (express.IsChecked == true)
            {
                strings[7] = ", ekspresowa realizacja";
                expressCost = 15f;
            }
            else
            {
                strings[7] = "";
                expressCost = 0f;
            }
            UpdateSummary();
        }
        private void OkClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Zamówienie zostało przyjęte do realizacji.", "Gotowe");
            var newWindow = new MainWindow();
            this.Close();
            newWindow.Show();
        }
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateSummary()
        {
            costBefore = quantityNum * formatCost * paperColorMul * grammageMul * colorMul * twoSideMul * UVMul + expressCost;
            costAfter = costBefore * (1f - discount);
            var text = "";
            foreach (var s in strings)
            {
                text += s;
            }
            summary.Text = text + "\nCena przed rabatem: " + costBefore.ToString("c2") + "\nNaliczony rabat: " + discount.ToString("P0") + "\nCena po rabacie: " + costAfter.ToString("c2");
        }
        private string Grammage()
        {
            var text = "";
            //if (!grammageGrid.IsLoaded)
            //    return text;
            foreach (RadioButton rb in grammageGrid.Children)
            {
                if (rb.IsChecked == true)
                {
                    text += ", " + rb.Tag.ToString();
                    switch (rb.Name)
                    {
                        case "gram1":
                            grammageMul = 1f;
                            break;
                        case "gram2":
                            grammageMul = 2f;
                            break;
                        case "gram3":
                            grammageMul = 2.5f;
                            break;
                        case "gram4":
                            grammageMul = 3f;
                            break;
                    }
                }
            }
            return text;
        }
        private string PrintoutOptions()
        {
            var text = "";
            //if (!printoutOptionsGrid.IsLoaded)
            //    return text;
            foreach (CheckBox cb in printoutOptionsGrid.Children)
            {
                if (cb.IsChecked == true)
                {
                    text += ", " + cb.Tag.ToString();
                    switch (cb.Name)
                    {
                        case "print1":
                            colorMul = 1.3f;
                            break;
                        case "print2":
                            twoSideMul = 1.5f;
                            break;
                        case "print3":
                            UVMul = 1.2f;
                            break;
                    }
                }
                else
                {
                    switch (cb.Name)
                    {
                        case "print1":
                            colorMul = 1f;
                            break;
                        case "print2":
                            twoSideMul = 1f;
                            break;
                        case "print3":
                            UVMul = 1f;
                            break;
                    }
                }
            }
            return text;
        }
        private string FormatValue()
        {
            return ", format " + formatLabel.Content.ToString().Substring(0, 2);
        }
    }
}
