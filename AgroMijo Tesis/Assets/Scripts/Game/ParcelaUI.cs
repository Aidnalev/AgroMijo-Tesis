using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ParcelaUI : MonoBehaviour
{
    [Header("Referencias de UI (arrástralas en el Inspector)")]
    public TMP_Text textoNombre;
    public TMP_Text textoCultivo;
    public TMP_Text textoSuelo;   // muestra "Suelo: ?" hasta que se estudie
    public TMP_Text textoAgua;    // muestra disponibilidad de agua actual
    public TMP_Text textoMejoras; // muestra el nivel de mejoras actual
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

        // Suelo: oculto hasta que el jugador pague el estudio
        textoSuelo.text = parcelaAsociada.estudiada
            ? $"Suelo: {parcelaAsociada.tipoSuelo}"
            : "Suelo: ?";

        // Agua: valor actual con una barra visual simple
        int barras = Mathf.RoundToInt(parcelaAsociada.disponibilidadAgua * 5);
        string barraAgua = new string('|', barras).PadRight(5, '.');
        textoAgua.text = $"Agua: [{barraAgua}] {parcelaAsociada.disponibilidadAgua * 100f:F0}%";

        // Mejoras: nivel actual y qué tiene
        string descMejora = parcelaAsociada.nivelMejora switch
        {
            0 => "Sin mejoras",
            1 => "Vial",
            2 => "Vial + Riego",
            3 => "Vial + Riego + Fertiliz.",
            _ => ""
        };
        textoMejoras.text = parcelaAsociada.nivelMejora < 3
            ? $"Mejoras: {descMejora} ({parcelaAsociada.nivelMejora}/3)"
            : $"Mejoras: {descMejora} (Max)";

        // Cultivo: siempre visible, independiente de la decisión pendiente
        if (parcelaAsociada.estado == EstadoParcela.Plantada)
        {
            int ciclosRestantes = parcelaAsociada.cultivoActual.duracionCiclos
                                - (GameManager.Instancia.cicloActual - parcelaAsociada.cicloEnQueSePlanto);
            textoCultivo.text = $"{parcelaAsociada.cultivoActual.nombreCultivo}  ({ciclosRestantes} ciclo(s))";
        }
        else
        {
            textoCultivo.text = "Vacía";
        }

        // Decisión pendiente: se muestra junto al nombre de la parcela
        string decisionTexto = parcelaAsociada.decisionPendiente.tipo switch
        {
            TipoDecision.Plantar  => $"Plantar: {parcelaAsociada.decisionPendiente.cultivoSeleccionado?.nombreCultivo ?? "?"}",
            TipoDecision.Estudiar => "Estudiar terreno",
            TipoDecision.Mejorar  => "Mejorando",
            TipoDecision.Esperar  => "Esperar",
            _                     => ""
        };

        textoNombre.text = string.IsNullOrEmpty(decisionTexto)
            ? parcelaAsociada.nombreParcela
            : $"{parcelaAsociada.nombreParcela}  [{decisionTexto}]";
    }
}
