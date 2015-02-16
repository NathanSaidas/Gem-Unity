using System;
namespace Gem
{
    [Flags]
    public enum ActorIdentifier
    {
        Organic         = 1 << 0,
        Mechanical      = 1 << 1,
        Ethereal        = 1 << 2,
        Giant           = 1 << 3,
        Critter         = 1 << 4,
        Self            = 1 << 5,
        World           = 1 << 6,
        Magic           = 1 << 7,

            

    }
}