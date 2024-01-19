using OpenNGS.Item.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CharacterInfo = OpenNGS.Character.Common.CharacterInfo;
namespace OpenNGS.Systems
{

    public class PlayerContext : StorageContext
    {
        public StorageSet<Character> Characters { get; set; }
        public StorageSet<Inventory> Inventorys { get; set; }
    }


    public class Inventory
    {
        public List<OpenNGS.Item.Common.ItemData>  Items;
    }

    public class Character
    {
        public List<Inventory> Inventorys;
        public CharacterInfo Info;
        public GameAttributeSet Attribute;

        public Character(CharacterInfo info)
        {
            Info = info;
            ResetAttribute();
        }

        public void RefreshCharacter(CharacterInfo info)
        {
            Info = info;
            ResetAttribute();
        }

        private void ResetAttribute()
        {
            Attribute = new GameAttributeSet(Info.attributes);
        }
    }

    public class Player
    {
        public List<Inventory> Inventorys;


        public List<Character> Characters;


    }
}
