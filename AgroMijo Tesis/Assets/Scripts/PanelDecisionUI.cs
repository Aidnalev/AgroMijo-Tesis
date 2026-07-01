using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Coloca este script en el panel de decisión (inicialmente activo en la jerarquía,
// el propio script lo oculta en Awake()).
public class PanelDecisionUI : MonoBehaviour
{
    public static PanelDecisionUI Instancia { get; private set; }

    [Header("Estructura general")]
    public GameObject panelPrincipal;       // Panel_Decision completo
    public TMP_Text textoTituloParcela;

    [Header("Submenú: opciones principales")]
    [Tooltip("Objeto que agrupa los botones Plantar/Estudiar/Mejorar/Esperar")]
    public GameObject panelOpcionesPrincipales;
    public Button botonPlantar; // se desactiva si la parcela ya tiene un cultivo creciendo

    [Header("Submenú: selección de cultivo")]
    [Tooltip("El submenú completo de selección de cultivo (incluye el botón Volver)")]
    public GameObject panelSeleccionCultivo;

    [Tooltip("OJO: este debe ser un contenedor EXCLUSIVO solo para los botones de cultivo generados. NO uses aquí Panel_Decision, Panel_Cultivos completo, ni el Canvas — si lo haces, Destroy() borrará de más.")]
    public Transform contenedorBotonesCultivo;

    public Button botonCultivoPrefab;

    [Header("Costos de referencia (ajusta luego con datos reales)")]
    public float costoEstudio = 30000f;
    public float costoMejora = 80000f;

    private ParcelaUI parcelaActual;

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    public void Abrir(ParcelaUI parcela)
    {
        parcelaActual = parcela;
        textoTituloParcela.text = parcela.parcelaAsociada.nombreParcela;

        bool yaPlantada = parcela.parcelaAsociada.estado == EstadoParcela.Plantada;
        botonPlantar.interactable = !yaPlantada;

        panelOpcionesPrincipales.SetActive(true);
        panelSeleccionCultivo.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    public void Cerrar()
    {
        panelPrincipal.SetActive(false);
        parcelaActual = null;
    }

    // Conecta esto al botón "Plantar cultivo"
    public void OnClickPlantar()
    {
        GenerarBotonesDeCultivo();
        panelOpcionesPrincipales.SetActive(false);
        panelSeleccionCultivo.SetActive(true);
    }

    // Conecta esto al botón "Volver" dentro del submenú de selección de cultivo
    public void OnClickVolverDesdeCultivos()
    {
        panelSeleccionCultivo.SetActive(false);
        panelOpcionesPrincipales.SetActive(true);
    }

    private void GenerarBotonesDeCultivo()
    {
        foreach (Transform hijo in contenedorBotonesCultivo)
        {
            Destroy(hijo.gameObject);
        }

        foreach (CultivoData cultivo in GameManager.Instancia.cultivosDisponibles)
        {
            Button boton = Instantiate(botonCultivoPrefab, contenedorBotonesCultivo);
            boton.GetComponentInChildren<TMP_Text>().text = $"{cultivo.nombreCultivo} (${cultivo.costoSemilla:N0})";
            boton.onClick.AddListener(() => ConfirmarPlantar(cultivo));
        }
    }

    private void ConfirmarPlantar(CultivoData cultivo)
    {
        bool exito = GameManager.Instancia.IntentarPlantar(parcelaActual.parcelaAsociada, cultivo);
        if (exito) FinalizarDecision();
    }

    // Conecta esto al botón "Estudiar terreno"
    public void OnClickEstudiar()
    {
        bool exito = GameManager.Instancia.IntentarEstudiarTerreno(parcelaActual.parcelaAsociada, costoEstudio);
        if (exito) FinalizarDecision();
    }

    // Conecta esto al botón "Mejorar drenaje"
    public void OnClickMejorar()
    {
        bool exito = GameManager.Instancia.IntentarMejorarDrenaje(parcelaActual.parcelaAsociada, costoMejora);
        if (exito) FinalizarDecision();
    }

    // Conecta esto al botón "Esperar"
    public void OnClickEsperar()
    {
        GameManager.Instancia.IntentarEsperar(parcelaActual.parcelaAsociada);
        FinalizarDecision();
    }

    private void FinalizarDecision()
    {
        parcelaActual.ActualizarVisual();
        Cerrar();
    }
}