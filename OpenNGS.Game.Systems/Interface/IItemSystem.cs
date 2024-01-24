using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Systems
{
    public interface IItemSystem
    {
        public bool IsEnoughByItemID(uint nItemID, uint nCounts);
        public bool IsEnoughByGuid(uint nGuid, uint nCounts);
        public bool AddItemsByID(uint nItemID, uint nCounts);
        public bool RemoveItemsByID(uint nItemID, uint nCounts);
        public bool RemoveItemsByGuid(uint nGuid, uint nCounts);
        public uint GetGuidByItemID(uint nItemID);
    }

}