
using System.Collections.Generic;

namespace GhostGen
{
	public interface IViewFactory
	{
		Dictionary<int, string> GetGameViewMap();
		UIView                  CreateView( int p_viewId );
	}
}