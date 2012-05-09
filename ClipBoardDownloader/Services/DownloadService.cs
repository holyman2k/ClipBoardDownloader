using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using ClipBoardDownloader.Model;
using ClipBoardDownloader.Config;

namespace ClipBoardDownloader.Services
{
    class DownloadService
    {
        public void download(Download download)
        {
            DirectoryInfo dir = new DirectoryInfo(Setting.BASE_PATH);
            if (!dir.Exists)
                dir.Create();

            string filename = Path.Combine(Setting.BASE_PATH, download.Filename);
            WebClient Client = new WebClient();
            Client.DownloadFile(download.URL, filename);
        }
    }
}
