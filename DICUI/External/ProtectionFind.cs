﻿//this file is part of BurnOut
//Copyright (C)2005-2010 Gernot Knippen
//
//This program is free software; you can redistribute it and/or
//modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation; either
//version 2 of the License, or (at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU General Public License for more details.
//
//You can get a copy of the GNU General Public License
//by writing to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DICUI.External
{
    // TODO: Add any missing protection schemes
    public class ProtectionFind
    {
        public bool IsCDCheck;
        public bool IsDummyFiles;
        private bool IsImpulseReactorWithoutVersion;
        private bool IsSafeDiscRemovedVersion;
        private bool IsSolidShieldWithoutVersion;
        private bool IsStarForceWithoutVersion;
        private bool IsLaserLockWithoutVersion;

        private string SecuROMpaulversion;

        public ProtectionFind()
        {
        }

        public string Scan(string path, bool advancedscan, bool sizelimit = true)
        {
            string version = "";

            IsImpulseReactorWithoutVersion = false;
            IsLaserLockWithoutVersion = false;
            IsSafeDiscRemovedVersion = false;
            IsSolidShieldWithoutVersion = false;
            IsStarForceWithoutVersion = false;
            SecuROMpaulversion = "";

            string[] filesstr = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

            FileInfo[] files = new FileInfo[filesstr.Length];
            for (int i = 0; i < filesstr.Length; i++)
            {
                files[i] = new FileInfo(filesstr[i]);
            }
            filesstr = null;

            string[] EXEFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".icd") || s.EndsWith(".dat") || s.EndsWith(".exe") || s.EndsWith(".dll")).ToArray();

            if (EXEFiles.Length != 0)
            {
                for (int i = 0; i < EXEFiles.Length; i++)
                {
                    FileInfo filei = new FileInfo(EXEFiles[i]);
                    if (filei.Length > 352 && !(sizelimit && filei.Length > 20971520))
                    {
                        Console.WriteLine("scanning file Nr." + i + "(" + EXEFiles[i] + ")");
                        string protectionname = ScanInFile(EXEFiles[i], advancedscan);
                        if (!String.IsNullOrEmpty(protectionname))
                        {
                            if (IsImpulseReactorWithoutVersion)
                            {
                                IsImpulseReactorWithoutVersion = false;
                                if (ImpulseReactor(out version, files))
                                    return "Impulse Reactor " + version;
                            }
                            else if (IsLaserLockWithoutVersion)
                            {
                                IsLaserLockWithoutVersion = false;
                                if (LaserLock(out version, path, files) && version.Length > 0)
                                    return "LaserLock " + version;
                            }
                            else if (IsSafeDiscRemovedVersion)
                            {
                                IsSafeDiscRemovedVersion = false;
                                if (SafeDisc2(out version, path, files) && version != "2-4")
                                    return "SafeDisc " + version;
                                if (SafeDisc3(out version, path, files) && version != "2-4")
                                    return "SafeDisc " + version;
                                if (SafeDisc4(out version, path, files) && version != "2-4")
                                    return "SafeDisc " + version;
                            }
                            else if (IsSolidShieldWithoutVersion)
                            {
                                IsSolidShieldWithoutVersion = false;
                                if (SolidShield(out version, files) && version.Length > 0)
                                    return "SolidShield " + version;
                            }
                            else if (IsStarForceWithoutVersion)
                            {
                                IsStarForceWithoutVersion = false;
                                if (StarForce(out version, files) && version.Length > 0)
                                    return "StarForce " + version;
                            }

                            if (SecuROMpaulversion.Length > 0)
                            {
                                if (!protectionname.StartsWith("SecuROM Product Activation"))
                                    return protectionname + " + SecuROM Product Activation" + SecuROMpaulversion;
                            }
                            else
                                return protectionname;
                        }
                    }
                }
            }

            EXEFiles = null;

            if (AACS(path))
                return "AACS";
            if (AlphaDVD(files))
                return "Alpha-DVD";
            if (Bitpool(files))
                return "Bitpool";
            if (ByteShield(path, files))
                return "ByteShield";
            if (Cactus(out version, files))
                return "Cactus Data Shield " + version;
            if (CDCops(path, files))
                return "CD-Cops";
            if (CDProtector(path, files))
                return "CD-Protector";
            if (CDLock(path))
                return "CD-Lock";
            if (CDX(files))
                return "CD-X";
            if (DiskGuard(files))
                return "Diskguard";
            if (DVDCrypt(files))
                return "DVD Crypt";
            if (DVDMoviePROTECT(path))
                return "DVD-Movie-PROTECT";
            if (FreeLock(files))
                return "FreeLock";
            if (HexalockAutoLock(files))
                return "Hexalock AutoLock";
            if (ImpulseReactor(out version, files))
                return "Impulse Reactor " + version;
            if (IndyVCD(files))
                return "IndyVCD";
            if (Key2AudioXS(files))
                return "Key2Audio XS";
            if (LaserLock(out version, path, files))
                return "LaserLock " + version;
            if (MediaCloQ(files))
                return "MediaCloQ";
            if (MediaMaxCD3(files))
                return "MediaMax CD-3";
            if (Origin(files))
                return "Origin";
            if (ProtectDVDVideo(path))
                return "Protect DVD-Video";
            if (PSX(path, files))
                return "PSX Libcrypt";
            if (SafeCast(files))
                return "SafeCast";
            if (SafeDiscLite(files))
                return "SafeDisc Lite";
            if (SafeDisc4(out version, path, files))
                return "SafeDisc " + version;
            if (SafeDisc3(out version, path, files))
                return "SafeDisc " + version;
            if (SafeDisc2(out version, path, files))
                return "SafeDisc " + version;
            if (TZCopyProtector(files))
                return "TZCopyProtector";
            if (SafeDisc1(out version, path, files))
                return "SafeDisc " + version;
            if (Safe_Lock(files))
                return "SafeLock";
            if (SecuROM(files))
                return "SecuROM";
            if (SecuROMnew(files))
                return "SecuROM new";
            if (SmartE(files))
                return "SmartE";
            if (SolidShield(out version, files))
                return "SolidShield " + version;
            if (Softlock(files))
                return "Softlock";
            if (StarForce(out version, files))
                return "StarForce " + version;
            if (Steam(files))
                return "Steam";
            if (Tages(files))
                return "TAGES";
            if (UPlay(files))
                return "UPlay";
            if (VOBProtectCDDVD(files))
                return "VOB ProtectCD/DVD";
            if (WinLock(files))
                return "Winlock";
            if (WTMCDProtect(path))
                return "WTM CD Protect";
            if (WTMCopyProtection(out version, files))
                return "WTM Copy Protection " + version;
            if (XCP(path, files))
                return "XCP";
            if (CopyKiller(files))
                return "Could be CopyKiller / SecuROM";
            if (DummyFiles(files))
                IsDummyFiles = true;
            return "";
        }

        private string ScanInFile(string file, bool advancedscan)
        {
            string FileContent = "";
            StreamReader sr;
            int position;
            try
            {
                sr = new StreamReader(file, Encoding.Default);
            }
            catch
            {
                return "";
            }

            try
            {
                if (!((sr.BaseStream.ReadByte() == 0x4D) && (sr.BaseStream.ReadByte() == 0x5A))) // file is non-executable
                {
                    sr.Close();
                    return "";
                }
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                FileContent = sr.ReadToEnd();
            }
            catch (Exception)
            {
                if (FileContent.Length == 0)
                {
                    sr.Close();
                    return "";
                }
            }

            sr.Close();

            position = FileContent.IndexOf("BoG_ *90.0&!!  Yy>");
            if (position > -1)
            {
                position--;
                if (FileContent.IndexOf("product activation library") > 0)
                    return "SafeCast " + GetSafeDiscVersion(file, position);
                else
                    return "SafeDisc " + GetSafeDiscVersion(file, position);
            }

            position = FileContent.IndexOf("AddD" + (char)0x03 + (char)0x00 + (char)0x00 + (char)0x00);
            if (position > -1)
            {
                return "SecuROM " + GetSecuROM4Version(file, position);
            }

            position = FileContent.IndexOf("" + (char)0xCA + (char)0xDD + (char)0xDD + (char)0xAC + (char)0x03);
            if (position > -1)
            {
                position--;;
                return "SecuROM " + GetSecuROM4and5Version(file, position);
            }

            if (FileContent.StartsWith(".securom" + (char)0xE0 + (char)0xC0))
                return "SecuROM " + GetSecuROM7Version(file);

            if (FileContent.Contains("_and_play.dll" + (char)0x00 + "drm_pagui_doit"))
            {
                SecuROMpaulversion = GetFileVersion(file);
                return "SecuROM Product Activation " + SecuROMpaulversion;
            }

            position = FileContent.IndexOf("CD-Cops,  ver. ");
            if (position > -1)
            {
                return "CD-Cops " + GetCDDVDCopsVersion(file, position);
            }

            position = FileContent.IndexOf("DVD-Cops,  ver. ");
            if (position > -1)
            {
                return "DVD-Cops " + GetCDDVDCopsVersion(file, position);
            }

            position = FileContent.IndexOf("VOB ProtectCD");
            if (position > -1)
            {
                position--;
                return "VOB ProtectCD/DVD " + GetProtectCDoldVersion(file, position);
            }

            position = FileContent.IndexOf("V SUHPISYS");
            if (position > -1)
            {
                if (FileContent.Substring(position + 10, 3) == "DVD")
                    return "Sysiphus DVD " + GetSysiphusVersion(file, position);
                else
                    return "Sysiphus " + GetSysiphusVersion(file, position);
            }

            if (FileContent.Contains(".sforce") || FileContent.Contains(".brick"))
            {
                IsStarForceWithoutVersion = true;
                return "StarForce 3-5";
            }

            if (FileContent.Contains("P" + (char)0x00 + "r" + (char)0x00 + "o" + (char)0x00 + "t" + (char)0x00 + "e" + (char)0x00 + "c" + (char)0x00 + "t" + (char)0x00 + "e" + (char)0x00 + "d" + (char)0x00 + " " + (char)0x00 + "M" + (char)0x00 + "o" + (char)0x00 + "d" + (char)0x00 + "u" + (char)0x00 + "l" + (char)0x00 + "e"))
            {
                IsStarForceWithoutVersion = true;
                return "StarForce 5";
            }

            if (FileContent.Contains("(" + (char)0x00 + "c" + (char)0x00 + ")" + (char)0x00 + " " + (char)0x00 + "P" + (char)0x00 + "r" + (char)0x00 + "o" + (char)0x00 + "t" + (char)0x00 + "e" + (char)0x00 + "c" + (char)0x00 + "t" + (char)0x00 + "i" + (char)0x00 + "o" + (char)0x00 + "n"
                + (char)0x00 + " " + (char)0x00 + "T" + (char)0x00 + "e" + (char)0x00 + "c" + (char)0x00 + "h" + (char)0x00 + "n" + (char)0x00 + "o" + (char)0x00 + "l" + (char)0x00 + "o" + (char)0x00 + "g" + (char)0x00 + "y" + (char)0x00)
            || FileContent.Contains("Protection Technology, Ltd."))
            {
                //if (FileContent.Contains("PSA_GetDiscLabel")
                //if (FileContent.Contains("(c) Protection Technology")
                position = FileContent.IndexOf("TradeName") - 1;
                if (position != -1 && position != -2)
                    return "StarForce " + GetFileVersion(file) + " (" + FileContent.Substring(position + 22, 30).Split((char)0x00)[0] + ")";
                else
                    return "StarForce " + GetFileVersion(file);
            }

            if (FileContent.Contains("D" + (char)0x00 + "V" + (char)0x00 + "M" + (char)0x00 + " " + (char)0x00 + "L" + (char)0x00 + "i" + (char)0x00 + "b" + (char)0x00 + "r" + (char)0x00 + "a" + (char)0x00 + "r" + (char)0x00 + "y"))
            {
                return "SolidShield " + GetFileVersion(file);
            }

            if (FileContent.Contains("Packed by SPEEnc V2 Asterios Parlamentas.PE"))
            {
                position = FileContent.IndexOf("GetModuleHandleA" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "GetProcAddress" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "LoadLibraryA" + (char)0x00 + (char)0x00 + "KERNEL32.dll" + (char)0x00 + "ëy" + (char)0x01 + "SNIF");
                if (position > -1)
                {
                    position--;
                    return "LaserLock " + GetLaserLockVersion(FileContent, position) + " " + GetLaserLockBuild(FileContent, true);
                }
                else
                    return "LaserLock Marathon " + GetLaserLockBuild(FileContent, false);
            }

            position = FileContent.IndexOf("GetModuleHandleA" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "GetProcAddress" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "LoadLibraryA" + (char)0x00 + (char)0x00 + "KERNEL32.dll" + (char)0x00 + "ëy" + (char)0x01 + "SNIF");
            if (position > -1)
            {
                position--;;
                return "LaserLock " + GetLaserLockVersion(FileContent, position) + " " + GetLaserLockBuild(FileContent, false);
            }

            if (FileContent.IndexOf("LASERLOK_INIT" + (char)0xC + "LASERLOK_RUN" + (char)0xE + "LASERLOK_CHECK" + (char)0xF + "LASERLOK_CHECK2" + (char)0xF + "LASERLOK_CHECK3") > -1)
            {
                IsLaserLockWithoutVersion = true;
                return "LaserLock 5";
            }

            if (FileContent.IndexOf(":\\LASERLOK\\LASERLOK.IN" + (char)0x0 + "C:\\NOMOUSE.SP") > -1)
            {
                IsLaserLockWithoutVersion = true;
                return "LaserLock 3";
            }

            if (FileContent.IndexOf(".ext    ") > -1)
            {
                position = FileContent.IndexOf("kernel32.dll" + (char)0x00 + (char)0x00 + (char)0x00 + "VirtualProtect");
                if (position > -1)
                {
                    position--;;
                    return "JoWooD X-Prot " + GetJoWooDXProt1Version(file, position);
                }
                else
                    return "JoWooD X-Prot v1";
            }

            position = FileContent.IndexOf("HúMETINF");
            if (position > -1)
            {
                position--;
                if (advancedscan)
                {
                    string version = EVORE.SearchProtectDiscVersion(file);
                    if (version.Length > 0)
                    {
                        string[] astrVersionArray = version.Split('.');
                        if (astrVersionArray[0] == "9")
                        {
                            if (GetProtectDiscVersionBuild76till10(file, position, out int ibuild).Length > 0)
                                return "ProtectDisc " + astrVersionArray[0] + "." + astrVersionArray[1] + astrVersionArray[2] + "." + astrVersionArray[3] + " (Build " + ibuild + ")";
                        }
                        else
                            return "ProtectDisc " + astrVersionArray[0] + "." + astrVersionArray[1] + "." + astrVersionArray[2] + " (Build " + astrVersionArray[3] + ")";
                    }
                }

                return "ProtectDisc " + GetProtectDiscVersionBuild76till10(file, position, out int irefBuild);
            }
            
            if (FileContent.IndexOf((char)0x00 + (char)0x00 + "BoG_") > -1
                || SuffixInStr(FileContent, "stxt", "774", "371") > -1)
            {
                if (advancedscan)
                {
                    string version = EVORE.SearchSafeDiscVersion(file);
                    if (version.Length > 0)
                        return "SafeDisc " + version;
                }

                IsSafeDiscRemovedVersion = true;
                return "SafeDisc 3.20-4.xx (version removed)";
            }

            position = FileContent.IndexOf("ACE-PCD");
            if (position > -1)
            {
                position--;
                if (advancedscan)
                {
                    string version;
                    version = EVORE.SearchProtectDiscVersion(file);
                    if (version.Length > 0)
                    {
                        string[] astrVersionArray = version.Split('.');
                        return "ProtectDisc " + astrVersionArray[0] + "." + astrVersionArray[1] + "." + astrVersionArray[2] + " (Build " + astrVersionArray[3] + ")";
                    }
                }
                return "ProtectDisc " + GetProtectDiscVersionBuild6till8(file, position);
            }

            position = FileContent.IndexOf("DCP-BOV" + (char)0x00 + (char)0x00);
            if (position > -1)
            {
                position--;
                string version;
                version = GetVOBProtectCDDVDVersion(file, position);
                if (version.Length > 0)
                {
                    return "VOB ProtectCD/DVD " + version;
                }
                if (advancedscan)
                {
                    version = EVORE.SearchProtectDiscVersion(file);
                    if (version.Length > 0)
                    {
                        if (version.StartsWith("2"))
                        {
                            version = "6" + version.Substring(1);
                        }
                        return "VOB ProtectCD/DVD " + version;
                    }
                }
                return "VOB ProtectCD/DVD 5.9-6.0" + GetVOBProtectCDDVDBuild(file, position);
            }

            if (FileContent.IndexOf("" + (char)0x20 + (char)0xC2 + (char)0x16 + (char)0x0 + (char)0xA8 + (char)0xC1 + (char)0x16
                + (char)0x0 + (char)0xB8 + (char)0xC1 + (char)0x16 + (char)0x0 + (char)0x86 + (char)0xC8 + (char)0x16 + (char)0x0
                + (char)0x9A + (char)0xC1 + (char)0x16 + (char)0x0 + (char)0x10 + (char)0xC2 + (char)0x16 + (char)0x0) > -1)
            {
                return "ActiveMARK 5";
            }

            if (FileContent.IndexOf("TMSAMVOF") > -1)
            {
                return "ActiveMARK";
            }

            if (FileContent.IndexOf("SETTEC") > -1)
            {
                return "Alpha-ROM";
            }

            if (FileContent.IndexOf(".nicode" + (char)0x00) > -1 || FileContent.IndexOf("ARMDEBUG") > -1)
            {
                return "Armadillo";
            }

            if (FileContent.IndexOf(".ldr") > -1 && FileContent.IndexOf(".ldt") > -1)
            {
                //|| FileContent.IndexOf((char)89 + (char)195 + (char)85 + (char)139 + (char)236 + (char)131 _
                // & (char)236 + (char)48 + (char)83 + (char)86 + (char)87) ' = YÃU‹ìƒì0SVW
                return "3PLock";
            }

            if (FileContent.Contains(".grand" + (char)0x00))
            {
                return "CD-Cops";
            }

            if (FileContent.Contains("" + (char)50 + (char)242 + (char)0x02 + (char)130 + (char)195 + (char)188 + (char)11
                + (char)36 + (char)153 + (char)173 + (char)39 + (char)67 + (char)228 + (char)157 + (char)115 + (char)116
                + (char)153 + (char)250 + (char)50 + (char)36 + (char)157 + (char)41 + (char)52 + (char)255 + (char)116))
            {
                return "CD-Lock";
            }

            if (FileContent.Contains("~0017.tmp"))
            {
                return "CDSHiELD SE";
            }

            if (FileContent.Contains(".cenega"))
            {
                return "Cenega ProtectDVD";
            }

            if (SuffixInStr(FileContent, "icd", "1" + (char)0x00, "2" + (char)0x00) > -1 || FileContent.Contains("CODE-LOCK.OCX"))
            {
                return "Code Lock";
            }

            if (FileContent.Contains("Tom Commander"))
            {
                return "CopyKiller";
            }

            if (FileContent.Contains("" + (char)0x3F + (char)0x3F + (char)0x5B + (char)0x5B + (char)0x5F + (char)0x5F
                + (char)0x5B + (char)0x5B + (char)0x5F + (char)0x0 + (char)0x7B + (char)0x7B + (char)0x0
                + (char)0x0 + (char)0x7B + (char)0x7B + (char)0x0 + (char)0x0 + (char)0x0 + (char)0x0 + (char)0x0
                + (char)0x0 + (char)0x0 + (char)0x0 + (char)0x0 + (char)0x3F + (char)0x3B + (char)0x3B + (char)0x3F
                + (char)0x3F + (char)0x3B + (char)0x3B + (char)0x3F + (char)0x3F))
            {
                return "EXE Stealth";
            }

            if (FileContent.Contains("xlive.dll"))
            {
                return "Games for Windows - Live";
            }

            if (FileContent.Contains("CVPInitializeClient"))
            {
                if (FileContent.Contains("A" + (char)0x0 + "T" + (char)0x0 + "T" + (char)0x0 + "L" + (char)0x0 + "I" + (char)0x0 + "S" + (char)0x0 + "T" + (char)0x0 + (char)0x0 + (char)0x0 + "E" + (char)0x0 + "L" + (char)0x0 + "E" + (char)0x0 + "M" + (char)0x0 + "E" + (char)0x0 + "N" + (char)0x0 + "T" + (char)0x0 + (char)0x0 + (char)0x0 + "N" + (char)0x0 + "O" + (char)0x0 + "T" + (char)0x0 + "A" + (char)0x0 + "T" + (char)0x0 + "I" + (char)0x0 + "O" + (char)0x0 + "N"))
                {
                    return "Impulse Reactor " + GetFileVersion(file);
                }
                else
                {
                    IsImpulseReactorWithoutVersion = true;
                    return "Impulse Reactor";
                }
            }

            if (FileContent.Contains("@HC09    "))
            {
                return "JoWooD X-Prot v2";
            }

            if (FileContent.Contains("KEY-LOCK COMMAND"))
            {
                return "Key-Lock (Dongle)";
            }

            if (FileContent.Contains((char)0x00 + "Allocator" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00))
            {
                return "Ring-Protech";
            }

            if (FileContent.Contains("SafeLock"))
            {
                return "SafeLock";
            }

            if (SuffixInStr(FileContent, ".cms_", "t" + (char)0x00, "d" + (char)0x00) > -1)
            {
                return "SecuROM 1-3";
            }

            if (FileContent.Contains("BITARTS"))
            {
                return "SmartE";
            }

            if (FileContent.Contains("" + (char)0x42 + (char)0x0 + (char)0x49 + (char)0x0 + (char)0x4E + (char)0x0 + (char)0x7 + (char)0x0 +
                (char)0x49 + (char)0x0 + (char)0x44 + (char)0x0 + (char)0x52 + (char)0x0 + (char)0x5F + (char)0x0 +
                (char)0x53 + (char)0x0 + (char)0x47 + (char)0x0 + (char)0x54 + (char)0x0))
            {
                return "SolidShield"; //B.I.N...I.D.R._.S.G.T.
            }

            position = FileContent.IndexOf("" + (char)0xEF + (char)0xBE + (char)0xAD + (char)0xDE);
            if (position > -1)
            {
                position--;
                if (FileContent.Substring(position + 5, 3) == "" + (char)0x00 + (char)0x00 + (char)0x00
                && FileContent.Substring(position + 16, 4) == "" + (char)0x00 + (char)0x10 + (char)0x00 + (char)0x00)
                {
                    IsSolidShieldWithoutVersion = true;
                    return "SolidShield 1";
                }
            }

            position = SuffixInStr(FileContent, "" + (char)0xAD + (char)0xDE + (char)0xFE + (char)0xCA, "" + (char)0x5, "" + (char)0x4);
            if (position > -1)
            {
                position--;
                if (FileContent.Substring(position + 5, 3) == "" + (char)0x00 + (char)0x00 + (char)0x00
                    && FileContent.Substring(position + 16, 4) == "" + (char)0x00 + (char)0x10 + (char)0x00 + (char)0x00)
                {
                    return "SolidShield 2";
                }
                else if (FileContent.Substring(position + 5, 3) == "" + (char)0x00 + (char)0x00 + (char)0x00
                    && FileContent.Substring(position + 16, 4) == "" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00)
                {
                    position = FileContent.IndexOf("T" + (char)0x0 + "a" + (char)0x0 + "g" + (char)0x0 + "e" + (char)0x0 + "s" + (char)0x0 + "S" + (char)0x0 + "e" + (char)0x0 + "t" + (char)0x0 + "u" + (char)0x0 + "p"
                        + (char)0x0 + (char)0x0 + (char)0x0 + (char)0x0 + (char)0x0 + (char)0x30 + (char)0x0 + (char)0x8 + (char)0x0 + (char)0x1 + (char)0x0
                        + "F" + (char)0x0 + "i" + (char)0x0 + "l" + (char)0x0 + "e" + (char)0x0 + "V" + (char)0x0 + "e" + (char)0x0 + "r" + (char)0x0 + "s" + (char)0x0 + "i" + (char)0x0 + "o" + (char)0x0 + "n"
                        + (char)0x0 + (char)0x0 + (char)0x0 + (char)0x0);
                    if (position > -1)
                    {
                        position--;
                        return "SolidShield 2 + Tagès " + FileContent.Substring(position + 0x38, 1) + "." + FileContent.Substring(position + 0x38 + 4, 1) + "." + FileContent.Substring(position + 0x38 + 8, 1) + "." + FileContent.Substring(position + 0x38 + 12, 1);
                    }
                    else
                    {
                        IsSolidShieldWithoutVersion = true;
                        return "SolidShield 2";
                    }
                }
            }

            if (FileContent.Contains("?SVKP" + (char)0x00 + (char)0x00))
            {
                return "SVK Protector";
            }

            if (FileContent.Contains("protected-tages-runtime.exe") ||
                FileContent.Contains("tagesprotection.com"))
            {
                return "Tagès " + GetFileVersion(file);
            }

            position = FileContent.IndexOf("" + (char)0xE8 + (char)0x75 + (char)0x0 + (char)0x0 + (char)0x0 + (char)0xE8);
            if (position > -1)
            {
                position--;
                if (FileContent.Substring(position + 8, 3) == "" + (char)0xFF + (char)0xFF + (char)0x68)
                {
                    return "Tagès " + GetTagesVersion(file, position);
                }
            }

            if (FileContent.Contains(".vob.pcd"))
            {
                return "VOB ProtectCD";
            }

            if (FileContent.Contains("WTM76545"))
            {
                return "WTM CD Protect";
            }

            if (FileContent.Contains("XPROT   "))
            {
                return "Xtreme-Protector";
            }

            if (!IsCDCheck)
            {
                if (FileContent.Contains("GetDriveType") || FileContent.Contains("GetVolumeInformation"))
                {
                    IsCDCheck = true;
                }
            }

            return "";
        }

        #region Protections

        private bool AACS(string path)
        {
            if (File.Exists(Path.Combine(path, "aacs", "VTKF000.AACS")))
                return true;
            if (File.Exists(Path.Combine(path, "AACS", "CPSUnit00001.cci")))
                return true;
            return false;
        }

        private bool AlphaDVD(FileInfo[] files)
        {
            return files.Select(fi => Path.GetFileName(fi.FullName)).Contains("PlayDVD.exe");
        }

        private bool Bitpool(FileInfo[] files)
        {
            return files.Select(fi => Path.GetFileName(fi.FullName)).Contains("bitpool.rsc");
        }

        private bool ByteShield(string path, FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("Byteshield.dll"))
                return true;
            if (Directory.GetFiles(path, "*.bbz", SearchOption.AllDirectories).Length > 0)
                return true;
            return false;
        }

        private bool Cactus(out string version, FileInfo[] files)
        {
            bool found = false;
            int fileindex;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("yucca.cds"))
                found = true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("wmmp.exe"))
                found = true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("PJSTREAM.DLL"))
                found = true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CACTUSPJ.exe"))
                found = true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CDSPlayer.app"))
                found = true;
            if (found)
            {
                //get the exact version
                fileindex = files.Select(fi => Path.GetFileName(fi.FullName)).ToList().IndexOf("CDSPlayer.app");
                if (fileindex > -1)
                {
                    StreamReader sr = new StreamReader(files[fileindex].FullName);
                    version = sr.ReadLine().Substring(3) + " (" + sr.ReadLine() + ")";
                }
                else
                    version = "200";
                return true;
            }

            version = "";
            return false;
        }

        private bool CDCops(string path, FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CDCOPS.DLL"))
                return true;
            if (Directory.GetFiles(path, "*.GZ_", SearchOption.AllDirectories).Length > 0)
                return true;
            if (Directory.GetFiles(path, "*.W_X", SearchOption.AllDirectories).Length > 0)
                return true;
            if (Directory.GetFiles(path, "*.Qz", SearchOption.AllDirectories).Length > 0)
                return true;
            if (Directory.GetFiles(path, "*.QZ_", SearchOption.AllDirectories).Length > 0)
                return true;
            return false;
        }

        private bool CDLock(string path)
        {
            if (Directory.GetFiles(path, "*.AFP", SearchOption.AllDirectories).Length > 0)
                return true;
            return false;
        }

        private bool CDProtector(string path, FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("_cdp16.dat"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("_cdp16.dll"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("_cdp32.dat"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("_cdp32.dll"))
                return true;
            return false;
        }

        //for BurnOut not detecting itself as SafeLock protected
        private bool CDX(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CHKCDX16.DLL"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CHKCDX32.DLL"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CHKCDXNT.DLL"))
                return true;
            return false;
        }

        private bool CopyKiller(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("Autorun.dat"))
                return true;
            return false;
        }

        private bool DiskGuard(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("IOSLINK.VXD"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("IOSLINK.DLL"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("IOSLINK.SYS"))
                return true;
            return false;
        }

        private bool DVDCrypt(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("DvdCrypt.pdb"))
                return true;
            return false;
        }

        private bool DVDMoviePROTECT(string path)
        {
            if (Directory.Exists(Path.Combine(path, "VIDEO_TS")))
            {
                string[] bupfiles = Directory.GetFiles(path, "*.bup", SearchOption.AllDirectories);
                for (int i = 0; i < bupfiles.Length; i++)
                {
                    FileInfo bupfile = new FileInfo(bupfiles[i]);
                    FileInfo ifofile = new FileInfo(bupfile.DirectoryName + "\\" + bupfile.Name.Substring(0, bupfile.Name.Length - bupfile.Extension.Length) + ".ifo");
                    if (bupfile.Length != ifofile.Length)
                        return true;
                }
            }

            return false;
        }

        private bool FreeLock(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("FREELOCK.IMG"))
                return true;
            return false;
        }

        private bool HexalockAutoLock(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("Start_Here.exe"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("HCPSMng.exe"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("MFINT.DLL"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("MFIMP.DLL"))
                return true;
            return false;
        }

        private bool ImpulseReactor(out string version, FileInfo[] files)
        {
            version = "";
            int i = files.Select(fi => Path.GetFileName(fi.FullName)).ToList().IndexOf("ImpulseReactor.dll");
            if (i > -1)
            {
                version = GetFileVersion(files[i].FullName);
            }

            return false;
        }

        private bool IndyVCD(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("INDYVCD.AX"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("INDYMP3.idt"))
                return true;
            return false;
        }

        private bool Key2AudioXS(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("SDKHM.EXE"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("SDKHM.DLL"))
                return true;
            return false;
        }

        private bool LaserLock(out string version, string path, FileInfo[] files)
        {
            version = "";
            int nomouseindex = files.Select(fi => Path.GetFileName(fi.FullName)).ToList().IndexOf("NOMOUSE.SP");
            if (nomouseindex > -1)
            {
                version = GetLaserLockVersion16Bit(files[nomouseindex].FullName);
                return true;
            }

            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("NOMOUSE.COM"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("l16dll.dll"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("laserlok.in"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("laserlok.o10"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("laserlok.011"))
                return true;
            if (Directory.Exists(Path.Combine(path, "LASERLOK")))
                return true;

            return false;
        }

        private bool MediaCloQ(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("sunncomm.ico"))
                return true;
            return false;
        }

        private bool MediaMaxCD3(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("LaunchCd.exe"))
                return true;
            return false;
        }

        // TODO: Properly fill out
        private bool Origin(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("OriginInstall.exe"))
                return true;
            return false;
        }

        private bool ProtectDVDVideo(string path)
        {
            if (Directory.Exists(Path.Combine(path, "VIDEO_TS")))
            {
                string[] ifofiles = Directory.GetFiles(path, "*.ifo", SearchOption.AllDirectories);
                for (int i = 0; i < ifofiles.Length; i++)
                {
                    FileInfo ifofile = new FileInfo(ifofiles[i]);
                    if (ifofile.Length == 0)
                        return true;
                }
            }

            return false;
        }

        // TODO: Fix
        private bool PSX(string path, FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("SLES_016.83"))
                return true;
            if (Directory.GetFiles(path, "*.cnf", SearchOption.AllDirectories).Length > 0)
                return true;
            return false;
        }

        private bool SafeCast(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("cdac11ba.exe"))
                return true;
            return false;
        }

        private bool SafeDisc1(out string version, string path, FileInfo[] files)
        {
            version = "";
            bool found = false;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("00000001.TMP"))
                found = true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CLCD16.DLL"))
                found = true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CLCD32.DLL"))
                found = true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CLOKSPL.EXE"))
                found = true;
            int fileindex = files.Select(fi => Path.GetFileName(fi.FullName)).ToList().IndexOf("DPLAYERX.DLL");
            if (fileindex > -1)
            {
                found = true;
                if (files[fileindex].Length == 81408)
                    version = "1.0x";
                if (files[fileindex].Length == 155648)
                    version = "1.1x";
                if (files[fileindex].Length == 156160)
                    version = "1.1x-1.2x";
                if (files[fileindex].Length == 163328)
                    version = "1.3x";
                if (files[fileindex].Length == 165888)
                    version = "1.35";
                if (files[fileindex].Length == 172544)
                    version = "1.40";
                if (files[fileindex].Length == 173568)
                    version = "1.4x";
                if (files[fileindex].Length == 136704)
                    version = "1.4x";
                if (files[fileindex].Length == 138752)
                    version = "1.5x";
            }
            fileindex = files.Select(fi => Path.GetFileName(fi.FullName).ToLower()).ToList().IndexOf("drvmgt.dll");
            if (fileindex > -1)
            {
                found = true;
                if (version.Length == 0)
                {
                    if (files[fileindex].Length == 34816)
                        version = "1.0x";
                    if (files[fileindex].Length == 32256)
                        version = "1.1x-1.3x";
                    if (files[fileindex].Length == 31744)
                        version = "1.4x";
                    if (files[fileindex].Length == 34304)
                        version = "1.5x";
                }
            }
            if (Directory.GetFiles(path, "*.ICD", SearchOption.AllDirectories).Length > 0)
                found = true;
            if (Directory.GetFiles(path, "*.016", SearchOption.AllDirectories).Length > 0)
                found = true;
            if (Directory.GetFiles(path, "*.256", SearchOption.AllDirectories).Length > 0)
                found = true;
            if (found == true)
            {
                if (version.Length == 0)
                    version = "1";
                return true;
            }

            return false;
        }

        private bool SafeDisc2(out string version, string path, FileInfo[] files)
        {
            version = "";
            bool found = false;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("00000002.TMP"))
                found = true;
            int fileindexdrvmgt = files.Select(fi => Path.GetFileName(fi.FullName).ToLower()).ToList().IndexOf("drvmgt.dll");
            int fileindexsecdrv = files.Select(fi => Path.GetFileName(fi.FullName).ToLower()).ToList().IndexOf("secdrv.sys");
            if (fileindexsecdrv > -1)
            {
                if (files[fileindexsecdrv].Length == 18768)
                {
                    found = true;
                    version = "2.50";
                }
            }
            if (fileindexsecdrv > -1 && fileindexdrvmgt > -1)
            {
                if (files[fileindexdrvmgt].Length == 34304 && files[fileindexsecdrv].Length == 20128)
                    version = "2.10";
                if (files[fileindexdrvmgt].Length == 34304 && files[fileindexsecdrv].Length == 27440)
                    version = "2.30";
                if (files[fileindexdrvmgt].Length == 34304 && files[fileindexsecdrv].Length == 28624)
                    version = "2.40";
                if (files[fileindexdrvmgt].Length == 35840 && files[fileindexsecdrv].Length == 28400)
                    version = "2.51";
                if (files[fileindexdrvmgt].Length == 35840 && files[fileindexsecdrv].Length == 29392)
                    version = "2.60";
                if (files[fileindexdrvmgt].Length == 40960 && files[fileindexsecdrv].Length == 11376)
                    version = "2.70";
                if (files[fileindexdrvmgt].Length == 23552 && files[fileindexsecdrv].Length == 12464)
                    version = "2.80";
                if (files[fileindexdrvmgt].Length == 41472 && files[fileindexsecdrv].Length == 12400)
                    version = "2.90";
                if (files[fileindexdrvmgt].Length == 41472 && files[fileindexsecdrv].Length == 12528)
                    version = "3.10";
                if (files[fileindexdrvmgt].Length == 24064 && files[fileindexsecdrv].Length == 12528)
                    version = "3.15";
                if (files[fileindexdrvmgt].Length == 24064 && files[fileindexsecdrv].Length == 11973)
                    version = "3.20";
                if (version != "")
                    found = true;
            }
            if (fileindexdrvmgt > -1 && version == "")
            {
                if (files[fileindexdrvmgt].Length == 34304)
                    version = "2.0x";
                if (files[fileindexdrvmgt].Length == 35840)
                    version = "2.6x";
                if (files[fileindexdrvmgt].Length == 40960)
                    version = "2.7x";
                if (files[fileindexdrvmgt].Length == 23552)
                    version = "2.8x";
                if (files[fileindexdrvmgt].Length == 41472)
                    version = "2.9x";
                if (version != "")
                    found = true;
            }
            if (found == true)
            {
                if (version == "")
                    version = "2-4";
                return true;
            }

            return false;
        }

        // TODO: Populate properly
        private bool SafeDisc3(out string version, string path, FileInfo[] files)
        {
            version = "";
            return false;
        }

        // TODO: Populate properly
        private bool SafeDisc4(out string version, string path, FileInfo[] files)
        {
            version = "";
            return false;
        }

        private bool SafeDiscLite(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("00000001.LT1"))
                return true;
            return false;
        }

        //for BurnOut not detecting itself as SafeLock protected
        private bool Safe_Lock(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("SafeLock.dat"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("SafeLock.001"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("SafeLock.128"))
                return true;
            return false;
        }

        private bool SecuROM(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CMS16.DLL"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CMS_95.DLL"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CMS_NT.DLL"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CMS32_95.DLL"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("CMS32_NT.DLL"))
                return true;
            return false;
        }

        private bool SecuROMnew(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("SINTF32.DLL"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("SINTF16.DLL"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("SINTFNT.DLL"))
                return true;
            return false;
        }

        private bool SmartE(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("00001.TMP"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("00002.TMP"))
                return true;
            return false;
        }

        private bool SolidShield(out string version, FileInfo[] files)
        {
            version = "";
            int fileindex = files.Select(fi => Path.GetFileName(fi.FullName)).ToList().IndexOf("dvm.dll");
            if (fileindex > -1)
            {
                version = ScanInFile(files[fileindex].FullName, false);
                return true;
            }
            fileindex = files.Select(fi => Path.GetFileName(fi.FullName)).ToList().IndexOf("hc.dll");
            if (fileindex > -1)
            {
                version = ScanInFile(files[fileindex].FullName, false);
                return true;
            }
            fileindex = files.Select(fi => Path.GetFileName(fi.FullName)).ToList().IndexOf("solidshield-cd.dll");
            if (fileindex > -1)
            {
                version = ScanInFile(files[fileindex].FullName, false);
                return true;
            }
            fileindex = files.Select(fi => Path.GetFileName(fi.FullName)).ToList().IndexOf("c11prot.dll");
            if (fileindex > -1)
            {
                version = ScanInFile(files[fileindex].FullName, false);
                return true;
            }

            return false;
        }

        private bool Softlock(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("SOFTLOCKI.dat"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("SOFTLOCKC.dat"))
                return true;
            return false;
        }

        private bool StarForce(out string version, FileInfo[] files)
        {
            version = "";
            int fileindex = files.Select(fi => Path.GetFileName(fi.FullName)).ToList().IndexOf("protect.dll");
            if (fileindex > -1)
            {
                version = ScanInFile(files[fileindex].FullName, false);
                return true;
            }
            fileindex = files.Select(fi => Path.GetFileName(fi.FullName)).ToList().IndexOf("protect.exe");
            if (fileindex > -1)
            {
                version = ScanInFile(files[fileindex].FullName, false);
                return true;
            }

            return false;
        }

        // TODO: Properly fill out
        private bool Steam(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("SteamInstall.exe"))
                return true;
            return false;
        }

        private bool Tages(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("Tages.dll"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("tagesclient.exe"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("TagesSetup.exe"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("TagesSetup_x64.exe"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("Wave.aif"))
                return true;
            return false;
        }

        private bool TZCopyProtector(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("_742893.016"))
                return true;
            return false;
        }

        // TODO: Properly fill out
        private bool UPlay(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("UPlayInstall.exe"))
                return true;
            return false;
        }

        private bool VOBProtectCDDVD(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("VOB-PCD.KEY"))
                return true;
            return false;
        }

        private bool WinLock(FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("WinLock.PSX"))
                return true;
            return false;
        }

        private bool WTMCDProtect(string path)
        {
            if (Directory.GetFiles(path, "*.IMP", SearchOption.AllDirectories).Length > 0)
                return true;
            return false;
        }

        private bool WTMCopyProtection(out string version, FileInfo[] files)
        {
            version = "";
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("imp.dat")
                || files.Select(fi => Path.GetFileName(fi.FullName)).Contains("wtmfiles.dat"))
            {
                if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("Viewer.exe"))
                {
                    int index = files.Select(fi => Path.GetFileName(fi.FullName)).ToList().IndexOf("Viewer.exe");
                    version = GetFileVersion(files[index].FullName);
                }
            }

            return false;
        }

        private bool XCP(string path, FileInfo[] files)
        {
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("XCP.DAT"))
                return true;
            if (files.Select(fi => Path.GetFileName(fi.FullName)).Contains("ECDPlayerControl.ocx"))
                return true;
            if (File.Exists(Path.Combine(path, "contents", "go.exe")))
                return true;
            return false;
        }

        private bool DummyFiles(FileInfo[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    if (files[i].Length > 681574400) // 681574400 Bytes = 650 Mb
                        return true;
                }
                catch { }
            }

            return false;
        }

        #endregion

        #region Version detections

        private string GetCDDVDCopsVersion(string file, int position)
        {
            char[] version;
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream);
            br.BaseStream.Seek(position + 15, SeekOrigin.Begin); // Begin reading after "CD-Cops,  ver."
            version = br.ReadChars(4);
            if (version[0] == 0x0)
                return "";
            return new string(version);
        }
        
        private string GetJoWooDXProt1Version(string file, int position)
        {
            char[] version = new char[5];
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream);
            br.BaseStream.Seek(position + 67, SeekOrigin.Begin);
            version[0] = br.ReadChar();
            br.ReadByte();
            version[1] = br.ReadChar();
            br.ReadByte();
            version[2] = br.ReadChar();
            br.ReadByte();
            version[3] = br.ReadChar();
            version[4] = br.ReadChar();
            br.Close();

            return version[0] + "." + version[1] + "." + version[2] + "." + version[3] + version[4];
        }

        private string GetLaserLockBuild(string FileContent, bool Version2)
        {
            int position = FileContent.IndexOf("Unkown" + (char)0 + "Unkown");
            string Year;
            string Month;
            string Day;
            if (Version2)
            {
                Day = FileContent.Substring(position + 14, 2);
                Month = FileContent.Substring(position + 14 + 3, 2);
                Year = "20" + FileContent.Substring(position + 14 + 6, 2);
            }
            else
            {
                Day = FileContent.Substring(position + 13, 2);
                Month = FileContent.Substring(position + 13 + 3, 2);
                Year = "20" + FileContent.Substring(position + 13 + 6, 2);
            }

            return "(Build " + Year + "-" + Month + "-" + Day + ")";
        }

        private string GetLaserLockVersion(string FileContent, int position)
        {
            return FileContent.Substring(position + 76, 4);
        }

        private string GetLaserLockVersion16Bit(string file)
        {
            char[] version = new char[3];
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream);
            br.BaseStream.Seek(71, SeekOrigin.Begin);
            version[0] = br.ReadChar();
            br.ReadByte();
            version[1] = br.ReadChar();
            version[2] = br.ReadChar();
            br.Close();

            if (Char.IsNumber(version[0]) && Char.IsNumber(version[1]) && Char.IsNumber(version[2]))
                return version[0] + "." + version[1] + version[2];
            return "";
        }

        private string GetProtectCDoldVersion(string file, int position)
        {
            char[] version = new char[3];
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream);
            br.BaseStream.Seek(position + 16, SeekOrigin.Begin); // Begin reading after "VOB ProtectCD"
            version[0] = br.ReadChar();
            br.ReadByte();
            version[1] = br.ReadChar();
            version[2] = br.ReadChar();
            br.Close();
            if (Char.IsNumber(version[0]) && Char.IsNumber(version[1]) && Char.IsNumber(version[2]))
                return version[0] + "." + version[1] + version[2];
            return "old";
        }

        private string GetProtectDiscVersionBuild6till8(string file, int position)
        {
            string version;
            string strBuild;
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream);

            br.BaseStream.Seek(position - 12, SeekOrigin.Begin);
            if (br.ReadByte() == 0xA && br.ReadByte() == 0xD && br.ReadByte() == 0xA && br.ReadByte() == 0xD) // ProtectDisc 6-7 with Build Number in plain text
            {
                br.BaseStream.Seek(position - 12 - 6, SeekOrigin.Begin);
                if (new string(br.ReadChars(6)) == "Henrik") // ProtectDisc 7
                {
                    version = "7.1-7.5";
                    br.BaseStream.Seek(position - 12 - 6 - 6, SeekOrigin.Begin);
                }
                else // ProtectDisc 6
                {
                    version = "6";
                    br.BaseStream.Seek(position - 12 - 10, SeekOrigin.Begin);
                    while (true) //search for e.g. "Build 050913 -  September 2005"
                    {
                        if (Char.IsNumber(br.ReadChar()))
                            break;
                        br.BaseStream.Seek(-2, SeekOrigin.Current); //search upwards
                    }

                    br.BaseStream.Seek(-5, SeekOrigin.Current);
                }
            }
            else
            {
                br.BaseStream.Seek(position + 28, SeekOrigin.Begin);
                if (br.ReadByte() == 0xFB)
                {
                    br.Close();
                    return "7.6-7.x";
                }
                else
                {
                    br.Close();
                    return "8.0";
                }
            }
            strBuild = "" + br.ReadChar() + br.ReadChar() + br.ReadChar() + br.ReadChar() + br.ReadChar();
            br.Close();
            return version + " (Build " + strBuild + ")";
        }

        private string GetProtectDiscVersionBuild76till10(string file, int position, out int irefBuild)
        {
            irefBuild = 0;
            byte version, subversion, versionindicatorPD9, subsubversionPD9x, subversionPD9x1, subversionPD9x2;
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream);
            br.BaseStream.Seek(position + 37, SeekOrigin.Begin);
            subversion = br.ReadByte();
            br.ReadByte();
            version = br.ReadByte();
            br.BaseStream.Seek(position + 49, SeekOrigin.Begin);
            irefBuild = br.ReadInt32();
            br.BaseStream.Seek(position + 53, SeekOrigin.Begin);
            versionindicatorPD9 = br.ReadByte();
            br.BaseStream.Seek(position + 0x40, SeekOrigin.Begin);
            subsubversionPD9x = br.ReadByte();
            subversionPD9x2 = br.ReadByte();
            subversionPD9x1 = br.ReadByte();
            br.Close();

            // version 7
            if (version == 0xAC)
                return "7." + (subversion ^ 0x43) + " (Build " + irefBuild + ")";
            // version 8
            else if (version == 0xA2)
            {
                if (subversion == 0x46)
                {
                    if ((irefBuild & 0x3A00) == 0x3A00)
                        return "8.2" + " (Build " + irefBuild + ")";
                    else
                        return "8.1" + " (Build " + irefBuild + ")";
                }
                return "8." + (subversion ^ 0x47) + " (Build " + irefBuild + ")";
            }
            // version 9
            else if (version == 0xA3)
            {
                // version removed or not given
                if ((subversionPD9x2 == 0x5F && subversionPD9x1 == 0x61) || (subversionPD9x1 == 0 && subversionPD9x2 == 0))
                {
                    if (versionindicatorPD9 == 0xB)
                        return "9.0-9.4" + " (Build " + irefBuild + ")";
                    else if (versionindicatorPD9 == 0xC)
                    {
                        if (subversionPD9x2 == 0x5F && subversionPD9x1 == 0x61)
                            return "9.5-9.11" + " (Build " + irefBuild + ")";
                        else if (subversionPD9x1 == 0 && subversionPD9x2 == 0)
                            return "9.11-9.20" + " (Build " + irefBuild + ")";
                    }
                    else
                        return "9." + subversionPD9x1 + subversionPD9x2 + "." + subsubversionPD9x + " (Build " + irefBuild + ")";
                }
            }
            else if (version == 0xA0)
            {
                // version removed
                if (subversionPD9x1 != 0 || subversionPD9x2 != 0)
                    return "10." + subversionPD9x1 + "." + subsubversionPD9x + " (Build " + irefBuild + ")";
                else
                    return "10.x (Build " + irefBuild + ")";
            }
            else
                return "7.6-10.x (Build " + irefBuild + ")";

            return "";
        }

        private string GetSafeDiscVersion(string file, int position)
        {
            int version;
            int subVersion;
            int subsubVersion;
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream);
            br.BaseStream.Seek(position + 20, SeekOrigin.Begin); // Begin reading after "BoG_ *90.0&!!  Yy>" for old SafeDisc
            version = br.ReadInt32();
            subVersion = br.ReadInt32();
            subsubVersion = br.ReadInt32();
            if (version != 0)
                return version + "." + subVersion.ToString("00") + "." + subsubVersion.ToString("000");
            br.BaseStream.Seek(position + 18 + 14, SeekOrigin.Begin); // Begin reading after "BoG_ *90.0&!!  Yy>" for newer SafeDisc
            version = br.ReadInt32();
            subVersion = br.ReadInt32();
            subsubVersion = br.ReadInt32();
            br.Close();
            if (version == 0)
                return "";
            return version + "." + subVersion.ToString("00") + "." + subsubVersion.ToString("000");
        }

        private string GetSecuROM4Version(string file, int position)
        {
            char version;
            char subVersion1;
            char subVersion2;
            char subsubVersion1;
            char subsubVersion2;
            char subsubsubVersion1;
            char subsubsubVersion2;
            char subsubsubVersion3;
            char subsubsubVersion4;
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream, Encoding.Default);
            br.BaseStream.Seek(position + 8, SeekOrigin.Begin); // Begin reading after "AddD"
            version = br.ReadChar();
            br.ReadByte();
            subVersion1 = br.ReadChar();
            subVersion2 = br.ReadChar();
            br.ReadByte();
            subsubVersion1 = br.ReadChar();
            subsubVersion2 = br.ReadChar();
            br.ReadByte();
            subsubsubVersion1 = br.ReadChar();
            subsubsubVersion2 = br.ReadChar();
            subsubsubVersion3 = br.ReadChar();
            subsubsubVersion4 = br.ReadChar();
            br.Close();
            return version + "." + subVersion1 + subVersion2 + "." + subsubVersion1 + subsubVersion2 + "." + subsubsubVersion1 + subsubsubVersion2 + subsubsubVersion3 + subsubsubVersion4;
        }

        private string GetSecuROM4and5Version(string file, int position)
        {
            byte version;
            byte subVersion1;
            byte subVersion2;
            byte subsubVersion1;
            byte subsubVersion2;
            byte subsubsubVersion1;
            byte subsubsubVersion2;
            byte subsubsubVersion3;
            byte subsubsubVersion4;
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream);
            br.BaseStream.Seek(position + 8, SeekOrigin.Begin); // Begin reading after "ÊÝÝ¬"
            version = (byte)(br.ReadByte() & 0xF);
            br.ReadByte();
            subVersion1 = (byte)(br.ReadByte() ^ 36);
            subVersion2 = (byte)(br.ReadByte() ^ 28);
            br.ReadByte();
            subsubVersion1 = (byte)(br.ReadByte() ^ 42);
            subsubVersion2 = (byte)(br.ReadByte() ^ 8);
            br.ReadByte();
            subsubsubVersion1 = (byte)(br.ReadByte() ^ 16);
            subsubsubVersion2 = (byte)(br.ReadByte() ^ 116);
            subsubsubVersion3 = (byte)(br.ReadByte() ^ 34);
            subsubsubVersion4 = (byte)(br.ReadByte() ^ 22);
            br.Close();
            if (version == 0 || version > 9)
                return "";
            return version + "." + subVersion1 + subVersion2 + "." + subsubVersion1 + subsubVersion2 + "." + subsubsubVersion1 + subsubsubVersion2 + subsubsubVersion3 + subsubsubVersion4;
        }

        private string GetSecuROM7Version(string file)
        {
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream);
            br.BaseStream.Seek(236, SeekOrigin.Begin);
            byte[] bytes = br.ReadBytes(4);
            // if bytes(0) = 0xED && bytes(3) = 0x5C {
            if (bytes[3] == 0x5C)
            {
                //SecuROM 7 new and 8
                br.Close();
                return (bytes[0] ^ 0xEA).ToString() + "." + (bytes[1] ^ 0x2C).ToString("00") + "." + (bytes[2] ^ 0x8).ToString("0000");
            }
            else // SecuROM 7 old
            {
                br.BaseStream.Seek(122, SeekOrigin.Begin);
                bytes = br.ReadBytes(2);
                br.Close();
                return "7." + (bytes[0] ^ 0x10).ToString("00") + "." + (bytes[1] ^ 0x10).ToString("0000");
                //return "7.01-7.10"
            }
        }

        private string GetSysiphusVersion(string file, int position)
        {
            char version;
            char subVersion;
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream, Encoding.Default);
            br.BaseStream.Seek(position - 3, SeekOrigin.Begin);
            subVersion = br.ReadChar();
            br.ReadChar();
            version = br.ReadChar();
            br.Close();
            if (Char.IsNumber(version) && Char.IsNumber(subVersion))
                return version + "." + subVersion;
            else
                return "";
        }

        private string GetTagesVersion(string file, int position)
        {
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream);
            br.BaseStream.Seek(position + 7, SeekOrigin.Begin);
            byte bVersion = br.ReadByte();
            br.Close();
            switch(bVersion)
            {
                case 0x1B:
                    return "5.3-5.4";
                case 0x14:
                    return "5.5.0";
                case 0x4:
                    return "5.5.2";
            }
            return "";
        }

        private string GetVOBProtectCDDVDBuild(string file, int position)
        {
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream);
            br.BaseStream.Seek(position - 13, SeekOrigin.Begin);
            if (!Char.IsNumber(br.ReadChar()))
                return ""; //Build info removed
            br.BaseStream.Seek(position - 4, SeekOrigin.Begin);
            int build = br.ReadInt16();
            br.Close();
            return " (Build " + build + ")";
        }

        private string GetVOBProtectCDDVDVersion(string file, int position)
        {
            byte version;
            byte subVersion;
            byte subsubVersion;
            BinaryReader br = new BinaryReader(new StreamReader(file).BaseStream, Encoding.Default);
            br.BaseStream.Seek(position - 2, SeekOrigin.Begin);
            version = br.ReadByte();
            if (version == 5)
            {
                br.BaseStream.Seek(position - 4, SeekOrigin.Begin);
                subsubVersion = (byte)((br.ReadByte() & 0xF0) >> 4);
                subVersion = (byte)((br.ReadByte() & 0xF0) >> 4);
                br.Close();
                return version + "." + subVersion + "." + subsubVersion;
            }
            else
                return "";
        }

#endregion

        private string GetFileVersion(string file)
        {
            FileVersionInfo fvinfo = FileVersionInfo.GetVersionInfo(file);
            if (fvinfo.FileVersion == null)
                return "";
            if (fvinfo.FileVersion != "")
                return fvinfo.FileVersion.Replace(", ", ".");
            else
                return fvinfo.ProductVersion.Replace(", ", ".");
        }

        private int SuffixInStr(string String1, string String2, string Suffix1, string Suffix2)
        {
            int rtn = 1;
            while (rtn > 0)
            {
                rtn = String1.IndexOf(String2, rtn + 1);
                if (rtn > -1)
                {
                    if (String1.Substring(rtn - 1 + String2.Length, Suffix1.Length) == Suffix1
                        || String1.Substring(rtn - 1 + String2.Length, Suffix2.Length) == Suffix2)
                        return rtn;
                }
            }

            return -1;
        }
    }
}