﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;

namespace DICUI
{
    public partial class MainWindow : Window
    {
        // Private paths
        private string defaultOutputPath;
        private string dicPath;
        private string psxtPath;
        private string sgRawPath;
        private string subdumpPath;

        // Private UI-related variables
        private List<Tuple<char, string>> _drives { get; set; }
        private List<int> _driveSpeeds { get { return new List<int> { 1, 2, 3, 4, 6, 8, 12, 16, 20, 24, 32, 40, 44, 48, 52, 56, 72 }; } }
        private List<Tuple<string, KnownSystem?, DiscType?>> _systems { get; set; }
        private Process childProcess { get; set; }
        private Window childWindow { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Get all settings
            GetSettings();

            // Populate the list of systems
            PopulateSystems();

            // Populate the list of drives
            PopulateDrives();

            // Populate the list of drive speeds
            PopulateDriveSpeeds();
            SetSupportedDriveSpeed();
        }

        #region Events

        private void btn_StartStop_Click(object sender, RoutedEventArgs e)
        {
            // Dump or stop the dump
            if ((string)btn_StartStop.Content == UIElements.StartDumping)
            {
                StartDumping();
            }
            else if ((string)btn_StartStop.Content == UIElements.StopDumping)
            {
                CancelDumping();
                EjectDisc();
            }
        }

        private void btn_OutputDirectoryBrowse_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder();
            EnsureDiscInformation();
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            PopulateDrives();
            SetSupportedDriveSpeed();
            EnsureDiscInformation();
        }

        private void cmb_DiscType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetOutputNames();
            EnsureDiscInformation();
        }

        private void cmb_DriveLetter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSupportedDriveSpeed();
            GetOutputNames();
            EnsureDiscInformation();
        }

        private void cmb_DriveSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnsureDiscInformation();
        }

        private void tbr_Properties_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void tbr_Properties_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void btn_Settings_Accept_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            childWindow.Close();
            GetSettings();
        }

        private void btn_Settings_Cancel_Click(object sender, RoutedEventArgs e)
        {
            childWindow.Close();
        }

        private void txt_OutputFilename_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnsureDiscInformation();
        }

        private void txt_OutputDirectory_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnsureDiscInformation();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get a complete list of supported systems and fill the combo box
        /// </summary>
        private void PopulateSystems()
        {
            _systems = Utilities.CreateListOfSystems();
            cmb_DiscType.ItemsSource = _systems;
            cmb_DiscType.DisplayMemberPath = "Item1";
            cmb_DiscType.SelectedIndex = 0;
            cmb_DiscType_SelectionChanged(null, null);

            btn_StartStop.IsEnabled = false;
        }

        /// <summary>
        /// Get a complete list of active disc drives and fill the combo box
        /// </summary>
        /// <remarks>TODO: Find a way for this to periodically run, or have it hook to a "drive change" event</remarks>
        private void PopulateDrives()
        {
            // Populate the list of drives and add it to the combo box
            _drives = Utilities.CreateListOfDrives();
            cmb_DriveLetter.ItemsSource = _drives;
            cmb_DriveLetter.DisplayMemberPath = "Item1";
            cmb_DriveLetter.SelectedIndex = 0;
            cmb_DriveLetter_SelectionChanged(null, null);

            if (cmb_DriveLetter.Items.Count > 0)
            {
                lbl_Status.Content = "Valid optical disc found! Choose your Disc Type";
                btn_StartStop.IsEnabled = true;
            }
            else
            {
                lbl_Status.Content = "No valid optical disc found!";
                btn_StartStop.IsEnabled = false;
            }
        }

        /// <summary>
        /// Get a complete list of (possible) disc drive speeds, and fill the combo box
        /// </summary>
        private void PopulateDriveSpeeds()
        {
            cmb_DriveSpeed.ItemsSource = _driveSpeeds;
            cmb_DriveSpeed.SelectedItem = 8;
        }

        /// <summary>
        /// Browse for an output folder
        /// </summary>
        private void BrowseFolder()
        {
            WinForms.FolderBrowserDialog folderDialog = new WinForms.FolderBrowserDialog { ShowNewFolderButton = false, SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory };
            WinForms.DialogResult result = folderDialog.ShowDialog();

            if (result == WinForms.DialogResult.OK)
            {
                txt_OutputDirectory.Text = folderDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Begin the dumping process using the given inputs
        /// </summary>
        private async void StartDumping()
        {
            // Local variables
            string driveLetter = cmb_DriveLetter.Text;
            string outputDirectory = txt_OutputDirectory.Text;
            string outputFilename = txt_OutputFilename.Text;
            btn_StartStop.Content = UIElements.StopDumping;

            // Get the currently selected item
            var selected = cmb_DiscType.SelectedValue as Tuple<string, KnownSystem?, DiscType?>;

            // Validate that everything is good
            if (string.IsNullOrWhiteSpace(txt_CustomParameters.Text)
                || !Utilities.ValidateParameters(txt_CustomParameters.Text))
            {
                lbl_Status.Content = "Error! Current configuration is not supported!";
                btn_StartStop.Content = UIElements.StartDumping;
                return;
            }

            // Validate that the required program exits
            if (!File.Exists(dicPath))
            {
                lbl_Status.Content = "Error! Could not find DiscImageCreator!";
                btn_StartStop.Content = UIElements.StartDumping;
                return;
            }

            // If a complete dump already exists
            if (DumpInformation.FoundAllFiles(outputDirectory, outputFilename, selected.Item3))
            {
                MessageBoxResult result = MessageBox.Show("A complete dump already exists! Are you sure you want to overwrite?", "Overwrite?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.No || result == MessageBoxResult.Cancel || result == MessageBoxResult.None)
                {
                    lbl_Status.Content = "Dumping aborted!";
                    btn_StartStop.Content = UIElements.StartDumping;
                    return;
                }
            }

            lbl_Status.Content = "Beginning dumping process";
            string parameters = txt_CustomParameters.Text;

            await Task.Run(() =>
            {
                childProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = dicPath,
                        Arguments = parameters,
                    },
                };
                childProcess.Start();
                childProcess.WaitForExit();
            });

            if (chk_EjectWhenDone.IsChecked == true)
            {
                EjectDisc();
            }

            // Special cases
            switch (selected.Item2)
            {
                case KnownSystem.MicrosoftXBOXOne:
                case KnownSystem.SonyPlayStation4:
                    if (!File.Exists(sgRawPath))
                    {
                        lbl_Status.Content = "Error! Could not find sg-raw!";
                        break;
                    }

                    await Task.Run(() =>
                    {
                        childProcess = new Process()
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = sgRawPath,
                                Arguments = "-v -r 4100 -R " + driveLetter + ": " + "ad 01 00 00 00 00 00 00 10 04 00 00 -o \"PIC.bin\""
                            },
                        };
                        childProcess.Start();
                        childProcess.WaitForExit();
                    });
                    break;
                case KnownSystem.SegaSaturn:
                    if (!File.Exists(subdumpPath))
                    {
                        lbl_Status.Content = "Error! Could not find subdump!";
                        break;
                    }

                    await Task.Run(() =>
                    {
                        childProcess = new Process()
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = subdumpPath,
                                Arguments = "-i " + driveLetter + ": -f " + Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(outputFilename) + "_subdump.sub") + "-mode 6 -rereadnum 25 -fix 2",
                            },
                        };
                        childProcess.Start();
                        childProcess.WaitForExit();
                    });
                    break;
                case KnownSystem.SonyPlayStation:
                    if (!File.Exists(psxtPath))
                    {
                        lbl_Status.Content = "Error! Could not find psxt001z!";
                        break;
                    }

                    // Invoke the program with all 3 configurations
                    // TODO: Use these outputs for PSX information
                    await Task.Run(() =>
                    {
                        childProcess = new Process()
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = psxtPath,
                                Arguments = "\"" + DumpInformation.GetFirstTrack(outputDirectory, outputFilename) + "\" > " + "\"" + Path.Combine(outputDirectory, "psxt001z.txt"),
                            },
                        };
                        childProcess.Start();
                        childProcess.WaitForExit();

                        childProcess = new Process()
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = psxtPath,
                                Arguments = "--libcrypt \"" + Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(outputFilename) + ".sub") + "\" > \"" + Path.Combine(outputDirectory, "libcrypt.txt"),
                            },
                        };
                        childProcess.Start();
                        childProcess.WaitForExit();

                        childProcess = new Process()
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = psxtPath,
                                Arguments = "--libcryptdrvfast " + driveLetter + " > " + "\"" + Path.Combine(outputDirectory, "libcryptdrv.log"),
                            },
                        };
                        childProcess.Start();
                        childProcess.WaitForExit();
                    });
                    break;
            }

            // Check to make sure that the output had all the correct files
            if (!DumpInformation.FoundAllFiles(outputDirectory, outputFilename, selected.Item3))
            {
                lbl_Status.Content = "Error! Please check output directory as dump may be incomplete!";
                btn_StartStop.Content = UIElements.StartDumping;
                EjectDisc();
                return;
            }

            lbl_Status.Content = "Dumping complete!";
            EjectDisc();

            Dictionary<string, string> templateValues = DumpInformation.ExtractOutputInformation(outputDirectory, outputFilename, selected.Item2, selected.Item3);
            List<string> formattedValues = DumpInformation.FormatOutputData(templateValues, selected.Item2, selected.Item3);
            bool success = DumpInformation.WriteOutputData(outputDirectory, outputFilename, formattedValues);

            btn_StartStop.Content = UIElements.StartDumping;
        }

        /// <summary>
        /// Cancel an in-progress dumping process
        /// </summary>
        private void CancelDumping()
        {
            try
            {
                childProcess.Kill();
            }
            catch
            { }
        }

        /// <summary>
        /// Eject the disc using DIC
        /// </summary>
        private async void EjectDisc()
        {
            // Validate that the required program exits
            if (!File.Exists(dicPath))
            {
                return;
            }

            CancelDumping();

            var driveTuple = cmb_DriveLetter.SelectedItem as Tuple<char, string>;
            await Task.Run(() =>
            {
                childProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = dicPath,
                        Arguments = DICCommands.EjectCommand + " " + driveTuple.Item1,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                    },
                };
                childProcess.Start();
                childProcess.WaitForExit();
            });
        }

        /// <summary>
        /// Ensure information is consistent with the currently selected disc type
        /// </summary>
        private void EnsureDiscInformation()
        {
            // If we're on a separator, go to the next item
            var tuple = cmb_DiscType.SelectedItem as Tuple<string, KnownSystem?, DiscType?>;
            if (tuple.Item2 == null && tuple.Item3 == null)
            {
                cmb_DiscType.SelectedIndex++;
                tuple = cmb_DiscType.SelectedItem as Tuple<string, KnownSystem?, DiscType?>;
            }

            // If we're on an unsupported type, update the status accordingly
            switch (tuple.Item3)
            {
                case DiscType.NONE:
                    lbl_Status.Content = "Please select a valid disc type";
                    btn_StartStop.IsEnabled = false;
                    break;
                case DiscType.GameCubeGameDisc:
                case DiscType.GDROM:
                    lbl_Status.Content = string.Format("{0} discs are partially supported by DIC", Utilities.DiscTypeToString(tuple.Item3));
                    btn_StartStop.IsEnabled = true;
                    break;
                case DiscType.HDDVD:
                case DiscType.UMD:
                case DiscType.WiiOpticalDisc:
                case DiscType.WiiUOpticalDisc:
                    lbl_Status.Content = string.Format("{0} discs are not currently supported by DIC", Utilities.DiscTypeToString(tuple.Item3));
                    btn_StartStop.IsEnabled = false;
                    break;
                default:
                    lbl_Status.Content = string.Format("{0} ready to dump", Utilities.DiscTypeToString(tuple.Item3));
                    btn_StartStop.IsEnabled = true;
                    break;
            }

            // If we're in a type that doesn't support drive speeds
            switch (tuple.Item3)
            {
                case DiscType.BD25:
                case DiscType.BD50:
                    cmb_DriveSpeed.IsEnabled = false;
                    break;
                default:
                    cmb_DriveSpeed.IsEnabled = true;
                    break;
            }

            // Special case for Custom input
            if (tuple.Item1 == "Custom Input" && tuple.Item2 == KnownSystem.NONE && tuple.Item3 == DiscType.NONE)
            {
                txt_CustomParameters.IsEnabled = true;
                txt_OutputFilename.IsEnabled = false;
                txt_OutputDirectory.IsEnabled = false;
                btn_OutputDirectoryBrowse.IsEnabled = false;
                cmb_DriveLetter.IsEnabled = false;
                cmb_DriveSpeed.IsEnabled = false;
                btn_StartStop.IsEnabled = true;
                lbl_Status.Content = "User input mode";
            }
            else
            {
                txt_CustomParameters.IsEnabled = false;
                txt_OutputFilename.IsEnabled = true;
                txt_OutputDirectory.IsEnabled = true;
                btn_OutputDirectoryBrowse.IsEnabled = true;
                cmb_DriveLetter.IsEnabled = true;
                cmb_DriveSpeed.IsEnabled = true;

                // Populate with the correct params for inputs (if we're not on the default option)
                if (cmb_DiscType.SelectedIndex > 0)
                {
                    var selected = cmb_DiscType.SelectedValue as Tuple<string, KnownSystem?, DiscType?>;
                    var driveletter = cmb_DriveLetter.SelectedValue as Tuple<char, string>;
                    string discType = Utilities.GetBaseCommand(selected.Item3);
                    List<string> defaultParams = Utilities.GetDefaultParameters(selected.Item2, selected.Item3);
                    txt_CustomParameters.Text = discType
                        + " " + driveletter.Item1
                        + " \"" + Path.Combine(txt_OutputDirectory.Text, txt_OutputFilename.Text) + "\" "
                        + (selected.Item3 != DiscType.BD25 && selected.Item3 != DiscType.BD50 ? (int)cmb_DriveSpeed.SelectedItem + " " : "")
                        + string.Join(" ", defaultParams);
                }
            }
        }

        /// <summary>
        /// Get the default output directory name from the currently selected drive
        /// </summary>
        private void GetOutputNames()
        {
            var driveTuple = cmb_DriveLetter.SelectedItem as Tuple<char, string>;
            var discTuple = cmb_DiscType.SelectedItem as Tuple<string, KnownSystem?, DiscType?>;

            if (driveTuple != null && discTuple != null)
            {
                txt_OutputDirectory.Text = Path.Combine(defaultOutputPath, driveTuple.Item2);
                txt_OutputFilename.Text = driveTuple.Item2 + Utilities.GetDefaultExtension(discTuple.Item3);
            }
            else
            {
                txt_OutputDirectory.Text = defaultOutputPath;
                txt_OutputFilename.Text = "disc.bin";
            }
        }

        /// <summary>
        /// Get the highest supported drive speed as reported by DiscImageCreator
        /// </summary>
        private void SetSupportedDriveSpeed()
        {
            // Get the drive letter from the selected item
            var selected = cmb_DriveLetter.SelectedItem as Tuple<char, string>;
            if (selected == null)
            {
                return;
            }

            // Validate that the required program exits
            if (!File.Exists(dicPath))
            {
                return;
            }

            char driveLetter = selected.Item1;
            childProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = dicPath,
                    Arguments = DICCommands.DriveSpeedCommand + " " + driveLetter,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                },
            };
            childProcess.Start();
            childProcess.WaitForExit();
            string output = childProcess.StandardOutput.ReadToEnd();

            int index = output.IndexOf("ReadSpeedMaximum:");
            string readspeed = Regex.Match(output.Substring(index), @"ReadSpeedMaximum: [0-9]+KB/sec \(([0-9]*)x\)").Groups[1].Value;
            if (!Int32.TryParse(readspeed, out int speed))
            {
                return;
            }

            cmb_DriveSpeed.SelectedValue = speed;
        }

        /// <summary>
        /// Show all user-configurable settings in a new window
        /// </summary>
        private void ShowSettings()
        {
            // Create the child window for settings
            childWindow = new Window()
            {
                ShowInTaskbar = false,
                Owner = Application.Current.MainWindow,
                Width = 500,
                Height = 250,
                ResizeMode = ResizeMode.NoResize,
            };

            // Create the new Grid-based window
            var grid = new Grid
            {
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = (GridLength)(new GridLengthConverter().ConvertFromString(String.Format("{0:n1}*", 1.2))) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = (GridLength)(new GridLengthConverter().ConvertFromString(String.Format("{0:n1}*", 2.5))) });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            // Create all of the individual items in the panel
            Label dicPathLabel = new Label();
            dicPathLabel.Content = "DiscImageCreator Path:";
            dicPathLabel.FontWeight = (FontWeight)(new FontWeightConverter().ConvertFromString("Bold"));
            dicPathLabel.VerticalAlignment = VerticalAlignment.Center;
            dicPathLabel.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(dicPathLabel, 0);
            Grid.SetColumn(dicPathLabel, 0);

            TextBox dicPathSetting = new TextBox();
            dicPathSetting.Text = ConfigurationManager.AppSettings["dicPath"];
            dicPathSetting.VerticalAlignment = VerticalAlignment.Center;
            dicPathSetting.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid.SetRow(dicPathSetting, 0);
            Grid.SetColumn(dicPathSetting, 1);

            Label psxt001zPathLabel = new Label();
            psxt001zPathLabel.Content = "psxt001z Path:";
            psxt001zPathLabel.FontWeight = (FontWeight)(new FontWeightConverter().ConvertFromString("Bold"));
            psxt001zPathLabel.VerticalAlignment = VerticalAlignment.Center;
            psxt001zPathLabel.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(psxt001zPathLabel, 1);
            Grid.SetColumn(psxt001zPathLabel, 0);

            TextBox psxt001zPathSetting = new TextBox();
            psxt001zPathSetting.Text = ConfigurationManager.AppSettings["psxt001zPath"];
            psxt001zPathSetting.VerticalAlignment = VerticalAlignment.Center;
            psxt001zPathSetting.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid.SetRow(psxt001zPathSetting, 1);
            Grid.SetColumn(psxt001zPathSetting, 1);

            Label sgRawPathLabel = new Label();
            sgRawPathLabel.Content = "sg-raw Path:";
            sgRawPathLabel.FontWeight = (FontWeight)(new FontWeightConverter().ConvertFromString("Bold"));
            sgRawPathLabel.VerticalAlignment = VerticalAlignment.Center;
            sgRawPathLabel.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(sgRawPathLabel, 2);
            Grid.SetColumn(sgRawPathLabel, 0);

            TextBox sgRawPathSetting = new TextBox();
            sgRawPathSetting.Text = ConfigurationManager.AppSettings["sgRawPath"];
            sgRawPathSetting.VerticalAlignment = VerticalAlignment.Center;
            sgRawPathSetting.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid.SetRow(sgRawPathSetting, 2);
            Grid.SetColumn(sgRawPathSetting, 1);

            Label subdumpPathLabel = new Label();
            subdumpPathLabel.Content = "subdump Path:";
            subdumpPathLabel.FontWeight = (FontWeight)(new FontWeightConverter().ConvertFromString("Bold"));
            subdumpPathLabel.VerticalAlignment = VerticalAlignment.Center;
            subdumpPathLabel.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(subdumpPathLabel, 3);
            Grid.SetColumn(subdumpPathLabel, 0);

            TextBox subdumpPathSetting = new TextBox();
            subdumpPathSetting.Text = ConfigurationManager.AppSettings["subdumpPath"];
            subdumpPathSetting.VerticalAlignment = VerticalAlignment.Center;
            subdumpPathSetting.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid.SetRow(subdumpPathSetting, 3);
            Grid.SetColumn(subdumpPathSetting, 1);

            Label defaultOutputPathLabel = new Label();
            defaultOutputPathLabel.Content = "Default Output Path:";
            defaultOutputPathLabel.FontWeight = (FontWeight)(new FontWeightConverter().ConvertFromString("Bold"));
            defaultOutputPathLabel.VerticalAlignment = VerticalAlignment.Center;
            defaultOutputPathLabel.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(defaultOutputPathLabel, 4);
            Grid.SetColumn(defaultOutputPathLabel, 0);

            TextBox defaultOutputPathSetting = new TextBox();
            defaultOutputPathSetting.Text = ConfigurationManager.AppSettings["defaultOutputPath"];
            defaultOutputPathSetting.VerticalAlignment = VerticalAlignment.Center;
            defaultOutputPathSetting.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid.SetRow(defaultOutputPathSetting, 4);
            Grid.SetColumn(defaultOutputPathSetting, 1);

            var buttonGrid = new Grid
            {
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(buttonGrid, 5);
            Grid.SetColumn(buttonGrid, 0);
            Grid.SetColumnSpan(buttonGrid, 2);

            Button acceptButton = new Button();
            acceptButton.Name = "btn_Settings_Accept";
            acceptButton.Content = "Accept";
            acceptButton.Click += btn_Settings_Accept_Click;
            acceptButton.VerticalAlignment = VerticalAlignment.Center;
            acceptButton.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(acceptButton, 0);
            Grid.SetColumn(acceptButton, 0);

            Button cancelButton = new Button();
            cancelButton.Name = "btn_Settings_Cancel";
            cancelButton.Content = "Cancel";
            cancelButton.Click += btn_Settings_Cancel_Click;
            cancelButton.VerticalAlignment = VerticalAlignment.Center;
            cancelButton.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(cancelButton, 0);
            Grid.SetColumn(cancelButton, 1);

            buttonGrid.Children.Add(acceptButton);
            buttonGrid.Children.Add(cancelButton);

            // Add all of the UI elements
            grid.Children.Add(dicPathLabel);
            grid.Children.Add(dicPathSetting);
            grid.Children.Add(psxt001zPathLabel);
            grid.Children.Add(psxt001zPathSetting);
            grid.Children.Add(sgRawPathLabel);
            grid.Children.Add(sgRawPathSetting);
            grid.Children.Add(subdumpPathLabel);
            grid.Children.Add(subdumpPathSetting);
            grid.Children.Add(defaultOutputPathLabel);
            grid.Children.Add(defaultOutputPathSetting);
            grid.Children.Add(buttonGrid);

            // Now show the child window
            childWindow.Content = grid;
            childWindow.Show();
        }

        /// <summary>
        /// Save settings from the child window, if possible
        /// </summary>
        private void SaveSettings()
        {
            // If the child window is disposed, we don't think about it
            if (childWindow == null)
            {
                return;
            }

            // Clear the old settings and set new ones
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configFile.AppSettings.Settings.Remove("dicPath");
            configFile.AppSettings.Settings.Add("dicPath", ((TextBox)(((Grid)childWindow.Content).Children[1])).Text);
            configFile.AppSettings.Settings.Remove("psxt001zPath");
            configFile.AppSettings.Settings.Add("psxt001zPath", ((TextBox)(((Grid)childWindow.Content).Children[3])).Text);
            configFile.AppSettings.Settings.Remove("sgRawPath");
            configFile.AppSettings.Settings.Add("sgRawPath", ((TextBox)(((Grid)childWindow.Content).Children[5])).Text);
            configFile.AppSettings.Settings.Remove("subdumpPath");
            configFile.AppSettings.Settings.Add("subdumpPath", ((TextBox)(((Grid)childWindow.Content).Children[7])).Text);
            configFile.AppSettings.Settings.Remove("defaultOutputPath");
            configFile.AppSettings.Settings.Add("defaultOutputPath", ((TextBox)(((Grid)childWindow.Content).Children[9])).Text);
            configFile.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// Get settings from the configuration, if possible
        /// </summary>
        private void GetSettings()
        {
            dicPath = ConfigurationManager.AppSettings["dicPath"] ?? "Programs\\DiscImageCreator.exe";
            psxtPath = ConfigurationManager.AppSettings["psxt001zPath"] ?? "psxt001z.exe";
            sgRawPath = ConfigurationManager.AppSettings["sgRawPath"] ?? "sg_raw.exe";
            subdumpPath = ConfigurationManager.AppSettings["subdumpPath"] ?? "subdump.exe";
            defaultOutputPath = ConfigurationManager.AppSettings["defaultOutputPath"] ?? "ISO";
        }

        #endregion
    }
}
