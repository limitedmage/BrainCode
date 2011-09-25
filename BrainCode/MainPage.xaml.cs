using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;

namespace BrainCode {
    public partial class MainPage : PhoneApplicationPage {
        private Dictionary<string, string> _samples = new Dictionary<string, string>() {
            {"Current", ""},
            {"Hello world", "++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++.>+.+++++++..+++.>++.<<+++++++++++++++.>.+++.------.--------.>+.>."},
            {"Print input (cat)", ",[.,]"},
            {"Single-digit Addition", ",>++++++[<-------->-],[<+>-]<."},
            {"Blank", ""}
        };

        private IsolatedStorageSettings _settings = IsolatedStorageSettings.ApplicationSettings;
        
        // Constructor
        public MainPage() {
            InitializeComponent();

            SamplePicker.ItemsSource = _samples;

            if (_settings.Contains("code")) {
                Code.Text = (string) _settings["code"];
                _samples["Current"] = Code.Text;
            } else {
                Code.Text = _samples["Hello world"];
                _samples["Current"] = Code.Text;
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e) {
            _settings["code"] = Code.Text;

            base.OnNavigatedFrom(e);
        }

        private void Run_Click(object sender, EventArgs e) {
            var ip = new InputPrompt {Title = "Input string"};
            ip.Completed += (s, ev) => {
                var result = ev.Result;
                var output = new StringBuilder();

                var bf = new Brainfuck(
                    Code.Text,
                    c => output.Append(c),
                    () => {
                        if (result != string.Empty) {
                            var next = result[0];
                            result = result.Substring(1);
                            return next;
                        } else {
                            return (char) 0;
                        }
                    },
                    trace => MessageBox.Show(trace, "Debug trace", MessageBoxButton.OK)
                );

                try {
                    bf.Run();
                    MessageBox.Show(output.ToString(), "Output", MessageBoxButton.OK);
                } catch (ArgumentException ae) {
                    MessageBox.Show(ae.Message, "Error", MessageBoxButton.OK);
                } catch (IndexOutOfRangeException iore) {
                    MessageBox.Show("You exceeded the limit of memory or stack.", "Error", MessageBoxButton.OK);
                }
            };
            ip.Show();
        }

        private void Key_Click(object sender, RoutedEventArgs e) {
            var b = e.OriginalSource as Button;
            if (b != null) {
                if (b == EnterButton) {
                    Code.Text += '\n';
                } else if (b == BackspaceButton) {
                    Code.Text = Code.Text.Substring(0, Code.Text.Length - 1);
                } else {
                    Code.Text += b.Content as string;
                }
            }
        }

        private void Samples_Click(object sender, EventArgs e) {
            SamplePicker.Open();
        }

        private bool _firing = false;
        private void SamplePicker_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!_firing) {
                _firing = true;

                _samples["Current"] = Code.Text;
                if (SamplePicker.SelectedItem != null) {
                    var item = (KeyValuePair<string, string>) SamplePicker.SelectedItem;
                    Code.Text = item.Value;
                    SamplePicker.SelectedIndex = 0;
                }

                _firing = false;
            }
        }

        private void Help_Click(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.Relative));
        }
    }
}