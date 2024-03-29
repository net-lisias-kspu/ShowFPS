/*
	This file is part of ShowFPS /L Unleashed
		© 2024 Lisias T : http://lisias.net <support@lisias.net>
		© 2018-2023 LinuxGuruGamer
		© 2016-2017 Elián Hanisch <lambdae2@gmail.com>

	ShowFPS /L Unleashed is licensed as follows:
		* LGPL 3.0 : https://www.gnu.org/licenses/lgpl-3.0.txt

	ShowFPS /L Unleashed is distributed in the hope that it will be useful, but
	WITHOUT ANY WARRANTY; without even the implied warranty ofMERCHANTABILITY
	or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU Lesser General Public License 3.0
	along with ShowFPS /L Unleashed . If not, see <https://www.gnu.org/licenses/>.

*/
using System;
using UnityEngine;

using KSPe.Util;
using DATA = KSPe.IO.Data<ShowFPS.Startup>;

namespace ShowFPS
{
    public static class Settings
    {
        private static readonly DATA.ConfigNode SETTINGS = DATA.ConfigNode.For("SETTINGS", "settings.cfg");

		internal static bool startVisible;

		private static readonly DictionaryValueList<GameScenes, Vector2> FpsWidgetPositions = new DictionaryValueList<GameScenes, Vector2>();
		internal static Vector2 FpsWidgetPosition => FpsWidgetPositions[HighLogic.LoadedScene];

        internal static int fontSize = 10;

        internal const int GRAPHWIDTH = 600;
        internal const int GRAPHHEIGHT = 256;

        internal static int GraphWidth = GRAPHWIDTH;
        internal static int GraphHeight = GRAPHHEIGHT;
        internal static float winX = 80;
        internal static float winY = 80;

        internal static KeyCode keyToggleWindow;
        internal static KeyCode keyScaleUp;
        internal static KeyCode keyScaleDown;
        internal static KeyBinding pluginToggle;

        internal static bool showPerfectSym;
        internal static bool periodicRescale = false;
        internal static float frequency = 0.5f;
        internal static float alpha = 1f;

        public static void LoadConfig()
        {
			bool newSettings = false;

			if (SETTINGS.IsLoadable)
				SETTINGS.Load();
			else
				newSettings = true;

			KSPe.ConfigNodeWithSteroids settings = SETTINGS.NodeWithSteroids;

            startVisible = settings.GetValue<bool>("startVisible", false);

			// These values are based on screen size
			FpsWidgetPositions.Clear();
			if (settings.HasNode("fpswidget_positions"))
			{
				KSPe.ConfigNodeWithSteroids cns = KSPe.ConfigNodeWithSteroids.from(settings.GetNode("fpswidget_positions"));
				foreach (GameScenes gs in Enum.GetValues(typeof(GameScenes)))
					FpsWidgetPositions[gs] = cns.GetValue<Vector2>(gs.ToString(), new Vector2(50, 50));
			}
			else
				foreach (GameScenes gs in Enum.GetValues(typeof(GameScenes)))
					FpsWidgetPositions[gs] = new Vector2(50, 50);

            fontSize = settings.GetValue<int>("fontSize", 10);
			keyToggleWindow = GetKeyCode(settings, "keyToggleWindow", KeyCode.KeypadMultiply);
			keyScaleUp = GetKeyCode(settings, "keyScaleUp", KeyCode.KeypadPlus);
			keyScaleDown = GetKeyCode(settings, "keyScaleDown", KeyCode.KeypadMinus);

			pluginToggle = new KeyBinding(KeyCode.F8);
			if (settings.HasNode("plugin_key"))
				pluginToggle.Load(settings.GetNode("plugin_key"));

            showPerfectSym = settings.GetValue<bool>("showPerfectSym", false);
            periodicRescale = settings.GetValue<bool>("periodicRescale", false);
            frequency = settings.GetValue<float>("frequency", 0.5f);
            alpha = settings.GetValue<float>("alpha", 1f);
            winX = settings.GetValue<float>("winX", 80f);
            winY = settings.GetValue<float>("winY", 80f);

            if (newSettings) SaveConfig();
        }

		internal static void UpdateFpsWidgetPosition(float x, float y) => FpsWidgetPositions[HighLogic.LoadedScene] = new Vector2(x, y);

		private static KeyCode GetKeyCode(KSPe.ConfigNodeWithSteroids cn, string name, KeyCode defaultValue)
		{
			string stringValue = "";
			if (!cn.TryGetValue(name, ref stringValue)) return defaultValue;
			try		{ return (KeyCode)Enum.Parse(typeof(KeyCode), stringValue); }
			catch	{ return defaultValue; }
		}

        public static void SaveConfig()
        {
			ConfigNode settings = SETTINGS.Node;

            settings.SetValue("startVisible", startVisible, false);

			{
				ConfigNode cn = settings.HasNode("fpswidget_positions") ? settings.GetNode("fpswidget_positions") : settings.AddNode("fpswidget_positions");
				foreach (GameScenes gs in FpsWidgetPositions.Keys)
					cn.SetValue(gs.ToString(), FpsWidgetPositions[gs], true);
			}

            settings.SetValue("fontSize", fontSize, true);

            settings.SetValue("keyToggleWindow", keyToggleWindow.ToString(), true);
            settings.SetValue("keyScaleUp", keyScaleUp.ToString(), true);
            settings.SetValue("keyScaleDown", keyScaleDown.ToString(), true);

			{
				if (settings.HasNode("plugin_key")) settings.RemoveNode("plugin_key");
				ConfigNode cn = settings.AddNode("plugin_key");
				pluginToggle.Save(cn);
			}

            settings.SetValue("showPerfectSym", showPerfectSym, true);
            settings.SetValue("periodicRescale", periodicRescale, true);
            settings.SetValue("frequency", frequency, true);
            settings.SetValue("alpha", alpha, true);
            settings.SetValue("winX", winX, true);
            settings.SetValue("winY", winY, true);

			SETTINGS.Save();
        }
    }

}
