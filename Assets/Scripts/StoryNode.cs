using System;
using System.Collections.Generic;

[Serializable]
public class StoryData
{
    public List<StoryNode> nodes;
}

[Serializable]
public class StoryNode
{
    public string id;
    public string region;
    public string text;
    public string background;
    public string music;
    public List<Choice> choices;
}

public enum StatType
{
    None,
    Strength,
    Charisma,
    Perception,
    Wisdom
}

[Serializable]
public class Choice
{
    public string text;
    public string nextId;         // Başarı olursa gidilecek sahne
    public string failureNextId;  // Başarısızlıkta gidilecek sahne

    public string requiredStat;   // StatType enum'un string hali
    public int requiredStatValue;

    public string gainStat;
    public int gainAmount;
    public int lossAmount;

    public int hpChangeOnSuccess;
    public int hpChangeOnFail;

    public bool useProbability;
}
