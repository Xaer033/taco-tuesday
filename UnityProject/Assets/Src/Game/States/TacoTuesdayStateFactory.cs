using UnityEngine;
using GhostGen;

public class TacoTuesdayState
{
    public const int NO_STATE = -1;

    public const int INTRO = 1;
    public const int MAIN_MENU = 2;
    public const int GAMEPLAY = 3;
    public const int CREDITS = 4;
    public const int SETUP_GAME = 5;
}


public class TacoTuesdayStateFactory : IStateFactory
{
    public IGameState CreateState(int stateId)
    {
        switch (stateId)
        {
            case TacoTuesdayState.INTRO: return new IntroState();
            case TacoTuesdayState.MAIN_MENU: return new MainMenuState();
            case TacoTuesdayState.GAMEPLAY: return new GameplayState();
            case TacoTuesdayState.SETUP_GAME: return new PlayerSetupState();
            case TacoTuesdayState.CREDITS: break;
        }

        Debug.LogError("Error: state ID: " + stateId + " does not exist!");
        return null;
    }
}
