using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanelEventosUI : MonoBehaviour
{
    public static PanelEventosUI Instancia { get; private set; }

    [Header("Estructura")]
    public GameObject  panelPrincipal;
    public Transform   contenedorFilasEvento;
    public FilaEventoUI filaEventoPrefab;

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    public void Abrir()
    {
        RefrescarLista();
        panelPrincipal.SetActive(true);
    }

    public void Cerrar()
    {
        panelPrincipal.SetActive(false);
    }

    private void RefrescarLista()
    {
        foreach (Transform hijo in contenedorFilasEvento)
            Destroy(hijo.gameObject);

        List<EventoGlobalActivo> activos = GameManager.Instancia.eventosActivosPersistentes;
        bool hayAlguno = false;

        foreach (EventoGlobalActivo activo in activos)
        {
            if (activo.resuelto) continue;
            hayAlguno = true;

            FilaEventoUI fila = Instantiate(filaEventoPrefab, contenedorFilasEvento);
            fila.textoNombre.text      = activo.datos.nombreEvento;
            fila.textoDescripcion.text = activo.datos.descripcionAlOcurrir;

            if (activo.datos.esResolvible)
            {
                fila.botonResolver.gameObject.SetActive(true);
                ActualizarTextoBoton(fila, activo);

                EventoGlobalActivo capturado = activo;
                fila.botonResolver.onClick.AddListener(() => OnClickToggle(capturado, fila));
            }
            else
            {
                fila.botonResolver.gameObject.SetActive(false);
            }
        }

        if (!hayAlguno)
        {
            FilaEventoUI fila = Instantiate(filaEventoPrefab, contenedorFilasEvento);
            fila.textoNombre.text      = "Sin eventos activos";
            fila.textoDescripcion.text = "";
            fila.botonResolver.gameObject.SetActive(false);
        }
    }

    private void OnClickToggle(EventoGlobalActivo activo, FilaEventoUI fila)
    {
        GameManager.Instancia.ToggleResolucionEvento(activo);
        ActualizarTextoBoton(fila, activo); // refresca solo este botón, sin reconstruir toda la lista
    }

    private void ActualizarTextoBoton(FilaEventoUI fila, EventoGlobalActivo activo)
    {
        fila.botonResolver.GetComponentInChildren<TMP_Text>().text = activo.resolucionPendiente
            ? $"[Pendiente] Cancelar"
            : $"Resolver (${activo.datos.costoResolucion:N0})";
    }
}
