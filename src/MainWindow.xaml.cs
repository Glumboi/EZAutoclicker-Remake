using EZAutoclickerWPF.Image;
using EZAutoclickerWPF.Mouse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using Wpf.Ui.Controls;

namespace EZAutoclickerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow
    {
        #region extern imports

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        #endregion extern imports

        private enum HotkeyIDS
        {
            HOTKEY1 = 0,
            HOTKEY2 = 1
        }

        private HwndSource source;
        private static System.Windows.Threading.DispatcherTimer _mouseTimer = new System.Windows.Threading.DispatcherTimer();
        private static bool isRunning = false;
        private System.Threading.Timer mouseTimer;

        #region hotkey initialization

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(HwndHook);
            RegisterHotKey();
        }

        protected override void OnClosed(EventArgs e)
        {
            source.RemoveHook(HwndHook);
            source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }

        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            if (!RegisterHotKey(helper.Handle, (int)HotkeyIDS.HOTKEY1, 6, 0x79))
            {
                // handle error
            }
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, (int)HotkeyIDS.HOTKEY1);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case (int)HotkeyIDS.HOTKEY1:
                            StartClicking();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        #endregion hotkey initialization

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Checks the values of the settings and clicks the mouse depending on how they are set
        /// </summary>
        private void DoClicks()
        {
            if (Properties.Settings.Default.RightClick == false && Properties.Settings.Default.MiddleClick == false)
            {
                Click.DoLeftClick();
            }

            if (Properties.Settings.Default.RightClick == true && Properties.Settings.Default.MiddleClick == false)
            {
                Click.DoMouseRightClick();
            }

            if (Properties.Settings.Default.RightClick == false && Properties.Settings.Default.MiddleClick == true)
            {
                Click.DoMouseRightClick();
            }

            if (Properties.Settings.Default.RightClick == true && Properties.Settings.Default.MiddleClick == true)
            {
                StopClicking();
                System.Windows.MessageBox.Show("Can't enable middle and right click at the same time!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Starts/stops the click event timer
        /// </summary>
        /// <param name="buttonClick">Used so when you click the start button it doesnt stop directly</param>
        private void StartClicking(bool buttonClick = false)
        {
            if (isRunning && buttonClick == false)
            {
                StopClicking();
                return;
            }
            mouseTimer.Change(0, Int32.Parse(Speed_TextBox.Text));
            isRunning = true;
            UpdateIcons();
        }

        /// <summary>
        /// Stops the clicking by setting the timer's interval to infinite
        /// </summary>
        private void StopClicking()
        {
            mouseTimer.Change(Timeout.Infinite, Timeout.Infinite);
            isRunning = false;
            UpdateIcons();
        }

        /// <summary>
        /// Loads the settings of the app's properties
        /// </summary>
        private void LoadSettings()
        {
            MiddleClick_CheckBox.IsChecked = Properties.Settings.Default.MiddleClick;
            RightClick_CheckBox.IsChecked = Properties.Settings.Default.RightClick;
        }

        /// <summary>
        /// Saves the settings of the app's properties
        /// </summary>
        private void SaveSettings()
        {
            Properties.Settings.Default.MiddleClick = (bool)MiddleClick_CheckBox.IsChecked;
            Properties.Settings.Default.RightClick = (bool)RightClick_CheckBox.IsChecked;
        }

        /// <summary>
        /// Updates the icon of the app and titlebar depending on the current state of the clicker
        /// </summary>
        private void UpdateIcons()
        {
            if (isRunning)
            {
                TitleBar.Icon = ImageExt.ToImageSource(Properties.Resources.On);
                this.Icon = ImageExt.ToImageSource(Properties.Resources.On);
                return;
            }

            TitleBar.Icon = ImageExt.ToImageSource(Properties.Resources.Off);
            this.Icon = ImageExt.ToImageSource(Properties.Resources.Off);
        }

        #region eventhandlers

        private async void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateIcons();
            await VersionControl.Versions.CheckGitHubNewerVersion();
            mouseTimer = new System.Threading.Timer(MouseTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            RegisterHotKey();
            LoadSettings();
        }

        private void Github_HyperLink_Click(object sender, RoutedEventArgs e)
        {
            Web.WebBrowser.OpenUrl("https://github.com/Glumboi");
        }

        private void Project_HyperLink_Click(object sender, RoutedEventArgs e)
        {
            //Open project url in browser
        }

        private void MouseTimerCallback(Object state)
        {
            DoClicks();
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            StartClicking(true);
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            StopClicking();
        }

        private void Log_Button_Click(object sender, RoutedEventArgs e)
        {
            Logging.CreateLogs.MakeLog("_Info", "Info log created at the ");
        }

        private void MiddleClick_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void MiddleClick_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void RightClick_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void RightClick_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        #endregion eventhandlers
    }
}