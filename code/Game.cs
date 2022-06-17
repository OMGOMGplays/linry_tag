using Sandbox;
using System;
using System.Linq;

namespace LT 
{
	public partial class LTGame : Game 
	{
		public LTGame() 
		{
			if (IsServer) 
			{
				_ = new LTHud();
			}
		}

		public override void ClientJoined( Client cl )
		{
			base.ClientJoined( cl );

			var pawn = new LTPlayer(cl);
			pawn.Respawn();

			cl.Pawn = pawn;
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
		}
	}
}
