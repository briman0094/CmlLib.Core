﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using ICSharpCode.SharpZipLib.Zip;
using CmlLib.Utils;

namespace CmlLib.Launcher
{
    public class MForge
    {
        const string MavenServer = "https://files.minecraftforge.net/maven/net/minecraftforge/forge/";

        public MForge()
        {

        }

        public event DownloadFileChangedHandler FileChanged;

        public void InstallForge(string mcversion, string forgeversion)
        {
            var versionname = $"{mcversion}-forge{mcversion}-{forgeversion}";
            var manifest = Path.Join(
                Minecraft.Versions,
                versionname,
                versionname + ".json"
            );

            var installer = $"${MavenServer}${mcversion}-${forgeversion}/forge-${mcversion}-${forgeversion}-installer.jar";

            var jsondata = new StringBuilder();
            var res = WebRequest.Create(installer).GetResponse().GetResponseStream();
            using (res)
            using (var s = new ZipInputStream(res))
            {
                ZipEntry e;
                while ((e = s.GetNextEntry()) != null)
                {
                    if (!e.IsFile || e.Name != "install_profile.json")
                        continue;

                    var buffer = new byte[1024];
                    while (true)
                    {
                        int size = s.Read(buffer, 0, buffer.Length);
                        if (size == 0)
                            break;

                        jsondata.Append(Encoding.UTF8.GetString(buffer, 0, size));
                    }
                }
            }

            if (jsondata.Length == 0)
                throw new Exception("can't find profile file");

            var libraries = JObject.Parse(jsondata.ToString())["versionInfo"];

            FileChanged?.Invoke(new DownloadFileChangedEventArgs()
            {
                FileKind = MFile.Library,
                FileName = "universal",
                ProgressedFileCount = 0,
                TotalFileCount = 1
            });

            var universalUrl = $"{MavenServer}{mcversion}-{forgeversion}/forge-{mcversion}-{forgeversion}-universal.jar";

            var dest = Path.Join(
                Minecraft.Library,
                "net",
                "minecraftforge",
                "forge",
                $"{mcversion}-{forgeversion}",
                $"forge-{mcversion}-{forgeversion}.jar"
            );

            Directory.CreateDirectory(Path.GetDirectoryName(dest));
            var downloader = new WebDownload();
            downloader.DownloadFile(universalUrl, dest);

            Directory.CreateDirectory(Path.GetDirectoryName(manifest));
            File.WriteAllText(manifest, libraries.ToString());

            FileChanged?.Invoke(new DownloadFileChangedEventArgs()
            {
                FileKind = MFile.Library,
                FileName = "universal",
                ProgressedFileCount = 1,
                TotalFileCount = 1
            });
        }
    }
}
