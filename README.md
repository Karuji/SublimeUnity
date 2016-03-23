# Sublime Unity
Sublime Unity is a Unity plugin/asset to aid in the use of Sublime Text 3 as the primary editor for Unity.

To me this is the guide I wish I had when I started using a Mac and was trying to get trh

This document will also strive to get Sublime Text running in an almost IDE like fashion. Short of a debugger it can do anything MonoDevelop or Visual Studio can do.

There are two parts to this:

1. Talking about the asset itself.
2. The Setup required to get it running.

As a note I'm focusing on the OS X environment here. Other OS users can, probably, follow along. Since Windows has UnityVS, and a *nix user will think this child's play I'm not going to be writing about them.

Since I'm focusing on OS X I'm going to go a bit outside the scope of _just_ Unity and Sublime, and give some general dev tips that have really helped with with working on the platform :)

## The Asset Itself
The asset itself is fairly simple. It can generate a OmniSharp Unity appropriate _project name_.sublime-project

__Image Here__

## Getting it all setup

I hope it's not going out on a limb to assume that you have [Unity](http://unity3d.com/get-unity/download) and [Sublime Text 3](http://www.sublimetext.com/3) installed.

### Installing Homebrew<a name="InstallHomeBrew"></a>
_This is for the Mac crowd._
[HomeBrew](http://brew.sh) is a command line package manager on OS X. It's like apt-get on Ubuntu/Debian linux, but more human friendly.

Open a Terminal and enter 

    ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"`

You might need to restart your terminal, but now we get to do a bit of magic

### Install Mono
Wait you might ask: Doesn't Unity come with Mono? Yes, but Sublime can't talk to it.

To install mono on a Mac

    brew install mono

Everyone else: grab it from Mono's [site.](http://www.mono-project.com/)

### Install Omnisharp
Omnisharp is what provides the intellisensing and auto complete to Sublime. Mac folk can install it via

    brew tap omnisharp/omnisharp-roslyn
    brew update
    brew install omnisharp

Otherwise just grab it from their [site.](http://www.omnisharp.net/)

OmniSharp won't actually do anything until we get it talking so Sublime so lets go and get Sublime set up.

### Package Control
Now that we have all the backend stuff installed it's time to move onto getting Sublime up and running.

[Package Control](https://packagecontrol.io/) is basically HomeBrew for Sublime Text, and is easily accessible from the _Control Palette `cmd+shift+p`

Package Control asks people giving instructions very nicely not to give the install code since it changes with every release. So follow the instructions on [this page](https://packagecontrol.io/installation) quick and you'll be setup.

Got package control installed? Good, 'cause we're about to install quick a few packages. But first you should restart Sublime.

### Installing Packages

Packages, and their customisation and configuration, are kinda the heart of getting this all running, and we'll go in a descending order of importance of importance for the packages themselves.

#### OmniSharp
[OmniSharp](https://packagecontrol.io/packages/OmniSharp) is responsible for all the nice autocompletion and intellisensing. Until this was installed there was no way for Sublime to know what the OmniSharp server was doing.

To install type `cmd+shit+p` then `package control install` then `omnisharp` and hit enter.

The command palette (cmd+shift+p) has intellityping? so you don't have to type it all out.

The rest of the packages are Quality of Life things, this is the core of what we're setting up. Now you might be tempted to try it out, but it's not going to work. Without the Sublime Unity generate .sublime-project OmniSharp won't know how the files in the Unity project reference each other.

Speaking of quality of life: Sublime plugins have a number of customisable settings. Personally I really like hiding OmniSharp's warnings since they don't really help when dealing with Unity. To do this: In the Sublime menus go `Sublime Text > Preferences > Package Settings > OmniSharp > Settings - User` You'll see a some code along the lines of

    {
        "omnisharp_server_config_location": "/Users/UserName/Library/Application Support/Sublime Text 3/Packages/OmniSharp/PrebuiltOmniSharpServer/config.json"
    }

You want to edit this to be

    {
        "omnisharp_server_config_location": "/Users/UserName/Library/Application Support/Sublime Text 3/Packages/OmniSharp/PrebuiltOmniSharpServer/config.json",
        "omnisharp_onsave_showwarningwindows": false
    }

If you're familiar with JSON then you'll feel right at home. If you're new to JSON: it's important to not that each 'record' ends with a `,` so just remember to add one before inserting the new line.

OmniSharp needs a per-project sublime-project, but that's what Sublime Unity is there for. So skip down to it if you want to see how it works.

#### Unity3D
[Unity3D](https://packagecontrol.io/packages/Unity3D) adds Unity C# and JavaScript style syntax highlighting (maybe boo, but who uses boo?) if you want to switch a file to use the Unity C# instead of regular C# open the command palette and type `ssn Unity C#` there are also configs in Sublime that you can set all .cs files to be opened as UnityC#, but that would be kinda dumb

#### ApplySyntax
[ApplySyntax](https://packagecontrol.io/packages/ApplySyntax) apply syntax is a system that will automatically mark a file as using a specific syntax. So you don't have to worry about the Unity C# syntax highlighting showing up outside of a Unity file.

You're going to need to edit the user settings for ApplySyntax so that it has a rule to determine which .cs files are the Unity ones.

In the Sublime menus go `Sublime Text > Preferences > Package Settings > ApplySyntax > Settings - User` and paste the following code in

    {
        "reraise_exceptions": false,
        "new_file_syntax": false,
        "add_exts_to_lang_settings": true,
        "debug": false,
        "syntaxes": 
        [
            {
                // Unity 3D C# format detection.
                "syntax": "Unity3D/UnityC#",
                "match": "all",
                "rules": 
                [
                    {"file_path": ".*\\.cs$"},
                    {"contains": "using UnityEngine;"}
                ]
            }
        ]
    }

If you take a quick look at the code you'll notice that the rule that ApplySyntax uses to determine if a .cs file is a Unity C# file is the presence of the term `using UnityEngine;`

One small side note: if you have Unity C# files already

**Important Note:** We still have one step to do before this will actually work.

_Small Sublime lesson:_ when you get a package (especially the small ones like Unity3D) they are downloaded and store in a .zip format.

ApplySyntax uses `.../Sublime Text 3/Packages/` as it's base. Unity3D is currently in `.../Sublime Text3/Installed Packages/` in the sublime-package zip. In order to fix this we need to open Terminal and run the following commands.

    cd ~/Library/Application\ Support/Sublime\ Text\ 3/
    cp Installed\ Packages/Unity3D.sublime-package Packages/
    cd Packages/
    unzip Unity3D.sublime-package -d Unity3D
    rm Unity3D.sublime-package

And with that ApplySyntax is done _whew!_

#### Open in Relevant Window
[Open in Relevant Window](https://packagecontrol.io/packages/Open%20in%20Relevant%20Window) will eventually help us deal with one of Unity/Sublime combo's more annoying habits: double clicking on a .cs file in Unity won't open it in the in the window of the correct .sublime-project, and thus we have to drag it in for the intellisense to work.

### Sublime Unity
Sublime Unity is the last part that ties this all together. It's a Unity Editor extension that adds a menu item, with keyboard shortcuts, to quickly make a .sublime-project and open an existing one for any Unity project.

Either use the .unity-asset, or just add the source code to the Assets folder.

The .sublime-project is what lets OmniSharp know where to look for autocompletes, and all that other fun intellisensing.

And the open Sublime opens that project, so that the whole system is running, since if Sublime doesn't have it as the open project then most of this isn't going to work.

So are we done?

No, but it's just two things left.

1 We need to edit you're bash profile. Open terminal and type `open ~/.bash_profile` and check if you have the text `export PATH="$PATH:/usr/local/bin"` in it. If not add it.

2 Type `ln -s "/Applications/Sublime Text.app/Contents/SharedSupport/bin/subl" /usr/local/bin/subl` in the terminal

And you're done. Also you can now open Sublime from the command line, and pass it any file, or folder to be opened.

SublimeUnity is the end tool for a process that I picked up from [Denny Scott](http://blog.zephyr-ware.com/unity-and-sublime/) who made a really excellent blog outlining a lot of the process used here, and has some great extra tips for Sublime that I highly recommend that you check out!

Happy coding, and Make Games!
