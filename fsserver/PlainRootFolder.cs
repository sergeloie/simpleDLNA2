using System.IO;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class PlainRootFolder : PlainFolder
  {
    private readonly string title;

    // Корневая папка одного каталога-источника.
    internal PlainRootFolder(FileServer server, DirectoryInfo di)
      : this(server, di, null, true)
    {
    }

    // Корневая папка, вложенная в виртуальный корень при нескольких источниках.
    internal PlainRootFolder(FileServer server, DirectoryInfo di, string title,
      bool isMaster)
      : base(server, null, di)
    {
      Id = isMaster ? Identifiers.GENERAL_ROOT : null;
      this.title = string.IsNullOrWhiteSpace(title) ? di.Name : title;
    }

    public override string Title => title;
  }
}
