using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameData{
        
    public class LevelSettingData
    {
        public int ID { get; set; }
        public string LevelName { get; set; }
        public int InitGold { get; set; }
        public int InitLevel { get; set; }
        public float GlodexpRiseStartTime { get; set; }
        public float GlodexpRiseInterval { get; set; }
        public int GoldAddRate { get; set; }
        public int ExpAddRate { get; set; }
        public bool HasSpringTower { get; set; }
        public bool HasSuperTroop { get; set; }
        public bool ShopOnlyInsideSpring { get; set; }
        public List<int> HasSpringBuff { get; set; }
        public List<int> AvoliableSummons { get; set; }
        public List<int> DefaultSummons { get; set; }
        public int DefaultSummonLevel { get; set; }
        public string MiniMapRes { get; set; }
        public int smallDragon { get; set; }
        public int bigDragon { get; set; }
        public List<int> buffMonsterID { get; set; }
        public List<int> Resurgence { get; set; }
        public float InitialResurrection { get; set; }
        public float UltimatelyResurrection { get; set; }
        public float IntervalResurrection { get; set; }
        public int InitialSurrender { get; set; }
        public int NextSurrender { get; set; }
    }   
}   
