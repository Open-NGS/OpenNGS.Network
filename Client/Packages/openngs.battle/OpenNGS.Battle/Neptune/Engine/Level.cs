using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Neptune.Datas;

namespace Neptune
{
    public class Level
    {
        int LevelID;
        int RoundID;

        public Level(int levelID, int roundID)
        {
            this.LevelID = levelID;
            this.RoundID = roundID;
        }

        /// <summary>
        /// Campaign Config
        /// </summary>
        public void Config()
        {
            LevelData level = NeptuneBattle.Instance.DataProvider.GetLevelData(this.LevelID);
            if (level != null && level.LevelScript != null && level.LevelScript.Count >= this.RoundID)
            {
                string text = level.LevelScript[this.RoundID - 1];
                string[] script = text.Split(":".ToCharArray());
                switch (script[0])
                {
                    case "AddMP":
                        this.AddMP(script[1], int.Parse(script[2]));
                        break;
                    case "SetMP":
                        this.SetMP(script[1], int.Parse(script[2]));
                        break;
                    case "AddRage":
                        this.AddRage(script[1], int.Parse(script[2]));
                        break;
                    case "SetRage":
                        this.SetRage(script[1], int.Parse(script[2]));
                        break;
                    case "AddPoint":
                        this.AddPoint(script[1], int.Parse(script[2]));
                        break;
                    case "SetPoint":
                        this.SetPoint(script[1], int.Parse(script[2]));
                        break;
                    case "NoManualSkill":
                        this.NoManualSkill(script[1]);
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
                List<Actor> roleList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideA];
                foreach (Actor role in roleList)
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
                List<Actor> roleList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideA];
                foreach (Actor role in roleList)
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
                List<Actor> roleList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideA];
                foreach (Actor role in roleList)
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
                Actor monsterBoss = null;
                List<Actor> enemyList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideB];
                foreach (Actor m in enemyList)
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
                Actor monsterBoss = null;
                List<Actor> enemyList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideB];
                foreach (Actor m in enemyList)
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
                Actor monsterBoss = null;
                List<Actor> enemyList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideB];
                foreach (Actor m in enemyList)
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
                Actor monsterBoss = null;
                List<Actor> enemyList = NeptuneBattle.Instance.AliveRoles[(int)RoleSide.SideB];
                foreach (Actor m in enemyList)
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