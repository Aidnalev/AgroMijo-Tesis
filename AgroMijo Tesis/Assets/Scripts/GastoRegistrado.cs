// Categorías de gasto para el desglose del reporte.
public enum CategoriaGasto
{
    Semilla,
    Estudio,
    Mejora,
    ResolucionEvento
}

// Un gasto individual ocurrido en el ciclo.
// Se acumula en GameManager y se vuelca al ReporteCiclo al avanzar.
[System.Serializable]
public class GastoRegistrado
{
    public string        descripcion;
    public float         monto;
    public CategoriaGasto categoria;
}
