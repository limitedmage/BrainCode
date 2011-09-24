using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using System.Text;
using System.IO.IsolatedStorage;

namespace BrainCode {
    public partial class MainPage : PhoneApplicationPage {
        private Dictionary<string, string> _samples = new Dictionary<string, string>() {
            {"Hello world", "++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++.>+.+++++++..+++.>++.<<+++++++++++++++.>.+++.------.--------.>+.>."},
            {"Cat", ",[.,]"},
            {"Addition", ",>++++++[<-------->-],[<+>-]<."}
        };

        private IsolatedStorageSettings _settings = IsolatedStorageSettings.ApplicationSettings;
        
        // Constructor
        public MainPage() {
            InitializeComponent();

            SamplePicker.ItemsSource = _samples;

            if (_settings.Contains("code")) {
                Code.Text = (string) _settings["code"];
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

                bf.Run();

                MessageBox.Show(output.ToString(), "Output", MessageBoxButton.OK);
            };
            ip.Show();
        }

        private void Key_Click(object sender, RoutedEventArgs e) {
            var b = e.OriginalSource as Button;
            if (b != null) {
                var content = b.Content as string;
                switch (content) {
                    case "↵":
                        Code.Text += '\n';
                        break;
                    case "←":
                        Code.Text = Code.Text.Substring(0, Code.Text.Length - 1);
                        break;
                    default:
                        Code.Text += content;
                        break;
                }
            }
        }

        private void Samples_Click(object sender, EventArgs e) {
            SamplePicker.Open();
        }

        private void SamplePicker_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count > 0) {
                var item = (KeyValuePair<string, string>) e.AddedItems[0];
                Code.Text = item.Value;
            }
        }

        private void Help_Click(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.Relative));
        }
    }
}