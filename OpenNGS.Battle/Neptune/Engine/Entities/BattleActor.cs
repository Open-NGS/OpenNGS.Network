using Neptune;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActor : BattleEntity, ISafeListElement
{
    public BitArray RemoveState { get; set; }
}
