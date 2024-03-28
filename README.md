# ShowFPS /L Unleashed

Simple FPS display for KSP.

[Unleashed](https://ksp.lisias.net/add-ons-unleashed/) fork by Lisias.


## In a Hurry

* [Latest Release](https://github.com/net-lisias-kspu/ShowFPS/releases)
	+ [Binaries](https://github.com/net-lisias-kspu/ShowFPS/tree/Archive)
* [Source](https://github.com/net-lisias-kspu/ShowFPS)
* Documentation
	+ [Project's README](https://github.com/net-lisias-kspu/ShowFPS/blob/master/README.md)
	+ [Install Instructions](https://github.com/net-lisias-kspu/ShowFPS/blob/master/INSTALL.md)
	+ [Change Log](./CHANGE_LOG.md)
	+ [TODO](./TODO.md) list


## Description

### Features

Simple framerate counter in the top right corner of the screen. For those that don't have access to the usual Windows utilities for display it, like Linux users.

Graph window to show FPS over time.  Graph shows current FPS, a moving average of the FPS, and the Simulation rate (ie:  how much is it lagging, related to yellow/red clocking).  It can also show a line of a normal sim rate (no lagging).

Along the left of the graph will be a scale for the FPS.  Along the right will be a scale for the symrate.

### Usage

Just press `F8` for toggle the FPS counter on and off. Pressing `Ctrl+F8`
while is enabled will also do a very minimalist benchmark, calculating the 
average FPS and the lowest count.

You can click and drag the counter anywhere you like. You might also change
the default key `F8` to anything you like in the `settings.cfg` file.


#### Graph Window Controls

	Buttons
		Refresh					Redraw the graph
		Clear					Clear all data form graph 
		Rescale					Rescale the graph to fix all the data.

	Toggles
		Show Max Symrate		Show a grey line of what the normal (no losses) symrate should be
		Periodic auto-rescale	Will automatically rescale the graph once a minute, if necessary

	Sliders
		Transparency			Lets the background of the graph be transparent.  Only takes effect on newly drawn lines or by resizing the graph
		Frequency				How often to plot a datapoint

	Resizer
		A resizer control is at the lower right of the screen.  Click and drag it to resize the window

The toggle and slider settings are saved between game sessions


## Installation

Detailed installation instructions are now on its own file (see the [In a Hurry](#in-a-hurry) section) and on the distribution file.

## License:

* This work is double licensed as follows:
	+ [LGPL 3.0](https://www.gnu.org/licenses/lgpl-3.0.txt). See [here](./LICENSE)
		+ You are free to:
			- Use : unpack and use the material in any computer or device
			- Redistribute : redistribute the original package in any medium
			- Adapt : Reuse, modify or incorporate source code into your works (and redistribute it!) 
		+ Under the following terms:
			- You retain any copyright notices
			- You recognise and respect any trademarks
			- You don't impersonate the authors, neither redistribute a derivative that could be misrepresented as theirs.
			- You credit the author and republish the copyright notices on your works where the code is used.
			- You relicense (and fully comply) your works using LGPL 2.0
				- Please note that upgrading the license to a new GPL version **IS NOT ALLOWED** for this fork, as this author **DID NOT** added the "or (at your option) any later version" on the license.

Please note the copyrights and trademarks in [NOTICE](./NOTICE).


## UPSTREAM

* Elián Hanisch <lambdae2@gmail.com> ROOT
	+ Any further information is currently M.I.A.
* [LinuxGuruGamer](https://forum.kerbalspaceprogram.com/profile/129964-linuxgurugamer/) CURRENT
	+ [Forum](https://forum.kerbalspaceprogram.com/topic/172692-*/) 
	+ [SpaceDock](https://spacedock.info/mod/1757/ShowFPS)
	+ [Github](https://github.com/linuxgurugamer/ShowFPS)
