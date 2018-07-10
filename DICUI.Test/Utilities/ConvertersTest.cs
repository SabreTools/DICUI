﻿using System;
using DICUI.Data;
using DICUI.Utilities;
using Xunit;

namespace DICUI.Test.Utilities
{
    public class ConvertersTest
    {
        [Theory]
        [InlineData(DICCommand.Audio, MediaType.CD)]
        [InlineData(DICCommand.BluRay, MediaType.BluRay)]
        [InlineData(DICCommand.Close, null)]
        [InlineData(DICCommand.CompactDisc, MediaType.CD)]
        [InlineData(DICCommand.Data, MediaType.CD)]
        [InlineData(DICCommand.DigitalVideoDisc, MediaType.DVD)]
        [InlineData(DICCommand.Eject, null)]
        [InlineData(DICCommand.Floppy, MediaType.Floppy)]
        [InlineData(DICCommand.GDROM, MediaType.GDROM)]
        [InlineData(DICCommand.MDS, null)]
        [InlineData(DICCommand.Reset, null)]
        [InlineData(DICCommand.Start, null)]
        [InlineData(DICCommand.Stop, null)]
        [InlineData(DICCommand.Sub, null)]
        [InlineData(DICCommand.Swap, MediaType.GDROM)]
        [InlineData(DICCommand.XBOX, MediaType.DVD)]
        public void BaseCommandToMediaTypeTest(DICCommand command, MediaType? expected)
        {
            MediaType? actual = Converters.BaseCommmandToMediaType(command);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(DICCommand.Audio, KnownSystem.AudioCD)]
        [InlineData(DICCommand.BluRay, KnownSystem.SonyPlayStation3)]
        [InlineData(DICCommand.Close, null)]
        [InlineData(DICCommand.CompactDisc, KnownSystem.IBMPCCompatible)]
        [InlineData(DICCommand.Data, KnownSystem.IBMPCCompatible)]
        [InlineData(DICCommand.DigitalVideoDisc, KnownSystem.IBMPCCompatible)]
        [InlineData(DICCommand.Eject, null)]
        [InlineData(DICCommand.Floppy, KnownSystem.IBMPCCompatible)]
        [InlineData(DICCommand.GDROM, KnownSystem.SegaDreamcast)]
        [InlineData(DICCommand.MDS, null)]
        [InlineData(DICCommand.Reset, null)]
        [InlineData(DICCommand.Start, null)]
        [InlineData(DICCommand.Stop, null)]
        [InlineData(DICCommand.Sub, null)]
        [InlineData(DICCommand.Swap, KnownSystem.SegaDreamcast)]
        [InlineData(DICCommand.XBOX, KnownSystem.MicrosoftXBOX)]
        public void BaseCommandToKnownSystemTest(DICCommand command, KnownSystem? expected)
        {
            KnownSystem? actual = Converters.BaseCommandToKnownSystem(command);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(MediaType.CD, ".bin")]
        [InlineData(MediaType.DVD, ".iso")]
        [InlineData(MediaType.LaserDisc, ".raw")]
        [InlineData(MediaType.WiiUOpticalDisc, ".wud")]
        [InlineData(MediaType.Floppy, ".img")]
        [InlineData(MediaType.Cassette, ".wav")]
        [InlineData(MediaType.NONE, null)]
        public void MediaTypeToExtensionTest(MediaType? mediaType, string expected)
        {
            string actual = Converters.MediaTypeToExtension(mediaType);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(MediaType.CD, "CD-ROM")]
        [InlineData(MediaType.LaserDisc, "LaserDisc")]
        [InlineData(MediaType.NONE, "Unknown")]
        public void MediaTypeToStringTest(MediaType? mediaType, string expected)
        {
            string actual = Converters.MediaTypeToString(mediaType);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(KnownSystem.MicrosoftXBOX, "Microsoft XBOX")]
        [InlineData(KnownSystem.NECPC88, "NEC PC-88")]
        [InlineData(KnownSystem.KonamiPython, "Konami Python")]
        [InlineData(KnownSystem.HDDVDVideo, "HD-DVD-Video")]
        [InlineData(KnownSystem.Custom, "Custom Input")]
        [InlineData(KnownSystem.NONE, "Unknown")]
        public void KnownSystemToStringTest(KnownSystem? knownSystem, string expected)
        {
            string actual = Converters.KnownSystemToString(knownSystem);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void KnownSystemHasValidCategory()
        {
            var values = Validators.CreateListOfSystems();
            KnownSystem[] markers = { KnownSystem.MarkerArcadeEnd, KnownSystem.MarkerConsoleEnd, KnownSystem.MarkerComputerEnd, KnownSystem.MarkerOtherEnd };

            values.ForEach(system => {
                if (system == KnownSystem.NONE || system == KnownSystem.Custom)
                    return;

                // we check that the category is the first category value higher than the system
                KnownSystemCategory category = ((KnownSystem?)system).Category();
                KnownSystem marker = KnownSystem.NONE;

                switch (category)
                {
                    case KnownSystemCategory.Arcade: marker = KnownSystem.MarkerArcadeEnd; break;
                    case KnownSystemCategory.Console: marker = KnownSystem.MarkerConsoleEnd; break;
                    case KnownSystemCategory.Computer: marker = KnownSystem.MarkerComputerEnd; break;
                    case KnownSystemCategory.Other: marker = KnownSystem.MarkerOtherEnd; break;
                }

                Assert.NotEqual(KnownSystem.NONE, marker);
                Assert.True(marker > system);

                Array.ForEach(markers, mmarker =>
                {
                    // a marker can be the same of the found one, or one of a category before or a category after but never in the middle between
                    // the system and the mapped category
                    Assert.True(mmarker == marker || mmarker < system || mmarker > marker);
                });
            });
        }
    }
}
