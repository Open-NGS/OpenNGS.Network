using Neptune;
using Neptune.GameData;

public abstract class AbilityBase : PoolObj
{
    protected static readonly float defaultEvtX = 1f;
    protected static readonly float defaultEvtY = 1f;

    public BattleActor Caster;
    public BattleActor Target;

    public float Duration;

    public int HitTimes = 0;
    public float KeepHp = -1;


    public void Create()
    {
        this.Caster = null;
        this.Target = null;

        this.Duration = 0;

        this.HitTimes = 0;
        this.KeepHp = -1;
    }
}
