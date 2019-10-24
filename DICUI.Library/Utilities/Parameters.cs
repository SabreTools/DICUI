﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DICUI.Data;

namespace DICUI.Utilities
{
    /// <summary>
    ///  Represents a generic set of DiscImageCreator parameters
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// Base DiscImageCreator command to run
        /// </summary>
        public DICCommand Command { get; set; }

        /// <summary>
        /// Drive letter or path to pass to DiscImageCreator
        /// </summary>
        public string DriveLetter { get; set; }

        /// <summary>
        /// Drive speed to set, if applicable
        /// </summary>
        public int? DriveSpeed { get; set; }

        /// <summary>
        /// Destination filename for DiscImageCreator output
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Optiarc drive output filename for merging
        /// </summary>
        public string OptiarcFilename { get; set; }

        /// <summary>
        /// Start LBA value for dumping specific sectors
        /// </summary>
        public int? StartLBAValue { get; set; }

        /// <summary>
        /// End LBA value for dumping specific sectors
        /// </summary>
        public int? EndLBAValue { get; set; }

        /// <summary>
        /// Set of flags to pass to DiscImageCreator
        /// </summary>
        private Dictionary<DICFlag, bool> _flags = new Dictionary<DICFlag, bool>();
        public bool this[DICFlag key]
        {
            get
            {
                if (_flags.ContainsKey(key))
                    return _flags[key];
                return false;
            }
            set
            {
                _flags[key] = value;
            }
        }
        internal IEnumerable<DICFlag> Keys => _flags.Keys;

        #region DIC Flag Values

        /// <summary>
        /// Manual offset for Audio CD
        /// </summary>
        public int? AddOffsetValue { get; set; }

        /// <summary>
        /// 0xbe opcode value for dumping
        /// Possible values: raw (default), pack
        /// </summary>
        public string BEOpcodeValue { get; set; }

        /// <summary>
        /// C2 reread options for dumping
        /// [0] - Reread value
        /// [1] - 0 reread issue sector (default), 1 reread all
        /// [2] - First LBA to reread (default 0)
        /// [3] - Last LBA to reread (default EOS)
        /// </summary>
        public int?[] C2OpcodeValue { get; set; } = new int?[4];

        /// <summary>
        /// Set the force unit access flag value (default 1)
        /// </summary>
        public int? ForceUnitAccessValue { get; set; }

        /// <summary>
        /// Set the no skip security sector flag value (default 100)
        /// </summary>
        public int? NoSkipSecuritySectorValue { get; set; }

        /// <summary>
        /// Set scan file timeout value (default 60)
        /// </summary>
        public int? ScanFileProtectValue { get; set; }

        /// <summary>
        /// Beginning and ending sectors to skip for physical protection
        /// </summary>
        public int?[] SkipSectorValue { get; set; } = new int?[2];

        /// <summary>
        /// Set the subchanel read level
        /// Possible values: 0 no next sub, 1 next sub (default), 2 next and next next
        /// </summary>
        public int? SubchannelReadLevelValue { get; set; }

        /// <summary>
        /// Set number of empty bytes to insert at the head of first track for VideoNow
        /// </summary>
        public int? VideoNowValue { get; set; }

        #endregion

        /// <summary>
        /// Populate a Parameters object from a param string
        /// </summary>
        /// <param name="parameters">String possibly representing a set of parameters</param>
        public Parameters(string parameters)
        {
            // If any parameters are not valid, wipe out everything
            if (!ValidateAndSetParameters(parameters))
            {
                Command = DICCommand.NONE;

                DriveLetter = null;
                DriveSpeed = null;

                Filename = null;

                StartLBAValue = null;
                EndLBAValue = null;

                _flags = new Dictionary<DICFlag, bool>();

                AddOffsetValue = null;
                BEOpcodeValue = null;
                C2OpcodeValue = new int?[4];
                ForceUnitAccessValue = null;
                NoSkipSecuritySectorValue = null;
                ScanFileProtectValue = null;
                SubchannelReadLevelValue = null;
                VideoNowValue = null;
            }
        }

        /// <summary>
        /// Generate parameters based on a set of known inputs
        /// </summary>
        /// <param name="system">KnownSystem value to use</param>
        /// <param name="type">MediaType value to use</param>
        /// <param name="driveLetter">Drive letter to use</param>
        /// <param name="filename">Filename to use</param>
        /// <param name="driveSpeed">Drive speed to use</param>
        /// <param name="paranoid">Enable paranoid mode (safer dumping)</param>
        /// <param name="rereadCount">User-defined reread count</param>
        public Parameters(KnownSystem? system, MediaType? type, char driveLetter, string filename, int? driveSpeed, bool paranoid, int rereadCount)
        {
            SetBaseCommand(system, type);
            DriveLetter = driveLetter.ToString();
            DriveSpeed = driveSpeed;
            Filename = filename;
            SetDefaultParameters(system, type, paranoid, rereadCount);
        }

        /// <summary>
        /// Determine the base flags to use for checking a commandline
        /// </summary>
        /// <param name="type">Output nullable MediaType containing the found MediaType, if possible</param>
        /// <param name="system">Output nullable KnownSystem containing the found KnownSystem, if possible</param>
        /// <param name="letter">Output string containing the found drive letter</param>
        /// <param name="path">Output string containing the found path</param>
        /// <returns>False on error (and all outputs set to null), true otherwise</returns>
        public bool DetermineFlags(out MediaType? type, out KnownSystem? system, out string letter, out string path)
        {
            // Populate all output variables with null
            type = null; system = null; letter = null; path = null;

            // If we're not already valid, output false
            if (!IsValid())
                return false;

            // Set the default outputs
            type = Converters.ToMediaType(Command);
            system = Converters.ToKnownSystem(Command);
            letter = DriveLetter;
            path = Filename;

            // Determine what the commandline should look like given the first item
            switch (Command)
            {
                case DICCommand.Audio:
                case DICCommand.CompactDisc:
                case DICCommand.Data:
                case DICCommand.DigitalVideoDisc:
                case DICCommand.GDROM:
                case DICCommand.Swap:
                    // GameCube and Wii
                    if (this[DICFlag.Raw])
                    {
                        type = MediaType.NintendoGameCubeGameDisc;
                        system = KnownSystem.NintendoGameCube;
                    }

                    // PlayStation
                    else if (this[DICFlag.NoFixSubQLibCrypt]
                        || this[DICFlag.ScanAntiMod])
                    {
                        type = MediaType.CDROM;
                        system = KnownSystem.SonyPlayStation;
                    }

                    // Saturn
                    else if (this[DICFlag.SeventyFour])
                    {
                        type = MediaType.CDROM;
                        system = KnownSystem.SegaSaturn;
                    }

                    break;
            }

            return true;
        }

        /// <summary>
        /// Blindly generate a parameter string based on the inputs
        /// </summary>
        /// <returns>Correctly formatted parameter string, null on error</returns>
        public string GenerateParameters()
        {
            List<string> parameters = new List<string>();

            if (Command != DICCommand.NONE)
                parameters.Add(Command.LongName());
            else
                return null;

            // Drive Letter
            if (Command == DICCommand.Audio
                || Command == DICCommand.BluRay
                || Command == DICCommand.Close
                || Command == DICCommand.CompactDisc
                || Command == DICCommand.Data
                || Command == DICCommand.DigitalVideoDisc
                || Command == DICCommand.Disk
                || Command == DICCommand.DriveSpeed
                || Command == DICCommand.Eject
                || Command == DICCommand.Floppy
                || Command == DICCommand.GDROM
                || Command == DICCommand.Reset
                || Command == DICCommand.SACD
                || Command == DICCommand.Start
                || Command == DICCommand.Stop
                || Command == DICCommand.Swap
                || Command == DICCommand.XBOX
                || Command == DICCommand.XBOXSwap
                || Command == DICCommand.XGD2Swap
                || Command == DICCommand.XGD3Swap)
            {
                if (DriveLetter != null)
                    parameters.Add(DriveLetter);
                else
                    return null;
            }

            // Filename
            if (Command == DICCommand.Audio
                || Command == DICCommand.BluRay
                || Command == DICCommand.CompactDisc
                || Command == DICCommand.Data
                || Command == DICCommand.DigitalVideoDisc
                || Command == DICCommand.Disk
                || Command == DICCommand.Floppy
                || Command == DICCommand.GDROM
                || Command == DICCommand.MDS
                || Command == DICCommand.Merge
                || Command == DICCommand.SACD
                || Command == DICCommand.Swap
                || Command == DICCommand.Sub
                || Command == DICCommand.XBOX
                || Command == DICCommand.XBOXSwap
                || Command == DICCommand.XGD2Swap
                || Command == DICCommand.XGD3Swap)
            {
                if (Filename != null)
                    parameters.Add("\"" + Filename.Trim('"') + "\"");
                else
                    return null;
            }

            // Optiarc Filename
            if (Command == DICCommand.Merge)
            {
                if (OptiarcFilename != null)
                    parameters.Add("\"" + OptiarcFilename.Trim('"') + "\"");
                else
                    return null;
            }

            // Drive Speed
            if (Command == DICCommand.Audio
                || Command == DICCommand.BluRay
                || Command == DICCommand.CompactDisc
                || Command == DICCommand.Data
                || Command == DICCommand.DigitalVideoDisc
                || Command == DICCommand.GDROM
                || Command == DICCommand.SACD
                || Command == DICCommand.Swap
                || Command == DICCommand.XBOX
                || Command == DICCommand.XBOXSwap
                || Command == DICCommand.XGD2Swap
                || Command == DICCommand.XGD3Swap)
            {
                if (DriveSpeed != null)
                    parameters.Add(DriveSpeed.ToString());
                else
                    return null;
            }

            // LBA Markers
            if (Command == DICCommand.Audio
                || Command == DICCommand.Data)
            {
                if (StartLBAValue != null && StartLBAValue > 0
                    && EndLBAValue != null && EndLBAValue > 0)
                {
                    parameters.Add(StartLBAValue.ToString());
                    parameters.Add(EndLBAValue.ToString());
                }
                else
                    return null;
            }

            // Add Offset
            if (Command == DICCommand.Audio
                || Command == DICCommand.CompactDisc)
            {
                if (this[DICFlag.AddOffset])
                {
                    parameters.Add(DICFlag.AddOffset.LongName());
                    if (AddOffsetValue != null)
                        parameters.Add(AddOffsetValue.ToString());
                    else
                        return null;
                }
            }

            // AMSF Dumping
            if (Command == DICCommand.CompactDisc)
            {
                if (this[DICFlag.AMSF])
                    parameters.Add(DICFlag.AMSF.LongName());
            }

            // Atari Jaguar CD
            if (Command == DICCommand.CompactDisc)
            {
                if (this[DICFlag.AtariJaguar])
                    parameters.Add(DICFlag.AtariJaguar.LongName());
            }

            // BE Opcode
            if (Command == DICCommand.Audio
               || Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.GDROM
               || Command == DICCommand.Swap)
            {
                if (this[DICFlag.BEOpcode] && !this[DICFlag.D8Opcode])
                {
                    parameters.Add(DICFlag.BEOpcode.LongName());
                    if (BEOpcodeValue != null
                        && (BEOpcodeValue == "raw" || BEOpcodeValue == "pack"))
                        parameters.Add(BEOpcodeValue);
                }
            }

            // C2 Opcode
            if (Command == DICCommand.Audio
               || Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.GDROM
               || Command == DICCommand.Swap)
            {
                if (this[DICFlag.C2Opcode])
                {
                    parameters.Add(DICFlag.C2Opcode.LongName());
                    if (C2OpcodeValue[0] != null)
                    {
                        if (C2OpcodeValue[0] > 0)
                            parameters.Add(C2OpcodeValue[0].ToString());
                        else
                            return null;
                    }
                    if (C2OpcodeValue[1] != null)
                    {
                        if (C2OpcodeValue[1] == 0)
                            parameters.Add(C2OpcodeValue[1].ToString());
                        else if (C2OpcodeValue[1] == 1)
                        {
                            parameters.Add(C2OpcodeValue[1].ToString());
                            if (C2OpcodeValue[2] != null && C2OpcodeValue[3] != null)
                            {
                                if (C2OpcodeValue[2] > 0 && C2OpcodeValue[3] > 0)
                                {
                                    parameters.Add(C2OpcodeValue[2].ToString());
                                    parameters.Add(C2OpcodeValue[3].ToString());
                                }
                                else
                                    return null;
                            }
                        }
                        else
                            return null;
                    }
                }
            }

            // Copyright Management Information
            if (Command == DICCommand.DigitalVideoDisc)
            {
                if (this[DICFlag.CopyrightManagementInformation])
                    parameters.Add(DICFlag.CopyrightManagementInformation.LongName());
            }

            // D8 Opcode
            if (Command == DICCommand.Audio
               || Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.GDROM
               || Command == DICCommand.Swap)
            {
                if (this[DICFlag.D8Opcode])
                    parameters.Add(DICFlag.D8Opcode.LongName());
            }

            // Disable Beep
            if (Command == DICCommand.Audio
               || Command == DICCommand.BluRay
               || Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.DigitalVideoDisc
               || Command == DICCommand.GDROM
               || Command == DICCommand.Swap
               || Command == DICCommand.XBOX)
            {
                if (this[DICFlag.DisableBeep])
                    parameters.Add(DICFlag.DisableBeep.LongName());
            }

            // Force Unit Access
            if (Command == DICCommand.BluRay
               || Command == DICCommand.CompactDisc
               || Command == DICCommand.DigitalVideoDisc
               || Command == DICCommand.Swap
               || Command == DICCommand.XBOX)
            {
                if (this[DICFlag.ForceUnitAccess])
                {
                    parameters.Add(DICFlag.ForceUnitAccess.LongName());
                    if (ForceUnitAccessValue != null)
                        parameters.Add(ForceUnitAccessValue.ToString());
                }
            }

            // MCN
            if (Command == DICCommand.CompactDisc)
            {
                if (this[DICFlag.MCN])
                    parameters.Add(DICFlag.MCN.LongName());
            }

            // Multi-Session
            if (Command == DICCommand.CompactDisc)
            {
                if (this[DICFlag.MultiSession])
                    parameters.Add(DICFlag.MultiSession.LongName());
            }

            // Not fix SubP
            if (Command == DICCommand.Audio
               || Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.GDROM
               || Command == DICCommand.Swap)
            {
                if (this[DICFlag.NoFixSubP])
                    parameters.Add(DICFlag.NoFixSubP.LongName());
            }

            // Not fix SubQ
            if (Command == DICCommand.Audio
               || Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.GDROM
               || Command == DICCommand.Swap)
            {
                if (this[DICFlag.NoFixSubQ])
                    parameters.Add(DICFlag.NoFixSubQ.LongName());
            }

            // Not fix SubQ (PlayStation LibCrypt)
            if (Command == DICCommand.Audio
               || Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.GDROM
               || Command == DICCommand.Swap)
            {
                if (this[DICFlag.NoFixSubQLibCrypt])
                    parameters.Add(DICFlag.NoFixSubQLibCrypt.LongName());
            }
            
            // Not fix SubQ (SecuROM)
            if (Command == DICCommand.Audio
               || Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.GDROM
               || Command == DICCommand.Swap)
            {
                if (this[DICFlag.NoFixSubQSecuROM])
                    parameters.Add(DICFlag.NoFixSubQSecuROM.LongName());
            }

            // Not fix SubRtoW
            if (Command == DICCommand.Audio
               || Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.GDROM
               || Command == DICCommand.Swap)
            {
                if (this[DICFlag.NoFixSubRtoW])
                    parameters.Add(DICFlag.NoFixSubRtoW.LongName());
            }

            // Not skip security sectors
            if (Command == DICCommand.XBOX
                || Command == DICCommand.XBOXSwap
                || Command == DICCommand.XGD2Swap
                || Command == DICCommand.XGD3Swap)
            {
                if (this[DICFlag.NoSkipSS])
                {
                    parameters.Add(DICFlag.NoSkipSS.LongName());
                    if (NoSkipSecuritySectorValue != null)
                        parameters.Add(NoSkipSecuritySectorValue.ToString());
                }
            }

            // Raw read (2064 byte/sector)
            if (Command == DICCommand.DigitalVideoDisc)
            {
                if (this[DICFlag.Raw])
                    parameters.Add(DICFlag.Raw.LongName());
            }

            // Reverse read
            if (Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.DigitalVideoDisc)
            {
                if (this[DICFlag.Reverse])
                    parameters.Add(DICFlag.Reverse.LongName());
            }

            // Scan PlayStation anti-mod strings
            if (Command == DICCommand.CompactDisc
               || Command == DICCommand.Data)
            {
                if (this[DICFlag.ScanAntiMod])
                    parameters.Add(DICFlag.ScanAntiMod.LongName());
            }

            // Scan file to detect protect
            if (Command == DICCommand.Audio
               || Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.DigitalVideoDisc
               || Command == DICCommand.Swap)
            {
                if (this[DICFlag.ScanFileProtect])
                {
                    parameters.Add(DICFlag.ScanFileProtect.LongName());
                    if (ScanFileProtectValue != null)
                    {
                        if (ScanFileProtectValue > 0)
                            parameters.Add(ScanFileProtectValue.ToString());
                        else
                            return null;
                    }
                }
            }

            // Scan file to detect protect
            if (Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.Swap)
            {
                if (this[DICFlag.ScanSectorProtect])
                    parameters.Add(DICFlag.ScanSectorProtect.LongName());
            }

            // Scan 74:00:00 (Saturn)
            if (Command == DICCommand.Swap)
            {
                if (this[DICFlag.SeventyFour])
                    parameters.Add(DICFlag.SeventyFour.LongName());
            }

            // Skip sectors
            if (Command == DICCommand.Data)
            {
                if (this[DICFlag.SkipSector])
                {
                    if (SkipSectorValue[0] != null && SkipSectorValue[1] != null)
                    {
                        parameters.Add(DICFlag.SkipSector.LongName());
                        if (SkipSectorValue[0] >= 0 && SkipSectorValue[1] >= 0)
                        {
                            parameters.Add(SkipSectorValue[0].ToString());
                            parameters.Add(SkipSectorValue[1].ToString());
                        }
                        else
                            return null;
                    }
                    else
                        return null;
                }
            }

            // Set Subchannel read level
            if (Command == DICCommand.Audio
               || Command == DICCommand.CompactDisc
               || Command == DICCommand.Data
               || Command == DICCommand.GDROM
               || Command == DICCommand.Swap)
            {
                if (this[DICFlag.SubchannelReadLevel])
                {
                    parameters.Add(DICFlag.SubchannelReadLevel.LongName());
                    if (SubchannelReadLevelValue != null)
                    {
                        if (SubchannelReadLevelValue >= 0 && SubchannelReadLevelValue <= 2)
                            parameters.Add(SubchannelReadLevelValue.ToString());
                        else
                            return null;
                    }
                }
            }

            // VideoNow
            if (Command == DICCommand.CompactDisc)
            {
                if (this[DICFlag.VideoNow])
                {
                    parameters.Add(DICFlag.VideoNow.LongName());
                    if (VideoNowValue != null)
                    {
                        if (VideoNowValue >= 0)
                            parameters.Add(VideoNowValue.ToString());
                        else
                            return null;
                    }
                }
            }

            // VideoNow Color
            if (Command == DICCommand.CompactDisc)
            {
                if (this[DICFlag.VideoNowColor])
                    parameters.Add(DICFlag.VideoNowColor.LongName());
            }

            return string.Join(" ", parameters);
        }

        /// <summary>
        /// Returns if the current Parameter object is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return GenerateParameters() != null;
        }

        /// <summary>
        /// Scan a possible parameter string and populate whatever possible
        /// </summary>
        /// <param name="parameters">String possibly representing parameters</param>
        /// <returns></returns>
        private bool ValidateAndSetParameters(string parameters)
        {
            // The string has to be valid by itself first
            if (string.IsNullOrWhiteSpace(parameters))
                return false;

            // Now split the string into parts for easier validation
            // https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
            parameters = parameters.Trim();
            List<string> parts = Regex.Matches(parameters, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();

            // Determine what the commandline should look like given the first item
            int index = -1;
            switch (parts[0])
            {
                case DICCommandStrings.Audio:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]))
                        return false;
                    else
                        Filename = parts[2];

                    if (!DoesExist(parts, 3) || !IsValidNumber(parts[3], lowerBound: 0, upperBound: 72))
                        return false;
                    else
                        DriveSpeed = Int32.Parse(parts[3]);

                    if (!DoesExist(parts, 4) || !IsValidNumber(parts[4], lowerBound: 0))
                        return false;
                    else
                        StartLBAValue = Int32.Parse(parts[4]);

                    if (!DoesExist(parts, 5) || !IsValidNumber(parts[5], lowerBound: 0))
                        return false;
                    else
                        EndLBAValue = Int32.Parse(parts[5]);

                    Command = DICCommand.Audio;
                    index = 6;
                    break;

                case DICCommandStrings.BluRay:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]))
                        return false;
                    else
                        Filename = parts[2];

                    if (!DoesExist(parts, 3) || !IsValidNumber(parts[3], lowerBound: 0, upperBound: 72))
                        return false;
                    else
                        DriveSpeed = Int32.Parse(parts[3]);

                    Command = DICCommand.BluRay;
                    index = 4;
                    break;

                case DICCommandStrings.Close:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (parts.Count > 2)
                        return false;

                    Command = DICCommand.Close;
                    break;

                case DICCommandStrings.CompactDisc:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]))
                        return false;
                    else
                        Filename = parts[2];

                    if (!DoesExist(parts, 3) || !IsValidNumber(parts[3], lowerBound: 0, upperBound: 72))
                        return false;
                    else
                        DriveSpeed = Int32.Parse(parts[3]);

                    Command = DICCommand.CompactDisc;
                    index = 4;
                    break;

                case DICCommandStrings.Data:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]))
                        return false;
                    else
                        Filename = parts[2];

                    if (!DoesExist(parts, 3) || !IsValidNumber(parts[3], lowerBound: 0, upperBound: 72))
                        return false;
                    else
                        DriveSpeed = Int32.Parse(parts[3]);

                    if (!DoesExist(parts, 4) || !IsValidNumber(parts[4], lowerBound: 0))
                        return false;
                    else
                        StartLBAValue = Int32.Parse(parts[4]);

                    if (!DoesExist(parts, 5) || !IsValidNumber(parts[5], lowerBound: 0))
                        return false;
                    else
                        EndLBAValue = Int32.Parse(parts[5]);

                    Command = DICCommand.Data;
                    index = 6;
                    break;

                case DICCommandStrings.DigitalVideoDisc:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]))
                        return false;
                    else
                        Filename = parts[2];

                    if (!DoesExist(parts, 3) || !IsValidNumber(parts[3], lowerBound: 0, upperBound: 24)) // Officially 0-16
                        return false;
                    else
                        DriveSpeed = Int32.Parse(parts[3]);

                    Command = DICCommand.DigitalVideoDisc;
                    index = 4;
                    break;

                case DICCommandStrings.Disk:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]))
                        return false;
                    else
                        Filename = parts[2];

                    if (parts.Count > 3)
                        return false;

                    Command = DICCommand.Disk;
                    break;

                case DICCommandStrings.DriveSpeed:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (parts.Count > 2)
                        return false;

                    Command = DICCommand.DriveSpeed;
                    break;

                case DICCommandStrings.Eject:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (parts.Count > 2)
                        return false;

                    Command = DICCommand.Eject;
                    break;

                case DICCommandStrings.Floppy:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]))
                        return false;
                    else
                        Filename = parts[2];

                    if (parts.Count > 3)
                        return false;

                    Command = DICCommand.Floppy;
                    break;

                case DICCommandStrings.GDROM:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]))
                        return false;
                    else
                        Filename = parts[2];

                    if (!DoesExist(parts, 3) || !IsValidNumber(parts[3], lowerBound: 0, upperBound: 72))
                        return false;
                    else
                        DriveSpeed = Int32.Parse(parts[3]);

                    Command = DICCommand.GDROM;
                    index = 4;
                    break;

                case DICCommandStrings.MDS:
                    if (!DoesExist(parts, 1) || IsFlag(parts[1]) || !File.Exists(parts[1]))
                        return false;
                    else
                        Filename = parts[1];

                    if (parts.Count > 2)
                        return false;

                    Command = DICCommand.MDS;
                    break;

                case DICCommandStrings.Merge:
                    if (!DoesExist(parts, 1) || IsFlag(parts[1]) || !File.Exists(parts[1]))
                        return false;
                    else
                        Filename = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]) || !File.Exists(parts[2]))
                        return false;
                    else
                        OptiarcFilename = parts[2];

                    if (parts.Count > 3)
                        return false;

                    Command = DICCommand.Merge;
                    break;

                case DICCommandStrings.Reset:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (parts.Count > 2)
                        return false;

                    Command = DICCommand.Reset;
                    break;

                case DICCommandStrings.SACD:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]))
                        return false;
                    else
                        Filename = parts[2];

                    if (!DoesExist(parts, 3) || !IsValidNumber(parts[3], lowerBound: 0, upperBound: 16))
                        return false;
                    else
                        DriveSpeed = Int32.Parse(parts[3]);

                    if (parts.Count > 4)
                        return false;

                    Command = DICCommand.SACD;
                    break;

                case DICCommandStrings.Start:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (parts.Count > 2)
                        return false;

                    Command = DICCommand.Start;
                    break;

                case DICCommandStrings.Stop:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (parts.Count > 2)
                        return false;

                    Command = DICCommand.Stop;
                    break;

                case DICCommandStrings.Sub:
                    if (!DoesExist(parts, 1) || IsFlag(parts[1]) || !File.Exists(parts[1]))
                        return false;
                    else
                        Filename = parts[1];

                    if (parts.Count > 2)
                        return false;

                    Command = DICCommand.Sub;
                    break;

                case DICCommandStrings.Swap:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]))
                        return false;
                    else
                        Filename = parts[2];

                    if (!DoesExist(parts, 3) || !IsValidNumber(parts[3], lowerBound: 0, upperBound: 72))
                        return false;
                    else
                        DriveSpeed = Int32.Parse(parts[3]);

                    Command = DICCommand.Swap;
                    index = 4;
                    break;

                case DICCommandStrings.XBOX:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]))
                        return false;
                    else
                        Filename = parts[2];

                    if (!DoesExist(parts, 3) || !IsValidNumber(parts[3], lowerBound: 0, upperBound: 72))
                        return false;
                    else
                        DriveSpeed = Int32.Parse(parts[3]);

                    Command = DICCommand.XBOX;
                    index = 4;
                    break;

                case DICCommandStrings.XBOXSwap:
                case DICCommandStrings.XGD2Swap:
                case DICCommandStrings.XGD3Swap:
                    if (!DoesExist(parts, 1) || !IsValidDriveLetter(parts[1]))
                        return false;
                    else
                        DriveLetter = parts[1];

                    if (!DoesExist(parts, 2) || IsFlag(parts[2]))
                        return false;
                    else
                        Filename = parts[2];

                    if (!DoesExist(parts, 3) || !IsValidNumber(parts[3], lowerBound: 0, upperBound: 72))
                        return false;
                    else
                        DriveSpeed = Int32.Parse(parts[3]);

                    for (int i = 4; i < parts.Count; i++)
                    {
                        if (!Int64.TryParse(parts[i], out long temp))
                            return false;
                    }

                    break;
                default:
                    return false;
            }

            // Loop through all auxilary flags, if necessary
            if (index > 0)
            {
                for (int i = index; i < parts.Count; i++)
                {
                    switch (parts[i])
                    {
                        case DICFlagStrings.AddOffset:
                            if (parts[0] != DICCommandStrings.Audio
                                && parts[0] != DICCommandStrings.CompactDisc)
                                return false;
                            else if (!DoesExist(parts, i + 1))
                                return false;
                            else if (!IsValidNumber(parts[i + 1]))
                                return false;

                            this[DICFlag.AddOffset] = true;
                            AddOffsetValue = Int32.Parse(parts[i + 1]);
                            i++;
                            break;

                        case DICFlagStrings.AMSF:
                            if (parts[0] != DICCommandStrings.CompactDisc)
                                return false;

                            this[DICFlag.AMSF] = true;
                            break;

                        case DICFlagStrings.AtariJaguar:
                            if (parts[0] != DICCommandStrings.CompactDisc)
                                return false;

                            this[DICFlag.AtariJaguar] = true;
                            break;

                        case DICFlagStrings.BEOpcode:
                            if (parts[0] != DICCommandStrings.Audio
                                && parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.Data
                                && parts[0] != DICCommandStrings.GDROM)
                                return false;
                            else if (!DoesExist(parts, i + 1))
                            {
                                this[DICFlag.BEOpcode] = true;
                                break;
                            }
                            else if (IsFlag(parts[i + 1]))
                            {
                                this[DICFlag.BEOpcode] = true;
                                break;
                            }
                            else if (parts[i + 1] != "raw" && (parts[i + 1] != "pack"))
                                return false;

                            this[DICFlag.BEOpcode] = true;
                            BEOpcodeValue = parts[i + 1];
                            i++;
                            break;

                        case DICFlagStrings.C2Opcode:
                            if (parts[0] != DICCommandStrings.Audio
                                && parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.Data
                                && parts[0] != DICCommandStrings.GDROM)
                                return false;

                            this[DICFlag.C2Opcode] = true;
                            for (int j = 0; j < 4; j++)
                            {
                                if (!DoesExist(parts, i + 1))
                                    break;
                                else if (IsFlag(parts[i + 1]))
                                    break;
                                else if (!IsValidNumber(parts[i + 1], lowerBound: 0))
                                    return false;
                                else
                                {
                                    C2OpcodeValue[j] = Int32.Parse(parts[i + 1]);
                                    i++;
                                }
                            }

                            break;

                        case DICFlagStrings.CopyrightManagementInformation:
                            if (parts[0] != DICCommandStrings.DigitalVideoDisc)
                                return false;

                            this[DICFlag.CopyrightManagementInformation] = true;
                            break;

                        case DICFlagStrings.D8Opcode:
                            if (parts[0] != DICCommandStrings.Audio
                                && parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.Data
                                && parts[0] != DICCommandStrings.GDROM)
                                return false;

                            this[DICFlag.D8Opcode] = true;
                            break;

                        case DICFlagStrings.DisableBeep:
                            if (parts[0] != DICCommandStrings.Audio
                                && parts[0] != DICCommandStrings.BluRay
                                && parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.Data
                                && parts[0] != DICCommandStrings.DigitalVideoDisc
                                && parts[0] != DICCommandStrings.GDROM
                                && parts[0] != DICCommandStrings.XBOX)
                                return false;

                            this[DICFlag.DisableBeep] = true;
                            break;

                        case DICFlagStrings.ForceUnitAccess:
                            if (parts[0] != DICCommandStrings.Audio
                                && parts[0] != DICCommandStrings.BluRay
                                && parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.DigitalVideoDisc
                                && parts[0] != DICCommandStrings.Data
                                && parts[0] != DICCommandStrings.GDROM
                                && parts[0] != DICCommandStrings.XBOX)
                                return false;
                            else if (!DoesExist(parts, i + 1))
                            {
                                this[DICFlag.ForceUnitAccess] = true;
                                break;
                            }
                            else if (IsFlag(parts[i + 1]))
                            {
                                this[DICFlag.ForceUnitAccess] = true;
                                break;
                            }
                            else if (!IsValidNumber(parts[i + 1], lowerBound: 0))
                                return false;

                            this[DICFlag.ForceUnitAccess] = true;
                            ForceUnitAccessValue = Int32.Parse(parts[i + 1]);
                            i++;
                            break;

                        case DICFlagStrings.MCN:
                            if (parts[0] != DICCommandStrings.CompactDisc)
                                return false;

                            this[DICFlag.MCN] = true;
                            break;

                        case DICFlagStrings.MultiSession:
                            if (parts[0] != DICCommandStrings.CompactDisc)
                                return false;

                            this[DICFlag.MultiSession] = true;
                            break;

                        case DICFlagStrings.NoFixSubP:
                            if (parts[0] != DICCommandStrings.Audio
                                && parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.Data
                                && parts[0] != DICCommandStrings.GDROM)
                                return false;

                            this[DICFlag.NoFixSubP] = true;
                            break;

                        case DICFlagStrings.NoFixSubQ:
                            if (parts[0] != DICCommandStrings.Audio
                                && parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.Data
                                && parts[0] != DICCommandStrings.GDROM)
                                return false;

                            this[DICFlag.NoFixSubQ] = true;
                            break;

                        case DICFlagStrings.NoFixSubQLibCrypt:
                            if (parts[0] != DICCommandStrings.CompactDisc)
                                return false;

                            this[DICFlag.NoFixSubQLibCrypt] = true;
                            break;

                        case DICFlagStrings.NoFixSubRtoW:
                            if (parts[0] != DICCommandStrings.Audio
                                && parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.Data
                                && parts[0] != DICCommandStrings.GDROM)
                                return false;

                            this[DICFlag.NoFixSubRtoW] = true;
                            break;

                        case DICFlagStrings.NoFixSubQSecuROM:
                            if (parts[0] != DICCommandStrings.Audio
                                && parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.Data
                                && parts[0] != DICCommandStrings.GDROM)
                                return false;

                            this[DICFlag.NoFixSubQSecuROM] = true;
                            break;

                        case DICFlagStrings.NoSkipSS:
                            if (parts[0] != DICCommandStrings.XBOX
                                && parts[0] != DICCommandStrings.XBOXSwap
                                && parts[0] != DICCommandStrings.XGD2Swap
                                && parts[0] != DICCommandStrings.XGD3Swap)
                                return false;
                            else if (!DoesExist(parts, i + 1))
                            {
                                this[DICFlag.NoSkipSS] = true;
                                break;
                            }
                            else if (IsFlag(parts[i + 1]))
                            {
                                this[DICFlag.NoSkipSS] = true;
                                break;
                            }
                            else if (!IsValidNumber(parts[i + 1], lowerBound: 0))
                                return false;

                            this[DICFlag.NoSkipSS] = true;
                            ForceUnitAccessValue = Int32.Parse(parts[i + 1]);
                            i++;
                            break;

                        case DICFlagStrings.Raw:
                            if (parts[0] != DICCommandStrings.DigitalVideoDisc)
                                return false;

                            this[DICFlag.Raw] = true;
                            break;

                        case DICFlagStrings.Reverse:
                            if (parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.Data
                                && parts[0] != DICCommandStrings.DigitalVideoDisc)
                                return false;

                            this[DICFlag.Reverse] = true;
                            break;

                        case DICFlagStrings.ScanAntiMod:
                            if (parts[0] != DICCommandStrings.CompactDisc)
                                return false;

                            this[DICFlag.ScanAntiMod] = true;
                            break;

                        case DICFlagStrings.ScanFileProtect:
                            if (parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.Data
                                && parts[0] != DICCommandStrings.DigitalVideoDisc)
                                return false;
                            else if (!DoesExist(parts, i + 1))
                            {
                                this[DICFlag.ScanFileProtect] = true;
                                break;
                            }
                            else if (IsFlag(parts[i + 1]))
                            {
                                this[DICFlag.ScanFileProtect] = true;
                                break;
                            }
                            else if (!IsValidNumber(parts[i + 1], lowerBound: 0))
                                return false;

                            this[DICFlag.ScanFileProtect] = true;
                            ScanFileProtectValue = Int32.Parse(parts[i + 1]);
                            i++;
                            break;

                        case DICFlagStrings.ScanSectorProtect:
                            if (parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.Data)
                                return false;

                            this[DICFlag.ScanSectorProtect] = true;
                            break;

                        case DICFlagStrings.SeventyFour:
                            if (parts[0] != DICCommandStrings.Swap)
                                return false;

                            this[DICFlag.SeventyFour] = true;
                            break;

                        case DICFlagStrings.SkipSector:
                            if (parts[0] != DICCommandStrings.Data)
                                return false;
                            else if (!DoesExist(parts, i + 1) || !DoesExist(parts, i + 2))
                                return false;
                            else if (IsFlag(parts[i + 1]) || IsFlag(parts[i + 2]))
                                return false;
                            else if (!IsValidNumber(parts[i + 1], lowerBound: 0) || !IsValidNumber(parts[i + 2], lowerBound: 0))
                                return false;

                            this[DICFlag.SkipSector] = true;
                            SkipSectorValue[0] = Int32.Parse(parts[i + 1]);
                            SkipSectorValue[1] = Int32.Parse(parts[i + 2]);
                            i += 2;
                            break;

                        case DICFlagStrings.SubchannelReadLevel:
                            if (parts[0] != DICCommandStrings.Audio
                                && parts[0] != DICCommandStrings.CompactDisc
                                && parts[0] != DICCommandStrings.Data
                                && parts[0] != DICCommandStrings.GDROM)
                                return false;
                            else if (!DoesExist(parts, i + 1))
                            {
                                this[DICFlag.SubchannelReadLevel] = true;
                                break;
                            }
                            else if (IsFlag(parts[i + 1]))
                            {
                                this[DICFlag.SubchannelReadLevel] = true;
                                break;
                            }
                            else if (!IsValidNumber(parts[i + 1], lowerBound: 0, upperBound: 2))
                                return false;

                            this[DICFlag.SubchannelReadLevel] = true;
                            SubchannelReadLevelValue = Int32.Parse(parts[i + 1]);
                            i++;
                            break;

                        case DICFlagStrings.VideoNow:
                            if (parts[0] != DICCommandStrings.CompactDisc)
                                return false;
                            else if (!DoesExist(parts, i + 1))
                            {
                                this[DICFlag.VideoNow] = true;
                                break;
                            }
                            else if (IsFlag(parts[i + 1]))
                            {
                                this[DICFlag.VideoNow] = true;
                                break;
                            }
                            else if (!IsValidNumber(parts[i + 1], lowerBound: 0))
                                return false;

                            this[DICFlag.VideoNow] = true;
                            VideoNowValue = Int32.Parse(parts[i + 1]);
                            i++;
                            break;

                        case DICFlagStrings.VideoNowColor:
                            if (parts[0] != DICCommandStrings.CompactDisc)
                                return false;

                            this[DICFlag.VideoNowColor] = true;
                            break;

                        default:
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns whether a string is a valid drive letter
        /// </summary>
        /// <param name="parameter">String value to check</param>
        /// <returns>True if it's a valid drive letter, false otherwise</returns>
        private bool IsValidDriveLetter(string parameter)
        {
            if (!Regex.IsMatch(parameter, @"^[A-Z]:?\\?$"))
                return false;

            return true;
        }

        /// <summary>
        /// Returns whether a string is a flag (starts with '/')
        /// </summary>
        /// <param name="parameter">String value to check</param>
        /// <returns>True if it's a flag, false otherwise</returns>
        private bool IsFlag(string parameter)
        {
            if (parameter.Trim('\"').StartsWith("/"))
                return true;

            return false;
        }

        /// <summary>
        /// Returns whether or not the selected item exists
        /// </summary>
        /// <param name="parameters">List of parameters to check against</param>
        /// <param name="index">Current index</param>
        /// <returns>True if the next item exists, false otherwise</returns>
        private bool DoesExist(List<string> parameters, int index)
        {
            if (index >= parameters.Count)
                return false;

            return true;
        }

        /// <summary>
        /// Returns whether a string is a valid number
        /// </summary>
        /// <param name="parameter">String value to check</param>
        /// <param name="lowerBound">Lower bound (>=)</param>
        /// <param name="upperBound">Upper bound (<=)</param>
        /// <returns>True if it's a valid number, false otherwise</returns>
        private bool IsValidNumber(string parameter, int lowerBound = -1, int upperBound = -1)
        {
            if (!Int32.TryParse(parameter, out int temp))
                return false;
            else if (lowerBound != -1 && temp < lowerBound)
                return false;
            else if (upperBound != -1 && temp > upperBound)
                return false;

            return true;
        }

        /// <summary>
        /// Set the DIC command to be used for a given system and media type
        /// </summary>
        /// <param name="system">KnownSystem value to check</param>
        /// <param name="type">MediaType value to check</param>
        private void SetBaseCommand(KnownSystem? system, MediaType? type)
        {
            // If we have an invalid combination, we should Command = null
            if (!Validators.GetValidMediaTypes(system).Contains(type))
            {
                Command = DICCommand.NONE;
                return;
            }

            switch (type)
            {
                case MediaType.CDROM:
                    if (system == KnownSystem.SuperAudioCD)
                        Command = DICCommand.SACD;
                    else
                        Command = DICCommand.CompactDisc;
                    return;
                case MediaType.DVD:
                    if (system == KnownSystem.MicrosoftXBOX
                        || system == KnownSystem.MicrosoftXBOX360)
                    {
                        Command = DICCommand.XBOX;
                        return;
                    }
                    Command = DICCommand.DigitalVideoDisc;
                    return;
                case MediaType.GDROM:
                    Command = DICCommand.GDROM;
                    return;
                case MediaType.HDDVD:
                    Command = DICCommand.DigitalVideoDisc;
                    return;
                case MediaType.BluRay:
                    Command = DICCommand.BluRay;
                    return;
                case MediaType.NintendoGameCubeGameDisc:
                    Command = DICCommand.DigitalVideoDisc;
                    return;
                case MediaType.NintendoWiiOpticalDisc:
                    Command = DICCommand.DigitalVideoDisc;
                    return;
                case MediaType.FloppyDisk:
                    Command = DICCommand.Floppy;
                    return;
                case MediaType.HardDisk:
                    Command = DICCommand.Disk;
                    return;

                default:
                    Command = DICCommand.NONE;
                    return;
            }
        }

        /// <summary>
        /// Set default parameters for a given system and media type
        /// </summary>
        /// <param name="system">KnownSystem value to check</param>
        /// <param name="type">MediaType value to check</param>
        /// <param name="paranoid">Enable paranoid mode (safer dumping)</param>
        /// <param name="rereadCount">User-defined reread count</param>
        private void SetDefaultParameters(KnownSystem? system, MediaType? type, bool paranoid, int rereadCount)
        {
            // First check to see if the combination of system and MediaType is valid
            var validTypes = Validators.GetValidMediaTypes(system);
            if (!validTypes.Contains(type))
                return;

            // Set the C2 reread count
            switch (rereadCount)
            {
                case -1:
                    C2OpcodeValue[0] = null;
                    break;
                case 0:
                    C2OpcodeValue[0] = 20;
                    break;
                default:
                    C2OpcodeValue[0] = rereadCount;
                    break;
            }

            // Now sort based on disc type
            switch (type)
            {
                case MediaType.CDROM:
                    this[DICFlag.C2Opcode] = true;

                    switch (system)
                    {
                        case KnownSystem.AppleMacintosh:
                        case KnownSystem.IBMPCCompatible:
                            this[DICFlag.NoFixSubQSecuROM] = true;
                            this[DICFlag.ScanFileProtect] = true;

                            if (paranoid)
                            {
                                this[DICFlag.ScanSectorProtect] = true;
                                this[DICFlag.SubchannelReadLevel] = true;
                                SubchannelReadLevelValue = 2;
                            }
                            break;
                        case KnownSystem.AtariJaguarCD:
                            this[DICFlag.AtariJaguar] = true;
                            break;
                        case KnownSystem.HasbroVideoNow:
                        case KnownSystem.HasbroVideoNowJr:
                            this[DICFlag.VideoNow] = true;
                            this.VideoNowValue = 18032;
                            break;
                        case KnownSystem.HasbroVideoNowColor:
                            this[DICFlag.VideoNowColor] = true;
                            break;
                        case KnownSystem.HasbroVideoNowXP:
                            this[DICFlag.VideoNow] = true;
                            this.VideoNowValue = 20832;
                            break;
                        case KnownSystem.NECPCEngineTurboGrafxCD:
                            this[DICFlag.MCN] = true;
                            break;
                        case KnownSystem.SonyPlayStation:
                            this[DICFlag.ScanAntiMod] = true;
                            this[DICFlag.NoFixSubQLibCrypt] = true;
                            break;
                    }
                    break;
                case MediaType.DVD:
                    if (paranoid)
                    {
                        this[DICFlag.CopyrightManagementInformation] = true;
                        this[DICFlag.ScanFileProtect] = true;
                    }
                    break;
                case MediaType.GDROM:
                    this[DICFlag.C2Opcode] = true;
                    break;
                case MediaType.HDDVD:
                    if (paranoid)
                        this[DICFlag.CopyrightManagementInformation] = true;
                    break;
                case MediaType.BluRay:
                    // Currently no defaults set
                    break;

                // Special Formats
                case MediaType.NintendoGameCubeGameDisc:
                    this[DICFlag.Raw] = true;
                    break;
                case MediaType.NintendoWiiOpticalDisc:
                    this[DICFlag.Raw] = true;
                    break;

                // Non-optical
                case MediaType.FloppyDisk:
                    // Currently no defaults set
                    break;
            }
        }
    }
}
