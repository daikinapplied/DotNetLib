# DotNetLib Projects
The following specific projects are available, used to create corresponding NuGet packages for NuGet.org.  Read their associated readme file for specific details.

| Library                                                                    | Framework            | NuGet Package and Specific Readme Information                                                                                                                                                                                                                                                                           |
|----------------------------------------------------------------------------|----------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
|<img src="Daikin.DotNetLib.Application/Images/application.png" width="64"/> | .NET Standard 2.0    | [![NuGet](https://img.shields.io/nuget/v/Daikin.DotNetLib.Application.svg)](https://www.nuget.org/packages/Daikin.DotNetLib.Application) [Daikin.DotNetLib.Application](Daikin.DotNetLib.Application/README.md) has basic functions to generically assist with applications                                             |
|<img src="Daikin.DotNetLib.Data/Images/data.png" width="64"/>               | .NET Standard 2.0    | [![NuGet](https://img.shields.io/nuget/v/Daikin.DotNetLib.Data.svg)](https://www.nuget.org/packages/Daikin.DotNetLib.Data) [Daikin.DotNetLib.Data](Daikin.DotNetLib.Data/README.md) contains basic classes to assist with specific data types                                                                           |
|<img src="Daikin.DotNetLib.DotNetNuke/Images/dotnetnuke.png" width="64"/>   | .NET Framework 4.7.2 | [![NuGet](https://img.shields.io/nuget/v/Daikin.DotNetLib.DotNetNuke.svg)](https://www.nuget.org/packages/Daikin.DotNetLib.DotNetNuke) [Daikin.DotNetLib.DotNetNuke](Daikin.DotNetLib.DotNetNuke/README.md) supports Module Development on DNN (formerly DotNetNuke), the C# open source Content Management System (CMS)|
|<img src="Daikin.DotNetLib.Facebook/Images/facebook.png" width="64"/>       | .NET Standard 2.0    | [![NuGet](https://img.shields.io/nuget/v/Daikin.DotNetLib.Facebook.svg)](https://www.nuget.org/packages/Daikin.DotNetLib.Facebook) [Daikin.DotNetLib.Facebook](Daikin.DotNetLib.Facebook/README.md) supports Facebook API calls to pull content for a single source of information                                      |
|<img src="Daikin.DotNetLib.MsTeams/Images/teams.png" width="64"/>           | .NET Standard 2.0    | [![NuGet](https://img.shields.io/nuget/v/Daikin.DotNetLib.MsTeams.svg)](https://www.nuget.org/packages/Daikin.DotNetLib.Teams) [Daikin.DotNetLib.MsTeams](Daikin.DotNetLib.MsTeams/README.md) are helpers for Microsoft Teams integration                                                                               |
|<img src="Daikin.DotNetLib.Network/Images/network.png" width="64"/>         | .NET Standard 2.0    | [![NuGet](https://img.shields.io/nuget/v/Daikin.DotNetLib.Network.svg)](https://www.nuget.org/packages/Daikin.DotNetLib.Network) [Daikin.DotNetLib.Network](Daikin.DotNetLib.Network/README.md) are helpers for making Web Api calls (typically JSON-based), SMTP, Networking, etc. calls                               |
|<img src="Daikin.DotNetLib.Security/Images/security.png" width="64"/>       | .NET Standard 2.0    | [![NuGet](https://img.shields.io/nuget/v/Daikin.DotNetLib.Security.svg)](https://www.nuget.org/packages/Daikin.DotNetLib.Security) [Daikin.DotNetLib.Security](Daikin.DotNetLib.Security/README.md) provides security helper to make it easier to encrypt and decrypt data                                              |
|<img src="Daikin.DotNetLib.Serial/Images/serial.png" width="64"/>           | .NET Framework 4.7.2 | [![NuGet](https://img.shields.io/nuget/v/Daikin.DotNetLib.Serial.svg)](https://www.nuget.org/packages/Daikin.DotNetLib.Serial) [Daikin.DotNetLib.Serial](Daikin.DotNetLib.Serial/README.md) are Windows-based USB and Serial port helpers for Windows-based applications                                                |
|<img src="Daikin.DotNetLib.Serilog/Images/serilog.png" width="64"/>         | .NET Standard 2.0    | [![NuGet](https://img.shields.io/nuget/v/Daikin.DotNetLib.Serilog.svg)](https://www.nuget.org/packages/Daikin.DotNetLib.Serilog) [Daikin.DotNetLib.Serilog](Daikin.DotNetLib.Serilog/README.md) are contains helper functions to assist with Serilog logging usage, especially with Microsoft and Azure SQL Server      |
|<img src="Daikin.DotNetLib.Windows/Images/windows.png" width="64"/>         | .NET Framework 4.7.2 | [![NuGet](https://img.shields.io/nuget/v/Daikin.DotNetLib.Windows.svg)](https://www.nuget.org/packages/Daikin.DotNetLib.Windows) [Daikin.DotNetLib.Windows](Daikin.DotNetLib.Windows/README.md) are helpers for Windows-based applications                                                                              |
|<img src="Daikin.DotNetLib.Core.Tests/Images/debugging.png" width="64"/>    | .NET 6               | [Daikin.DotNetLib.Core.Tests](Daikin.DotNetLib.Core.Tests/README.md) are .NET Core xUnit Tests, serves as coding samples                                                                                                                                                                                                |

# Introduction
<img src="Images/DotNetLib.png" width="96" align="right" alt="DotNetLib Logo"/>
These .NET Standard, Core, and Framework libraries are a collection of work intended to share (give back), collaborate, and learn.

# Getting Started
This presumes a strong understanding of the following tools and technologies, and that they are installed.

- Visual Studio 2022 (Community, Professional, or Enterprise)
- C# development
- Web API (for specific projects)
- Graph API (for specific projects)
- Code Signing Certificate must be installed to digitally sign NuGet packages, PowerShell scripts, and DLLs.
- NuGet packages (nupkg) must be digitally signed before deploying to NuGet.org.  See the PowerShell script *.\PublishNuGet.ps1*.  This script can be run within the Package Manager Console in Visual Studio.
- Signtool.exe (see below)

## Digital Signing via Signtool.exe
Signtool.exe is installed as part of Visual Studio 2019 and 2017 via *C:\Program Files (x86)\Microsoft SDKs\ClickOnce\SignTool*.  This tool is available via the Windows 10 SDK.

All of the projects use the following Post Build Event (e.g., Project Properties > *Build Events* > *Post-built event command line*):

	codesign.bat "$(TargetPath)"

Create a batch file called *codesign.bat*.  

In the *codesign.bat* file, include something like:

	@ECHO OFF
	REM Change the Subject to your own Code-Signing Certificate that is installed in your Local Machine Store
	SET subject=Daikin Applied Americas Inc.
	SET filespec=%~1
	IF /I "%filespec:~-5%"=="nupkg" GOTO NuGetSign
	:SignTool
	"C:\Program Files (x86)\Microsoft SDKs\ClickOnce\SignTool\signtool.exe" sign /sm /n "%subject%" /t "http://timestamp.verisign.com/scripts/timstamp.dll" "%filespec%"
	GOTO Done
	:NuGetSign
	REM Get NuGet CommandLine from https://www.nuget.org/downloads
	nuget sign "%filespec%" -CertificateStoreLocation "LocalMachine" -CertificateSubjectName "%subject" -TimeStamper "http://timestamp.comodoca.com?td=sha256"
	GOTO Done
	:Done
	ECHO.

This solution handles executables (exe files), dynamic link libraries (dll files), PowerShell (ps1 files), and NuGet Packages (nupkg files), making it very versitile by calling *codesign &lt;yourfile&gt;*.  As long as every developer has *codesign.bat* setup, they can all compile this project using a desired code signing certificate.

Add *codesign.bat* to your Windows System PATH:
1. Open Control Panel (Start > type *control panel*)
2. Open the *System* control panel item
3. Select *Advanced system settings* on the left pane
4. Click on *Environmental Variables* button in the *Advanced* tab (open by default)
5. In the System variable section, locate the PATH environment variable.  Add to the front the full path to *codesign.bat*.

After updating the PATH, it is recommended you restart your computer or restart all PowerShell, Command Prompt, and applications so they are able to access the updated path.

# Contribution
While primarily (at this point) done by the [Daikin Applied](https://www.daikinapplied.com) Web Development Team, all .NET developers are welcome to strengthen these libraries. 

Original development contribution by [Hans Dickel](https://www.recursivegeek.com) as a way to give back to the development community and share his geekiness and passion for the Microsoft development stack.

# License
This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).

~ End ~
