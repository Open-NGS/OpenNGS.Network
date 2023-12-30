using System;
using System.Collections.Generic;
using Systems;

/// <summary>
/// The GameMode defines the game being played. It governs the game rules.
/// </summary>
/// 
public abstract class GameMode
{
    public int Id;
    public List<int> hudList;
    public int mapId;

    public void Init()
    {
        OnInit();
    }
    public void Clear()
    {
        OnClear();
    }

    protected virtual void OnInit()
    {

    }

    protected virtual void OnClear()
    {

    }

}
