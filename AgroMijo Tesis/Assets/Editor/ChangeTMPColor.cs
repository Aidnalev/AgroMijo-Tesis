using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTMPColor : EditorWindow
{
    [MenuItem("Tools/AgroMijo/Actualizar Colores de Texto")]
    public static void UpdateTextColors()
    {
        TMP_Text[] texts = Object.FindObjectsByType<TMP_Text>();

        int blancos = 0;
        int negros = 0;

        foreach (TMP_Text text in texts)
        {
            // Si el texto está dentro de un Button → blanco
            Button button = text.GetComponentInParent<Button>();

            if (button != null)
            {
                text.color = Color.white;
                blancos++;
            }
            else
            {
                // El resto de textos → negro
                text.color = Color.black;
                negros++;
            }

            EditorUtility.SetDirty(text);
        }

        Debug.Log($"Botones: {blancos} textos blancos | Otros: {negros} textos negros.");
    }
}