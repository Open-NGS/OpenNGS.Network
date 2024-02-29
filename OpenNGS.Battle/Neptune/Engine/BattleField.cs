using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Neptune;
/// <summary>
/// Battle Field Manager
/// </summary>
public static class BattleField {

    public class BattleArea
    {
        /// <summary>
        /// Area center position
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// Area rotation
        /// </summary>
        public Vector3 rotation;
        public Matrix4x4 matrix;
        public Rect rect;
    }


    /// <summary>
    /// 逻辑 - 世界坐标比例系数
    /// </summary>
    public static float LogicWorldFactor = 100f;

    /// <summary>
    /// 世界 - 逻辑坐标比例系数
    /// </summary>
    public static float WorldLogicFactor = UFloat.Round(0.01f);


    public static Rect BattleAreaRect= new Rect();
    static private List<BattleArea> battleCenter = new List<BattleArea>();
    static private BattleArea battleIdentity= new BattleArea() { matrix = Matrix4x4.identity };

    /// <summary>
    /// Get current battle area
    /// </summary>
    static public BattleArea Current
    {
        get
        {
            if (battleCenter.Count >= NeptuneBattle.Instance.Round)
            {
                return battleCenter[NeptuneBattle.Instance.Round - 1];
            }
            else
            {
                return battleIdentity;
            }
        }
    }

    public static Vector3 Forward
    {
        get { return BattleField.Reverse ? Vector3.left : Vector3.right; }
    }

    public static Vector3 Backward
    {
        get { return BattleField.Reverse ? Vector3.right : Vector3.left; }
    }

    static public void Clear()
    {
        battleCenter.Clear();
    }

    static public void AddBattleArea(BattleArea area)
    {
        battleCenter.Add(area);
        BattleAreaRect = area.rect;
    }

    public static bool Reverse = false;

    static public Vector3 WorldToLogic(Vector3 world)
    {
        Vector3 logic = Current.matrix.inverse.MultiplyPoint3x4(world);
        logic = new Vector3(UFloat.Round(logic.x) * BattleField.LogicWorldFactor, UFloat.Round(logic.z) * BattleField.LogicWorldFactor, UFloat.Round(logic.y) * BattleField.LogicWorldFactor);
        return logic;
    }

    static public Vector3 LogicToWorld(UVector3 logic)
    {
        Vector3 world = new Vector3(logic.x * BattleField.WorldLogicFactor, logic.z * BattleField.WorldLogicFactor, logic.y * BattleField.WorldLogicFactor);
        world = Current.matrix.MultiplyPoint3x4(world);
        return world;
    }

    static public bool InBattleArea(Vector3 pos)
    {
        Rect rect = BattleField.Current.rect;//获取战斗区域
        return rect.Contains(pos);
    }

    public static Vector2 LogicPos2RC(Vector2 logicPos)
    {
        //if (BattleField.Reverse)
        //{
        //    logicPos = -logicPos;
        //}

        Vector2 rc = EngineConst.Vector2Zero;

        rc.x = Mathf.FloorToInt((logicPos.x + NavLayer.NavMap.Height *0.5f) / NavLayer.NavMap.GridSize);
        rc.y = Mathf.FloorToInt((logicPos.y + NavLayer.NavMap.Width *0.5f) / NavLayer.NavMap.GridSize);
        rc.x = Mathf.Clamp(rc.x, 0, NavLayer.NavMap.GridRows - 1);
        rc.y = Mathf.Clamp(rc.y, 0, NavLayer.NavMap.GridCols - 1);

        return rc;
    }
}
