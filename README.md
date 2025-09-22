# Overview
This project is a redesign of [GPlus](https://github.com/MiniHood/G-Plus) and a retry of the [V2](https://github.com/MiniHood/G-Plus/tree/v2) branch. GPlus V2 is a .NET attempt at creating a hub for multi client testing on Garry's Mod. The project can also be used specifically for 'botting' servers to increase player count. The project uses Sandboxie, steam and Garry's Mod to create real connections to servers. The tool allows you to control each client, including allowing you to run lua on all or specific clients and connect to servers.  
## Disclaimer
This project is not completed and is still in it's development stage. This will not work as expected until this disclaimer is removed. Keep up to date through our [discord](https://discord.gg/nSynjcDwTF).

I'm sure this doesn't need to be said, but anything you do with this tool is completely on you. Don't be stupid and don't be malicious.
## Installation
### Windows GUI
Installation can be done through installing and extracting the newest [release](https://github.com/MiniHood/G-Plus/releases)
## Setup
### Windows GUI
#### Requirements
- [Steam](https://steamcommunity.com/)
- [Sandboxie Plus](https://sandboxie-plus.com/)
- [.NET 6.0+](https://dotnet.microsoft.com/en-us/)
- A working internet connection (fast is preferred but not needed)
- One or multiple steam accounts with a legitmate [Garry's Mod](https://store.steampowered.com/app/4000/Garrys_Mod/) copy.
- Windows 10+ (Any below has not and will not be tested)
If you meet the requirements above, install the application and run it. It will download a local copy of SteamCMD with the inputted details.
## Build
### Windows GUI
#### General Requirements
You must have a copy of Visual Studio 2020+
#### Required Packages
- `Install-Package CoreRCON -Version 5.4.2`
- `Install-Package Newtonsoft.Json -Version 13.0.4`
- `Install-Package ReaLTaiizor -Version 3.8.1.3`
- `Install-Package System.Management -Version 9.0.9`
- `Install-Package System.ServiceProcess.ServiceController -Version 9.0.9`

Packages should automatically install, however if they don't use the above commands.
## Support
Support can be received through the [Plus Studios discord](https://discord.gg/nSynjcDwTF).
## Authors & Special Thanks

- [@HowNiceOfYou](https://github.com/MiniHood)
