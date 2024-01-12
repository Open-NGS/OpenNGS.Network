using OpenNGS.Assets;
using System.Collections;
using System.Collections.Generic;
using OpenNGS.UI.UGUI;
using UnityEngine;
using UnityEngine.U2D;

namespace OpenNGS.UI
{
    public class UGUISystem : Singleton<UGUISystem>, IUISystem
    {
        public void InitSystem()
        {
            // preload fonts asset bundle
            PreloadFonts();

            // late binding atlas when sprite atlas packed in asset bundles
            SpriteAtlasManager.atlasRequested += RequestAtlas;
        }

        public IView CreateView(string id, string package, string component, int layer, bool cache)
        {
            var view = UIFactory.CreateView(package, component);
            OpenNGSDebug.Log($"UGUISystem CreateView {id}  {view}");
            view.Init(id, layer, cache);
            return view;
        }

        /// <summary>
        /// Todo:
        /// Move this function to UIManger.cs in Game
        /// Late binding atlas when sprite atlas packed in asset bundles
        /// </summary>
        private void RequestAtlas(string tag, System.Action<SpriteAtlas> callback)
        {
            Debug.Log($"Need Load Atlas {tag}");
            var sa = OpenNGSResources.Load<SpriteAtlas>($"UI/Atlas/{tag}.spriteatlas");
            if (sa != null)
            {
                Debug.Log($"Load Atlas {tag} success");
            }
            callback(sa);
        }

        /// <summary>
        /// Todo:
        /// Move this function to UIManger.cs in Game
        /// Load specified font assets in game build assets font folder 
        /// </summary>
        private void PreloadFonts()
        {
            OpenNGSResources.Load<Font>("UI/Fonts/OpenNGSSans-W3.ttf", float.MaxValue);
        }
    }
}
