using System;

/// <summary>
/// Deterministic random wrapper used for gameplay logic.
/// Do not use UnityEngine.Random for simulation-critical behavior.
/// </summary>
public sealed class BattleRandom
{
    private readonly Random _rng;

    public BattleRandom(int seed)
    {
        _rng = new Random(seed);
    }

    public float Range(float min, float max)
    {
        if (max < min)
        {
            (min, max) = (max, min);
        }

        return (float)(_rng.NextDouble() * (max - min) + min);
    }

    public int Range(int min, int maxExclusive)
    {
        if (maxExclusive <= min)
        {
            return min;
        }

        return _rng.Next(min, maxExclusive);
    }

    public bool Chance(float probability)
    {
        if (probability <= 0f) return false;
        if (probability >= 1f) return true;
        return _rng.NextDouble() < probability;
    }

    public float Value => (float)_rng.NextDouble();
}
