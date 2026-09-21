using System;

[Serializable]
public class PlayerProfile
{
    public string id;
    public string alias;

    public PlayerProfile(string id, string alias)
    {
        this.id = id;
        this.alias = alias;
    }
}