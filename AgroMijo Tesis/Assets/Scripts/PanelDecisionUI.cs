using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelDecisionUI : MonoBehaviour
{
    public static PanelDecisionUI Instancia { get; private set; }

    [Header("Estructura general")]
    public GameObject panelPrincipal;
    public TMP_Text textoTituloParcela;

    [Header("Submenú: opciones principales")]
    public GameObject panelOpcionesPrincipales;
    public Button botonPlantar;   // se desactiva si la parcela ya tiene cultivo creciendo
    public Button botonEstudiar;  // se desactiva si la parcela ya fue estudiada

    [Header("Submenú: selección de cultivo")]
    public GameObject panelSeleccionCultivo;
    public Transform  contenedorBotonesCultivo; // contenedor EXCLUSIVO para botones generados
    public Button     botonCultivoPrefab;

    private ParcelaUI parcelaActual;

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    public void Abrir(ParcelaUI parcelaUI)
    {
        parcelaActual = parcelaUI;
        Parcela p = parcelaUI.parcelaAsociada;

        textoTituloParcela.text = p.nombreParcela;

        // Plantar solo si la parcela está vacía Y no hay ya una decisión de plantar pendiente
        botonPlantar.interactable  = p.estado == EstadoParcela.Vacia;
        // Estudiar solo si aún no fue estudiada (pagar dos veces no tiene sentido)
        botonEstudiar.interactable = !p.estudiada;

        panelOpcionesPrincipales.SetActive(true);
        panelSeleccionCultivo.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    public void Cerrar()
    {
        panelPrincipal.SetActive(false);
        parcelaActual = null;
    }

    // ── Botón "Plantar cultivo" ───────────────────────────────────────────────
    public void OnClickPlantar()
    {
        GenerarBotonesDeCultivo();
        panelOpcionesPrincipales.SetActive(false);
        panelSeleccionCultivo.SetActive(true);
    }

    public void OnClickVolverDesdeCultivos()
    {
        panelSeleccionCultivo.SetActive(false);
        panelOpcionesPrincipales.SetActive(true);
    }

    private void GenerarBotonesDeCultivo()
    {
        foreach (Transform hijo in contenedorBotonesCultivo)
            Destroy(hijo.gameObject);

        foreach (CultivoData cultivo in GameManager.Instancia.cultivosDisponibles)
        {
            Button boton = Instantiate(botonCultivoPrefab, contenedorBotonesCultivo);
            boton.GetComponentInChildren<TMP_Text>().text =
                $"{cultivo.nombreCultivo}  (${cultivo.costoSemilla:N0})";
            boton.onClick.AddListener(() => ConfirmarPlantar(cultivo));
        }
    }

    private void ConfirmarPlantar(CultivoData cultivo)
    {
        // Solo guarda la decisión — el dinero se cobra al avanzar el ciclo
        GameManager.Instancia.GuardarDecisionPlantar(parcelaActual.parcelaAsociada, cultivo);
        FinalizarDecision();
    }

    // ── Botón "Estudiar terreno" ──────────────────────────────────────────────
    public void OnClickEstudiar()
    {
        GameManager.Instancia.GuardarDecisionEstudiar(parcelaActual.parcelaAsociada);
        FinalizarDecision();
    }

    // ── Botón "Mejorar drenaje" ───────────────────────────────────────────────
    public void OnClickMejorar()
    {
        GameManager.Instancia.GuardarDecisionMejorar(parcelaActual.parcelaAsociada);
        FinalizarDecision();
    }

    // ── Botón "Esperar" ───────────────────────────────────────────────────────
    public void OnClickEsperar()
    {
        GameManager.Instancia.GuardarDecisionEsperar(parcelaActual.parcelaAsociada);
        FinalizarDecision();
    }

    private void FinalizarDecision()
    {
        parcelaActual.ActualizarVisual();
        Cerrar();
    }
}
