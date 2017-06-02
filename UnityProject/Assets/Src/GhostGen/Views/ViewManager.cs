using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GhostGen
{
	public class ViewManager : System.Object
	{
		class LayerContainer
		{
			public int viewId;
			public int layer;
			public UIView view;
			public GameObject rootObject;
		}



		public ViewManager( IViewFactory p_viewFactory, Canvas mainCanvas )
		{
			_viewFactory 	= p_viewFactory;
			//_gameViewPaths = _viewFactory.GetGameViewMap ();

			_currentViews 	= new Dictionary< int, LayerContainer >();
			//_transitionList = new List< LayerContainer > ();

			//_mainCamera = Camera.main;
            _mainCanvas = mainCanvas;

        }


		public void Step( float p_deltaTime )
		{

		}


		public UIView CreateView( int viewId, int layer = 0)
		{
			LayerContainer newLayer = new LayerContainer ();
			newLayer.layer 	= layer;
			newLayer.viewId = viewId;
            newLayer.view   = _createView(viewId);
            newLayer.rootObject = newLayer.view.gameObject;

			_clearLayer (layer);
			_currentViews.Add (layer, newLayer);

			return newLayer.view;
		}

		public UIView FindView( int viewId )
		{
			LayerContainer c = _findContainer (viewId);
			if( c == null )
				return null;

			return c.view;
		}


		public void MoveViewToNewLayer( int viewId, int layer )
		{
			LayerContainer c = _findContainer (viewId);
			if( c == null )
				return;
			
			_currentViews.Remove( c.layer );

			_clearLayer( layer );

			c.layer = layer;

			_currentViews.Add (layer, c);
		}

		public void RemoveView( int viewId )
		{
			LayerContainer c = _findContainer( viewId );
			if( c == null )
				return;

			_removeLayerContainer( c );
		}
		//removeAllViews();
		//clearLayer( int layer );


//------------------- Private Implementation -------------------
//--------------------------------------------------------------
		private Dictionary< int, LayerContainer > _currentViews;
		//private List<LayerContainer> _transitionList;

		private IViewFactory _viewFactory;
		//private Dictionary< int, string > _gameViewPaths;

		//private Camera _mainCamera;
        private Canvas _mainCanvas;

		private LayerContainer _findContainer( int viewId )
		{
			foreach( KeyValuePair< int, LayerContainer> pair in _currentViews )
			{
				if( pair.Value.viewId == viewId )
					return pair.Value;
			}

			return null;
		}

		private void _clearLayer( int p_layer )
		{
			if( _currentViews.ContainsKey( p_layer) )
				_removeLayerContainer( _currentViews[ p_layer ] );
		}

		private void _removeLayerContainer( LayerContainer p_container )
		{
			//Clean up/start transition if it has one
			p_container.view.OnViewOutro( null);

			//Enter transition if one exists
			//if( p_container.view.hasTransition )
			//{
			//	p_container.view.isTransitioning = true;
			//	p_container.view.onTransitionExit();

			//	_transitionList.Add (p_container);
			//} 
			//else //or just destroy it right away
			{
				_destroyLayerContainer( p_container );
			}

			//either way it will be removed from the regular update
			_currentViews.Remove( p_container.layer );
		}

		

		private UIView _createView( int p_view )
		{
			UIView v = _viewFactory.CreateView( p_view );
            v.transform.SetParent(_mainCanvas.transform, false);
            return v;
		}
        

		private void _destroyLayerContainer( LayerContainer p_container )
		{
			p_container.view = null;

			GameObject.Destroy (p_container.rootObject);
		}
	}
}
