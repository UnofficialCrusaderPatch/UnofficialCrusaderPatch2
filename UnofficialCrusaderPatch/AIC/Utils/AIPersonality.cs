using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace UCPAIConversion
{
    public class AIPersonality
    {
        private int _CriticalPopularity;
        private int _LowestPopularity;
        private int _HighestPopularity;
        private int _TaxesMin;
        private int _TaxesMax;
        private int _PopulationPerFarm;
        private int _PopulationPerWoodcutter;
        private int _PopulationPerQuarry;
        private int _PopulationPerIronmine;
        private int _PopulationPerPitchrig;
        private int _MaxQuarries;
        private int _MaxIronmines;
        private int _MaxWoodcutters;
        private int _MaxPitchrigs;
        private int _MaxFarms;
        private int _TradeAmountFood;
        private int _TradeAmountEquipment;
        private int _MinimumGoodsRequiredAfterTrade;
        private int _RecruitProbDefDefault;
        private int _RecruitProbDefWeak;
        private int _RecruitProbDefStrong;
        private int _RecruitProbRaidDefault;
        private int _RecruitProbRaidWeak;
        private int _RecruitProbRaidStrong;
        private int _RecruitProbAttackDefault;
        private int _RecruitProbAttackWeak;
        private int _RecruitProbAttackStrong;
        private int _SortieUnitRangedMin;
        private int _SortieUnitMeleeMin;
        private int _DefTotal;
        private int _RaidUnitsBase;
        private int _AttForceBase;
        private int _AttForceRallyPercentage;
        private int _CowThrowInterval;
        private int _AttUnitPatrolGroupsCount;
        private int _AttUnitBackupGroupsCount;
        private int _AttMainGroupsCount;

        // Index: 0 Hex: 0x00
        public int Unknown000 { get; set; }

        // Index: 1 Hex: 0x04
        public int Unknown001 { get; set; }

        // Index: 2 Hex: 0x08
        public int Unknown002 { get; set; }

        // Index: 3 Hex: 0x0C
        public int Unknown003 { get; set; }

        // Index: 4 Hex: 0x10
        public int Unknown004 { get; set; }

        // Index: 5 Hex: 0x14
        public int Unknown005 { get; set; }

        // Index: 6 Hex: 0x18
        [RWComment("Values range from 0 to 10000, where 100 popularity equals 10000. Below this value, the AI sells more stuff than usual to get money.")]
        public int CriticalPopularity
        {
            get
            {
                return _CriticalPopularity;
            }
            set
            {
                if (value < 0 || value > 10000)
                {
                    throw new ArgumentException();
                }
                _CriticalPopularity = value;
            }
        }

        // Index: 7 Hex: 0x1C
        [RWComment("Below this value the AI sets taxes to zero until it reaches 'HighestPopularity' again.")]
        public int LowestPopularity
        {
            get
            {
                return _LowestPopularity;
            }
            set
            {
                if (value < 0 || value > 10000)
                {
                    throw new ArgumentException();
                }
                _LowestPopularity = value;
            }
        }

        // Index: 8 Hex: 0x20
        [RWComment("Above this value the AI sets taxes back up")]
        public int HighestPopularity
        {
            get
            {
                return _HighestPopularity;
            }
            set
            {
                if (value < 0 || value > 10000)
                {
                    throw new ArgumentException();
                }
                _HighestPopularity = value;
            }
        }

        // Index: 9 Hex: 0x24
        [RWNames("Unknown009")]
        [RWComment("Ranging from 0 (being +7 gifts) to 11 (being -24 taxes)")]
        public int TaxesMin
        {
            get
            {
                return _TaxesMin;
            }
            set
            {
                if (value < 0 || value > 12)
                {
                    throw new ArgumentException();
                }
                _TaxesMin = value;
            }
        }

        // Index: 10 Hex: 0x28
        [RWNames("Unknown010")]
        [RWComment("Ranging from 0 (being +7 gifts) to 11 (being -24 taxes)")]
        public int TaxesMax
        {
            get
            {
                return _TaxesMax;
            }
            set
            {
                if (value < 0 || value > 12)
                {
                    throw new ArgumentException();
                }
                _TaxesMax = value;
            }
        }

        // Index: 11 Hex: 0x2C
        public int Unknown011 { get; set; }

        // Index: 12 Hex: 0x30
        [RWComment("An array of farm slots, which the AI builds in the given sequence.")]
        [ScriptIgnore]
        public FarmBuilding _Farm1 { get; set; }
        public string Farm1 {
            get => Enum.GetName(typeof(FarmBuilding), _Farm1);
            set
            {
                _Farm1 = (FarmBuilding)Enum.Parse(typeof(FarmBuilding), value);
            }
        }

        // Index: 13 Hex: 0x34
        [ScriptIgnore]
        public FarmBuilding _Farm2 { get; set; }
        public string Farm2
        {
            get => Enum.GetName(typeof(FarmBuilding), _Farm2);
            set
            {
                _Farm2 = (FarmBuilding)Enum.Parse(typeof(FarmBuilding), value);
            }
        }

        // Index: 14 Hex: 0x38
        [ScriptIgnore]
        public FarmBuilding _Farm3 { get; set; }
        public string Farm3
        {
            get => Enum.GetName(typeof(FarmBuilding), _Farm3);
            set
            {
                _Farm3 = (FarmBuilding)Enum.Parse(typeof(FarmBuilding), value);
            }
        }


        // Index: 15 Hex: 0x3C
        [ScriptIgnore]
        public FarmBuilding _Farm4 { get; set; }
        public string Farm4
        {
            get => Enum.GetName(typeof(FarmBuilding), _Farm4);
            set
            {
                _Farm4 = (FarmBuilding)Enum.Parse(typeof(FarmBuilding), value);
            }
        }

        // Index: 16 Hex: 0x40
        [ScriptIgnore]
        public FarmBuilding _Farm5 { get; set; }
        public string Farm5
        {
            get => Enum.GetName(typeof(FarmBuilding), _Farm5);
            set
            {
                _Farm5 = (FarmBuilding)Enum.Parse(typeof(FarmBuilding), value);
            }
        }

        // Index: 17 Hex: 0x44
        [ScriptIgnore]
        public FarmBuilding _Farm6 { get; set; }
        public string Farm6
        {
            get => Enum.GetName(typeof(FarmBuilding), _Farm6);
            set
            {
                _Farm6 = (FarmBuilding)Enum.Parse(typeof(FarmBuilding), value);
            }
        }

        // Index: 18 Hex: 0x48
        [ScriptIgnore]
        public FarmBuilding _Farm7 { get; set; }
        public string Farm7
        {
            get => Enum.GetName(typeof(FarmBuilding), _Farm7);
            set
            {
                _Farm7 = (FarmBuilding)Enum.Parse(typeof(FarmBuilding), value);
            }
        }

        // Index: 19 Hex: 0x4C
        [ScriptIgnore]
        public FarmBuilding _Farm8 { get; set; }
        public string Farm8
        {
            get => Enum.GetName(typeof(FarmBuilding), _Farm8);
            set
            {
                _Farm8 = (FarmBuilding)Enum.Parse(typeof(FarmBuilding), value);
            }
        }

        // Index: 20 Hex: 0x50
        [RWComment("The AI builds one farm for each amount of this population value. (Also check MaxFarms)")]
        public int PopulationPerFarm
        {
            get
            {
                return _PopulationPerFarm;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _PopulationPerFarm = value;
            }
        }

        // Index: 21 Hex: 0x54
        public int PopulationPerWoodcutter
        {
            get
            {
                return _PopulationPerWoodcutter;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _PopulationPerWoodcutter = value;
            }
        }

        // Index: 22 Hex: 0x58
        public int PopulationPerQuarry
        {
            get
            {
                return _PopulationPerQuarry;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _PopulationPerQuarry = value;
            }
        }

        // Index: 23 Hex: 0x5C
        public int PopulationPerIronmine
        {
            get
            {
                return _PopulationPerIronmine;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _PopulationPerIronmine = value;
            }
        }

        // Index: 24 Hex: 0x60
        public int PopulationPerPitchrig
        {
            get
            {
                return _PopulationPerPitchrig;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _PopulationPerPitchrig = value;
            }
        }

        // Index: 25 Hex: 0x64
        [RWComment("Setting this to zero will not disable building! Set PopulationPerQuarry to zero instead!")]
        public int MaxQuarries
        {
            get
            {
                return _MaxQuarries;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _MaxQuarries = value;
            }
        }

        // Index: 26 Hex: 0x68
        [RWComment("Setting this to zero will not disable building! Set PopulationPerIronmine to zero instead!")]
        public int MaxIronmines
        {
            get
            {
                return _MaxIronmines;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _MaxIronmines = value;
            }
        }

        // Index: 27 Hex: 0x6C
        [RWComment("Setting this to zero will not disable building! Set PopulationPerWoodcutter to zero instead!")]
        public int MaxWoodcutters
        {
            get
            {
                return _MaxWoodcutters;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _MaxWoodcutters = value;
            }
        }

        // Index: 28 Hex: 0x70
        [RWComment("Setting this to zero will not disable building! Set PopulationPerPitchrig to zero instead!")]
        public int MaxPitchrigs
        {
            get
            {
                return _MaxPitchrigs;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _MaxPitchrigs = value;
            }
        }

        // Index: 29 Hex: 0x74
        //[RWComment("The maximum amount of farms the AI builds. HopFarms are excluded from this! (Also check PopulationPerFarm)")]
        [RWComment("Setting this to zero will not disable building! Set PopulationPerFarm to zero instead!")]
        public int MaxFarms
        {
            get
            {
                return _MaxFarms;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _MaxFarms = value;
            }
        }

        // Index: 30 Hex: 0x78
        [RWComment("This is only considered <= 5000 gold")]
        public int BuildInterval { get; set; }

        // Index: 31 Hex: 0x7C
        [RWComment("The delay before the AI rebuilds destroyed buildings.")]
        public int ResourceRebuildDelay { get; set; }

        // Index: 32 Hex: 0x80
        [RWComment("Includes wheat. Applies to each kind of food.")]
        public int MaxFood { get; set; }

        // Index: 33 Hex: 0x84
        [RWComment("Reserves that are only consumed if current popularity < LowestPopularity. If the AI has less than this amount it will prioritize buying the missing food.")]
        public int MinimumApples { get; set; }

        // Index: 34 Hex: 0x88
        [RWComment("Reserves that are only consumed if current popularity < LowestPopularity. If the AI has less than this amount it will prioritize buying the missing food.")]
        public int MinimumCheese { get; set; }

        // Index: 35 Hex: 0x8C
        [RWComment("Reserves that are only consumed if current popularity < LowestPopularity. If the AI has less than this amount it will prioritize buying the missing food.")]
        public int MinimumBread { get; set; }

        // Index: 36 Hex: 0x90
        public int MinimumWheat { get; set; }

        // Index: 37 Hex: 0x94
        [RWComment("Unclear")]
        public int MinimumHop { get; set; }

        // Index: 38 Hex: 0x98
        public int TradeAmountFood
        {
            get
            {
                return _TradeAmountFood;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _TradeAmountFood = value;
            }
        }

        // Index: 39 Hex: 0x9C
        public int TradeAmountEquipment
        {
            get
            {
                return _TradeAmountEquipment;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _TradeAmountEquipment = value;
            }
        }

        // Index: 40 Hex: 0xA0
        public int Unknown040 { get; set; }

        // Index: 41 Hex: 0xA4
        [RWNames("Unknown041", "MinimumGoodsRequiredAfterTrade")]
        [RWComment("If the AI would have less than this amount of a good after sending them it won't send them to the requesting player.")] //Includes ResourceVariance?
        public int MinimumGoodsRequiredAfterTrade
        {
            get
            {
                return _MinimumGoodsRequiredAfterTrade;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _MinimumGoodsRequiredAfterTrade = value;
            }
        }

        // Index: 42 Hex: 0xA8
        [RWNames("Unknown042")]
        [RWComment("Above this value of food the AI will give out double rations.")]
        public int DoubleRationsFoodThreshold { get; set; }

        // Index: 43 Hex: 0xAC
        public int MaxWood { get; set; }

        // Index: 44 Hex: 0xB0
        public int MaxStone { get; set; }

        // Index: 45 Hex: 0xB4
        public int MaxResourceOther { get; set; }

        // Index: 46 Hex: 0xB8
        [RWComment("Applies to each type of weapon or armour.")]
        public int MaxEquipment { get; set; }

        // Index: 47 Hex: 0xBC
        public int MaxBeer { get; set; }

        // Index: 48 Hex: 0xC0
        [RWComment("added to all max resource values?")]
        public int MaxResourceVariance { get; set; }

        // Index: 49 Hex: 0xC4
        [RWNames("Unknown049", "RecruitGoldThreshold")]
        [RWComment("A (gold) threshold which disables buying resources for production chains, weapons / armour and recruiting of most units.")]
        public int RecruitGoldThreshold { get; set; }

        // Index: 50 Hex: 0xC8
        [ScriptIgnore]
        public BlacksmithSetting _BlacksmithSetting { get; set; }
        public string BlacksmithSetting
        {
            get => Enum.GetName(typeof(BlacksmithSetting), _BlacksmithSetting);
            set
            {
                _BlacksmithSetting = (BlacksmithSetting)Enum.Parse(typeof(BlacksmithSetting), value);
            }
        }

        // Index: 51 Hex: 0xCC
        [ScriptIgnore]
        public FletcherSetting _FletcherSetting { get; set; }
        public string FletcherSetting
        {
            get => Enum.GetName(typeof(FletcherSetting), _FletcherSetting);
            set
            {
                _FletcherSetting = (FletcherSetting)Enum.Parse(typeof(FletcherSetting), value);
            }
        }

        // Index: 52 Hex: 0xD0
        [ScriptIgnore]
        public PoleturnerSetting _PoleturnerSetting { get; set; }
        public string PoleturnerSetting
        {
            get => Enum.GetName(typeof(PoleturnerSetting), _PoleturnerSetting);
            set
            {
                _PoleturnerSetting = (PoleturnerSetting)Enum.Parse(typeof(PoleturnerSetting), value);
            }
        }


        // Index: 53 Hex: 0xD4
        [ScriptIgnore]
        public Resource _SellResource01 { get; set; }
        public string SellResource01
        {
            get => Enum.GetName(typeof(Resource), _SellResource01);
            set
            {
                _SellResource01 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 54 Hex: 0xD8
        [ScriptIgnore]
        public Resource _SellResource02 { get; set; }
        public string SellResource02
        {
            get => Enum.GetName(typeof(Resource), _SellResource02);
            set
            {
                _SellResource02 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 55 Hex: 0xDC
        [ScriptIgnore]
        public Resource _SellResource03 { get; set; }
        public string SellResource03
        {
            get => Enum.GetName(typeof(Resource), _SellResource03);
            set
            {
                _SellResource03 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 56 Hex: 0xE0
        [ScriptIgnore]
        public Resource _SellResource04 { get; set; }
        public string SellResource04
        {
            get => Enum.GetName(typeof(Resource), _SellResource04);
            set
            {
                _SellResource04 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 57 Hex: 0xE4
        [ScriptIgnore]
        public Resource _SellResource05 { get; set; }
        public string SellResource05
        {
            get => Enum.GetName(typeof(Resource), _SellResource05);
            set
            {
                _SellResource05 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 58 Hex: 0xE8
        [ScriptIgnore]
        public Resource _SellResource06 { get; set; }
        public string SellResource06
        {
            get => Enum.GetName(typeof(Resource), _SellResource06);
            set
            {
                _SellResource06 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 59 Hex: 0xEC
        [ScriptIgnore]
        public Resource _SellResource07 { get; set; }
        public string SellResource07
        {
            get => Enum.GetName(typeof(Resource), _SellResource07);
            set
            {
                _SellResource07 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 60 Hex: 0xF0
        [ScriptIgnore]
        public Resource _SellResource08 { get; set; }
        public string SellResource08
        {
            get => Enum.GetName(typeof(Resource), _SellResource08);
            set
            {
                _SellResource08 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 61 Hex: 0xF4
        [ScriptIgnore]
        public Resource _SellResource09 { get; set; }
        public string SellResource09
        {
            get => Enum.GetName(typeof(Resource), _SellResource09);
            set
            {
                _SellResource09 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 62 Hex: 0xF8
        [ScriptIgnore]
        public Resource _SellResource10 { get; set; }
        public string SellResource10
        {
            get => Enum.GetName(typeof(Resource), _SellResource10);
            set
            {
                _SellResource10 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 63 Hex: 0xFC
        [ScriptIgnore]
        public Resource _SellResource11 { get; set; }
        public string SellResource11
        {
            get => Enum.GetName(typeof(Resource), _SellResource11);
            set
            {
                _SellResource11 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 64 Hex: 0x100
        [ScriptIgnore]
        public Resource _SellResource12 { get; set; }
        public string SellResource12
        {
            get => Enum.GetName(typeof(Resource), _SellResource12);
            set
            {
                _SellResource12 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 65 Hex: 0x104
        [ScriptIgnore]
        public Resource _SellResource13 { get; set; }
        public string SellResource13
        {
            get => Enum.GetName(typeof(Resource), _SellResource13);
            set
            {
                _SellResource13 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 66 Hex: 0x108
        [ScriptIgnore]
        public Resource _SellResource14 { get; set; }
        public string SellResource14
        {
            get => Enum.GetName(typeof(Resource), _SellResource14);
            set
            {
                _SellResource14 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 67 Hex: 0x10C
        [ScriptIgnore]
        public Resource _SellResource15 { get; set; }
        public string SellResource15
        {
            get => Enum.GetName(typeof(Resource), _SellResource15);
            set
            {
                _SellResource15 = (Resource)Enum.Parse(typeof(Resource), value);
            }
        }

        // Index: 68 Hex: 0x110
        [RWNames("Unknown068")]
        [RWComment("The amount of time for castle patrols set up in the AIV to move from one rally point to the next. (Remark: Only spearmen, horse archers and macemen currently do)")]
        public int DefWallPatrolRallyTime { get; set; }

        // Index: 69 Hex: 0x114
        [RWNames("Unknown069")]
        public int DefWallPatrolGroups { get; set; }

        // Index: 70 Hex: 0x118
        [RWComment("This one makes no sense. In code: [if (Gold + Threshold > 30) then RecruitEngineer()], maybe it was supposed to be minus...")]
        public int DefSiegeEngineGoldThreshold { get; set; }

        // Index: 71 Hex: 0x11C
        [RWComment("The delay before the AI builds its first defensive siege engine.")]
        public int DefSiegeEngineBuildDelay { get; set; }

        // Index: 72 Hex: 0x120
        //[RWNames("Unknown072")]
        public int Unknown072 { get; set; }

        // Index: 73 Hex: 0x124
        //[RWNames("Unknown073")]
        public int Unknown073 { get; set; }

        // Index: 74 Hex: 0x128
        [RWComment("The probability with which this AI reinforces missing defense troops. Note: These are ignored at the beginning of the game, as there are only sortie and defensive units being recruited.")]
        public int RecruitProbDefDefault
        {
            get
            {
                return _RecruitProbDefDefault;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException();
                }
                _RecruitProbDefDefault = value;
            }
        }

        // Index: 75 Hex: 0x12C
        public int RecruitProbDefWeak
        {
            get
            {
                return _RecruitProbDefWeak;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException();
                }
                _RecruitProbDefWeak = value;
            }
        }

        // Index: 76 Hex: 0x130
        public int RecruitProbDefStrong
        {
            get
            {
                return _RecruitProbDefStrong;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException();
                }
                _RecruitProbDefStrong = value;
            }
        }

        // Index: 77 Hex: 0x134
        public int RecruitProbRaidDefault
        {
            get
            {
                return _RecruitProbRaidDefault;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException();
                }
                _RecruitProbRaidDefault = value;
            }
        }

        // Index: 78 Hex: 0x138
        public int RecruitProbRaidWeak
        {
            get
            {
                return _RecruitProbRaidWeak;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException();
                }
                _RecruitProbRaidWeak = value;
            }
        }

        // Index: 79 Hex: 0x13C
        public int RecruitProbRaidStrong
        {
            get
            {
                return _RecruitProbRaidStrong;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException();
                }
                _RecruitProbRaidStrong = value;
            }
        }

        // Index: 80 Hex: 0x140
        public int RecruitProbAttackDefault
        {
            get
            {
                return _RecruitProbAttackDefault;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException();
                }
                _RecruitProbAttackDefault = value;
            }
        }

        // Index: 81 Hex: 0x144
        public int RecruitProbAttackWeak
        {
            get
            {
                return _RecruitProbAttackWeak;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException();
                }
                _RecruitProbAttackWeak = value;
            }
        }

        // Index: 82 Hex: 0x148
        public int RecruitProbAttackStrong
        {
            get
            {
                return _RecruitProbAttackStrong;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException();
                }
                _RecruitProbAttackStrong = value;
            }
        }

        // Index: 83 Hex: 0x14C
        [RWComment("These units are only sent out if more than this amount of them has already been recruited.")]
        [RWNames("SortieUnitRangedMax", "Unknown083")]
        public int SortieUnitRangedMin
        {
            get
            {
                return _SortieUnitRangedMin;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _SortieUnitRangedMin = value;
            }
        }

        // Index: 84 Hex: 0x150
        [ScriptIgnore]
        [RWComment("Type of ranged units that go to the last attacked farm or building, and guard it until another is attacked. Setting it to None may cause buggy recruitment behavior.")]
        public Unit _SortieUnitRanged { get; set; }
        public string SortieUnitRanged
        {
            get => Enum.GetName(typeof(Unit), _SortieUnitRanged);
            set
            {
                _SortieUnitRanged = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }


        // Index: 85 Hex: 0x154
        [RWNames("SortieUnitMeleeMax", "Unknown085")]
        [RWComment("These units are only sent out if more than this amount of them has already been recruited.")]
        public int SortieUnitMeleeMin
        {
            get
            {
                return _SortieUnitMeleeMin;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _SortieUnitMeleeMin = value;
            }
        }

        // Index: 86 Hex: 0x158
        [ScriptIgnore]
        [RWComment("Type of melee units to attack enemy units shooting at the AI's buildings or workers. Setting it to None may cause buggy recruitment behavior.")]
        public Unit _SortieUnitMelee { get; set; }
        public string SortieUnitMelee
        {
            get => Enum.GetName(typeof(Unit), _SortieUnitMelee);
            set
            {
                _SortieUnitMelee = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 87 Hex: 0x15C
        [RWComment("Amount of units that dig own moat.")]
        public int DefDiggingUnitMax { get; set; }

        // Index: 88 Hex: 0x160
        [ScriptIgnore]
        [RWComment("Type of unit to dig own moat.")]
        public DiggingUnit _DefDiggingUnit { get; set; }
        public string DefDiggingUnit
        {
            get => Enum.GetName(typeof(DiggingUnit), _DefDiggingUnit);
            set
            {
                _DefDiggingUnit = (DiggingUnit)Enum.Parse(typeof(DiggingUnit), value);
            }
        }

        // Index: 89 Hex: 0x164
        public int RecruitInterval { get; set; }

        // Index: 90 Hex: 0x168
        [RWComment("The 'weak' state sets in if the AI is completely trashed. For example troops < 8, gold < 200, population < 15, etc")]
        public int RecruitIntervalWeak { get; set; }

        // Index: 91 Hex: 0x16C
        [RWComment("The 'strong' state sets in if for example the AI has troops >= 40, gold >= 200, population >= 40, etc")]
        public int RecruitIntervalStrong { get; set; }

        // Index: 92 Hex: 0x170
        [RWNames("Unknown092")]
        [RWComment("The total count of all defensive units (wall defense + patrols).")]
        public int DefTotal
        {
            get
            {
                return _DefTotal;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _DefTotal = value;
            }
        }

        // Index: 93 Hex: 0x174
        [RWNames("Unknown093")]
        [RWComment("The number of groups the patrols defending the outer economy split into.")]
        public int OuterPatrolGroupsCount { get; set; }

        // Index: 94 Hex: 0x178
        [RWNames("Unknown094")]
        [RWComment("Whether the patrols stay at one place (quarry) or move around.")]
        public bool OuterPatrolGroupsMove { get; set; }

        // Index: 95 Hex: 0x17C
        [RWNames("Unknown095")]
        //[RWComment("The amount of patrol units the AI saves up before sending them out to do their duty.")]
        [RWComment("The delay after which the AI sends out patrols to defend the outer economy. 4 is approximately one month, 24 being half a year.")]
        public int OuterPatrolRallyDelay { get; set; }

        // Index: 96 Hex: 0x180
        public int DefWalls { get; set; }

        // Index: 97 Hex: 0x184
        [ScriptIgnore]
        public Unit _DefUnit1 { get; set; }
        public string DefUnit1
        {
            get => Enum.GetName(typeof(Unit), _DefUnit1);
            set
            {
                _DefUnit1 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 98 Hex: 0x188
        [ScriptIgnore]
        public Unit _DefUnit2 { get; set; }
        public string DefUnit2
        {
            get => Enum.GetName(typeof(Unit), _DefUnit2);
            set
            {
                _DefUnit2 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 99 Hex: 0x18C
        [ScriptIgnore]
        public Unit _DefUnit3 { get; set; }
        public string DefUnit3
        {
            get => Enum.GetName(typeof(Unit), _DefUnit3);
            set
            {
                _DefUnit3 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 100 Hex: 0x190
        [ScriptIgnore]
        public Unit _DefUnit4 { get; set; }
        public string DefUnit4
        {
            get => Enum.GetName(typeof(Unit), _DefUnit4);
            set
            {
                _DefUnit4 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 101 Hex: 0x194
        [ScriptIgnore]
        public Unit _DefUnit5 { get; set; }
        public string DefUnit5
        {
            get => Enum.GetName(typeof(Unit), _DefUnit5);
            set
            {
                _DefUnit5 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 102 Hex: 0x198
        [ScriptIgnore]
        public Unit _DefUnit6 { get; set; }
        public string DefUnit6
        {
            get => Enum.GetName(typeof(Unit), _DefUnit6);
            set
            {
                _DefUnit6 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 103 Hex: 0x19C
        [ScriptIgnore]
        public Unit _DefUnit7 { get; set; }
        public string DefUnit7
        {
            get => Enum.GetName(typeof(Unit), _DefUnit7);
            set
            {
                _DefUnit7 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 104 Hex: 0x1A0
        [ScriptIgnore]
        public Unit _DefUnit8 { get; set; }
        public string DefUnit8
        {
            get => Enum.GetName(typeof(Unit), _DefUnit8);
            set
            {
                _DefUnit8 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 105 Hex: 0x1A4
        [RWComment("Base amount of raid troops, Special case: [unknown trigger => end result multiplied by 1.25]")]
        public int RaidUnitsBase
        {
            get
            {
                return _RaidUnitsBase;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _RaidUnitsBase = value;
            }
        }

        // Index: 106 Hex: 0x1A8
        [RWComment("Maximum random addition to raid troops. Special cases: [gold > 5000 => multiplied by 2][gold < 1000 => set to 0][enemy gold < 500 => divided by -2]")]
        public int RaidUnitsRandom { get; set; }

        // Index: 107 Hex: 0x1AC
        [ScriptIgnore]
        public Unit _RaidUnit1 { get; set; }
        public string RaidUnit1
        {
            get => Enum.GetName(typeof(Unit), _RaidUnit1);
            set
            {
                _RaidUnit1 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 108 Hex: 0x1B0
        [ScriptIgnore]
        public Unit _RaidUnit2 { get; set; }
        public string RaidUnit2
        {
            get => Enum.GetName(typeof(Unit), _RaidUnit2);
            set
            {
                _RaidUnit2 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 109 Hex: 0x1B4
        [ScriptIgnore]
        public Unit _RaidUnit3 { get; set; }
        public string RaidUnit3
        {
            get => Enum.GetName(typeof(Unit), _RaidUnit3);
            set
            {
                _RaidUnit3 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 110 Hex: 0x1B8
        [ScriptIgnore]
        public Unit _RaidUnit4 { get; set; }
        public string RaidUnit4
        {
            get => Enum.GetName(typeof(Unit), _RaidUnit4);
            set
            {
                _RaidUnit4 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 111 Hex: 0x1BC
        [ScriptIgnore]
        public Unit _RaidUnit5 { get; set; }
        public string RaidUnit5
        {
            get => Enum.GetName(typeof(Unit), _RaidUnit5);
            set
            {
                _RaidUnit5 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 112 Hex: 0x1C0
        [ScriptIgnore]
        public Unit _RaidUnit6 { get; set; }
        public string RaidUnit6
        {
            get => Enum.GetName(typeof(Unit), _RaidUnit6);
            set
            {
                _RaidUnit6 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 113 Hex: 0x1C4
        [ScriptIgnore]
        public Unit _RaidUnit7 { get; set; }
        public string RaidUnit7
        {
            get => Enum.GetName(typeof(Unit), _RaidUnit7);
            set
            {
                _RaidUnit7 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 114 Hex: 0x1C8
        [ScriptIgnore]
        public Unit _RaidUnit8 { get; set; }
        public string RaidUnit8
        {
            get => Enum.GetName(typeof(Unit), _RaidUnit8);
            set
            {
                _RaidUnit8 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 115 Hex: 0x1CC
        [ScriptIgnore]
        [RWNames("Unknown115")]
        public HarassingSiegeEngine _HarassingSiegeEngine1 { get; set; }
        public string HarassingSiegeEngine1
        {
            get => Enum.GetName(typeof(HarassingSiegeEngine), _HarassingSiegeEngine1);
            set
            {
                _HarassingSiegeEngine1 = (HarassingSiegeEngine)Enum.Parse(typeof(HarassingSiegeEngine), value);
            }
        }

        // Index: 116 Hex: 0x1D0
        [ScriptIgnore]
        [RWNames("Unknown116")]
        public HarassingSiegeEngine _HarassingSiegeEngine2 { get; set; }
        public string HarassingSiegeEngine2
        {
            get => Enum.GetName(typeof(HarassingSiegeEngine), _HarassingSiegeEngine2);
            set
            {
                _HarassingSiegeEngine2 = (HarassingSiegeEngine)Enum.Parse(typeof(HarassingSiegeEngine), value);
            }
        }

        // Index: 117 Hex: 0x1D4
        [ScriptIgnore]
        [RWNames("Unknown117")]
        public HarassingSiegeEngine _HarassingSiegeEngine3 { get; set; }
        public string HarassingSiegeEngine3
        {
            get => Enum.GetName(typeof(HarassingSiegeEngine), _HarassingSiegeEngine3);
            set
            {
                _HarassingSiegeEngine3 = (HarassingSiegeEngine)Enum.Parse(typeof(HarassingSiegeEngine), value);
            }
        }

        // Index: 118 Hex: 0x1D8
        [ScriptIgnore]
        [RWNames("Unknown118")]
        public HarassingSiegeEngine _HarassingSiegeEngine4 { get; set; }
        public string HarassingSiegeEngine4
        {
            get => Enum.GetName(typeof(HarassingSiegeEngine), _HarassingSiegeEngine4);
            set
            {
                _HarassingSiegeEngine4 = (HarassingSiegeEngine)Enum.Parse(typeof(HarassingSiegeEngine), value);
            }
        }

        // Index: 119 Hex: 0x1DC
        [ScriptIgnore]
        [RWNames("Unknown119")]
        public HarassingSiegeEngine _HarassingSiegeEngine5 { get; set; }
        public string HarassingSiegeEngine5
        {
            get => Enum.GetName(typeof(HarassingSiegeEngine), _HarassingSiegeEngine5);
            set
            {
                _HarassingSiegeEngine5 = (HarassingSiegeEngine)Enum.Parse(typeof(HarassingSiegeEngine), value);
            }
        }

        // Index: 120 Hex: 0x1E0
        [ScriptIgnore]
        [RWNames("Unknown120")]
        public HarassingSiegeEngine _HarassingSiegeEngine6 { get; set; }
        public string HarassingSiegeEngine6
        {
            get => Enum.GetName(typeof(HarassingSiegeEngine), _HarassingSiegeEngine6);
            set
            {
                _HarassingSiegeEngine6 = (HarassingSiegeEngine)Enum.Parse(typeof(HarassingSiegeEngine), value);
            }
        }

        // Index: 121 Hex: 0x1E4
        [ScriptIgnore]
        [RWNames("Unknown121")]
        public HarassingSiegeEngine _HarassingSiegeEngine7 { get; set; }
        public string HarassingSiegeEngine7
        {
            get => Enum.GetName(typeof(HarassingSiegeEngine), _HarassingSiegeEngine7);
            set
            {
                _HarassingSiegeEngine7 = (HarassingSiegeEngine)Enum.Parse(typeof(HarassingSiegeEngine), value);
            }
        }

        // Index: 122 Hex: 0x1E8
        [ScriptIgnore]
        [RWNames("Unknown122")]
        public HarassingSiegeEngine _HarassingSiegeEngine8 { get; set; }
        public string HarassingSiegeEngine8
        {
            get => Enum.GetName(typeof(HarassingSiegeEngine), _HarassingSiegeEngine8);
            set
            {
                _HarassingSiegeEngine8 = (HarassingSiegeEngine)Enum.Parse(typeof(HarassingSiegeEngine), value);
            }
        }

        // Index: 123 Hex: 0x1EC
        [RWNames("Unknown123", "RaidCatapultsMax")]
        [RWComment("The maximum of harassing siege engines an AI builds.")]
        public int HarassingSiegeEnginesMax { get; set; }

        // Index: 124 Hex: 0x1F0
        public int Unknown124 { get; set; }

        // Index: 125 Hex: 0x1F4
        [RWComment("The base amount of troops with which this AI attacks")]
        public int AttForceBase
        {
            get
            {
                return _AttForceBase;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _AttForceBase = value;
            }
        }

        // Index: 126 Hex: 0x1F8
        [RWComment("The maximum random amount of additional troops in an attack (this is not the amount by which the troops increase per attack!)")]
        public int AttForceRandom { get; set; }

        // Index: 127 Hex: 0x1FC
        [RWNames("Unknown127", "AttForceRallyDistanceRandom")]
        [RWComment("If the AI has more than this amount of units in the attack force, it will help their allies or siege an enemy if commanded to do so.")]
        public int AttForceSupportAllyThreshold { get; set; }

        // Index: 128 Hex: 0x200
        [RWComment("The %-amount of units of the attack force that the AI will rally before attacking. (0 - 100)")]
        [RWNames("Unknown128")]
        public int AttForceRallyPercentage
        {
            get
            {
                return _AttForceRallyPercentage;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _AttForceRallyPercentage = value;
            }
        }

        // Index: 129 Hex: 0x204
        public int Unknown129 { get; set; }

        // Index: 130 Hex: 0x208
        [RWNames("Unknown130")] //Delay between siege engines being build and the army moving?
        public int Unknown130 { get; set; }

        // Index: 131 Hex: 0x20C
        [RWComment("The delay that the AI waits until the AttUnitPatrol get a new attack-move order.")]
        [RWNames("AttUnitPatrolRecommandDelay")]
        public int AttUnitPatrolRecommandDelay { get; set; }

        // Index: 132 Hex: 0x210
        public int Unknown132 { get; set; }

        // Index: 133 Hex: 0x214
        [ScriptIgnore]
        public SiegeEngine _SiegeEngine1 { get; set; }
        public string SiegeEngine1
        {
            get => Enum.GetName(typeof(SiegeEngine), _SiegeEngine1);
            set
            {
                _SiegeEngine1 = (SiegeEngine)Enum.Parse(typeof(SiegeEngine), value);
            }
        }

        // Index: 134 Hex: 0x218
        [ScriptIgnore]
        public SiegeEngine _SiegeEngine2 { get; set; }
        public string SiegeEngine2
        {
            get => Enum.GetName(typeof(SiegeEngine), _SiegeEngine2);
            set
            {
                _SiegeEngine2 = (SiegeEngine)Enum.Parse(typeof(SiegeEngine), value);
            }
        }


        // Index: 135 Hex: 0x21C
        [ScriptIgnore]
        public SiegeEngine _SiegeEngine3 { get; set; }
        public string SiegeEngine3
        {
            get => Enum.GetName(typeof(SiegeEngine), _SiegeEngine3);
            set
            {
                _SiegeEngine3 = (SiegeEngine)Enum.Parse(typeof(SiegeEngine), value);
            }
        }

        // Index: 136 Hex: 0x220
        [ScriptIgnore]
        public SiegeEngine _SiegeEngine4 { get; set; }
        public string SiegeEngine4
        {
            get => Enum.GetName(typeof(SiegeEngine), _SiegeEngine4);
            set
            {
                _SiegeEngine4= (SiegeEngine) Enum.Parse(typeof(SiegeEngine), value);
            }
        }

        // Index: 137 Hex: 0x224
        [ScriptIgnore]
        public SiegeEngine _SiegeEngine5 { get; set; }
        public string SiegeEngine5
        {
            get => Enum.GetName(typeof(SiegeEngine), _SiegeEngine5);
            set
            {
                _SiegeEngine5 = (SiegeEngine)Enum.Parse(typeof(SiegeEngine), value);
            }
        }

        // Index: 138 Hex: 0x228
        [ScriptIgnore]
        public SiegeEngine _SiegeEngine6 { get; set; }
        public string SiegeEngine6
        {
            get => Enum.GetName(typeof(SiegeEngine), _SiegeEngine6);
            set
            {
                _SiegeEngine6 = (SiegeEngine)Enum.Parse(typeof(SiegeEngine), value);
            }
        }

        // Index: 139 Hex: 0x22C
        [ScriptIgnore]
        public SiegeEngine _SiegeEngine7 { get; set; }
        public string SiegeEngine7
        {
            get => Enum.GetName(typeof(SiegeEngine), _SiegeEngine7);
            set
            {
                _SiegeEngine7 = (SiegeEngine)Enum.Parse(typeof(SiegeEngine), value);
            }
        }

        // Index: 140 Hex: 0x230
        [ScriptIgnore]
        public SiegeEngine _SiegeEngine8 { get; set; }
        public string SiegeEngine8
        {
            get => Enum.GetName(typeof(SiegeEngine), _SiegeEngine8);
            set
            {
                _SiegeEngine8 = (SiegeEngine)Enum.Parse(typeof(SiegeEngine), value);
            }
        }

        // Index: 141 Hex: 0x234
        //Maybe the amount of stones thrown by all catapults until a cow is thrown instead
        [RWNames("Unknown141")]
        [RWComment("The amount of stones needed to be thrown until the AI throws a diseased cow instead (catapults & trebuchets). Value 0 disables cows and -1 makes the AI not throw any boulders, only cows.")]
        public int CowThrowInterval
        {
            get
            {
                return _CowThrowInterval;
            }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentException();
                }
                _CowThrowInterval = value;
            }
        }

        // Index: 142 Hex: 0x238
        public int Unknown142 { get; set; }

        // Index: 143 Hex: 0x23C
        public int AttMaxEngineers { get; set; }

        // Index: 144 Hex: 0x240
        [ScriptIgnore]
        [RWComment("This unit is only recruited if the target enemy has moat and used preferably to fill up enemy moat.")]
        public DiggingUnit _AttDiggingUnit { get; set; }
        public string AttDiggingUnit
        {
            get => Enum.GetName(typeof(DiggingUnit), _AttDiggingUnit);
            set
            {
                _AttDiggingUnit = (DiggingUnit)Enum.Parse(typeof(DiggingUnit), value);
            }
        }

        // Index: 145 Hex: 0x244
        public int AttDiggingUnitMax { get; set; }

        // Index: 146 Hex: 0x248
        [ScriptIgnore]
        [RWNames("Unknown146")]
        //Not without improved attack waves: [RWComment("These units split from the main attack force to destroy enemy buildings. If the enemy walls are nearby those may be attacked.")]
        public Unit _AttUnit2 { get; set; }
        public string AttUnit2
        {
            get => Enum.GetName(typeof(Unit), _AttUnit2);
            set
            {
                _AttUnit2 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 147 Hex: 0x24C
        [RWNames("Unknown147")]
        public int AttUnit2Max { get; set; }

        // Index: 148 Hex: 0x250
        public int AttMaxAssassins { get; set; }

        // Index: 149 Hex: 0x254
        public int AttMaxLaddermen { get; set; }

        // Index: 150 Hex: 0x258
        public int AttMaxTunnelers { get; set; }

        // Index: 151 Hex: 0x25C
        [ScriptIgnore]
        [RWNames("Unknown151", "AttUnitRangedPush")]
        [RWComment("Ranged attack unit that patrols around the enemy castle / keep. Preferably ranged units should be used here.")]
        public Unit _AttUnitPatrol { get; set; }
        public string AttUnitPatrol
        {
            get => Enum.GetName(typeof(Unit), _AttUnitPatrol);
            set
            {
                _AttUnitPatrol = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 152 Hex: 0x260
        [RWNames("Unknown152", "AttUnitRangedPushMax")]
        public int AttUnitPatrolMax { get; set; }

        // Index: 153 Hex: 0x264
        [RWNames("Unknown153", "RangedPushGroupsCount")]
        [RWComment("# of groups the AttUnitPatrol split into. BUGGY! More than 1 group results to only a single group attacking, the others standing idle.")]
        public int AttUnitPatrolGroupsCount
        {
            get
            {
                return _AttUnitPatrolGroupsCount;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _AttUnitPatrolGroupsCount = value;
            }
        }

        // Index: 154 Hex: 0x268
        [ScriptIgnore]
        [RWComment("Attacking unit that holds position and doesn't attack until the walls are breached.")]
        public Unit _AttUnitBackup { get; set; } // ranged?
        public string AttUnitBackup
        {
            get => Enum.GetName(typeof(Unit), _AttUnitBackup);
            set
            {
                _AttUnitBackup = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 155 Hex: 0x26C
        public int AttUnitBackupMax { get; set; }

        // Index: 156 Hex: 0x270
        [RWNames("Unknown156", "RangedBackupGroupsCount")]
        [RWComment("# of groups the AttUnitBackup split into. If shields are present in the army, one will be added to each group (if possible).")]
        public int AttUnitBackupGroupsCount
        {
            get
            {
                return _AttUnitBackupGroupsCount;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _AttUnitBackupGroupsCount = value;
            }
        }

        // Index: 157 Hex: 0x274
        [ScriptIgnore]
        [RWNames("Unknown157", "AttUnit5")]
        [RWComment("Units that engage enemy groups of units outside the castle. Prioritizes larger groups no matter where they are on the map. Otherwise destroys buildings outside the castle.")]
        public Unit _AttUnitEngage { get; set; }
        public string AttUnitEngage
        {
            get => Enum.GetName(typeof(Unit), _AttUnitEngage);
            set
            {
                _AttUnitEngage = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 158 Hex: 0x278
        [RWNames("Unknown158", "AttUnit5Max")]
        public int AttUnitEngageMax { get; set; }

        // Index: 159 Hex: 0x27C
        [ScriptIgnore]
        [RWComment("These units patrol between siege engines in order to protect them.")]
        public Unit _AttUnitSiegeDef { get; set; }
        public string AttUnitSiegeDef
        {
            get => Enum.GetName(typeof(Unit), _AttUnitSiegeDef);
            set
            {
                _AttUnitSiegeDef = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 160 Hex: 0x280
        public int AttUnitSiegeDefMax { get; set; }

        // Index: 161 Hex: 0x284
        [RWNames("AttUnitSiegeDefGroupsCount")]
        public int AttUnitSiegeDefGroupsCount { get; set; }

        // Index: 162 Hex: 0x288
        [ScriptIgnore]
        [RWComment("AttUntiMain1 to AttUnitMain4 is a list of the main strike force the AI recruits for sieges. Priotizes in order of the list, but only recruits units for which they have enough gold. So try to place expensive units higher up.")]
        public Unit _AttUnitMain1 { get; set; }
        public string AttUnitMain1
        {
            get => Enum.GetName(typeof(Unit), _AttUnitMain1);
            set
            {
                _AttUnitMain1 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 163 Hex: 0x28C
        [ScriptIgnore]
        public Unit _AttUnitMain2 { get; set; }
        public string AttUnitMain2
        {
            get => Enum.GetName(typeof(Unit), _AttUnitMain2);
            set
            {
                _AttUnitMain2 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 164 Hex: 0x290
        [ScriptIgnore]
        [RWNames("Unknown164")]
        public Unit _AttUnitMain3 { get; set; }
        public string AttUnitMain3
        {
            get => Enum.GetName(typeof(Unit), _AttUnitMain3);
            set
            {
                _AttUnitMain3 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 165 Hex: 0x294
        [ScriptIgnore]
        [RWNames("Unknown165")]
        public Unit _AttUnitMain4 { get; set; }
        public string AttUnitMain4
        {
            get => Enum.GetName(typeof(Unit), _AttUnitMain4);
            set
            {
                _AttUnitMain4 = (Unit)Enum.Parse(typeof(Unit), value);
            }
        }

        // Index: 166 Hex: 0x298
        [RWComment("This does nothing")]
        public int AttMaxDefault { get; set; }

        // Index: 167 Hex: 0x29C
        [RWComment("# of groups all the AttUnitMain split into. Maximum is 3")]
        [RWNames("Unknown167")]
        public int AttMainGroupsCount
        {
            get
            {
                return _AttMainGroupsCount;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                _AttMainGroupsCount = value;
            }
        }

        // Index: 168 Hex: 0x2A0
        [ScriptIgnore]
        public TargetingType _TargetChoice { get; set; }
        public string TargetChoice
        {
            get => Enum.GetName(typeof(TargetingType), _TargetChoice);
            set
            {
                _TargetChoice = (TargetingType) Enum.Parse(typeof(TargetingType), value);
            }
        }
    }
}
