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
    public bool             isMasterClient  { get; private set; }

    public static GameContext Create(GameMode type, List<string> playerList, bool isMasterClient)
    {
        GameContext gc = new GameContext();
        gc.gameMode = type;
        gc.playerNameList = playerList;
        gc.isMasterClient = isMasterClient;
        return gc;
    }

    private GameContext() { }
}
