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

namespace ShowFPS
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class Startup : MonoBehaviour
	{
        private void Start()
        {
            Log.force("Version {0}", Version.Text);

            try
            {
                //KSPe.Util.Compatibility.Check<Startup>(typeof(Version), typeof(Configuration));
                KSPe.Util.Installation.Check<Startup>(typeof(Version));
            }
            catch (KSPe.Util.InstallmentException e)
            {
                Log.error(e.ToShortMessage());
                KSPe.Common.Dialogs.ShowStopperAlertBox.Show(e);
            }
        }
	}
}
