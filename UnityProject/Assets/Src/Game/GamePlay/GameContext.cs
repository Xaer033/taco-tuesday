using System.Collections.Generic;

public enum GameMode
{
    SINGLE_PLAYER,
    PASS_AND_PLAY,
    ONLINE
}

public class GameContext
{
    public GameMode         gameMode        { get; private set; }
    public List<string>     playerNameList  { get; private set; }

    public static GameContext Create(GameMode type, List<string> playerList)
    {
        GameContext gc = new GameContext();
        gc.gameMode = type;
        gc.playerNameList = playerList;
        return gc;
    }

    private GameContext() { }
}
