using System;
using System.IO;
using System.Linq;
using System.Threading;
using Rig;

class MinerListReader
{
    public MinerListReader(IMinerslist data)
    {
        string folderName = System.IO.Path.GetFullPath(System.IO.Path.Combine(RigEx.MainFolderPath, "Shortcut"));
        
        if (!System.IO.Directory.Exists(folderName))
        {
            System.IO.Directory.CreateDirectory(folderName);
            RigEx.WriteLineColors($"put any miner shortcuts in the folder\n\t\t\t{folderName}".AddTimeStamp(),ConsoleColor.Red);
        }
        var Documents = System.IO.Directory.GetFiles(folderName,"*.lnk", SearchOption.AllDirectories);
        if (Documents == null || !Documents.Any())
        {
            RigEx.WriteLineColors($"not found any shortcuts in\n\t\t\t{folderName}".AddTimeStamp(),ConsoleColor.Red);
        }
        RigEx.WriteLineColors($"found:".AddTimeStamp(), ConsoleColor.DarkCyan);
        foreach (var document in Documents)
        {
            var minerInfo = new MinerInfo(GetFileName(document), document);
            RigEx.WriteLineColors($"name:{minerInfo.Name}".AddTimeStamp(),ConsoleColor.DarkCyan);
            data.MinersList.Add(minerInfo);
        }
    }

    private string GetFileName(string documentPath)
    {
        return Path.GetFileNameWithoutExtension(documentPath);
    }
}

