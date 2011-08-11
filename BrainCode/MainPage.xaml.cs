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
                () => {
                    if (Input.Text != string.Empty) {
                        char next = Input.Text[0];
                        Input.Text = Input.Text.Substring(1);
                        return next;
                    } else {
                        return (char)0;
                    }
                }
            );

            bf.Run();

            MessageBox.Show(output.ToString());
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) {
            // hello world program
            //Code.Text = "++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++.>+.+++++++..+++.>++.<<+++++++++++++++.>.+++.------.--------.>+.>.";

            // cat program
            //Code.Text = ",[.,]";
            //Input.Text = "echo me this";

            // addition
            Code.Text = ",>++++++[<-------->-],[<+>-]<.";
            Input.Text = "43";
        }
    }
}