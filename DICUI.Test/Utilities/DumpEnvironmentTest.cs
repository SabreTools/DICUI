﻿using System.IO;
using DICUI.Data;
using DICUI.Utilities;
using Xunit;

namespace DICUI.Test
{
    public class DumpEnvironmentTest
    {
        [Theory]
        [InlineData(null, 'D', false, MediaType.NONE, false)]
        [InlineData("", 'D', false, MediaType.NONE, false)]
        [InlineData("cd F test.bin 8 /c2 20", 'F', false, MediaType.CDROM, true)]
        [InlineData("fd A test.img", 'A', true, MediaType.FloppyDisk, true)]
        [InlineData("dvd X test.iso 8 /raw", 'X', false, MediaType.FloppyDisk, false)]
        [InlineData("stop D", 'D', false, MediaType.DVD, true)]
        public void ParametersValidTest(string parameters, char letter, bool isFloppy, MediaType? mediaType, bool expected)
        {
            var env = new DumpEnvironment
            {
                Parameters = new DiscImageCreator.Parameters(parameters),
                Drive = isFloppy
                    ? new Drive(InternalDriveType.Floppy, new DriveInfo(letter.ToString()))
                    : new Drive(InternalDriveType.Optical, new DriveInfo(letter.ToString())),
                Type = mediaType,
            };

            bool actual = env.ParametersValid();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null, null, null)]
        [InlineData(" ", "", " ", "")]
        [InlineData("super", "blah.bin", "super", "blah.bin")]
        [InlineData("super\\hero", "blah.bin", "super\\hero", "blah.bin")]
        [InlineData("super.hero", "blah.bin", "super.hero", "blah.bin")]
        [InlineData("superhero", "blah.rev.bin", "superhero", "blah.rev.bin")]
        [InlineData("super&hero", "blah.bin", "super&hero", "blah.bin")]
        [InlineData("superhero", "blah&foo.bin", "superhero", "blah&foo.bin")]
        public void FixOutputPathsTest(string outputDirectory, string outputFilename, string expectedOutputDirectory, string expectedOutputFilename)
        {
            var env = new DumpEnvironment
            {
                OutputDirectory = outputDirectory,
                OutputFilename = outputFilename,
            };

            env.FixOutputPaths();
            Assert.Equal(expectedOutputDirectory, env.OutputDirectory);
            Assert.Equal(expectedOutputFilename, env.OutputFilename);
        }

        [Fact]
        public void GetFirstTrackTest()
        {
            // TODO: Implement
            Assert.True(true);
        }

        [Fact]
        public void FormatOutputDataTest()
        {
            // TODO: Implement
            Assert.True(true);
        }

        [Fact]
        public void WriteOutputDataTest()
        {
            // TODO: Implement
            Assert.True(true);
        }

        [Fact]
        public void EjectDiscTest()
        {
            // TODO: Implement
            Assert.True(true);
        }

        [Fact]
        public void CancelDumpingTest()
        {
            // TODO: Implement
            Assert.True(true);
        }

        [Fact]
        public void StartDumpingTest()
        {
            // TODO: Implement
            Assert.True(true);
        }
    }
}
