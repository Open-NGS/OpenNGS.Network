using UnityEngine;
using UnityEngine.Events;

class BundleLoadAction
{
    UnityAction<AssetBundleInfo> onLoadBundle;
    UnityAction<string> onLoadText;
    UnityAction<byte[]> onLoadBytes;
    UnityAction<Texture2D> onLoadTexture;

    private float startTime;
    private AssetBundleType type;
    private BundleLoadAction()
    {
        float startTime = Time.realtimeSinceStartup;
    }

    public BundleLoadAction(UnityAction<AssetBundleInfo> onLoadBundle) : this()
    {
        this.onLoadBundle = onLoadBundle;
        type = AssetBundleType.AssetBundle;
    }

    public BundleLoadAction(UnityAction<string> onLoadText) : this()
    {
        this.onLoadText = onLoadText;
        type = AssetBundleType.Text;
    }
    public BundleLoadAction(UnityAction<byte[]> onLoadBytes) : this()
    {
        this.onLoadBytes = onLoadBytes;
        type = AssetBundleType.Bytes;
    }
    public BundleLoadAction(UnityAction<Texture2D> onLoadTexture) : this()
    {
        this.onLoadTexture = onLoadTexture;
        type = AssetBundleType.Texture;
    }

    public void OnLoadBundle(AssetBundleInfo bundleInfo)
    {
#if DEVELOPMENT
        Debug.LogFormat("LoadBundle Done : [{0}]{1}  Elapsed:{2}", type,bundleInfo != null ? bundleInfo.name : "null", Time.realtimeSinceStartup - startTime);
#endif
        if (type == AssetBundleType.AssetBundle)
        {
            if (this.onLoadBundle != null)
            {
                this.onLoadBundle(bundleInfo);
            }
        }
        else if (type == AssetBundleType.Text)
        {
            if (bundleInfo != null)
            {
                onLoadText(bundleInfo.text);
                bundleInfo.Unload();
            }
            else
                onLoadText(null);
        }
        else if (type == AssetBundleType.Bytes)
        {
            if (bundleInfo != null)
            {
                onLoadBytes(bundleInfo.bytes);
                bundleInfo.Unload();
            }
            else
                onLoadBytes(null);
        }
        else if (type == AssetBundleType.Texture)
        {
            if (bundleInfo == null || bundleInfo.texture == null)
            {
                onLoadTexture(null);
                return;
            }
            onLoadTexture(bundleInfo.texture);
            bundleInfo.Unload();
        }
    }
}