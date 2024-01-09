using Item.RPC;
using OpenNGS.Item.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.API
{

    /// <summary>
    /// 
    /// </summary>
    
    internal class ItemAPI
    {
        public async Task<List<ItemData>> GetItemss()
        {
            List<ItemData> data = new List<ItemData>();

            return await Task<List<ItemData>>.FromResult(data);
        }
    }
}
