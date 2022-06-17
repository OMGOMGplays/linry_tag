using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace LT 
{
	[Library]
	public partial class LTHud : HudEntity<RootPanel> 
	{
		public LTHud() 
		{
			if (!IsClient) return;

			RootPanel.AddChild<LTChatBox>();
		}
	}
}
