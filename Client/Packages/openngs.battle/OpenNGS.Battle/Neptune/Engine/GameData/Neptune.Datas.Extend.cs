using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptune.Datas
{
    public static class RoleAttributeEnumExtend
    {
        public static string Name(this RoleAttribute value)
        {
            throw new System.NotImplementedException();
            //if (value >= RoleAttribute.MAX && value < RoleAttribute.RATIOMAX)
            //    return ((RoleAttribute)(value - RoleAttribute.RATIOBASE)).ToString() + RoleAttributes.PF_A;
            //return value.ToString();
        }
    }


    public partial class AbilityData
    {

        public bool IsControlAbility
        {
            get
            {
                if (this.ControlEffects == null || this.ControlEffects.Length <= 0)
                {
                    return false;
                }

                foreach (var effect in this.ControlEffects)
                {
                    if (NeptuneConst.ConflictingAbilities.ContainsKey((int)effect))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool IsUnControllable
        {
            get
            {
                if (this.ControlEffects == null || this.ControlEffects.Length <= 0)
                {
                    return false;
                }
                foreach (var effect in this.ControlEffects)
                {
                    if ((ControlEffect)effect == ControlEffect.Unaffected)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }


}
