#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using TMPro;

public static class ChangeTMPFont
{
    [MenuItem("AgroMijo/Change All TMP Fonts to Nunito Sans")]
    public static void ChangeFonts()
    {
        TMP_FontAsset nunito = FindNunitoFont();

        if (nunito == null)
        {
            Debug.LogError("No se encontró el Font Asset de Nunito Sans.");
            return;
        }

        TMP_Text[] texts = Object.FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Include
        );

        int changed = 0;

        foreach (TMP_Text text in texts)
        {
            Undo.RecordObject(text, "Change TMP Font to Nunito Sans");

            text.font = nunito;

            EditorUtility.SetDirty(text);

            changed++;
        }

        Debug.Log($"AgroMijo: {changed} textos TMP cambiados a Nunito Sans.");
    }

    private static TMP_FontAsset FindNunitoFont()
    {
        string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);

            if (font != null && font.name.Contains("NunitoSans"))
            {
                return font;
            }
        }

        return null;
    }
}

#endif