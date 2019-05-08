using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GhostGen
{
	public class GameStateMachine : System.Object
	{
		public GameStateMachine( IStateFactory p_stateFactory )
		{
			_currentState 	= null;
			_currentId 		= -6666666;
			_stateFactory 	= p_stateFactory;
		}

		public void Step( float p_deltaTime )
		{
			if( _currentState != null )
				_currentState.Step( p_deltaTime );
		}

		public void ChangeState( int stateId )
		{
			if (_currentId == stateId)
				return;

			if( _currentState != null )
				_currentState.Exit( );

			_currentState = _stateFactory.CreateState( stateId );
			_currentState.Init(this);
		}
        
//------------------- Private Implementation -------------------
//--------------------------------------------------------------
		private IStateFactory 	_stateFactory;
		private IGameState 		_currentState;
       
		private int _currentId;
	}
}