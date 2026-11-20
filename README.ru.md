# SimpleDLNA — современная сборка

Простой DLNA-медиасервер для Windows без настроек: указываете папки с медиа — и он сразу
появляется на телевизоре и других UPnP/DLNA-клиентах.

English version: [README.md](README.md)

## Происхождение и благодарности

| Проект | Роль |
| --- | --- |
| [nmaier/simpleDLNA](https://github.com/nmaier/simpleDLNA) | Оригинальный проект Нильса Майера (BSD-2-Clause) |
| [vaginessa/simpleDLNA2](https://github.com/vaginessa/simpleDLNA2) | Репозиторий, с которого склонирован этот форк |
| [sergeloie/simpleDLNA2](https://github.com/sergeloie/simpleDLNA2) | Этот форк: порт на .NET 8 и дальнейшая поддержка |

Спасибо Нильсу Майеру за исходный сервер. Все последующие изменения перечислены в
[CHANGELOG.md](CHANGELOG.md).

## Возможности

- Обнаружение по DLNA/UPnP SSDP и потоковая передача по HTTP, без конфигурации.
- Просмотр по папкам или виртуальным представлений (`music`, `bytitle`, `series`, `dimension`,
  `filter`, `new`, `flatten` и др.), естественная сортировка и фильтрация.
- SQLite-кэш метаданных, фоновое кэширование обложек и миниатюр, опциональное использование ffmpeg.
- **Несколько каталогов-источников отображаются отдельными папками в корне**, а не сливаются в
  одно дерево. Старое поведение включается флагом `--merge-dirs`.
- Два интерфейса: консольный сервер (`sdlna`) и GUI на WinForms (`SimpleDLNA`).
- Ограничение доступа по IP, MAC и User-Agent.

## Требования

- Windows x64.
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) для запуска
  готовых бинарников из `build/`.
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) для сборки из исходников.
- ffmpeg не обязателен — используется только для миниатюр видео.

## Сборка

```bat
dotnet restore sdlna.sln
dotnet build sdlna.sln -c Release
```

Исполняемые файлы:

- `sdlna/bin/Release/net8.0-windows/sdlna.exe` — консольный сервер
- `SimpleDLNA/bin/Release/net8.0-windows/SimpleDLNA.exe` — GUI

Публикация готовой папки (зависит от установленного .NET) в `build/`:

```bat
dotnet publish sdlna/sdlna.csproj -c Release -o build
dotnet publish SimpleDLNA/SimpleDLNA.csproj -c Release -o SimpleDLNA/publish
```

Полностью автономная сборка (на целевом компьютере .NET не нужен):

```bat
dotnet publish sdlna/sdlna.csproj -c Release -r win-x64 --self-contained true -o publish/sdlna
```

## Запуск

```bat
sdlna.exe [OPTIONS] <Directory>...
```

Полный список параметров: `sdlna.exe --help`. Основные:

| Параметр | Назначение |
| --- | --- |
| `-p, --port PORT` | Порт прослушивания; `0` — выбрать свободный |
| `-c, --cache FILE` | Файл SQLite-кэша метаданных |
| `-n, --name NAME` | Дружелюбное имя сервера |
| `-t, --type TYPES` | Типы медиа: `video`, `image`, `audio`, `all` |
| `-v, --view VIEW` | Применить виртуальное представление (можно повторять) |
| `-s, --sort ORDER` | Порядок сортировки (см. `--list-sort-orders`) |
| `--no-rescanning` | Не сканировать каталоги повторно после первого прохода |
| `--seperate` | Поднять отдельный DLNA-сервер на каждый каталог (свои имя и UUID) |
| `--merge-dirs` | Объединить все каталоги в одно дерево (старое поведение) |
| `-l, --log-level LEVEL` | `OFF`, `DEBUG`, `INFO`, `WARN`, `ERROR`, `FATAL` |
| `--log-file FILE` | Дополнительно писать логи в файл |

Пример:

```bat
sdlna.exe -c cache.db -p 0 C:\Media\Movies C:\Media\Series
```

Запуск как службы Windows через [nssm](https://nssm.cc/):

```bat
nssm install SimpleDLNA "C:\path\sdlna.exe" "-c C:\path\cache.db C:\Media"
```

### Несколько каталогов-источников

При двух и более каталогах в корне создаётся по одной папке на каждый источник — телевизор
показывает `Movies` и `Series` раздельно. Флаг `--merge-dirs` возвращает старое «смешанное»
дерево. `--seperate` — другой режим: отдельный DLNA-сервер на каждый каталог.

## Частый вопрос: названия звуковых дорожек на телевизоре

Сервер не может переименовать дорожки в меню переключения телевизора. Это меню строит сам плеер
телевизора из метаданных самого медиафайла — тега `title` у каждого аудиопотока. В DLNA/DIDL нет
поля для подписей отдельных потоков: сервер отдаёт один элемент `<res>` и байты файла.

Если `title` уже заполнен, а телевизор всё равно показывает только «язык + кодек», ограничение
на стороне прошивки телевизора — со стороны сервера это не исправить. Изменить тег можно только
в самом файле, например:

```bat
mkvmerge -o out.mkv --track-name 1:"Русский (AVO)" in.mkv
ffmpeg -i in.mkv -map 0 -c copy -metadata:s:a:0 title="Русский (AVO)" out.mkv
```

## Совместимость

Формат SQLite-кэша унаследован от оригинального проекта и использует `BinaryFormatter`.
Он явно включён через `EnableUnsafeBinaryFormatterSerialization`, поэтому старые файлы кэша
продолжают читаться. Для чистого пересбора метаданных удалите файл кэша.

## Лицензия

BSD 2-Clause, см. [LICENSE](LICENSE). Сторонние пакеты сохраняют свои лицензии (log4net —
Apache-2.0, TagLibSharp — LGPL-2.1-only, GetOptNet — MIT, System.Data.SQLite — public domain).
