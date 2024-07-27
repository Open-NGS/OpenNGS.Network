using UnityEngine;

namespace OpenNGS.Assets
{
    public class AssetBundleInfo
    {

        private AssetBundle assetBundle;
        public AssetBundle AssetBundle
        {
            get
            {
                return assetBundle;
            }
        }

        private int refCount =0;
        public int RefCount { get { return refCount; } }

        public float UnloadTime { get; private set; }

        public AssetBundleInfo(AssetBundle bundle, float unloadTime)
        {
            this.assetBundle = bundle;
            this.UnloadTime = unloadTime;
        }

        public T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            refCount++;
            T asset = this.assetBundle.LoadAsset<T>(name);
            if(asset==null)
            {
                Debug.LogWarningFormat("{0} not found in AssetBundle:{1}", name, this.assetBundle.name);
            }
            return asset;
        }

        internal void ImmediatelyUnload()
        {
            this.refCount = 0;
            if (this.assetBundle != null)
            {
#if DEBUG_LOG
                Debug.LogFormat("ImmediatelyUnload : {0} : {1}", this.assetBundle.name, UnityEngine.Time.deltaTime);
#endif
                this.assetBundle.Unload(false);
            }
            this.assetBundle = null;
        }
    }
}
