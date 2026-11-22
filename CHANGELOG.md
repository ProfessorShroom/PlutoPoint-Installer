# Changelog

## 8.1.1.1

### Changed
- The progress bar is now purple instead of see-through black, and matches the colours of each holiday and birthday.
- The shine on the progress bar now goes all the way across.

## 8.1.1.0

### Changed
- Made the "Computer Repair Centre Installer" title larger.

## 8.1.0.0

### Changed
- "Computer Repair Centre" in the title now uses the Neo Tech font, the same as the Computer Repair Centre website.
- "Computer Repair Centre" in the title now has a colour gradient, it changes colour for each holiday and birthday.
- The rest of the installer now uses the Ubuntu font, including the install log, which used Consolas.
- The Ubuntu and Neo Tech fonts are now built into the installer, so they show even if the computer doesn't have them installed.
- Redesigned the Software and Utilities boxes to match the install log, with see-through rounded backgrounds instead of outlines and the headings centred inside. The tops of all three boxes now line up.
- The Software and Utilities headings are now white, and change colour with holidays and birthdays like the title does.

## 8.0.0.0 - The Reliability Update

### Changed
- The OS label is no longer a few pixels above Current location due to the emjoi, as they are now one text block.
- Rebuilt how apps are installed. Every app now goes through the same steps, so they all behave the same way.
- After installing, each app is checked to make sure it's actually there. If the installer said it worked but the app can't be found, it's shown as "Unverified" in the summary.
- Already installed apps are now also found from Windows' installed programs list, so apps installed somewhere else (like Steam on another drive) aren't reinstalled.
- NanaZip is now detected from Windows' app list instead of searching the WindowsApps folder.
- A failed download no longer crashes the installer. That app is marked as failed and the rest carry on.
- If the latest LibreOffice version can't be found, it now uses the backup download instead of stopping the whole install.
- The LibreOffice backup link now comes from Downloads.json (libreoffice.professorshroom.com) instead of an old hard-coded link.
- The installer now waits for the download links to load, instead of crashing if Install was clicked too quickly.
- The Install button is greyed out while installing, so it can't be run twice at once.
- While installing, the Install button becomes a Cancel button. Cancelling stops the install after the current step, marks the remaining apps as skipped and still puts the sleep settings back.
- Google Chrome's install and the LibreOffice version check no longer freeze the window.
- If the OEM logo can't be downloaded, the shop details are still set.
- Moving the taskbar no longer crashes if its registry key is missing.
- AMD options on ARM devices now show as skipped instead of failed.
- The Nvidia App download now retries if it fails.
- VLC Media Player is now counted in the progress bar, and the progress bar starts from zero if Install is run again.
- Downloads now use HttpClient instead of the old WebClient.
- Updated Avalonia to 11.3.23.
- The standard and beta launchers are now built from one project.
- update.xml and updateBeta.xml are now updated automatically when a new launcher is released.
- The installer release workflow now uses setup-dotnet v5.

### Fixed
- Fixed sleep not actually being turned off during the install. The original sleep and screen timeout settings are now saved and put back afterwards, even if the install stops early.
- Fixed the power button setting not taking effect.
- Fixed the password not being asked for if the location check failed or hadn't finished yet.
- Fixed the location not showing in the log.
- Fixed Google Chrome sometimes being marked as both failed and installed.

## 7.1.0.2

### Changed
- Now deletes the beta launcher too.

## 7.1.0.1

### Changed
- Organised all software alphabetically.

## 7.1.0.0

### Changed
- Beta launcher now picks the newest installer from GitHub, whether that's a beta or a full release.
- If a full release has the same or a higher version than the latest beta, the beta launcher downloads the release instead, so it can be used as a main launcher.
- Both launchers shows which version they're downloading.
- If GitHub can't be reached, the beta launcher falls back to the latest release instead of failing.
- Beta launcher no longer uses the crcinstallerbeta.professorshroom.com link.
- Betas are now tagged like v7.1.0.0b1 and published as GitHub pre-releases, so the standard launcher never downloads them.
- Launcher releases and the installer-latest release no longer take GitHub's "Latest" label from the current installer release.

### Fixed
- Fixed the release badge in the README picking up launcher releases.

## 7.0.8.3

### Changed
- Expanded the Window as now there's two extra icons it needs more space.

## 7.0.8.2

### Fixed
- Fixed crash caused by .ico files instead of .png.

## 7.0.8.1

### Added
- Added a crash handler.

## 7.0.8.0

### Added
- Added AMD Chipset Drivers as an install option, ticked automatically when an AMD CPU is detected.
- Added AMD Software: Adrenalin Edition as an install option, ticked automatically when an AMD GPU is detected (including Radeon graphics built into Ryzen CPUs).

### Changed
- AMD Chipset Drivers download the latest version from AMD, with backup links if it can't be found, and install silently.
- AMD Software: Adrenalin Edition uses AMD's auto-detect installer, so it picks the right driver for the graphics card. It's skipped if the download link can't be found.
- AMD downloads now send the right headers so AMD's site allows them, and anything that isn't a real installer is rejected.
- AMD options are skipped on ARM devices.
- AMD GPU detection now only matches AMD graphics hardware, not AMD chipset devices.

## 7.0.7.0

### Added
- Added a backup Nvidia App download link to Downloads.json, so it can be updated without a new release.

### Changed
- Intel and AMD GPU detection now use the same vendor ID check, so they no longer give false matches on names like "Core" or "Xe".
- The Nvidia App installer now downloads straight to disk instead of into memory.
- A failed Nvidia App download no longer stops the rest of the install. It's marked as failed and the other apps carry on.
- Nvidia App is only marked as installed if the installer finishes successfully.

### Fixed
- Fixed Nvidia GPUs not being detected on laptops and fresh installs without an Nvidia driver. Detection now uses PCI vendor IDs instead of adapter names.
- Fixed "Could not find Nvidia App download link". The installer now looks like a browser to Nvidia's site and checks several Nvidia pages for the link.

## 7.0.6.3

### Changed
- Winget now puts two files on the Desktop, both .lock files, so the installer deletes them afterwards.

## 7.0.6.2

### Added
- Added Steam and Discord to taskbar if installed.

## 7.0.6.1

### Changed
- Tweaked dialog during Microsoft Office 2024 install.
- The Office installer is now silent.

### Fixed
- Fixed Outlook not being placed on the desktop and taskbar due to it being called "Outlook (classic)".

## 7.0.6.0

### Added
- Added Microsoft Office 2024 Pro Plus as an install option (requires product key).

### Changed
- If Microsoft Office 2024 is selected it skips LibreOffice (if ticked) and uninstalls LibreOffice if it's already installed.
- If Microsoft Office 2024 is installed it will place desktop and taskbar icons.

## 7.0.5.0

### Added
- Added ARM64 support. Apps that now have a native ARM64 version will install that rather than the x64 version. Not all currently do, so they will still install the x64 version which runs under emulation on Windows 11 anyway.

### Removed
- Removed Bing Wallpapers fom winget as it wasn't setting the wallpaper by default which defeats the purpose.

## 7.0.4.2

### Changed
- Changed Chandler's Ford's hours.

## 7.0.4.1

### Changed
- Corrected cursor size from 20 to 32 as it's being set in decimal not hex.

## 7.0.4.0

### Changed
- If the power box is not checked the installer will now set the screen and sleep timeout back to Windows 11 defaults rather 10 and 20 minutes respectively.

## 7.0.3.1

### Changed
- Corrected registry key used to change cursor size.

## 7.0.3.0

### Changed
- Now resets the cursor size to the default 20, as someone keeps making cursors massive.

## 7.0.2.1

### Changed
- Changed Romsey opening hours on Saturdays to prepare for upcoming change.

## 7.0.2.0

### Added
- Added a per-event text colour so button and title text stays readable against lighter holiday themes — fixes washed-out white text on Pluto's Birthday, Valentine's Day, Duck Day, Dachshund Day, Pancake Day, and birthdays.

### Changed
- Gave Beta builds their own orange colour scheme (background gradient, buttons, and title) matched to the Beta logo, instead of the default blue/purple.
- Beta builds now skip seasonal/holiday theming entirely, so Beta branding and holiday colours never clash.
- Seasonal events can now update the window title too (e.g. "Merry Christmas!"), not just the install log.

## 7.0.1.4

### Fixed
- Fixed "Last updated on" showing today's date instead of the actual build date — it was reading the exe's filesystem timestamp, which resets whenever the file is downloaded. Now the build date is baked at compile time via AssemblyUpdateDateAttribute.

## 7.0.1.3

### Changed
- File Explorer will now open to "This PC" by default on Windows 11 too, not just Windows 10.

## 7.0.1.2

### Changed
- Made values nullable.

## 7.0.1.1

### Changed
- Version number correction.

## 7.0.1.0

### Added
- Added a seperate Beta launcher with auto update just like the release launcher for easier maintenance.
- Added Beta branding to all beta builds to make them easier to identify.

### Removed
- Removed Firefox's new auto-start with Windows feature as that's just annoying.

## 7.0.0.0 - The .NET 8 / Avalonia Update

### Added
- Added a dropdown to manually override which shop's OEM info gets installed, separate from the automatic IP-based location check — the automatic check still decides whether the password prompt is needed, the dropdown only affects which shop's info gets written.
- Added a label next to Location that tells you the OS the installer is being run on.

### Changed
- Migrated the whole installer from .NET Framework 4.8 to .NET 8.
- Dropped support for Windows 7, 8, and 8.1 — .NET 8 doesn't run on them at all.
- Rewritten the entire GUI from WinForms to Avalonia.
- The build date will now automatically be set whenever the installer is built.

## 6.9.0.1

### Fixed
- Fixed hang caused by winget first-time run.

## 6.9.0.0 - The Winget Update

### Added
- Added winget as the first attempt for Chrome, Firefox, Thunderbird, LibreOffice, AnyDesk, Discord, Steam, VLC, and Bing Wallpaper — falls back automatically to the existing direct-download method if winget is unavailable or the install can't be verified.

### Changed
- Nvidia App and BitDefender are unaffected and continue to use their existing install methods only.
- winget installs are independently verified afterward via winget list rather than trusting its exit code, since winget's exit codes are unreliable.
- All required unattended-install flags are set (--accept-package-agreements, --accept-source-agreements, --disable-interactivity), so first-run license/source prompts can't hang the install on a fresh machine.

## 6.8.0.0 - The Class Update 2

### Added
- Added download retry (up to 3 attempts with backoff) for app installers, so a flaky connection no longer fails the whole install outright.
- Added an end-of-run summary showing what succeeded, what was skipped, and what failed.
- Added persistent install logging to disk (%LocalAppData%\ComputerRepairCentre\Logs) so a completed run can be reviewed after the fact.

### Changed
- Taskbar layout applies cleanly on next reboot/sign-in with no visible console window, and cleans up after itself automatically.
- Centralised Computer Repair Centre OEM info into a single shared method — shop logo and manufacturer name no longer duplicated per location.
- Moved PasswordHashes, LocationHashes, DownloadUrls, FileDeletionHelper, and PasswordForm out of the main form file into their own classes under Models/ and UI/.

### Fixed
- Fixed AnyDesk detection incorrectly reporting as not installed, causing unnecessary reinstalls.

### Removed
- Removed a redundant LibreOffice version check that ran on startup and blocked the UI.

## 6.7.4.3

### Changed
- Taskbar XML file is removed after first reboot to prevent constant app pinning.

## 6.7.4.2

### Changed
- If LibreOffice is installed Desktop icons will be created for Writer and Calc.

## 6.7.4.1

### Added
- Added File Explorer to taskbar pinning.

## 6.7.4.0

### Added
- Added "Pin apps to taskbar" option — dynamically pins Chrome, Firefox, Thunderbird, and LibreOffice Writer/Calc based on what was actually installed, replacing the default Windows/Edge pins.

### Removed
- Removed old icon files no longer being used.

## 6.7.3.0

### Removed
- Removed HP Hotkey Support as its no longer used.

## 6.7.2.0

### Changed
- When refurb is selected, the power button function will be set to shutdown rather than sleep.

## 6.7.1.2

### Changed
- Renamed Launcher exe.

## 6.7.1.1

### Changed
- Made the powercfg commands silent.

## 6.7.1.0

### Added
- Added Custura.
- Added an auto-update feature to the launcher so it will update itself when a new version is released.

### Changed
- Moved working directory from the root of C:/ to %localappdata%/ComputerRepairCentre.
- Moved OEM information to C:\ProgramData\Computer Repair Centre\OEM\.

## 6.7.0.0 - The Sound/Class Update

### Changed
- Moved most classes to their own files for easier editing and maintenance.
- Reworked the sound system to allow for easier adding of new sounds and to allow for different sounds to be played on different events.
- Moved from wav files to NAudio for smaller file sizes and better sound quality.
- Reworked the way the installer checks for holidays and birthdays to allow for easier adding of new holidays and birthdays.

## 6.6.6.2

### Added
- Added /norestart to LibreOffice installer.

## 6.6.6.1

### Changed
- Moved the progress bar down sligtly as it was slightly misaligned with the buttons.

## 6.6.6.0

### Changed
- Reworked the font system.

### Removed
- Removed rounded buttons and progress bar, moved back to a square look. Square is back in fashion.

## 6.6.5.0

### Added
- Added May the 4th.

## 6.6.4.1

### Fixed
- Fixed a typo with the VLC path causing a crash.

## 6.6.4.0

### Removed
- Removed "Remove Windows AI & Copilot" as it's unreliable.
- Removed Microsoft Office 2007, finally!

## 6.6.3.6

### Changed
- Updated the way the installer gets the current LibreOffice version.

## 6.6.3.5

### Fixed
- Fixed progress bar not completing.

## 6.6.3.4

### Changed
- Forgot to remove one instance of NanaZipExe in Microsoft Office 2007.

## 6.6.3.3

### Changed
- Moved NanaZip exe string out of the if statement, so it can be used across the installer.

## 6.6.3.2

### Changed
- Updated NanaZip exe name due to new version.

## 6.6.3.1

### Changed
- Small cleanup of code.
- Tweaked GUI for neater layout.

## 6.6.3.0

### Added
- Added custom rounded buttons.

### Changed
- Resized buttons for a more modern look.
- Rewritten IP hash check with HttpClient instead of WebClient.
- Moved password hash check to a json file for easier access.
- Reworked the text box slightly to allow redrawing when GUI is resized.
- Moved download URL's to a json to allow for easier changing of URL's if needed.

### Removed
- Removed shutdown button as it wasn't very useful.

## 6.6.2.0

### Added
- Added a fallback download for LibreOffice as their server is a bit flakey and the installer will sometimes hang if it cannot download the latest msi.

## 6.6.1.0

### Added
- Added gradient support to the rounded boxes.

## 6.6.0.0 - The Gradient Update

### Added
- Added gradient background.
- Added a custom rounded progress bar with gradient support.

### Changed
- Changed RichTextBox to a label within a panel to allow transparency.
- Made all objects transparent.
- Reconfigured AppendText to support label and panel.
- Moved the rounded group box to its own cs file.

## 6.5.4.1

### Changed
- Corrected minor typo.

## 6.5.4.0

### Changed
- Restructured birthday control so its easier to add and remove.
- Restructured holiday/event images so its easier to scale and simpler to add and remove.
- Did someone say duck?

## 6.5.3.0

### Added
- Added a checkbox for aligning the taskbar to the left on Windows 11 like previous versions, instead of just doing it automatically. This box is checked by default.

## 6.5.2.0

### Changed
- Slightly increased Install button size.

### Removed
- Removed taskbar pinning feature as it just wasn't reliable.

## 6.5.1.0

### Added
- Added test button for easier testing of new features.

### Changed
- The installer will now pin Google Chrome, Mozilla Firefox and Mozilla Thunderbird to the taskbar if installed.
- If Google Chrome or Mozilla Firefox are installed then Microsoft Edge will be unpinned from the taskbar.

## 6.5.0.0

### Changed
- Moved all URL's to my NextCloud as FTP was a pain.
- Changed the way IP's are stored. They are now stored in a json file, hashed for security; this makes changing them a lot easier.
- Moved location text to run rather than install.

## 6.4.8.0

### Added
- Added shutdown button.

### Changed
- Moved Version/Readme link to [professorshroom.com](https://professorshroom.com)

## 6.4.7.2

### Added
- Added a logic check to shutdown and restart check boxes so they both can't selected at the same time.

### Fixed
- Fixed a few more lines missing emojis.

## 6.4.7.1

### Added
- Added line to let you know that copilot is being removed.

### Fixed
- Fixed a few lines missing emojis.

## 6.4.7.0

### Changed
- Copilot should now be removed with the rest of the Windows AI components.

### Fixed
- Fixed Ubuntu font not applying to every line.

## 6.4.6.1

### Changed
- Simplified adding text to text box.

## 6.4.6.0

### Changed
- Converted the main installer text box to a rich text box.

## 6.4.5.0

### Changed
- Changed taskbar to align to the left in Windows 11.
- Tried to fix font not setting in text box.

## 6.4.4.0

### Changed
- Removal of Windows AI will now only run if the operating system is Windows 11.

## 6.4.3.1

### Changed
- Corrected a font style for the installer text box.

## 6.4.3.0

### Changed
- Changed font.
- Re-enabled remove Windows AI.

## 6.4.2.0

### Added
- Added shutdown button to shutdown computer when installation is complete.

### Changed
- Remade all icons along the bottom of the installer.

## 6.4.1.2

### Added
- Added New Year sound.

## 6.4.1.1

### Added
- Added New Year colours and image.

### Changed
- Corrected version number.

## 6.4.1.0

### Changed
- Re-enabled Windows AI Removal.
- Rewritten how the holiday themed image is drawn to reduce CPU usage and lag.

## 6.4.0.1

### Changed
- Disabled Windows AI Removal temporarily to try and fix slow loading.

## 6.4.0.0

### Added
- Added different icons for different national holidays, and Pluto's birthday.

## 6.3.1.0

### Added
- Added remove AI check box. When selected will remove all AI components from Windows 11.

## 6.3.0.0

### Changed
- Updated all Computer Repair Centre icons.

## 6.2.2.0

### Added
- Added New Year check.

## 6.2.1.0

### Added
- Added Christmas songs.

## 6.2.0.1

### Fixed
- Fixed Nvidia silent install.

## 6.2.0.0

### Changed
- Slightly redesigned the GUI to make it a bit nicer and more organised.

### Removed
- Removed AMD check box as the way AMD design thier installer makes it impossible download and install silently. I might readd later, all the detection code is there still.

## 6.1.18.1

### Changed
- Changed Nvidia downloader to match AMD.

### Fixed
- Fixed AMD software URL.

## 6.1.18.0

### Added
- Added Intel CPU and GPU checker just incase I want to add those in the future, if ARC becomes more mainstream.

### Changed
- Moved Windows version check to before you click install, like the hardware checks are.

### Removed
- Removed some unused flags.

## 6.1.17.0

### Added
- Added check box to empty the recycle bin on completion.
- Added AMD hardware checker just like Nvidia.

### Changed
- Updated Nvidia icon.
- Updated Office 2007 icon.

## 6.1.16.1

### Changed
- I never updated app.manifest to include support OS', so I added Windows 10 and 11.

### Removed
- Removed click sound, as I don't like it.

## 6.1.16.0

### Added
- Added custom update date attribute.

### Fixed
- Fixed last updated line. It now shows the day the installer was updated rather than just the current day.

## 6.1.15.0

### Added
- Added a location check label.

### Fixed
- Fixed sounds not playing correctly.

## 6.1.14.1

### Added
- Added multiple password support for IP check, this will allow temporary usaged by multiple different people if needed.

## 6.1.14.0

### Added
- Added password protection if the installer is run from a different IP address from one of the listed safe IP's.

### Changed
- IP addresses are now encrypted.

## 6.1.13.0

### Added
- Added button click and hover sounds.

## 6.1.12.6

### Added
- Added a couple more world days.

## 6.1.12.5

### Changed
- Made the birthday sound quieter.

## 6.1.12.4

### Fixed
- Fixed national Dachshund day date.

## 6.1.12.3

### Added
- Added national Dachshund day.

## 6.1.12.2

### Added
- Added automatically changing build date & version number.

## 6.1.12.1

### Changed
- Updated GitHub download URL's.

## 6.1.12.0

### Changed
- Updated launcher exe, changed download URL to crcinstaller.professorshroom.com to allow URL changes in the future.

## 6.1.11.2

### Fixed
- Fixed LibreOffice URL.

## 6.1.11.1

### Fixed
- Fixed error causing all birthdays to display as Charlie.

## 6.1.11.0

### Changed
- Changed Google Chrome URL to Google's own latest enterprise download link.
- Changed Mozilla Thunderbird URL to Mozilla's own latest download link.

## 6.1.10.0

### Added
- Added LibreOffice version checker with the same logic used for Nvidia App to download the latest version.

### Changed
- Changed Discord URL to Discord's own latest download link.
- Changed Mozilla Firefox URL to Mozilla's own latest download link.

## 6.1.9.1

### Fixed
- Fixed typo with Nvidia App exe.

## 6.1.9.0

### Changed
- Installer will check if the computer has an Nvidia GPU and install the Nvidia App if it does. This can be disabled if the user wants.

## 6.1.8.5

### Changed
- Updated Romsey IP as we now finally have a decent ISP.

## 6.1.8.4

### Added
- Added national puffin day. We missed out this year, sad.

## 6.1.8.3

### Fixed
- Fixed pancake day date.

## 6.1.8.2

### Changed
- Changed Google Chrome and NanaZip file names to make future updates easier.
- Updated Google Chrome.
- Updated VLC Media Player.

## 6.1.8.1

### Fixed
- Fixed a typo in VLC Media Players installation.

## 6.1.8.0

### Added
- Added VLC Media Player, for Adam.

## 6.1.7.1

### Added
- Added pancake day.

### Changed
- Changed some text in NanaZip to make it align better with other text.

## 6.1.7.0

### Changed
- Updated LibreOffice.
- Updated Mozilla Firefox.
- Updated Mozilla ThunderBird.
- Updated NanaZip.
- NanaZip now checks the installation folder dynamically which should allow NanaZip to update and not break the current scripts.

## 6.1.6.2

### Changed
- Updated the icons I replaced yesterday, as they were too big.

## 6.1.6.1

### Changed
- Disabled automatic restart.
- Replaced some icons.

## 6.1.6.0

### Added
- Added Valentines day colours and also images for every holiday.

## 6.1.5.0

### Fixed
- Fixed Microsoft Office 2007 causing an error when extracting - will now compelte extraction to the Desktop as designed.

## 6.1.4.0

### Changed
- Reverted to 6.1.2.8 as Office was not causing the crash.

### Removed
- Removed end task in the taskbar as that seemed to be causing issues, I may add it back in when I find out why.

## 6.1.3.0

### Changed
- Microsoft Office 2007 will no longer extract, it will just copy to the Desktop.

## 6.1.2.8

### Removed
- Removed LibreOffice XCD file as it was causing issue with LibreOffice opening.

## 6.1.2.7

### Changed
- Tried to fix the installer not being deleted on the Desktop.

## 6.1.2.6

### Changed
- Made file deleted async to avoid hanging on larger files like BitDefender.exe.

### Removed
- Removed auto arrange Desktop icons as I cannot get it to work, I don't think it's possible.

## 6.1.2.5

### Added
- Added a check for the EXE version of AnyDesk too just incase it is already installed, no point having both.

### Fixed
- Fixed BitDefender crash.

## 6.1.2.4

### Fixed
- Fixed BitDefender URL being incorrect so not installing.
- Fixed AnyDesk installation check, the MSI installer installs to a different location than the EXE for some reason.

## 6.1.2.3

### Added
- Added line to state the last date the installer was updated.

### Changed
- Updated power icon.
- Tried another fix for auto arrange Desktop icons.

## 6.1.2.2

### Fixed
- Fixed a typo in the launcherPath causing the launcher not to delete on the Desktop.

## 6.1.2.1

### Changed
- Attempted to fix deleting the launcher on the Desktop.
- Attempted to fix auto arrange Desktop icons.

## 6.1.2.0

### Changed
- Will now delete launcher file on the Desktop if it exists.
- Attempted to auto arrange Desktop icons for a cleaner look, unsure if this will work, if it doesn't work I will remove it.

## 6.1.1.2

### Changed
- Changed how the progress bar works to prevent crashes in the future.

## 6.1.1.1

### Changed
- Made NanaZip installation async.

### Fixed
- Fixed crash caused by HP Hotkey Support progress bar.

## 6.1.1.0

### Added
- Added HP Hotkey Support.

## 6.1.0.0

### Added
- Added AnyDesk, BitDefender, Discord, Mozilla Thunderbird, and Steam.
- Added Microsoft Office 2007, which extracts to the Desktop only (it isn't installed).
- Added NanaZip; it's optional via a checkbox, unless Microsoft Office 2007 is selected, in which case it's installed regardless of the tickbox state.
- Added Bing Wallpapers.
- Added an IP checker to detect where the installer is being run from, along with OEM information for Romsey, Chandlers Ford, and Highcliffe.
- Added holiday messages and colours.
- Added an option to disable sleep and screen timeout while on AC power.
- Added a restart checkbox, left unticked by default.

### Changed
- First full release of version 6 — the entire installer has been rewritten in C#, carrying over the majority of features from the previous version.
- LibreOffice installs now default file associations to Office 2007-2021 formats (docx, xlsx, etc.).
- Ported over all the Windows 10 and Windows 11 registry tweaks from the previous installer.
- Checks if Firefox is already installed before installing it and skips if so — the same check applies across the rest of the software.
- Cleans up installation files once the install finishes.
- Displays the version number in the readme.

## 5.2024.11.03.0

### Changed
- Moved some scripts to scripts folder.
- Now sets LibreOffice default file type to Office 2007-2021 (docx, xlsx, etc.).

## 5.2024.11.02.4

### Changed
- Changed some icon paths as I accidentally left in some test paths.

## 5.2024.11.02.3

### Added
- Added apps folder.

### Changed
- Changed power times if not not run on a refurb.

## 5.2024.11.02.2

### Fixed
- Fixed a couple of broken icons.

### Removed
- Removed Teams as doesn't seem needed now.

## 5.2024.11.02.1

### Fixed
- Fixed issue with folders not being created.

## 5.2024.11.02.0

### Added
- Added BitDefender as the world is being silly about Kaspersky.
- Added subfolders to 'C:\Computer Repair Centre' to tidy up installation folder.

### Changed
- Updated all the icons.

### Fixed
- Fixed Office 2007 URL typo.

### Removed
- Removed Windows 7 games, as I don't think anyones ever used it.
- Removed TeamViewer.

## 5.2024.10.31.0

### Changed
- Made the check for Kaspersky more precise as if run on a system with Kapersky VPN or Kaspersky Password Manager it would skip the installation as it believed Kaspersky was already installed.
- Readded Microsoft Office 2007, but will extract to the Desktop. Old people.

## 5.2024.10.29.6

### Changed
- Changed Halloween sound effect as it's probably a bit over the top.

## 5.2024.10.29.5

### Fixed
- Fixed elseIf command downloading correct sound file.

## 5.2024.10.29.4

### Changed
- Forgot date in download section.

## 5.2024.10.29.3

### Changed
- Reorganised some lines of code.

## 5.2024.10.29.2

### Changed
- Forgot to add sync hash to second part of the download code.

## 5.2024.10.29.1

### Added
- Added sync hash for holidays during download section.

## 5.2024.10.29.0

### Changed
- Corrected typo that stopped the sound from changing on holidays.

## 5.2024.10.23.1

### Fixed
- Fixed some typos that have been in the readme for a loooooong time.

## 5.2024.10.23.0

### Removed
- Removed iTunes as it's no longer supported.

## 5.2024.10.12.0

### Added
- Added birthday colours and sound.

## 5.2024.10.11.0

### Changed
- Reduced Halloween to 3 days.
- Changed Christmas colours to red and white.

## 5.2024.10.10.0

### Added
- Added completed sound effect once installer is finished.
- Added task end button to taskbar in Windows 11.

## 5.2024.10.07.3

### Changed
- Renamed LibreOffice installation file.

## 5.2024.10.07.2

### Fixed
- Fixed NanaZip as they changed the exe name.

## 5.2024.10.07.1

### Changed
- Updated NanaZip.
- Updated version number on installer exe.
- Updated LibreOffice msi to newer version.

### Removed
- Removed all old Office assets.

## 5.2024.10.07.0

### Removed
- Removed Office 2007 and moved CF to LibreOffice.

## 5.2024.08.09.0

### Changed
- Changed website URL's and opening hours in OEM info.

## 5.2024.05.09.0

### Changed
- Will now disable automatic device encryption is run on Windows 11.

## 5.2024.05.03.1

### Changed
- Dropped a }.

## 5.2024.05.03.0

### Removed
- Removed LibreOffice winget install as it keeps hanging.

## 5.2024.05.02.1

### Removed
- Removed line that causes installing Winget to hang.

## 5.2024.05.02.0

### Changed
- Winget installation will now run twice, it seems on some machines it isn't installing correctly.

## 5.2024.04.20.0

### Removed
- Removed dash from disabling sleep on AC power.

## 5.2024.04.15.0

### Added
- Added manual check for IP in CF and Romsey if the hardlinks aren't working.

### Changed
- Updated Chandlers Ford IP.

## 5.2024.04.02.0

### Changed
- Moved the screen and sleep pause to before downloading the prerequisites.

## 5.2024.03.25.2

### Changed
- If run in Chandlers Ford then the taskbar will not be moved to the left on Windows 11.

## 5.2024.03.25.1

### Changed
- Changed winget installation command.

## 5.2024.03.25.0

### Changed
- The reason for recent hanging is somehow the installer is downloading old files despite not even being listed, so I've removed them to see if that helps.

## 5.2024.03.23.0

### Changed
- Tried to fix hanging on winget installation.
- Updated NanaZip.
- Updated Winget.

## 5.2024.03.22.0

### Changed
- Updated LibreOffice on the server as the link no longer exists on their site. Tried to fix occasional hanging.

## 5.2024.02.29.0

### Changed
- If run on Windows 11 the taskbar will be aligned to the left like in Windows 10.

## 5.2024.02.7.0

### Changed
- Tried to fix Google Chrome installation from failing on the first attempt.

## 5.2024.01.20.0

### Changed
- Forget to add download for the refurb icon.

## 5.2024.01.19.1

### Added
- Added refurb icon.

## 5.2024.01.19.0

### Added
- Added refurb box so not all devices are set to never sleep.

## 5.2024.01.13.0

### Removed
- Removed old apps from initial download.

## 5.2024.01.03.3

### Changed
- I accidentally removed winget so I've readded it.

## 5.2024.01.03.2

### Changed
- Forgot to change NanaZip URL.

## 5.2024.01.03.1

### Fixed
- Fixed typos.

## 5.2024.01.03.0

### Changed
- Moved all the apps to our server instead of git.

## 5.2024.01.02.1

### Changed
- Forgot quotations around URLs.

## 5.2024.01.02.0

### Changed
- Moved app downloads to during the install rather than before to speed up initial download.

## 5.2023.11.03.0

### Changed
- Any log files will be removed before creating a new one just incase the installer is run more than once. Correctly this time.

## 5.2023.10.27.0

### Changed
- Buttons weren't Halloweeny enough.

## 5.2023.10.20.2

### Changed
- Any log files will be removed before creating a new one just incase the installer is run more than once.

## 5.2023.10.20.1

### Changed
- Once all files are cleaned up a log file will be create in C:\Computer Repair Centre\ with the installation date.

## 5.2023.10.20.0

### Removed
- Removed unzip of cleanupo files as they're no longer zipped.

## 5.2023.10.19.2

### Changed
- Tidied up the script that deletes files after installation so you don't get an error if Office 2007 wasn't installed.

## 5.2023.10.19.1

### Changed
- Forgot to remove Bing files unzip during NanaZip installation.

## 5.2023.10.19.0

### Removed
- Removed all of the old Bing Wallpaper installation files as the new method seems to work fine.

## 5.2023.10.18.2

### Changed
- Forgot to actually add it.

## 5.2023.10.18.1

### Changed
- Testing out new Bing Wallpapers installation method.

## 5.2023.10.18.0

### Fixed
- Fixed dark mode not activating when selected.

## 5.2023.10.16.1

### Changed
- Moved app to our server.

## 5.2023.10.16.0

### Changed
- Updated winget to see if that fixes recent winget issues.

## 5.2023.10.12.1

### Changed
- Also added backup for VLC Media Player.

## 5.2023.10.12.0

### Added
- Added backup installers of Google Chrome, Mozilla Firefox & LibreOffice due to issues with winget.

## 5.2023.10.09.0

### Changed
- Update winget ID as it was changed in the latest update.

## 5.2023.10.03.1

### Changed
- Stopped the installer being orange for the whole of October.

## 5.2023.10.03.0

### Added
- Added more Christmas songs.

### Changed
- Increased Halloween to a week before Halloween.

## 5.2023.09.18.2

### Fixed
- Fixed missing icons.

## 5.2023.09.18.1

### Changed
- Updated Discord icon.
- Reorganised assets folder.

## 5.2023.09.18.0

### Added
- Added version to bottom of the main window.
- Added view changelog button at the bottom of the main window.

## 5.2023.09.11.2

### Changed
- Changed VLC to only install on Windows 10.

### Fixed
- Fixed Discord icon.

## 5.2023.09.11.1

### Added
- Added some missing progress bar additions.

## 5.2023.09.11.0

### Added
- Added Discord.
- Added Steam.

### Changed
- Kaspersky will no longer be installed in Romsey or Highcliffe due to no longer having the option for a trial.

## 5.2023.07.21.2

### Changed
- Forgot to add --install switch so AnyDesk just opened.

## 5.2023.07.21.1

### Changed
- Moved '

## 5.2023.07.21.0

### Changed
- Changed to AnyDesk EXE instead of MSI.

## 5.2023.07.20.2

### Changed
- Changed AnyDesk MSI location to the AnyDesk site so it's always up to date.

## 5.2023.07.20.1

### Changed
- Changed AnyDesk installer to an MSI as the winget installer is frequently broken.

### Removed
- Removed TeamViewer check from AnyDesk install as it's no longer relevant.

## 5.2023.07.20.0

### Added
- Added check for Microsoft Office 2007 and added loop breaks for both Office and Kaspersky after 5 minutes.

## 5.2023.07.18.1

### Added
- Added waits after every file extraction and added 5 minute wait to Office 2007 install.

## 5.2023.07.18.0

### Fixed
- Fixed typo in NanaZip path and added Geeth's birthday.

## 5.2023.07.17.1

### Changed
- If Windows 7 games are installed then icons will be put on the Desktop for the games.

## 5.2023.07.17.0

### Changed
- Replaced 7-zip with NanaZip.

## 5.2023.04.29.0

### Changed
- If AnyDesk is selected then TeamViewer will also be removed so this can run on already installed machines to switch to AnyDesk.

## 5.2023.04.21.0

### Added
- Added AnyDesk and disabled TeamViewer from being selected by default.

## 5.2023.03.10.0

### Changed
- The installer will now update the Windows Store for Windows 11 as well as 10 because Adam is forgetful.

## 5.2023.02.16.0

### Changed
- Changed restart box icon as I don't think anyone knew what it was for.
- Changed this years Christmas song.

## 5.2023.01.31.0

### Added
- Added Kaspersky Standard to installer as a proper install.

## 5.2023.01.30.3

### Removed
- Removed fix attempt 1 files.

## 5.2023.01.30.2

### Changed
- That fixe worked so tied up check boxes and removed check box for sleep mode as who cares about carbon emissions.

## 5.2023.01.30.1

### Changed
- That fix didn't work, fix attempt number 2.

## 5.2023.01.30.0

### Changed
- Attemped to fix AC power sleep.

## 5.2023.01.24.4

### Changed
- Attempted to fix TeamViewer not installing.

## 5.2023.01.24.3

### Changed
- Amended force command.

## 5.2023.01.24.2

### Changed
- Correct deleteFiles.ps1

## 5.2023.01.24.1

### Changed
- If run on Windows 10 it will now update the Windows Store before installing Winget.

## 5.2023.01.24.0

### Added
- Added force command if a program fails to install on the retry.

## 5.2023.01.23.9

### Fixed
- Fixed typo in kaspersky download URL. It's too cold to type tonight.

## 5.2023.01.23.8

### Changed
- Forgot to add Kaspersky move command.

## 5.2023.01.23.7

### Added
- Added Kaspersky Standard exe temporarily until I figure out how to enable a silent install. This must be run manually after the install is complete.

### Changed
- Updated deleteFiles.ps1.
- Re-enabled Kaspersky.

## 5.2023.01.23.6

### Changed
- Corrected typo stopping winget package from downloading.

## 5.2023.01.23.5

### Fixed
- Fixed issue with Microsoft .NET Windows Desktop Runtime 3.1.

## 5.2023.01.23.4

### Added
- Added Microsoft .NET Windows Desktop Runtime 3.1.

### Changed
- If run on Windows 10 it will install winget and the prerequisites.

### Fixed
- Fixed hanging issue.

## 5.2023.01.23.3

### Changed
- Cleaned up checkboxes.

### Removed
- Removed Microsoft .NET Windows Desktop Runtime 3.1.

## 5.2023.01.23.2

### Changed
- Changed the way winget installs.

## 5.2023.01.23.1

### Added
- Added winget installation for Windows 10.

## 5.2023.01.23.0

### Changed
- Moved from chocolatey from to winget.
- Disabled Kaspersky for now until I can work out a silent install.

### Removed
- Removed Microsoft Office 2019 and 2021.
- Removed HashTab.
- Removed uBlock Origin.

## 4.2022.12.19.0

### Changed
- Updated copyright.
- Made light mode default in Chandlers Ford as Adam is old.

## 4.2022.12.02.4

### Added
- Added sleep after Christmas message.

## 4.2022.12.02.3

### Removed
- Removed Callum :(

## 4.2022.12.02.2

### Removed
- Remove misplaced number causing crashes.

## 4.2022.12.02.1

### Changed
- Corrected date.

## 4.2022.12.02.0

### Fixed
- Fixed Christmas message.

## 4.2022.10.31.3

### Fixed
- Fixed Halloween message not showing up.

## 4.2022.10.31.2

### Added
- Added Christmas colours and message for December.

## 4.2022.10.31.1

### Added
- Added Halloween colours.

## 4.2022.10.31.0

### Changed
- No longer sets "This PC" as default as the new Windows 11 home mode does the same thing.

## 4.2022.10.17.1

### Fixed
- Fixed missing ".

## 4.2022.10.17.0

### Changed
- Reverted Office 2021 back to one file as issue wasn't with installer it was with download site cert.

## 4.2022.10.15.2

### Fixed
- Fixed Office path typo.

## 4.2022.10.15.1

### Added
- Added progress steps for each part of Office 2021.

### Fixed
- Fixed Office 2021 download error.

## 4.2022.10.15.0

### Added
- Added Office 2021 folder to the cleanup script.

### Changed
- Split Office 2021 into 3 zips as it's so big.

## 4.2022.10.11.0

### Changed
- Updated domains for IP check.

## 4.2022.10.05.0

### Added
- Added Office 2021.

### Changed
- Stopped Office 2019 from installing on Windows 11.

## 4.2022.09.24.0

### Added
- Added Highcliffe phone number.

## 4.2022.09.17.0

### Changed
- Attempted to fix monitor and standby timeouts not changing.

## 4.2022.09.14.0

### Added
- Added known Highcliffe details, still awaiting phone number.

## 4.2022.09.10.4

### Changed
- Installer will now wait for the HP install.cmd to finish before continuing.

## 4.2022.09.10.3

### Changed
- Moved } that was in the wrong place stopping the HP software from installing.

## 4.2022.09.10.2

### Changed
- Changed the order in which sleep will change timeout modes to make sure they're set correctly.

## 4.2022.09.10.1

### Fixed
- Fixed two conflicting arguments.

## 4.2022.09.10.0

### Fixed
- Fixed not setting HP EliteBook status correctly.

## 4.2022.09.09.0

### Added
- Added HP Hotkey Support for HP EliteBooks as Steve can't install it.

## 4.2022.09.07.1

### Changed
- Installer will now sing happy birthday.

## 4.2022.09.07.0

### Added
- Added Highcliffe.
- Added Callum's birthday.

### Changed
- Improved way of getting the IP.

## 4.2022.08.04.0

### Changed
- Moved Office 2007 & Kaspersky files to BRM/FFITR domain for faster download speeds.

## 4.2022.07.21.2

### Fixed
- Fixed typo.

## 4.2022.07.21.1

### Changed
- Will now uninstall Kaspersky VPN & Secure Connection after checking if install has complete to make sure it removes just in case the install fails for any reason.

## 4.2022.07.21.0

### Added
- Added "Reset chocolatey" button to remove previous installations of chocolatey if needed.

## 4.2022.07.18.2

### Changed
- Only downloads Windows 7 Games if it is selected to speed up loading the program.

## 4.2022.07.18.1

### Fixed
- Fixed Windows 7 Games check box from overlapping with Zoom.

## 4.2022.07.18.0

### Added
- Added Windows 7 Games as an install option.

## 4.2022.07.08.2

### Fixed
- Fixed iTunes icon.

## 4.2022.07.08.1

### Added
- Added birthday messages.

### Changed
- Changed all checkboxes to images rather than text.

## 4.2022.07.08.0

### Fixed
- Fixed typo causing buttons to be blank.

## 4.2022.07.07.1

### Added
- Added dark mode.
- Added check boxes for all branches that can be checked and unchecked if ever needed.
- Added Highcliffe location but disabled until branch is opened.

### Changed
- Modernised the GUI slightly.

## 4.2022.07.07.0

### Changed
- Updated EXE to run first time every time.

## 3.2022.07.02.5

### Changed
- Corrected progress bar.

## 3.2022.07.02.4

### Changed
- Will now close Office 2019 installer once it has completed the install.

## 3.2022.07.02.3

### Added
- Added last updated line when installer is run.

## 3.2022.07.02.2

### Changed
- Stopped the installer from deleting choco on every run.
- Will now remove CRC Installer x64.exe from Desktop on reboot.

## 3.2022.07.02.1

### Fixed
- Fixed Word path error.

## 3.2022.07.02.0

### Fixed
- Fixed issue with installer not checking for already installed programs & fixed Office 2019 activation issue.

## 3.2022.07.01.0

### Added
- Added Desktop icons for Office 2007 and 2019 installs.

### Changed
- The installer will now check if a program is already installed before running the install command to speed up the installs on machines it has been run on previously.
- Cleaned up Office 2019 install.
- Attemped to fix progress bar but it might need some further tweaks to get the progress point correct.

## 3.2022.05.22.0

### Changed
- Updated IP checker to check for Romsey IP instead of Chandler's Ford as the IP doesn't have to be update periodically.

## 3.2022.05.19.1

### Fixed
- Fixed darkmode overriding wallpaper selection.

## 3.2022.05.19.0

### Changed
- Enabled dark mode by default.

## 3.2022.05.07.0

### Changed
- Updated Kaspersky.
- Updated download link for Kaspersky.
- Updated the readme to include new features I previously forgot to add.
- Corrected some typos.
- uBlock Origin is now disabled by default due to some customer complaints about removing it.

### Removed
- Removed taskbar pin as this currently doesn't work on Windows 11.

## 3.2021.12.13.1

### Fixed
- Fixed progress bar.

## 3.2021.12.13.0

### Changed
- Will now remove old installations of Chocolatey to prevent issues with programs not installing correctly if there is a crash or other issues.

## 3.2021.10.26.0

### Added
- Added HashTab to prerequisites for convenience.

## 3.2021.10.25.0

### Added
- Added 4K support to Bing Wallpapers.

### Changed
- Changed version format to keep track of when it was updated easier.

## 3.10.12.9

### Fixed
- Fixed typo causing 2 Bing wallpaper tasks.

## 3.10.12.8

### Changed
- Changed Bing wallpaper to run at 1PM everyday and at every logon.
- Corrected progress bar on installer.

## 3.10.12.7

### Changed
- Changed all unzipping commands to overrite to avoid any future problems.

## 3.10.12.6

### Changed
- Stopped installer from hanging on extracting zip files. I really hope this is the last one because this is boring now.

## 3.10.12.5

### Changed
- Changed how Bing wallpapers downloads files.

## 3.10.12.4

### Fixed
- Fixed spelling error.

## 3.10.12.3

### Fixed
- Fixed zipped files not extracting in time.

## 3.10.12.2

### Fixed
- Fixed Bing wallpapers not downloading.

## 3.10.12.1

### Changed
- Bing wallpapers now saves the files it's own folder to keep things tidy.

## 3.10.12.0

### Changed
- Made some changes to Bing Wallpaper so it now saves the file with the date in the name so the user can keep wallpapers they like.
- Zipped up the Bing Wallpaper files to stop files from failing from downloading.

## 3.10.11.2

### Changed
- Updated Choco.

## 3.10.11.1

### Fixed
- Fixed issue with Teams being installed with TeamViewer.

## 3.10.11.0

### Added
- Added MalwareBytes as install option.

### Fixed
- Fixed Teams icon.

## 3.10.10.0

### Added
- Added initial support for Windows 11.
- Added Teams as an install option.
- Added Windows 11 tweaks to match Windows 10. More to come later on release.

### Changed
- Installer will now check software MD5 checksums before installation.
- Installer now installs Microsoft Visual C++ Redistributable as part of the prerequisites.

### Fixed
- Fixed Kaspersky mailing list issue.

### Removed
- Removed some Windows 10 tweaks to make it run nicer for the end user.

## 3.10.9.0

### Changed
- Updated Kaspersky Internet Security.

## 3.10.8.2

### Fixed
- Fixed text for reboot message going off the screen.

## 3.10.8.1

### Added
- Added a message to indicate that the machine is recognised as a refurb unit.

## 3.10.8.0

### Changed
- The refurb and reboot box will now be ticked if the installer is run on a HP EliteBook as these are common refurb units.

## 3.10.7.1

### Changed
- Updated Romsey hours.

## 3.10.7.0

### Changed
- The installer now disables the weather taskbar widget.

## 3.10.6.0

### Changed
- Bing Wallpapers task will now run at 1AM incase user leaves the device on and will run at earliest opportunity if not.

## 3.10.5.0

### Changed
- Installer will now delete leftover files on next startup.

## 3.10.4.1

### Fixed
- Fixed typo stopping Bing Wallpapers from working.

## 3.10.4.0

### Changed
- Reverted Bing Wallapers to save into the Picture folder.

### Fixed
- Fixed issue with user selected wallpapers.

## 3.10.3.0

### Changed
- Bing Wallpapers now saves the C Drive instead of the Pictures folder to allow slideslow mode.
- Bing Wallpapers will no longer override user selected wallpapers.

## 3.10.2.0

### Changed
- Installer will now remove Kaspersky VPN after installing Kaspersky Internet Security.

## 3.10.1.4

### Changed
- Stopped a Powershell window from appearing during the Bing wallpaper task.

## 3.10.1.3

### Fixed
- Fixed Zoom interfering with Bing wallpapers.

## 3.10.1.2

### Fixed
- Fixed Kaspersky Internet Security icon.

## 3.10.1.1

### Fixed
- Fixed Kaspersky Internet Security URL issue.

## 3.10.1.0

### Changed
- Updated Kaspersky Internet Security 2021 to 21.3.10.391.

## 3.10.0.1

### Changed
- Stopped Zoom from being checked by default.

## 3.10.0.0

### Added
- Added Zoom to installer options.

### Changed
- If the user selects Kaspersky Internet Security they are now notified that the system requires a reboot.
- Updated Kaspersky Internet Security to 2021.
- Licensed under Mozilla Public License 2.0.

## 3.9.0.2

### Fixed
- Fixed Bing Wallpaper resetting task every time it runs.

## 3.9.0.1

### Fixed
- Fixed issue with Bing wallpaper task not knowing what app to use.

## 3.9.0.0

### Changed
- Updated Chandler's Ford IP address.
- Wallpaper option has been replaced with the option to install Bing wallpaper that will grab a new wallpaper everyday.

### Removed
- Removed custom wallpaper and theme installation.
- Removed wallpaper support on Windows 7.

## 3.8.1.1

### Removed
- Removed Visual C++ Runtimes temporarily.

## 3.8.1.0

### Added
- Added a box to disable sleep mode on AC power.

### Changed
- Installer will now instal various Visual C++ Runtimes as part of the prerequisites.

## 3.8.0.0

### Added
- Added a reboot box that if checked will reboot the computer once the installer has finished, with a 30 second delay so the user can cancel if required.
- Added a countdown timer to the reboot button if a reboot is pending to show user how much time they have until the compuer is rebooted.

## 3.7.2.1

### Removed
- Removed quotation marks causing crashes from previous update.

## 3.7.2.0

### Changed
- Installer will now notify you if the system needs a reboot.

## 3.7.1.3

### Fixed
- Fixed typo causing Microsoft Office 2007 to not install.

## 3.7.1.2

### Changed
- Changed how Microsoft Office activates to hopefully prevent Windows Defender disabling it.

## 3.7.1.1

### Fixed
- Fixed Microsoft Office 2019 activation bug.

## 3.7.1.0

### Changed
- Microsoft Office 2019 should now automatically activate.

## 3.7.0.2

### Added
- Added Microsoft Office 2007 icon.

## 3.7.0.1

### Changed
- Microsoft Office 2007 will be auto selected in Chandler's Ford.

## 3.7.0.0

### Added
- Added Microsoft Office 2007 which will automatically activate.

## 3.6.2.0

### Changed
- Microsoft Office 2019 should now automatically activate.

## 3.6.1.0

### Changed
- Updated multiple icons.

## 3.6.0.2

### Fixed
- Fixed Microsoft Office icon.

## 3.6.0.1

### Fixed
- Fixed crashing issue.

## 3.6.0.0

### Changed
- Updated for 2021, added option to install Microsoft Office 2019 Volume License and changed Chandler's Ford IP.

## 3.5.5.0

### Changed
- Installer now enables the Action Centre and toast notifications.

## 3.5.4.2

### Changed
- Google Chrome is now pinned again.

## 3.5.4.1

### Fixed
- Fixed link location for Kaspersky.

## 3.5.4.0

### Changed
- Updated Kaspersky.

## 3.5.3.2

### Changed
- Installer now leaves Cortana search bar in the taskbar. Again. Because of old people.

## 3.5.3.1

### Fixed
- Fixed issue with Kaspersky not downloading.

## 3.5.3.0

### Changed
- Installer now leaves Cortana search bar in the taskbar.

## 3.5.2.0

### Added
- Added more wallpapers so installer with now randomly pick between two different wallpapers sets. Can easily add more in the future.

## 3.5.1.0

### Added
- Added Chocolatey to the prerequisites and stopped disabling monitor timeout on Windows 10 as it wasn't working.

## 3.5.0.4

### Fixed
- Fixed issue with default broswer sometimes not settings correctly.

## 3.5.0.3

### Changed
- Updated download URLs from legacy to current.

## 3.5.0.2

### Changed
- Installer now installs Microsoft .NET 3.5 and .NET 4.6.2 as a prerequisite.

## 3.5.0.1

### Changed
- When installing prerequisites the installer will now show what is being installed.

## 3.5.0.0

### Changed
- Default browser will change to Google Chrome if installed, Mozilla Firefox if installed or Mozilla Firefox is both Chrome and Firefox are installed.

### Removed
- Removed most of the support for Windows 7, no future features will come to 7 but programs will continue to install and wallpapers will continue to be set.
- Removed official support for Windows 8 & 8.1 but basic programs will continue to install.

## 3.4.1.1

### Fixed
- Fixed bug causing installer to hang.

## 3.4.1.0

### Changed
- Re-enabled taskbar pinning for Windows 10.

## 3.4.0.1

### Fixed
- Fixed missing token.

## 3.4.0.0

### Changed
- Installer can now enable the set theme in light and dark mode depending on the dark mode selection.

## 3.3.5.3

### Changed
- Updated Chandler's Ford IP.

## 3.3.5.2

### Fixed
- Fixed another issue with light mode. Take the hint old people.

## 3.3.5.1

### Fixed
- Fixed issue with dark mode being enabled even with box not ticked. Because old people.
- Fixed issue with installer only working on second launch, it should now work on first launch.

## 3.3.5.0

### Changed
- Disabled Windows dark mode by default because old people and updated Chandler's Ford IP.

## 3.3.4.0

### Changed
- Updated Firefox logo.

## 3.3.3.1

### Added
- Added in last update date to information box.

## 3.3.3.0

### Changed
- Updated Kaspersky Internet Security to 2020.

## 3.3.2.0

### Fixed
- Fixed issue with LibreOffice not installing correctly.

## 3.3.1.2

### Fixed
- Fixed checksum issues.

## 3.3.1.1

### Removed
- Removed birthday messages.

## 3.3.1.0

### Changed
- Enabled VLC by default.

## 3.3.0.0

### Changed
- Temporarily disabled taskbar pining until a new solution is found for Windows 1903.
- Moved night mode to its own check box so it can be disabled. Fixed theme for Windows 1903.

### Fixed
- Fixed Google Chrome and Mozilla Firefox uBlock Origin installs with the new version of Windows 1903.

## 3.2.2.2

### Removed
- Removed old installer leftover files and cleaned up sleep functions.

## 3.2.2.1

### Removed
- Removed left over code.

## 3.2.2.0

### Added
- Added close button.

### Changed
- Birthday feature now says the name of whose birthday it is.

## 3.2.1.0

### Added
- Added birthday easter egg.

## 3.2.0.2

### Fixed
- Fixed wallpapers not downloading.

## 3.2.0.1

### Fixed
- Fixed Computer Repair Centre icon not downloading.

## 3.2.0.0

### Changed
- Rewritten the executable in C++ so Windows Defender and other anti-virus software won't delete it. Added in loading form while the files download.

## 3.1.0.0

### Changed
- The installer is now multithreaded so the GUI uses one thread and the script uses another. This speeds it up and also stops the GUI from freezing while the script is in progress. Also added am adaptive progress bar to show much progress is left.

## 3.0.9.11

### Fixed
- Fixed progress box not auto-scrolling.

## 3.0.9.10

### Fixed
- Fixed crashing issue.

## 3.0.9.9

### Added
- Added features from 3.0.9.8.

## Revert to 3.0.9.7

### Changed
- Reverted due to mass rename of "Checked".

## 3.0.9.8

### Fixed
- Fixed some typos and added in some features for Windows 8.x.

## 3.0.9.7

### Changed
- Reverted to 3.0.8.0 pinning.

## 3.0.9.6

### Changed
- Changed taskbar pinning to a VBS script.

## 3.0.9.5

### Changed
- Reverted to old way of pinning taskbar icons and re-renabled Cortana search bar for Chandler's Ford.

## 3.0.9.4

### Changed
- Updated OEM logo.

## 3.0.9.3

### Fixed
- Fixed dark mode.

## 3.0.9.2

### Removed
- Removed most of the build in Windows 10 tracking features.

## 3.0.9.1

### Fixed
- Fixed crashing issue.

## 3.0.9.0

### Added
- Added XML file to replace taskbar pinning on Windows 10.

## 3.0.8.5

### Removed
- Removed auto-arranging Desktop icons due to locking issue.

## 3.0.8.4

### Fixed
- Fixed crashing issue.

## 3.0.8.3

### Changed
- Installer cleans up files before downloading new files.

## 3.0.8.2

### Added
- Added auto-arranging Desktop, now removed Kaspersky Safe Money icon from the Desktop and updated taskbar pinning for Windows 8 and 8.1.

## 3.0.8.1

### Fixed
- Fixed formatting issue and changed icon.

## 3.0.8.0

### Added
- Added dark mode for Windows 10, added more wallpapers, added option to disable taskbar repin and fixed pinning icons on Windows 7.

## 3.0.7.4

### Changed
- Replaced taskbar pin with better PowerShell script.

## 3.0.7.3

### Fixed
- Fixed paths.

## 3.0.7.2

### Fixed
- Fixed error with exe.

## 3.0.7.1

### Fixed
- Fixed crashing error.

## 3.0.7.0

### Fixed
- Fixed wallpaper icon not downloading and updated host address.

## 3.0.6.3

### Changed
- Improved tashbar pinning.

## 3.0.6.2

### Fixed
- Fixed system pin and added Josh's birthday.

## 3.0.6.1

### Changed
- Updated Kaspersky.

## 3.0.6.0

### Changed
- Updated UI and moved from Kaspersky 2018 to 2019.

## 3.0.5.4

### Changed
- Adding auto-arranging Desktop icons.

## 3.0.5.3

### Fixed
- Fixed issue with pin to taskbar not working.

## 3.0.5.2

### Changed
- Updated URL in executable.

## 3.0.5.1

### Fixed
- Fixed URL.

## 3.0.5.0

### Changed
- Moved to self hosted GitLab to avoid download issues.

## 3.0.4.2

### Changed
- Improved the way uBlock Origin is installed on Firefox.

## 3.0.4.1

### Fixed
- Fixed problem with uBlock Origin icon.

## 3.0.4.0

### Added
- Added option for uBlock Origin to be installed on Mozilla Firefox and Google Chrome.

### Changed
- Updated UI and made interface smaller.

## 3.0.3.0

### Added
- Added Skype as install option.

## 3.0.2.2

### Fixed
- Fixed issue with Kaspersky Internet Security 2018.

## 3.0.2.1

### Removed
- Removed progress bar due to invisible issues and added console that minimises on open.

## 3.0.2.0

### Added
- Added loading bar during file downloads and reverted to Kaspersky Internet Security 2018 due to Windows 10 issues.

## 3.0.1.1

### Removed
- Removes Safe Money icon from Desktop.

## 3.0.1.0

### Changed
- Improved the cleanup process after installation and fixing locking up issue.

## 3.0.0.1

### Removed
- Removed testing bug and changed downloading so it now only downloads files if necessary.

## 3.0.0.0

### Changed
- Renamed to NorthPoint Installer and moved to GitLab. Modernised the GUI and improved performace.

## 2.6.0

### Changed
- Now unistalls Kaspersky Secure Connection after installing Kaspersky Internet Security 2019.

## 2.5.2

### Fixed
- Fixed IP issue with Chandlers Ford.

## 2.5.1

### Changed
- LibreOffice will be checked if run from Romsey and unchecked if run from Chandlers Ford.

## 2.5.0

### Added
- Added Mozilla Thunderbird and iTunes. Set LibreOffice to be ticked by default.

## 2.4.2

### Changed
- Updated Kaspersky Internet Security to 2019.

## 2.4.1

### Changed
- Will now auto-arranges Desktop icons after the installer has finished.

## 2.4.0

### Changed
- Deletes "Microsoft Edge" icon from the Desktop in the new April 2018 update of Windows 10.

## 2.3.1

### Changed
- Now disables standby and monitor timeout when plugged in.

## 2.3.0

### Added
- Added "close.ps1" to remove all unnecessary files after installation is complete.

## 2.2.4

### Fixed
- Fixed issue with wallpapers hanging if folder already exists.

## 2.2.3

### Changed
- Cleaned up spacing in code.

## 2.2.2

### Fixed
- Fixed birthday Easter egg.

## 2.2.1

### Removed
- Removed orphan code.

## 2.2.0

### Changed
- Cleaned up code, changed requisites and added more wallpapers.

## 2.1.2

### Added
- Added random wallpaper to checkboxes so you now have choice.

## 2.1.1

### Added
- Added "close.ps1" script that forces a restart of the computer.

## 2.1.0

### Added
- Added feature that sets random wallpaper per machine.

## 2.0.3

### Fixed
- Fixed problem with execution policy not changing.

## 2.0.2

### Changed
- Executable now sets execution policy and fixed issue with Kaspersky Internet Kaspersky.

## 2.0.1

### Added
- Added Kaspersky Internet Security 2018 & changed spacing for icons.

## 2.0.0

### Changed
- Rewritten executable to work with Windows Defender and download quicker. Removed log file, Apache OpenOffice & WPSOffice.

## 1.12.1

### Fixed
- Fixed crashing issue.

## 1.12.0

### Added
- Added a lot more Windows 10 Tweaks.

## 1.11.4

### Changed
- No longer installs LibreOffice by defualt and added the new Mozilla Firefox Quantum.

## 1.11.3

### Removed
- Removed the moving of Windows.Old folders due to permissions issue.

## 1.11.2

### Added
- Added sleep after moving previous "Users" folder.

## 1.11.1

### Fixed
- Fixed previous "Users" folder not being renamed.

## 1.11.0

### Changed
- Installer will check if the programs have installed successfully and notify you of any failures.

## 1.10.1

### Changed
- Now checks for up to 5 previous Windows installations instead of 1.

## 1.10.0

### Changed
- Checks for previous Windows installation and then moves old documents to the desktop if it finds any.

## 1.9.1

### Changed
- Installer now tells you local info in text box.

## 1.9.0

### Added
- Added Apache OpenOffice and WPS Office as install options.

## 1.8.7

### Fixed
- Fixed an issue where it should be checking IP instead of SSID.

## 1.8.6

### Changed
- Replaced SSID check with public IP check so it now works on towers as well as laptops and rearranged installation process.

## 1.8.5

### Changed
- Moved location check to run immediately after "Install" is pressed.

## 1.8.4

### Added
- Added form that lets you select your location if it can't find it.

## 1.8.3

### Added
- Added feature to find IP and then install the correct OEM information if Wi-Fi is nto available.

## 1.8.2

### Fixed
- Fixed crashing issue with Wi-Fi searching.

## 1.8.1

### Changed
- Improved taskbar pinning on Windows 7, 8, and 8.1. Reintroduced taskbar pinning for Windows 10.

## 1.8.0

### Added
- Added feature that auto detects current Wi-Fi network, eliminating the need for multiple installers.

## 1.7.5

### Removed
- Removed commands that disable some telememtary and Cortana.

## 1.7.4

### Fixed
- Fixed incorrect wording when installing Mozilla Firefox.

## 1.7.3

### Fixed
- Fixed command prompt not going invisible on Romsey installer and changed wording for installation.

## 1.7.2

### Changed
- Changed directory for icon file.

## 1.7.1

### Changed
- Updated exe to include icon and loading animation.

## 1.7.0

### Added
- Added loading form so you know the program is loading.

## 1.6.0

### Changed
- Made command line invisible for sleaker look.

## 1.5.6

### Fixed
- Fixed crashing issue on Windows 7.

## 1.5.5

### Fixed
- Fixed problem with Kaspersky and TeamViewer not installing.

## 1.5.4

### Fixed
- Fixed crashing problem.

## 1.5.3

### Fixed
- Fixed issue with Documents folder.

## 1.5.2

### Fixed
- Fixed file cleanup on completion.

## 1.5.1

### Removed
- Removed extra lines for downloading files.

## 1.5.0

### Changed
- Temporarily removed automatically setting taskbar icons in Windows 10 due to issue and added KIS Chocolatey package.

## 1.4.1

### Fixed
- Fixed OEM website HTTPS information.

## 1.4.0

### Added
- Added autoscrolling feature to list box and reduced size of install window.

## 1.3.0

### Changed
- Renamed to CRC Installer and added new Romsey version.

## 1.2.1

### Fixed
- Fixed issue with Google Chrome checksums.

## 1.2.0

### Changed
- Replaced text with icon of program for cleaner look.

## 1.1.3

### Fixed
- Fixed crashing issue with icon file.

## 1.1.2

### Removed
- Removed exit button and will now close 15 seconds after the install has finished.

## 1.1.1

### Fixed
- Fixed issue with downloading CleanCF.ps1.

## 1.1.0

### Changed
- Programs now install in alphabetical order and added an exit button than cleans up temporary files.
