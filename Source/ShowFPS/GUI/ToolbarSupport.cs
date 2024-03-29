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
using UnityEngine;

using KSPe.Annotations;
using Toolbar = KSPe.UI.Toolbar;
using GUI = KSPe.UI.GUI;
using GUILayout = KSPe.UI.GUILayout;
using KSPe.UI.Toolbar;
using KSP.UI.Screens;

namespace ShowFPS
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public class ToolbarController : MonoBehaviour
	{
		private static ToolbarController _Instance = null;
		internal static ToolbarController Instance => _Instance;
		private static KSPe.UI.Toolbar.Toolbar ToolbarInstance => KSPe.UI.Toolbar.Controller.Instance.Get<ToolbarController>();

		[UsedImplicitly]
		private void Awake()
		{
			_Instance = this;
			DontDestroyOnLoad(this);
		}

		[UsedImplicitly]
		private void Start()
		{
			KSPe.UI.Toolbar.Controller.Instance.Register<ToolbarController>(Version.FriendlyName);
		}

		[UsedImplicitly]
		private void OnDestroy()
		{
			this.Destroy();
			ToolbarInstance.Destroy();
		}

		private Graph owner = null;
		private Button button;

		internal void Create(Graph owner)
		{
			this.owner = owner;
			if (null != this.button)
			{
				ToolbarInstance.ButtonsActive(true, true);
				return;
			}

			this.button = Toolbar.Button.Create(this
						,	ApplicationLauncher.AppScenes.SPACECENTER |
							ApplicationLauncher.AppScenes.FLIGHT |
							ApplicationLauncher.AppScenes.MAPVIEW |
							ApplicationLauncher.AppScenes.VAB |
							ApplicationLauncher.AppScenes.SPH |
							ApplicationLauncher.AppScenes.TRACKSTATION
						, Icons.IconOn, Icons.IconOff
						, Icons.BlizzyOn, Icons.BlizzyOff
					)
				;

			this.button.Toolbar
						.Add(Toolbar.Button.ToolbarEvents.Kind.Active,
							new Toolbar.Button.Event(this.OnRaisingEdge, this.OnFallingEdge)
						);
			;

			ToolbarInstance.Add(this.button);
		}

		internal void Destroy()
		{
			ToolbarInstance.ButtonsActive(false, false);
			if (null != this.button) this.button.Active = false;
			this.owner = null;
		}

		private void OnRaisingEdge()
		{
			if (null == this.owner) return; // Better safer than sorry!
			this.owner.Toggle(true);
		}

		private void OnFallingEdge()
		{
			if (null == this.owner) return; // Better safer than sorry!
			this.owner.Toggle(false);
		}

		internal void Toggle() => this.button.Active = !this.button.Active;
	}
}
