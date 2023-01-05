using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalGM
{
    /// <summary>
    /// 7z压缩
    /// </summary>
    public class R7z
    {
        public R7z()
        {
            var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "7z\\7z.dll");
            SevenZipBase.SetLibraryPath(path);
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="targetName">target.7z</param>
        /// <param name="files">xxx.txt,xxx.txt</param>
        /// <returns></returns>
        public bool CompressFiles(string targetName, params string[] files)
        {
            try
            {
                var tmp = new SevenZipCompressor();
                tmp.ScanOnlyWritable = true;
                tmp.CompressFiles(targetName, files);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }
        /// <summary>
        /// 加密压缩文件
        /// </summary>
        /// <param name="targetName">target.7z</param>
        /// <param name="files">xxx.txt,xxx.txt</param>
        /// <returns></returns>
        public bool CompressFilesEncrypted(string targetName, string password, params string[] files)
        {
            try
            {
                var tmp = new SevenZipCompressor();
                tmp.ScanOnlyWritable = true;
                tmp.CompressFilesEncrypted(targetName, password, files);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }
        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="CompressFile">压缩的文件</param>
        /// <param name="targetAddress">解压的目标地址下</param>
        /// <param name="password">解密密码</param>
        /// <returns></returns>
        public bool Decompression(string CompressFile, string targetAddress, string password = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    using (var tmp = new SevenZipExtractor(CompressFile))
                    {
                        for (int i = 0; i < tmp.ArchiveFileData.Count; i++)
                        {
                            tmp.ExtractFiles(targetAddress, tmp.ArchiveFileData[i].Index);
                        }
                    }
                }
                else
                {
                    using (var tmp = new SevenZipExtractor(CompressFile, password))
                    {
                        for (int i = 0; i < tmp.ArchiveFileData.Count; i++)
                        {
                            tmp.ExtractFiles(targetAddress, tmp.ArchiveFileData[i].Index);
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return false;
        }
    }
}
