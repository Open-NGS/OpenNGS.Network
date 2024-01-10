using UnityEngine;
using UnityEditor;

#if UNITY_SWITCH

namespace nn.editor
{
    [InitializeOnLoad]
    public class OnLoad
    {
        static OnLoad()
        {
            const string TdfTempFilePath = "Temp\\FsSaveData.tdf";
            if (System.IO.File.Exists(TdfTempFilePath) == false)
            {
                System.IO.File.Copy("FsSaveDataTemplate.tdf", TdfTempFilePath);

                const string TargetEnvDefName = "NINTENDO_SDK_TARGETENV_DEF_FILE_PATH";
                System.Environment.SetEnvironmentVariable(TargetEnvDefName, System.IO.Directory.GetCurrentDirectory() + "\\" + TdfTempFilePath);

                string saveDataPath = System.IO.Directory.GetCurrentDirectory() + "\\save";
                if (!System.IO.Directory.Exists(saveDataPath))
                {
                    System.IO.Directory.CreateDirectory(saveDataPath + "\\system");
                    System.IO.Directory.CreateDirectory(saveDataPath + "\\user");
                }

                System.Text.StringBuilder dst = new System.Text.StringBuilder();
                foreach (string str in System.IO.File.ReadAllLines(System.Environment.GetEnvironmentVariable(TargetEnvDefName)))
                {
                    if(str.Contains("SDK_FS_WIN_BIS_SYSTEM_ROOT_PATH"))
                    {
                        dst.AppendLine(
                            "    <Variable Name=\"SDK_FS_WIN_BIS_SYSTEM_ROOT_PATH\" ValueType=\"string\">" +
                            saveDataPath + "\\system" +
                            "</Variable>"
                            );
                    }
                    else if(str.Contains("SDK_FS_WIN_BIS_USER_ROOT_PATH"))
                    {
                        dst.AppendLine(
                            "    <Variable Name=\"SDK_FS_WIN_BIS_USER_ROOT_PATH\" ValueType=\"string\">" +
                            saveDataPath + "\\user" +
                            "</Variable>"
                            );
                    }
                    else
                    {
                        dst.AppendLine(str);
                    }
                }
                System.IO.File.WriteAllText(
                    System.Environment.GetEnvironmentVariable(TargetEnvDefName),
                    dst.ToString(),
                    new System.Text.UTF8Encoding(true));    // UTF-8 with BOM
            }
        }
    }
}
#endif