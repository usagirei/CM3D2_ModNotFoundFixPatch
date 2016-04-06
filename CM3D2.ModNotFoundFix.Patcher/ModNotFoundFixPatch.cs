// --------------------------------------------------
// CM3D2.ModNotFoundFix.Patcher - ModNotFoundFixPatch.cs
// --------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ReiPatcher;
using ReiPatcher.Patch;

namespace CM3D2.ModNotFoundFix.Patcher
{
    /*
    *    Based on the **Sybarys.FixNoModError.Patch** (cm3d2_f_19), but for ReiPatcher instead of Sybarys Loader.
    *    Prevents Game From Hanging on Missing Mods
    */

    public class ModNotFoundFixPatch : PatchBase
    {
        public const string TOKEN = "CM3D2_MODNOTFOUNDFIX";
        public override string Name => "CM3D2 Mod Not Found Fix Patch";

        public override bool CanPatch(PatcherArguments args)
        {
            if (args.Assembly.Name.Name != "Assembly-CSharp")
                return false;

            if (GetPatchedAttributes(args.Assembly).Any(attribute => attribute.Info == TOKEN))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Assembly Already Patched");
                Console.ForegroundColor = ConsoleColor.Gray;
                return false;
            }
            return true;
        }

        public override void Patch(PatcherArguments args)
        {
            //Debugger.Launch();
            var aMod = args.Assembly.MainModule;

            var tMenu = aMod.GetType("Menu");
            var tFile = aMod.Import(typeof (File)).Resolve();
            var tString = aMod.Import(typeof (String)).Resolve();
            var mGetBaseItem = tMenu.Methods.First(def => def.Name == "GetBaseItemFromMod");
            var mFileExists = tFile.Methods.First(def => def.Name == "Exists");
            var mFileExistsRef = aMod.Import(mFileExists);
            var fStringEmpty = tString.Fields.First(def => def.Name == "Empty");
            var fStringEmptyRef = aMod.Import(fStringEmpty);

            // First Point
            var firstOp =
                mGetBaseItem.Body.Instructions.First();

            // Patching
            var ilp = mGetBaseItem.Body.GetILProcessor();

            ilp.InsertBefore(firstOp, ilp.Create(OpCodes.Ldarg_0)); //------------------ LDARG_0
            ilp.InsertBefore(firstOp, ilp.Create(OpCodes.Call, mFileExistsRef)); //----- CALL File::Exists
            ilp.InsertBefore(firstOp, ilp.Create(OpCodes.Brtrue, firstOp)); //---------- BRTRUE $firstOp
            ilp.InsertBefore(firstOp, ilp.Create(OpCodes.Ldsfld, fStringEmptyRef)); //-- LDSFLD String::Empty
            ilp.InsertBefore(firstOp, ilp.Create(OpCodes.Ret)); //---------------------- RET

            SetPatchedAttribute(args.Assembly, TOKEN);
        }

        public override void PrePatch()
        {
            RPConfig.RequestAssembly("Assembly-CSharp.dll");
        }
    }
}