using System;
using System.Collections.Generic;

namespace MayhemCore
{
    internal class ModuleTypeComparer : IComparer<ModuleType>
    {
        public int Compare(ModuleType x, ModuleType y)
        {
            EnsureNotNull(x);
            EnsureNotNull(y);
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }

        private static void EnsureNotNull(ModuleType type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
        }
    }
}
