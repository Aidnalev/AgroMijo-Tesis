using System;
using System.Collections.Generic;

[Serializable]
public class ProfileData
{
    public List<PlayerProfile> profiles = new List<PlayerProfile>();
    public string currentProfileId;
}