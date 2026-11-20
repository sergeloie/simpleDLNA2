# SimpleDLNA — modernized

A simple, zero-config DLNA media server for Windows. Point it at your media folders and it
becomes available on your TV and other UPnP/DLNA clients right away.

Русская версия: [README.ru.md](README.ru.md)

## Origins and credits

| Project | Role |
| --- | --- |
| [nmaier/simpleDLNA](https://github.com/nmaier/simpleDLNA) | Original project by Nils Maier (BSD-2-Clause) |
| [vaginessa/simpleDLNA2](https://github.com/vaginessa/simpleDLNA2) | Repository this fork was cloned from |
| [sergeloie/simpleDLNA2](https://github.com/sergeloie/simpleDLNA2) | This fork: .NET 8 port and ongoing maintenance |

Thanks to Nils Maier for the original server. All downstream changes are described in
[CHANGELOG.md](CHANGELOG.md).

## Features

- DLNA/UPnP SSDP discovery and HTTP media streaming; no configuration needed.
- Browse by folders or by virtual views (`music`, `bytitle`, `series`, `dimension`, `filter`,
  `new`, `flatten`, ...), with natural sorting and filtering.
- SQLite metadata cache, background cover/thumbnail caching, optional ffmpeg integration.
- **Multiple source directories are shown as separate folders in the root** (instead of being
  merged into one tree). Legacy behaviour is available via `--merge-dirs`.
- Two front ends: a console server (`sdlna`) and a WinForms tray GUI (`SimpleDLNA`).
- Per-IP, per-MAC and per-User-Agent authorization.

## Requirements

- Windows x64.
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) to run the
  framework-dependent binaries in `build/`.
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) to build from source.
- ffmpeg is optional (used for video thumbnails only).

## Build

```bat
dotnet restore sdlna.sln
dotnet build sdlna.sln -c Release
```

Executables:

- `sdlna/bin/Release/net8.0-windows/sdlna.exe` — console server
- `SimpleDLNA/bin/Release/net8.0-windows/SimpleDLNA.exe` — GUI

Publish a ready-to-copy folder (framework-dependent) into `build/`:

```bat
dotnet publish sdlna/sdlna.csproj -c Release -o build
dotnet publish SimpleDLNA/SimpleDLNA.csproj -c Release -o SimpleDLNA/publish
```

Fully self-contained build (no .NET runtime required on the target machine):

```bat
dotnet publish sdlna/sdlna.csproj -c Release -r win-x64 --self-contained true -o publish/sdlna
```

## Usage

```bat
sdlna.exe [OPTIONS] <Directory>...
```

Run `sdlna.exe --help` for the full list. The most useful options:

| Option | Meaning |
| --- | --- |
| `-p, --port PORT` | Listening port; `0` picks a free one |
| `-c, --cache FILE` | SQLite metadata cache file |
| `-n, --name NAME` | Friendly server name |
| `-t, --type TYPES` | Media types to serve (`video`, `image`, `audio`, `all`) |
| `-v, --view VIEW` | Apply a virtual view (repeatable) |
| `-s, --sort ORDER` | Sort order (see `--list-sort-orders`) |
| `--no-rescanning` | Do not re-scan directories after the first pass |
| `--seperate` | Mount every directory as a separate DLNA server (separate name and UUID) |
| `--merge-dirs` | Merge all source directories into one tree (legacy behaviour) |
| `-l, --log-level LEVEL` | `OFF`, `DEBUG`, `INFO`, `WARN`, `ERROR`, `FATAL` |
| `--log-file FILE` | Also write logs to a file |

Example:

```bat
sdlna.exe -c cache.db -p 0 C:\Media\Movies C:\Media\Series
```

Running as a Windows service with [nssm](https://nssm.cc/):

```bat
nssm install SimpleDLNA "C:\path\sdlna.exe" "-c C:\path\cache.db C:\Media"
```

### Multiple source directories

With two or more directories the root contains one folder per source directory, so the TV shows
`Movies` and `Series` separately. Pass `--merge-dirs` to get the old mixed tree.
`--seperate` is different: it starts one DLNA server per directory.

## FAQ: audio track names on the TV

The server cannot rename audio tracks in the TV's track-switch menu. That menu is built by the
TV's own player from the media file's stream metadata (the `title` tag of each audio stream).
DLNA/DIDL has no field for per-stream captions; the server only sends one `<res>` element and
the file bytes.

If `title` is already set but the TV still shows only "language + codec", the limitation is in
the TV firmware. Changing the tag is a property of the file, not something the server can do.
Tools:

```bat
mkvmerge -o out.mkv --track-name 1:"Russian (AVO)" in.mkv
ffmpeg -i in.mkv -map 0 -c copy -metadata:s:a:0 title="Russian (AVO)" out.mkv
```

## Compatibility note

The SQLite cache format is inherited from the original project and uses `BinaryFormatter`.
It is explicitly enabled via `EnableUnsafeBinaryFormatterSerialization` so old cache files keep
working. Delete the cache file if you want a clean rebuild of metadata.

## License

BSD 2-Clause. See [LICENSE](LICENSE). Third-party packages keep their own licenses (log4net —
Apache-2.0, TagLibSharp — LGPL-2.1-only, GetOptNet — MIT, System.Data.SQLite — public domain).
