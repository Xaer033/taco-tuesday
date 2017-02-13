using GhostGen;

public class BaseController
{
    protected ViewFactory viewFactory
    {
        get { return Singleton.instance.viewFactory; }
    }
}
