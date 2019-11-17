### 1.15 (2019-11-16)

- Updated to DIC version 20191116
- Support non-optical, non-floppy stuff better
- Update protection scanning and fix some things

### 1.14 (2019-10-01)

- Updated to DIC version 20191001
- Added builds for .NET 4.6.2, .NET 4.7.2, and .NET Core 3.0
- Updated and fixed a bunch of things related to Redump
- Fixed path persistence when changing system and media type
- Added more system autodetects
- Added new, optional, disc information filling window (WIP)

### 1.13 (2019-07-02)

- Added new DIC commands and flags
- Made DICUI Check more robust
- Added and updated systems along with format cleanup
- Created a new SubmissionInfo template (and internals)
- Added automatic grabbing of Redump information, if possible
- Better media format support (BD, UMD)
- Initial disc type detection

### 1.12.2 (2019-04-19)

- Added DICUI Check, a new standalone tool for parsing DIC output from platforms unsupported by DICUI
- Added a few machines/formats
- Updated to DIC version 20190326
- Added DMI data extraction for Xbox and X360

### 1.12.1 (2019-01-28)

- Fixed !submissionInfo.txt output for CD-ROM and GD-ROM

### 1.12 (2019-01-27)

- Added a few new systems and formats
- Added new DIC commands and flags
- Updated the `!submissionInfo.txt` file order
- Fixed Audio CD handling
- Added Sega CD / Mega CD header extraction
- Readded Floppy Disk as a supported format
- And more! See the full Git commit list for more details

### 1.11 (2018-09-20)

- Fix formatting of XBOX and XBOX 360 security sector output
- Add new XBOX swap commands and outputs
- Fixes for some PlayStation 2 and 4 outputs
- Added external programs to AppVeyor builds
- Fixed `.` in path issues with DIC; attempted to fix issues with `&`
- Combined XBOX 360 XGD 2/3 due to new DIC support with Kreon drives
- Fixed (semi-)longstanding bug with XBOX disc layer detection
- Added DVD-Video protection output
- Made custom parameters work a little more intuitively
- Added *EXPERIMENTAL* Winforms-based UI
- And more! See the full Git commit list for more details

### 1.10 (2018-07-29)

- Added many new options for user customization
- Added unit testing and an AppVeyor build
- Many code refactorings
- **LOG WINDOW**
- Separated out protection scan and Unshield ports to new projects
- Added "empty drive" support; should help with 3DO and HFS dumping
- And much more! See the full Git commit list for more details

### 1.07 (2018-06-27)

- Separated system and media type for easier navigation
- Combined instances of single- and dual-layer discs
- Removed reliance on **sg-raw** and **psxt001z**
- Added system and disc type to the submission info
- First attempt at getting current disc type
- Made the three PSX-specific fields (**EDC**, **Anti-modchip**, and **LibCrypt**) automatically filled in, when possible
- Many, many, many behind the scenes updates for speed, future features, and stability

### 1.06 (2018-06-15)

- Fixed not being able to use the `/c2` flag properly
- Fixed times when the ability to start dumping was improperly allowed
- Added full support for XBOX and XBOX360 (XDG1, XDG2) dumping through DIC (using a Kreon, or presumably a 0800)

### 1.05a (2018-06-14)

- Fixed some ordering and nullability issues
- Added automatic fields for PS1, PS2, Saturn

### 1.05 (2018-06-14)

- Miscellaneous fixes around custom parameter validation, dump information accuracy, settings window, and TODO cleanup
- Add many more supported platforms, mostly arcade (based on publicly available information)
- Add floppy disk dumping support
- Add optional disc eject on completion
- Add subdump for Sega Saturn
- Fully support newest version of DIC including all new flags and commands
- PlayStation and Saturn discs still don't have all internal information automatically generated

### 1.04b (2018-06-13)

- Added subIntention reading
- Fixed extra extensions being appended
- Fixed internationalization error (number formatting)
- Fixed "Custom Input" not working

### 1.04a (2018-06-13)

- Fixed issue with empty trays
- Added settings dialog

### 1.04 (2018-06-13)

- Behind-the-scenes fixes and formatting
- Better checks for external programs
- Automatically changing disc information
- Custom parameters (and parameter validation)
- Automatic drive speed selection
- Automatic submission information creation
- Add ability to stop a dump from the UI

### 1.03 (2018-06-08)

- edccchk now run on all CD-Roms
- Discs unsupported by Windows are now regonized
- Extra \ when accepting default save has been removed.
 
### 1.02b (2018-05-18)

- Added missing DLL

### 1.02 (2018-05-18)

- Fixed XBOX One and PS4 Drive Speed issue.
- Started implementing DiscImageCreator Path selection.
- Conforming my naming for objects and variable

### 1.01d (2018-05-18)

-Combine IBM PC-CD options, misc fixes.