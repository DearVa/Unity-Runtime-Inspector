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

# Q&A
1. **Why my dnSpy says "You must add a reference to assembly 'UnityEngine.CoreModule.dll'" when compiling?  
  Maybe your dnSpy is out of date. Make sure you have completely decompress the whole "Managed" directory, instead of just decompress the "Assembly-CSharp.dll".
2. **Why my game crashes after inject this dll?  
  Make sure to use the correct version. You can open "mscorlib.dll" with dnSpy. You should use "Android-mscorlib-2.0.0.0" if its version is 2.0.0.0, otherwise you can use master branch. Another possibility is that the game has some kind of anti-injection mechanism.
3. **Is it capable for il2cpp-backend games?  
  No.

# Branches：
* master: 
  
  Using .NET Framework 4.5. It depends on mscorlib 4.0.0.0. But cannot running on Android.
* Android-mscorlib-2.0.0.0: 
  
  Using .NET Framework 3.5. It depends on mscorlib 2.0.0.0. As it shows, it can run on Android however Mono.CSharp cannot use any more.
