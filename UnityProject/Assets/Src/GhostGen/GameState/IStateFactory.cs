using UnityEngine;
using System.Collections;


namespace GhostGen
{
	public interface IStateFactory 
	{
		IGameState GetState( int stateId );
	}
}