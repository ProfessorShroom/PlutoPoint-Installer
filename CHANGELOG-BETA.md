# Beta Changelog

Release notes for beta versions. Full releases are in [CHANGELOG.md](CHANGELOG.md).

## 9.0.0.0b1 - The .NET 10 Update

### Added
- AMD Chipset Drivers, AMD Software: Adrenalin Edition and Nvidia App are now skipped if they're already installed.
- Installers that get stuck are now stopped after 10 minutes, or 20 minutes for BitDefender, Microsoft Office 2024 and AMD Software: Adrenalin Edition.
- Downloads that receive nothing for 60 seconds now fail and retry instead of waiting forever.
- Cancel now stops downloads straight away. While an app is installing it becomes Force Stop, which ends that installer.
- Beta builds now have a β badge.

### Changed
- Updated to .NET 10 and Avalonia 12.2.0.
- Beta builds no longer use orange, and now show holidays and birthdays.
- Long titles now shrink to fit instead of running into the OEM dropdown.
- Windows settings are now applied separately, so one failing no longer skips the rest.
- Closing or restarting during an install now stops it, puts the sleep settings back and deletes the downloaded files first.
- The launcher now carries on to the installer if it can't check for or download its own update.
- The launcher's backup download link now uses HTTPS.
- Split the main window's code into smaller files, and added tests.
- The changelog in the README is now made from CHANGELOG.md automatically.
- The launcher now gets its version from its release tag.
- Download links are now checked every week.

### Fixed
- Fixed the window freezing during winget checks.
- Fixed Empty Recycle Bin always saying it worked.
- Fixed Close cancelling shutdowns the installer didn't schedule.
- Fixed the year range in the copyright workflow's commit message.
