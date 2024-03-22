
public interface ILevelProcessSystem
{
    public void InitLevelStage(ILevelStageFactor factor);
    public void StartBegin();
    public void UpdateStages(float deltaTime);
}
