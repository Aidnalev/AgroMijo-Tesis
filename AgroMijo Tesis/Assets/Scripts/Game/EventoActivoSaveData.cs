// Versión "plana" de EventoGlobalActivo para serializar a JSON.
// El evento se guarda por nombre y se busca en eventosPosibles al cargar.
[System.Serializable]
public class EventoActivoSaveData
{
    public string eventoNombre;       // nombre del EventoGlobalData
    public int    cicloEnQueOcurrio;
    public bool   resuelto;
    public int    ciclosActivo;
    public bool   resolucionPendiente;
}
