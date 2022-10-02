using System;
using System.Collections.Generic;
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
            BinaryEdit resourceSellEdit = new BinaryEdit("bal_resource_sell");
            for (int i = 0; i < BalanceEnums.resourceNames.Count; i++)
            {
                List<BinElement> currentResourceEdit = new List<BinElement>();
                BalanceConfig.ResourceConfig resourceConfig;
                if (resources.TryGetValue(BalanceEnums.unitNames[i], out resourceConfig))
                {
                    if (resourceConfig.sell.HasValue)
                    {
                        ValidateResourceValue("sell cost", resourceConfig.sell.Value, i, Int16.MaxValue);
                        currentResourceEdit.Add(new BinInt32(resourceConfig.sell.Value));
                    }
                }

                // if current entry is not specified then add skip
                if (currentResourceEdit.Count == 0)
                {
                    resourceSellEdit.Add(new BinSkip(4));
                }

                // Add to the combined list of edits
                foreach (BinElement element in currentResourceEdit)
                {
                    currentResourceEdit.Add(element);
                }
            }
            return resourceSellEdit;
        }

        private static ChangeEdit GetResourceBuyEdit(Dictionary<string, BalanceConfig.ResourceConfig> resources)
        {
            BinaryEdit resourceBuyEdit = new BinaryEdit("bal_resource_buy");
            for (int i = 0; i < BalanceEnums.resourceNames.Count; i++)
            {
                List<BinElement> currentResourceEdit = new List<BinElement>();
                BalanceConfig.ResourceConfig resourceConfig;
                if (resources.TryGetValue(BalanceEnums.unitNames[i], out resourceConfig))
                {
                    if (resourceConfig.sell.HasValue)
                    {
                        ValidateResourceValue("buy cost", resourceConfig.sell.Value, i, Int16.MaxValue);
                        currentResourceEdit.Add(new BinInt32(resourceConfig.sell.Value));
                    }
                }

                // if current entry is not specified then add skip
                if (currentResourceEdit.Count == 0)
                {
                    resourceBuyEdit.Add(new BinSkip(4));
                }

                // Add to the combined list of edits
                foreach (BinElement element in currentResourceEdit)
                {
                    currentResourceEdit.Add(element);
                }
            }
            return resourceBuyEdit;
        }

        private static ChangeEdit GetUnitMeleeDmgEdit(Dictionary<string, BalanceConfig.UnitConfig> units)
        {
            BinaryEdit unitMeleeDmgEdit = new BinaryEdit("bal_unit_meleedmg");
            for (int i = 0; i < BalanceEnums.unitNames.Count; i++)
            {
                List<BinElement> currentAttackerEdit = new List<BinElement>();
                BalanceConfig.UnitConfig unitConfig;
                if (units.TryGetValue(BalanceEnums.unitNames[i], out unitConfig))
                {
                    if (unitConfig.meleeDamageVs != null)
                    {
                        for (int j = 0; j < BalanceEnums.unitNames.Count; j++)
                        {
                            List<BinElement> currentDefenderUnitEdit = new List<BinElement>();
                            int dmgValue;
                            if (unitConfig.meleeDamageVs.TryGetValue(BalanceEnums.unitNames[j], out dmgValue))
                            {
                                ValidateUnitValue("melee damage vs " + BalanceEnums.unitNames[j], dmgValue, i, Int32.MaxValue);
                                currentDefenderUnitEdit.Add(new BinInt32(dmgValue));
                            }

                            // if current entry is not specified then add skip
                            if (currentDefenderUnitEdit.Count == 0)
                            {
                                currentAttackerEdit.Add(new BinSkip(4 * BalanceEnums.unitNames.Count));
                            }

                            // Add to the combined list of edits
                            foreach (BinElement element in currentDefenderUnitEdit)
                            {
                                currentAttackerEdit.Add(element);
                            }
                        }
                    }
                }

                // Add to the combined list of edits
                foreach (BinElement element in currentAttackerEdit)
                {
                    unitMeleeDmgEdit.Add(element);
                }
            }
            return unitMeleeDmgEdit;
        }

        private static ChangeEdit GetUnitStoneDmgEdit(Dictionary<string, BalanceConfig.UnitConfig> units)
        {
            BinaryEdit unitStoneDmgEdit = new BinaryEdit("bal_unit_stonedmg");
            for (int i = 0; i < BalanceEnums.unitNames.Count; i++)
            {
                List<BinElement> currentUnitEdit = new List<BinElement>();
                BalanceConfig.UnitConfig unitConfig;
                if (units.TryGetValue(BalanceEnums.unitNames[i], out unitConfig))
                {
                    if (unitConfig.stoneDamage.HasValue)
                    {
                        ValidateUnitValue("xbow damage", unitConfig.stoneDamage.Value, i, Int32.MaxValue);
                        currentUnitEdit.Add(new BinInt32(unitConfig.stoneDamage.Value));
                    }
                }

                // if current entry is not specified then add skip
                if (currentUnitEdit.Count == 0)
                {
                    unitStoneDmgEdit.Add(new BinSkip(4));
                }

                // Add to the combined list of edits
                foreach (BinElement element in currentUnitEdit)
                {
                    unitStoneDmgEdit.Add(element);
                }
            }
            return unitStoneDmgEdit;
        }

        private static ChangeEdit GetUnitXbowDmgEdit(Dictionary<string, BalanceConfig.UnitConfig> units)
        {
            BinaryEdit unitXbowDmgEdit = new BinaryEdit("bal_unit_xbowdmg");
            for (int i = 0; i < BalanceEnums.unitNames.Count; i++)
            {
                List<BinElement> currentUnitEdit = new List<BinElement>();
                BalanceConfig.UnitConfig unitConfig;
                if (units.TryGetValue(BalanceEnums.unitNames[i], out unitConfig))
                {
                    if (unitConfig.xbowDamage.HasValue)
                    {
                        ValidateUnitValue("xbow damage", unitConfig.xbowDamage.Value, i, Int32.MaxValue);
                        currentUnitEdit.Add(new BinInt32(unitConfig.xbowDamage.Value));
                    }
                }

                // if current entry is not specified then add skip
                if (currentUnitEdit.Count == 0)
                {
                    unitXbowDmgEdit.Add(new BinSkip(4));
                }

                // Add to the combined list of edits
                foreach (BinElement element in currentUnitEdit)
                {
                    unitXbowDmgEdit.Add(element);
                }
            }
            return unitXbowDmgEdit;
        }

        private static ChangeEdit GetUnitArrowDmgEdit(Dictionary<string, BalanceConfig.UnitConfig> units)
        {
            BinaryEdit unitArrowDmgEdit = new BinaryEdit("bal_unit_arrowdmg");
            for (int i = 0; i < BalanceEnums.unitNames.Count; i++)
            {
                List<BinElement> currentUnitEdit = new List<BinElement>();
                BalanceConfig.UnitConfig unitConfig;
                if (units.TryGetValue(BalanceEnums.unitNames[i], out unitConfig))
                {
                    if (unitConfig.arrowDamage.HasValue)
                    {
                        ValidateUnitValue("arrow damage", unitConfig.arrowDamage.Value, i, Int32.MaxValue);
                        currentUnitEdit.Add(new BinInt32(unitConfig.arrowDamage.Value));
                    }
                }

                // if current entry is not specified then add skip
                if (currentUnitEdit.Count == 0)
                {
                    unitArrowDmgEdit.Add(new BinSkip(4));
                }

                // Add to the combined list of edits
                foreach (BinElement element in currentUnitEdit)
                {
                    unitArrowDmgEdit.Add(element);
                }
            }
            return unitArrowDmgEdit;
        }

        private static ChangeEdit GetUnitHealthEdit(Dictionary<string, BalanceConfig.UnitConfig> units)
        {
            BinaryEdit unitHealthEdit = new BinaryEdit("bal_unit_health");
            for (int i = 0; i < BalanceEnums.unitNames.Count; i++)
            {
                List<BinElement> currentUnitEdit = new List<BinElement>();
                BalanceConfig.UnitConfig unitConfig;
                if (units.TryGetValue(BalanceEnums.unitNames[i], out unitConfig))
                {
                    if (unitConfig.health.HasValue)
                    {
                        ValidateUnitValue("health", unitConfig.health.Value, i, Int32.MaxValue);
                        currentUnitEdit.Add(new BinInt32(unitConfig.health.Value));
                    }
                }

                // if current entry is not specified then add skip
                if (currentUnitEdit.Count == 0)
                {
                    unitHealthEdit.Add(new BinSkip(4));
                }

                // Add to the combined list of edits
                foreach (BinElement element in currentUnitEdit)
                {
                    unitHealthEdit.Add(element);
                }
            }
            return unitHealthEdit;
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
                        ValidateBuildingValue("health", buildingConfig.health.Value, i, Int32.MaxValue);
                        currentBuildingEdit.Add(new BinInt32(buildingConfig.health.Value));
                    }
                }

                // if current entry is not specified then add skip
                if (currentBuildingEdit.Count == 0)
                {
                    buildingHealthEdit.Add(new BinSkip(4));
                }

                // Add to the combined list of edits
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
                                ValidateBuildingValue("resource", resourceCost, i, Int16.MaxValue);
                                currentBuildingEdit.Add(new BinInt32(resourceCost));
                            }
                        }
                        else
                        {
                            throw new Exception("Invalid building cost for " + BalanceEnums.buildingNames[i]);
                        }
                    }
                }

                // if current entry is not specified then add skip
                if (currentBuildingEdit.Count == 0)
                {
                    for (int j = 0; j < buildingCostLength; j++)
                    {
                        currentBuildingEdit.Add(new BinSkip(4));
                    }
                }

                // Add to the combined list of edits
                foreach (BinElement element in currentBuildingEdit)
                {
                    buildingCostEdit.Add(element);
                }
            }
            return buildingCostEdit;
        }

        private static void ValidateBuildingValue(string valueType, int value, int itemIndex, int maxValue)
        {
            if (value < 0 || value > maxValue)
            {
                string errorMessage = GetBuildingErrorMessage(valueType, value, itemIndex);
                throw new Exception(errorMessage);
            }
        }

        private static void ValidateResourceValue(string valueType, int value, int itemIndex, int maxValue)
        {
            if (value < 0 || value > maxValue)
            {
                string errorMessage = GetResourceErrorMessage(valueType, value, itemIndex);
                throw new Exception(errorMessage);
            }
        }

        private static void ValidateUnitValue(string valueType, int value, int itemIndex, int maxValue)
        {
            if (value < 0 || value > maxValue)
            {
                string errorMessage = GetUnitErrorMessage(valueType, value, itemIndex);
                throw new Exception(errorMessage);
            }
        }

        private static string GetBuildingErrorMessage(string valueType, int invalidValue, int buildingIndex)
        {
            return "Invalid " + valueType + " of " + invalidValue.ToString() + " for building " + BalanceEnums.buildingNames[buildingIndex];
        }

        private static string GetResourceErrorMessage(string valueType, int invalidValue, int buildingIndex)
        {
            return "Invalid " + valueType + " of " + invalidValue.ToString() + " for resource " + BalanceEnums.resourceNames[buildingIndex];
        }

        private static string GetUnitErrorMessage(string valueType, int invalidValue, int buildingIndex)
        {
            return "Invalid " + valueType + " of " + invalidValue.ToString() + " for unit " + BalanceEnums.unitNames[buildingIndex];
        }
    }
}
