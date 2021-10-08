![issues|welcome](https://img.shields.io/badge/issues-welcome-brightgreen)
![pulls|welcome](https://img.shields.io/badge/pulls-welcome-brightgreen)

# What can it do?
Enable you to inspect a running Unity game just like in Unity.

# Features
* Hierarchy - You can browse all GameObjects in the scene
* Inspector - You can browse and modify the components of a GameObject
* Console - You can run C# code runtime, with AutoComplete. Input history will be save, use ↑/↓ to switch.
* Commands - You can use some pre-defined commands in Console, such as #bind, #unbind...

# Screen shots
![Preview_Hierarchy](https://raw.githubusercontent.com/DearVa/UnityInGameDbg/master/img/Preview_Hierarchy.png)
![Preview_Inspector](https://raw.githubusercontent.com/DearVa/UnityInGameDbg/master/img/Preview_Inspector.png)
![Preview_ViewMesh](https://raw.githubusercontent.com/DearVa/UnityInGameDbg/master/img/Preview_ViewMesh.png)
![Preview_InGameConsole](https://raw.githubusercontent.com/DearVa/UnityInGameDbg/master/img/Preview_InGameConsole.jpg)

# Quick Start
✅Better Inject Method
-------
1. Compile and Put **"RuntimeInspector.dll"**, **"ICSharpCode.Decompiler.dll"** and **"Mono.CSharp.dll"** into **AUnityGame\AUnityGame_Data\Managed** Directory **("Humanizer.dll", "System.Collections.Immutable.dll" and "System.Reflection.Metadata.dll" maybe also necessary)**.
2. Use [SharpMonoInjector](https://github.com/warbler/SharpMonoInjector) to inject. Select **"RuntimeInspector.dll"**, Namespace is also **"RuntimeInspector"**, Class Name is **"ViewerCreator"**, Method Name is **"Create"**.
3. Run your game, select its process in SharpMonoInjector, then click **"inject"**.

~~Old Inject Method~~
-------
1. Compile and Put **"RuntimeInspector.dll"**, **"ICSharpCode.Decompiler.dll"** and **"Mono.CSharp.dll"** into **AUnityGame\AUnityGame_Data\Managed** Directory (**"Humanizer.dll", "System.Collections.Immutable.dll" and "System.Reflection.Metadata.dll" maybe also necessary**).
2. Open **Assembly-CSharp.dll** with [dnspy](https://github.com/dnSpy/dnSpy), remember to make a backup **(Most games use this assembly as main dll, some uses other name, youcan find it by yourself)**!
3. Edit a Management-like class such as "Manager", "Game" or "UI", disassemble it and add the "RuntimeInspector.dll" reference, then add "InGameDebug.ViewerCreator.Create();" in Start() or Awake() method.
4. Compile and save it. If there's nothing wrong, when you enter the game, three buttons will be displayed above the screen.


# Console Commands
**All Console Commands starts with #. Nesting is not supported so far, and you can only input 1 command at a time.**
* **#bind**\
  Bind a key to a C# expression.\
  Example:
  ```#bind(R, Camera.main.transform.Rotate(new Vector3(0, 10f, 0)))```\
  When you press R key, Camera.main will Ratate.

* **#unbind**\
  Unbind a key.\
  Example:
  ```#unbind(R)```

* **#listObj [Doing]**\
  List all GameObjects in the scene to Console.

* **#highlight [Doing]**\
  Highlight a GameObject in Hierarchy.

* **#inspect [Doing]**\
  Inspect a GameObject in Inspector.

# Q&A
1. **Why my dnSpy says "You must add a reference to assembly 'UnityEngine.CoreModule.dll'" when compiling?**  
  Maybe your dnSpy is out of date. Make sure you have completely decompress the whole "Managed" directory, instead of just decompress the "Assembly-CSharp.dll".

2. **Why my game crashes after inject this dll?**  
  Make sure to use the correct version. You can open "mscorlib.dll" with dnSpy. You should use "Android-mscorlib-2.0.0.0" if its version is 2.0.0.0, otherwise you can use master branch. Another possibility is that there are some kind of anti-injection mechanism in the game.

3. **Is it capable for il2cpp-backend games?**  
  No.

# Branches：
* master: 
  
  Using .NET Framework 4.5. It depends on mscorlib 4.0.0.0. But cannot running on Android.
* Android-mscorlib-2.0.0.0: 
  
  Using .NET Framework 3.5. It depends on mscorlib 2.0.0.0. As it shows, it can run on Android however Mono.CSharp cannot use any more.
