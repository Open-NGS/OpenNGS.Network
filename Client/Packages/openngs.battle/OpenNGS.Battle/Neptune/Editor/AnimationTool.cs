using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Neptune.Datas;
using Newtonsoft.Json;
using Object = UnityEngine.Object;
using Neptune;
#if UNITY_EDITOR
public class AnimationTool : EditorWindow
{
    static Dictionary<string, Dictionary<string, Dictionary<float, AniEventData>>> AnimAtkFrames = new Dictionary<string, Dictionary<string, Dictionary<float, AniEventData>>>();
    static Dictionary<string, Dictionary<string, AnimationConfigData>> AnimDurations = new Dictionary<string, Dictionary<string, AnimationConfigData>>();

    [MenuItem("OpenNGS/Animation/Export Config from FBX")]
    public static void ExportFBXAnimationConfig()
    {
        ExportFBXAnimationConfig(false);
    }

    [MenuItem("OpenNGS/Animation/Remove Cheer")]
    public static void RemoveCheerAnim()
    {
        RemoveUnitAnimation("Cheer");
    }

    static void RemoveUnitAnimation(string name)
    {
        string[] folders = Directory.GetDirectories("Assets/Units", "*", SearchOption.TopDirectoryOnly);
        float i = 0, total = folders.Length;
        AssetDatabase.StartAssetEditing();
        foreach (string folder in folders)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Remove Animation", "process " + folder, i++ / total))
            {
                break;
            }
            //枚举每个目录下的动画资源
            string foldername = Path.GetFileName(folder);
            string[] anim_files = Directory.GetFiles("Assets/Units/" + foldername, "*.txt");

            foreach (string jsonfile in anim_files)
            {
                try
                {
                    Dictionary<string, float[]> config = fastJSON.JSON.ToObject<Dictionary<string, float[]>>(File.ReadAllText(jsonfile));
                    if (config.ContainsKey(name))
                    {
                        config.Remove(name);
                        fastJSON.JSONParameters jp = new fastJSON.JSONParameters();
                        File.WriteAllText(jsonfile, fastJSON.JSON.ToNiceJSON(config, jp));
                    }
                    bool needSave = false;
                    string fbxfile = Path.GetDirectoryName(jsonfile) + "/" + Path.GetFileNameWithoutExtension(jsonfile) + ".FBX";
                    if (File.Exists(fbxfile))
                    {
                        ModelImporter importer = (ModelImporter)ModelImporter.GetAtPath(fbxfile);
                        if (importer != null && importer.importAnimation)
                        {
                            List<ModelImporterClipAnimation> clips = new List<ModelImporterClipAnimation>();
                            foreach (ModelImporterClipAnimation clip in importer.clipAnimations)
                            {
                                if (clip.name != name)
                                {
                                    needSave = true;
                                    clips.Add(clip);
                                }
                            }
                            importer.clipAnimations = clips.ToArray();
                        }
                        if (needSave)
                            importer.SaveAndReimport();
                    }
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("ERROR", "Process " + jsonfile + "\n" + ex.Message, "OK");
                }
            }
        }
        AssetDatabase.StopAssetEditing();
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("", "Animation process completed", "OK");
    }

    //[MenuItem("OpenNGS/Resource Tools/Export Expose Bones", false, 3)]
    private static void ExportExposeBones()
    {
        string[] allPath = new string[] {
            "Assets/Game/BuildAssets/UnitsRaw"
        };

        List<string> allPrefabs = new List<string>();
        foreach (string path in allPath)
        {
            string[] prefabs = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
            allPrefabs.AddRange(prefabs);
        }

        string txtResult = "Prefab, Node, Node Path, FX\r\n";

        for (int i = 0; i < allPrefabs.Count; i++)
        {
            string prefabPath = allPrefabs[i];
            if (EditorUtility.DisplayCancelableProgressBar("Export Expose Bones", "Check : [" + i + "/" + allPrefabs.Count + "]" + prefabPath, (i + 1) / (float)allPrefabs.Count))
                break;

            if (prefabPath.EndsWith("_ui.prefab", StringComparison.CurrentCultureIgnoreCase))
                continue;

            Object prefab = AssetDatabase.LoadMainAssetAtPath(prefabPath);
            GameObject obj = prefab as GameObject;

            Animator ani = obj.GetComponent<Animator>();
            if (ani == null)
                continue;

            if (!ani.hasTransformHierarchy)
                continue;

            bool result = false;

            List<string> exposeBones = new List<string>();
            try
            {
                result = CheckBones(obj.transform, ref txtResult, exposeBones);
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Optimize Error", ex.ToString(), "OK");
                result = false;
                break;
            }

            string objname = obj.name;
            string fbxfile = "";
            string[] alldeps = AssetDatabase.GetDependencies(prefabPath, false);
            for (int di = 0; di < alldeps.Length; di++)
            {
                string name = alldeps[di];
                if (!name.StartsWith("Assets/Units/", StringComparison.CurrentCultureIgnoreCase))
                    continue;

                if (!name.EndsWith(".fbx", StringComparison.CurrentCultureIgnoreCase))
                    continue;
                if (fbxfile == "")
                    fbxfile = name;
                else
                    UnityEngine.Debug.LogError("FBX Dup : " + objname + " : " + fbxfile + " > " + name);
            }
            if (fbxfile != "")
            {
                string cfg_file = Path.GetDirectoryName(fbxfile) + "/" + Path.GetFileNameWithoutExtension(fbxfile) + "_EB.txt";
                fastJSON.JSONParameters jp = new fastJSON.JSONParameters();
                File.WriteAllText(cfg_file, fastJSON.JSON.ToNiceJSON(exposeBones, jp));
            }
        }
        try
        {
            File.WriteAllText("UnitBones.csv", txtResult);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(ex.ToString());
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("OpenNGS/Resource Tools/Optimize GameObject", false, 3)]
    private static void OptimizeGameObject()
    {
        string[] allPath = new string[] {
            "Assets/Game/BuildAssets/UnitsRaw"
        };

        List<string> allPrefabs = new List<string>();
        foreach (string path in allPath)
        {
            string[] prefabs = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
            allPrefabs.AddRange(prefabs);
        }

        for (int i = 0; i < allPrefabs.Count; i++)
        {
            string prefabPath = allPrefabs[i].Replace("\\", "/");
            string targetPrefabPath = prefabPath.Replace("/UnitsRaw/", "/Units/");

            if (EditorUtility.DisplayCancelableProgressBar("Optimize GameObject", "Optimize : [" + i + "/" + allPrefabs.Count + "]" + prefabPath, (i + 1) / (float)allPrefabs.Count))
                break;

            string srcHash = HashUtil.ComputeFileHash(prefabPath, HashUtil.HashType.MD5);

            bool changed = false;
            AssetImporter srcImporter = AssetImporter.GetAtPath(prefabPath);
            if (!string.IsNullOrEmpty(srcImporter.assetBundleName))
                srcImporter.assetBundleName = null;
            if (srcImporter.userData != srcHash)
            {
                srcImporter.userData = srcHash;
                changed = true;
                srcImporter.SaveAndReimport();
            }

            AssetImporter dstImporter = AssetImporter.GetAtPath(targetPrefabPath);
            if (dstImporter == null)
            {
                changed = true;
            }
            else if (srcHash == dstImporter.userData)
            {
                continue;
            }

            try
            {
                File.Delete(targetPrefabPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                File.Copy(prefabPath, targetPrefabPath, true);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                dstImporter = AssetImporter.GetAtPath(targetPrefabPath);
                dstImporter.userData = srcHash;
                dstImporter.SaveAndReimport();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("{0}", ex.ToString());
                break;
            }

            if (prefabPath.EndsWith("_ui.prefab", StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }

            Object prefab = AssetDatabase.LoadMainAssetAtPath(targetPrefabPath);
            GameObject obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            //GameObject obj =  prefab as GameObject;

            Animator ani = obj.GetComponent<Animator>();
            if (ani == null)
            {
                DestroyImmediate(obj);
                continue;
            }

            bool result = false;

            List<string> exposeBones = new List<string>();
            try
            {
                result = OptimizePrefab(obj);
                if (result)
                {
                    PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
                    string raw = File.ReadAllText(targetPrefabPath);
                    raw = raw.Replace("m_HasTransformHierarchy: 1\n", "m_HasTransformHierarchy: 0\n");
                    File.WriteAllText(targetPrefabPath, raw);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                }
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Optimize Error", ex.ToString(), "OK");
                result = false;
                break;
            }
            DestroyImmediate(obj);
            
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        EditorUtility.ClearProgressBar();
    }

    private static bool OptimizePrefab(GameObject src)
    {
        GameObject tmp = Instantiate(src);
        AnimatorUtility.OptimizeTransformHierarchy(tmp, null);

        Animator srcAni = src.GetComponent<Animator>();
        SkinnedMeshRenderer[] sourceRenderers = src.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        SkinnedMeshRenderer[] tmpRenderers = tmp.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (int i = 0; i < sourceRenderers.Length; i++)
        {
            sourceRenderers[i].motionVectors = false;
            sourceRenderers[i].skinnedMotionVectors = false;
            sourceRenderers[i].receiveShadows = false;
            sourceRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            sourceRenderers[i].rootBone = null;
            sourceRenderers[i].bones = null;


            string json = EditorJsonUtility.ToJson(tmpRenderers[i].transform);
            EditorJsonUtility.FromJsonOverwrite(json, sourceRenderers[i].transform);
        }

        DestroyImmediate(tmp);
        List<Transform> exposeList = new List<Transform>();
        OptimizeBones(src.transform, exposeList);
        for (int i = src.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = src.transform.GetChild(i);
            if (child.IsBone())
            {
                if (exposeList.Contains(child))
                {
                    for (int j = child.childCount - 1; j >= 0; j--)
                    {
                        Transform subchild = child.GetChild(j);
                        if (IsAnchorPoint(subchild.name) || subchild.name.IndexOf("fx_") >= 0 || subchild.GetComponents<ParticleSystem>().Length > 0)
                            continue;
                        DestroyImmediate(subchild.gameObject);
                    }
                }
                else
                {
                    ParticleSystem[] pss = child.GetComponentsInChildren<ParticleSystem>();
                    if (pss.Length > 0)
                    {
                        UnityEngine.Debug.LogErrorFormat("{0} have at least 1 FX in {1} : {2}", src.name, child.name, pss[0].name);
                        return false;
                    }
                    else
                        DestroyImmediate(child.gameObject);
                }
            }
        }
        return true;
    }

    private static void OptimizeBones(Transform src, List<Transform> exposeList)
    {
        for (int i = 0; i < src.childCount; i++)
        {
            Transform child = src.GetChild(i);
            if (src == src.root && !child.IsBone())
                continue;

            else if (IsAnchorPoint(child.name) || child.name.IndexOf("fx_") >= 0 || child.GetComponents<ParticleSystem>().Length > 0)
            {
                if (!exposeList.Contains(child.parent))
                    exposeList.Add(child.parent);
            }
            else
                OptimizeBones(child, exposeList);
        }
        if (src == src.root)
        {
            exposeList.Sort((a, b) =>
            {
                string pa = GetFullPath(a);
                string pb = GetFullPath(b);

                int la = pa.Count((c) => c == '/');
                int lb = pb.Count((c) => c == '/');

                if (la != lb)
                    return la.CompareTo(lb);

                return a.name.CompareTo(b.name);
            });
            for (int i = exposeList.Count - 1; i > -1; i--)
            {
                UnityEngine.Debug.LogFormat("Transfer {0} to {1}", GetFullPath(exposeList[i]), src.name);
                exposeList[i].SetParent(src, true);
                exposeList[i].SetAsFirstSibling();
            }
        }
    }

    private static string GetFullPath(Transform go)
    {
        string result = go.name;
        if (go.parent != null && go.parent != go.root)
        {
            return GetFullPath(go.parent) + "/" + result;
        }
        return result;
    }

    public static bool IsAnchorPoint(string name)
    {
        return Enum.IsDefined(typeof(AnchorPoint), name);
    }
    private static bool CheckBones(Transform node, ref string txtResult, List<string> exposeBones)
    {
        for (int i = 0; i < node.childCount; i++)
        {
            Transform child = node.GetChild(i);
            if (child.name.Equals("fx", StringComparison.CurrentCultureIgnoreCase))
                continue;

            if (child.parent != child.root)
            {
                if (IsAnchorPoint(child.name) || child.name.IndexOf("fx_") >= 0 || child.GetComponents<ParticleSystem>().Length > 0)
                {
                    string bonePath = GetFullPath(child.parent);
                    txtResult += string.Format("{0}, {1}, {2}, {3}\r\n", node.root.name, child.parent.name, bonePath, child.name);
                    if (!exposeBones.Contains(bonePath))
                        exposeBones.Add(bonePath);
                }
                else
                    CheckBones(child, ref txtResult, exposeBones);
            }
            else if (child.IsBone())
                CheckBones(child, ref txtResult, exposeBones);
        }
        return true;
    }

    //[MenuItem("OpenNGS/Resource Tools/Process Expose Bones Prefab", false, 3)]
    public static void ProcessPrefab()
    {
        string[] allPath = new string[] {
            "Assets/Game/BuildAssets/UnitsRaw"
        };

        List<string> allPrefabs = new List<string>();
        foreach (string path in allPath)
        {
            string[] prefabs = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
            allPrefabs.AddRange(prefabs);
        }

        string txtResult = "Prefab, Node, Node Path, FX\r\n";

        for (int i = 0; i < allPrefabs.Count; i++)
        {
            string prefabPath = allPrefabs[i];
            if (EditorUtility.DisplayCancelableProgressBar("Batch Expose prefab", "Exposing : [" + i + "/" + allPrefabs.Count + "]" + prefabPath, (i + 1) / (float)allPrefabs.Count))
                break;

            if (prefabPath.EndsWith("_ui.prefab", StringComparison.CurrentCultureIgnoreCase))
                continue;

            Object prefab = AssetDatabase.LoadMainAssetAtPath(prefabPath);
            GameObject obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            bool result = false;
            try
            {
                Animator ani = obj.GetComponent<Animator>();
                if (ani == null)
                    continue;

                if (!ani.hasTransformHierarchy)
                    continue;

                string objname = obj.name;
                string fbxfile = "";
                string[] alldeps = AssetDatabase.GetDependencies(prefabPath, false);
                for (int di = 0; di < alldeps.Length; di++)
                {
                    string name = alldeps[di];
                    if (!name.StartsWith("Assets/Units/", StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    if (!name.EndsWith(".fbx", StringComparison.CurrentCultureIgnoreCase))
                        continue;
                    if (fbxfile == "")
                        fbxfile = name;
                    else
                        UnityEngine.Debug.LogError("FBX Dup : " + objname + " : " + fbxfile + " > " + name);
                }
                if (fbxfile != "")
                {
                    Object targetprefab = AssetDatabase.LoadMainAssetAtPath(fbxfile);
                    GameObject targetobj = PrefabUtility.InstantiatePrefab(targetprefab) as GameObject;

                    if (TransferPrefab(obj, targetobj) == false)
                    {
                        break;
                    }
                    PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
                    GameObject.DestroyImmediate(targetobj);
                }
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Transfer Error", ex.ToString(), "OK");
                break;
            }
            GameObject.DestroyImmediate(obj);
        }
        EditorUtility.ClearProgressBar();
    }

    private static bool TransferPrefab(GameObject src, GameObject dst)
    {
        Animator srcAni = src.GetComponent<Animator>();
        Animator dstAni = dst.GetComponent<Animator>();
        if (dstAni != null && srcAni != null)
            dstAni.runtimeAnimatorController = srcAni.runtimeAnimatorController;
        string json = "";


        SkinnedMeshRenderer[] sourceRenderers = src.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        SkinnedMeshRenderer[] targetRenderers = dst.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i].name != sourceRenderers[i].name)
            {
                UnityEngine.Debug.LogErrorFormat("Skinned Mesh not match: {0} -> {1}", src.name, dst.name);
                return false;
            }
            targetRenderers[i].sharedMaterials = sourceRenderers[i].sharedMaterials.ToArray();
            targetRenderers[i].motionVectors = sourceRenderers[i].motionVectors;
            targetRenderers[i].skinnedMotionVectors = sourceRenderers[i].skinnedMotionVectors;
            targetRenderers[i].receiveShadows = sourceRenderers[i].receiveShadows;
            targetRenderers[i].shadowCastingMode = sourceRenderers[i].shadowCastingMode;
            sourceRenderers[i].rootBone = null;
            sourceRenderers[i].bones = null;

            json = EditorJsonUtility.ToJson(targetRenderers[i]);
            EditorJsonUtility.FromJsonOverwrite(json, sourceRenderers[i]);
            json = EditorJsonUtility.ToJson(targetRenderers[i].transform);
            EditorJsonUtility.FromJsonOverwrite(json, sourceRenderers[i].transform);
        }
        List<Transform> exposeList = new List<Transform>();
        TransferFX(src.transform, dst.transform, src.transform, exposeList);
        for (int i = src.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = src.transform.GetChild(i);
            if (child.name.StartsWith("bone", StringComparison.CurrentCultureIgnoreCase) || child.name.StartsWith("bip", StringComparison.CurrentCultureIgnoreCase))
            {
                if (exposeList.Contains(child))
                {
                    for (int j = child.childCount - 1; j >= 0; j--)
                    {
                        Transform subchild = child.GetChild(j);
                        if (IsAnchorPoint(subchild.name) || subchild.name.IndexOf("fx_") >= 0 || subchild.GetComponents<ParticleSystem>().Length > 0)
                            continue;
                        DestroyImmediate(subchild.gameObject);
                    }
                }
                else
                {
                    ParticleSystem[] pss = child.GetComponentsInChildren<ParticleSystem>();
                    if (pss.Length > 0)
                    {
                        UnityEngine.Debug.LogErrorFormat("{0} have at least 1 FX in {1} : {2}", src.name, child.name, pss[0].name);
                        return false;
                    }
                    else
                        DestroyImmediate(child.gameObject);
                }
            }
        }
        json = EditorJsonUtility.ToJson(dstAni);
        EditorJsonUtility.FromJsonOverwrite(json, srcAni);
        return true;
    }

    private static void TransferFX(Transform src, Transform dst, Transform root, List<Transform> exposeList)
    {
        for (int i = 0; i < src.childCount; i++)
        {
            Transform child = src.GetChild(i);
            if (src == root && !child.name.StartsWith("bone", StringComparison.CurrentCultureIgnoreCase) && !child.name.StartsWith("bip", StringComparison.CurrentCultureIgnoreCase))
                continue;

            else if (IsAnchorPoint(child.name) || child.name.IndexOf("fx_") >= 0 || child.GetComponents<ParticleSystem>().Length > 0)
            {
                if (!exposeList.Contains(child.parent))
                    exposeList.Add(child.parent);
            }
            else
                TransferFX(child, dst, root, exposeList);
        }
        if (src == root)
            for (int i = exposeList.Count - 1; i > -1; i--)
            {
                UnityEngine.Debug.LogFormat("Transfer {0} to {1}", exposeList[i].name, root.name);
                exposeList[i].SetParent(root, true);
                exposeList[i].SetAsFirstSibling();
            }
    }


    //[MenuItem("OpenNGS/Animation/Particle Test")]
    //public static void ParticleTest()
    //{
    //    List<string> files = new List<string>();
    //    string[] files1 = Directory.GetFiles("Assets/FX/", "*.prefab", SearchOption.AllDirectories);
    //    string[] files2 = Directory.GetFiles("Assets/Game/BuildAssets/FX/", "*.prefab", SearchOption.AllDirectories);
    //    files.AddRange(files1);
    //    files.AddRange(files2);

    //    float i = 0, total = files.Count;
    //    StringBuilder sb = new StringBuilder();
    //    foreach (string anim_file in files)
    //    {
    //        EditorUtility.DisplayProgressBar("Particle Test", "process " + anim_file, i++ / total);
    //        string anim_name = System.IO.Path.GetFileNameWithoutExtension(anim_file);
    //        ParticleSystem particleroot = AssetDatabase.LoadAssetAtPath<ParticleSystem>(anim_file);
    //        if (particleroot != null)
    //        {
    //            sb.Append(ArtUtility.CheckParticle(particleroot, anim_file));
    //            foreach (ParticleSystem particle in particleroot.GetComponentsInChildren<ParticleSystem>(true))
    //            {
    //                sb.Append(ArtUtility.CheckParticle(particle, anim_file));
    //            }
    //        }
    //    }
    //    Debug.Log("ParticleSystem Invalid Size List:\n" + sb.ToString());
    //    EditorUtility.ClearProgressBar();
    //}

    [MenuItem("OpenNGS/Animation/Statistic Bones from FBX")]
    public static void StatisticFBXBones()
    {
        string[] folders = Directory.GetDirectories("Assets/Units", "*", SearchOption.TopDirectoryOnly);
        float i = 0, total = folders.Length;

        Dictionary<string, int> bones = new Dictionary<string, int>();
        foreach (string folder in folders)
        {
            EditorUtility.DisplayProgressBar("Statistic Bones", "process " + folder, i++ / total);
            //枚举每个目录下的动画资源
            string name = Path.GetFileName(folder);
            string[] anim_files = Directory.GetFiles("Assets/Units/" + name, "*.fbx");

            foreach (string anim_file in anim_files)
            {
                string anim_name = System.IO.Path.GetFileNameWithoutExtension(anim_file);
                Object model = AssetDatabase.LoadMainAssetAtPath(anim_file);
                //int bonenum = ArtUtility.StatisticBones((GameObject)model);
                //if (bonenum > 0)
                //{
                //    bones[model.name] = bonenum;
                //}
            }
        }

        string result = "Result: " + string.Join("\r\n", bones.OrderByDescending(x => x.Value).ToList().ConvertAll<string>(
            new System.Converter<KeyValuePair<string, int>, string>(
                e => string.Format("{0}bones:{1}", e.Key.PadRight(16), e.Value)
                )
            ).ToArray());
        EditorUtility.ClearProgressBar();
        UnityEngine.Debug.Log(result);
    }

    public static void ExportFBXAnimationConfig(bool silence)
    {
        try
        {

            AnimAtkFrames.Clear();
            AnimDurations.Clear();


            if (!silence && !EditorUtility.DisplayDialog("Export animation config",
                "This operation will overwrite AnimationConfig.txt and AniEvent.txt!\n\nAre you sure you want to continue?",
                "Yes", "Do Not Overwrite"))
            {
                return;
            }

            string[] folders = Directory.GetDirectories("Assets/Units", "*", SearchOption.TopDirectoryOnly);
            float i = 0, total = folders.Length;
            foreach (string folder in folders)
            {
                if (!silence)
                    EditorUtility.DisplayProgressBar("Process Animation", "process " + folder, i++ / total);
                //枚举每个目录下的动画资源
                ProcessFolder(folder);
            }
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.Formatting = Formatting.Indented;
            setting.NullValueHandling = NullValueHandling.Ignore;
            string dura = JsonConvert.SerializeObject(AnimDurations, Formatting.Indented, setting);
            string atkframe = JsonConvert.SerializeObject(AnimAtkFrames, Formatting.Indented, setting);
            File.WriteAllText("Assets/Game/BuildAssets/Data/AnimationConfig.txt", dura);
            File.WriteAllText("Assets/Game/BuildAssets/Data/AniEvent.txt", atkframe);

            if (!silence)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("", "Animation process completed", "OK");
            }
        }
        catch (Exception exception)
        {
            UnityEngine.Debug.LogError(exception);
            if (!silence)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("", "Animation process Failed", "OK");
            }
            throw;
        }
    }

    static void ProcessFolder(string folder)
    {
        string name = Path.GetFileName(folder);
        AnimDurations[name] = new Dictionary<string, AnimationConfigData>();
        AnimAtkFrames[name] = new Dictionary<string, Dictionary<float, AniEventData>>();

        //查找引用
        string[] anim_ref = Directory.GetFiles("Assets/Units/" + name, "*.ref");
        string reffilename = "";
        if (anim_ref.Length != 0)
        {
            reffilename = System.IO.Path.GetFileNameWithoutExtension(anim_ref[0]);
        }

        string[] anim_files = Directory.GetFiles("Assets/Units/" + (reffilename.Length != 0 ? reffilename : name), "*.fbx");

        foreach (string anim_file in anim_files)
        {
            if (!anim_file.Contains("LOD"))
            {
                ProcessFBX(anim_file, name, reffilename);
            }
        }
    }

    static void ProcessFBX(string path, string anim_name, string refname)
    {
        string anim_name1 = System.IO.Path.GetFileNameWithoutExtension(path);

        anim_name = refname.Length == 0 ? anim_name1 : anim_name1.Replace(refname, anim_name);

        Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
        AnimDurations[anim_name] = new Dictionary<string, AnimationConfigData>();
        AnimAtkFrames[anim_name] = new Dictionary<string, Dictionary<float, AniEventData>>();
        //AnimEventRes[anim_name] = new GameData.AnimEventResData();
        //AnimEventRes[anim_name].PlaySounds = new Dictionary<string, List<string>>();
        //AnimEventRes[anim_name].SkillList = new List<int>();
        foreach (Object o in objs)
        {
            if (o is AnimationClip)
            {
                AnimationClip clip = (AnimationClip)o;
                if (clip.name.Contains("__preview__"))
                    continue;
                ProcessClip(anim_name, clip);
            }
        }
    }

    static void ProcessClip(string anim_name, AnimationClip clip)
    {
        AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clip);
        AnimationClipCurveData[] caveDatas = AnimationUtility.GetAllCurves(clip);

        string clipname = clip.name.Replace(anim_name, "");
        //生成动画周期数据
        AnimDurations[anim_name][clipname] = new AnimationConfigData();
        AnimDurations[anim_name][clipname].Role = anim_name;
        AnimDurations[anim_name][clipname].Animation = clipname;
        AnimDurations[anim_name][clipname].TotalTime = clip.length;

        //在这里获得动作的事件
        Dictionary<float, AniEventData> datas = new Dictionary<float, AniEventData>();

        foreach (AnimationEvent evt in events)
        {
            if (evt.functionName == "AttackSpot")
            {
                float time = evt.time;
                datas[time] = new AniEventData();
                datas[time].Role = anim_name;
                datas[time].Action = clipname;
                datas[time].Time = time;
                datas[time].Type = "AttackSpot";

                bool haveHitPoint = false;
                foreach (AnimationClipCurveData cave in caveDatas)
                {
                    if (cave.path.ToLower() == @"hitpoint" || cave.path.ToLower() == @"attackspot")
                    {
                        haveHitPoint = true;
                        if (cave.propertyName == "m_LocalPosition.x")
                        {
                            datas[time].Y = -cave.curve.Evaluate(evt.time) * BattleField.LogicWorldFactor;
                        }
                        if (cave.propertyName == "m_LocalPosition.y")
                        {
                            datas[time].Z = cave.curve.Evaluate(evt.time) * BattleField.LogicWorldFactor;
                        }
                        if (cave.propertyName == "m_LocalPosition.z")
                        {
                            datas[time].X = cave.curve.Evaluate(evt.time) * BattleField.LogicWorldFactor;
                        }
                    }
                }
                if (!haveHitPoint && !string.IsNullOrEmpty(evt.stringParameter))
                {
                    string[] param = evt.stringParameter.Split(",".ToCharArray());
                    datas[time].X = float.Parse(param[0]);
                    datas[time].Y = float.Parse(param[1]);
                }
            }
            if (evt.functionName == "DoAction")
            {
                float time = evt.time;
                AniEventData newEvent = new AniEventData();
                newEvent.Role = anim_name;
                newEvent.Action = clipname;
                newEvent.Time = time;
                if (!string.IsNullOrEmpty(evt.stringParameter))
                {
                    string[] arglist = evt.stringParameter.Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    if (arglist.Length == 0)
                    {
                        Debug.LogErrorFormat("DoAction args error:{1}", evt.stringParameter);
                        continue;
                    }

                    switch (arglist[0].ToLower())
                    {
                        case "dash":
                            newEvent.Type = "Dash";
                            break;
                        case "teleport":
                            newEvent.Type = "Teleport";
                            break;
                        case "camshake":
                            newEvent.Type = "Camshake";
                            break;
                        case "invisibility":
                            newEvent.Type = "Invisibility";
                            break;
                        case "end":
                            newEvent.Type = "End";
                            break;
                        default:
                            continue;
                    }

                    newEvent.X = arglist.Length > 1 ? float.Parse(arglist[1]) : 0;
                    newEvent.Y = arglist.Length > 2 ? float.Parse(arglist[2]) : 0;
                    newEvent.Z = arglist.Length > 3 ? float.Parse(arglist[3]) : 0;
                    newEvent.Param1 = arglist.Length > 4 ? float.Parse(arglist[4]) : 0;
                    newEvent.Param2 = arglist.Length > 5 ? float.Parse(arglist[5]) : 0;
                    newEvent.Param3 = arglist.Length > 6 ? float.Parse(arglist[6]) : 0;
                    newEvent.Param4 = arglist.Length > 7 ? float.Parse(arglist[7]) : 0;
                    newEvent.Param5 = arglist.Length > 8 ? float.Parse(arglist[8]) : 0;
                    newEvent.Param6 = arglist.Length > 9 ? float.Parse(arglist[9]) : 0;
                    newEvent.Param7 = arglist.Length > 10 ? float.Parse(arglist[10]) : 0;
                    newEvent.Param8 = arglist.Length > 11 ? float.Parse(arglist[11]) : 0;
                }
                datas[time] = newEvent;
            }
            if (evt.functionName == "Skill")
            {
                float time = evt.time;
                AniEventData newEvent = new AniEventData();
                newEvent.Role = anim_name;
                newEvent.Action = clipname;
                newEvent.Time = time;
                newEvent.Type = "Skill";
                newEvent.X = evt.intParameter;
                datas[time] = newEvent;
                //if (!AnimEventRes[anim_name].SkillList.Contains(evt.intParameter))
                //{
                //    AnimEventRes[anim_name].SkillList.Add(evt.intParameter);
                //}
            }
            if (evt.functionName == "PlaySound")
            {
                //if (!AnimEventRes[anim_name].PlaySounds.ContainsKey(clipname))
                //{
                //    AnimEventRes[anim_name].PlaySounds[clipname] = new List<string>();
                //}
                //AnimEventRes[anim_name].PlaySounds[clipname].Add(evt.stringParameter);
            }




        }
        if (datas.Count > 0)
            AnimAtkFrames[anim_name][clipname] = datas;
    }
}


public static class TransformExtend
{
    public static bool IsBone(this Transform transform)
    {
        return transform.name.StartsWith("bone", StringComparison.CurrentCultureIgnoreCase) || transform.name.StartsWith("bip", StringComparison.CurrentCultureIgnoreCase) || transform.name.StartsWith("dummy", StringComparison.CurrentCultureIgnoreCase);
    }
}
#endif