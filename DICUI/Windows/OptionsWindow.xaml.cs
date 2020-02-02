﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using DICUI.Web;
using Button = System.Windows.Controls.Button;
using TextBox = System.Windows.Controls.TextBox;

namespace DICUI.Windows
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private readonly MainWindow _mainWindow;
        private readonly Options _options;

        public OptionsWindow(MainWindow mainWindow, Options options)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _options = options;      
        }

        private OpenFileDialog CreateOpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dialog.Filter = "Executables (*.exe)|*.exe";
            dialog.FilterIndex = 0;
            dialog.RestoreDirectory = true;

            return dialog;
        }

        private FolderBrowserDialog CreateFolderBrowserDialog()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            return dialog;
        }

        private string[] PathSettings()
        {
            string[] pathSettings = { "ChefPath", "CreatorPath", "DefaultOutputPath", "SubDumpPath" };
            return pathSettings;
        }

        private TextBox TextBoxForPathSetting(string name)
        {
            return FindName(name + "TextBox") as TextBox;
        }

        private void BrowseForPathClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            // strips button prefix to obtain the setting name
            string pathSettingName = button.Name.Substring(0, button.Name.IndexOf("Button"));

            // TODO: hack for now, then we'll see
            bool shouldBrowseForPath = pathSettingName == "DefaultOutputPath";

            CommonDialog dialog = shouldBrowseForPath ? (CommonDialog)CreateFolderBrowserDialog() : CreateOpenFileDialog();
            using (dialog)
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string path;
                    bool exists;

                    if (shouldBrowseForPath)
                    {
                        path = (dialog as FolderBrowserDialog).SelectedPath;
                        exists = Directory.Exists(path);
                    }
                    else
                    {
                        path = (dialog as OpenFileDialog).FileName;
                        exists = File.Exists(path);
                    }

                    if (exists)
                        TextBoxForPathSetting(pathSettingName).Text = path;
                    else
                    {
                        System.Windows.MessageBox.Show(
                            "Specified path doesn't exists!",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
        }

        public void Refresh()
        {
            Array.ForEach(PathSettings(), setting => TextBoxForPathSetting(setting).Text = _options.Get(setting));

            DumpSpeedCDSlider.Value = _options.PreferredDumpSpeedCD;
            DumpSpeedDVDSlider.Value = _options.PreferredDumpSpeedDVD;
            DumpSpeedBDSlider.Value = _options.PreferredDumpSpeedBD;

            RedumpUsernameTextBox.Text = _options.Username;
            RedumpPasswordBox.Password = _options.Password;
        }

        #region Event Handlers

        private void OnAcceptClick(object sender, EventArgs e)
        {
            Array.ForEach(PathSettings(), setting => _options.Set(setting, TextBoxForPathSetting(setting).Text));

            _options.PreferredDumpSpeedCD = Convert.ToInt32(DumpSpeedCDSlider.Value);
            _options.PreferredDumpSpeedDVD = Convert.ToInt32(DumpSpeedDVDSlider.Value);
            _options.PreferredDumpSpeedBD = Convert.ToInt32(DumpSpeedBDSlider.Value);

            _options.Username = RedumpUsernameTextBox.Text;
            _options.Password = RedumpPasswordBox.Password;

            _options.Save();
            Hide();

            _mainWindow.OnOptionsUpdated();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            // just hide the window and don't care
            Hide();
        }

        private void OnRedumpTestClick(object sender, EventArgs e)
        {
            using (CookieAwareWebClient wc = new CookieAwareWebClient())
            {
                RedumpAccess access = new RedumpAccess();
                if (access.RedumpLogin(wc, RedumpUsernameTextBox.Text, RedumpPasswordBox.Password))
                    System.Windows.MessageBox.Show("Redump login credentials accepted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    System.Windows.MessageBox.Show("Redump login credentials denied!", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
