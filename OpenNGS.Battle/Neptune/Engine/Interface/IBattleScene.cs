using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine.Events;
using Neptune.Datas;


namespace Neptune
{
    /// <summary>
    /// Battle scene interface
    /// </summary>
    public interface IBattleScene
    {
        /*
         *******************************
         *  Battle Base Interface
         *******************************
         */
        /// <summary>
        /// Get current battle field forward direction
        /// </summary>
        Vector3 ForwardDirection { get; }

        /// <summary>
        /// Get current battle field forward angles
        /// </summary>
        Vector3 ForwardAngles { get; }
        /// <summary>
        /// Get battle points for units spawn & initialization
        /// </summary>
        Vector3[] BattlePoints { get; }

        bool BattleJumpMode { get; }

        /// <summary>
        /// Add a new joint to current battle
        /// </summary>
        /// <param name="joint">joint</param>
        void AddJoint(IAgent joint);

        /// <summary>
        /// Process battle result and display UI
        /// </summary>
        /// <param name="param"></param>
        /// <param name="delay">delay seconds</param>
        //void BattleResultProcess(BattleParam param, float delay);

        /// <summary>
        /// Suspend current battle
        /// </summary>
        /// <param name="type">type</param>
        void PauseBattle(int type);
        /// <summary>
        /// Resume current battle
        /// </summary>
        /// <param name="type">type</param>
        void ResumeBattle(int type);
        /// 宝箱掉落
        /// </summary>
        /// <param name="delay"></param>
        void CollectLoots(float delay = 0);
        /// <summary>
        /// get or set auto battle mode
        /// </summary>
        bool AutoBattleMode { get; set; }

        /*
         *******************************
         *  Battle Event Handler
         *******************************
         */

        /// <summary>
        /// Battle start event
        /// </summary>
        /// <param name="battle"></param>
        void OnBattleStart(string type, CombatConfigData battle);

        void OnBattleEnd(bool immediately = false);

        /*
         *******************************
         *  UI Interactive
         *******************************
         */

        /// <summary>
        /// Show next round button
        /// </summary>
        void ShowNextButton(UnityAction action = null);
        /// <summary>
        /// Show droped golds
        /// </summary>
        /// <param name="num">golds amount</param>
        void ShowDropGolds(int num, Actor role);
        /// <summary>
        /// Show droped Loots by monster
        /// </summary>
        /// <param name="round">round index</param>
        /// <param name="monster">monstrt</param>
        /// <param name="lost_hp">lost hp(percent)</param>
        /// <param name="last_lost_hp">last lost hp(percent)</param>
        void ShowDropLoots(int round, Actor monster, float lost_hp, float last_lost_hp);

        /*
         *******************************
         *  Effects controll
         *******************************
         */

        /// <summary>
        /// Pupup battle statue text
        /// </summary>
        /// <param name="str"></param>
        /// <param name="actor"></param>
        /// <param name="crit">crit or not</param>
        /// <param name="type"></param>
        /// <param name="side"></param>
        /// <param name="exType"></param>
        /// <param name="abilityData"></param>
        void PopupText(PopupType type, string str, int number, IAgent actor, IAgent from, bool crit, RoleSide side, RoleAttribute exType = RoleAttribute.None, AbilityData abilityData = null);

        /// <summary>
        /// freeze screen
        /// </summary>
        void PlayEffect(ScreenEffect effect, float intensity, float duration);

        List<GameObject> GetHeroObjList();

        List<GameObject> GetMonsterObjList(int waveIndex);

        /// <summary>
        /// 战斗正式开始
        /// </summary>
        void StartBattle();

        /// <summary>
        /// 退出战斗
        /// </summary>
        void EndBattle();

        void OnRoleDeath(Actor role, Actor attacker, Skill fromSkill = null);

        void RoleReborn(Actor hero);

        /// <summary>
        /// 获取主摄像机
        /// </summary>
        /// <returns></returns>
        [Obsolete("将来需要弃用，所有摄像机相关接口应该有一个全局摄像机管理类统一处理")]
        Camera GetMainCamera();

        /// <summary>
        /// 获取特效是否在镜头内可见
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        bool EffecVisible(Effect effect);

        void AddEffectOnMap(Effect effect);
    }

}