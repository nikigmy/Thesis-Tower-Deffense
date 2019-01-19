using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;
using System.Linq;

public class PostBuild
{
    [PostProcessBuild]
    public static void CopySetup(BuildTarget target, string pathToBuiltProject)
    {
        try
        {
            if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                var levelsPath = pathToBuiltProject + "/../" + Constants.cst_Levels;
                System.IO.Directory.CreateDirectory(levelsPath);
                foreach (var level in Directory.GetFiles(Application.dataPath + "/Resources/" + Constants.cst_Levels).Where(x => x.EndsWith(Constants.cst_Xml)))
                {
                    var fileName = Path.GetFileNameWithoutExtension(level);
                    File.Copy(level, levelsPath + "/" + fileName + Constants.cst_Xml);
                }
                File.Copy(Application.dataPath + "/Resources/" + Constants.cst_Setup + Constants.cst_Xml, pathToBuiltProject + "/../" + Constants.cst_Setup + Constants.cst_Xml);
                File.Copy(Application.dataPath + "/Resources/" + Constants.cst_Config + Constants.cst_Xml, pathToBuiltProject + "/../" + Constants.cst_Config + Constants.cst_Xml);
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("Post build failed");
        }
    }
}
