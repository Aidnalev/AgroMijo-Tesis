using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Coloca este script en cada botón/tarjeta de parcela del Canvas.
public class ParcelaUI : MonoBehaviour
{
    [Header("Referencias de UI (arrástralas en el Inspector)")]
    public TMP_Text textoNombre;
    public GameObject iconoCheck;

    [Tooltip("Índice de esta parcela en la lista de GameManager.parcelas (0 = primera)")]
    public int indiceParcela = 0;

    [HideInInspector] public Parcela parcelaAsociada;

    private void Start()
    {
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
