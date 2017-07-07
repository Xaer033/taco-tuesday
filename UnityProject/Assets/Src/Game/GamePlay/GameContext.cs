using System.Collections.Generic;

public class GameContext
{
    public enum GameType
    {
        SINGLE_PLAYER,
        PASS_AND_PLAY,
        ONLINE
    }

    public GameType         gameType        { get; set; }
    public List<string>     playerNameList  { get; set; }

    public static GameContext Create(GameType type)
    {
        GameContext gc = new GameContext();
        gc.gameType = type;
        gc.playerNameList = new List<string>(PlayerGroup.kMaxPlayerCount);
        return gc;
    }

    private GameContext() { }
}
