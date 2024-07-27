using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Build;
using UnityEngine;

namespace OpenNGS.Build
{
#if UNITY_2022_1_OR_NEWER
    public class BuildPlayerProcessor : UnityEditor.Build.BuildPlayerProcessor
    {
        public override void PrepareForBuild(BuildPlayerContext buildPlayerContext)
        {
            Debug.LogFormat("BuildPlayerProcessor.AddAdditionalPathToStreamingAssets:{0}", BuildPipline.AssetBundlePath);
            if (System.IO.Directory.Exists(BuildPipline.AssetBundlePath))
            {
                buildPlayerContext.AddAdditionalPathToStreamingAssets(BuildPipline.AssetBundlePath);
            }
        }
    }
#endif

}
