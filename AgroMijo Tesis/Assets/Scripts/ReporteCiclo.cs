using System.Collections.Generic;

[System.Serializable]
public class ReporteCiclo
{
    public int   numeroCiclo;
    public float presupuestoInicial;
    public float presupuestoFinal;
    public float gananciaTotal;
    public float gastoTotal;

    // Jornales del ciclo
    public int   jornalesNecesarios;
    public int   jornalesFamiliaresUsados;
    public int   jornalesContratados;
    public float modificadorJornales; // 1 = pleno rendimiento, <1 = penalización

    public List<GastoRegistrado> detalleGastos     = new List<GastoRegistrado>();
    public List<string>          eventosOcurridos  = new List<string>();
    public List<string>          cosechasRealizadas = new List<string>();
}
