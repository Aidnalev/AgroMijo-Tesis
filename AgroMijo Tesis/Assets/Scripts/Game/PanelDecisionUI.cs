
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelDecisionUI : MonoBehaviour
{
    public static PanelDecisionUI Instancia { get; private set; }

    [Header("Estructura general")]
    public GameObject panelPrincipal;
    public TMP_Text textoTituloParcela;
    public TMP_Text textoInfoParcela;

    [Header("Submenú: opciones principales")]
    public GameObject panelOpcionesPrincipales;
    public Button botonPlantar;
    public Button botonEstudiar;
    public Button botonMejorar;

    [Header("Submenú: selección de cultivo")]
    public GameObject panelSeleccionCultivo;
    public Transform contenedorBotonesCultivo;
    public Button botonCultivoPrefab;

    private ParcelaUI parcelaActual;
    private ParcelaMundo parcelaMundoActual;

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    public void Abrir(ParcelaUI parcelaUI)
    {
        if (parcelaActual != null && parcelaActual != parcelaUI)
        {
            parcelaActual.Cerrar();

            if (parcelaMundoActual != null)
                parcelaMundoActual.EstablecerSeleccionada(false);
        }

        parcelaActual = parcelaUI;

        // Busca el objeto 3D asociado a esta parcela.
        parcelaMundoActual = null;

        foreach (ParcelaMundo parcelaMundo in FindObjectsByType<ParcelaMundo>())
        {
            if (parcelaMundo.parcelaUI == parcelaUI)
            {
                parcelaMundoActual = parcelaMundo;
                break;
            }
        }

        if (parcelaMundoActual != null)
            parcelaMundoActual.EstablecerSeleccionada(true);

        Parcela p = parcelaUI.parcelaAsociada;

        textoTituloParcela.text = p.nombreParcela;

        textoInfoParcela.text = p.estudiada
            ? $"Suelo: {p.tipoSuelo}"
            : "Suelo: desconocido  (estudia el terreno para revelar)";

        botonPlantar.interactable = p.estado == EstadoParcela.Vacia;
        botonEstudiar.interactable = !p.estudiada;
        botonMejorar.interactable = p.nivelMejora < 3;

        panelOpcionesPrincipales.SetActive(true);
        panelSeleccionCultivo.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    public void Cerrar()
    {
        parcelaActual?.Cerrar();

        if (parcelaMundoActual != null)
            parcelaMundoActual.EstablecerSeleccionada(false);

        parcelaMundoActual = null;
        parcelaActual = null;

        panelPrincipal.SetActive(false);
    }

    public void OnClickPlantar()
    {
        TutorialManager.Instancia?.NotificarAccion(
            TutorialCondicion.PlantarClickeado
        );

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
            Button boton = Instantiate(
                botonCultivoPrefab,
                contenedorBotonesCultivo
            );

            boton.GetComponentInChildren<TMP_Text>().text =
                $"{cultivo.nombreCultivo}  (${cultivo.costoSemilla:N0})";

            boton.onClick.AddListener(
                () => ConfirmarPlantar(cultivo)
            );
        }
    }

    private void ConfirmarPlantar(CultivoData cultivo)
    {
        GameManager.Instancia.GuardarDecisionPlantar(
            parcelaActual.parcelaAsociada,
            cultivo
        );

        TutorialManager.Instancia?.NotificarAccion(
            TutorialCondicion.CultivoSeleccionado
        );

        FinalizarDecision();
    }

    public void OnClickEstudiar()
    {
        GameManager.Instancia.GuardarDecisionEstudiar(
            parcelaActual.parcelaAsociada
        );

        TutorialManager.Instancia?.NotificarAccion(
            TutorialCondicion.EstudiarClickeado
        );

        FinalizarDecision();
    }

    public void OnClickMejorar()
    {
        GameManager.Instancia.GuardarDecisionMejorar(
            parcelaActual.parcelaAsociada
        );

        TutorialManager.Instancia?.NotificarAccion(
            TutorialCondicion.MejorarClickeado
        );

        FinalizarDecision();
    }

    public void OnClickEsperar()
    {
        GameManager.Instancia.GuardarDecisionEsperar(
            parcelaActual.parcelaAsociada
        );

        TutorialManager.Instancia?.NotificarAccion(
            TutorialCondicion.EsperarClickeado
        );

        FinalizarDecision();
    }

    private void FinalizarDecision()
    {
        parcelaActual.ActualizarVisual();
        HUDManager.Instancia.ActualizarHUD();
        Cerrar();
    }
}
