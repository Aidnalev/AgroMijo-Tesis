using UnityEngine;
using TMPro;

// Coloca este script en Panel_Catalogo (inicialmente activo, se oculta en Awake).
// Se abre desde un botón del HUD — sugerencia: llámalo "Ver cultivos".
// Genera automáticamente una fila por cada cultivo en GameManager.cultivosDisponibles.
public class PanelCatalogoCultivosUI : MonoBehaviour
{
    public static PanelCatalogoCultivosUI Instancia { get; private set; }

    [Header("Estructura")]
    public GameObject    panelPrincipal;
    public Transform     contenedorFilas;  // Vertical Layout Group aquí
    public FilaCultivoUI filasPrefab;      // prefab con los 4 TMP_Text

    private bool generado = false; // el catálogo no cambia en juego, solo se genera una vez

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    // Conecta al botón "Ver cultivos" del HUD
    public void Abrir()
    {
        if (!generado) GenerarCatalogo();
        panelPrincipal.SetActive(true);
    }

    public void Cerrar()
    {
        panelPrincipal.SetActive(false);
    }

    private void GenerarCatalogo()
    {
        foreach (CultivoData cultivo in GameManager.Instancia.cultivosDisponibles)
        {
            FilaCultivoUI fila = Instantiate(filasPrefab, contenedorFilas);

            fila.textoNombre.text = cultivo.nombreCultivo;

            fila.textoEconomia.text =
                $"Semilla: ${cultivo.costoSemilla:N0}  |  " +
                $"Rendimiento base: ${cultivo.rendimientoBase:N0}  |  " +
                $"Duracion: {cultivo.duracionCiclos} ciclo(s)";

            fila.textoJornales.text =
                $"Jornales — " +
                $"Plantar: {cultivo.jornalesParaPlantar}  |  " +
                $"Mantenimiento: {cultivo.jornalesMantenimientoPorCiclo}/ciclo  |  " +
                $"Cosecha: {cultivo.jornalesParaCosechar}";

            fila.textoSuelos.text =
                $"Suelo — " +
                $"Arcilloso: {FormatearMod(cultivo.modificadorArcilloso)}  |  " +
                $"Arenoso: {FormatearMod(cultivo.modificadorArenoso)}  |  " +
                $"Franco: {FormatearMod(cultivo.modificadorFranco)}  |  " +
                $"Limoso: {FormatearMod(cultivo.modificadorLimoso)}";
        }

        generado = true;
    }

    // Convierte el modificador float a texto legible:
    // 1.2 → "+20%"  |  0.8 → "-20%"  |  1.0 → "neutro"
    private string FormatearMod(float mod)
    {
        float porcentaje = (mod - 1f) * 100f;
        if (Mathf.Abs(porcentaje) < 0.5f) return "neutro";
        return porcentaje > 0
            ? $"+{porcentaje:F0}%"
            : $"{porcentaje:F0}%";
    }
}
