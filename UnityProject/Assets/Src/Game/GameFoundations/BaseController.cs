using GhostGen;

public class BaseController
{

    protected ViewFactory viewFactory
    {
        get { return GameManager.Get().viewFactory; }
    }
}
