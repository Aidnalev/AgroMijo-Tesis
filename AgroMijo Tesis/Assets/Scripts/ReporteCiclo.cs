using System.Collections.Generic;

[System.Serializable]
public class ReporteCiclo
{
    public int   numeroCiclo;
    public float presupuestoInicial;
    public float presupuestoFinal;
    public float gananciaTotal;
    public float gastoTotal;

    public List<GastoRegistrado> detalleGastos    = new List<GastoRegistrado>();
    public List<string>          eventosOcurridos  = new List<string>();
    public List<string>          cosechasRealizadas = new List<string>();
}
