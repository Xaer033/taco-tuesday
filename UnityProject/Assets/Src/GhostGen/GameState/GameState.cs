
namespace GhostGen
{
	public interface IGameState
	{
		void Init( GameStateMachine stateMachine);

		void Step( float deltaTime );

		void Exit();
	}
}
