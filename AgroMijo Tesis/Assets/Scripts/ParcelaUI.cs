using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ParcelaUI : MonoBehaviour
{
    [Header("Referencias de UI (arrástralas en el Inspector)")]
    public TMP_Text textoNombre;
    public TMP_Text textoCultivo;  // muestra estado del terreno o decisión pendiente
    public GameObject iconoCheck;

    [Tooltip("Índice de esta parcela en la lista de GameManager.parcelas (0 = primera)")]
    public int indiceParcela = 0;

    [HideInInspector] public Parcela parcelaAsociada;

    private void Start()
    {
        parcelaAsociada    = GameManager.Instancia.parcelas[indiceParcela];
        textoNombre.text   = parcelaAsociada.nombreParcela;
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

        // Primero mostramos la decisión pendiente si ya se tomó una
        switch (parcelaAsociada.decisionPendiente.tipo)
        {
            case TipoDecision.Plantar:
                string nombreCultivo = parcelaAsociada.decisionPendiente.cultivoSeleccionado?.nombreCultivo ?? "?";
                textoCultivo.text = $"► Plantar: {nombreCultivo}";
                return;

            case TipoDecision.Estudiar:
                textoCultivo.text = "► Estudiar terreno";
                return;

            case TipoDecision.Mejorar:
                textoCultivo.text = "► Mejorar drenaje";
                return;

            case TipoDecision.Esperar:
                textoCultivo.text = "► Esperando";
                return;
        }

        // Si no hay decisión pendiente, mostramos el estado real de la parcela
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
