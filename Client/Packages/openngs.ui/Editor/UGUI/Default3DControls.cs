using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;

namespace OpenNGS.UI
{
    /// <summary>
    /// Class for Image3D Create Menu
    /// </summary>
    public static class Default3DControls
    {
        private const float  kWidth       = 160f;
        private const float  kThickHeight = 60f;

        private static Vector2 s_ThickElementSize       = new Vector2(kWidth, kThickHeight);
        private static Vector2 s_ImageElementSize       = new Vector2(100f, 100f);
        private static Color   s_DefaultColor           = new Color(18f / 255f, 138f / 255f, 1f);
        private static GameObject CreateUIElementRoot(string name, Vector2 size, params Type[] components)
        {
            GameObject child = DefaultControls.factory.CreateGameObject(name, components);
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }
        
        private static GameObject CreateUIObject(string name, GameObject parent, params Type[] components)
        {
            GameObject go = DefaultControls.factory.CreateGameObject(name, components);
            UI3DMenuOptions.SetParentAndAlign(go, parent);
            return go;
        }
        
        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor     = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor    = new Color(0.521f, 0.521f, 0.521f);
        }

        public static GameObject CreateImage3D()
        {
            GameObject go = CreateUIElementRoot("Image3D", s_ImageElementSize, typeof(Image3D));
            var image = go.GetComponent<Image3D>();
            image.material = Resources.Load<Material>("Materials/UI3DMaterial-Diffuse");
            image.sprite3D = Resources.Load<Sprite3D>("Sprite3D/Rectangle");
            image.color = s_DefaultColor;
            return go;
        }

        public static GameObject CreateButton3D()
        {
            GameObject buttonRoot = CreateUIElementRoot("Button3D", s_ThickElementSize, typeof(Image3D), typeof(Button));

            GameObject childText = CreateUIObject("Text", buttonRoot, typeof(Text));

            Image3D image = buttonRoot.GetComponent<Image3D>();
            image.material = Resources.Load<Material>("Materials/UI3DMaterial-Diffuse");
            image.thickness = 50f;
            image.sprite3D = Resources.Load<Sprite3D>("Sprite3D/Rounded Rectangle Radius@20x");
            image.color = s_DefaultColor;
            
            Button bt = buttonRoot.GetComponent<Button>();
            SetDefaultColorTransitionValues(bt);

            Text text = childText.GetComponent<Text>();
            text.text = "Button3D";
            text.alignment = TextAnchor.MiddleCenter;

            Vector3 pos = text.transform.localPosition;
            pos.z = -0.01f;
            text.transform.localPosition = pos;
            
            if (text.font == null)
            {
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;

            return buttonRoot;
        }
        
        public static GameObject CreateSlider3D()
        {
            // todo:
            GameObject go = new GameObject();
            return go;
        }
        
        public static GameObject CreateInputField3D()
        {
            // todo:
            GameObject go = new GameObject();
            return go;
        }
    }
}
