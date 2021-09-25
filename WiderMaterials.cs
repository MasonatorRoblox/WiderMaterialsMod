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
using HarmonyLib;
using Common.Extension;
using UnityEngine;
using PolyTechFramework;

namespace WiderMaterials
{
	[BepInPlugin("polytech.widerMaterials", "Wider Materials", "2.3.0")]
	[BepInProcess("Poly Bridge 2")]
	[BepInDependency(PolyTechMain.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
	public class WiderMaterialsMain : PolyTechMod
	{
		public void Awake()
		{
			int order = 0;
			base.Config.Bind(ModEnabledDef, true, new ConfigDescription("Enable Mod (Material width will refresh on sim start/end)", null, new ConfigurationManagerAttributes { Order = order }));
			WiderMaterialsMain.ModEnabled = (ConfigEntry<bool>)base.Config[ModEnabledDef];
			ModEnabled.SettingChanged += onEnableDisable;
			order--;
			base.Config.Bind<float>(WiderMaterialsMain.RoadWidthDef, 1f, new ConfigDescription("Road Width (1f is the default value)", null, new ConfigurationManagerAttributes { Order = order }));
            WiderMaterialsMain.RoadWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.RoadWidthDef];
			order--;
			base.Config.Bind<float>(WiderMaterialsMain.WoodWidthDef, 1f, new ConfigDescription("Wood Width (1f is the default value)", null, new ConfigurationManagerAttributes { Order = order }));
            WiderMaterialsMain.WoodWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.WoodWidthDef];
			order--;
			base.Config.Bind<float>(WiderMaterialsMain.SteelWidthDef, 1f, new ConfigDescription("Steel Width (1f is the default value)", null, new ConfigurationManagerAttributes { Order = order }));
            WiderMaterialsMain.SteelWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.SteelWidthDef];
			order--;
			base.Config.Bind<float>(WiderMaterialsMain.HydraulicWidthDef, 1f, new ConfigDescription("Hydraulic Width (1.1f is the default value)", null, new ConfigurationManagerAttributes { Order = order }));
            WiderMaterialsMain.HydraulicWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.HydraulicWidthDef];
			order--;
			base.Config.Bind<float>(WiderMaterialsMain.RopeWidthDef, 1f, new ConfigDescription("Rope Width (1f is the default value)", null, new ConfigurationManagerAttributes { Order = order }));
           	WiderMaterialsMain.RopeWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.RopeWidthDef];
			order--;
			base.Config.Bind<float>(WiderMaterialsMain.CableWidthDef, 1f, new ConfigDescription("Cable Width (1f is the default value)", null, new ConfigurationManagerAttributes { Order = order }));
            WiderMaterialsMain.CableWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.CableWidthDef];
			order--;
			//base.Config.Bind<float>(WiderMaterialsMain.SpringWidthDef, 1f, new ConfigDescription("Spring Width (1f is the default value)", null, new ConfigurationManagerAttributes { Order = order }));
            //WiderMaterialsMain.SpringWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.SpringWidthDef];
			this.repositoryUrl = "https://github.com/MasonatorRoblox/WiderMaterialsMod/";
			Harmony harmony = new Harmony("polytech.widerMaterials");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			this.isEnabled = true;
			this.isCheat = false;
			this.authors = new string[] {"Mason (Masonator)", "Conqu3red", "MoonlitJolty"};
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
					TransformExtension.SetLocalScaleZ(__instance.m_MeshRenderer.transform, WiderMaterialsMain.HydraulicWidth.Value * 1.1f);
				}
				bool rope = __instance.m_Material.m_MaterialType == BridgeMaterialType.ROPE;
				bool cable = __instance.m_Material.m_MaterialType == BridgeMaterialType.CABLE;
			}
		}
		[HarmonyPatch(typeof(BridgeEdge))]
		[HarmonyPatch("_UpdateTransform")]
		public class getValues
		{
			public static void Prefix(BridgeEdge __instance)
			{
				bool isRope = __instance.m_Material.m_MaterialType == BridgeMaterialType.ROPE;
				bool isCable = __instance.m_Material.m_MaterialType == BridgeMaterialType.CABLE;
			}
		}
		[HarmonyPatch(typeof(BridgeRope))]
		[HarmonyPatch("UpdateLinks")]
		public class patchRopes
		{
			public static void Postfix(BridgeRope __instance/*, getValues isRope, getValues isCable*/)
			{
				/*bool rope = Convert.ToBoolean(isRope);
				bool cable = Convert.ToBoolean(isCable);*/
				Vector3[] array = __instance.m_PhysicsRope.ComputeNodePositions();
				int num = array.Length - 1;
				int num2 = Mathf.Min(num, __instance.m_Links.Count);
				if(!ModEnabled.Value/* || !(rope || cable)*/) return;
				/*if (rope)
				{*/
					for (int j = 0; j < num2; j++)
					{
						__instance.m_Links[j].m_Link.SetActive(true);
						__instance.m_Links[j].m_Link.transform.position = (array[j] + array[j + 1]) / 2f;
						Vector3 normalized = (array[j + 1] - array[j]).normalized;
						float num3 = 57.29578f * Mathf.Acos(Vector3.Dot(normalized, Vector3.right));
						__instance.m_Links[j].m_Link.transform.rotation = Quaternion.identity;
						__instance.m_Links[j].m_Link.transform.Rotate(0f, 0f, (Vector3.Dot(Vector3.up, normalized) < 0f) ? (-num3) : num3, Space.Self);
						__instance.m_Links[j].m_Link.transform.localScale = new Vector3(Vector3.Distance(array[j], array[j + 1]), 1f, WiderMaterialsMain.RopeWidth.Value);
					}/*
				}
				else if (cable)
				{	
					for (int j = 0; j < num_2; j++)
					{
						__instance.m_Links[j].m_Link.SetActive(true);
						__instance.m_Links[j].m_Link.transform.position = (array[j] + array[j + 1]) / 2f;
						Vector3 normalized = (array[j + 1] - array[j]).normalized;
						float num3 = 57.29578f * Mathf.Acos(Vector3.Dot(normalized, Vector3.right));
						__instance.m_Links[j].m_Link.transform.rotation = Quaternion.identity;
						__instance.m_Links[j].m_Link.transform.Rotate(0f, 0f, (Vector3.Dot(Vector3.up, normalized) < 0f) ? (-num3) : num3, Space.Self);
						__instance.m_Links[j].m_Link.transform.localScale = new Vector3(Vector3.Distance(array[j], array[j + 1]), 1f, WiderMaterialsMain.CableWidth.Value);
					}
				}*/
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
		public static ConfigDefinition RopeWidthDef = new ConfigDefinition("Material Width", "Rope Width");
        public static ConfigEntry<float> RopeWidth;
		public static ConfigDefinition CableWidthDef = new ConfigDefinition("Material Width", "Cable Width");
        public static ConfigEntry<float> CableWidth;
		//public static ConfigDefinition SpringWidthDef = new ConfigDefinition("Material Width", "Spring Width");
        //public static ConfigEntry<float> SpringWidth;

		#pragma warning disable 0169, 0414, 0649
        internal sealed class ConfigurationManagerAttributes
        {
            public bool? ShowRangeAsPercent;
            public System.Action<BepInEx.Configuration.ConfigEntryBase> CustomDrawer;
            public bool? Browsable;
            public string Category;
            public object DefaultValue;
            public bool? HideDefaultButton;
            public bool? HideSettingName;
            public string Description;
            public string DispName;
            public int? Order;
            public bool? ReadOnly;
            public bool? IsAdvanced;
            public System.Func<object, string> ObjToStr;
            public System.Func<string, object> StrToObj;
        }
	}
}
