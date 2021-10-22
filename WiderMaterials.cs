using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Common.Extension;
using UnityEngine;
using PolyPhysics;
using PolyTechFramework;

namespace WiderMaterials
{
	[BepInPlugin("polytech.widerMaterials", "Wider Materials", "3.0.0")]
	[BepInProcess("Poly Bridge 2")]
	[BepInDependency(PolyTechMain.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
	public class WiderMaterialsMain : PolyTechMod
	{
		public void Awake()
		{
			int order = 0;
			base.Config.Bind<bool>(ModEnabledDef, true, new ConfigDescription("Enable Mod", null, new ConfigurationManagerAttributes { Order = order }));
			WiderMaterialsMain.ModEnabled = (ConfigEntry<bool>)base.Config[WiderMaterialsMain.ModEnabledDef];
			ModEnabled.SettingChanged += onEnableDisable;
			order--;
			base.Config.Bind<bool>(WiderMaterialsMain.SetToPresetsDef, false, new ConfigDescription("Set all width values to their presets", null, new ConfigurationManagerAttributes { Order = order, CustomDrawer = SetAllToPresetButton, HideDefaultButton = true }));
            WiderMaterialsMain.SetToPresets = (ConfigEntry<bool>)base.Config[WiderMaterialsMain.SetToPresetsDef];
			order--;
			base.Config.Bind<bool>(WiderMaterialsMain.SetPresetsToCurrentDef, false, new ConfigDescription("Set all presets to the current width values", null, new ConfigurationManagerAttributes { Order = order, CustomDrawer = SetAllPresetsToCurrentButton, HideDefaultButton = true }));
            WiderMaterialsMain.SetPresetsToCurrent = (ConfigEntry<bool>)base.Config[WiderMaterialsMain.SetPresetsToCurrentDef];
			order--;
			base.Config.Bind<float>(WiderMaterialsMain.RoadWidthDef, 1f, new ConfigDescription("Road Width", null, new ConfigurationManagerAttributes { Order = order, CustomDrawer = ValueWithSetPresetButton, HideDefaultButton = true }));
            WiderMaterialsMain.RoadWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.RoadWidthDef];
			order--;
				base.Config.Bind<float>(WiderMaterialsMain.RoadWidthPresetDef, 1f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true, Order = order }));
            	WiderMaterialsMain.RoadWidthPreset = (ConfigEntry<float>)base.Config[WiderMaterialsMain.RoadWidthPresetDef];
				order--;
			base.Config.Bind<float>(WiderMaterialsMain.WoodWidthDef, 1f, new ConfigDescription("Wood Width", null, new ConfigurationManagerAttributes { Order = order, CustomDrawer = ValueWithSetPresetButton, HideDefaultButton = true }));
            WiderMaterialsMain.WoodWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.WoodWidthDef];
			order--;
				base.Config.Bind<float>(WiderMaterialsMain.WoodWidthPresetDef, 1f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true, Order = order }));
            	WiderMaterialsMain.WoodWidthPreset = (ConfigEntry<float>)base.Config[WiderMaterialsMain.WoodWidthPresetDef];
				order--;
			base.Config.Bind<float>(WiderMaterialsMain.SteelWidthDef, 1f, new ConfigDescription("Steel Width", null, new ConfigurationManagerAttributes { Order = order, CustomDrawer = ValueWithSetPresetButton, HideDefaultButton = true }));
            WiderMaterialsMain.SteelWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.SteelWidthDef];
			order--;
				base.Config.Bind<float>(WiderMaterialsMain.SteelWidthPresetDef, 1f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true, Order = order }));
            	WiderMaterialsMain.SteelWidthPreset = (ConfigEntry<float>)base.Config[WiderMaterialsMain.SteelWidthPresetDef];
				order--;
			base.Config.Bind<float>(WiderMaterialsMain.HydraulicWidthDef, 1f, new ConfigDescription("Hydraulic Width", null, new ConfigurationManagerAttributes { Order = order, CustomDrawer = ValueWithSetPresetButton, HideDefaultButton = true }));
            WiderMaterialsMain.HydraulicWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.HydraulicWidthDef];
			order--;
				base.Config.Bind<float>(WiderMaterialsMain.HydraulicWidthPresetDef, 1f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true, Order = order }));
            	WiderMaterialsMain.HydraulicWidthPreset = (ConfigEntry<float>)base.Config[WiderMaterialsMain.HydraulicWidthPresetDef];
				order--;
			base.Config.Bind<float>(WiderMaterialsMain.RopeWidthDef, 1f, new ConfigDescription("Rope/Cable Width", null, new ConfigurationManagerAttributes { Order = order, CustomDrawer = ValueWithSetPresetButton, HideDefaultButton = true }));
           	WiderMaterialsMain.RopeWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.RopeWidthDef];
			order--;
				base.Config.Bind<float>(WiderMaterialsMain.RopeWidthPresetDef, 1f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true, Order = order }));
           		WiderMaterialsMain.RopeWidthPreset = (ConfigEntry<float>)base.Config[WiderMaterialsMain.RopeWidthPresetDef];
				order--;
			//base.Config.Bind<float>(WiderMaterialsMain.CableWidthDef, 1f, new ConfigDescription("Cable Width (1f is the default value)", null, new ConfigurationManagerAttributes { Order = order }));
            //WiderMaterialsMain.CableWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.CableWidthDef];
			//order--;
			base.Config.Bind<float>(WiderMaterialsMain.SpringWidthDef, 1f, new ConfigDescription("Spring Width", null, new ConfigurationManagerAttributes { Order = order, CustomDrawer = ValueWithSetPresetButton, HideDefaultButton = true }));
            WiderMaterialsMain.SpringWidth = (ConfigEntry<float>)base.Config[WiderMaterialsMain.SpringWidthDef];
			order--;
				base.Config.Bind<float>(WiderMaterialsMain.SpringWidthPresetDef, 1f, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true, Order = order }));
            	WiderMaterialsMain.SpringWidthPreset = (ConfigEntry<float>)base.Config[WiderMaterialsMain.SpringWidthPresetDef];
			roadval = RoadWidth.Value.ToString();
			woodval = WoodWidth.Value.ToString();
			steelval = SteelWidth.Value.ToString();
			hydroval = HydraulicWidth.Value.ToString();
			ropeval = RopeWidth.Value.ToString();
			springval = SpringWidth.Value.ToString();

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

		//Patches all beam-based materials (roads, wood, steel, hydraulics)
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
			}
		}

		//Patches all rope-based materials (ropes and cables), both are controlled by a single value because I can't figure out how to make it tell them apart
		[HarmonyPatch(typeof(BridgeRope))]
		[HarmonyPatch("UpdateLinks")]
		public class patchRopes
		{
			public static void Postfix(BridgeRope __instance/*;l, int materialtype*/)
			{
				if(!ModEnabled.Value) return;
				Vector3[] array = __instance.m_PhysicsRope.ComputeNodePositions();
				int num = array.Length - 1;
				int num2 = Mathf.Min(num, __instance.m_Links.Count);
				//if(!ModEnabled.Value || materialtype == 0) return;
				//if (materialtype == 1 && ModEnabled.Value)
				//{
					for (int j = 0; j < num2; j++)
					{
						__instance.m_Links[j].m_Link.transform.localScale = new Vector3(Vector3.Distance(array[j], array[j + 1]), 1f, WiderMaterialsMain.RopeWidth.Value);
					}
				//}
				/*else if (materialtype == 2 && ModEnabled.Value)
				{	
					for (int j = 0; j < num2; j++)
					{
						__instance.m_Links[j].m_Link.transform.localScale = new Vector3(Vector3.Distance(array[j], array[j + 1]), 1f, WiderMaterialsMain.CableWidth.Value);
					}
				}*/
			}
		}

		//Patches springs
		[HarmonyPatch(typeof(BridgeSpring))]
		[HarmonyPatch("SetPositionsOfLinks")]
		public class SetLinkPosition
		{
			public static void Postfix(BridgeSpring __instance, BridgeSpringLink link, Vector3 offset)
			{
				Vector3 newoffset;
				newoffset = new Vector3 (offset.x, offset.y, offset.z * WiderMaterialsMain.SpringWidth.Value);
				//offset.z = offset.z * WiderMaterialsMain.SpringWidth.Value;
				if(!ModEnabled.Value) return;
				Vector2 vector = __instance.m_ParentEdge.m_JointA.transform.position;
				Vector2 vector2 = __instance.m_ParentEdge.m_JointB.transform.position;
				if (__instance.m_ParentEdge.m_PhysicsEdge && __instance.m_ParentEdge.m_PhysicsEdge.handle)
				{
					Edge physicsEdge = __instance.m_ParentEdge.m_PhysicsEdge;
					vector = physicsEdge.node0.smoothPos;
					vector2 = physicsEdge.node1.smoothPos;
				}
				link.m_meshGenerator.SetPositionFromTo(vector, vector2, Vector3.forward, newoffset);	
			}
		}

		public static ConfigDefinition ModEnabledDef = new ConfigDefinition("Enable/Disable", "Enabled");
        public static ConfigEntry<bool> ModEnabled;
		public static ConfigDefinition SetToPresetsDef = new ConfigDefinition("Material Width", "Load Presets");
        public static ConfigEntry<bool> SetToPresets;
		public static ConfigDefinition SetPresetsToCurrentDef = new ConfigDefinition("Material Width", "Set Presets To Current Values");
        public static ConfigEntry<bool> SetPresetsToCurrent;
		public static ConfigDefinition RoadWidthDef = new ConfigDefinition("Material Width", "Road Width");
        public static ConfigEntry<float> RoadWidth;
		public static string roadval;
		public static ConfigDefinition RoadWidthPresetDef = new ConfigDefinition("Material Width", "Road Width Preset");
        public static ConfigEntry<float> RoadWidthPreset;
		public static ConfigDefinition WoodWidthDef = new ConfigDefinition("Material Width", "Wood Width");
        public static ConfigEntry<float> WoodWidth;
		public static string woodval;
		public static ConfigDefinition WoodWidthPresetDef = new ConfigDefinition("Material Width", "Wood Width Preset");
		public static ConfigEntry<float> WoodWidthPreset;
		public static ConfigDefinition SteelWidthDef = new ConfigDefinition("Material Width", "Steel Width");
        public static ConfigEntry<float> SteelWidth;
		public static string steelval;
		public static ConfigDefinition SteelWidthPresetDef = new ConfigDefinition("Material Width", "Steel Width Preset");
		public static ConfigEntry<float> SteelWidthPreset;
		public static ConfigDefinition HydraulicWidthDef = new ConfigDefinition("Material Width", "Hydraulic Width");
        public static ConfigEntry<float> HydraulicWidth;
		public static string hydroval;
		public static ConfigDefinition HydraulicWidthPresetDef = new ConfigDefinition("Material Width", "Hydraulic Width Preset");
		public static ConfigEntry<float> HydraulicWidthPreset;
		public static ConfigDefinition RopeWidthDef = new ConfigDefinition("Material Width", "Rope/Cable Width");
        public static ConfigEntry<float> RopeWidth;
		public static string ropeval;
		public static ConfigDefinition RopeWidthPresetDef = new ConfigDefinition("Material Width", "Rope/Cable Width Preset");
		public static ConfigEntry<float> RopeWidthPreset;
		//public static ConfigDefinition CableWidthDef = new ConfigDefinition("Material Width", "Cable Width");
        //public static ConfigEntry<float> CableWidth;
		public static ConfigDefinition SpringWidthDef = new ConfigDefinition("Material Width", "Spring Width");
        public static ConfigEntry<float> SpringWidth;
		public static string springval;
		public static ConfigDefinition SpringWidthPresetDef = new ConfigDefinition("Material Width", "Spring Width Preset");
		public static ConfigEntry<float> SpringWidthPreset;
		public static ConfigDefinition DefaultWidthForAllDef = new ConfigDefinition("Material Width", "Default Width Value for All Materials");
		public static ConfigEntry<float> DefaultWidthForAll;

		static void SetAllToPresetButton(BepInEx.Configuration.ConfigEntryBase entry)
		{
    		// Make sure to use GUILayout.ExpandWidth(true) to use all available space
			if (GUILayout.Button("Load All Presets", GUILayout.ExpandWidth(true)))
			{
				roadval = RoadWidthPreset.Value.ToString();
				woodval = WoodWidthPreset.Value.ToString();
				steelval = SteelWidthPreset.Value.ToString();
				hydroval = HydraulicWidthPreset.Value.ToString();
				ropeval = RopeWidthPreset.Value.ToString();
				springval = SpringWidthPreset.Value.ToString();
				updateConfig();
				PopUpMessage.DisplayOkOnly("Presets loaded successfully", null);
			}
		}

		static void SetAllPresetsToCurrentButton(BepInEx.Configuration.ConfigEntryBase entry)
		{
    		// Make sure to use GUILayout.ExpandWidth(true) to use all available space
			if (GUILayout.Button("Set All Presets To Current Width Values", GUILayout.ExpandWidth(true)))
			{
				if (RoadWidthPreset.Value == CatchParseError(roadval) && WoodWidthPreset.Value == CatchParseError(woodval) && SteelWidthPreset.Value == CatchParseError(steelval) && HydraulicWidthPreset.Value == CatchParseError(hydroval) && RopeWidthPreset.Value == CatchParseError(ropeval) && SpringWidthPreset.Value == CatchParseError(springval))
				{
					PopUpWarning.Display("No differences between old preset and new preset. Nothing has changed.");
					return;
				}
				PopUpMessage.Display("Are you sure you want to change all of the presets to the current width values? This cannot be undone.", delegate()
				{
					RoadWidthPreset.Value = CatchParseError(roadval);
					WoodWidthPreset.Value = CatchParseError(woodval);
					SteelWidthPreset.Value = CatchParseError(steelval);
					HydraulicWidthPreset.Value = CatchParseError(hydroval);
					RopeWidthPreset.Value = CatchParseError(ropeval);
					SpringWidthPreset.Value = CatchParseError(springval);
					updateConfig();
					PopUpMessage.DisplayOkOnly("Presets set successfully.", null);
				});
			}
		}

		private static void updateConfig()
		{
			if (roadval == null) roadval = "1";
			WiderMaterialsMain.RoadWidth.Value = CatchParseError(sanitizeValues(roadval));
			if (woodval == null) woodval = "1";
			WiderMaterialsMain.WoodWidth.Value = CatchParseError(sanitizeValues(woodval));
			if (steelval == null) steelval = "1";
			WiderMaterialsMain.SteelWidth.Value = CatchParseError(sanitizeValues(steelval));
			if (hydroval == null) hydroval = "1";
			WiderMaterialsMain.HydraulicWidth.Value = CatchParseError(sanitizeValues(hydroval));
			if (ropeval == null) ropeval = "1";
			WiderMaterialsMain.RopeWidth.Value = CatchParseError(sanitizeValues(ropeval));
			if (springval == null) springval = "1";
			WiderMaterialsMain.SpringWidth.Value = CatchParseError(sanitizeValues(springval));
		}

		private static string sanitizeValues(string raw)
		{
			string pattern = @"\.";
			Regex rgx = new Regex(pattern);
			string allnumber = rgx.Replace(raw, "", -1, raw.IndexOf(".") + 1);
			allnumber = Regex.Replace(allnumber, @"[^\d\.]", "");
			if (allnumber.IsNullOrWhiteSpace()) allnumber = "0";
			if (allnumber.IndexOf(".") == 0)
			{
				allnumber = "0" + allnumber;
			}
			if (allnumber.LastIndexOf(".") == (allnumber.Length - 1))
			{
				allnumber = allnumber + "0";
			}
			if (allnumber.IndexOf(".") == (allnumber.Length - 1))
				{
					allnumber = allnumber + "0";
				}
				if ((allnumber.Count(f => f == '.') <= 1) && (allnumber.IndexOfAny("0123456789".ToCharArray()) == 0) && (allnumber.LastIndexOfAny("0123456789".ToCharArray()) == allnumber.Length - 1))
				{
					return allnumber;
				}
				else
				{
					if (!(allnumber.Count(f => f == '.') <= 1))
					{
						PopUpWarning.Display("1 or less dots check error 1");
					}
					if (!(allnumber.IndexOfAny("0123456789".ToCharArray()) == 0))
					{
						PopUpWarning.Display("First character is digit check error 1");
					}
					if (!(allnumber.LastIndexOfAny("0123456789".ToCharArray()) == allnumber.Length - 1))
					{
						PopUpWarning.Display("Last character is digit check error 1");
					}
					return "0";
				}
		}

		private static float CatchParseError(string input)
		{
			if (float.TryParse(input, out float output))
			{
				return output;
			}
			else
			{
				PopUpWarning.Display("Int failed to parse");
				return 0;
			}
		}

		static void ValueWithSetPresetButton(BepInEx.Configuration.ConfigEntryBase entry)
		{
			GUILayout.BeginHorizontal();
			//Manually adding the fields
			if (entry == RoadWidth)
			{
				roadval = GUILayout.TextField(roadval, new GUILayoutOption[]{GUILayout.ExpandWidth(true)});
				roadval = sanitizeValues(roadval);
			}
			else if (entry == WoodWidth)
			{
				woodval = GUILayout.TextField(woodval, new GUILayoutOption[]{GUILayout.ExpandWidth(true)});
				woodval = sanitizeValues(woodval);
			}
			else if (entry == SteelWidth)
			{
				steelval = GUILayout.TextField(steelval, new GUILayoutOption[]{GUILayout.ExpandWidth(true)});
				steelval = sanitizeValues(steelval);
			}
			else if (entry == HydraulicWidth)
			{
				hydroval = GUILayout.TextField(hydroval, new GUILayoutOption[]{GUILayout.ExpandWidth(true)});
				hydroval = sanitizeValues(hydroval);
			}
			else if (entry == RopeWidth)
			{
				ropeval = GUILayout.TextField(ropeval, new GUILayoutOption[]{GUILayout.ExpandWidth(true)});
				ropeval = sanitizeValues(ropeval);
			}
			else if (entry == SpringWidth)
			{
				springval = GUILayout.TextField(springval, new GUILayoutOption[]{GUILayout.ExpandWidth(true)});
				springval = sanitizeValues(springval);
			}
			updateConfig();

			//Reset button
			if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
			{
				if (entry == RoadWidth) roadval = "1";
				else if (entry == WoodWidth) woodval = "1";
				else if (entry == SteelWidth) steelval = "1";
				else if (entry == HydraulicWidth) hydroval = "1";
				else if (entry == RopeWidth) ropeval = "1";
				else if (entry == SpringWidth) springval = "1";
				updateConfig();
			}

			//Load Preset button
			if (GUILayout.Button("Load Preset", GUILayout.ExpandWidth(false)))
			{
				if (entry == RoadWidth) roadval = RoadWidthPreset.Value.ToString();
				else if (entry == WoodWidth) woodval = WoodWidthPreset.Value.ToString();
				else if (entry == SteelWidth) steelval = SteelWidthPreset.Value.ToString();
				else if (entry == HydraulicWidth) hydroval = HydraulicWidthPreset.Value.ToString();
				else if (entry == RopeWidth) ropeval = RopeWidthPreset.Value.ToString();
				else if (entry == SpringWidth) springval = SpringWidthPreset.Value.ToString();
				updateConfig();
				GameUI.ShowMessage(ScreenMessageLocation.TOP_LEFT, "Loaded preset", 5f);
			}

			//Set Preset button
			if (GUILayout.Button("Set Preset", GUILayout.ExpandWidth(false)))
			{
				if (entry == RoadWidth) RoadWidthPreset.Value = CatchParseError(roadval);
				else if (entry == WoodWidth) WoodWidthPreset.Value = CatchParseError(woodval);
				else if (entry == SteelWidth) SteelWidthPreset.Value = CatchParseError(steelval);
				else if (entry == HydraulicWidth) HydraulicWidthPreset.Value = CatchParseError(hydroval);
				else if (entry == RopeWidth) RopeWidthPreset.Value = CatchParseError(ropeval);
				else if (entry == SpringWidth) SpringWidthPreset.Value = CatchParseError(springval);
				updateConfig();
				GameUI.ShowMessage(ScreenMessageLocation.TOP_LEFT, "Updated preset", 5f);
			}
			GUILayout.EndHorizontal();
		}

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
