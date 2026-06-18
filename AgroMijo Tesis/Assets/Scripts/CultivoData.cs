using UnityEngine;

// Tipos de suelo representativos. Ajusta estos valores cuando tengas
// los datos reales de la Provincia Comunera (pendiente RF-20 en la matriz).
public enum TipoSuelo
{
    Arcilloso,
    Arenoso,
    Franco,
    Limoso
}

// Representa un cultivo seleccionable. Crea uno por cada uno de tus 4 cultivos
// desde el menú: clic derecho en el Proyecto > Create > AgroMijo > Cultivo
[CreateAssetMenu(fileName = "NuevoCultivo", menuName = "AgroMijo/Cultivo")]
public class CultivoData : ScriptableObject
{
    [Header("Identificación")]
    public string nombreCultivo = "Nombre del cultivo";

    [Header("Ciclo de crecimiento")]
    [Tooltip("Cuántos ciclos (meses) tarda en estar listo para cosechar")]
    public int duracionCiclos = 3;

    [Header("Economía")]
    public float costoSemilla = 50000f;

    [Tooltip("Ganancia base por cosecha, antes de aplicar modificadores de suelo y agua")]
    public float rendimientoBase = 200000f;

    [Header("Modificadores por tipo de suelo")]
    [Tooltip("1 = neutro, mayor a 1 = favorable, menor a 1 = desfavorable")]
    public float modificadorArcilloso = 1f;
    public float modificadorArenoso = 1f;
    public float modificadorFranco = 1f;
    public float modificadorLimoso = 1f;

    public float ObtenerModificadorPorSuelo(TipoSuelo tipo)
    {
        switch (tipo)
        {
            case TipoSuelo.Arcilloso: return modificadorArcilloso;
            case TipoSuelo.Arenoso: return modificadorArenoso;
            case TipoSuelo.Franco: return modificadorFranco;
            case TipoSuelo.Limoso: return modificadorLimoso;
            default: return 1f;
        }
    }
}
