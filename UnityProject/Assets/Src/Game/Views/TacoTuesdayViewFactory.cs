using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GhostGen;

public class TacoTuesdayViews
{
	public const int Splash = 0;
	public const int IntroMovie = 1;
	public const int Gameplay = 2;
}

public class TacoTuesdayViewFactory : IViewFactory
{
	const string kViewDirectory = "GUI/";

	public TacoTuesdayViewFactory()
	{
		_setupViewDirectories ();
	}


	public Dictionary< int, string > GetGameViewMap()
	{
		return _viewDirectoryMap;
	}

	public UIView CreateView( int viewId )
	{
        return _createView(viewId);
	}

    public UIView CreateView(string viewPath)
    {
        return _createView(viewPath);
    }

    public void RemoveView(UIView view)
    {
        if (view == null)
            return;
        
        view.OnViewOutro(false, ()=> GameObject.Destroy(view));
    }


    //------------------- Private Implementation -------------------
    //--------------------------------------------------------------
    private Dictionary< int , string > _viewDirectoryMap;
    

    private UIView _createView(string viewPath)
    {
        UIView viewBase = Resources.Load< UIView>(viewPath);
        if (viewBase == null)
        {
            Debug.LogError("Error: The view Path: " + viewPath +
                               " could not be found");
            return null;
        }
        return GameObject.Instantiate<UIView>(viewBase);
    }

    private UIView   _createView(int viewId)
    {
        Debug.Assert(_viewDirectoryMap.ContainsKey(viewId));

        UIView canvasBase = Resources.Load<UIView>(_viewDirectoryMap[viewId]);
        if (canvasBase == null)
        {

            Debug.LogError("Error: The viewId: " + viewId +
                               " could not be found at path: " +
                               _viewDirectoryMap[viewId]);
            return null;
        }
        return GameObject.Instantiate<UIView>(canvasBase);
    }

    private void _setupViewDirectories()
	{
		_viewDirectoryMap = new Dictionary<int, string> ();

		_viewDirectoryMap.Add( TacoTuesdayViews.Splash, 		kViewDirectory + "SplashView" 	);
		_viewDirectoryMap.Add( TacoTuesdayViews.IntroMovie, 	kViewDirectory + "IntroView" 	);
		_viewDirectoryMap.Add( TacoTuesdayViews.Gameplay, 	    kViewDirectory + "GameView" );
	}
}
