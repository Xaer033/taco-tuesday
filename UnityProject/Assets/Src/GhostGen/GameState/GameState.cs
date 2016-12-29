
namespace GhostGen
{
	public interface IGameState
	{
		void Init( GameController gameManager );

		void Step( float deltaTime );

		void Exit( GameController gameManager );
	}
}
