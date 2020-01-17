# Introduction
<img src="Images/dotnetnuke.png" width="64" align="right" alt="Daikin.DotNetLib.DotNetNuke Logo"/>
This is a .NET Framework 4.5 library to support DNN (formerly known as DotNetNuke) Content Management System C# Module Development.  This serves as a centralized library, with a NuGet package being available to the custom modules, whether MVC or Webforms.

While currently using an older .NET Framework Library, for maximum reach, this version is used until there is a need to upgrade.  This allows maximum coverage of hosting providers, many of which still don't support the latest version, or do so with additional fees.

# Getting Started
Use the library to access helpers in the following areas:

- Save Settings at a Portal, Module, and/or User level (use for module management).  Set the cache duration for a setting saved.
- Set and clear application cache settings.
- Build responsive Tables, clearing the rows while keeping the header.

# Saving Settings 
This is a customized Master Setting library that allows saving settings, typically for Modules, at the desired resolution such as at the Module level, Portal level, or User level (or combinations therein).

For example, you may wish to have a setting that is at the Port level so that all modules pull that setting.  This is especially usefule when having modules contain integration information that is independent of a module being placed on a page (or multiple instances of the same module on a page).

To use, check out the Settings class.  The VerifyStorage() method should be called to create the necessary table and stored procedure in the DNN database.

# Documentation Overview
Check out the primary [README.md](../README.md) documentation.

~ end ~
