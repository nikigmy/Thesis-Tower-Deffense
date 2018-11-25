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
                var levelsPath = pathToBuiltProject + "/../Levels";
                System.IO.Directory.CreateDirectory(levelsPath);
                foreach (var level in Directory.GetFiles(Application.dataPath + "/Resources/Levels").Where(x => x.EndsWith(".xml")))
                {
                    var fileName = Path.GetFileNameWithoutExtension(level);
                    File.Copy(level, levelsPath + "/" + fileName + ".xml");
                }
                File.Copy(Application.dataPath + "/Resources/Setup.xml", pathToBuiltProject + "/../Setup.xml");
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("Post build failed");
        }
    }
}
