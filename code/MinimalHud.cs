using Sandbox.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace MinimalExample
{
	public partial class MinimalHudEntity : Sandbox.HudEntity<RootPanel>
	{
		public MinimalHudEntity()
		{
			if ( IsClient )
			{
				RootPanel.SetTemplate( "/minimalhud.html" );
			}
		}
	}

}
