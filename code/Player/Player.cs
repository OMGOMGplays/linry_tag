using Sandbox;

namespace LT 
{
	public partial class LTPlayer : Player 
	{
		public override void Respawn()
		{
			base.Respawn();

			if (Rand.Int(0, 1) == 0) 
			{
				SetModel("models/citizen/citizen.vmdl");
			}

			if (Rand.Int(0, 1) == 1)
			{
				SetModel("models/linry.vmdl");
			}

			CameraMode = new ThirdPersonCamera();
			Animator = new StandardPlayerAnimator();
			Controller = new WalkController();
		}
	}
}
