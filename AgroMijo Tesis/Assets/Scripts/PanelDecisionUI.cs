using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Coloca este script en el panel de decisión (inicialmente inactivo en la jerarquía).
// Se abre cuando el jugador hace clic en una parcela.
public class PanelDecisionUI : MonoBehaviour
{
    public static PanelDecisionUI Instancia { get; private set; }

    [Header("Referencias de UI")]
    public GameObject panelPrincipal;          // el panel completo, se activa/desactiva
    public TMP_Text textoTituloParcela;
    public GameObject panelSeleccionCultivo;   // submenú con los botones de cultivo, inactivo al inicio
    public Transform contenedorBotonesCultivo; // contenedor donde se generan los botones de cultivo
    public Button botonCultivoPrefab;          // prefab de un botón simple con un Text hijo

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
        panelSeleccionCultivo.SetActive(true);
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
            boton.GetComponentInChildren<Text>().text = $"{cultivo.nombreCultivo} (${cultivo.costoSemilla:N0})";
            boton.onClick.AddListener(() => ConfirmarPlantar(cultivo));
        }
    }

    private void ConfirmarPlantar(CultivoData cultivo)
    {
        bool exito = GameManager.Instancia.IntentarPlantar(parcelaActual.parcelaAsociada, cultivo);
        if (exito) FinalizarDecision();
        // si exito es false, no había presupuesto suficiente; aquí podrías mostrar un aviso
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
