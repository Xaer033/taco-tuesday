using System;

public interface IGameModeController  
{
    void Start(Action onGameOverCallback);
    void CleanUp();
}
