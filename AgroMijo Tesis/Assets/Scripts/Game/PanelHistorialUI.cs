using UnityEngine;

// Coloca este script en Panel_Historial (inicialmente activo, se oculta en Awake).
// Accesible desde un botón del HUD — "Ver historial".
// Se regenera cada vez que se abre para mostrar los ciclos más recientes.
public class PanelHistorialUI : MonoBehaviour
{
    public static PanelHistorialUI Instancia { get; private set; }

    public GameObject    panelPrincipal;
    public Transform     contenedorFilas;  // Vertical Layout Group + Content Size Fitter
    public FilaHistorialUI filaPrefab;

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    // Conecta al botón "Ver historial" del HUD
    public void Abrir()
    {
        Refrescar();
        panelPrincipal.SetActive(true);
    }

    public void Cerrar()
    {
        panelPrincipal.SetActive(false);
    }

    private void Refrescar()
    {
        foreach (Transform hijo in contenedorFilas)
            Destroy(hijo.gameObject);

        var historial = GameManager.Instancia.historialCiclos;

        if (historial.Count == 0)
        {
            FilaHistorialUI fila = Instantiate(filaPrefab, contenedorFilas);
            fila.textoEncabezado.text = "Sin ciclos completados aun.";
            fila.textoResumen.text    = "";
            fila.botonDetalle.gameObject.SetActive(false);
            return;
        }

        // Mostrar del más reciente al más antiguo
        for (int i = historial.Count - 1; i >= 0; i--)
        {
            FilaHistorialUI fila = Instantiate(filaPrefab, contenedorFilas);
            fila.Setup(historial[i]);
        }
    }
}
