﻿using DICUI.Data;

namespace DICUI.Utilities
{
    public static class Extensions
    {
        #region Redump Information Lists

        /// <summary>
        /// List of systems that are not publically accessible
        /// </summary>
        public static readonly RedumpSystem[] BannedSystems = new RedumpSystem[]
        {
            RedumpSystem.AudioCD,
            RedumpSystem.BDVideo,
            RedumpSystem.DVDVideo,
            RedumpSystem.HasbroVideoNow,
            RedumpSystem.HasbroVideoNowColor,
            RedumpSystem.HasbroVideoNowJr,
            RedumpSystem.HasbroVideoNowXP,
            RedumpSystem.KonamiM2,
            RedumpSystem.MicrosoftXbox360,
            RedumpSystem.MicrosoftXboxOne,
            RedumpSystem.NavisoftNaviken21,
            RedumpSystem.NintendoWii,
            RedumpSystem.NintendoWiiU,
            RedumpSystem.PanasonicM2,
            RedumpSystem.PhilipsCDiDigitalVideo,
            RedumpSystem.SegaRingEdge,
            RedumpSystem.SegaRingEdge2,
            RedumpSystem.SonyPlayStation3,
            RedumpSystem.SonyPlayStation4,
            RedumpSystem.VideoCD,
        };

        /// <summary>
        /// List of systems that have a Cues pack
        /// </summary>
        public static readonly RedumpSystem[] HasCues = new RedumpSystem[]
        {
            RedumpSystem.AppleMacintosh,
            RedumpSystem.AudioCD,
            RedumpSystem.BandaiPippin,
            RedumpSystem.BandaiPlaydiaQuickInteractiveSystem,
            RedumpSystem.CommodoreAmigaCD,
            RedumpSystem.CommodoreAmigaCD32,
            RedumpSystem.CommodoreAmigaCDTV,
            RedumpSystem.FujitsuFMTownsseries,
            RedumpSystem.HasbroVideoNow,
            RedumpSystem.HasbroVideoNowJr,
            RedumpSystem.IBMPCcompatible,
            RedumpSystem.IncredibleTechnologiesEagle,
            RedumpSystem.KonamieAmusement,
            RedumpSystem.KonamiFireBeat,
            RedumpSystem.KonamiM2,
            RedumpSystem.KonamiSystemGV,
            RedumpSystem.MattelHyperScan,
            RedumpSystem.MicrosoftXbox,
            RedumpSystem.MicrosoftXbox360,
            RedumpSystem.NamcoSystem246,
            RedumpSystem.NavisoftNaviken21,
            RedumpSystem.NECPC88series,
            RedumpSystem.NECPC98series,
            RedumpSystem.NECPCEngineCDTurboGrafxCD,
            RedumpSystem.NECPCFXPCFXGA,
            RedumpSystem.PalmOS,
            RedumpSystem.Panasonic3DOInteractiveMultiplayer,
            RedumpSystem.PanasonicM2,
            RedumpSystem.PhilipsCDi,
            RedumpSystem.PhilipsCDiDigitalVideo,
            RedumpSystem.PhotoCD,
            RedumpSystem.PlayStationGameSharkUpdates,
            RedumpSystem.SegaChihiro,
            RedumpSystem.SegaDreamcast,
            RedumpSystem.SegaMegaCDSegaCD,
            RedumpSystem.SegaNaomi,
            RedumpSystem.SegaNaomi2,
            RedumpSystem.SegaSaturn,
            RedumpSystem.SegaTriforce,
            RedumpSystem.SNKNeoGeoCD,
            RedumpSystem.SonyPlayStation,
            RedumpSystem.SonyPlayStation2,
            RedumpSystem.SonyPlayStation3,
            RedumpSystem.TABAustriaQuizard,
            RedumpSystem.TomyKissSite,
            RedumpSystem.VideoCD,
            RedumpSystem.VTechVFlashVSmilePro,
};

        /// <summary>
        /// List of systems that has a Dat pack
        /// </summary>
        public static readonly RedumpSystem[] HasDat = new RedumpSystem[]
        {
            RedumpSystem.MicrosoftXboxBIOS,
            RedumpSystem.NintendoGameCubeBIOS,
            RedumpSystem.SonyPlayStationBIOS,
            RedumpSystem.SonyPlayStation2BIOS,

            RedumpSystem.AppleMacintosh,
            RedumpSystem.AudioCD,
            RedumpSystem.BDVideo,
            RedumpSystem.BandaiPippin,
            RedumpSystem.BandaiPlaydiaQuickInteractiveSystem,
            RedumpSystem.CommodoreAmigaCD,
            RedumpSystem.CommodoreAmigaCD32,
            RedumpSystem.CommodoreAmigaCDTV,
            RedumpSystem.DVDVideo,
            RedumpSystem.FujitsuFMTownsseries,
            RedumpSystem.HasbroVideoNow,
            RedumpSystem.HasbroVideoNowJr,
            RedumpSystem.IBMPCcompatible,
            RedumpSystem.IncredibleTechnologiesEagle,
            RedumpSystem.KonamiFireBeat,
            RedumpSystem.KonamiM2,
            RedumpSystem.KonamiSystemGV,
            RedumpSystem.KonamieAmusement,
            RedumpSystem.MattelHyperScan,
            RedumpSystem.MicrosoftXbox,
            RedumpSystem.MicrosoftXbox360,
            RedumpSystem.MicrosoftXboxOne,
            RedumpSystem.NamcoSystem246,
            RedumpSystem.NavisoftNaviken21,
            RedumpSystem.NECPC88series,
            RedumpSystem.NECPC98series,
            RedumpSystem.NECPCEngineCDTurboGrafxCD,
            RedumpSystem.NECPCFXPCFXGA,
            RedumpSystem.NinendoGameCube,
            RedumpSystem.NintendoWii,
            RedumpSystem.NintendoWiiU,
            RedumpSystem.PalmOS,
            RedumpSystem.Panasonic3DOInteractiveMultiplayer,
            RedumpSystem.PanasonicM2,
            RedumpSystem.PhilipsCDi,
            RedumpSystem.PhilipsCDiDigitalVideo,
            RedumpSystem.PhotoCD,
            RedumpSystem.PlayStationGameSharkUpdates,
            RedumpSystem.SegaChihiro,
            RedumpSystem.SegaDreamcast,
            RedumpSystem.SegaLindbergh,
            RedumpSystem.SegaMegaCDSegaCD,
            RedumpSystem.SegaNaomi,
            RedumpSystem.SegaNaomi2,
            RedumpSystem.SegaRingEdge,
            RedumpSystem.SegaRingEdge2,
            RedumpSystem.SegaSaturn,
            RedumpSystem.SegaTriforce,
            RedumpSystem.SNKNeoGeoCD,
            RedumpSystem.SonyPlayStation,
            RedumpSystem.SonyPlayStation2,
            RedumpSystem.SonyPlayStation3,
            RedumpSystem.SonyPlayStation4,
            RedumpSystem.SonyPlayStationPortable,
            RedumpSystem.TABAustriaQuizard,
            RedumpSystem.TomyKissSite,
            RedumpSystem.VideoCD,
            RedumpSystem.VMLabsNUON,
            RedumpSystem.VTechVFlashVSmilePro,
            RedumpSystem.ZAPiTGamesGameWaveFamilyEntertainmentSystem,
        };

        /// <summary>
        /// List of systems that has a Decrypted Keys pack
        /// </summary>
        public static readonly RedumpSystem[] HasDkeys = new RedumpSystem[]
        {
            RedumpSystem.SonyPlayStation3,
        };

        /// <summary>
        /// List of systems that has a GDI pack
        /// </summary>
        public static readonly RedumpSystem[] HasGdi = new RedumpSystem[]
        {
            RedumpSystem.SegaChihiro,
            RedumpSystem.SegaDreamcast,
            RedumpSystem.SegaNaomi,
            RedumpSystem.SegaNaomi2,
            RedumpSystem.SegaTriforce,
        };

        /// <summary>
        /// List of systems that has a Keys pack
        /// </summary>
        public static readonly RedumpSystem[] HasKeys = new RedumpSystem[]
        {
            RedumpSystem.NintendoWiiU,
            RedumpSystem.SonyPlayStation3,
        };

        /// <summary>
        /// List of systems that has an LSD pack
        /// </summary>
        public static readonly RedumpSystem[] HasLsd = new RedumpSystem[]
        {
            RedumpSystem.IBMPCcompatible,
            RedumpSystem.SonyPlayStation,
        };

        /// <summary>
        /// List of systems that has an SBI pack
        /// </summary>
        public static readonly RedumpSystem[] HasSbi = new RedumpSystem[]
        {
            RedumpSystem.IBMPCcompatible,
            RedumpSystem.SonyPlayStation,
        };

        #endregion

        public static bool DoesSupportDriveSpeed(this MediaType? type)
        {
            switch (type)
            {
                case MediaType.CDROM:
                case MediaType.DVD:
                case MediaType.GDROM:
                case MediaType.HDDVD:
                case MediaType.BluRay:
                case MediaType.NintendoGameCubeGameDisc:
                case MediaType.NintendoWiiOpticalDisc:
                    return true;
                default:
                    return false;
            }
        }

        public static KnownSystemCategory Category(this KnownSystem? system)
        {
            if (system < KnownSystem.MarkerDiscBasedConsoleEnd)
                return KnownSystemCategory.DiscBasedConsole;
            /*
            else if (system < KnownSystem.MarkerOtherConsoleEnd)
                return KnownSystemCategory.OtherConsole;
            */
            else if (system < KnownSystem.MarkerComputerEnd)
                return KnownSystemCategory.Computer;
            else if (system < KnownSystem.MarkerArcadeEnd)
                return KnownSystemCategory.Arcade;
            else if (system < KnownSystem.MarkerOtherEnd)
                return KnownSystemCategory.Other;
            else
                return KnownSystemCategory.Custom;
        }

        public static bool IsMarker(this KnownSystem? system)
        {
            switch (system)
            {
                case KnownSystem.MarkerArcadeEnd:
                case KnownSystem.MarkerComputerEnd:
                case KnownSystem.MarkerDiscBasedConsoleEnd:
                // case KnownSystem.MarkerOtherConsoleEnd:
                case KnownSystem.MarkerOtherEnd:
                    return true;
                default:
                    return false;
            }
        }
    }
}
