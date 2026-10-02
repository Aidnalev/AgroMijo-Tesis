using System;
using UnityEngine;

[Serializable]
public class EscenarioConfig
{
    public float dineroInicial = 10000000f;
    public float costoJornal = 55000f;

    public string[] cultivos = Array.Empty<string>();
    public string[] parcelas = Array.Empty<string>();
}