using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameData
{
    public class WaypointData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PathData> Pathes { get; set; }
    }

    public class PathData
    {
        public int Id { get; set; }
        public PathFlag PathFlag { get; set; }
        public List<NodeData> Nodes { get; set; }
    }

    public class NodeData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Ignorable { get; set; }
        public ArenaFlag ArenaFlag { get; set; }
        public bool Tower { get; set; }
    }

    public enum PathFlag
    {
        RIGHT,
        MIDDLE,
        LEFT
    }

    public enum ArenaFlag
    {
        SELFRIGHT,
        SELFLEFT,
        OPPONENTRIGHT,
        OPPONENTLEFT,
        NULL
    }
}