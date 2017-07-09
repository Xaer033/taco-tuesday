
namespace GhostGen
{
	public interface IGameState
	{
		void Init( GameController p_gameController);

		void Step( float deltaTime );

		void Exit( GameController p_gameController);
	}
}
