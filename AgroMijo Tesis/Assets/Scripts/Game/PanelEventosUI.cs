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
        TutorialManager.Instancia?.NotificarAccion(TutorialCondicion.EventoAbierto);
    }

    public void Cerrar()
    {
        panelPrincipal.SetActive(false);
        TutorialManager.Instancia?.NotificarAccion(TutorialCondicion.EventoCerrado);
    }

    private void RefrescarLista()
    {
        foreach (Transform hijo in contenedorFilasEvento)
            Destroy(hijo.gameObject);

        List<EventoGlobalActivo> activos = GameManager.Instancia.eventosActivosPersistentes;
        bool hayAlguno = false;

        foreach (EventoGlobalActivo activo in activos)
        {
            if (!EventoDebeMostrarse(activo))
                continue;

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
        ActualizarTextoBoton(fila, activo);
        if (activo.resolucionPendiente)
            TutorialManager.Instancia?.NotificarAccion(TutorialCondicion.EventoResuelto);
    }

    private void ActualizarTextoBoton(FilaEventoUI fila, EventoGlobalActivo activo)
    {
        fila.botonResolver.GetComponentInChildren<TMP_Text>().text = activo.resolucionPendiente
            ? $"[Pendiente] Cancelar"
            : $"Resolver (${activo.datos.costoResolucion:N0})";
    }
    private bool EventoDebeMostrarse(EventoGlobalActivo activo)
    {
        // Si todavía está activo, siempre se muestra.
        if (!activo.resuelto)
            return true;

        // Si se resolvió automáticamente, no se muestra.
        if (!activo.resueltoPorJugador)
            return false;

        // Si no tiene efecto posterior, no hay nada que mostrar.
        if (activo.datos.momentoEfecto != MomentoEfectoEvento.AlResolver)
            return false;

        // 0 = duración infinita.
        if (activo.datos.ciclosEfectoDespuesDeResolver == 0)
            return true;

        // Mientras todavía tenga ciclos de efecto.
        return activo.ciclosEfectoPosterior < activo.datos.ciclosEfectoDespuesDeResolver;
    }
}
