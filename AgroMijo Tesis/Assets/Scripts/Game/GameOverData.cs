using System;
using System.Collections.Generic;

// Registro permanente de una partida completada.
// Se guarda en disco vinculado al perfil y nunca se borra
// (a diferencia del save activo que se borra al terminar).
[Serializable]
public class GameOverData
{
    public string profileId;
    public string profileAlias;
    public string fechaPartida;       // "2026-04-15 14:32"
    public int    ciclosJugados;
    public string razonFin;           // "Ciclos completados" / "Quiebra" / "Manual"

    public float presupuestoInicial;  // al empezar la partida (ciclo 0)
    public float presupuestoFinal;
    public float gananciaAcumulada;
    public float gastoAcumulado;

    public List<ReporteCiclo> historialCiclos = new List<ReporteCiclo>();
}

// Contenedor de todos los registros de un perfil (para serializar con JsonUtility)
[Serializable]
public class PerfilRegistros
{
    public List<GameOverData> registros = new List<GameOverData>();
}
