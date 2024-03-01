using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using Neptune;

/// <summary>
/// Actor controller interface，Actor controller must implement this interface
/// </summary>
public interface IActorController : IController
{
    void SetRole(BattleActor role);

    void Init();


    /// <summary>
    /// 设置动作并立即开始
    /// </summary>
    /// <param name="anim_name">动作名称</param>
    void SetAnimation(string animName, int transformId = 1, bool immediately = false);
    /// <summary>
    /// 设置下一个动作
    /// 下一个动作会在当前动作结束后开始
    /// </summary>
    /// <param name="anim_name">动作名称</param>
    void SetNextAnimation(string action_name, int transformId = 1);
    /// <summary>
    /// 设置动作速率
    /// </summary>
    /// <param name="i">速度</param>
    void SetAnimationSpeed(float v, bool force = false);
    /// <summary>
    /// 设置动作循环模式
    /// </summary>
    /// <param name="loop">是否循环</param>
    void SetAnimationLoop(bool loop);
    /// <summary>
    /// 设置动作流逝时间
    /// </summary>
    /// <param name="elapsed">流逝时间</param>
    void SetAnimationElapsed(double elapsed);
    /// <summary>
    /// 动作完成事件
    /// </summary>
    void OnAnimationEnd();

    /// <summary>
    /// 添加特效到当前Actor
    /// </summary>
    /// <param name="name">特效名称</param>
    /// <param name="z">显示顺序</param>
    int AddEffect(string name, Neptune.GameData.EffectType type, Vector3 offset,IActorController source = null, string bindingNode = "");
    /// <summary>
    /// 移除当前Actor上的指定特效（根据名称）
    /// </summary>
    /// <param name="name">名称</param>
    void RemoveEffect(string name);
    /// <summary>
    /// 移除当前Actor上的指定特效（根据id）
    /// </summary>
    /// <param name="id">ID</param>
    void RemoveEffect(int id);

    void RemoveAllEffect();

    /// <summary>
    /// 设置Shader
    /// </summary>
    /// <param name="fx"></param>
    int SetRoleFX(int fxtID);
    /// <summary>
    /// 重置Shader
    /// </summary>
    void ResetRoleFX(int fxID);
    /// <summary>
    /// 中断当前声音
    /// </summary>
    void StopSound();
    /// <summary>
    /// 对当前Actor进行着色
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    void SetColor(float r, float g, float b);

    /// <summary>
    /// 设置当前角色的高亮状态
    /// </summary>
    /// <param name="flag">true: 设置高亮， false： 取消高亮</param>
    void Highlight(bool flag);

    /// <summary>
    /// 逻辑帧循环
    /// </summary>
    /// <param name="passTime"></param>
    void OnEnterFrame(float passTime);

    /// <summary>
    /// 死亡事件处理
    /// </summary>
    void OnDeath();

    /// <summary>
    /// 受到伤害
    /// </summary>
    void OnDamaged();

    void SetReversed(bool reversed);

    void OnDeployed();

    void SetEffectChild(int id, int index);
    
    void StopVoice();

    Transform GetAttackTransform();

    /// <summary>
    /// 是否可以看到此角色
    /// </summary>
    bool InSight { get; }

    /// <summary>
    /// 是否可渲染
    /// 通常只有在摄像机范围内的对象才可渲染
    /// </summary>
    bool IsRenderable { get; }

    void OnDodge();
}


/// <summary>
/// NullActorController
/// IUnitController的空实现
/// </summary>
public class NullActorController : IActorController
{

    public Vector3 Direction { get; set; }
    public bool IsRunning { get { return false; } }

    public bool InSight
    {
        get
        {
            return false;
        }
    }

    public bool IsRenderable
    {
        get
        {
            return false;
        }
    }

    public void SetAnimationLoop(bool loop)
    {
    }

    public void StopSound()
    {

    }

    public void SetAnimation(string animName, int transformId = 1, bool immediately = false)
    {
    }

    public void SetAnimationSpeed(float v, bool force = false)
    {
    }
    public void SetAnimationElapsed(double elapsed)
    {

    }
    public void OnAnimationEnd()
    {
    }

    public int AddEffect(string name, Neptune.GameData.EffectType type, Vector3 offset, IActorController source = null, string bindingNode = "")
    {
        return -1;
    }

    public void RemoveEffect(string name)
    {

    }

    public void RemoveAllEffect()
    {
        
    }
    public int SetRoleFX(int effectId)
    {
        return 0;
    }

    public void SetColor(float r,float g,float b)
    {

    }

    public void Highlight(bool flag)
    {
    }

    public void SetPosition(Vector3 pos)
    {
        
    }

    public void SetRotation(float rot)
	{

	}

    public void SetScale(Vector3 svale)
    {
    }

    public void RemoveEffect(int id)
    {
    }


    public void ResetRoleFX(int effectId)
    {

    }


    public void SetVisible(bool visible, bool isTransparent, string ignoreEffectNames = "NA")
    {

    }

    public void SetNextAnimation(string anim_name,int transformId =1)
    {
        
    }



    public void Suspend()
    {
        
    }

    public void Resume()
    {
        
    }

    public void Stop(float delay = 0)
    {

    }
    
    public void OnDeath()
    {

    }

    public void SetAnimation(int transformID, int animationID, bool immediately)
    {
    }

    public void SetNextAnimation(int transformID, int animationID)
    {
    }

    public void SetReversed(bool reversed)
    {
    }

    public void Highlight()
    {
        
    }

    public void Reset()
    {
    }

    public void OnDamaged()
    {
    }

    public void OnDeployed()
    {
    }

    public void SetRole(BattleActor role)
    {
        
    }

    public void Init()
    {
        
    }

    public void SetEffectChild(int id, int index)
    {
 
    }

    public void JointPlaySound(string name)
    {
        
    }
    public void PlayVoice(string name)
    {

    }

    public void StopVoice()
    {

    }

    public Transform GetAttackTransform()
    {
        return null;
    }

    public void OnEnterFrame(float passTime)
    {
    }

    public void OnDodge()
    {
        
    }
}