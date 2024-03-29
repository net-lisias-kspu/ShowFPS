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
using KSPe.Annotations;

using UnityEngine;
using H = KSPe.IO.Hierarchy<ShowFPS.Startup>;
using T = KSPe.Util.Image.Texture2D;

namespace ShowFPS
{
	internal static class Icons
	{
		private const string ICONSDIR = "Icons";

		private static Texture2D _IconOff = null;
		internal static Texture2D IconOff => _IconOff ?? (_IconOff = T.LoadFromFile(H.GAMEDATA.Solve("PluginData", ICONSDIR, "fps-38.off")));

		private static Texture2D _IconOn = null;
		internal static Texture2D IconOn => _IconOn ?? (_IconOn = T.LoadFromFile(H.GAMEDATA.Solve("PluginData", ICONSDIR, "fps-38")));

		private static Texture2D _BlizzyOff = null;
		internal static Texture2D BlizzyOff => _BlizzyOff ?? (_BlizzyOff = T.LoadFromFile(H.GAMEDATA.Solve("PluginData", ICONSDIR, "fps-24.off")));

		private static Texture2D _BlizzyOn = null;
		internal static Texture2D BlizzyOn => _BlizzyOn ?? (_BlizzyOn = T.LoadFromFile(H.GAMEDATA.Solve("PluginData", ICONSDIR, "fps-24")));
	}
}
