using GhostGen;

public class BaseController
{
    protected UIView _view;

    protected ViewFactory viewFactory
    {
        get
        {
            return Singleton.instance.gui.viewFactory;
        }
    }

    protected UIView view
    {
        get { return _view; }
        set { _view = value; }
    }

    public virtual void RemoveView(bool immediately = false)
    {
        if(view != null)
        {
            viewFactory.RemoveView(view, immediately);
        }
    }
}
