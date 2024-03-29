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
using System.Collections;
using UnityEngine;

using GUI = KSPe.UI.GUI;
using GUILayout = KSPe.UI.GUILayout;

namespace ShowFPS
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class ShowFPS : MonoBehaviour
    {
        internal static FPSCounter instance;

        void Awake()
        {
            if (instance == null)
            {
                Settings.LoadConfig();
                instance = gameObject.AddComponent<FPSCounter>();
                DontDestroyOnLoad(gameObject);
            }
        }

        void OnDestroy()
        {
            if (instance != null)
            {
                Settings.SaveConfig();
            }
            instance = null;
        }
    }

    /* Code adapted from the example in http://wiki.unity3d.com/index.php?title=FramesPerSecond 
     * written by Annop "Nargus" Prapasapong. */
    [RequireComponent(typeof(GUIText))]
    public class FPSCounter : MonoBehaviour
    {
        new bool enabled = false;

        float curFPS;


        bool drag;

        GUIText guiText;

        void Awake()
        {
            StartCoroutine(FPS());
            guiText = gameObject.GetComponent<GUIText>();
            guiText.enabled = false;
        }

        void OnMouseDown()
        {
            Log.dbg("[ShowFPS: OnMouseDown");
            drag = true;
        }

        void OnMouseUp()
        {
            Log.dbg("[ShowFPS: OnMouseUp");
            drag = false;

            Settings.position_x = x;
            Settings.position_y = y;
            Settings.SaveConfig();
        }

        float x, y;
#if DEBUG
        int cnt = 0;
#endif
        void Update()
        {
#if DEBUG
            if (cnt++ == 100)
            {
                Log.dbg("[ShowFPS]: x, y: {0}, {1}", Settings.position_x, Settings.position_y);
                cnt = 0;
            }
#endif
            if (drag)
            {
                x = Input.mousePosition.x ;
                y = (Screen.height - Input.mousePosition.y);
                Log.dbg("ShowFPS.update, mouse x, y: {0}, {1}", x, y);
                guiText.transform.position = new Vector3(x , 0f);
            }
            else
            {
                x = Settings.position_x;
                y = Settings.position_y;
            }
            if (Settings.pluginToggle.GetKeyDown())
            {
#if true
                if (Input.GetKey(KeyCode.LeftControl)
                        || Input.GetKey(KeyCode.RightControl))
                {
                    if (!enabled)
                    {
                        return;
                    }
                }
                else
                {
                    enabled = !enabled;
                    guiText.enabled = enabled;

                    //guiText.useGUILayout = false; Works only on KSP >= 1.5
                }
#endif
            }
            if (enabled)
            {
                if (drag || fpsPos.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                {
                    bool b = Input.GetMouseButton(0);

                    if (!drag && b)
                    {
                        OnMouseDown();
                    }
                    else
                        if (drag && !b)
                        OnMouseUp();
                    drag = b;

                }
                else
                {
                    if (drag)
                    {
                        Settings.position_x = x;
                        Settings.position_y = y;

                        Settings.SaveConfig();

                        drag = false;
                    }
                }
            }
        }
        void DrawOutline(int offset, Rect r, string t, int strength, GUIStyle style, Color outColor, Color inColor)
        {
            Color backup = style.normal.textColor;
            style.normal.textColor = outColor;

            float yOffset = (r.height) * offset;

            style.normal.textColor = inColor;
            GUI.Label(new Rect(r.x, r.y + yOffset, r.width, r.height), t, style);
            style.normal.textColor = backup;
        }

        private const int LEFT = 10;
        private const int TOP = 20;
        private const int WIDTH = 50;
        private const int HEIGHT = 10;

        private Rect fpsPos = new Rect(LEFT, TOP, WIDTH, HEIGHT);
        GUIStyle timeLabelStyle = null;
        public void OnGUI()
        {
            if (enabled)
            {
                if (timeLabelStyle == null)
                {
                    timeLabelStyle = new GUIStyle(GUI.skin.label);
                }
                Vector2 size = timeLabelStyle.CalcSize(new GUIContent(curFPS.ToString("F2")));

                //fpsPos.Set(x, y , 200f, size.y);
                fpsPos.Set(x, y, size.x, size.y);

                if (curFPS > 60)
                    DrawOutline(0, fpsPos, Math.Round(curFPS).ToString("F0") + " fps", 1, timeLabelStyle, Color.black, Color.white);
                else
                    DrawOutline(0, fpsPos, Math.Round(curFPS, 1).ToString("F1") + " fps", 1, timeLabelStyle, Color.black, Color.white);
            }

        }

        IEnumerator FPS()
        {
            for (; null == Graph.instance; )
                yield return new WaitForSeconds(Settings.frequency);

            for (; ; )
            {
                if (!enabled) // double the wait if not enabled
                {
                    yield return new WaitForSeconds(Settings.frequency);
                }

                // Capture frame-per-second
                int lastFrameCount = Time.frameCount;
                float lastTime = Time.realtimeSinceStartup;
                yield return new WaitForSeconds(Settings.frequency);

                float timeSpan = Time.realtimeSinceStartup - lastTime;
                int frameCount = Time.frameCount - lastFrameCount;

                // Display it
                curFPS = frameCount / timeSpan;
                double symRate = Math.Min(Time.maximumDeltaTime / Time.deltaTime, 1f);
#if DEBUG
                //double symRate = frameCount / (timeSpan / Planetarium.fetch.fixedDeltaTime) ;
                Log.dbg(
                        "curFPS: {0:0.00}, frameCount: {1},  timeSpan: {2}, deltaTime: {3}, maximumDeltaTime: {4}, symRate: {5}, 1/symrate: {6}",
                        curFPS, frameCount, timeSpan, Time.deltaTime, Time.maximumDeltaTime, symRate, (1 / symRate)
                    );
#endif
                Graph.instance.AddFPSValue(curFPS, (float)symRate);
            }
        }
    }
}

