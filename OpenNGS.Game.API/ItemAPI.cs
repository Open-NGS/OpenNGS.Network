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
        public async Task<List<OpenNGS.Item.Common.ItemData>> GetItemss()
        {
            List<OpenNGS.Item.Common.ItemData> data = new List<OpenNGS.Item.Common.ItemData>();

            return await Task<List<OpenNGS.Item.Common.ItemData>>.FromResult(data);
        }
    }
}
