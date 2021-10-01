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

        vInfantryScore = vi;
        vHeavyScore = vh;
        vFlyingScore = vf;

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

            default:
                return null;
        }
    }

}


public enum UnitList { Marine, Tank}


public class Marine : Unit
{
    public Marine() : base(UnitType.Infantry, "Marine", 1.0f, 0.5f, 1.5f, 10)
    { }
}


public class Tank : Unit
{
    public Tank() : base(UnitType.Infantry, "Tank", 1.0f, 1.5f, 0.5f, 300)
    { }
}
