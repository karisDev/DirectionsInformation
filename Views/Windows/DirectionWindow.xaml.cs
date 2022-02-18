using System;
using System.Windows;
using System.Windows.Input;

namespace DirectionsInformation.Views.Windows
{
    public partial class DirectionWindow : Window
    {
        public DirectionWindow()
        {
            InitializeComponent();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
                this.Hide();
            }
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                    var point = System.Windows.Forms.Control.MousePosition;
                    Left = point.X - (Width / 2);
                    Top = point.Y - 10;
                }

                DragMove();
            }
            catch { }
        }

        private void Home_Checked(object sender, RoutedEventArgs e)
        {
            MainTransitioner.SelectedIndex = 0;
        }

        private void Location_Checked(object sender, RoutedEventArgs e)
        {
            MainTransitioner.SelectedIndex = 1;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
