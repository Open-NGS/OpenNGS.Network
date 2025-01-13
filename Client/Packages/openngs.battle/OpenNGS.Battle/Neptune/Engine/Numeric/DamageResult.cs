using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DamageResult
{
    public bool IsCritical;
    public bool IsImmunization;
    public bool IsFatal;
    /// <summary>
    /// 最终对目标的有效伤害
    /// </summary>
    public int FinalDamage;

    public void Reset() {
        IsCritical = false;
        IsImmunization = false;
        IsFatal = false;
        FinalDamage = 0;
    }
}