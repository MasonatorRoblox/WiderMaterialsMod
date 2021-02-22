using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Common.Extension;
using HarmonyLib;
using UnityEngine;
using PolyTechFramework;

namespace WiderMaterials
{
	[BepInPlugin("polytech.widerMaterials", "Wider Materials", "2.2.0")]
	[BepInProcess("Poly Bridge 2")]
	[BepInDependency(PolyTechMain.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
	public class WiderMaterialsMain : PolyTechMod
	{
		public void Awake()
		{
			base.Config.Bind(ModEnabledDef, true, new ConfigDescription("Enable Mod (Material width will refresh on sim start/end)", null));
			WiderMaterialsMain.ModEnabled = (ConfigEntry<bool>)base.Config[ModEnabledDef];
			ModEnabled.SettingChanged += onEnableDisable;
			base.Config.Bind<float>(WiderMaterialsMain.RoadWidthDef, 1f, new ConfigDescription("Road Width (1f is the default value)", null));
            WiderMaterialsMain.RoadWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.RoadWidthDef];
			base.Config.Bind<float>(WiderMaterialsMain.WoodWidthDef, 1f, new ConfigDescription("Wood Width (1f is the default value)", null));
            WiderMaterialsMain.WoodWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.WoodWidthDef];
			base.Config.Bind<float>(WiderMaterialsMain.SteelWidthDef, 1f, new ConfigDescription("Steel Width (1f is the default value)", null));
            WiderMaterialsMain.SteelWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.SteelWidthDef];
			base.Config.Bind<float>(WiderMaterialsMain.HydraulicWidthDef, 1f, new ConfigDescription("Hydraulic Width (1f is the default value)", null));
            WiderMaterialsMain.HydraulicWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.HydraulicWidthDef];
			//Everything related to ropes, cables, and springs are commented off until I or someone else can find a way to properly increase their width.
			//base.Config.Bind<float>(WiderMaterialsMain.RopeWidthDef, 1f, new ConfigDescription("Rope Width (1f is the default value)", null));
           	//WiderMaterialsMain.RopeWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.RopeWidthDef];
			//base.Config.Bind<float>(WiderMaterialsMain.CableWidthDef, 1f, new ConfigDescription("Cable Width (1f is the default value)", null));
            //WiderMaterialsMain.CableWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.CableWidthDef];
			//base.Config.Bind<float>(WiderMaterialsMain.SpringWidthDef, 1f, new ConfigDescription("Spring Width (1f is the default value)", null));
            //WiderMaterialsMain.SpringWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.SpringWidthDef];
			this.repositoryUrl = "https://github.com/MasonatorRoblox/WiderMaterialsMod/";
			Harmony harmony = new Harmony("polytech.widerMaterials");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			this.isEnabled = true;
			this.isCheat = false;
			this.authors = new string[] {"Mason (MasonatorRoblox)", "Conqu3red", "MoonlitJolty"};
			PolyTechMain.registerMod(this);
		}

		private void onEnableDisable(object sender, EventArgs e)
        {
            if (ModEnabled.Value) enableMod();
            else disableMod();
            this.isEnabled = ModEnabled.Value;
		}
		public override void enableMod()
        {
        }

        public override void disableMod()
		{			
		}
		
		[HarmonyPatch(typeof(BridgeEdge))]
		[HarmonyPatch("_UpdateTransform")]
		public static class patchRoads
		{
			public static void Postfix(BridgeEdge __instance)
			{
				if(!ModEnabled.Value) return;
				bool road = __instance.m_Material.m_MaterialType == BridgeMaterialType.ROAD || __instance.m_Material.m_MaterialType == BridgeMaterialType.REINFORCED_ROAD;
				if (road)
				{
					TransformExtension.SetLocalScaleZ(__instance.m_MeshRenderer.transform, WiderMaterialsMain.RoadWidth.Value);
				}
				bool wood = __instance.m_Material.m_MaterialType == BridgeMaterialType.WOOD;
				if (wood)
				{
					TransformExtension.SetLocalScaleZ(__instance.m_MeshRenderer.transform, WiderMaterialsMain.WoodWidth.Value);
				}
				bool steel = __instance.m_Material.m_MaterialType == BridgeMaterialType.STEEL;
				if (steel)
				{
					TransformExtension.SetLocalScaleZ(__instance.m_MeshRenderer.transform, WiderMaterialsMain.SteelWidth.Value);
				}
				bool hydraulic = __instance.m_Material.m_MaterialType == BridgeMaterialType.HYDRAULICS;
				if (hydraulic)
				{
					TransformExtension.SetLocalScaleZ(__instance.m_MeshRenderer.transform, WiderMaterialsMain.HydraulicWidth.Value);
				}
				//bool rope = __instance.m_Material.m_MaterialType == BridgeMaterialType.ROPE;
				//if (rope)
				//{
				//	TransformExtension.SetLocalScaleZ(__instance.m_MeshRenderer.transform, WiderMaterialsMain.RopeWidth.Value);
				//}
				//bool cable = __instance.m_Material.m_MaterialType == BridgeMaterialType.CABLE;
				//if (cable)
				//{
				//	TransformExtension.SetLocalScaleZ(__instance.m_MeshRenderer.transform, WiderMaterialsMain.CableWidth.Value);
				//}
				//bool spring = __instance.m_Material.m_MaterialType == BridgeMaterialType.SPRING;
				//if (spring)
				//{
				//	TransformExtension.SetLocalScaleZ(__instance.m_MeshRenderer.transform, WiderMaterialsMain.SpringWidth.Value);
				//}
			}
		}
		public static ConfigDefinition ModEnabledDef = new ConfigDefinition("Enable/Disable", "Enabled");
        public static ConfigEntry<bool> ModEnabled;
		public static ConfigDefinition RoadWidthDef = new ConfigDefinition("Material Width", "Road Width");
        public static ConfigEntry<float> RoadWidth;
		public static ConfigDefinition WoodWidthDef = new ConfigDefinition("Material Width", "Wood Width");
        public static ConfigEntry<float> WoodWidth;
		public static ConfigDefinition SteelWidthDef = new ConfigDefinition("Material Width", "Steel Width");
        public static ConfigEntry<float> SteelWidth;
		public static ConfigDefinition HydraulicWidthDef = new ConfigDefinition("Material Width", "Hydraulic Width");
        public static ConfigEntry<float> HydraulicWidth;
		//public static ConfigDefinition RopeWidthDef = new ConfigDefinition("Material Width", "Rope Width");
        //public static ConfigEntry<float> RopeWidth;
		//public static ConfigDefinition CableWidthDef = new ConfigDefinition("Material Width", "Cable Width");
        //public static ConfigEntry<float> CableWidth;
		//public static ConfigDefinition SpringWidthDef = new ConfigDefinition("Material Width", "Spring Width");
        //public static ConfigEntry<float> SpringWidth;
	}
}
