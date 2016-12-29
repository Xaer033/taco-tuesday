using UnityEngine;
using GhostGen;


public class TacoTuesdayState
{
	public const int NoState 	= -1;
	public const int Intro 		= 1;

}

public class TacoTuesdayStateFactory : IStateFactory
{
	
	public IGameState CreateState( int stateId )
	{
		switch( stateId )
		{
			case TacoTuesdayState.Intro: 	return new IntroState();
		}

		Debug.LogError( "Error: state ID: " + stateId + " does not exist!" );
		return null;
	}
}
