using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType { Infantry, Heavy, Flying, None}
public enum GameResources { BioMass, Metals}

public abstract class Unit
{
    public UnitType type { get; private set; }

    public float vInfantryScore { get; private set; }
    public float vHeavyScore { get; private set; }
    public float vFlyingScore { get; private set; }

    public Dictionary<GameResources, int> resourceCosts = new Dictionary<GameResources, int>();

    public string name { get; private set; }

    public float scoreValue { get; private set; }

    public Unit(UnitType type, string name, float vi, float vh, float vf, float scoreValue)
    {
        this.type = type;
        this.name = name;

        vInfantryScore = vi;
        vHeavyScore = vh;
        vFlyingScore = vf;

        if (Mathf.RoundToInt(vi*10+vh*10+vf*10) != 3*10) //Float imprecision showing up here
        {
            Debug.LogError(name + " stats not adding up to 3 : " + vi + " + " + vh + " + " + vf);
        }

        this.scoreValue = scoreValue;
    }

    public float getScoreAgainst(UnitType type)
    {
        switch(type)
        {
            case UnitType.Infantry:
                return vInfantryScore;

            case UnitType.Heavy:
                return vHeavyScore;

            case UnitType.Flying:
                return vFlyingScore;
        }

        return -1;
    }

    public static Unit getUnit(UnitList unit)
    {
        switch (unit)
        {
            case UnitList.Marine:
                return new Marine();

            case UnitList.Tank:
                return new Tank();

            case UnitList.Samurai:
                return new Samurai();

            case UnitList.Bomber:
                return new Bomber();

            case UnitList.Fighter:
                return new Fighter();

            default:
                return null;
        }
    }

}


public enum UnitList { Marine, Samurai, Tank, Bomber, Fighter }


public class Marine : Unit
{
    public Marine() : base(UnitType.Infantry, "Marine", 1.0f, 0.5f, 1.5f, 10)
    {
        resourceCosts.Add(GameResources.BioMass, 5 * (int)scoreValue);
        resourceCosts.Add(GameResources.Metals, 1 * (int)scoreValue);
    }
}


public class Samurai : Unit
{
    public Samurai() : base(UnitType.Infantry, "Samurai", 2.5f, 0.5f, 0.0f, 10)
    {
        resourceCosts.Add(GameResources.BioMass, 5 * (int)scoreValue);
        resourceCosts.Add(GameResources.Metals, 5 * (int)scoreValue);
    }
}


public class Tank : Unit
{
    public Tank() : base(UnitType.Heavy, "Tank", 1.0f, 2.0f, 0.0f, 300)
    {
        resourceCosts.Add(GameResources.BioMass, 1 * (int)scoreValue);
        resourceCosts.Add(GameResources.Metals, 6 * (int)scoreValue);
    }
}


public class Bomber : Unit
{
    public Bomber() : base(UnitType.Flying, "Bomber", 0.5f, 2.0f, 0.5f, 200)
    {
        resourceCosts.Add(GameResources.BioMass, 1 * (int)scoreValue);
        resourceCosts.Add(GameResources.Metals, 6 * (int)scoreValue);
    }
}


public class Fighter : Unit
{
    public Fighter() : base(UnitType.Flying, "Fighter", 0.2f, 0.3f, 2.5f, 50)
    {
        resourceCosts.Add(GameResources.BioMass, 1 * (int)scoreValue);
        resourceCosts.Add(GameResources.Metals, 5 * (int)scoreValue);
    }
}
