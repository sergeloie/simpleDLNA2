# Changelog

All notable changes to this fork are documented here.

## 2026-11-20 (release packaging)

- Stopped tracking compiled binaries in `build/`; added `build/` and `publish/` to `.gitignore`.
- Ready-to-run Windows x64 archives are now published as
  [GitHub Releases](https://github.com/sergeloie/simpleDLNA2/releases): a self-contained console
  server and a smaller framework-dependent archive with both console server and GUI.
- READMEs now link to the latest release.

## 2026-11-20

### Added

- Separate display of source directories in the DLNA root when more than one directory is
  supplied. Each directory becomes its own folder; `--merge-dirs` restores the legacy merged
  tree. `--seperate` (one DLNA server per directory) is unchanged.
- English `README.md` and Russian `README.ru.md`, links to the original projects.
- `Directory.Build.props` with shared build settings for all projects.

### Changed

- Migrated all 7 projects from legacy `.csproj` / `packages.config` to SDK-style projects,
  `net8.0-windows`, x64, C# latest.
- Updated NuGet packages: log4net 3.6.0, TagLibSharp 2.3.0, GetOptNet 4.0.8,
  System.Data.SQLite.Core 1.0.119, Microsoft.IO.RecyclableMemoryStream 4.0.1,
  System.Configuration.ConfigurationManager 8.0.0.
- WinForms DPI awareness is now configured via `ApplicationHighDpiMode=PerMonitorV2`;
  added an application manifest with long-path support and modern Windows declarations.
- Fixed assembly version wildcards (`1.2.*` is no longer supported by SDK-style projects).
- Rebuilt `build/sdlna.exe` and dependencies with the published .NET 8 output.

### Fixed

- Console startup crashed when output was redirected (e.g. running as a service via nssm):
  `Console.TreatControlCAsInput` is no longer touched on .NET 8.
- `StreamPump` used `Delegate.BeginInvoke`, which throws `PlatformNotSupportedException` on
  .NET 8; replaced with `ThreadPool.QueueUserWorkItem`.
- Removed obsolete `Socket.UseOnlyOverlappedIO` and `Assembly.CodeBase` usages.

### Removed

- Obsolete setup/installer classes (`NgenInstaller`, `PathEnvironmentInstaller`) and
  `System.Configuration.Install` dependencies.
- Unused EntityFramework 6 references and .NET Framework bootstrapper configuration.

## Upstream

- Based on [vaginessa/simpleDLNA2](https://github.com/vaginessa/simpleDLNA2).
- Originally [nmaier/simpleDLNA](https://github.com/nmaier/simpleDLNA) by Nils Maier.
