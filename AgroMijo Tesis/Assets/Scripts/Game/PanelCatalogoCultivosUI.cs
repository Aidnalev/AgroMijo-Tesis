using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Controla el catálogo de cultivos.
// Genera los botones de selección y muestra la información
// del cultivo seleccionado.
public class PanelCatalogoCultivosUI : MonoBehaviour
{
    public static PanelCatalogoCultivosUI Instancia { get; private set; }

    [Header("Panel principal")]
    public GameObject panelPrincipal;

    [Header("Selección de cultivos")]
    public Transform contenedorBotones;
    public Button botonCultivoPrefab;

    [Header("Información del cultivo")]
    public TMP_Text textoNombre;

    [Header("Economía")]
    public TMP_Text textoSemilla;
    public TMP_Text textoRendimiento;
    public TMP_Text textoDuracion;

    [Header("Jornales")]
    public TMP_Text textoPlantar;
    public TMP_Text textoMantenimiento;
    public TMP_Text textoCosecha;

    [Header("Suelos")]
    public TMP_Text textoArcilloso;
    public TMP_Text textoArenoso;
    public TMP_Text textoFranco;
    public TMP_Text textoFrancoArcilloso;
    public TMP_Text textoFrancoArenoso;

    private bool generado = false;

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    public void Abrir()
    {
        if (!generado)
            GenerarCatalogo();

        panelPrincipal.SetActive(true);

        TutorialManager.Instancia?.NotificarAccion(
            TutorialCondicion.CatalogoAbierto
        );
    }

    public void Cerrar()
    {
        panelPrincipal.SetActive(false);

        TutorialManager.Instancia?.NotificarAccion(
            TutorialCondicion.CatalogoCerrado
        );
    }

    private void GenerarCatalogo()
    {
        foreach (CultivoData cultivo in GameManager.Instancia.cultivosDisponibles)
        {
            Button boton = Instantiate(
                botonCultivoPrefab,
                contenedorBotones
            );

            boton.GetComponentInChildren<TMP_Text>().text =
                cultivo.nombreCultivo;

            CultivoData cultivoSeleccionado = cultivo;

            boton.onClick.AddListener(() =>
            {
                MostrarCultivo(cultivoSeleccionado);
            });
        }

        // Mostrar el primer cultivo automáticamente
        if (GameManager.Instancia.cultivosDisponibles.Count > 0)
        {
            MostrarCultivo(
                GameManager.Instancia.cultivosDisponibles[0]
            );
        }

        generado = true;
    }

    private void MostrarCultivo(CultivoData cultivo)
    {
        textoNombre.text = cultivo.nombreCultivo;

        // Economía
        textoSemilla.text =
            $"${cultivo.costoSemilla:N0}";

        textoRendimiento.text =
            $"${cultivo.rendimientoBase:N0}";

        textoDuracion.text =
            $"{cultivo.duracionCiclos} periodo(s)";

        // Jornales
        textoPlantar.text =
            $"{cultivo.jornalesParaPlantar}";

        textoMantenimiento.text =
            $"{cultivo.jornalesMantenimientoPorCiclo}/periodo";

        textoCosecha.text =
            $"{cultivo.jornalesParaCosechar}";

        // Suelos
        textoArcilloso.text =
            FormatearMod(cultivo.modificadorArcilloso);

        textoArenoso.text =
            FormatearMod(cultivo.modificadorArenoso);

        textoFranco.text =
            FormatearMod(cultivo.modificadorFranco);

        textoFrancoArcilloso.text =
            FormatearMod(cultivo.modificadorFrancoArcilloso);

        textoFrancoArenoso.text =
            FormatearMod(cultivo.modificadorFrancoArenoso);
    }

    private string FormatearMod(float mod)
    {
        float porcentaje = (mod - 1f) * 100f;

        if (Mathf.Abs(porcentaje) < 0.5f)
            return "Neutro";

        return porcentaje > 0
            ? $"+{porcentaje:F0}%"
            : $"{porcentaje:F0}%";
    }
}