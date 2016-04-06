#### Mod Not Found Error Patch

Based on the **Sybarys.FixNoModError.Patch** (cm3d2_f_19), but for ReiPatcher instead of Sybarys Loader.

Prevents Game From Hanging on Missing Mods

---
#### Installing

Place the **CM3D2.ModNotFoundFix.Patcher.dll** in your CM3D2 ReiPatcher Patches directory, and execute ReiPatcher.

---
#### Building

Make sure you have MSBuild v14 installed (Comes with VS2015)
Place **Mono.Cecil** and **ReiPatcher.exe** references in the References folder, then Execute **build.bat**