using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UCP.Patching;

namespace UCP.Balance
{
    class BalanceHelper
    {
        public static ChangeEdit[] GetBinaryEdits(Dictionary<String, Dictionary<String, Object>> balanceConfig)
        {
            List<ChangeEdit> editList = new List<ChangeEdit>();

            ChangeEdit buildingCostEdit = GetBuildingCostEdit();
            ChangeEdit buildingHealthEdit = GetBuildingHealthEdit();
            ChangeEdit unitHealthEdit = GetUnitHealthEdit();
            ChangeEdit unitArrowDmgEdit = GetUnitArrowDmgEdit();
            ChangeEdit unitXbowDmgEdit = GetUnitXbowDmgEdit();
            ChangeEdit unitStoneDmgEdit = GetUnitStoneDmgEdit();
            ChangeEdit unitMeleeDmgEdit = GetUnitMeleeDmgEdit();

            if (buildingCostEdit != null)
            {
                editList.Add(buildingCostEdit);
            }

            if (buildingHealthEdit != null)
            {
                editList.Add(buildingHealthEdit);
            }

            if (unitHealthEdit != null)
            {
                editList.Add(unitHealthEdit);
            }

            if (unitArrowDmgEdit != null)
            {
                editList.Add(unitArrowDmgEdit);
            }

            if (unitMeleeDmgEdit != null)
            {
                editList.Add(unitMeleeDmgEdit);
            }

            if (unitStoneDmgEdit != null)
            {
                editList.Add(unitStoneDmgEdit);
            }

            if (unitXbowDmgEdit != null)
            {
                editList.Add(unitXbowDmgEdit);
            }

            return editList.ToArray();
        }

        private static ChangeEdit GetUnitMeleeDmgEdit()
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetUnitStoneDmgEdit()
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetUnitXbowDmgEdit()
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetUnitArrowDmgEdit()
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetUnitHealthEdit()
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetBuildingHealthEdit()
        {
            throw new NotImplementedException();
        }

        private static ChangeEdit GetBuildingCostEdit()
        {
            throw new NotImplementedException();
        }
    }
}
