using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Coloca este script en cada botón/tarjeta de parcela del Canvas.
public class ParcelaUI : MonoBehaviour
{
    [Header("Referencias de UI (arrástralas en el Inspector)")]
    public TMP_Text textoNombre;
    public TMP_Text textoCultivo; // muestra qué hay plantado y cuántos ciclos faltan
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

        if (parcelaAsociada.estado == EstadoParcela.Plantada)
        {
            int ciclosRestantes = parcelaAsociada.cultivoActual.duracionCiclos
                                - (GameManager.Instancia.cicloActual - parcelaAsociada.cicloEnQueSePlanto);
            textoCultivo.text = $"{parcelaAsociada.cultivoActual.nombreCultivo}\n{ciclosRestantes} ciclo(s) para cosechar";
        }
        else
        {
            textoCultivo.text = "Vacía";
        }
    }
}