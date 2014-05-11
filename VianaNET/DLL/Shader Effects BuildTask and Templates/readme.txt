To install the VS project and item templates for building Shader Effects for WPF:

* Install ShaderBuildTaskSetup.msi 
* Right click on Templates.zip and choose Extract All.  Choose your Visual Studio directory (mine is at "C:\Users\gregsc\Documents\Visual Studio 2008") as the directory to extract into.  Not any of the subdirectories, but that directory itself.  This will ask you if you want to merge files into the existing directory structure.  Say Yes. This will then merge the new VS templates into the "Templates" directory under your VS directory. (See ** for details)

At this point, you should be able to do "New Project" and see "WPF Shader Effect Library", and new Item and see "WPF Shader Effect".

One important note:  when you create a new "WPF Shader Effect" Item, you need to select the .fx file in Solution Explorer and choose a "Build Action" of Effect from the property grid.  Otherwise the effect won't build.  (I'm trying to figure out what magic needs to happen in the VS template to make this automatic).


(**) If you've set up your VS user project/item template directories to something other than the default, you'll want to extract there.  You can see what they are by, in VS, selecting Tools | Options | Projects and Solutions | General.
