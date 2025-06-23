using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNGS.Localize;

namespace OpenNGS.Core
{
    public partial class NGSText
    {
        public static implicit operator string(NGSText text)
        {
            return LocalizationSystem.Instance.GetText(text.Key);
        }
        public NGSText(string strKey)
        {
            Key = strKey;
            OnConstructor();
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
        public static NGSText Text(string strKey, string value)
        {
            NGSText _ngText = new NGSText(strKey);
            return _ngText;
        }
    }
}
