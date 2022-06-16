using Sandbox;

namespace LT 
{
	public partial class LTGame : Game 
	{
		public override void ClientJoined( Client cl )
		{
			base.ClientJoined( cl );

			var pawn = new LTPlayer();
			pawn.Respawn();

			cl.Pawn = pawn;
		}
	}
}
