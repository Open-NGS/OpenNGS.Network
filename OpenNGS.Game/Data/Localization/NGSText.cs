using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNGS.Localize;

namespace OpenNGS
{
    public partial class NGSText
    {
        public static implicit operator string(NGSText text)
        {
            return LocalizationSystem.Instance.GetText(text.Key);
        }

        public override string ToString()
        {
            if (Value != null)
            {
                return Value.ToString();
            }

            return "null";
        }

        public string Value => this;
    }
}
