using UnityEngine;
using TMPro;

// Ponle este script al prefab de fila del catálogo.
// Arrastra cada Text al campo correspondiente en el Inspector del prefab.
// Estructura sugerida del prefab:
//   - Txt_Nombre       → nombre del cultivo (más grande o en negrita)
//   - Txt_Economia     → costo de semilla, rendimiento base y duración
//   - Txt_Jornales     → jornales para plantar, mantener y cosechar
//   - Txt_Suelos       → modificadores por tipo de suelo
public class FilaCultivoUI : MonoBehaviour
{
    public TMP_Text textoNombre;
    public TMP_Text textoEconomia;
    public TMP_Text textoJornales;
    public TMP_Text textoSuelos;
}
