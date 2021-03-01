# What can it do?
Enable you to debug a running Unity game just like in Unity.

# Screen shot
![Preview_Hierarchy](https://raw.githubusercontent.com/DearVa/UnityInGameDbg/master/Preview_Hierarchy.png)
![Preview_Inspector](https://raw.githubusercontent.com/DearVa/UnityInGameDbg/master/Preview_Inspector.png)
![Preview_ViewMesh](https://raw.githubusercontent.com/DearVa/UnityInGameDbg/master/Preview_ViewMesh.png)

# How to use it?
1. Compile and Put "InGameDebugger.dll" and "Mono.CSharp.dll" into AUnityGame\AUnityGame_Data\Managed Directory.
2. Open Assembly-CSharp.dll with dnspy(https://github.com/dnSpy/dnSpy), remember to make a backup!
3. Edit a Management-like class such as "Game" or "UI", disassemble it and add the "InGameDebug.dll" reference, then add "InGameDebug.ViewerCreater.Create();" in Start() or Awake() method.
4. Compile and save it. If there's nothing wrong, when you enter the game, three buttons will be displayed above the screen.
