using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCP.AICharacters
{
    /// <summary>
    /// Crusader-intern military unit IDs
    /// </summary>
    public enum Unit
    {
        None = 0x00,
        Tunneler = 0x05,
        EuropArcher = 0x16,
        Crossbowman = 0x17,
        Spearman = 0x18,
        Pikeman = 0x19,
        Maceman = 0x1A,
        Swordsman = 0x1B,
        Knight = 0x1C,
        Ladderman = 0x1D,
        Engineer = 0x1E,
        Monk = 0x25,
        ArabArcher = 0x46,
        Slave = 0x47,
        Slinger = 0x48,
        Assassin = 0x49,
        HorseArcher = 0x4A,
        ArabSwordsman = 0x4B,
        FireThrower = 0x4C
    }

    public enum DiggingUnit
    {
        None = 0x00,
        EuropArcher = 0x16,
        Spearman = 0x18,
        Pikeman = 0x19,
        Maceman = 0x1A,
        Engineer = 0x1E,
        Slave = 0x47
    }

    public enum MeleeUnit
    {
        None = 0x00,
        Tunneler = 0x05,
        Spearman = 0x18,
        Pikeman = 0x19,
        Maceman = 0x1A,
        Swordsman = 0x1B,
        Knight = 0x1C,
        //Ladderman = 0x1D,
        //Engineer = 0x1E,
        Monk = 0x25,
        Slave = 0x47,
        Assassin = 0x49,
        ArabSwordsman = 0x4B
    }

    public enum RangedUnit
    {
        None = 0x00,
        EuropArcher = 0x16,
        Crossbowman = 0x17,
        ArabArcher = 0x46,
        Slinger = 0x48,
        HorseArcher = 0x4A,
        FireThrower = 0x4C
    }

    // helped by ngc92 (https://github.com/ngc92)
    public enum NPC
    {
        Peasant     = 1,
        BurningMan  = 2,
        Lumberjack  = 3,
        Fletcher    = 4,
        Tunneler    = 5,
        Hunter      = 6,
        Unknown7	= 7,
        Unknown8    = 8,
        Unknown9	= 9,
        PitchWorker	= 10,
        Unknown11	= 11,
        Unknown12	= 12,
        Unknown13	= 13,
        Unknown14   = 14,
        Child       = 15,
        Baker       = 16,
        Woman       = 17,
        Poleturner  = 18,
        Smith       = 19,
        Armorer     = 20,
        Tanner      = 21,
        EuropArcher = 22,
        CrossbowMan = 23,
        Spearman    = 24,
        Pikeman     = 25,
        Maceman     = 26,
        Swordsman   = 27,
        Knight      = 28,
        Ladderman   = 29,
        Engineer    = 30,
        Unknown31	= 31,
        Unknown32	= 32,
        Priest      = 33,
        Unknown34	= 34,
        Drunkard    = 35,
        Innkeeper   = 36,
        Monk        = 37,
        Unknown38	= 38,
        Catapult    = 39,
        Trebuchet   = 40,
        Mangonel    = 41,
        Unknown42	= 42,
        Unknown43	= 43,
        Antelope    = 44,
        Lion        = 45,
        Rabbit      = 46,
        Camel       = 47,
        Unknown48	= 48,
        Unknown49	= 49,
        SiegeTent   = 50,
        Unknown51	= 51,
        Unknown52	= 52,
        Fireman     = 53,
        Ghost       = 54,
        Lord        = 55,
        Lady        = 56,
        Jester      = 57,
        Siegetower  = 58,
        Ram         = 59,
        Shield      = 60,
        Ballista    = 61,
        Chicken     = 62,
        Unknown63	= 63,
        Unknown64	= 64,
        Juggler     = 65,
        FireEater   = 66,
        Dog         = 67,
        Unknown68	= 68,
        Unknown69	= 69,
        ArabArcher  = 70,
        Slave       = 71,
        Slinger     = 72,
        Assassin    = 73,
        HorseArcher = 74,
        ArabSwordsman = 75,
        FireThrower = 76,
        FireBallista = 77,
    }
}
