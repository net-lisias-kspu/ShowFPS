//  ShowFPS.cs
//
//  Author:
//       Elián Hanisch <lambdae2@gmail.com>
//
//  Copyright (c) 2013-2016 Elián Hanisch
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ShowFPS
{
    [KSPAddon(KSPAddon.Startup.EveryScene, true)]
    public class ShowFPS : MonoBehaviour
    {
        static FPSCounter instance;
        Text guiText;

        void Awake ()
        {
            if (instance == null) {
                Settings.LoadConfig ();
                instance = gameObject.AddComponent<FPSCounter> ();
                guiText = gameObject.GetComponent<Text> ();
                guiText.transform.position = new Vector3 (Settings.position_x, Settings.position_y, 0f);
                DontDestroyOnLoad (gameObject);
            }
        }

        void OnDestroy ()
        {
            if (instance != null) {
                Settings.SaveConfig ();
            }
        }
    }

    /* Code adapted from the example in http://wiki.unity3d.com/index.php?title=FramesPerSecond 
     * written by Annop "Nargus" Prapasapong. */
    [RequireComponent(typeof(Text))]
    public class FPSCounter : MonoBehaviour
    {
        new bool enabled = false;
        float frequency = 0.5f;

        float curFPS;
        float minFPS;

        bool benchmark = false;
        float benchStartTime;
        float benchTime;
        float benchFrames;
        float benchStartFrames;

        bool drag;
        float offset_x;
        float offset_y;

        Text guiText;
     
        void Awake ()
        {
            StartCoroutine (FPS ());
            guiText = gameObject.GetComponent<Text> ();
            guiText.enabled = false;
        }

        void OnMouseDown ()
        {
            drag = true;
            offset_x = guiText.transform.position.x - Input.mousePosition.x / Screen.width;
            offset_y = guiText.transform.position.y - Input.mousePosition.y / Screen.height;
        }

        void OnMouseUp ()
        {
            drag = false;
            Settings.position_x = guiText.transform.position.x;
            Settings.position_y = guiText.transform.position.y;
        }

        void resetBenchmark ()
        {
            minFPS = curFPS;
            benchStartTime = Time.realtimeSinceStartup;
            benchStartFrames = Time.frameCount;
        }
        float x, y;

        void Update ()
        {
            if (drag) {
                 x = Input.mousePosition.x / Screen.width;
                 y = Input.mousePosition.y / Screen.height;
                guiText.transform.position = new Vector3 (x + offset_x, y + offset_y, 0f);
            }
            if (PluginKeys.PLUGIN_TOGGLE.GetKeyDown()) {
                Debug.Log("ShowFPS, PLUGIN_TOGGLE");
                if (Input.GetKey(KeyCode.LeftControl) 
                        || Input.GetKey(KeyCode.RightControl)) {
                    if (!enabled) {
                        return;
                    }
                    benchmark = !benchmark;
                    if (benchmark) {
                        resetBenchmark ();
                    }
                } else {
                    enabled = !enabled;
                    guiText.enabled = enabled;

                    guiText.useGUILayout = false;
                    if (!enabled) {
                        benchmark = false;
                    }
                }
            }
        }
        void DrawOutline(Rect r, string t, int strength, GUIStyle style, Color outColor, Color inColor)
        {
            Color backup = style.normal.textColor;
            style.normal.textColor = outColor;
            for (int i = -strength; i <= strength; i++)
            {
                GUI.Label(new Rect(r.x - strength, r.y + i, r.width, r.height), t, style);
                GUI.Label(new Rect(r.x + strength, r.y + i, r.width, r.height), t, style);
            }
            for (int i = -strength + 1; i <= strength - 1; i++)
            {
                GUI.Label(new Rect(r.x + i, r.y - strength, r.width, r.height), t, style);
                GUI.Label(new Rect(r.x + i, r.y + strength, r.width, r.height), t, style);
            }
            style.normal.textColor = inColor;
            GUI.Label(r, t, style);
            style.normal.textColor = backup;
        }
        [Persistent]
        int timeSize = 10;
        private Rect fpsPos = new Rect(0,0,10,10);
        GUIStyle timeLabelStyle = null;
        public void OnGUI()
        {
            if (enabled)
            {
                if (timeLabelStyle == null)
                {
                    timeLabelStyle = new GUIStyle(GUI.skin.label);
                    //gametimeX = Mathf.Clamp(gametimeX, 0, Screen.width);
                    //gametimeY = Mathf.Clamp(gametimeY, 0, Screen.height);
                    timeLabelStyle.fontSize = timeSize;
                }
                Vector2 size = timeLabelStyle.CalcSize(new GUIContent(curFPS.ToString()));
                fpsPos.Set(x * Screen.width + offset_x, y * Screen.height + offset_y, 200f, size.y);
                DrawOutline(fpsPos, curFPS.ToString(), 1, timeLabelStyle, Color.black, Color.white);
                Debug.Log("ShowFPS.OnGUI, curFPS: " + curFPS + ", fpsPos: " + fpsPos + ",  size.y: " + size.y);
            }

        }

        IEnumerator FPS ()
        {
            for (;;) {
                if (!enabled) {
                    yield return new WaitForSeconds (frequency);
                }

                // Capture frame-per-second
                int lastFrameCount = Time.frameCount;
                float lastTime = Time.realtimeSinceStartup;
                yield return new WaitForSeconds (frequency);
                float timeSpan = Time.realtimeSinceStartup - lastTime;
                int frameCount = Time.frameCount - lastFrameCount;
     
                // Display it
                curFPS = frameCount / timeSpan;
                if (!benchmark) {
                    guiText.text = String.Format("{0:F1} fps", curFPS);
                } else {
                    benchTime = Time.realtimeSinceStartup - benchStartTime;
                    benchFrames = Time.frameCount - benchStartFrames;
                    if (curFPS < minFPS) {
                        minFPS = curFPS;
                    }
                    guiText.text = String.Format(
                        "{0:F1} fps\nbenchmark\navg: {1:F1}\nmin: {2:F1}",
                        curFPS,
                        benchFrames / benchTime,
                        minFPS);
                }
            }
        }
    }

    public static class Settings
    {
        const string configPath = "GameData/ShowFPS/settings.cfg";
        static string configAbsolutePath;
        static ConfigNode settings;

        public static float position_x;
        public static float position_y;

        public static void LoadConfig ()
        {
            configAbsolutePath = Path.Combine (KSPUtil.ApplicationRootPath, configPath);
            settings = ConfigNode.Load (configAbsolutePath) ?? new ConfigNode ();

            position_x = GetValue ("position_x", 0.93f);
            position_y = GetValue ("position_y", 0.93f);

            PluginKeys.Setup ();
        }

        public static void SaveConfig ()
        {
            SetValue ("position_x", position_x);
            SetValue ("position_y", position_y);
            SetValue ("plugin_key", PluginKeys.PLUGIN_TOGGLE.primary.ToString());

            settings.Save (configAbsolutePath);
        }

        public static void SetValue (string key, object value)
        {
            if (settings.HasValue(key)) {
                settings.RemoveValue(key);
            }
            settings.AddValue(key, value);
        }

        public static int GetValue (string key, int defaultValue)
        {
            int value;
            return int.TryParse (settings.GetValue (key), out value) ? value : defaultValue;
        }

        public static bool GetValue (string key, bool defaultValue)
        {
            bool value;
            return bool.TryParse (settings.GetValue (key), out value) ? value : defaultValue;
        }

        public static float GetValue (string key, float defaultValue)
        {
            float value;
            return float.TryParse (settings.GetValue (key), out value) ? value : defaultValue;
        }

        public static string GetValue (string key, string defaultValue)
        {
            string value = settings.GetValue(key);
            return String.IsNullOrEmpty (value) ? defaultValue : value;
        }
    }

    public static class PluginKeys 
    {
        public static KeyBinding PLUGIN_TOGGLE = new KeyBinding (KeyCode.F8);

        public static void Setup ()
        {
            PLUGIN_TOGGLE = new KeyBinding(Parse(Settings.GetValue ("plugin_key", PLUGIN_TOGGLE.primary.ToString())));
        }

        public static KeyCode Parse(string value) {
            return (KeyCode)Enum.Parse (typeof(KeyCode), value);
        }
    }
}

