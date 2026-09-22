// Versión "plana" de Parcela para serializar a JSON.
// No tiene referencias a ScriptableObjects — solo primitivos.
// El cultivo se guarda por nombre y se busca al cargar.
[System.Serializable]
public class ParcelaSaveData
{
    public string nombreParcela;
    public int    tipoSuelo;             // cast de TipoSuelo enum
    public float  aguaBase;
    public float  disponibilidadAgua;
    public float  accesoVial;
    public bool   estudiada;
    public int    nivelMejora;
    public int    estado;                // cast de EstadoParcela enum
    public string cultivoActualNombre;   // nombre del CultivoData, null si vacía
    public int    cicloEnQueSePlanto;
}
