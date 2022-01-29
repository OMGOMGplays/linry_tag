using Sandbox;
using Sandbox.UI;

namespace LT 
{
	partial class LTPlayer 
	{
		[Net] public PlayerClass PlayerClass {get; set;}

		/// <param name ="pclass"></param>
		public void SetClass(PlayerClass pclass) 
		{
			Host.AssertServer();

			if (PlayerClass == pclass) return;

			bool willRespawn = LifeState == LifeState.Alive;

			if (willRespawn) Respawn();
			else 
			{
				ChatBox.AddInformation(To.Single(Client), "how the fuck");
			}
		}

		private void SetupPlayerClass() 
		{
			if (PlayerClass == null) return;

			SetModel(PlayerClass.Model);

			if (Controller is LTWalkController walk) 
			{
				walk.DefaultSpeed = PlayerClass.MaxSpeed;
				walk.SprintSpeed = PlayerClass.MaxSpeed;
				walk.WalkSpeed = PlayerClass.MaxSpeed;
				walk.CrouchSpeed = PlayerClass.CrouchSpeed;

				walk.EyeHeight = PlayerClass.EyeHeight;
			}
		}
	}
}
