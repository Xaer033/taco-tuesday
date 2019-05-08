using System;

public interface IGameModeController  
{
    void Start(Action onGameOverCallback);
    void Step(double now);
    void CleanUp();
}
