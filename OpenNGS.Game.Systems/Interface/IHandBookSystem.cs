using OpenNGS.HandBook.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Systems
{
    public interface IHandBookSystem
    {
        public OpenNGS.HandBook.Common.HANDBOOK_STATUS GetHandBookStatus(uint nHandBookID);
        public void UpdateHandBookIfNeed();
        public Dictionary<uint, HandBookInfo> GetHandBookData();
        public void SetHandBookIndex(uint groupID);
    }
}
