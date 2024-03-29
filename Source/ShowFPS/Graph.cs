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
/*
 Copyright (c) 2016 Gerry Iles (Padishar)

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
*/

using System;
using UnityEngine;

using GUI = KSPe.UI.GUI;
using GUILayout = KSPe.UI.GUILayout;


namespace ShowFPS
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class Graph : MonoBehaviour
    {
        internal static Graph instance;

        const int GraphX = 10;
        const int GraphY = 36;
        const int LabelX = 10;
        const int LabelY = 18;
        const int LabelHeight = 20;
        int LabelWidth = Settings.GRAPHWIDTH;
        int WndWidth = Settings.GRAPHWIDTH + 8;
        int WndHeight = Settings.GRAPHHEIGHT + 42;

        Rect windowPos;
        Rect windowDragRect;
        Rect helpWinPos;
        int windowId = 0;
        string windowTitle;
        bool showUI = false;
        bool showHelp = false;
        Rect labelRect;
        Rect graphRect;

        const int FPS = 0;
        const int FPS_AVG = 1;
        const int SYMRATE = 2;

        internal float[,] fpsValues;

        Texture2D texGraph;

        int valIndex = 0;           // The current index into the values array
        int lastRendered = 0;       // The last index of the values array that has been rendered into the texture

        string guiStr;              // The string at the top of the window (only updated when required)


        bool fullUpdate = true;     // Flag to force re-render of entire texture (e.g. when changing scale)

        int scaleIndex = 4;         // Index of the current vertical scale
        static readonly float[] valCycle = new float[] { 5, 10, 15, 30, 40, 50, 60, 70, 80, 90, 100, 120, 150, 200 };
        static readonly int numScales = valCycle.Length;


        Color[] blackLine;
        Color[] redLine;
        Color[] yellowLine;
        Color[] greenLine;
        Color[] blueLine;
        Color[] greyLine;

        static internal GUIStyle labelStyle = null;

        private ResizeHandle resizeHandle;

        static GUIStyle resizeBoxStyle;

        static GUIStyle greenFont, redFont, redButtonFont;

        float? movingAvg;
        float curFPS;
        const float SYM_MULT = 0.5f;

        private void InitGui()
        {
            labelStyle = new GUIStyle(GUI.skin.label);

            // resize button
            resizeBoxStyle = new GUIStyle(GUI.skin.box);
            resizeBoxStyle.fontSize = 10;
            resizeBoxStyle.normal.textColor = XKCDColors.LightGrey;

            greenFont = new GUIStyle(GUI.skin.label);
            redFont = new GUIStyle(GUI.skin.label);
            redButtonFont = new GUIStyle(GUI.skin.button);
            redFont.fontStyle = FontStyle.Bold;
            redFont.normal.textColor = Color.red;

            redButtonFont.fontStyle = FontStyle.Bold;
            redButtonFont.normal.textColor =
                redButtonFont.hover.textColor = Color.red;
            greenFont.normal.textColor = Color.green;

            GameEvents.onShowUI.Add(ShowGUI);
            GameEvents.onHideUI.Add(HideGUI);
        }

        bool isKSPGUIActive = true;
        void ShowGUI()
        {
            isKSPGUIActive = true;
        }

        void HideGUI()
        {
            isKSPGUIActive = false;
        }



        private void InitGraphWindow()
        {
            LabelWidth = Settings.GraphWidth;
            WndWidth = Settings.GraphWidth + 8;
            WndHeight = Settings.GraphHeight + 42;

            windowPos.Set(Settings.winX, Settings.winY, WndWidth, WndHeight);
            windowDragRect.Set(0, 0, WndWidth, WndHeight);
            labelRect.Set(LabelX, LabelY, LabelWidth, LabelHeight);
            graphRect.Set(GraphX, GraphY, Settings.GraphWidth, Settings.GraphHeight);

            texGraph = new Texture2D(Settings.GraphWidth, Settings.GraphHeight, TextureFormat.ARGB32, false);

            yellowLine = new Color[Settings.GraphHeight];
            redLine = new Color[Settings.GraphHeight];
            greenLine = new Color[Settings.GraphHeight];
            blueLine = new Color[Settings.GraphHeight];
            greyLine = new Color[Settings.GraphHeight];
            blackLine = new Color[Settings.GraphHeight];
            for (int i = 0; i < blackLine.Length; i++)
            {
                blackLine[i] = Color.black;
                blackLine[i].a = Settings.alpha;

                yellowLine[i] = Color.yellow;
                redLine[i] = Color.red;
                greenLine[i] = Color.green;
                blueLine[i] = Color.blue;
                greyLine[i] = Color.grey;
            }

            for (int i = 0; i < Settings.GraphWidth; i++)
                texGraph.SetPixels(i, 0, 1, Settings.GraphHeight, blackLine);

            this.resizeHandle = new ResizeHandle();

            RedrawGraph();
        }

        internal void Toggle(bool newValue)
        {
			this.showUI = newValue;

            if (this.showUI)
            {
                this.InitGraphWindow();
            }
            else
            {
                this.showHelp = false;
                SaveWinSettings();
            }
        }

        void Awake()
        {
            DontDestroyOnLoad(this);

            instance = this;

            windowId = Guid.NewGuid().GetHashCode();
            windowTitle = "Show FPS";

            helpWinPos.Set(40, 40, 500, 100);

            showUI = Settings.startVisible;

            // Force a full update of the graph texture
            fullUpdate = true;

            fpsValues = new float[Screen.width, 3];
            Array.Clear(fpsValues, 0, Settings.GraphWidth * 3);

            this.InitGraphWindow();
        }


        void Start()
        {
            ToolbarController.Instance.Create(this);
            areaStyle = new GUIStyle(HighLogic.Skin.textArea);
            areaStyle.richText = true;
        }

        internal void AddFPSValue(float fps, float symRate)
        {
            if (movingAvg == null)
                movingAvg = fps;
            else
                movingAvg = movingAvg * .9f + fps * .1f;

            curFPS = fpsValues[valIndex, FPS] = fps;
            fpsValues[valIndex, SYMRATE] = symRate * SYM_MULT;

            if (!resizeHandle.resizing)
            {
                fpsValues[valIndex, FPS_AVG] = (float)movingAvg;

                valIndex = (valIndex + 1) % Settings.GraphWidth;
                if (valIndex == lastRendered)
                    fullUpdate = true;
            }
        }

        void UpdateGuiStr()
        {
            guiStr = "Scale: " + valCycle[scaleIndex].ToString("F0") + "fps" + "        Current FPS: " + curFPS.ToString("F1") +
                "        Moving Average FPS: " + ((float)movingAvg).ToString("F1");
        }

        void Update()
        {
            if (GameSettings.MODIFIER_KEY.GetKey())
            {
                if (Input.GetKeyDown(Settings.keyToggleWindow))
                {
					ToolbarController.Instance.Toggle();
                }
                if (this.showUI && Input.GetKeyDown(Settings.keyScaleUp))
                {
                    // Increase scale
                    scaleIndex = (scaleIndex + 1) % numScales;
                    fullUpdate = true;
                }
                if (this.showUI && Input.GetKeyDown(Settings.keyScaleDown))
                {
                    // Decrease scale
                    scaleIndex = (scaleIndex + numScales - 1) % numScales;
                    fullUpdate = true;
                }
            }

            if (!showUI)
                return;

            if (fullUpdate)
            {
                fullUpdate = false;
                lastRendered = (valIndex + 1) % Settings.GraphWidth;
            }

            // If we want to update this time
            if (lastRendered != valIndex)
            {

                // We're going to wrap this back round to the start so copy the value so 
                int startlastRend = lastRendered;

                // Update the columns from lastRendered to valIndex wrapping round at the end
                if (startlastRend >= valIndex)
                {
                    for (int x = startlastRend; x < Settings.GraphWidth; x++)
                    {
                        //DrawColumn(texGraph, x, (int)((double)fpsValues[x, 0] * Settings.GraphHeight / scale), greenLine,  blackLine);
                        DrawData(x);
                    }

                    startlastRend = 0;
                }

                for (int x = startlastRend; x < valIndex; x++)
                {
                    //DrawColumn(texGraph, x, (int)((double)fpsValues[x, 0] * Settings.GraphHeight / scale), greenLine,  blackLine);
                    DrawData(x);
                }

                if (valIndex < Settings.GraphWidth)
                    texGraph.SetPixels(valIndex, 0, 1, Settings.GraphHeight, blueLine);
                if (valIndex != Settings.GraphWidth - 2)
                    texGraph.SetPixels((valIndex + 1) % Settings.GraphWidth, 0, 1, Settings.GraphHeight, blackLine);
                texGraph.Apply();

                lastRendered = valIndex;
            }
        }

        void DrawData(int x)
        {
            float scale = valCycle[scaleIndex];
            float heightAdj = Settings.GraphHeight / scale;

            if (fpsValues[x, FPS] > 0)
                DrawLine(texGraph, x, (int)((double)fpsValues[x, FPS] * heightAdj), greenLine, blackLine);
            if (fpsValues[x, FPS_AVG] > 0)
                DrawLine(texGraph, x, (int)((double)fpsValues[x, FPS_AVG] * heightAdj), yellowLine, null);
            if (Settings.showPerfectSym)
                DrawLine(texGraph, x, (int)((double)SYM_MULT * Settings.GraphHeight), greyLine, null);
            if (fpsValues[x, SYMRATE] > 0)
                DrawLine(texGraph, x, (int)((double)fpsValues[x, SYMRATE] * Settings.GraphHeight), redLine, null);

        }
        void RedrawGraph()
        {
            for (int x = 0; x < Settings.GraphWidth; x++)
            {
                DrawData(x);
            }
        }

#if false
        void DrawColumn(Texture2D tex, int x, int y, Color[] fgcol, Color[] bgcol)
        {
            if (y > Settings.GraphHeight - 1)
                y = Settings.GraphHeight - 1;
            tex.SetPixels(x, 0, 1, y + 1, fgcol);
            if (y < Settings.GraphHeight - 1)
                tex.SetPixels(x, y + 1, 1, Settings.GraphHeight - 1 - y, bgcol);
        }
#endif

        const int LINE_WIDTH = 3;
        void DrawLine(Texture2D tex, int x, int y, Color[] fgcol, Color[] bgcol)
        {
            if (y > Settings.GraphHeight - LINE_WIDTH)
                y = Settings.GraphHeight - LINE_WIDTH;

            if (bgcol != null) tex.SetPixels(x, 0, 1, Settings.GraphHeight, bgcol);
            tex.SetPixels(x, y, 1, LINE_WIDTH, fgcol);
        }

        void OnGUI()
        {
            if (isKSPGUIActive)
            {
                if (showUI)
                {
                    if (null == labelStyle) this.InitGui();
                    windowPos = GUILayout.Window(windowId, windowPos, WindowGUI, windowTitle);
                    // do this here since if it's done within the window you only recieve events that are inside of the window
                    this.resizeHandle.DoResize(ref this.windowPos);
                }

                if (showHelp)
                    helpWinPos = GUILayout.Window(windowId + 1, helpWinPos, helpWin, "ShowFPS Help");
            }
        }

        double lastRescaleTime = 0;

        void WindowGUI(int windowID)
        {
            if (GUI.Button(new Rect(4, 3f, 22f, 15f), new GUIContent("x"), redButtonFont))
                showUI = false;

            if (GUI.Button(new Rect(windowPos.width - 22, 2, 18, 15), "?"))
                showHelp = !showHelp;
            UpdateGuiStr();
            GUILayout.BeginHorizontal();
            GUILayout.Label(guiStr); //, labelStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Refresh"))
                this.InitGraphWindow();
            GUILayout.Space(10);
            if (GUILayout.Button("Clear"))
            {
                Array.Clear(fpsValues, 0, Settings.GraphWidth * 3);
                valIndex = lastRendered = 0;
                this.InitGraphWindow();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Rescale", GUILayout.Width(70)) || (Settings.periodicRescale && Planetarium.fetch.time - lastRescaleTime >= 60f))
            {
                float maxHeight = 5;
                int oldScaleIndex = scaleIndex;
                lastRescaleTime = Planetarium.fetch.time;
                for (int i = 0; i < Settings.GraphWidth; i++)
                    maxHeight = Math.Max(maxHeight, fpsValues[i, FPS]);

                for (int i = 0; i < valCycle.Length; i++)
                    if (maxHeight + 1 >= valCycle[i])
                        scaleIndex = i + 1;
                    else break;
                scaleIndex = Math.Min(valCycle.Length - 1, scaleIndex);
                if (oldScaleIndex != scaleIndex)
                    RedrawGraph();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
			bool oShowPerfectSym = GUILayout.Toggle(Settings.showPerfectSym, "Show Max Symrate");
            GUILayout.FlexibleSpace();
			bool oPeriodicRescale = GUILayout.Toggle(Settings.periodicRescale, "Periodic auto-rescale");
            GUILayout.EndHorizontal();
            if (!resizeHandle.resizing)
            {
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                GUILayout.Label(valCycle[scaleIndex].ToString("F0"), greenFont);
                GUILayout.FlexibleSpace();
                GUILayout.Label((valCycle[scaleIndex] * .75).ToString("F0"), greenFont);
                GUILayout.FlexibleSpace();
                GUILayout.Label((valCycle[scaleIndex] * .5).ToString("F0"), greenFont);
                GUILayout.FlexibleSpace();
                GUILayout.Label((valCycle[scaleIndex] * .25).ToString("F0"), greenFont);
                GUILayout.FlexibleSpace();
                GUILayout.Label(" 0", greenFont);
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.Box(texGraph, labelStyle);
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.Label(" ");
                GUILayout.FlexibleSpace();
                GUILayout.Label(" ");
                GUILayout.FlexibleSpace();
                GUILayout.Label("1", redFont);
                GUILayout.FlexibleSpace();
                GUILayout.Label("0.5",redFont );
                GUILayout.FlexibleSpace();
                GUILayout.Label("0", redFont);
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Transparency:", GUILayout.Width(130));
			float oAlpha = GUILayout.HorizontalSlider(Settings.alpha, 0.1f, 1f, GUILayout.Width(130));
            if (oAlpha != Settings.alpha)
            {
                blackLine = new Color[Settings.GraphHeight];
                for (int i = 0; i < blackLine.Length; i++)
                    blackLine[i].a = oAlpha;
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("Frequency (" + Settings.frequency.ToString("F2") + "s):", GUILayout.Width(130));
			float oFreq = GUILayout.HorizontalSlider(Settings.frequency, 0.25f, 1f, GUILayout.Width(130));

            GUILayout.EndHorizontal();

            this.resizeHandle.Draw(ref this.windowPos);

            GUI.DragWindow(windowDragRect);
            if (oShowPerfectSym != Settings.showPerfectSym ||
                oPeriodicRescale != Settings.periodicRescale ||
                oAlpha != Settings.alpha ||
                oFreq != Settings.frequency)
            {
                Settings.showPerfectSym = oShowPerfectSym;
                Settings.periodicRescale = oPeriodicRescale;
                Settings.alpha = oAlpha;
                Settings.frequency = oFreq;
                SaveWinSettings();
            }
        }

        void SaveWinSettings()
        {
			Settings.winX = windowPos.x;
			Settings.winY = windowPos.y;
			Settings.SaveConfig();
        }

        static GUIStyle areaStyle;

        const string helpText1 =
            "<B><color=yellow>General Controls</color></B>\n\n" +
            "<B>Mod-KeypadMultiply</B> toggles the display of the window.\n" +
            "<B>Mod-KeypadPlus</B> increases the vertical scale of the graph.\n" +
            "<B>Mod-KeypadMinus</B> decreases the vertical scale of the graph.\n\n" +

            "<B><color=yellow>Legend</color>\n\n</B>\b" +
            "<color=green>Green</color>      FPS﻿\n" +
            "<color=yellow>Yellow</color>     FPS Average﻿\n" +
            "<color=red>Red</color>        Simulation Rate\n" +
            "<color=grey>Grey</color>      Max Sim Rate";

        void helpWin(int windowID)
        {
            GUILayout.BeginHorizontal();
            GUILayout.TextArea(helpText1, areaStyle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close"))
                showHelp = false;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUI.DragWindow();
        }


        private class ResizeHandle
        {
            internal bool resizing;
            private Vector2 lastPosition = new Vector2(0, 0);
            private const float resizeBoxSize = 18;
            private const float resizeBoxMargin = 2;

            internal void Draw(ref Rect winRect)
            {

                var resizer = new Rect(winRect.width - resizeBoxSize - resizeBoxMargin, winRect.height - resizeBoxSize - resizeBoxMargin, resizeBoxSize, resizeBoxSize);
                GUI.Box(resizer, "//", resizeBoxStyle);

                if (!Event.current.isMouse)
                {
                    return;
                }

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
                    resizer.Contains(Event.current.mousePosition))
                {
                    this.resizing = true;
                    this.lastPosition.x = Input.mousePosition.x;
                    this.lastPosition.y = Input.mousePosition.y;

                    Event.current.Use();
                }
            }

            internal void DoResize(ref Rect winRect)
            {
                if (!this.resizing)
                {
                    return;
                }

                if (Input.GetMouseButton(0))
                {
                    var deltaX = Input.mousePosition.x - this.lastPosition.x;
                    var deltaY = Input.mousePosition.y - this.lastPosition.y;

                    //Event.current.delta does not make resizing very smooth.

                    this.lastPosition.x = Input.mousePosition.x;
                    this.lastPosition.y = Input.mousePosition.y;

                    winRect.width += deltaX;
                    winRect.height -= deltaY;


                    Settings.GraphWidth = (int)winRect.width - 8;
                    Settings.GraphHeight = (int)winRect.height - 42;

                    if (Event.current.isMouse)
                    {
                        Event.current.Use();
                    }
                }

                if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    this.resizing = false;

                    Event.current.Use();
                    instance.InitGraphWindow();
                }
            }
        } // ResizeHandle


    }
}
