using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Neptune.GameData;

namespace Neptune
{
    public class BattleLevel
    {
        int LevelID;
        int RoundID;

        public BattleLevel(int levelID, int roundID)
        {
            LevelID = levelID;
            RoundID = roundID;
        }

        /// <summary>
        /// Campaign Config
        /// </summary>
        public void Config()
        {
            BattleLevelData level = NeptuneBattle.Instance.DataProvider.GetLevelData(LevelID);
            if (level != null && level.LevelScript != null && level.LevelScript.Count >= RoundID)
            {
                string text = level.LevelScript[RoundID - 1];
                string[] script = text.Split(":".ToCharArray());
                switch (script[0])
                {
                    case "AddMP":
                        AddMP(script[1], int.Parse(script[2]));
                        break;
                    case "SetMP":
                        SetMP(script[1], int.Parse(script[2]));
                        break;
                    case "AddRage":
                        AddRage(script[1], int.Parse(script[2]));
                        break;
                    case "SetRage":
                        SetRage(script[1], int.Parse(script[2]));
                        break;
                    case "AddPoint":
                        AddPoint(script[1], int.Parse(script[2]));
                        break;
                    case "SetPoint":
                        SetPoint(script[1], int.Parse(script[2]));
                        break;
                    case "NoManualSkill":
                        NoManualSkill(script[1]);
                        break;
                }
            }
        }

        public void AddMP(string target, int mp)
        {
            if (target != "BOSS")
            {
                //            foreach (var v in EngineDataManager.Instance.Roles.Value)
                //            {
                //                if (EngineDataManager.Instance.Roles.Value[v.Key].Name == target)
                //                {
                List<BattleActor> roleList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideA];
                foreach (BattleActor role in roleList)
                {
                    if (role.Data.Name == target)
                    {
                        role.SetMP(role.MP + mp);
                        break;
                    }
                }
                //                }
                //            }
            }
        }

        public void AddRage(string target, int rage)
        {
            if (target != "BOSS")
            {
                //            foreach (var v in EngineDataManager.Instance.Roles.Value)
                //            {
                //                if (EngineDataManager.Instance.Roles.Value[v.Key].Name == target)
                //                {
                List<BattleActor> roleList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideA];
                foreach (BattleActor role in roleList)
                {
                    if (role.Data.Name == target)
                    {
                        role.SetRage(role.Rage + rage);
                        break;
                    }
                }
            }
        }

        public void AddPoint(string target, int point)
        {
            if (target != "BOSS")
            {
                //            foreach (var v in EngineDataManager.Instance.Roles.Value)
                //            {
                //                if (EngineDataManager.Instance.Roles.Value[v.Key].Name == target)
                //                {
                List<BattleActor> roleList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideA];
                foreach (BattleActor role in roleList)
                {
                    if (role.Data.Name == target)
                    {
                        role.SetPoint(role.Point + point);
                        break;
                    }
                }
                //                }
                //            }
            }
        }


        public void SetMP(string target, int mp)
        {
            if (target == "BOSS")
            {
                BattleActor monsterBoss = null;
                List<BattleActor> enemyList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideB];
                foreach (BattleActor m in enemyList)
                {
                    if (m.IsBoss)
                    {
                        monsterBoss = m;
                        break;
                    }
                }
                if (monsterBoss != null)
                {
                    monsterBoss.SetMP(mp);
                }
            }
        }

        public void SetRage(string target, int rage)
        {
            if (target == "BOSS")
            {
                BattleActor monsterBoss = null;
                List<BattleActor> enemyList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideB];
                foreach (BattleActor m in enemyList)
                {
                    if (m.IsBoss)
                    {
                        monsterBoss = m;
                        break;
                    }
                }
                if (monsterBoss != null)
                {
                    monsterBoss.SetRage(rage);
                }
            }
        }

        public void SetPoint(string target, int point)
        {
            if (target == "BOSS")
            {
                BattleActor monsterBoss = null;
                List<BattleActor> enemyList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideB];
                foreach (BattleActor m in enemyList)
                {
                    if (m.IsBoss)
                    {
                        monsterBoss = m;
                        break;
                    }
                }
                if (monsterBoss != null)
                {
                    monsterBoss.SetPoint(point);
                }
            }
        }

        public void NoManualSkill(string target)
        {
            if (target == "BOSS")
            {
                BattleActor monsterBoss = null;
                List<BattleActor> enemyList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideB];
                foreach (BattleActor m in enemyList)
                {
                    if (m.IsBoss)
                    {
                        monsterBoss = m;
                        break;
                    }
                }
                if (monsterBoss != null)
                {
                    //monsterBoss.IsTransformByManual = false;
                }
            }
        }
    }
}