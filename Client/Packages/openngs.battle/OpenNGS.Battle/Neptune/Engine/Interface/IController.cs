using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// IController
/// GameObject controller interface
/// </summary>
public interface IController
{
    /// <summary>
    /// Get or set direction of gameobject
    /// </summary>
    Vector3 Direction { get; set; }
    /// <summary>
    /// Get if gameobject is terminated
    /// </summary>
    /// <returns></returns>
    bool IsRunning { get; }
    /// <summary>
    /// Set position of gameobject
    /// </summary>
    /// <param name="pos"></param>
    void SetPosition(Vector3 pos);
    /// <summary>
    /// Set rotation of gameobject
    /// </summary>
    /// <param name="rot"></param>
    void SetRotation(float rot);

    /// <summary>
    /// Set scale of gameobject
    /// </summary>
    /// <param name="svale"></param>
    void SetScale(Vector3 svale);

    /// <summary>
    /// Set visiblity and Trasnparence of gameobject
    /// </summary>
    /// <param name="visible"></param>
    void SetVisible(bool visible, bool isTrasnparent = false, string ignoreEffectNames = "NA");
    /// <summary>
    /// Suspend gameobject
    /// </summary>
    void Suspend();
    /// <summary>
    /// Resume gameobject
    /// </summary>
    void Resume();
    /// <summary>
    /// Terninate gameobject
    /// </summary>
    void Stop(float delay = 0);
    void Reset();
    void JointPlaySound(string name);
    void PlayVoice(string name);
}
