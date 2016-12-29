using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GhostGen
{
	public class GameController : System.Object
	{
		public GameController( IStateFactory p_stateFactory, IViewFactory p_viewFactory, Canvas canvas )
		{
			_viewManager = new ViewManager( p_viewFactory, canvas );

			_currentState 	= null;
			_currentId 		= -6666666;
			_stateFactory 	= p_stateFactory;
		}

		public void Step( float p_deltaTime )
		{
			if( _currentState != null )
				_currentState.Step( p_deltaTime );

			if( _viewManager != null )
				_viewManager.Step( p_deltaTime );
		}

		public void ChangeState( int stateId )
		{
			if (_currentId == stateId)
				return;

			if( _currentState != null )
				_currentState.Exit( this );

			_currentState = _stateFactory.CreateState( stateId );
			_currentState.Init(this);
		}


		public ViewManager GetUI()
		{
			return _viewManager;
		}
//------------------- Private Implementation -------------------
//--------------------------------------------------------------
		private IStateFactory 	_stateFactory;
		private IGameState 		_currentState;

		private ViewManager 	_viewManager;
			
		private int _currentId;
		
	}
}