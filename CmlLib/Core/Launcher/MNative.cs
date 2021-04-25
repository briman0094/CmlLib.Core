﻿using CmlLib.Core.Version;
using CmlLib.Utils;
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

        private readonly MVersion version;
        private readonly MinecraftPath gamePath;

        public string ExtractNatives()
        {
            string path = gamePath.GetNativePath(version.Id);
            Directory.CreateDirectory(path);

            foreach (var item in version.Libraries)
            {
                try
                {
                    if (item.IsRequire && item.IsNative)
                    {
                        string zPath = Path.Combine(gamePath.Library, item.Path);
                        if (File.Exists(zPath))
                        {
                            var z = new SharpZip(zPath);
                            z.Unzip(path);
                        }
                    }
                }
                catch
                {
                    // ignore invalid native library file
                }
            }

            return path;
        }

        public void CleanNatives()
        {

            try
            {
                string path = gamePath.GetNativePath(version.Id);
                DirectoryInfo di = new DirectoryInfo(path);

                if (!di.Exists)
                    return;

                foreach (var item in di.GetFiles())
                {
                    item.Delete();
                }
            }
            catch
            {
                // ignore exception
                // will be overwriten to new file
            }
        }
    }
}
