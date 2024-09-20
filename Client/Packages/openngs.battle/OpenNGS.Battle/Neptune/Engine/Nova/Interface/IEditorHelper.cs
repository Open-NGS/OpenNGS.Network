using UnityEngine;

namespace Neptune
{
    public interface IEditorHelper
    {
        void Label(Vector3 position, string text);
        void Label(Vector3 position, string text, Color color);
    }
}