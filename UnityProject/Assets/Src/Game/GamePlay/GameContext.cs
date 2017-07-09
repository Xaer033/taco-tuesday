using System.Collections.Generic;

public enum GameType
{
    SINGLE_PLAYER,
    PASS_AND_PLAY,
    ONLINE
}

public class GameContext
{
    public GameType         gameType        { get; private set; }
    public List<string>     playerNameList  { get; private set; }

    public static GameContext Create(GameType type, List<string> playerList)
    {
        GameContext gc = new GameContext();
        gc.gameType = type;
        gc.playerNameList = playerList;
        return gc;
    }

    private GameContext() { }
}
