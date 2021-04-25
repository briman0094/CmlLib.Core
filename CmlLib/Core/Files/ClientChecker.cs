﻿using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using CmlLib.Utils;
using System.Threading.Tasks;

namespace CmlLib.Core.Files
{
    public sealed class ClientChecker : IFileChecker
    {
        public event DownloadFileChangedHandler ChangeFile;

        public bool CheckHash { get; set; } = true;

        public DownloadFile[] CheckFiles(MinecraftPath path, MVersion version)
        {
            ChangeFile?.Invoke(new DownloadFileChangedEventArgs(MFile.Minecraft, version.Jar, 1, 0));
            DownloadFile result = CheckClientFile(path, version);
            ChangeFile?.Invoke(new DownloadFileChangedEventArgs(MFile.Minecraft, version.Jar, 1, 1));

            if (result == null)
                return null;
            else
                return new DownloadFile[] { result };
        }

        public async Task<DownloadFile[]> CheckFilesTaskAsync(MinecraftPath path, MVersion version)
        {
            ChangeFile?.Invoke(new DownloadFileChangedEventArgs(MFile.Minecraft, version.Jar, 1, 0));
            DownloadFile result = await Task.Run(() => CheckClientFile(path, version));
            ChangeFile?.Invoke(new DownloadFileChangedEventArgs(MFile.Minecraft, version.Jar, 1, 1));

            if (result == null)
                return null;
            else
                return new DownloadFile[] { result };
        }

        private DownloadFile CheckClientFile(MinecraftPath path, MVersion version)
        {
            if (string.IsNullOrEmpty(version.ClientDownloadUrl)) return null;

            string id = version.Jar;
            string clientPath = path.GetVersionJarPath(id);

            if (!IOUtil.CheckFileValidation(clientPath, version.ClientHash, CheckHash))
            {
                return new DownloadFile
                {
                    Type = MFile.Minecraft,
                    Name = id,
                    Path = clientPath,
                    Url = version.ClientDownloadUrl
                };
            }
            else
                return null;
        }
    }
}
