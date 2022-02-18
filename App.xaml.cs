using DirectionsInformation.Models;
using DirectionsInformation.ViewModels;
using DirectionsInformation.Views.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;

namespace DirectionsInformation
{
    public partial class App : Application
    {
        private readonly Forms.NotifyIcon _notifyIcon;

        public App()
        {
            _notifyIcon = new Forms.NotifyIcon();

            AppConstants.Load();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new DirectionWindow
            {
                DataContext = new DirectionWindowViewModel(_notifyIcon)
            };
            MainWindow.Show();

            _notifyIcon.Icon = DirectionsInformation.Properties.Resources.planet_earth;
            //(Icon)Application.Current.FindResource("favicon"); //new System.Drawing.Icon("planet-earth.ico");
            _notifyIcon.Text = "Информация о точках";
            _notifyIcon.Visible = true;
            _notifyIcon.MouseClick += NotifyIcon_Click;
            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Открыть", null, ShowWindow);
            _notifyIcon.ContextMenuStrip.Items.Add("Выход", null, OnExitClicked);
            _notifyIcon.BalloonTipClicked += BallonTip_Click;
            base.OnStartup(e);
        }
        private void BallonTip_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }
        private void NotifyIcon_Click(object sender, Forms.MouseEventArgs e)
        {
            if (e.Button == Forms.MouseButtons.Left)
                ShowWindow();
        }
        private void ShowWindow([Optional] object sender, [Optional] EventArgs e)
        {
            MainWindow.Activate();
            MainWindow.Visibility = Visibility.Collapsed;
            MainWindow.Visibility = Visibility.Visible;
        }
        private void OnExitClicked(object sender, EventArgs e)
        {
            this.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();

            base.OnExit(e);
        }
    }
}