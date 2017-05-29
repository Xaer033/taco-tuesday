using UnityEngine;
using GhostGen;

public class TacoTuesdayState
{
    public const int NO_STATE = -1;
    public const int INTRO = 1;
    public const int MAIN_MENU = 2;
    public const int GAMEPLAY = 3;
    public const int CREDITS = 4;
}


public class TacoTuesdayStateFactory : IStateFactory
{

    public IGameState CreateState(int stateId)
    {
        switch (stateId)
        {
            case TacoTuesdayState.INTRO: return new IntroState();
            case TacoTuesdayState.MAIN_MENU: break;
            case TacoTuesdayState.GAMEPLAY: return new GameplayState();
            case TacoTuesdayState.CREDITS: break;
        }

        Debug.LogError("Error: state ID: " + stateId + " does not exist!");
        return null;
    }
}
