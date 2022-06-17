using Sandbox;

namespace LT 
{
	public partial class LTPlayer : Player 
	{
		private ClothingContainer Clothing = new();

		public enum LinryTeams
		{
			Unknown,
			Normal,
			Linry
		}

		public LinryTeams CurTeam = LinryTeams.Unknown;

		public LTPlayer() 
		{
		}

		public LTPlayer(Client cl) : this() 
		{
			Clothing.LoadFromClient(cl);
		}

		public override void Respawn()
		{
			base.Respawn();

			//If we're going to do teams, this is not the way to do it -Rifter
			/*if (Rand.Int(0, 1) == 0) 
			{
				SetModel("models/citizen/citizen.vmdl");
				Clothing.DressEntity(this); // Apply player clothing to the normal Terry's, so they can more easily be distinguished from eachother - Lokiv
			}

			if (Rand.Int(0, 1) == 1)
			{
				SetModel("models/linry.vmdl");
				Clothing.ClearEntities(); // Clear entities just in case the player spawned as a normal Terry, then became Linry - Lokiv
			}*/
			
			CameraMode = new ThirdPersonCamera();
			Animator = new StandardPlayerAnimator();
			Controller = new WalkController();
		}
	}
}
