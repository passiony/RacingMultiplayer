using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GT.Editor
{
    public class FileUnlocker
    {
        private const string ExePath = "C://FileUnlock.exe";
        private const string FILE_SUFFIX = ".temp";

        private static List<string> filterFileList = new List<string>()
        {
            "/GTFramework/Scripts/",
        };

        private static List<string> ignoreSuffix = new List<string>()
        {
            ".txt", ".meta", ".unity", ".json"
        };

        [MenuItem("Edit/Unlock Script _F6")]
        public static void UnlockScript()
        {
            if (!File.Exists(ExePath))
            {
                return;
            }

            UnlockXsls();

            foreach (var forder in filterFileList)
            {
                var path = Application.dataPath + forder;
                UnlockDirectory(path);
            }
        }

        [MenuItem("Assets/Unlock")]
        private static void UnlockSelect()
        {
            if (!File.Exists(ExePath))
            {
                return;
            }

            string[] strs = Selection.assetGUIDs;
            for (var i = 0; i < strs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(strs[i]);
                path = path.Substring(6, path.Length - 6);
                path = $"{Application.dataPath}{path}";
                if (Directory.Exists(path))
                {
                    UnlockDirectory(path);
                }
                else
                {
                    if (File.Exists(path))
                    {
                        string sourceFile = path.Replace("/", "\\");
                        string destFile = sourceFile + FILE_SUFFIX;
                        UnlockFile(new[] { sourceFile, destFile });
                    }
                }
            }
        }

        private static bool FileInIgnoreSuffixList(string filePath)
        {
            foreach (string suffix in ignoreSuffix)
            {
                if (filePath.EndsWith(suffix))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool FileInFilterList(string filePath)
        {
            string temp = filePath.Replace("\\", "/");
            foreach (string pathTemp in filterFileList)
            {
                if (temp.Contains(pathTemp))
                {
                    return true;
                }
            }

            return false;
        }

        public static void UnlockDirectory(string dirPath)
        {
            try
            {
                string[] files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
                EditorUtility.DisplayProgressBar("Unlock", "unlock files", 0);
                float total = files.Length;
                for (var i = 0; i < files.Length; i++)
                {
                    if (FileInIgnoreSuffixList(files[i]))
                    {
                        continue;
                    }

                    EditorUtility.DisplayProgressBar("Unlock", Path.GetFileName(files[i]), i / total);

                    string sourceFile = files[i].Replace("/", "\\");
                    string destFile = sourceFile + FILE_SUFFIX;
                    ThreadPool.QueueUserWorkItem(UnlockFile, new[] { sourceFile, destFile });
                }

                Debug.LogWarning($"unlock {total} files");
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", e.Message, "确定");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void UnlockFile(object state)
        {
            string[] array = (string[])state;
            string sourceFile = array[0];
            string destFile = array[1];

            CopyFile(sourceFile, destFile);
            File.Delete(sourceFile);
            ReNameFile(destFile, sourceFile);
        }

        private static void CopyFile(string source, string dest)
        {
            using (FileStream fsRead = new FileStream(source, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsWrite = new FileStream(dest, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] buffer = new byte[1024 * 8];
                    while (true)
                    {
                        int r = fsRead.Read(buffer, 0, buffer.Length);
                        if (r == 0)
                        {
                            break;
                        }

                        fsWrite.Write(buffer, 0, r);
                    }
                }
            }
        }

        private static void ReNameFile(string sourceFile, string destFile)
        {
            Process p = new Process();
            p.StartInfo.FileName = ExePath;
            StringBuilder sb = new StringBuilder();
            sb.Append($" -sourcePath=\"{sourceFile}\"");
            sb.Append($" -destPath=\"{destFile}\"");
            p.StartInfo.Arguments = sb.ToString();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                // if (!string.IsNullOrEmpty(e.Data))
                // {
                //     Debug.Log(e.Data);
                // }
            };
            p.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Debug.LogError(e.Data);
                }
            };
            p.Start();
            p.BeginOutputReadLine();
        }

        private static void UnlockXsls()
        {
            // var path = ConfigSetting.CommonFullPath + "/excels/";
            // Process p = new Process();
            // p.StartInfo.FileName = path + "wps.exe";
            // p.StartInfo.UseShellExecute = false;
            // p.StartInfo.RedirectStandardOutput = true;
            // p.StartInfo.RedirectStandardInput = true;
            // p.StartInfo.RedirectStandardError = true;
            // p.StartInfo.CreateNoWindow = true;
            // p.StartInfo.WorkingDirectory = path;
            // p.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            // {
            //     // if (!string.IsNullOrEmpty(e.Data))
            //     // {
            //     //     Debug.Log(e.Data);
            //     // }
            // };
            // p.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            // {
            //     if (!string.IsNullOrEmpty(e.Data))
            //     {
            //         Debug.LogError(e.Data);
            //     }
            // };
            // p.Start();
            // p.WaitForExit();
        }
    }
}