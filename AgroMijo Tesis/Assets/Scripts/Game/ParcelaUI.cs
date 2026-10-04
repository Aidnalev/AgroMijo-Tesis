using UnityEngine;
using TMPro;

// Ya no es un Button. Es un panel informativo que se abre y cierra
// junto con el PanelDecisionUI cuando el jugador clickea el cubo en el mundo.
public class ParcelaUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TMP_Text textoNombre;
    public TMP_Text textoCultivo;
    public TMP_Text textoSuelo;
    public TMP_Text textoAgua;
    public TMP_Text textoMejoras;
    public GameObject iconoCheck;

    [Tooltip("Índice en GameManager.parcelas (0 = primera)")]
    public int indiceParcela = 0;

    [Tooltip("Cubo del mundo 3D correspondiente (opcional)")]
    public ParcelaMundo parcelaMundo;

    [HideInInspector] public Parcela parcelaAsociada;

    // Inicialización lazy: se inicializa la primera vez que se abre
    private bool inicializado = false;

    // El panel empieza INACTIVO en el editor — ParcelaMundo lo activa al hacer clic
    public void Abrir()
    {
        if (!inicializado)
        {
            parcelaAsociada  = GameManager.Instancia.parcelas[indiceParcela];
            inicializado     = true;
        }
        gameObject.SetActive(true);
        textoNombre.text = parcelaAsociada.nombreParcela;
        ActualizarVisual();
    }

    public void Cerrar()
    {
        gameObject.SetActive(false);
    }

    public void ActualizarVisual()
    {
        if (parcelaAsociada == null) return; // aún no inicializado

        iconoCheck.SetActive(parcelaAsociada.decisionTomada);

        textoSuelo.text = parcelaAsociada.estudiada
            ? $"Suelo: {parcelaAsociada.tipoSuelo}"
            : "Suelo: ?";

        int barras = Mathf.RoundToInt(parcelaAsociada.disponibilidadAgua * 5);
        string barraAgua = new string('|', barras).PadRight(5, '.');
        textoAgua.text = $"Agua: [{barraAgua}] {parcelaAsociada.disponibilidadAgua * 100f:F0}%";

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

        if (parcelaAsociada.estado == EstadoParcela.Plantada)
        {
            int ciclosRestantes = parcelaAsociada.cultivoActual.duracionCiclos
                                - (GameManager.Instancia.cicloActual - parcelaAsociada.cicloEnQueSePlanto);
            textoCultivo.text = $"{parcelaAsociada.cultivoActual.nombreCultivo}  ({ciclosRestantes} periodo(s))";
        }
        else
        {
            textoCultivo.text = "Vacia";
        }

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

        parcelaMundo?.ActualizarVisual3D();
    }
}
