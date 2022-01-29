using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace LT 
{
	public enum LTPlayerClass 
	{
		Undefined = -1,
		Survivor,
		Linry,
		StoreLinry,
		DisguiseLinry
	}

	[Library("ltclass"), AutoGenerate]
	public class PlayerClass : Asset 
	{
		public static Dictionary<string, PlayerClass> All {get; set;} = new();

		public static Dictionary<LTPlayerClass, string> Names {get; set;} = new() 
		{
			{LTPlayerClass.Undefined, "undefined"},
			{LTPlayerClass.Survivor, "survivor"},
			{LTPlayerClass.Linry, "linry"},
			{LTPlayerClass.StoreLinry, "storelinry"},
			{LTPlayerClass.DisguiseLinry, "disguiselinry"}
		};

		public static Dictionary<string, LTPlayerClass> Entries {get; set;} = Names.ToDictionary(x => x.Value, x => x.Key);

		public string Title {get; set;}
		public float MaxHealth {get; set;}
		public float MaxSpeed {get; set;}
		public float CrouchSpeed {get; set;}
		public float EyeHeight {get; set;} = 75;
		public ICamera Camera {get; set;} = new FirstPersonCamera();

		[FGDType("studio"), Category("Models")]
		public string Model {get; set;}
		[FGDType("studio"), Category("Models")]
		public string Hands {get; set;}

		// public PlayerWeaponEntry[] Weapons {get; set;}

		[Hammer.Skip] public LTPlayerClass Entry => Entries[Name];

		// public class PlayerWeaponEntry 
		// {
		// 	public int Ammo {get; set;}

		// 	[ResourceType("ltweapons")]
		// 	public string WeaponPath {get; set;}

		// 	[Hammer.Skip] public WeaponData WeaponData => WeaponData.All.Where(x => x.Path == WeaponPath).FirstOrDefault();
		// }

		protected override void PostLoad()
		{
			base.PostLoad();
			Precache.Add(Model);
			Precache.Add(Hands);

			string classname = Name.ToLower();
			
			All[classname] = this;
			// Event.Run("HUD.Reload");
		}

		protected override void PostReload()
		{
			base.PostReload();
		}

		public string GetTag() 
		{
			return $"class_{Name}";
		}

		public static bool IsValid(string name) 
		{
			name = name.ToLower();

			return All.ContainsKey(name);
		}

		public static PlayerClass Get(string name) 
		{
			name = name.ToLower();

			if (!IsValid(name)) return null;
			return All[name];
		}

		public static PlayerClass Get(LTPlayerClass pclass) 
		{
			string name = Names[pclass];
			if (!IsValid(name)) return null;
			return All[name];
		}
	}
}
