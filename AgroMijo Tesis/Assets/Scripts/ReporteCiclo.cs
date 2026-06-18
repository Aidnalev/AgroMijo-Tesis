using System.Collections.Generic;

// Un reporte por cada ciclo jugado. El historial de estos reportes
// es lo que te permite comparar partidas (RF-14) y mostrar al jugador
// la relación entre su decisión y el resultado (RF-12, RF-13).
[System.Serializable]
public class ReporteCiclo
{
    public int numeroCiclo;
    public float presupuestoInicial;
    public float presupuestoFinal;
    public float gananciaTotal;
    public float gastoTotal;

    public List<string> eventosOcurridos = new List<string>();
    public List<string> cosechasRealizadas = new List<string>();
}
