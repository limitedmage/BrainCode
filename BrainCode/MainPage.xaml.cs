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
using Microsoft.Phone.Controls;
using System.Text;

namespace BrainCode {
    public partial class MainPage : PhoneApplicationPage {
        // Constructor
        public MainPage() {
            InitializeComponent();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e) {
            StringBuilder output = new StringBuilder();

            var bf = new Brainfuck(
                Code.Text,
                (c) => output.Append(c),
                () => 'a'
            );

            bf.Run();

            MessageBox.Show(output.ToString());
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) {
            // hello world program
            Code.Text = "++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++.>+.+++++++..+++.>++.<<+++++++++++++++.>.+++.------.--------.>+.>.";
        }
    }
}