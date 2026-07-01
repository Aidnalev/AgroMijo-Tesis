using System;

// No es un ScriptableObject ni un MonoBehaviour: es solo un dato serializable
// que el GameManager mantiene en su lista de eventos persistentes activos.
[Serializable]
public class EventoGlobalActivo
{
    public EventoGlobalData datos;
    public int cicloEnQueOcurrio;
    public bool resuelto = false;
    public int ciclosActivo = 0; // se incrementa cada ciclo que el evento sigue sin resolverse
}
