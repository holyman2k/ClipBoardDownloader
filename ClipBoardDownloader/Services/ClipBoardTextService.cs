using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ClipBoardDownloader.Model;

namespace ClipBoardDownloader.Services
{
    class ClipBoardTextService
    {
        private DownloadService downloadService;

        private int countLen, nextNumber;
        
        string prefix;

        string exp;

        public ClipBoardTextService(string prefix, string extensions, int countLen, int nextNumber)
        {
            downloadService = new DownloadService();
            this.prefix = prefix;
            this.countLen = countLen;
            this.nextNumber = nextNumber;
            this.exp = createRegularExpression(extensions);
            Console.WriteLine("regex: " + exp);
        }

        public int process(String clipText)
        {   
            Console.WriteLine("is matching: " + Regex.IsMatch(clipText, exp));
            if (Regex.IsMatch(clipText, exp))
            {
                string filename = createFileName(clipText);
                Console.WriteLine("file name: " + filename);
                Download download = new Download()
                {
                    Filename = filename,
                    URL = clipText,
                };
                downloadService.download(download);
                nextNumber++;
            }
            return nextNumber;
        }

        private string createFileName(string text)
        {
            StringBuilder sb = new StringBuilder(prefix);
            string format = "d" + countLen;
            sb.Append(nextNumber.ToString(format));
            sb.Append(".jpg");
            return sb.ToString();
        }

        private string createRegularExpression(string extensionsString)
        {
            
            StringBuilder exp = new StringBuilder(@"^http([s])?\://[a-zA-Z0-9/._-]+");

            if (extensionsString == null || extensionsString.Length == 0)
            {
                return exp.ToString();
            }

            String[] extensions = extensionsString.Split(",".ToCharArray());
            exp.Append("(");
            foreach (string ext in extensions)
            {
                if (ext != extensions[0])
                {
                    exp.Append("|");
                }
                exp.Append(ext.Trim());
            }
            exp.Append(")$");
            return exp.ToString();
        }
    }
}
