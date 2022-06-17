using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace LT 
{
	[Library]
	public partial class LTHud : RootPanel
	{
		public static LTHud Current;

		public LTHud() 
		{
			//Delete and nullify the hud if it already exists
			//for a new one to be replacing it
			if ( Current != null )
			{
				Current?.Delete();
				Current = null;
			}

			Current = this;

			AddChild<LTChatBox>();
		}
	}
}
