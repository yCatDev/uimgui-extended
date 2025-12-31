using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Dear ImGui/Create CustomFontByteData")]
public class CustomFontByteData : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField] private Font font;
#endif
    
    [HideInInspector, SerializeField] private byte[] fontData;

    public byte[] FontData => fontData;

#if UNITY_EDITOR
    private void OnValidate()
    {
        var path = Path.Combine(Application.dataPath, AssetDatabase.GetAssetPath(font).Substring("Assets".Length + 1));
        fontData = File.ReadAllBytes(path);
    }
#endif
}