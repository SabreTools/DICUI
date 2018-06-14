﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace DICUI.Utilities
{
    public static class Validation
    {
        /// <summary>
        /// Get a list of valid DiscTypes for a given system
        /// </summary>
        /// <param name="sys">KnownSystem value to check</param>
        /// <returns>List of DiscTypes</returns>
        public static List<DiscType?> GetValidDiscTypes(KnownSystem? sys)
        {
            List<DiscType?> types = new List<DiscType?>();
            
            switch (sys)
            {
                #region Consoles

                case KnownSystem.BandaiPlaydiaQuickInteractiveSystem:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.BandaiApplePippin:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.CommodoreAmigaCD32:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.CommodoreAmigaCDTV:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.MattelHyperscan:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.MicrosoftXBOX:
                    types.Add(DiscType.CD);
                    types.Add(DiscType.DVD5);
                    break;
                case KnownSystem.MicrosoftXBOX360:
                    types.Add(DiscType.CD);
                    types.Add(DiscType.DVD9);
                    types.Add(DiscType.HDDVD);
                    break;
                case KnownSystem.MicrosoftXBOXOne:
                    types.Add(DiscType.BD25);
                    types.Add(DiscType.BD50);
                    break;
                case KnownSystem.NECPCEngineTurboGrafxCD:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.NECPCFX:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.NintendoGameCube:
                    types.Add(DiscType.GameCubeGameDisc);
                    break;
                case KnownSystem.NintendoWii:
                    types.Add(DiscType.WiiOpticalDisc);
                    break;
                case KnownSystem.NintendoWiiU:
                    types.Add(DiscType.WiiUOpticalDisc);
                    break;
                case KnownSystem.Panasonic3DOInteractiveMultiplayer:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.PhilipsCDi:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.SegaCDMegaCD:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.SegaDreamcast:
                    types.Add(DiscType.GDROM);
                    break;
                case KnownSystem.SegaSaturn:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.SNKNeoGeoCD:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.SonyPlayStation:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.SonyPlayStation2:
                    types.Add(DiscType.CD);
                    types.Add(DiscType.DVD5);
                    types.Add(DiscType.DVD9);
                    break;
                case KnownSystem.SonyPlayStation3:
                    types.Add(DiscType.BD25);
                    types.Add(DiscType.BD50);
                    break;
                case KnownSystem.SonyPlayStation4:
                    types.Add(DiscType.BD25);
                    types.Add(DiscType.BD50);
                    break;
                case KnownSystem.SonyPlayStationPortable:
                    types.Add(DiscType.UMD);
                    break;
                case KnownSystem.VMLabsNuon:
                    types.Add(DiscType.DVD5);
                    types.Add(DiscType.DVD9);
                    break;
                case KnownSystem.VTechVFlashVSmilePro:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.ZAPiTGamesGameWaveFamilyEntertainmentSystem:
                    types.Add(DiscType.DVD5);
                    break;

                #endregion

                #region Computers

                case KnownSystem.AcornArchimedes:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.AppleMacintosh:
                    types.Add(DiscType.CD);
                    types.Add(DiscType.DVD5);
                    types.Add(DiscType.DVD9);
                    types.Add(DiscType.Floppy);
                    break;
                case KnownSystem.CommodoreAmigaCD:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.FujitsuFMTowns:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.IBMPCCompatible:
                    types.Add(DiscType.CD);
                    types.Add(DiscType.DVD5);
                    types.Add(DiscType.DVD9);
                    types.Add(DiscType.Floppy);
                    break;
                case KnownSystem.NECPC88:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.NECPC98:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.SharpX68000:
                    types.Add(DiscType.CD);
                    break;

                #endregion

                #region Arcade

                case KnownSystem.NamcoSegaNintendoTriforce:
                    types.Add(DiscType.GDROM);
                    break;
                case KnownSystem.SegaChihiro:
                    types.Add(DiscType.GDROM);
                    break;
                case KnownSystem.SegaLindbergh:
                    types.Add(DiscType.DVD5);
                    types.Add(DiscType.DVD9);
                    break;
                case KnownSystem.SegaNaomi:
                    types.Add(DiscType.GDROM);
                    break;
                case KnownSystem.SegaNaomi2:
                    types.Add(DiscType.GDROM);
                    break;
                case KnownSystem.SegaNu:
                    types.Add(DiscType.BD25);
                    types.Add(DiscType.BD50);
                    break;
                case KnownSystem.SegaRingEdge2:
                    types.Add(DiscType.DVD5);
                    types.Add(DiscType.DVD9);
                    break;
                case KnownSystem.TABAustriaQuizard:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.TandyMemorexVisualInformationSystem:
                    types.Add(DiscType.CD);
                    break;

                #endregion

                #region Others

                case KnownSystem.AudioCD:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.BDVideo:
                    types.Add(DiscType.BD25);
                    types.Add(DiscType.BD50);
                    break;
                case KnownSystem.DVDVideo:
                    types.Add(DiscType.DVD5);
                    types.Add(DiscType.DVD9);
                    break;
                case KnownSystem.EnhancedCD:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.PalmOS:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.PhilipsCDiDigitalVideo:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.PhotoCD:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.PlayStationGameSharkUpdates:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.TaoiKTV:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.TomyKissSite:
                    types.Add(DiscType.CD);
                    break;
                case KnownSystem.VideoCD:
                    types.Add(DiscType.CD);
                    break;

                #endregion

                case KnownSystem.NONE:
                default:
                    types.Add(DiscType.NONE);
                    break;
            }

            return types;
        }

        /// <summary>
        /// Create a list of systems matched to their respective enums
        /// </summary>
        /// <returns>Systems matched to enums, if possible</returns>
        /// <remarks>
        /// This returns a List of Tuples whose structure is as follows:
        ///		Item 1: Printable name
        ///		Item 2: KnownSystem mapping
        ///		Item 3: DiscType mapping
        ///	If something has a "string, null, null" value, it should be assumed that it is a separator
        /// </remarks>
        public static List<Tuple<string, KnownSystem?, DiscType?>> CreateListOfSystems()
        {
            List<Tuple<string, KnownSystem?, DiscType?>> mapping = new List<Tuple<string, KnownSystem?, DiscType?>>();

            foreach (KnownSystem system in Enum.GetValues(typeof(KnownSystem)))
            {
                // In the special cases of breaks, we want to add the proper mappings for sections
                switch (system)
                {
                    // Consoles section
                    case KnownSystem.BandaiPlaydiaQuickInteractiveSystem:
                        mapping.Add(new Tuple<string, KnownSystem?, DiscType?>("---------- Consoles ----------", null, null));
                        break;

                    // Computers section
                    case KnownSystem.AcornArchimedes:
                        mapping.Add(new Tuple<string, KnownSystem?, DiscType?>("---------- Computers ----------", null, null));
                        break;

                    // Arcade section
                    case KnownSystem.NamcoSegaNintendoTriforce:
                        mapping.Add(new Tuple<string, KnownSystem?, DiscType?>("---------- Arcade ----------", null, null));
                        break;

                    // Other section
                    case KnownSystem.AudioCD:
                        mapping.Add(new Tuple<string, KnownSystem?, DiscType?>("---------- Others ----------", null, null));
                        break;
                }

                // First, get a list of all DiscTypes for a given KnownSystem
                List<DiscType?> types = GetValidDiscTypes(system);

                // If we have a single type, we don't want to postfix the system name with it
                if (types.Count == 1)
                {
                    mapping.Add(new Tuple<string, KnownSystem?, DiscType?>(Converters.KnownSystemToString(system), system, types[0]));
                }
                // Otherwise, postfix the system name properly
                else
                {
                    foreach (DiscType type in types)
                    {
                        mapping.Add(new Tuple<string, KnownSystem?, DiscType?>(Converters.KnownSystemToString(system) + " (" + Converters.DiscTypeToString(type) + ")", system, type));
                    }
                }
            }

            // Add final mapping for "Custom"
            mapping.Add(new Tuple<string, KnownSystem?, DiscType?>("Custom Input", KnownSystem.NONE, DiscType.NONE));

            return mapping;
        }

        /// <summary>
        /// Create a list of active optical drives matched to their volume labels
        /// </summary>
        /// <returns>Active drives, matched to labels, if possible</returns>
        /// <remarks>
        /// https://stackoverflow.com/questions/3060796/how-to-distinguish-between-usb-and-floppy-devices?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
        /// https://msdn.microsoft.com/en-us/library/aa394173(v=vs.85).aspx
        /// This returns a List of Tuples whose structure is as follows:
        ///		Item 1: Drive letter
        ///		Item 2: Volume label
        ///		Item 3: (True for floppy drive, false otherwise)
        /// </remarks>
        public static List<Tuple<char, string, bool>> CreateListOfDrives()
        {
            // Get the floppy drives
            List<Tuple<char, string, bool>> floppyDrives = new List<Tuple<char, string, bool>>();
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_LogicalDisk");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    uint? mediaType = (uint?)queryObj["MediaType"];
                    if (mediaType != null && ((mediaType > 0 && mediaType < 11) || (mediaType > 12 && mediaType < 22)))
                    {
                        char devId = queryObj["DeviceID"].ToString()[0];
                        floppyDrives.Add(new Tuple<char, string, bool>(devId, "FLOPPY", true));
                    }
                }
            }
            catch
            {
                // No-op
            }

            // Get the optical disc drives
            List<Tuple<char, string, bool>> discDrives = DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.CDRom && d.IsReady)
                .Select(d => new Tuple<char, string, bool>(d.Name[0], d.VolumeLabel, false))
                .ToList();

            // Add the two lists together, order, and return
            floppyDrives.AddRange(discDrives);
            return floppyDrives.OrderBy(i => i.Item1).ToList();
        }

        /// <summary>
        /// Validate that at string would be valid as input to DiscImageCreator
        /// </summary>
        /// <param name="parameters">String representing all parameters</param>
        /// <returns>True if it would be valid, false otherwise</returns>
        public static bool ValidateParameters(string parameters)
        {
            // The string has to be valid by itself first
            if (String.IsNullOrWhiteSpace(parameters))
            {
                return false;
            }

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
                case DICCommands.CompactDisc:
                case DICCommands.GDROM:
                case DICCommands.Swap:
                case DICCommands.Data:
                    if (!IsValidDriveLetter(parts[1]))
                    {
                        return false;
                    }
                    else if (IsFlag(parts[2]))
                    {
                        return false;
                    }
                    else if (!IsValidNumber(parts[3], lowerBound: 0, upperBound: 72))
                    {
                        return false;
                    }

                    if (parts[0] == DICCommands.Swap)
                    {
                        if (parts.Count > 5)
                        {
                            return false;
                        }
                    }
                    else if (parts[0] == DICCommands.Data || parts[0] == DICCommands.Audio)
                    {
                        if (!IsValidNumber(parts[4]) || !IsValidNumber(parts[5]))
                        {
                            return false;
                        }

                        index = 6;
                    }
                    else
                    {
                        index = 4;
                    }

                    break;
                case DICCommands.DigitalVideoDisc:
                    if (!IsValidDriveLetter(parts[1]))
                    {
                        return false;
                    }
                    else if (IsFlag(parts[2]))
                    {
                        return false;
                    }
                    else if (!IsValidNumber(parts[3], lowerBound: 0, upperBound: 72)) // Officially 0-16
                    {
                        return false;
                    }

                    // Loop through all auxilary flags
                    for (int i = 4; i < parts.Count; i++)
                    {
                        switch (parts[i])
                        {
                            case DICFlags.DisableBeep:
                            case DICFlags.CMI:
                            case DICFlags.Raw:
                                // No-op, all of these are single flags
                                break;
                            case DICFlags.ForceUnitAccess:
                                // If the next item doesn't exist, it's good
                                if (!DoesNextExist(parts, i))
                                {
                                    break;
                                }
                                // If the next item is a flag, it's good
                                if (IsFlag(parts[i + 1]))
                                {
                                    break;
                                }
                                // If the next item isn't a valid number
                                else if (!IsValidNumber(parts[i + 1], lowerBound: 0))
                                {
                                    return false;
                                }
                                i++;
                                break;
                            default:
                                return false;
                        }
                    }
                    break;
                case DICCommands.BluRay:
                case DICCommands.XBOX:
                    if (!IsValidDriveLetter(parts[1]))
                    {
                        return false;
                    }
                    else if (IsFlag(parts[2]))
                    {
                        return false;
                    }

                    // Loop through all auxilary flags
                    for (int i = 3; i < parts.Count; i++)
                    {
                        switch (parts[i])
                        {
                            case DICFlags.DisableBeep:
                                // No-op, this is a single flag
                                break;
                            case DICFlags.ForceUnitAccess:
                                // If the next item doesn't exist, it's good
                                if (!DoesNextExist(parts, i))
                                {
                                    break;
                                }
                                // If the next item is a flag, it's good
                                if (IsFlag(parts[i + 1]))
                                {
                                    break;
                                }
                                // If the next item isn't a valid number
                                else if (!IsValidNumber(parts[i + 1], lowerBound: 0))
                                {
                                    return false;
                                }
                                i++;
                                break;
                            default:
                                return false;
                        }
                    }
                    break;
                case DICCommands.Floppy:
                    if (!IsValidDriveLetter(parts[1]))
                    {
                        return false;
                    }
                    else if (IsFlag(parts[2]))
                    {
                        return false;
                    }
                    else if (parts.Count > 3)
                    {
                        return false;
                    }
                    break;
                case DICCommands.Stop:
                case DICCommands.Start:
                case DICCommands.Eject:
                case DICCommands.Close:
                case DICCommands.Reset:
                case DICCommands.DriveSpeed:
                    if (!IsValidDriveLetter(parts[1]))
                    {
                        return false;
                    }
                    else if (parts.Count > 2)
                    {
                        return false;
                    }
                    break;
                case DICCommands.Sub:
                case DICCommands.MDS:
                    if (IsFlag(parts[1]))
                    {
                        return false;
                    }
                    else if (parts.Count > 2)
                    {
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
                    switch(parts[i])
                    {
                        case DICFlags.DisableBeep:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.GDROM
                                && parts[0] != DICCommands.Data
                                && parts[0] != DICCommands.Audio)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.D8Opcode:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.GDROM
                                && parts[0] != DICCommands.Data
                                && parts[0] != DICCommands.Audio)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.MCN:
                            if (parts[0] != DICCommands.CompactDisc)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.AMSF:
                            if (parts[0] != DICCommands.CompactDisc)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.Reverse:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.Data)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.MultiSession:
                            if (parts[0] != DICCommands.CompactDisc)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.ScanSectorProtect:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.Data)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.ScanAntiMod:
                            if (parts[0] != DICCommands.CompactDisc)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.NoFixSubP:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.GDROM
                                && parts[0] != DICCommands.Data
                                && parts[0] != DICCommands.Audio)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.NoFixSubQ:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.GDROM
                                && parts[0] != DICCommands.Data
                                && parts[0] != DICCommands.Audio)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.NoFixSubRtoW:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.GDROM
                                && parts[0] != DICCommands.Data
                                && parts[0] != DICCommands.Audio)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.NoFixSubQLibCrypt:
                            if (parts[0] != DICCommands.CompactDisc)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.NoFixSubQSecuROM:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.GDROM
                                && parts[0] != DICCommands.Data
                                && parts[0] != DICCommands.Audio)
                            {
                                return false;
                            }
                            break;
                        case DICFlags.ScanFileProtect:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.Data)
                            {
                                return false;
                            }
                        
                            // If the next item doesn't exist, it's good
                            if (!DoesNextExist(parts, i))
                            {
                                break;
                            }
                            // If the next item is a flag, it's good
                            if (IsFlag(parts[i + 1]))
                            {
                                break;
                            }
                            // If the next item isn't a valid number
                            else if (!IsValidNumber(parts[i + 1], lowerBound: 0))
                            {
                                return false;
                            }
                            i++;
                            break;
                        case DICFlags.ForceUnitAccess: // CD, GDROM, Data, Audio
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.GDROM
                                && parts[0] != DICCommands.Data
                                && parts[0] != DICCommands.Audio)
                            {
                                return false;
                            }

                            // If the next item doesn't exist, it's good
                            if (!DoesNextExist(parts, i))
                            {
                                break;
                            }
                            // If the next item is a flag, it's good
                            if (IsFlag(parts[i + 1]))
                            {
                                break;
                            }
                            // If the next item isn't a valid number
                            else if (!IsValidNumber(parts[i + 1], lowerBound: 0))
                            {
                                return false;
                            }
                            i++;
                            break;
                        case DICFlags.AddOffset:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.Audio)
                            {
                                return false;
                            }

                            // If the next item doesn't exist, it's not good
                            if (parts.Count == i + 1)
                            {
                                return false;
                            }
                            // If the next item isn't a valid number
                            else if (IsValidNumber(parts[i + 1]))
                            {
                                return false;
                            }
                            break;
                        case DICFlags.BEOpcode:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.GDROM
                                && parts[0] != DICCommands.Data
                                && parts[0] != DICCommands.Audio)
                            {
                                return false;
                            }

                            // If the next item doesn't exist, it's good
                            if (!DoesNextExist(parts, i))
                            {
                                break;
                            }
                            // If the next item is a flag, it's good
                            if (IsFlag(parts[i + 1]))
                            {
                                break;
                            }
                            else if (parts[i + 1] != "raw"
                                && (parts[i + 1] != "pack"))
                            {
                                return false;
                            }
                            i++;
                            break;
                        case DICFlags.C2Opcode:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.GDROM
                                && parts[0] != DICCommands.Data
                                && parts[0] != DICCommands.Audio)
                            {
                                return false;
                            }

                            for (int j = 1; j < 4; j++)
                            {
                                // If the next item doesn't exist, it's good
                                if (!DoesNextExist(parts, i + j - 1))
                                {
                                    break;
                                }
                                // If the next item is a flag, it's good
                                if (IsFlag(parts[i + j]))
                                {
                                    i += (j - 1);
                                    break;
                                }
                                // If the next item isn't a valid number
                                else if (!IsValidNumber(parts[i + j], lowerBound: 0))
                                {
                                    return false;
                                }
                            }
                            break;
                        case DICFlags.SubchannelReadLevel:
                            if (parts[0] != DICCommands.CompactDisc
                                && parts[0] != DICCommands.GDROM
                                && parts[0] != DICCommands.Data
                                && parts[0] != DICCommands.Audio)
                            {
                                return false;
                            }

                            // If the next item doesn't exist, it's good
                            if (!DoesNextExist(parts, i))
                            {
                                break;
                            }
                            // If the next item is a flag, it's good
                            if (IsFlag(parts[i + 1]))
                            {
                                break;
                            }
                            // If the next item isn't a valid number
                            else if (!IsValidNumber(parts[3], lowerBound: 0, upperBound: 2))
                            {
                                return false;
                            }
                            break;
                        case DICFlags.SeventyFour:
                            if (parts[0] != DICCommands.Swap)
                            {
                                return false;
                            }
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
        private static bool IsValidDriveLetter(string parameter)
        {
            if (!Regex.IsMatch(parameter, @"^[A-Z]:?\\?$"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns whether a string is a flag (starts with '/')
        /// </summary>
        /// <param name="parameter">String value to check</param>
        /// <returns>True if it's a flag, false otherwise</returns>
        private static bool IsFlag(string parameter)
        {
            if (parameter.Trim('\"').StartsWith("/"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the next item exists
        /// </summary>
        /// <param name="parameters">List of parameters to check against</param>
        /// <param name="index">Current index</param>
        /// <returns>True if the next item exists, false otherwise</returns>
        private static bool DoesNextExist(List<string> parameters, int index)
        {
            if (index >= parameters.Count - 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns whether a string is a valid number
        /// </summary>
        /// <param name="parameter">String value to check</param>
        /// <param name="lowerBound">Lower bound (>=)</param>
        /// <param name="upperBound">Upper bound (<=)</param>
        /// <returns>True if it's a valid number, false otherwise</returns>
        private static bool IsValidNumber(string parameter, int lowerBound = -1, int upperBound = -1)
        {
            if (!Int32.TryParse(parameter, out int temp))
            {
                return false;
            }
            else if (lowerBound != -1 && temp < lowerBound)
            {
                return false;
            }
            else if (upperBound != -1 && temp > upperBound)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determine the base flags to use for checking a commandline
        /// </summary>
        /// <param name="parameters">Parameters as a string to check</param>
        /// <param name="type">Output nullable DiscType containing the found DiscType, if possible</param>
        /// <param name="system">Output nullable KnownSystem containing the found KnownSystem, if possible</param>
        /// <param name="letter">Output string containing the found drive letter</param>
        /// <param name="path">Output string containing the found path</param>
        /// <returns>False on error (and all outputs set to null), true otherwise</returns>
        public static bool DetermineFlags(string parameters, out DiscType? type, out KnownSystem? system, out string letter, out string path)
        {
            // Populate all output variables with null
            type = null; system = null; letter = null; path = null;

            // The string has to be valid by itself first
            if (String.IsNullOrWhiteSpace(parameters))
            {
                return false;
            }

            // Now split the string into parts for easier validation
            // https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
            parameters = parameters.Trim();
            List<string> parts = Regex.Matches(parameters, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();

            type = Converters.BaseCommmandToDiscType(parts[0]);
            system = Converters.BaseCommandToKnownSystem(parts[0]);

            // Determine what the commandline should look like given the first item
            switch (parts[0])
            {
                case DICCommands.CompactDisc:
                case DICCommands.GDROM:
                case DICCommands.Swap:
                case DICCommands.Data:
                case DICCommands.Audio:
                case DICCommands.DigitalVideoDisc:
                case DICCommands.BluRay:
                case DICCommands.XBOX:
                case DICCommands.Floppy:
                    if (!IsValidDriveLetter(parts[1]))
                    {
                        return false;
                    }
                    letter = parts[1];

                    if (IsFlag(parts[2]))
                    {
                        return false;
                    }
                    path = parts[2].Trim('\"');

                    // Special case for GameCube/Wii
                    if (parts.Contains(DICFlags.Raw))
                    {
                        type = DiscType.GameCubeGameDisc;
                        system = KnownSystem.NintendoGameCube;
                    }
                    // Special case for Saturn
                    else if (parts.Contains(DICFlags.SeventyFour))
                    {
                        type = DiscType.CD;
                        system = KnownSystem.SegaSaturn;
                    }

                    break;
                case DICCommands.Stop:
                case DICCommands.Start:
                case DICCommands.Eject:
                case DICCommands.Close:
                case DICCommands.Reset:
                case DICCommands.DriveSpeed:
                    if (!IsValidDriveLetter(parts[1]))
                    {
                        return false;
                    }
                    letter = parts[1];

                    break;
                case DICCommands.Sub:
                case DICCommands.MDS:
                    if (IsFlag(parts[1]))
                    {
                        return false;
                    }
                    path = parts[1].Trim('\"');

                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
