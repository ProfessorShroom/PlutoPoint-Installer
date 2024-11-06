 ![Arch](https://img.shields.io/badge/Arch-AMD64-blue) ![OS](https://img.shields.io/badge/OS-Windows%2010%20|%20Windows%2011-green)

## PlutoPoint Installer Version 6.0.5.0b
###### Copyright (c) Charlie Howard 2024 All rights reserved.

A C# based GUI installer for Windows 10/11.

Installs Microsoft .NET (3.1, 5.0, 6.0, 7.0) and NanaZip by default as these are needed or recommended for the installer to work correctly.

Gives you the option to install the Computer Repair Centre OEM information, Bing Wallpapers, Dark mode, AnyDesk, BitDefender, Discord, Google Chrome, LibreOffice, MalwareBytes, Mozilla Firefox, Mozilla Thunderbird, Skype, Steam, VLC Media Player, and Zoom.

If run on Windows 10 or Windows 11 it disables hibernation mode and Wi-Fi sense.

If run on Windows 11 it will disable automatic device encryption and add "end task" to the programs in the taskbar.

Disables sleep and screen timeout if plugged into AC power during the install and then revert it once it has completed to prevent the computer going to sleep during the install which can cause issues, you also get the option to prevent sleep on AC power permanently with the "Refurb" box.

### Changelog

**Update 6.0.5.0b**

Added BitDefender, AnyDesk, Discord, Mozilla Thunderbird, and Steam, but currently they don't do anything.

Added the installation check from Firefox to the other software.

**Update 6.0.4.0b**

Fixed crash.

Will now check is Firefox is installed before installing and skip it if it is, will add this to other software if it works as intended.

**Update 6.0.3.1b**

Fixed URL to OEM BMP.

**Update 6.0.3.0b**

Added holiday messages and holiday colours.

Added Windows 10 registry tweaks.

Added option to disable sleep and screen timeout on AC power.


**Update 6.0.2.0b**

Added IP checker to check where the installer is being run from.

Added in all of the Windows 11 registry tweaks from the old installer.

Added OEM information, only in Romsey for now for testing.

**Update 6.0.1.0b**

Fixed Google Chrome installation.

Added Bing Wallpapers.

Added restart box but left it unticked by default for now.

Added version number with readme.

**Update 6.0.0.0b**

Rewritten the installer using C#, the majority of features are being carried over, but it may take a few updates to get all of the features fully implemented.

**Update 5.2024.11.03.0**

Moved some scripts to scripts folder.

Now sets LibreOffice default file type to Office 2007-2021 (docx, xlsx, etc.).

**Update 5.2024.11.02.4**

Changed some icon paths as I accidentally left in some test paths.

**Update 5.2024.11.02.3**

Added apps folder.
Changed power times if not not run on a refurb.

**Update 5.2024.11.02.2**

Fixed a couple of broken icons.

Removed Teams as doesn't seem needed now.

**Update 5.2024.11.02.1**

Fixed issue with folders not being created.

**Update 5.2024.11.02.0**

Removed Windows 7 games, as I don't think anyones ever used it.

Added BitDefender as the world is being silly about Kaspersky.

Fixed Office 2007 URL typo.

Updated all the icons.

Added subfolders to 'C:\Computer Repair Centre' to tidy up installation folder.

Removed TeamViewer.

**Update 5.2024.10.31.0**

Made the check for Kaspersky more precise as if run on a system with Kapersky VPN or Kaspersky Password Manager it would skip the installation as it believed Kaspersky was already installed.

Readded Microsoft Office 2007, but will extract to the Desktop. Old people.

**Update 5.2024.10.29.6**

Changed Halloween sound effect as it's probably a bit over the top.

**Update 5.2024.10.29.5**

Fixed elseIf command downloading correct sound file.

**Update 5.2024.10.29.4**

Forgot date in download section.

**Update 5.2024.10.29.3**

Reorganised some lines of code.

**Update 5.2024.10.29.2**

Forgot to add sync hash to second part of the download code.

**Update 5.2024.10.29.1**

Added sync hash for holidays during download section.

**Update 5.2024.10.29.0**

Corrected typo that stopped the sound from changing on holidays.

**Update 5.2024.10.23.1**

Fixed some typos that have been in the readme for a loooooong time.

**Update 5.2024.10.23.0**

Removed iTunes as it's no longer supported.

**Update 5.2024.10.12.0**

Added birthday colours and sound.

**Update 5.2024.10.11.0**

Reduced Halloween to 3 days.
Changed Christmas colours to red and white.

**Update 5.2024.10.10.0**

Added completed sound effect once installer is finished.
Added task end button to taskbar in Windows 11.

**Update 5.2024.10.07.3**

Renamed LibreOffice installation file.

**Update 5.2024.10.07.2**

Fixed NanaZip as they changed the exe name.

**Update 5.2024.10.07.1**

Updated NanaZip.
Removed all old Office assets.
Updated version number on installer exe.
Updated LibreOffice msi to newer version.

**Update 5.2024.10.07.0**

Removed Office 2007 and moved CF to LibreOffice.

**Update 5.2024.08.09.0**

Changed website URL's and opening hours in OEM info.

**Update 5.2024.05.09.0**

Will now disable automatic device encryption is run on Windows 11.

**Update 5.2024.05.03.1**

Dropped a }.

**Update 5.2024.05.03.0**

Removed LibreOffice winget install as it keeps hanging.

**Update 5.2024.05.02.1**

Removed line that causes installing Winget to hang.

**Update 5.2024.05.02.0**

Winget installation will now run twice, it seems on some machines it isn't installing correctly.

**Update 5.2024.04.20.0**

Removed dash from disabling sleep on AC power.

**Update 5.2024.04.15.0**

Updated Chandlers Ford IP.
Added manual check for IP in CF and Romsey if the hardlinks aren't working.

**Update 5.2024.04.02.0**

Moved the screen and sleep pause to before downloading the prerequisites. 

**Update 5.2024.03.25.2**

If run in Chandlers Ford then the taskbar will not be moved to the left on Windows 11.

**Update 5.2024.03.25.1**

Changed winget installation command.

**Update 5.2024.03.25.0**

The reason for recent hanging is somehow the installer is downloading old files despite not even being listed, so I've removed them to see if that helps.

**Update 5.2024.03.23.0**

Tried to fix hanging on winget installation.
Updated NanaZip.
Updated Winget.

**Update 5.2024.03.22.0**

Updated LibreOffice on the server as the link no longer exists on their site. Tried to fix occasional hanging.

**Update 5.2024.02.29.0**

If run on Windows 11 the taskbar will be aligned to the left like in Windows 10.

**Update 5.2024.02.7.0**

Tried to fix Google Chrome installation from failing on the first attempt.

**Update 5.2024.01.20.0**

Forget to add download for the refurb icon.

**Update 5.2024.01.19.1**

Added refurb icon.

**Update 5.2024.01.19.0**

Added refurb box so not all devices are set to never sleep.

**Update 5.2024.01.13.0**

Removed old apps from initial download.

**Update 5.2024.01.03.3**

I accidentally removed winget so I've readded it.

**Update 5.2024.01.03.2**

Forgot to change NanaZip URL.

**Update 5.2024.01.03.1**

Fixed typos.

**Update 5.2024.01.03.0**

Moved all the apps to our server instead of git.

**Update 5.2024.01.02.1**

Forgot quotations around URLs.

**Update 5.2024.01.02.0**

Moved app downloads to during the install rather than before to speed up initial download.

**Update 5.2023.11.03.0**

Any log files will be removed before creating a new one just incase the installer is run more than once. Correctly this time.

**Update 5.2023.10.27.0**

Buttons weren't Halloweeny enough.

**Update 5.2023.10.20.2**

Any log files will be removed before creating a new one just incase the installer is run more than once.

**Update 5.2023.10.20.1**

Once all files are cleaned up a log file will be create in C:\Computer Repair Centre\ with the installation date.

**Update 5.2023.10.20.0**

Removed unzip of cleanupo files as they're no longer zipped.

**Update 5.2023.10.19.2**

Tidied up the script that deletes files after installation so you don't get an error if Office 2007 wasn't installed.

**Update 5.2023.10.19.1**

Forgot to remove Bing files unzip during NanaZip installation.

**Update 5.2023.10.19.0**

Removed all of the old Bing Wallpaper installation files as the new method seems to work fine.

**Update 5.2023.10.18.2**

Forgot to actually add it.

**Update 5.2023.10.18.1**

Testing out new Bing Wallpapers installation method.

**Update 5.2023.10.18.0**

Fixed dark mode not activating when selected.

**Update 5.2023.10.16.1**

Moved app to our server.

**Update 5.2023.10.16.0**

Updated winget to see if that fixes recent winget issues.

**Update 5.2023.10.12.1**

Also added backup for VLC Media Player.

**Update 5.2023.10.12.0**

Added backup installers of Google Chrome, Mozilla Firefox & LibreOffice due to issues with winget.

**Update 5.2023.10.09.0**

Update winget ID as it was changed in the latest update.

**Update 5.2023.10.03.1**

Stopped the installer being orange for the whole of October.

**Update 5.2023.10.03.0**

Increased Halloween to a week before Halloween.  
Added more Christmas songs.

**Update 5.2023.09.18.2**

Fixed missing icons.

**Update 5.2023.09.18.1**

Updated Discord icon.  
Reorganised assets folder.

**Update 5.2023.09.18.0**

Added version to bottom of the main window.  
Added view changelog button at the bottom of the main window.

**Update 5.2023.09.11.2**

Fixed Discord icon.  
Changed VLC to only install on Windows 10.

**Update 5.2023.09.11.1**

Added some missing progress bar additions.

**Update 5.2023.09.11.0**

Kaspersky will no longer be installed in Romsey or Highcliffe due to no longer having the option for a trial.  
Added Discord.  
Added Steam.

**Update 5.2023.07.21.2**

Forgot to add --install switch so AnyDesk just opened.

**Update 5.2023.07.21.1**

Moved '

**Update 5.2023.07.21.0**

Changed to AnyDesk EXE instead of MSI.

**Update 5.2023.07.20.2**

Changed AnyDesk MSI location to the AnyDesk site so it's always up to date.

**Update 5.2023.07.20.1**

Changed AnyDesk installer to an MSI as the winget installer is frequently broken.  
Removed TeamViewer check from AnyDesk install as it's no longer relevant.

**Update 5.2023.07.20.0**

Added check for Microsoft Office 2007 and added loop breaks for both Office and Kaspersky after 5 minutes.

**Update 5.2023.07.18.1**

Added waits after every file extraction and added 5 minute wait to Office 2007 install.

**Update 5.2023.07.18.0**

Fixed typo in NanaZip path and added Geeth's birthday.

**Update 5.2023.07.17.1**

If Windows 7 games are installed then icons will be put on the Desktop for the games.

**Update 5.2023.07.17.0**

Replaced 7-zip with NanaZip.

**Update 5.2023.04.29.0**

If AnyDesk is selected then TeamViewer will also be removed so this can run on already installed machines to switch to AnyDesk.


**Update 5.2023.04.21.0**

Added AnyDesk and disabled TeamViewer from being selected by default.

**Update 5.2023.03.10.0**

The installer will now update the Windows Store for Windows 11 as well as 10 because Adam is forgetful.

**Update 5.2023.02.16.0**

Changed restart box icon as I don't think anyone knew what it was for.  
Changed this years Christmas song.

**Update 5.2023.01.31.0**

Added Kaspersky Standard to installer as a proper install.

**Update 5.2023.01.30.3**

Removed fix attempt 1 files.

**Update 5.2023.01.30.2**

That fixe worked so tied up check boxes and removed check box for sleep mode as who cares about carbon emissions.

**Update 5.2023.01.30.1**

That fix didn't work, fix attempt number 2.

**Update 5.2023.01.30.0**

Attemped to fix AC power sleep.

**Update 5.2023.01.24.4**

Attempted to fix TeamViewer not installing.

**Update 5.2023.01.24.3**

Amended force command.

**Update 5.2023.01.24.2**

Correct deleteFiles.ps1

**Update 5.2023.01.24.1**

If run on Windows 10 it will now update the Windows Store before installing Winget.

**Update 5.2023.01.24.0**

Added force command if a program fails to install on the retry.

**Update 5.2023.01.23.9**

Fixed typo in kaspersky download URL. It's too cold to type tonight.

**Update 5.2023.01.23.8**

Forgot to add Kaspersky move command.

**Update 5.2023.01.23.7**

Updated deleteFiles.ps1.  
Re-enabled Kaspersky.  
Added Kaspersky Standard exe temporarily until I figure out how to enable a silent install. This must be run manually after the install is complete.

**Update 5.2023.01.23.6**

Corrected typo stopping winget package from downloading.

**Update 5.2023.01.23.5**

Fixed issue with Microsoft .NET Windows Desktop Runtime 3.1.

**Update 5.2023.01.23.4**

If run on Windows 10 it will install winget and the prerequisites.  
Added Microsoft .NET Windows Desktop Runtime 3.1.  
Fixed hanging issue. 

**Update 5.2023.01.23.3**

Removed Microsoft .NET Windows Desktop Runtime 3.1.  
Cleaned up checkboxes.

**Update 5.2023.01.23.2**

Changed the way winget installs.

**Update 5.2023.01.23.1**

Added winget installation for Windows 10.

**Update 5.2023.01.23.0**

Moved from chocolatey from to winget.  
Removed Microsoft Office 2019 and 2021.  
Removed HashTab.  
Removed uBlock Origin.  
Disabled Kaspersky for now until I can work out a silent install.

**Update 4.2022.12.19.0**

Updated copyright.

**Update 4.2022.12.19.0**

Made light mode default in Chandlers Ford as Adam is old.

**Update 4.2022.12.02.4**

Added sleep after Christmas message.

**Update 4.2022.12.02.3**

Removed Callum :(
    
**Update 4.2022.12.02.2**

Remove misplaced number causing crashes.

**Update 4.2022.12.02.1**

Corrected date.

**Update 4.2022.12.02.0**

Fixed Christmas message.

**Update 4.2022.10.31.3**

Fixed Halloween message not showing up.

**Update 4.2022.10.31.2**

Added Christmas colours and message for December.

**Update 4.2022.10.31.1**

Added Halloween colours.

**Update 4.2022.10.31.0**

No longer sets "This PC" as default as the new Windows 11 home mode does the same thing.

**Update 4.2022.10.17.1**

Fixed missing ".

**Update 4.2022.10.17.0**

Reverted Office 2021 back to one file as issue wasn't with installer it was with download site cert.

**Update 4.2022.10.15.2**

Fixed Office path typo.

**Update 4.2022.10.15.1**

Fixed Office 2021 download error.  
Added progress steps for each part of Office 2021.

**Update 4.2022.10.15.0**

Split Office 2021 into 3 zips as it's so big.  
Added Office 2021 folder to the cleanup script.

**Update 4.2022.10.11.0**

Updated domains for IP check.

**Update 4.2022.10.05.0**

Stopped Office 2019 from installing on Windows 11.  
Added Office 2021.

**Update 4.2022.09.24.0**

Added Highcliffe phone number.

**Update 4.2022.09.17.0**

Attempted to fix monitor and standby timeouts not changing.

**Update 4.2022.09.14.0**

Added known Highcliffe details, still awaiting phone number.

**Update 4.2022.09.10.4**

Installer will now wait for the HP install.cmd to finish before continuing.

**Update 4.2022.09.10.3**

Moved } that was in the wrong place stopping the HP software from installing.

**Update 4.2022.09.10.2**

Changed the order in which sleep will change timeout modes to make sure they're set correctly.

**Update 4.2022.09.10.1**

Fixed two conflicting arguments.

**Update 4.2022.09.10.0**

Fixed not setting HP EliteBook status correctly.

**Update 4.2022.09.09.0**

Added HP Hotkey Support for HP EliteBooks as Steve can't install it.

**Update 4.2022.09.07.1**

Installer will now sing happy birthday.

**Update 4.2022.09.07.0**

Improved way of getting the IP.  
Added Highcliffe.  
Added Callum's birthday.

**Update 4.2022.08.04.0**

Moved Office 2007 & Kaspersky files to BRM/FFITR domain for faster download speeds.

**Update 4.2022.07.21.2**

Fixed typo.

**Update 4.2022.07.21.1**

Will now uninstall Kaspersky VPN & Secure Connection after checking if install has complete to make sure it removes just in case the install fails for any reason.

**Update 4.2022.07.21.0**

Added "Reset chocolatey" button to remove previous installations of chocolatey if needed.

**Update 4.2022.07.18.2**

Only downloads Windows 7 Games if it is selected to speed up loading the program.

**Update 4.2022.07.18.1**

Fixed Windows 7 Games check box from overlapping with Zoom.

**Update 4.2022.07.18.0**

Added Windows 7 Games as an install option.

**Update 4.2022.07.08.2**

Fixed iTunes icon.

**Update 4.2022.07.08.1**

Added birthday messages.  
Changed all checkboxes to images rather than text.

**Update 4.2022.07.08.0**

Fixed typo causing buttons to be blank.

**Update 4.2022.07.07.1**

Added dark mode.  
Modernised the GUI slightly.  
Added check boxes for all branches that can be checked and unchecked if ever needed.  
Added Highcliffe location but disabled until branch is opened.

**Update 4.2022.07.07.0**

Updated EXE to run first time every time.

**Update 3.2022.07.02.5**

Corrected progress bar.

**Update 3.2022.07.02.4**

Will now close Office 2019 installer once it has completed the install.

**Update 3.2022.07.02.3**

Added last updated line when installer is run.

**Update 3.2022.07.02.2**

Stopped the installer from deleting choco on every run.  
Will now remove CRC Installer x64.exe from Desktop on reboot.

**Update 3.2022.07.02.1**

Fixed Word path error.

**Update 3.2022.07.02.0**

Fixed issue with installer not checking for already installed programs & fixed Office 2019 activation issue.

**Update 3.2022.07.01.0**

The installer will now check if a program is already installed before running the install command to speed up the installs on machines it has been run on previously.  
Cleaned up Office 2019 install.  
Added Desktop icons for Office 2007 and 2019 installs.  
Attemped to fix progress bar but it might need some further tweaks to get the progress point correct.

**Update 3.2022.05.22.0**

Updated IP checker to check for Romsey IP instead of Chandler's Ford as the IP doesn't have to be update periodically.

**Update 3.2022.05.19.1**

Fixed darkmode overriding wallpaper selection.

**Update 3.2022.05.19.0**

Enabled dark mode by default.

**Update 3.2022.05.07.0**

Updated Kaspersky.

**Update 3.2022.05.07.0**

Updated download link for Kaspersky.  
Updated the readme to include new features I previously forgot to add.  
Corrected some typos.  
Removed taskbar pin as this currently doesn't work on Windows 11.  
uBlock Origin is now disabled by default due to some customer complaints about removing it.

**Update 3.2021.12.13.1**

Fixed progress bar.

**Update 3.2021.12.13.0**

Will now remove old installations of Chocolatey to prevent issues with programs not installing correctly if there is a crash or other issues.

**Update 3.2021.10.26.0**

Added HashTab to prerequisites for convenience.

**Update 3.2021.10.25.0**

Changed version format to keep track of when it was updated easier.  
Added 4K support to Bing Wallpapers.

**Update 3.10.12.9**

Fixed typo causing 2 Bing wallpaper tasks.

**Update 3.10.12.8**

Changed Bing wallpaper to run at 1PM everyday and at every logon.  
Corrected progress bar on installer.

**Update 3.10.12.7**

Changed all unzipping commands to overrite to avoid any future problems.

**Update 3.10.12.6**

Stopped installer from hanging on extracting zip files. I really hope this is the last one because this is boring now.

**Update 3.10.12.5**

Changed how Bing wallpapers downloads files.

**Update 3.10.12.4**

Fixed spelling error.

**Update 3.10.12.3**

Fixed zipped files not extracting in time.

**Update 3.10.12.2**

Fixed Bing wallpapers not downloading.

**Update 3.10.12.1**

Bing wallpapers now saves the files it's own folder to keep things tidy.

**Update 3.10.12.0**

Made some changes to Bing Wallpaper so it now saves the file with the date in the name so the user can keep wallpapers they like.  
Zipped up the Bing Wallpaper files to stop files from failing from downloading.

**Update 3.10.11.2**

Updated Choco.

**Update 3.10.11.1**

Fixed issue with Teams being installed with TeamViewer.

**Update 3.10.11.0**

Added MalwareBytes as install option.
Fixed Teams icon.

**Update 3.10.10.0**

Fixed Kaspersky mailing list issue.  
Installer will now check software MD5 checksums before installation.  
Added initial support for Windows 11.  
Installer now installs Microsoft Visual C++ Redistributable as part of the prerequisites.  
Added Teams as an install option.  
Removed some Windows 10 tweaks to make it run nicer for the end user.  
Added Windows 11 tweaks to match Windows 10. More to come later on release.

**Update 3.10.9.0**

Updated Kaspersky Internet Security.

**Update 3.10.8.2**

Fixed text for reboot message going off the screen.

**Update 3.10.8.1**

Added a message to indicate that the machine is recognised as a refurb unit.

**Update 3.10.8.0**

The refurb and reboot box will now be ticked if the installer is run on a HP EliteBook as these are common refurb units.

**Update 3.10.7.1**

Updated Romsey hours.

**Update 3.10.7.0**

The installer now disables the weather taskbar widget.

**Update 3.10.6.0**

Bing Wallpapers task will now run at 1AM incase user leaves the device on and will run at earliest opportunity if not.

**Update 3.10.5.0**

Installer will now delete leftover files on next startup.

**Update 3.10.4.1**

Fixed typo stopping Bing Wallpapers from working.

**Update 3.10.4.0**

Reverted Bing Wallapers to save into the Picture folder.  
Fixed issue with user selected wallpapers.

**Update 3.10.3.0**

Bing Wallpapers now saves the C Drive instead of the Pictures folder to allow slideslow mode.  
Bing Wallpapers will no longer override user selected wallpapers.

**Update 3.10.2.0**

Installer will now remove Kaspersky VPN after installing Kaspersky Internet Security.

**Update 3.10.1.4**

Stopped a Powershell window from appearing during the Bing wallpaper task.

**Update 3.10.1.3**

Fixed Zoom interfering with Bing wallpapers.

**Update 3.10.1.2**

Fixed Kaspersky Internet Security icon.

**Update 3.10.1.1**

Fixed Kaspersky Internet Security URL issue.

**Update 3.10.1.0**

Updated Kaspersky Internet Security 2021 to 21.3.10.391.

**Update 3.10.0.1**

Stopped Zoom from being checked by default.

**Update 3.10.0.0**

Added Zoom to installer options.

If the user selects Kaspersky Internet Security they are now notified that the system requires a reboot.

Updated Kaspersky Internet Security to 2021.

Licensed under Mozilla Public License 2.0.

**Update 3.9.0.2**

Fixed Bing Wallpaper resetting task every time it runs.

**Update 3.9.0.1**

Fixed issue with Bing wallpaper task not knowing what app to use.

**Update 3.9.0.0**

Updated Chandler's Ford IP address.

Removed custom wallpaper and theme installation.

Wallpaper option has been replaced with the option to install Bing wallpaper that will grab a new wallpaper everyday.

Removed wallpaper support on Windows 7.

**Update 3.8.1.1**

Removed Visual C++ Runtimes temporarily.

**Update 3.8.1.0**

Added a box to disable sleep mode on AC power.

Installer will now instal various Visual C++ Runtimes as part of the prerequisites.

**Update 3.8.0.0**

Added a reboot box that if checked will reboot the computer once the installer has finished, with a 30 second delay so the user can cancel if required.

Added a countdown timer to the reboot button if a reboot is pending to show user how much time they have until the compuer is rebooted.

**Update 3.7.2.1**

Removed quotation marks causing crashes from previous update.

**Update 3.7.2.0**

Installer will now notify you if the system needs a reboot.

**Update 3.7.1.3**

Fixed typo causing Microsoft Office 2007 to not install.

**Update 3.7.1.2**

Changed how Microsoft Office activates to hopefully prevent Windows Defender disabling it.

**Update 3.7.1.1**

Fixed Microsoft Office 2019 activation bug.

**Update 3.7.1.0**

Microsoft Office 2019 should now automatically activate.

**Update 3.7.0.2**

Added Microsoft Office 2007 icon.

**Update 3.7.0.1**

Microsoft Office 2007 will be auto selected in Chandler's Ford.

**Update 3.7.0.0**

Added Microsoft Office 2007 which will automatically activate.

**Update 3.6.2.0**

Microsoft Office 2019 should now automatically activate.

**Update 3.6.1.0**

Updated multiple icons.

**Update 3.6.0.2**

Fixed Microsoft Office icon.

**Update 3.6.0.1**

Fixed crashing issue.

**Update 3.6.0.0**

Updated for 2021, added option to install Microsoft Office 2019 Volume License and changed Chandler's Ford IP.

**Update 3.5.5.0**

Installer now enables the Action Centre and toast notifications.

**Update 3.5.4.2**

Google Chrome is now pinned again.

**Update 3.5.4.1**

Fixed link location for Kaspersky.

**Update 3.5.4.0**

Updated Kaspersky.

**Update 3.5.3.2**

Installer now leaves Cortana search bar in the taskbar. Again. Because of old people.

**Update 3.5.3.1**

Fixed issue with Kaspersky not downloading.

**Update 3.5.3.0**

Installer now leaves Cortana search bar in the taskbar.

**Update 3.5.2.0**

Added more wallpapers so installer with now randomly pick between two different wallpapers sets. Can easily add more in the future.

**Update 3.5.1.0**

Added Chocolatey to the prerequisites and stopped disabling monitor timeout on Windows 10 as it wasn't working.

**Update 3.5.0.4**

Fixed issue with default broswer sometimes not settings correctly.

**Update 3.5.0.3**

Updated download URLs from legacy to current.

**Update 3.5.0.2**

Installer now installs Microsoft .NET 3.5 and .NET 4.6.2 as a prerequisite.

**Update 3.5.0.1**

When installing prerequisites the installer will now show what is being installed.

**Update 3.5.0.0**

Removed most of the support for Windows 7, no future features will come to 7 but programs will continue to install and wallpapers will continue to be set.

Removed official support for Windows 8 & 8.1 but basic programs will continue to install.

Default browser will change to Google Chrome if installed, Mozilla Firefox if installed or Mozilla Firefox is both Chrome and Firefox are installed.

**Update 3.4.1.1**

Fixed bug causing installer to hang.

**Update 3.4.1.0**

Re-enabled taskbar pinning for Windows 10.

**Update 3.4.0.1**

Fixed missing token.

**Update 3.4.0.0**

Installer can now enable the set theme in light and dark mode depending on the dark mode selection.

**Update 3.3.5.3**

Updated Chandler's Ford IP.

**Update 3.3.5.2**

Fixed another issue with light mode. Take the hint old people.

**Update 3.3.5.1**

Fixed issue with dark mode being enabled even with box not ticked. Because old people.

Fixed issue with installer only working on second launch, it should now work on first launch.

**Update 3.3.5.0**

Disabled Windows dark mode by default because old people and updated Chandler's Ford IP.

**Update 3.3.4.0**

Updated Firefox logo.

**Update 3.3.3.1**

Added in last update date to information box.

**Update 3.3.3.0**

Updated Kaspersky Internet Security to 2020.

**Update 3.3.2.0**

Fixed issue with LibreOffice not installing correctly.

**Update 3.3.1.2**

Fixed checksum issues.

**Update 3.3.1.1**

Removed birthday messages.

**Update 3.3.1.0**

Enabled VLC by default.

**Update 3.3.0.0**

Fixed Google Chrome and Mozilla Firefox uBlock Origin installs with the new version of Windows 1903.

Temporarily disabled taskbar pining until a new solution is found for Windows 1903.

Moved night mode to its own check box so it can be disabled. Fixed theme for Windows 1903.

**Update 3.2.2.2**

Removed old installer leftover files and cleaned up sleep functions.

**Update 3.2.2.1**

Removed left over code.

**Update 3.2.2.0**

Birthday feature now says the name of whose birthday it is.

Added close button.

**Update 3.2.1.0**

Added birthday easter egg.

**Update 3.2.0.2**

Fixed wallpapers not downloading.

**Update 3.2.0.1**

Fixed Computer Repair Centre icon not downloading.

**Update 3.2.0.0**

Rewritten the executable in C++ so Windows Defender and other anti-virus software won't delete it. Added in loading form while the files download.

**Update 3.1.0.0**

The installer is now multithreaded so the GUI uses one thread and the script uses another. This speeds it up and also stops the GUI from freezing while the script is in progress. Also added am adaptive progress bar to show much progress is left.

**Update 3.0.9.11**

Fixed progress box not auto-scrolling.

**Update 3.0.9.10**

Fixed crashing issue.

**Update 3.0.9.9**

Added features from 3.0.9.8.

**Revert to 3.0.9.7**

Reverted due to mass rename of "Checked".

**Update 3.0.9.8**

Fixed some typos and added in some features for Windows 8.x.

**Update 3.0.9.7**

Reverted to 3.0.8.0 pinning.

**Update 3.0.9.6**

Changed taskbar pinning to a VBS script.

**Update 3.0.9.5**

Reverted to old way of pinning taskbar icons and re-renabled Cortana search bar for Chandler's Ford.

**Update 3.0.9.4**

Updated OEM logo.

**Update 3.0.9.3**

Fixed dark mode.

**Update 3.0.9.2**

Removed most of the build in Windows 10 tracking features.

**Update 3.0.9.1**

Fixed crashing issue.

**Update 3.0.9.0**

Added XML file to replace taskbar pinning on Windows 10.

**Update 3.0.8.5**

Removed auto-arranging Desktop icons due to locking issue.

**Update 3.0.8.4**

Fixed crashing issue.

**Update 3.0.8.3**

Installer cleans up files before downloading new files.

**Update 3.0.8.2**

Added auto-arranging Desktop, now removed Kaspersky Safe Money icon from the Desktop and updated taskbar pinning for Windows 8 and 8.1.

**Update 3.0.8.1**

Fixed formatting issue and changed icon.

**Update 3.0.8.0**

Added dark mode for Windows 10, added more wallpapers, added option to disable taskbar repin and fixed pinning icons on Windows 7.

**Update 3.0.7.4**

Replaced taskbar pin with better PowerShell script.

**Update 3.0.7.3**

Fixed paths.

**Update 3.0.7.2**

Fixed error with exe.

**Update 3.0.7.1**

Fixed crashing error.

**Update 3.0.7.0**

Fixed wallpaper icon not downloading and updated host address.

**Update 3.0.6.3**

Improved tashbar pinning.

**Update 3.0.6.2**

Fixed system pin and added Josh's birthday.

**Update 3.0.6.1**

Updated Kaspersky.

**Update 3.0.6.0**

Updated UI and moved from Kaspersky 2018 to 2019.

**Update 3.0.5.4**

Adding auto-arranging Desktop icons.

**Update 3.0.5.3**

Fixed issue with pin to taskbar not working.

**Update 3.0.5.2**

Updated URL in executable.

**Update 3.0.5.1**

Fixed URL.

**Update 3.0.5.0**

Moved to self hosted GitLab to avoid download issues.


**Update 3.0.4.2**

Improved the way uBlock Origin is installed on Firefox.

**Update 3.0.4.1**

Fixed problem with uBlock Origin icon.

**Update 3.0.4.0**

Updated UI and made interface smaller.
Added option for uBlock Origin to be installed on Mozilla Firefox and Google Chrome.

**Update 3.0.3.0**

Added Skype as install option.

**Update 3.0.2.2**

Fixed issue with Kaspersky Internet Security 2018.

**Update 3.0.2.1**

Removed progress bar due to invisible issues and added console that minimises on open.

**Update 3.0.2.0**

Added loading bar during file downloads and reverted to Kaspersky Internet Security 2018 due to Windows 10 issues.

**Update 3.0.1.1**

Removes Safe Money icon from Desktop.

**Update 3.0.1.0**

Improved the cleanup process after installation and fixing locking up issue.

**Update 3.0.0.1**

Removed testing bug and changed downloading so it now only downloads files if necessary.

**Update 3.0.0.0**

Renamed to NorthPoint Installer and moved to GitLab. Modernised the GUI and improved performace.

**Update 2.6.0**

Now unistalls Kaspersky Secure Connection after installing Kaspersky Internet Security 2019.

**Update 2.5.2**

Fixed IP issue with Chandlers Ford.

**Update 2.5.1**

LibreOffice will be checked if run from Romsey and unchecked if run from Chandlers Ford.

**Update 2.5.0**

Added Mozilla Thunderbird and iTunes. Set LibreOffice to be ticked by default.

**Update 2.4.2**

Updated Kaspersky Internet Security to 2019.

**Update 2.4.1**

Will now auto-arranges Desktop icons after the installer has finished.

**Update 2.4.0**

Deletes "Microsoft Edge" icon from the Desktop in the new April 2018 update of Windows 10.

**Update 2.3.1**

Now disables standby and monitor timeout when plugged in.

**Update 2.3.0**

Added "close.ps1" to remove all unnecessary files after installation is complete.

**Update 2.2.4**

Fixed issue with wallpapers hanging if folder already exists.

**Update 2.2.3**

Cleaned up spacing in code.

**Update 2.2.2**

Fixed birthday Easter egg.

**Update 2.2.1**

Removed orphan code.

**Update 2.2.0**

Cleaned up code, changed requisites and added more wallpapers.

**Update 2.1.2**

Added random wallpaper to checkboxes so you now have choice.

**Update 2.1.1**

Added "close.ps1" script that forces a restart of the computer.

**Update 2.1.0**

Added feature that sets random wallpaper per machine.

**Update 2.0.3**

Fixed problem with execution policy not changing.

**Update 2.0.2**

Executable now sets execution policy and fixed issue with Kaspersky Internet Kaspersky.

**Update 2.0.1**

Added Kaspersky Internet Security 2018 & changed spacing for icons.

**Update 2.0.0**

Rewritten executable to work with Windows Defender and download quicker. Removed log file, Apache OpenOffice & WPSOffice.

**Update 1.12.1**

Fixed crashing issue.

**Update 1.12.0**

Added a lot more Windows 10 Tweaks.

**Update 1.11.4**

No longer installs LibreOffice by defualt and added the new Mozilla Firefox Quantum.

**Update 1.11.3**

Removed the moving of Windows.Old folders due to permissions issue.

**Update 1.11.2**

Added sleep after moving previous "Users" folder.

**Update 1.11.1**

Fixed previous "Users" folder not being renamed.

**Update 1.11.0**

Installer will check if the programs have installed successfully and notify you of any failures.

**Update 1.10.1**

Now checks for up to 5 previous Windows installations instead of 1.

**Update 1.10.0**

Checks for previous Windows installation and then moves old documents to the desktop if it finds any.

**Update 1.9.1**

Installer now tells you local info in text box.

**Update 1.9.0**

Added Apache OpenOffice and WPS Office as install options.

**Update 1.8.7**

Fixed an issue where it should be checking IP instead of SSID.

**Update 1.8.6**

Replaced SSID check with public IP check so it now works on towers as well as laptops and rearranged installation process.

**Update 1.8.5**

Moved location check to run immediately after "Install" is pressed.

**Update 1.8.4**

Added form that lets you select your location if it can't find it.

**Update 1.8.3**

Added feature to find IP and then install the correct OEM information if Wi-Fi is nto available.

**Update 1.8.2**

Fixed crashing issue with Wi-Fi searching.

**Update 1.8.1**

Improved taskbar pinning on Windows 7, 8, and 8.1. Reintroduced taskbar pinning for Windows 10.

**Update 1.8.0**

Added feature that auto detects current Wi-Fi network, eliminating the need for multiple installers.

**Update 1.7.5**

Removed commands that disable some telememtary and Cortana.

**Update 1.7.4**

Fixed incorrect wording when installing Mozilla Firefox.

**Update 1.7.3**

Fixed command prompt not going invisible on Romsey installer and changed wording for installation.

**Update 1.7.2**

Changed directory for icon file.

**Update 1.7.1**

Updated exe to include icon and loading animation.

**Update 1.7.0**

Added loading form so you know the program is loading.

**Update 1.6.0**

Made command line invisible for sleaker look.

**Update 1.5.6**

Fixed crashing issue on Windows 7.

**Update 1.5.5**

Fixed problem with Kaspersky and TeamViewer not installing.

**Update 1.5.4**

Fixed crashing problem.

**Update 1.5.3**

Fixed issue with Documents folder.

**Update 1.5.2**

Fixed file cleanup on completion.

**Update 1.5.1**

Removed extra lines for downloading files.

**Update 1.5.0**

Temporarily removed automatically setting taskbar icons in Windows 10 due to issue and added KIS Chocolatey package.

**Update 1.4.1**

Fixed OEM website HTTPS information.

**Update 1.4.0**

Added autoscrolling feature to list box and reduced size of install window.

**Update 1.3.0**

Renamed to CRC Installer and added new Romsey version.

**Update 1.2.1**

Fixed issue with Google Chrome checksums.

**Update 1.2.0**

Replaced text with icon of program for cleaner look.

**Update 1.1.3**

Fixed crashing issue with icon file.

**Update 1.1.2**

Removed exit button and will now close 15 seconds after the install has finished.

**Update 1.1.1**

Fixed issue with downloading CleanCF.ps1.

**Update 1.1.0**

Programs now install in alphabetical order and added an exit button than cleans up temporary files.
