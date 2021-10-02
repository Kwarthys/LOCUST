using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType { Infantry, Heavy, Flying, None}

public abstract class Unit
{
    public UnitType type { get; private set; }

    public float vInfantryScore { get; private set; }
    public float vHeavyScore { get; private set; }
    public float vFlyingScore { get; private set; }

    public string name { get; private set; }

    public float scoreValue { get; private set; }

    public Unit(UnitType type, string name, float vi, float vh, float vf, float scoreValue)
    {
        this.type = type;
        this.name = name;

        vInfantryScore = vi;
        vHeavyScore = vh;
        vFlyingScore = vf;

        if(vi+vh+vf != 3.0)
        {
            Debug.LogError(name + " stats not adding up to 3");
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

            default:
                return null;
        }
    }

}


public enum UnitList { Marine, Tank, Samurai }


public class Marine : Unit
{
    public Marine() : base(UnitType.Infantry, "Marine", 1.0f, 0.5f, 1.5f, 10)
    { }
}


public class Tank : Unit
{
    public Tank() : base(UnitType.Heavy, "Tank", 1.0f, 2.0f, 0.0f, 300)
    { }
}


public class Samurai : Unit
{
    public Samurai() : base(UnitType.Infantry, "Samurai", 2.5f, 0.5f, 0.0f, 10)
    { }
}
