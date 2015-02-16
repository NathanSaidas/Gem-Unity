using UnityEngine;
using System.Collections;

namespace Gem
{
    public enum AbilityType
    {
        Self,
        SelfAura,
        Target,
        TargetSplash,
        SelfToTarget,
        SelfToTargetDestroyUponCollision,
        WorldPoint,
        WorldPointSplash,
        WorldPointDirection,
        WorldPointDirectionalSplash,
        SelfToWorldPoint,
        SelfToWorldPointDestroyUponCollision
    }

}

