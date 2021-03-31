﻿using CmlLib.Core.Files;
using CmlLib.Utils;
using System;
using System.IO;

namespace CmlLib.Core
{
    public class MNative
    {
        public MNative(MinecraftPath _path, MVersion _version)
        {
            version = _version;
            gamePath = _path;
        }

        MVersion version;
        MinecraftPath gamePath;

        public string ExtractNatives()
        {
            var path = gamePath.GetNativePath(version.Id);
            Directory.CreateDirectory(path);

            foreach (var item in version.Libraries)
            {
                try
                {
                    if (item.IsRequire && item.IsNative)
                    {
                        var zPath = Path.Combine(gamePath.Library, item.Path);
                        if (File.Exists(zPath))
                        {
                            var z = new SharpZip(zPath);
                            z.Unzip(path);
                        }
                    }
                }
                catch { }
            }

            return path;
        }

        public void CleanNatives()
        {

            try
            {
                var path = gamePath.GetNativePath(version.Id);
                DirectoryInfo di = new DirectoryInfo(path);

                if (!di.Exists)
                    return;

                foreach (var item in di.GetFiles())
                {
                    item.Delete();
                }
            }
            catch { }
        }
    }
}
