using OpenNGS.Item.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Systems
{

    public class PlayerContext : StorageContext
    {
        public StorageSet<Character> Characters { get; set; }
        public StorageSet<Inventory> Inventorys { get; set; }
    }


    public class Inventory
    {
        public List<ItemData>  Items;
    }

    public class Character
    {
        public List<Inventory> Inventorys;
    }

    public class Player
    {
        public List<Inventory> Inventorys;


        public List<Character> Characters;


    }
}
