using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType { Infantry, Heavy, Flying, None}
public enum GameResources { BioMass, Metals}

public struct GameCost
{
    public int bioMassCost;
    public int metalCost;

    public int getCost(GameResources r)
    {
        if (r == GameResources.BioMass) return bioMassCost;
        else return metalCost;
    }

    public GameCost(int bioMassCost, int metalCost)
    {
        this.bioMassCost = bioMassCost;
        this.metalCost = metalCost;
    }

    public GameCost(Dictionary<GameResources, int> resourceCosts)
    {
        if (resourceCosts.ContainsKey(GameResources.BioMass))
        {
            this.bioMassCost = resourceCosts[GameResources.BioMass];
        }
        else this.bioMassCost = 0;

        if (resourceCosts.ContainsKey(GameResources.Metals))
        {
            this.metalCost = resourceCosts[GameResources.Metals];
        }
        else this.metalCost = 0;
    }

    public override string ToString()
    {
        return GameResources.BioMass + " " + bioMassCost + ", " + GameResources.Metals + " " + metalCost;
    }

    public static GameCost operator +(GameCost a, GameCost b)
        => new GameCost(a.bioMassCost + b.bioMassCost, a.metalCost + b.metalCost);
}

public abstract class Unit
{
    public UnitType type { get; private set; }
    public UnitList unitName { get; private set; }

    public float vInfantryScore { get; private set; }
    public float vHeavyScore { get; private set; }
    public float vFlyingScore { get; private set; }

    public Dictionary<GameResources, int> resourceCosts = new Dictionary<GameResources, int>();

    public float scoreValue { get; private set; }

    public Unit(UnitType type, UnitList unitName, float vi, float vh, float vf, float scoreValue)
    {
        this.type = type;
        this.unitName = unitName;

        vInfantryScore = vi;
        vHeavyScore = vh;
        vFlyingScore = vf;

        if (Mathf.RoundToInt(vi*10+vh*10+vf*10) != 3*10) //Float imprecision showing up here
        {
            Debug.LogError(unitName + " stats not adding up to 3 : " + vi + " + " + vh + " + " + vf);
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

    public static GameCost getCost(UnitList unit, int number = 1)
    {
        Unit u = getUnit(unit);

        return new GameCost(u.resourceCosts[GameResources.BioMass] * number, u.resourceCosts[GameResources.Metals] * number);
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

    public static UnitList getRandomUnit()
    {
        System.Array values = System.Enum.GetValues(typeof(UnitList));
        return (UnitList)values.GetValue((int)(Random.value * values.Length));
    }

}


public enum UnitList { Marine, Samurai, Tank, Bomber, Fighter }


public class Marine : Unit
{
    public Marine() : base(UnitType.Infantry, UnitList.Marine, 1.0f, 0.5f, 1.5f, 1)
    {
        resourceCosts.Add(GameResources.BioMass, 6 * (int)scoreValue);
        resourceCosts.Add(GameResources.Metals, 0 * (int)scoreValue);
    }
}


public class Samurai : Unit
{
    public Samurai() : base(UnitType.Infantry, UnitList.Samurai, 2.5f, 0.5f, 0.0f, 1)
    {
        resourceCosts.Add(GameResources.BioMass, 5 * (int)scoreValue);
        resourceCosts.Add(GameResources.Metals, 5 * (int)scoreValue);
    }
}


public class Tank : Unit
{
    public Tank() : base(UnitType.Heavy, UnitList.Tank, 1.0f, 2.0f, 0.0f, 30)
    {
        resourceCosts.Add(GameResources.BioMass, 1 * (int)scoreValue);
        resourceCosts.Add(GameResources.Metals, 6 * (int)scoreValue);
    }
}


public class Bomber : Unit
{
    public Bomber() : base(UnitType.Flying, UnitList.Bomber, 0.5f, 2.0f, 0.5f, 20)
    {
        resourceCosts.Add(GameResources.BioMass, 1 * (int)scoreValue);
        resourceCosts.Add(GameResources.Metals, 6 * (int)scoreValue);
    }
}


public class Fighter : Unit
{
    public Fighter() : base(UnitType.Flying, UnitList.Fighter, 0.2f, 0.3f, 2.5f, 5)
    {
        resourceCosts.Add(GameResources.BioMass, 1 * (int)scoreValue);
        resourceCosts.Add(GameResources.Metals, 5 * (int)scoreValue);
    }
}
