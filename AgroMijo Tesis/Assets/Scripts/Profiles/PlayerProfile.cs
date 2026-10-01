using System;

[Serializable]
public class PlayerProfile
{
    public string id;
    public string alias;
    public bool sincronizado;

    public PlayerProfile(string id, string alias)
    {
        this.id = id;
        this.alias = alias;
        this.sincronizado = false;
    }
}