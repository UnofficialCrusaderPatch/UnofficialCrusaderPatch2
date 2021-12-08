using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UCP.Patching;

namespace UCP.Balance
{
    class BalanceHelper
    {
        public static ChangeEdit[] GetBinaryEdits(BalanceConfig balanceConfig)
        {
            List<ChangeEdit> editList = new List<ChangeEdit>();

            if (balanceConfig.buildings != null)
            {
                ChangeEdit buildingCostEdit = GetBuildingCostEdit(balanceConfig.buildings);
                ChangeEdit buildingHealthEdit = GetBuildingHealthEdit(balanceConfig.buildings);

                editList.Add(buildingCostEdit);
                editList.Add(buildingHealthEdit);
            }

            if (balanceConfig.resources != null)
            {
                ChangeEdit resourceBuyEdit = GetResourceBuyEdit(balanceConfig.resources);
                ChangeEdit resourceSellEdit = GetResourceSellEdit(balanceConfig.resources);

                editList.Add(resourceBuyEdit);
                editList.Add(resourceSellEdit);
            }


            if (balanceConfig.units != null)
            {
                ChangeEdit unitHealthEdit = GetUnitHealthEdit(balanceConfig.units);
                ChangeEdit unitArrowDmgEdit = GetUnitArrowDmgEdit(balanceConfig.units);
                ChangeEdit unitXbowDmgEdit = GetUnitXbowDmgEdit(balanceConfig.units);
                ChangeEdit unitStoneDmgEdit = GetUnitStoneDmgEdit(balanceConfig.units);
                ChangeEdit unitMeleeDmgEdit = GetUnitMeleeDmgEdit(balanceConfig.units);

                editList.Add(unitHealthEdit);
                editList.Add(unitArrowDmgEdit);
                editList.Add(unitMeleeDmgEdit);
                editList.Add(unitStoneDmgEdit);
                editList.Add(unitXbowDmgEdit);
            }
            return editList.ToArray();
        }

        private static ChangeEdit GetResourceSellEdit(Dictionary<string, BalanceConfig.ResourceConfig> resources)
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetResourceBuyEdit(Dictionary<string, BalanceConfig.ResourceConfig> resources)
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetUnitMeleeDmgEdit(Dictionary<string, BalanceConfig.UnitConfig> units)
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetUnitStoneDmgEdit(Dictionary<string, BalanceConfig.UnitConfig> units)
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetUnitXbowDmgEdit(Dictionary<string, BalanceConfig.UnitConfig> units)
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetUnitArrowDmgEdit(Dictionary<string, BalanceConfig.UnitConfig> units)
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetUnitHealthEdit(Dictionary<string, BalanceConfig.UnitConfig> units)
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetBuildingHealthEdit(Dictionary<string, BalanceConfig.BuildingConfig> buildings)
        {
            BinaryEdit buildingHealthEdit = new BinaryEdit("bal_building_health");
            for (int i = 0; i < BalanceEnums.buildingNames.Count; i++)
            {
                List<BinElement> currentBuildingEdit = new List<BinElement>();
                BalanceConfig.BuildingConfig buildingConfig;
                if (buildings.TryGetValue(BalanceEnums.buildingNames[i], out buildingConfig))
                {
                    if (buildingConfig.health.HasValue)
                    {
                        if (buildingConfig.health < 0 || buildingConfig.health > Int16.MaxValue)
                        {
                            throw new Exception("Invalid health of " + buildingConfig.health.ToString() + " for building " + BalanceEnums.buildingNames[i]);
                        }
                        currentBuildingEdit.Add(new BinInt32(buildingConfig.health.Value));
                    }
                }

                // if current building or current building health entry is not specified then add skip
                if (currentBuildingEdit.Count == 0)
                {
                    buildingHealthEdit.Add(new BinSkip(4));
                }

                // Add elements of currentBuildingEdit to the combined list of edits for building health binaryEdit
                foreach (BinElement element in currentBuildingEdit)
                {
                    buildingHealthEdit.Add(element);
                }
            }
            return buildingHealthEdit;
        }

        private static ChangeEdit GetBuildingCostEdit(Dictionary<string, BalanceConfig.BuildingConfig> buildings)
        {
            const int buildingCostLength = 5;
            BinaryEdit buildingCostEdit = new BinaryEdit("bal_building_cost");
            for (int i = 0; i < BalanceEnums.buildingNames.Count; i++)
            {
                List<BinElement> currentBuildingEdit = new List<BinElement>();
                BalanceConfig.BuildingConfig buildingConfig;
                if (buildings.TryGetValue(BalanceEnums.buildingNames[i], out buildingConfig)){
                    if (buildingConfig.cost != null)
                    {
                        if (buildingConfig.cost.Length == buildingCostLength)
                        {
                            foreach (int resourceCost in buildingConfig.cost)
                            {
                                if (resourceCost < 0 || resourceCost > Int16.MaxValue)
                                {
                                    throw new Exception("Invalid resource cost of " + resourceCost.ToString() + " for building " + BalanceEnums.buildingNames[i]);
                                }
                                currentBuildingEdit.Add(new BinInt32(resourceCost));
                            }
                        }
                        else
                        {
                            throw new Exception("Invalid building cost for " + BalanceEnums.buildingNames[i]);
                        }
                    }
                }

                // if current building or current building cost entry is not specified then add skip
                if (currentBuildingEdit.Count == 0)
                {
                    for (int j = 0; j < buildingCostLength; j++)
                    {
                        currentBuildingEdit.Add(new BinSkip(4));
                    }
                }

                // Add elements of currentBuildingEdit to the combined list of edits for building health binaryEdit
                foreach (BinElement element in currentBuildingEdit)
                {
                    buildingCostEdit.Add(element);
                }
            }
            return buildingCostEdit;
        }
    }
}
