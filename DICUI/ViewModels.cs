﻿using System;
using System.Windows.Media;
using DICUI.Windows;

namespace DICUI
{
    public class OptionsViewModel
    {
        private Options _options;

        public OptionsViewModel(Options options)
        {
            this._options = options;
        }

        public string InternalProgram
        {
            get { return _options.InternalProgram; }
            set { _options.InternalProgram = value; }
        }

        public bool QuietMode
        {
            get { return _options.QuietMode; }
            set { _options.QuietMode = value; }
        }

        public bool ParanoidMode
        {
            get { return _options.ParanoidMode; }
            set { _options.ParanoidMode = value; }
        }

        public bool ScanForProtection
        {
            get { return _options.ScanForProtection; }
            set { _options.ScanForProtection = value; }
        }

        public string RereadAmountForC2
        {
            get { return Convert.ToString(_options.RereadAmountForC2); }
            set
            {
                if (Int32.TryParse(value, out int result))
                    _options.RereadAmountForC2 = result;
            }
        }

        public bool AddPlaceholders
        {
            get { return _options.AddPlaceholders; }
            set { _options.AddPlaceholders = value; }
        }

        public bool PromptForDiscInformation
        {
            get { return _options.PromptForDiscInformation; }
            set { _options.PromptForDiscInformation = value; }
        }

        public bool IgnoreFixedDrives
        {
            get { return _options.IgnoreFixedDrives; }
            set { _options.IgnoreFixedDrives = value; }
        }

        public bool ResetDriveAfterDump
        {
            get { return _options.ResetDriveAfterDump; }
            set { _options.ResetDriveAfterDump = value; }
        }

        public bool SkipMediaTypeDetection
        {
            get { return _options.SkipMediaTypeDetection; }
            set { _options.SkipMediaTypeDetection = value; }
        }

        public bool SkipSystemDetection
        {
            get { return _options.SkipSystemDetection; }
            set { _options.SkipSystemDetection = value; }
        }

        public bool VerboseLogging
        {
            get { return _options.VerboseLogging; }
            set
            {
                _options.VerboseLogging = value;
                _options.Save();
            }
        }

        public bool OpenLogWindowAtStartup
        {
            get { return _options.OpenLogWindowAtStartup; }
            set
            {
                _options.OpenLogWindowAtStartup = value;
                _options.Save();
            }
        }

        public string Username
        {
            get { return _options.Username; }
            set { _options.Username = value; }
        }

        public string Password
        {
            get { return _options.Password; }
            set { _options.Password = value; }
        }
    }

    public class LoggerViewModel
    {
        private LogWindow _logWindow;

        public void SetWindow(LogWindow logWindow) => _logWindow = logWindow;

        public bool WindowVisible
        {
            get => _logWindow != null ? _logWindow.IsVisible : false;
            set
            {
                if (value)
                {
                    _logWindow.AdjustPositionToMainWindow();
                    _logWindow.Show();
                }
                else
                    _logWindow.Hide();
            }
        }

        public void VerboseLog(string text)
        {
            if (ViewModels.OptionsViewModel.VerboseLogging)
                _logWindow.AppendToTextBox(text, Brushes.Yellow);
        }

        public void VerboseLog(string format, params object[] args) => VerboseLog(string.Format(format, args));
        public void VerboseLogLn(string format, params object[] args) => VerboseLog(string.Format(format, args) + "\n");
    }

    public static class ViewModels
    {
        public static OptionsViewModel OptionsViewModel { get; set; }
        public static LoggerViewModel LoggerViewModel { get; set; } = new LoggerViewModel();
    }
}
