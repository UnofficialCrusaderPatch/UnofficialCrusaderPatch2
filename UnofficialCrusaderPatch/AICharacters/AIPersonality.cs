using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace UCP.AICharacters
{
    public class AIPersonality
    {
        public const int TotalFields = 169;

        static readonly FieldInfo[] fieldInfos = typeof(AIPersonality).GetFields();
        public void SetByIndex(int index, int value)
        {
            FieldInfo fi = fieldInfos[index];
            fi.SetValue(this, value);
        }

        public int GetByIndex(int index)
        {
            FieldInfo fi = fieldInfos[index];
            return (int)fi.GetValue(this);
        }

        // Index: 0 Hex: 0x00
        [RWNames("Unknown000")]
        public int Unknown000;

        // Index: 1 Hex: 0x04
        [RWNames("Unknown001")]
        public int Unknown001;

        // Index: 2 Hex: 0x08
        [RWNames("Unknown002")]
        public int Unknown002;

        // Index: 3 Hex: 0x0C
        [RWNames("Unknown003")]
        public int Unknown003;

        // Index: 4 Hex: 0x10
        [RWNames("Unknown004")]
        public int Unknown004;

        // Index: 5 Hex: 0x14
        [RWNames("Unknown005")]
        public int Unknown005;

        // Index: 6 Hex: 0x18
        [RWNames("Unknown006")]
        [RWComment("0 - 10000")]
        public int CriticalPopularity;

        // Index: 7 Hex: 0x1C
        [RWNames("Unknown007")]
        public int Unknown007;

        // Index: 8 Hex: 0x20
        [RWNames("Unknown008")]
        public int Unknown008;

        // Index: 9 Hex: 0x24
        [RWNames("Unknown009")]
        public int Unknown009;

        // Index: 10 Hex: 0x28
        [RWNames("Unknown010")]
        public int Unknown010;

        // Index: 11 Hex: 0x2C
        [RWNames("Unknown011")]
        public int Unknown011;

        // Index: 12 Hex: 0x30
        [RWNames("Unknown012")]
        public FarmBuildingType Farm1;

        // Index: 13 Hex: 0x34
        [RWNames("Unknown013")]
        public FarmBuildingType Farm2;

        // Index: 14 Hex: 0x38
        [RWNames("Unknown014")]
        public FarmBuildingType Farm3;

        // Index: 15 Hex: 0x3C
        [RWNames("Unknown015")]
        public FarmBuildingType Farm4;

        // Index: 16 Hex: 0x40
        [RWNames("Unknown016")]
        public FarmBuildingType Farm5;

        // Index: 17 Hex: 0x44
        [RWNames("Unknown017")]
        public FarmBuildingType Farm6;

        // Index: 18 Hex: 0x48
        [RWNames("Unknown018")]
        public FarmBuildingType Farm7;

        // Index: 19 Hex: 0x4C
        [RWNames("Unknown019")]
        public FarmBuildingType Farm8;

        // Index: 20 Hex: 0x50
        [RWNames("Unknown020")]
        public int Unknown020;

        // Index: 21 Hex: 0x54
        [RWNames("Unknown021")]
        public int Unknown021;

        // Index: 22 Hex: 0x58
        [RWNames("Unknown022")]
        public int Unknown022;

        // Index: 23 Hex: 0x5C
        [RWNames("Unknown023")]
        [RWComment("AIs with mines have a value of >= 10 there, don't know yet. But each value from 1 to 10 let's the rat build one iron mine.")]
        public int MaxIronMines;

        // Index: 24 Hex: 0x60
        [RWNames("Unknown024")]
        [RWComment("Some AIs with pitch have a value higher than 10 here, e.g. Pig with 20. Caliph has 7 and builds a lot. Wolf has a 15 and builds less.")]
        public int MaxPitchRigs;

        // Index: 25 Hex: 0x64
        [RWNames("Unknown025")]
        [RWComment("Philipp has a 1 here, but does not build any Quarries.")]
        public int QuarriesMax;

        // Index: 26 Hex: 0x68
        [RWNames("Unknown026")]
        public int Unknown026;

        // Index: 27 Hex: 0x6C
        [RWNames("Unknown027")]
        public int Unknown027;

        // Index: 28 Hex: 0x70
        [RWNames("Unknown028")]
        public int Unknown028;

        // Index: 29 Hex: 0x74
        [RWNames("Unknown029")]
        public int Unknown029;

        // Index: 30 Hex: 0x78
        [RWNames("Unknown030")]
        public int Unknown030;

        // Index: 31 Hex: 0x7C
        [RWNames("Unknown031")]
        public int Unknown031;

        // Index: 32 Hex: 0x80
        [RWNames("Unknown032")]
        [RWComment("includes flour?")]
        public int MaxFood;

        // Index: 33 Hex: 0x84
        [RWNames("Unknown033")]
        public int MinimumApples;

        // Index: 34 Hex: 0x88
        [RWNames("Unknown034")]
        public int MinimumCheese;

        // Index: 35 Hex: 0x8C
        [RWNames("Unknown035")]
        public int MinimumBread;

        // Index: 36 Hex: 0x90
        [RWNames("Unknown036")]
        public int MinimumWheat;

        // Index: 37 Hex: 0x94
        [RWNames("Unknown037")]
        public int MinimumHop;

        // Index: 38 Hex: 0x98
        [RWNames("Unknown038")]
        public int TradeAmountFood;

        // Index: 39 Hex: 0x9C
        [RWNames("Unknown039")]
        public int TradeAmountEquipment;

        // Index: 40 Hex: 0xA0
        [RWNames("Unknown040")]
        public int Unknown040;

        // Index: 41 Hex: 0xA4
        [RWNames("Unknown041")]
        public int Unknown041;

        // Index: 42 Hex: 0xA8
        [RWNames("Unknown042")]
        public int Unknown042;

        // Index: 43 Hex: 0xAC
        [RWNames("Unknown043")]
        public int MaxWood;

        // Index: 44 Hex: 0xB0
        [RWNames("Unknown044")]
        public int MaxStone;

        // Index: 45 Hex: 0xB4
        [RWNames("Unknown045")]
        public int MaxResourceOther;

        // Index: 46 Hex: 0xB8
        [RWNames("Unknown046")]
        public int MaxEquipment;

        // Index: 47 Hex: 0xBC
        [RWNames("Unknown047")]
        public int MaxBeer;

        // Index: 48 Hex: 0xC0
        [RWNames("Unknown048")]
        [RWComment("added to all max resource values?")]
        public int MaxResourceVariance;

        // Index: 49 Hex: 0xC4
        [RWNames("Unknown049")]
        public int Unknown049;

        // Index: 50 Hex: 0xC8
        [RWNames("Unknown050")]
        public int Unknown050;

        // Index: 51 Hex: 0xCC
        [RWNames("Unknown051")]
        public int Unknown051;

        // Index: 52 Hex: 0xD0
        [RWNames("Unknown052")]
        public int Unknown052;

        // Index: 53 Hex: 0xD4
        [RWNames("Unknown053")]
        public Resource SellResource01;

        // Index: 54 Hex: 0xD8
        [RWNames("Unknown054")]
        public Resource SellResource02;

        // Index: 55 Hex: 0xDC
        [RWNames("Unknown055")]
        public Resource SellResource03;

        // Index: 56 Hex: 0xE0
        [RWNames("Unknown056")]
        public Resource SellResource04;

        // Index: 57 Hex: 0xE4
        [RWNames("Unknown057")]
        public Resource SellResource05;

        // Index: 58 Hex: 0xE8
        [RWNames("Unknown058")]
        public Resource SellResource06;

        // Index: 59 Hex: 0xEC
        [RWNames("Unknown059")]
        public Resource SellResource07;

        // Index: 60 Hex: 0xF0
        [RWNames("Unknown060")]
        public Resource SellResource08;

        // Index: 61 Hex: 0xF4
        [RWNames("Unknown061")]
        public Resource SellResource09;

        // Index: 62 Hex: 0xF8
        [RWNames("Unknown062")]
        public Resource SellResource10;

        // Index: 63 Hex: 0xFC
        [RWNames("Unknown063")]
        public Resource SellResource11;

        // Index: 64 Hex: 0x100
        [RWNames("Unknown064")]
        public Resource SellResource12;

        // Index: 65 Hex: 0x104
        [RWNames("Unknown065")]
        public Resource SellResource13;

        // Index: 66 Hex: 0x108
        [RWNames("Unknown066")]
        public Resource SellResource14;

        // Index: 67 Hex: 0x10C
        [RWNames("Unknown067")]
        public Resource SellResource15;

        // Index: 68 Hex: 0x110
        [RWNames("Unknown068")]
        public int Unknown068;

        // Index: 69 Hex: 0x114
        [RWNames("Unknown069")]
        public int Unknown069;

        // Index: 70 Hex: 0x118
        [RWNames("Unknown070")]
        public int Unknown070;

        // Index: 71 Hex: 0x11C
        [RWNames("Unknown071")]
        public int Unknown071;

        // Index: 72 Hex: 0x120
        [RWNames("Unknown072")]
        public int Unknown072;

        // Index: 73 Hex: 0x124
        [RWNames("Unknown073")]
        public int Unknown073;

        // Index: 74 Hex: 0x128
        [RWNames("Unknown074")]
        public int Unknown074;

        // Index: 75 Hex: 0x12C
        [RWNames("Unknown075")]
        public int Unknown075;

        // Index: 76 Hex: 0x130
        [RWComment("The probability with which this AI reinforces missing defense troops.")]
        [RWNames("Unknown076")]
        public int DefRecruitAffinity;

        // Index: 77 Hex: 0x134
        [RWNames("Unknown077")]
        public int Unknown077;

        // Index: 78 Hex: 0x138
        [RWNames("Unknown078")]
        public int Unknown078;

        // Index: 79 Hex: 0x13C
        [RWNames("Unknown079")]
        public int Unknown079;

        // Index: 80 Hex: 0x140
        [RWNames("Unknown080")]
        public int Unknown080;

        // Index: 81 Hex: 0x144
        [RWNames("Unknown081")]
        public int Unknown081;

        // Index: 82 Hex: 0x148
        [RWNames("Unknown082")]
        public int Unknown082;

        // Index: 83 Hex: 0x14C
        [RWNames("Unknown083")]
        [RWComment("Amount of ranged fallout units.")]
        public int FalloutUnitRangedCount;

        // Index: 84 Hex: 0x150
        [RWNames("Unknown084")]
        [RWComment("Type of ranged units that go to the last attacked farm or building, and guard it until another is attacked.")]
        public Unit FalloutUnitRanged;

        // Index: 85 Hex: 0x154
        [RWNames("Unknown085")]
        [RWComment("Amount of melee fallout units.")]
        public int FalloutUnitMeleeCount;

        // Index: 86 Hex: 0x158
        [RWNames("Unknown086")]
        [RWComment("Type of melee units to attack enemy units shooting at the AI's buildings or workers.")]
        public Unit FalloutUnitMelee;

        // Index: 87 Hex: 0x15C
        [RWNames("Unknown087")]
        [RWComment("Amount of units that dig own moat.")]
        public int DefensiveDiggingUnitCount;

        // Index: 88 Hex: 0x160
        [RWNames("Unknown088")]
        [RWComment("Type of unit to dig own moat.")]
        public Unit DefensiveDiggingUnit;

        // Index: 89 Hex: 0x164
        [RWNames("Unknown089")]
        public int RecruitInterval;

        // Index: 90 Hex: 0x168
        [RWNames("Unknown090")]
        public int Unknown090;

        // Index: 91 Hex: 0x16C
        [RWNames("Unknown091")]
        public int Unknown091;

        // Index: 92 Hex: 0x170
        [RWNames("Unknown092")]
        public int DefTotal;

        // Index: 93 Hex: 0x174
        [RWNames("Unknown093")]
        public int Unknown093;

        // Index: 94 Hex: 0x178
        [RWNames("Unknown094")]
        public int Unknown094;

        // Index: 95 Hex: 0x17C
        [RWNames("Unknown095")]
        public int Unknown095;

        // Index: 96 Hex: 0x180
        [RWNames("Unknown096")]
        public int DefWalls;

        // Index: 97 Hex: 0x184
        [RWNames("Unknown097")]
        public Unit DefUnit1;

        // Index: 98 Hex: 0x188
        [RWNames("Unknown098")]
        public Unit DefUnit2;

        // Index: 99 Hex: 0x18C
        [RWNames("Unknown099")]
        public Unit DefUnit3;

        // Index: 100 Hex: 0x190
        [RWNames("Unknown100")]
        public Unit DefUnit4;

        // Index: 101 Hex: 0x194
        [RWNames("Unknown101")]
        public Unit DefUnit5;

        // Index: 102 Hex: 0x198
        [RWNames("Unknown102")]
        public Unit DefUnit6;

        // Index: 103 Hex: 0x19C
        [RWNames("Unknown103")]
        public Unit DefUnit7;

        // Index: 104 Hex: 0x1A0
        [RWNames("Unknown104")]
        public Unit DefUnit8;

        // Index: 105 Hex: 0x1A4
        [RWNames("Unknown105")]
        public int RaidMaxUnits;

        // Index: 106 Hex: 0x1A8
        [RWNames("Unknown106")]
        public int Unknown106;

        // Index: 107 Hex: 0x1AC
        [RWNames("Unknown107")]
        public Unit RaidUnit1;

        // Index: 108 Hex: 0x1B0
        [RWNames("Unknown108")]
        public Unit RaidUnit2;

        // Index: 109 Hex: 0x1B4
        [RWNames("Unknown109")]
        public Unit RaidUnit3;

        // Index: 110 Hex: 0x1B8
        [RWNames("Unknown110")]
        public Unit RaidUnit4;

        // Index: 111 Hex: 0x1BC
        [RWNames("Unknown111")]
        public Unit RaidUnit5;

        // Index: 112 Hex: 0x1C0
        [RWNames("Unknown112")]
        public Unit RaidUnit6;

        // Index: 113 Hex: 0x1C4
        [RWNames("Unknown113")]
        public Unit RaidUnit7;

        // Index: 114 Hex: 0x1C8
        [RWNames("Unknown114")]
        public Unit RaidUnit8;

        // Index: 115 Hex: 0x1CC
        [RWNames("Unknown115")]
        public int Unknown115;

        // Index: 116 Hex: 0x1D0
        [RWNames("Unknown116")]
        public int Unknown116;

        // Index: 117 Hex: 0x1D4
        [RWNames("Unknown117")]
        public int Unknown117;

        // Index: 118 Hex: 0x1D8
        [RWNames("Unknown118")]
        public int Unknown118;

        // Index: 119 Hex: 0x1DC
        [RWNames("Unknown119")]
        public int Unknown119;

        // Index: 120 Hex: 0x1E0
        [RWNames("Unknown120")]
        public int Unknown120;

        // Index: 121 Hex: 0x1E4
        [RWNames("Unknown121")]
        public int Unknown121;

        // Index: 122 Hex: 0x1E8
        [RWNames("Unknown122")]
        public int Unknown122;

        // Index: 123 Hex: 0x1EC
        [RWNames("Unknown123")]
        public int Unknown123;

        // Index: 124 Hex: 0x1F0
        [RWNames("Unknown124")]
        public int Unknown124;

        // Index: 125 Hex: 0x1F4
        [RWComment("The base amount of troops with which this AI attacks")]
        [RWNames("Unknown125")]
        public int AttForceBase;

        // Index: 126 Hex: 0x1F8
        [RWComment("The maximum random amount of additional troops in an attack")]
        [RWNames("Unknown126")]
        public int AttForceRandom;

        // Index: 127 Hex: 0x1FC
        [RWNames("Unknown127")]
        public int Unknown127;

        // Index: 128 Hex: 0x200
        [RWNames("Unknown128")]
        public int Unknown128;

        // Index: 129 Hex: 0x204
        [RWNames("Unknown129")]
        public int Unknown129;

        // Index: 130 Hex: 0x208
        [RWNames("Unknown130")]
        public int Unknown130;

        // Index: 131 Hex: 0x20C
        [RWNames("Unknown131")]
        public int Unknown131;

        // Index: 132 Hex: 0x210
        [RWNames("Unknown132")]
        public int Unknown132;

        // Index: 133 Hex: 0x214
        [RWNames("Unknown133")]
        public SiegeEngine SiegeEngine1;

        // Index: 134 Hex: 0x218
        [RWNames("Unknown134")]
        public SiegeEngine SiegeEngine2;

        // Index: 135 Hex: 0x21C
        [RWNames("Unknown135")]
        public SiegeEngine SiegeEngine3;

        // Index: 136 Hex: 0x220
        [RWNames("Unknown136")]
        public SiegeEngine SiegeEngine4;

        // Index: 137 Hex: 0x224
        [RWNames("Unknown137")]
        public SiegeEngine SiegeEngine5;

        // Index: 138 Hex: 0x228
        [RWNames("Unknown138")]
        public SiegeEngine SiegeEngine6;

        // Index: 139 Hex: 0x22C
        [RWNames("Unknown139")]
        public SiegeEngine SiegeEngine7;

        // Index: 140 Hex: 0x230
        [RWNames("Unknown140")]
        public SiegeEngine siegeEngine8;

        // Index: 141 Hex: 0x234
        [RWNames("Unknown141")]
        public int Unknown141;

        // Index: 142 Hex: 0x238
        [RWNames("Unknown142")]
        public int Unknown142;

        // Index: 143 Hex: 0x23C
        [RWNames("Unknown143")]
        public int AttMaxEngineers;

        // Index: 144 Hex: 0x240
        [RWNames("Unknown144")]
        [RWComment("This unit is only recruited if the target enemy has moat and used preferably to dig enemy moat.")]
        public Unit AttUnit1;

        // Index: 145 Hex: 0x244
        [RWNames("Unknown145")]
        public int AttMaxUnit1;

        // Index: 146 Hex: 0x248
        [RWNames("Unknown146")]
        public Unit AttUnit2;

        // Index: 147 Hex: 0x24C
        [RWNames("Unknown147")]
        public int AttMaxUnit2;

        // Index: 148 Hex: 0x250
        [RWNames("Unknown148")]
        public int AttMaxAssassins;

        // Index: 149 Hex: 0x254
        [RWNames("Unknown149")]
        public int AttMaxLaddermen;

        // Index: 150 Hex: 0x258
        [RWNames("Unknown150")]
        public int AttMaxTunnelers;

        // Index: 151 Hex: 0x25C
        [RWNames("Unknown151")]
        public Unit AttUnit3;

        // Index: 152 Hex: 0x260
        [RWNames("Unknown152")]
        public int AttMaxUnit3;

        // Index: 153 Hex: 0x264
        [RWNames("Unknown153")]
        public int Unknown153;

        // Index: 154 Hex: 0x268
        [RWNames("Unknown154")]
        [RWComment("Attacking unit that never moves, except if wall is breached.")]
        public Unit AttUnit4; // ranged?

        // Index: 155 Hex: 0x26C
        [RWNames("Unknown155")]
        public int AttMaxUnit4;

        // Index: 156 Hex: 0x270
        [RWNames("Unknown156")]
        public int Unknown156;

        // Index: 157 Hex: 0x274
        [RWNames("Unknown157")]
        public Unit AttUnit5;

        // Index: 158 Hex: 0x278
        [RWNames("Unknown158")]
        public int AttMaxUnit5;

        // Index: 159 Hex: 0x27C
        [RWNames("Unknown159")]
        public Unit AttUnit6;

        // Index: 160 Hex: 0x280
        [RWNames("Unknown160")]
        public int AttMaxUnit6;

        // Index: 161 Hex: 0x284
        [RWNames("Unknown161")]
        public int Unknown161;

        // Index: 162 Hex: 0x288
        [RWNames("Unknown162")]
        public Unit AttUnitDefault;

        // Index: 163 Hex: 0x28C
        [RWNames("Unknown163")]
        public Unit UnknownUnit163;

        // Index: 164 Hex: 0x290
        [RWNames("Unknown164")]
        public int Unknown164;

        // Index: 165 Hex: 0x294
        [RWNames("Unknown165")]
        public int Unknown165;

        // Index: 166 Hex: 0x298
        [RWComment("This does nothing")]
        [RWNames("Unknown166")]
        public int AttMaxDefault;

        // Index: 167 Hex: 0x29C
        [RWNames("Unknown167")]
        public int Unknown167;

        // Index: 168 Hex: 0x2A0
        [RWNames("Unknown168")]
        public TargetingType TargetChoice;
    }
}
