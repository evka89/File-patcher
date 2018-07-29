using Newtonsoft.Json;
using Patch_Maker.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Patch_Maker.BL
{

    public class PatchMaker
    {
        public delegate void PatchMakerGeneratingEventHandler(object sender, PatchMakerGeneratingEventArgs e);

        public event PatchMakerGeneratingEventHandler patchgeneratingprogress;

        private string FolderPathIn { get; set; }
        private string FolderPathOut { get; set; }

        public PatchMaker(string folderPath, string folderPathOut = "\\Generated\\")
        {
            FolderPathIn = folderPath;
            FolderPathOut = AppDomain.CurrentDomain.BaseDirectory + folderPathOut;
        }

        public bool GeneratePatch()
        {
            MessageBox.Show("Внимание сейчас будет выполнено копирование файлов! Не отключайте программу!");
            CpToPatch(FolderPathIn, FolderPathOut + "Upadate\\");
            GenerateUpdateInfoFile();
            MessageBox.Show("Завершено!");
            return true;
        }

      
        private void GenerateUpdateInfoFile()
        {
            try {
                string[] filesPaths = Directory.GetFiles(FolderPathOut + "Upadate\\", "*", SearchOption.AllDirectories);
                List<UpdateFileInfo> updateFileInfo = new List<UpdateFileInfo>();
                for(int i=0; i < filesPaths.Length; i++)
                {
                    string HASH = GetMD5HashFromFile(filesPaths[i]);
                    UpdateFileInfo fileInfo = new UpdateFileInfo();
                    fileInfo.FileMD5Hash = HASH;
                    fileInfo.LocalFilePath = filesPaths[i].Substring((FolderPathOut + "Upadate\\").Length);
                    updateFileInfo.Add(fileInfo);
                    RasePatchGenerateProgressEvent(new PatchMakerGeneratingEventArgs()
                    {
                        countFiles = filesPaths.Length,
                        currentDone = i
                    });
                }
                string outFile = JsonConvert.SerializeObject(updateFileInfo, Formatting.Indented);
                File.WriteAllText(FolderPathOut + "updateInfo.json", outFile);
            } catch (Exception e) {
                MessageBox.Show($"Error:{e.ToString()}");
            }
        }

        private string GetMD5HashFromFile(string filesPath)
        {
            using(var md5 = MD5.Create())
            {
                using(var stream = File.OpenRead(filesPath)) {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
        }
        private void CpToPatch(string SourceDirectory, string DestDirectory)
        {
            DirectoryInfo dirSource = new DirectoryInfo(SourceDirectory);
            DirectoryInfo dirDest = new DirectoryInfo(DestDirectory);

            CopyAll(dirSource, dirDest);
        }

        private void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        protected virtual void RasePatchGenerateProgressEvent(PatchMakerGeneratingEventArgs e)
        {
            if (patchgeneratingprogress != null)
            {
                patchgeneratingprogress(this, e);
            }
        }
    }
}
