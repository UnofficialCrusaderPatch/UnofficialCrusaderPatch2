﻿using System.Reflection;

namespace UCP.AICharacters
{
    public class AIPersonality
    {
        public const int TotalFields = 169;

        static readonly FieldInfo[] fieldInfos = typeof(AIPersonality).GetFields();
        public void SetByIndex(int index, int value)
        {
            FieldInfo fi = fieldInfos[index];
            if (fi.FieldType == typeof(bool))
            {
                fi.SetValue(this, value != 0);
            }
            else
            {
                fi.SetValue(this, value);
            }
        }

        public int GetByIndex(int index)
        {
            FieldInfo fi = fieldInfos[index];
            if (fi.FieldType == typeof(bool))
            {
                return (bool)fi.GetValue(this) ? 1 : 0;
            }
            return (int)fi.GetValue(this);
        }

        // Index: 0 Hex: 0x00
        public int Unknown000;

        // Index: 1 Hex: 0x04
        public int Unknown001;

        // Index: 2 Hex: 0x08
        public int Unknown002;

        // Index: 3 Hex: 0x0C
        public int Unknown003;

        // Index: 4 Hex: 0x10
        public int Unknown004;

        // Index: 5 Hex: 0x14
        public int Unknown005;

        // Index: 6 Hex: 0x18
        [RWComment("Values range from 0 to 10000, where 100 popularity equals 10000. Below this value, the AI sells more stuff than usual to get money.")]
        public int CriticalPopularity;

        // Index: 7 Hex: 0x1C
        [RWComment("Below this value the AI sets taxes to zero until it reaches 'HighestPopularity' again.")]
        public int LowestPopularity;

        // Index: 8 Hex: 0x20
        [RWComment("Above this value the AI sets taxes back up")]
        public int HighestPopularity;

        // Index: 9 Hex: 0x24
        [RWNames("Unknown009")]
        [RWComment("Ranging from 0 (being +7 gifts) to 11 (being -24 taxes)")]
        public int TaxesMin;

        // Index: 10 Hex: 0x28
        [RWNames("Unknown010")]
        [RWComment("Ranging from 0 (being +7 gifts) to 11 (being -24 taxes)")]
        public int TaxesMax;

        // Index: 11 Hex: 0x2C
        public int Unknown011;

        // Index: 12 Hex: 0x30
        [RWComment("An array of farm slots, which the AI builds in the given sequence.")]
        public FarmBuilding Farm1;

        // Index: 13 Hex: 0x34
        public FarmBuilding Farm2;

        // Index: 14 Hex: 0x38
        public FarmBuilding Farm3;

        // Index: 15 Hex: 0x3C
        public FarmBuilding Farm4;

        // Index: 16 Hex: 0x40
        public FarmBuilding Farm5;

        // Index: 17 Hex: 0x44
        public FarmBuilding Farm6;

        // Index: 18 Hex: 0x48
        public FarmBuilding Farm7;

        // Index: 19 Hex: 0x4C
        public FarmBuilding Farm8;

        // Index: 20 Hex: 0x50
        [RWComment("The AI builds one farm for each amount of this population value. (Also check MaxFarms)")]
        public int PopulationPerFarm;

        // Index: 21 Hex: 0x54
        public int PopulationPerWoodcutter;

        // Index: 22 Hex: 0x58
        public int PopulationPerQuarry;

        // Index: 23 Hex: 0x5C
        public int PopulationPerIronmine;

        // Index: 24 Hex: 0x60
        public int PopulationPerPitchrig;

        // Index: 25 Hex: 0x64
        [RWComment("Setting this to zero will not disable building! Set PopulationPerQuarry to zero instead!")]
        public int MaxQuarries;

        // Index: 26 Hex: 0x68
        [RWComment("Setting this to zero will not disable building! Set PopulationPerIronmine to zero instead!")]
        public int MaxIronmines;

        // Index: 27 Hex: 0x6C
        [RWComment("Setting this to zero will not disable building! Set PopulationPerWoodcutter to zero instead!")]
        public int MaxWoodcutters;

        // Index: 28 Hex: 0x70
        [RWComment("Setting this to zero will not disable building! Set PopulationPerPitchrig to zero instead!")]
        public int MaxPitchrigs;

        // Index: 29 Hex: 0x74
        //[RWComment("The maximum amount of farms the AI builds. HopFarms are excluded from this! (Also check PopulationPerFarm)")]
        [RWComment("Setting this to zero will not disable building! Set PopulationPerFarm to zero instead!")]
        public int MaxFarms;

        // Index: 30 Hex: 0x78
        [RWComment("This is only considered <= 5000 gold")]
        public int BuildInterval;

        // Index: 31 Hex: 0x7C
        [RWComment("The delay before the AI rebuilds destroyed buildings.")]
        public int ResourceRebuildDelay;

        // Index: 32 Hex: 0x80
        [RWComment("Includes wheat. Applies to each kind of food.")]
        public int MaxFood;

        // Index: 33 Hex: 0x84
        [RWComment("Reserves that are only consumed if current popularity < LowestPopularity. If the AI has less than this amount it will prioritize buying the missing food.")]
        public int MinimumApples;

        // Index: 34 Hex: 0x88
        [RWComment("Reserves that are only consumed if current popularity < LowestPopularity. If the AI has less than this amount it will prioritize buying the missing food.")]
        public int MinimumCheese;

        // Index: 35 Hex: 0x8C
        [RWComment("Reserves that are only consumed if current popularity < LowestPopularity. If the AI has less than this amount it will prioritize buying the missing food.")]
        public int MinimumBread;

        // Index: 36 Hex: 0x90
        public int MinimumWheat;

        // Index: 37 Hex: 0x94
        [RWComment("Unclear")]
        public int MinimumHop;

        // Index: 38 Hex: 0x98
        public int TradeAmountFood;

        // Index: 39 Hex: 0x9C
        public int TradeAmountEquipment;

        // Index: 40 Hex: 0xA0
        public int Unknown040;

        // Index: 41 Hex: 0xA4
        [RWNames("Unknown041", "MinimumGoodsRequiredAfterTrade")]
        [RWComment("If the AI would have less than this amount of a good after sending them it won't send them to the requesting player.")] //Includes ResourceVariance?
        public int MinimumGoodsRequiredAfterTribute;

        // Index: 42 Hex: 0xA8
        [RWNames("Unknown042")]
        [RWComment("Above this value of food the AI will give out double rations.")]
        public int DoubleRationsFoodThreshold;

        // Index: 43 Hex: 0xAC
        public int MaxWood;

        // Index: 44 Hex: 0xB0
        public int MaxStone;

        // Index: 45 Hex: 0xB4
        public int MaxResourceOther;

        // Index: 46 Hex: 0xB8
        [RWComment("Applies to each type of weapon or armour.")]
        public int MaxEquipment;

        // Index: 47 Hex: 0xBC
        public int MaxBeer;

        // Index: 48 Hex: 0xC0
        [RWComment("added to all max resource values?")]
        public int MaxResourceVariance;

        // Index: 49 Hex: 0xC4
        [RWNames("Unknown049", "RecruitGoldThreshold")]
        [RWComment("A (gold) threshold which disables buying resources for production chains, weapons / armour and recruiting of most units.")]
        public int InvestmentGoldThreshold;

        // Index: 50 Hex: 0xC8
        public BlacksmithSetting BlacksmithSetting;

        // Index: 51 Hex: 0xCC
        public FletcherSetting FletcherSetting;

        // Index: 52 Hex: 0xD0
        public PoleturnerSetting PoleturnerSetting;

        // Index: 53 Hex: 0xD4
        public Resource SellResource01;

        // Index: 54 Hex: 0xD8
        public Resource SellResource02;

        // Index: 55 Hex: 0xDC
        public Resource SellResource03;

        // Index: 56 Hex: 0xE0
        public Resource SellResource04;

        // Index: 57 Hex: 0xE4
        public Resource SellResource05;

        // Index: 58 Hex: 0xE8
        public Resource SellResource06;

        // Index: 59 Hex: 0xEC
        public Resource SellResource07;

        // Index: 60 Hex: 0xF0
        public Resource SellResource08;

        // Index: 61 Hex: 0xF4
        public Resource SellResource09;

        // Index: 62 Hex: 0xF8
        public Resource SellResource10;

        // Index: 63 Hex: 0xFC
        public Resource SellResource11;

        // Index: 64 Hex: 0x100
        public Resource SellResource12;

        // Index: 65 Hex: 0x104
        public Resource SellResource13;

        // Index: 66 Hex: 0x108
        public Resource SellResource14;

        // Index: 67 Hex: 0x10C
        public Resource SellResource15;

        // Index: 68 Hex: 0x110
        [RWNames("Unknown068")]
        [RWComment("The amount of time for castle patrols set up in the AIV to move from one rally point to the next. (Remark: Only spearmen, horse archers and macemen currently do)")]
        public int DefWallPatrolRallyTime;

        // Index: 69 Hex: 0x114
        [RWNames("Unknown069")]
        public int DefWallPatrolGroups;

        // Index: 70 Hex: 0x118
        [RWComment("This one makes no sense. In code: [if (Gold + Threshold > 30) then RecruitEngineer()], maybe it was supposed to be minus...")]
        public int DefSiegeEngineGoldThreshold;

        // Index: 71 Hex: 0x11C
        [RWComment("The delay before the AI builds its first defensive siege engine.")]
        public int DefSiegeEngineBuildDelay;

        // Index: 72 Hex: 0x120
        //[RWNames("Unknown072")]
        public int Unknown072;

        // Index: 73 Hex: 0x124
        //[RWNames("Unknown073")]
        public int Unknown073;

        // Index: 74 Hex: 0x128
        [RWComment("The probability with which this AI reinforces missing defense troops. Note: These are ignored at the beginning of the game, as there are only sortie and defensive units being recruited.")]
        public int RecruitProbDefDefault;

        // Index: 75 Hex: 0x12C
        public int RecruitProbDefWeak;

        // Index: 76 Hex: 0x130
        public int RecruitProbDefStrong;

        // Index: 77 Hex: 0x134
        public int RecruitProbRaidDefault;

        // Index: 78 Hex: 0x138
        public int RecruitProbRaidWeak;

        // Index: 79 Hex: 0x13C
        public int RecruitProbRaidStrong;

        // Index: 80 Hex: 0x140
        public int RecruitProbAttackDefault;

        // Index: 81 Hex: 0x144
        public int RecruitProbAttackWeak;

        // Index: 82 Hex: 0x148
        public int RecruitProbAttackStrong;

        // Index: 83 Hex: 0x14C
        [RWComment("These units are only sent out if more than this amout of them has already been recruited.")]
        [RWNames("SortieUnitRangedMax", "Unknown083")]
        public int SortieUnitRangedMin;

        // Index: 84 Hex: 0x150
        [RWComment("Type of ranged units that go to the last attacked farm or building, and guard it until another is attacked. Setting it to None may cause buggy recruitment behavior.")]
        public Unit SortieUnitRanged;

        // Index: 85 Hex: 0x154
        [RWNames("SortieUnitMeleeMax", "Unknown085")]
        [RWComment("These units are only sent out if more than this amout of them has already been recruited.")]
        public int SortieUnitMeleeMin;

        // Index: 86 Hex: 0x158
        [RWComment("Type of melee units to attack enemy units shooting at the AI's buildings or workers. Setting it to None may cause buggy recruitment behavior.")]
        public Unit SortieUnitMelee;

        // Index: 87 Hex: 0x15C
        [RWComment("Amount of units that dig own moat.")]
        public int DefDiggingUnitMax;

        // Index: 88 Hex: 0x160
        [RWComment("Type of unit to dig own moat.")]
        public DiggingUnit DefDiggingUnit;

        // Index: 89 Hex: 0x164
        public int RecruitInterval;

        // Index: 90 Hex: 0x168
        [RWComment("The 'weak' state sets in if the AI is completely trashed. F.e. troops < 8, gold < 200, population < 15, ...")]
        public int RecruitIntervalWeak;

        // Index: 91 Hex: 0x16C
        [RWComment("The 'strong' state sets in if f.e. the AI has troops >= 40, gold >= 200, population >= 40, ...")]
        public int RecruitIntervalStrong;

        // Index: 92 Hex: 0x170
        [RWNames("Unknown092")]
        [RWComment("The total count of all defensive units (wall defense + patrols).")]
        public int DefTotal;

        // Index: 93 Hex: 0x174
        [RWNames("Unknown093")]
        [RWComment("The number of groups the patrols defending the outer economy split into.")]
        public int OuterPatrolGroupsCount;

        // Index: 94 Hex: 0x178
        [RWNames("Unknown094")]
        [RWComment("Whether the patrols stay at one place (quarry) or move around.")]
        public bool OuterPatrolGroupsMove;

        // Index: 95 Hex: 0x17C
        [RWNames("Unknown095")]
        //[RWComment("The amount of patrol units the AI saves up before sending them out to do their duty.")]
        [RWComment("The delay after which the AI sends out patrols to defend the outer economy. 4 is approximately one month, 24 being half a year.")]
        public int OuterPatrolRallyDelay;

        // Index: 96 Hex: 0x180
        public int DefWalls;

        // Index: 97 Hex: 0x184
        public Unit DefUnit1;

        // Index: 98 Hex: 0x188
        public Unit DefUnit2;

        // Index: 99 Hex: 0x18C
        public Unit DefUnit3;

        // Index: 100 Hex: 0x190
        public Unit DefUnit4;

        // Index: 101 Hex: 0x194
        public Unit DefUnit5;

        // Index: 102 Hex: 0x198
        public Unit DefUnit6;

        // Index: 103 Hex: 0x19C
        public Unit DefUnit7;

        // Index: 104 Hex: 0x1A0
        public Unit DefUnit8;

        // Index: 105 Hex: 0x1A4
        [RWComment("Base amount of raid troops, Special case: [unknown trigger => end result multiplied by 1.25]")]
        public int RaidUnitsBase;

        // Index: 106 Hex: 0x1A8
        [RWComment("Maximum random addition to raid troops. Special cases: [gold > 5000 => multiplied by 2][gold < 1000 => set to 0][enemy gold < 500 => divided by -2]")]
        public int RaidUnitsRandom;

        // Index: 107 Hex: 0x1AC
        public Unit RaidUnit1;

        // Index: 108 Hex: 0x1B0
        public Unit RaidUnit2;

        // Index: 109 Hex: 0x1B4
        public Unit RaidUnit3;

        // Index: 110 Hex: 0x1B8
        public Unit RaidUnit4;

        // Index: 111 Hex: 0x1BC
        public Unit RaidUnit5;

        // Index: 112 Hex: 0x1C0
        public Unit RaidUnit6;

        // Index: 113 Hex: 0x1C4
        public Unit RaidUnit7;

        // Index: 114 Hex: 0x1C8
        public Unit RaidUnit8;

        // Index: 115 Hex: 0x1CC
        [RWNames("Unknown115")]
        public HarassingSiegeEngine HarassingSiegeEngine1;

        // Index: 116 Hex: 0x1D0
        [RWNames("Unknown116")]
        public HarassingSiegeEngine HarassingSiegeEngine2;

        // Index: 117 Hex: 0x1D4
        [RWNames("Unknown117")]
        public HarassingSiegeEngine HarassingSiegeEngine3;

        // Index: 118 Hex: 0x1D8
        [RWNames("Unknown118")]
        public HarassingSiegeEngine HarassingSiegeEngine4;

        // Index: 119 Hex: 0x1DC
        [RWNames("Unknown119")]
        public HarassingSiegeEngine HarassingSiegeEngine5;

        // Index: 120 Hex: 0x1E0
        [RWNames("Unknown120")]
        public HarassingSiegeEngine HarassingSiegeEngine6;

        // Index: 121 Hex: 0x1E4
        [RWNames("Unknown121")]
        public HarassingSiegeEngine HarassingSiegeEngine7;

        // Index: 122 Hex: 0x1E8
        [RWNames("Unknown122")]
        public HarassingSiegeEngine HarassingSiegeEngine8;

        // Index: 123 Hex: 0x1EC
        [RWNames("Unknown123", "RaidCatapultsMax")]
        [RWComment("The maximum of harassing siege engines an AI builds.")]
        public int HarassingSiegeEnginesMax;

        // Index: 124 Hex: 0x1F0
        public int Unknown124;

        // Index: 125 Hex: 0x1F4
        [RWComment("The base amount of troops with which this AI attacks")]
        public int AttForceBase;

        // Index: 126 Hex: 0x1F8
        [RWComment("The maximum random amount of additional troops in an attack (this is not the amount by which the troops increase per attack!)")]
        public int AttForceRandom;

        // Index: 127 Hex: 0x1FC
        [RWNames("Unknown127", "AttForceRallyDistanceRandom")]
        [RWComment("If the AI has more than this amount of units in the attack force, it will help their allies or siege an enemy if commanded to do so.")]
        public int AttForceSupportAllyThreshold;

        // Index: 128 Hex: 0x200
        [RWComment("The %-amount of units of the attack force that the AI will rally before attacking. (0 - 100)")]
        [RWNames("Unknown128")]
        public int AttForceRallyPercentage;

        // Index: 129 Hex: 0x204
        public int Unknown129;

        // Index: 130 Hex: 0x208
        [RWNames("Unknown130")] //Delay between siege engines being build and the army moving?
        public int Unknown130;

        // Index: 131 Hex: 0x20C
        [RWComment("The delay that the AI waits until the AttUnitPatrol get a new attack-move order.")]
        [RWNames("Unknown131")]
        public int AttUnitPatrolRecommandDelay;

        // Index: 132 Hex: 0x210
        public int Unknown132;

        // Index: 133 Hex: 0x214
        public SiegeEngine SiegeEngine1;

        // Index: 134 Hex: 0x218
        public SiegeEngine SiegeEngine2;

        // Index: 135 Hex: 0x21C
        public SiegeEngine SiegeEngine3;

        // Index: 136 Hex: 0x220
        public SiegeEngine SiegeEngine4;

        // Index: 137 Hex: 0x224
        public SiegeEngine SiegeEngine5;

        // Index: 138 Hex: 0x228
        public SiegeEngine SiegeEngine6;

        // Index: 139 Hex: 0x22C
        public SiegeEngine SiegeEngine7;

        // Index: 140 Hex: 0x230
        public SiegeEngine SiegeEngine8;

        // Index: 141 Hex: 0x234
        //Maybe the amount of stones thrown by all catapults until a cow is thrown instead
        [RWNames("Unknown141")]
        [RWComment("The amount of stones needed to be thrown until the AI throws a diseased cow instead (catapults & trebuchets). Value 0 disables cows and -1 makes the AI not throw any bolders, only cows.")]
        public int CowThrowInterval;

        // Index: 142 Hex: 0x238
        public int Unknown142;

        // Index: 143 Hex: 0x23C
        public int AttMaxEngineers;

        // Index: 144 Hex: 0x240
        [RWComment("This unit is only recruited if the target enemy has moat and used preferably to fill up enemy moat.")]
        public DiggingUnit AttDiggingUnit;

        // Index: 145 Hex: 0x244
        public int AttDiggingUnitMax;

        // Index: 146 Hex: 0x248
        [RWNames("Unknown146")]
        //Not without improved attack waves: [RWComment("These units split from the main attack force to destroy enemy buildings. If the enemy walls are nearby those may be attacked.")]
        public Unit AttUnit2;

        // Index: 147 Hex: 0x24C
        [RWNames("Unknown147")]
        public int AttUnit2Max;

        // Index: 148 Hex: 0x250
        public int AttMaxAssassins;

        // Index: 149 Hex: 0x254
        public int AttMaxLaddermen;

        // Index: 150 Hex: 0x258
        public int AttMaxTunnelers;

        // Index: 151 Hex: 0x25C
        [RWNames("Unknown151", "AttUnitRangedPush")]
        [RWComment("Ranged attack unit that patrols around the enemy castle / keep. Preferably ranged units should be used here.")]
        public Unit AttUnitPatrol;

        // Index: 152 Hex: 0x260
        [RWNames("Unknown152", "AttUnitRangedPushMax")]
        public int AttUnitPatrolMax;

        // Index: 153 Hex: 0x264
        [RWNames("Unknown153", "RangedPushGroupsCount")]
        [RWComment("# of groups the AttUnitPatrol split into. BUGGY! More than 1 group results to only a single group attacking, the others standing idle.")]
        public int AttUnitPatrolGroupsCount;

        // Index: 154 Hex: 0x268
        [RWComment("Attacking unit that holds position and doesn't attack until the walls are breached.")]
        public Unit AttUnitBackup; // ranged?

        // Index: 155 Hex: 0x26C
        public int AttUnitBackupMax;

        // Index: 156 Hex: 0x270
        [RWNames("Unknown156", "RangedBackupGroupsCount")]
        [RWComment("# of groups the AttUnitBackup split into. If shields are present in the army, one will be added to each group (if possible).")]
        public int AttUnitBackupGroupsCount;

        // Index: 157 Hex: 0x274
        [RWNames("Unknown157", "AttUnit5")]
        [RWComment("Units that engage enemy groups of units outside the castle. Prioritizes larger groups no matter where they are on the map. Otherwise destroys buildings outside the castle.")]
        public Unit AttUnitEngage;

        // Index: 158 Hex: 0x278
        [RWNames("Unknown158", "AttUnit5Max")]
        public int AttUnitEngageMax;

        // Index: 159 Hex: 0x27C
        [RWComment("These units patrol between siege engines in order to protect them.")]
        public Unit AttUnitSiegeDef;

        // Index: 160 Hex: 0x280
        public int AttUnitSiegeDefMax;

        // Index: 161 Hex: 0x284
        [RWNames("Unknown161")]
        public int AttUnitSiegeDefGroupsCount;

        // Index: 162 Hex: 0x288
        [RWComment("AttUntiMain1 to AttUnitMain4 is a list of the main strike force the AI recruits for sieges. Priotizes in order of the list, but only recruits units for which they have enough gold. So try to place expensive units higher up.")]
        public Unit AttUnitMain1;

        // Index: 163 Hex: 0x28C
        public Unit AttUnitMain2;

        // Index: 164 Hex: 0x290
        [RWNames("Unknown164")]
        public Unit AttUnitMain3;

        // Index: 165 Hex: 0x294
        [RWNames("Unknown165")]
        public Unit AttUnitMain4;

        // Index: 166 Hex: 0x298
        [RWComment("This does nothing")]
        public int AttMaxDefault;

        // Index: 167 Hex: 0x29C
        [RWComment("# of groups all the AttUnitMain split into. Maximum is 3")]
        [RWNames("Unknown167")]
        public int AttMainGroupsCount;

        // Index: 168 Hex: 0x2A0
        public TargetingType TargetChoice;
    }
}
