using TMPro;
using UnityEditor;
using UnityEngine;

public class ChangeTMPColor : EditorWindow
{
    [MenuItem("Tools/AgroMijo/Actualizar Colores de Texto")]
    public static void UpdateTextColors()
    {
        TMP_Text[] texts = Object.FindObjectsByType<TMP_Text>();

        foreach (TMP_Text text in texts)
        {
            text.color = Color.black;
            EditorUtility.SetDirty(text);
        }

        Debug.Log($"Se actualizaron {texts.Length} textos a negro puro.");
    }
}