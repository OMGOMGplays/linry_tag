using Sandbox;
using Sandbox.UI;

namespace LT.UI 
{
	[Library]
	public partial class LTHud : HudEntity<RootPanel>
	{
		public LTHud()
		{
			if ( !IsClient )
				return;

			RootPanel.StyleSheet.Load( "/ui/LTHud.scss" );

			RootPanel.AddChild<CrosshairCanvas>();
			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<VoiceList>();
			RootPanel.AddChild<KillFeed>();
			RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
			RootPanel.AddChild<Vitals>();
		}
	}
}
