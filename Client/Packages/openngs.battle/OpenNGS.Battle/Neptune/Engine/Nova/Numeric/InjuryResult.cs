using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neptune
{
    public class InjuryResult
    {
        public bool IsCritical;
        public bool IsImmunization;
        public bool IsFatal;
        /// <summary>
        /// 最终对目标的有效伤害
        /// </summary>
        public int FinalInjury;

        public void Reset()
        {
            IsCritical = false;
            IsImmunization = false;
            IsFatal = false;
            FinalInjury = 0;
        }
    }
}