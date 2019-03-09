using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCP.AICharacters
{
    /// <summary>
    /// Can be used to assign enums for certein AIPFields
    /// </summary>
    public static class AIPFieldType
    {
        static Dictionary<AIPField, Type> Enums = new Dictionary<AIPField, Type>()
        {
            { AIPField.DefUnit1, typeof(UnitType) },
            { AIPField.DefUnit2, typeof(UnitType) },
            { AIPField.DefUnit3, typeof(UnitType) },
            { AIPField.DefUnit4, typeof(UnitType) },
            { AIPField.DefUnit5, typeof(UnitType) },
            { AIPField.DefUnit6, typeof(UnitType) },
            { AIPField.DefUnit7, typeof(UnitType) },
            { AIPField.DefUnit8, typeof(UnitType) },
        };

        /// <summary>
        /// Returns an enum type or null for the given AIPField
        /// </summary>
        public static bool TryGet(AIPField field, out Type type)
        {
            return Enums.TryGetValue(field, out type);
        }
    }
}
