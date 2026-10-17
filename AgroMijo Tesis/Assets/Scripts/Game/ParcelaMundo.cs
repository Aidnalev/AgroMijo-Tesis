
using UnityEngine;
using UnityEngine.EventSystems;

public class ParcelaMundo : MonoBehaviour
{
    [Tooltip("La ParcelaUI correspondiente a este cubo en el Canvas")]
    public ParcelaUI parcelaUI;

    [Header("Visuales opcionales del cubo 3D")]
    public GameObject iconoCheck;
    public Renderer rendererCubo;

    [Header("Colores de la parcela")]
    public Color colorVacio = new Color(0.76f, 0.60f, 0.42f);
    public Color colorPlantado = new Color(0.30f, 0.65f, 0.25f);
    public Color colorSeleccionado = new Color(1f, 0.85f, 0.15f);

    private bool seleccionada = false;

    private void Start()
    {
        ActualizarVisual3D();
    }

    private void OnMouseDown()
    {
        // Si el click ocurrió sobre la interfaz, no hacer nada.
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        TutorialManager.Instancia?.NotificarAccion(
            TutorialCondicion.ParcelaClickeada
        );

        parcelaUI.Abrir();
        PanelDecisionUI.Instancia.Abrir(parcelaUI);
    }

    public void EstablecerSeleccionada(bool seleccionada)
    {
        this.seleccionada = seleccionada;
        ActualizarVisual3D();
    }

    public void ActualizarVisual3D()
    {
        if (parcelaUI == null) return;

        if (iconoCheck != null)
        {
            iconoCheck.SetActive(
                parcelaUI.parcelaAsociada.decisionTomada
            );
        }

        if (rendererCubo != null)
        {
            rendererCubo.material.color = seleccionada
                ? colorSeleccionado
                : parcelaUI.parcelaAsociada.estado == EstadoParcela.Plantada
                    ? colorPlantado
                    : colorVacio;
        }
    }
}
