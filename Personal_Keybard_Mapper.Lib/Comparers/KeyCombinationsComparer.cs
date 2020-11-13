using System.Collections;
using System.Collections.Generic;
using Personal_Keyboard_Mapper.Lib.Interfaces;

namespace Personal_Keyboard_Mapper.Lib.Comparers
{
    public class KeyCombinationsComparer : IEqualityComparer<IKeyCombination>
    {
        public bool Equals(IKeyCombination x, IKeyCombination y)
        {
            return (x != null) && (y != null) && (x.Equals(y));
        }

        public int GetHashCode(IKeyCombination obj)
        {
            var prime = 23;
            var hash = obj.GetHashCode() * prime;
            return hash;
        }
    }
}