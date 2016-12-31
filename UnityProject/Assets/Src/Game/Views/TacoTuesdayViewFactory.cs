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

	public UIView CreateView( int viewId, OnCreationFinishedHandle onCreateCallback )
	{
        GameObject viewObj = _createView(viewId);
        UIView view = viewObj.GetComponent<UIView>();
        view.OnCreationFinishedEvent += onCreateCallback;
        return view;
	}

    public UIView CreateView(string viewPath, OnCreationFinishedHandle onCreateCallback)
    {
        GameObject viewObj = _createView(viewPath);
        UIView view = viewObj.GetComponent<UIView>();
        view.OnCreationFinishedEvent += onCreateCallback;
        return view;
    }
    //------------------- Private Implementation -------------------
    //--------------------------------------------------------------
    private Dictionary< int , string > _viewDirectoryMap;


    private GameObject _createView(string viewPath)
    {
        GameObject viewBase = Resources.Load(viewPath) as GameObject;
        if (viewBase == null)
        {
            Debug.LogError("Error: The view Path: " + viewPath +
                               " could not be found");
            return null;
        }
        return GameObject.Instantiate(viewBase) as GameObject;
    }

    private GameObject _createView(int viewId)
    {
        Debug.Assert(_viewDirectoryMap.ContainsKey(viewId));

        GameObject canvasBase = Resources.Load(_viewDirectoryMap[viewId]) as GameObject;
        if (canvasBase == null)
        {

            Debug.LogError("Error: The viewId: " + viewId +
                               " could not be found at path: " +
                               _viewDirectoryMap[viewId]);
            return null;
        }
        return GameObject.Instantiate(canvasBase) as GameObject;
    }

    private void _setupViewDirectories()
	{
		_viewDirectoryMap = new Dictionary<int, string> ();

		_viewDirectoryMap.Add( TacoTuesdayViews.Splash, 		kViewDirectory + "SplashView" 	);
		_viewDirectoryMap.Add( TacoTuesdayViews.IntroMovie, 	kViewDirectory + "IntroView" 	);
		_viewDirectoryMap.Add( TacoTuesdayViews.Gameplay, 	    kViewDirectory + "GameView" );
	}
}
