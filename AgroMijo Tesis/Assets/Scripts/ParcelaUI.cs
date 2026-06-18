using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Coloca este script en cada botón/tarjeta de parcela del Canvas.
// Representa visualmente UNA Parcela de datos y muestra el check
// cuando el jugador ya tomó una decisión sobre ella en este ciclo.
public class ParcelaUI : MonoBehaviour
{
    [Header("Referencias de UI (arrástralas en el Inspector)")]
    public TMP_Text textoNombre;
    public GameObject iconoCheck; // una imagen simple de check, se activa/desactiva

    [Tooltip("Índice de esta parcela en la lista de GameManager.parcelas (0 = primera)")]
    public int indiceParcela = 0;

    [HideInInspector] public Parcela parcelaAsociada;

    private void Start()
    {
        // Se conecta automáticamente con la parcela de datos correspondiente
        parcelaAsociada = GameManager.Instancia.parcelas[indiceParcela];
        textoNombre.text = parcelaAsociada.nombreParcela;
        ActualizarVisual();

        GetComponent<Button>().onClick.AddListener(AlHacerClic);
    }

    private void AlHacerClic()
    {
        PanelDecisionUI.Instancia.Abrir(this);
    }

    public void ActualizarVisual()
    {
        iconoCheck.SetActive(parcelaAsociada.decisionTomada);
    }
}
