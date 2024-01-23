using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Systems
{
    public interface IItemSystem
    {
        public bool IsItemEnough(uint nItemID, uint nCounts);
        public bool AddItemsByID(uint nItemID, uint nCounts);
        public bool RemoveItemsByID(uint nItemID, uint nCounts);
        public uint GetGuidByItemID(uint nItemID);
        public bool RemoveItemsByGuid(uint nGuid, uint nCounts);
    }

}