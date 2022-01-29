using Sandbox;
using System;

namespace LT 
{
	public partial class LTWalkController : WalkController 
	{
		const float PlayerMaxSafeFallSpeed = 580;
		const float PlayerMinBounceSpeed = 200;
		const float PlayerFallPunchThreshold = 350;
		public float CrouchSpeed {get; set;} = 33.33f;

		[Net] public float BaseJumpHeight {get; set;} = 52.2006f;

		[Net] public float ForwardAccel {get; set;} = 450.0f;
		[Net] public float SideAccel {get; set;} = 450.0f;

		[ConVar.Replicated]
		public static float lt_maxvelocity {get; set;} = 3500;

		public float FallVelocity {get; set;}

		public override float GetWishSpeed()
		{
			float speed = base.GetWishSpeed();

			if (Pawn is LTPlayer player) 
			{
				if (Duck.IsActive) 
				{
					speed = CrouchSpeed;
				}
			}

			return speed;
		}

		public override void Simulate()
		{
			if (GroundEntity == null) 
			{
				FallVelocity = -Velocity.z;
			}

			CheckVelocity();
			CheckFalling();

			#region Base Simulate Replacement

			EyePosLocal = Vector3.Up * (65 * Pawn.Scale);
			UpdateBBox();

			EyePosLocal += TraceOffset;
			EyeRot = Input.Rotation;

			RestoreGroundPos();

			if (Unstuck.TestAndFix())
				return;

			CheckLadder();
			Swimming = Pawn.WaterLevel.Fraction > 0.6f;

			if (!Swimming && !IsTouchingLadder) 
			{
				Velocity -= new Vector3(0, 0, Gravity * 0.5f) * Time.Delta;
				Velocity += new Vector3(0, 0, BaseVelocity.z) * Time.Delta;

				BaseVelocity = BaseVelocity.WithZ(0);
			}

			if (AutoJump ? Input.Down(InputButton.Jump) : Input.Pressed(InputButton.Jump)) 
			{
				CheckJumpButton();
			}

			bool bStartOnGround = GroundEntity != null;
			if (bStartOnGround) 
			{
				Velocity = Velocity.WithZ(0);

				if (GroundEntity != null) 
				{
					ApplyFriction(GroundFriction * SurfaceFriction);
				}
			}

			WishVelocity = new Vector3(Input.Forward, Input.Left, 0);
			var inSpeed = WishVelocity.Length.Clamp(0, 1);

			if (!Swimming && !IsTouchingLadder) 
			{
				WishVelocity = WishVelocity.WithZ(0);
			}

			WishVelocity = WishVelocity.Normal * inSpeed;
			WishVelocity *= GetWishSpeed();

			Duck.PreTick();

			bool bStayOnGround = false;
			if (Swimming) 
			{
				ApplyFriction(1);
				WaterMove();
			}
			else if (IsTouchingLadder) 
			{
				LadderMove();
			}
			else if (GroundEntity != null) 
			{
				bStayOnGround = true;
				WalkMove();
			}
			else 
			{
				AirMove();
			}

			CategorizePosition(bStayOnGround);

			if (!Swimming && !IsTouchingLadder) 
			{
				Velocity -= new Vector3(0, 0, Gravity * 0.5f) * Time.Delta;
			}

			if (GroundEntity != null) 
			{
				Velocity = Velocity.WithZ(0);
			}

			SaveGroundPos();

			if (Debug) 
			{
				DebugOverlay.Box( Position + TraceOffset, mins, maxs, Color.Red );
				DebugOverlay.Box( Position, mins, maxs, Color.Blue );

				var lineOffset = 0;
				if ( Host.IsServer ) lineOffset = 10;

				DebugOverlay.ScreenText( lineOffset + 0, $"        Position: {Position}" );
				DebugOverlay.ScreenText( lineOffset + 1, $"        Velocity: {Velocity}" );
				DebugOverlay.ScreenText( lineOffset + 2, $"    BaseVelocity: {BaseVelocity}" );
				DebugOverlay.ScreenText( lineOffset + 3, $"    GroundEntity: {GroundEntity} [{GroundEntity?.Velocity}]" );
				DebugOverlay.ScreenText( lineOffset + 4, $" SurfaceFriction: {SurfaceFriction}" );
				DebugOverlay.ScreenText( lineOffset + 5, $"    WishVelocity: {WishVelocity}" );
			}
			#endregion
		}

		public virtual void CheckFalling() 
		{
			if (GroundEntity == null || FallVelocity <= 0)
				return;
			
			if (Pawn.LifeState != LifeState.Dead && FallVelocity >= PlayerFallPunchThreshold) 
			{
				float vol = 0.5f;

				if (!Pawn.WaterLevel.IsInWater) 
				{
					if (GroundEntity.Velocity.z < 0.0f) 
					{
						FallVelocity += GroundEntity.Velocity.z;
						FallVelocity = Math.Max(0.1f, FallVelocity);
					}

					if (FallVelocity > PlayerMaxSafeFallSpeed) 
					{
						vol = 1.0f;
					}
					else if (FallVelocity > PlayerMaxSafeFallSpeed / 2) 
					{
						vol = 0.85f;
					}
					else if (FallVelocity < PlayerMinBounceSpeed) 
					{
						vol = 0;
					}
				}

				PlayerRoughLandingEffects(vol);
			}

			FallVelocity = 0;
		}

		public void PlayerRoughLandingEffects(float vol) 
		{
			if (vol > 0.0) 
			{
				var player = Pawn as LTPlayer;

				var tr = Trace.Ray(Position, Position + Vector3.Down * 20)
				.Radius(1)
				.Ignore(player)
				.Run();

				tr.Surface.DoFootstep(player, tr, 0, vol);
			}
		}

		public virtual void CheckVelocity() 
		{
			Vector3 velocity = Velocity;

			for (int i = 0; i < 3; i++) 
			{
				if (velocity[i] > lt_maxvelocity) 
				{
					velocity[i] = lt_maxvelocity;
				}
			}
		}

		public override void CheckJumpButton()
		{
			if (Swimming) 
			{
				ClearGroundEntity();

				Velocity = Velocity.WithZ(100);

				return;
			}

			Vector3 Vel = Velocity;

			float JumpHeight = BaseJumpHeight;
			float GroundFactor = 1.0f;
			bool bSetZ = false;

			if (GroundEntity == null) 
			{
				return;
			}
			else if (Duck.IsActive) 
			{
				return;
			}

			ClearGroundEntity();

			float flMul = MathF.Sqrt(2.0f * Gravity * JumpHeight);

			if (bSetZ) 
			{
				Vel.z = flMul * GroundFactor * 1.2f;
			}
			else 
			{
				Vel.z += flMul * GroundFactor * 1.2f;

				Vel.z -= Gravity * 0.5f * Time.Delta;
			}

			Velocity = Vel;

			AddEvent("jump");
		}

		public override void UpdateGroundEntity( TraceResult tr )
		{
			base.UpdateGroundEntity( tr );
		}

		bool IsTouchingLadder {get; set;}

		void RestoreGroundPos() 
		{
			if (GroundEntity == null || GroundEntity.IsWorld)
				return;
		}

		void SaveGroundPos() 
		{
			if (GroundEntity == null || GroundEntity.IsWorld)
				return;
		}
	}
}
