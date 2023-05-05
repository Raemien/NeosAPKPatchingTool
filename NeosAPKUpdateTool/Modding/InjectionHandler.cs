using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace NeosAPKPatchingTool.Modding
{
    internal class InjectionHandler
    {
        private string BinDirectory { get; set; }
        public InjectionHandler(string managedPath)
        {
            BinDirectory = managedPath;
        }
        private static Instruction? FindMethodOperation(Collection<Instruction> instructions, string method, OpCode opcode)
        {
            return instructions.Where(
                op => op.OpCode == opcode
                && op.Operand is MethodReference methodReference
                && methodReference.FullName.Contains(method)).FirstOrDefault();
        }

        private MethodReference? GetModLoaderInit()
        {
            try
            {
                string nmlPath = Path.Combine(DependencyManager.DepDirectory, "NeosModLoader.dll");
                ModuleDefinition module = ModuleDefinition.ReadModule(nmlPath);

                TypeDefinition? engineType = module.Types.Where(modtype => modtype.FullName == "NeosModLoader.ExecutionHook").FirstOrDefault();
                if (engineType == null) throw new Exception("ExecutionHook does not exist!");

                MethodDefinition? initMethod = engineType.Methods.Where(method => method.Name == ".cctor").FirstOrDefault();
                if (initMethod == null) throw new Exception("ExecutionHook does not have a constructor!");
                return initMethod;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching entry point from NeosModLoader:\n{0}", ex.Message);
                throw;
            }
        }
        private MethodDefinition? GetEngineInit(ModuleDefinition module)
        {
            TypeDefinition? engineType = module.Types.Where(modtype => modtype.FullName == "FrooxEngine.Engine").FirstOrDefault();
            if (engineType == null) throw new Exception("FrooxEngine.Engine does not exist!");

            TypeDefinition? initNested = engineType.NestedTypes.Where(nestedtype => nestedtype.FullName.StartsWith("FrooxEngine.Engine/<Initialize>")).FirstOrDefault();
            if (initNested == null) throw new Exception("Unable to finded nested async init!");

            return initNested.Methods.Where(method => method.Name == "MoveNext").FirstOrDefault();
        }

        private void InsertExtraAssembly(ILProcessor processer, Instruction previous)
        {
            var instruct_Add = FindMethodOperation(processer.Body.Instructions, "Add", OpCodes.Callvirt);
            if (instruct_Add == null) throw new Exception("Unable to inject instruct_Add into NeosVR. The current version may be incompatible.");
            processer.InsertAfter(previous, instruct_Add);

            Instruction instruct_LoadStr = processer.Create(OpCodes.Ldstr, "NeosModLoader.dll");
            if (instruct_LoadStr == null) throw new Exception("Unable to inject instruct_LoadStr into NeosVR. The current version may be incompatible.");

            processer.InsertAfter(previous, instruct_LoadStr);

            var instruct_GetAssemblies = FindMethodOperation(processer.Body.Instructions, "FrooxEngine.Engine::get_ExtraAssemblies", OpCodes.Call);
            if (instruct_GetAssemblies == null) throw new Exception("Unable to inject instruct_GetAssemblies into NeosVR. The current version may be incompatible.");
            processer.InsertAfter(previous, instruct_GetAssemblies);
        }

        public void PatchFroox()
        {
            ModuleDefinition? module = null;
            try
            {
                string oldpath = Path.Combine(BinDirectory, "FrooxEngine-unmodified.dll");
                string newpath = Path.Combine(BinDirectory, "FrooxEngine.dll");
                File.Copy(newpath, oldpath, true);
                module = ModuleDefinition.ReadModule(oldpath);
                MethodDefinition? initMethod = GetEngineInit(module);
                if (initMethod == null) throw new Exception("Engine.Initialize async Task does not exist!");

                var importedRef = module.ImportReference(GetModLoaderInit());
                ILProcessor processer = initMethod.Body.GetILProcessor();
                Instruction instruct_initLoader = processer.Create(OpCodes.Call, importedRef);

                if (FindMethodOperation(initMethod.Body.Instructions, "NeosModLoader.ExecutionHook::.cctor", OpCodes.Call) != null)
                {
                    module.Dispose();
                    throw new Exception("Your NeosVR APK is already patched! No further action is required.");
                }

                Instruction? instruct_GetArgs = FindMethodOperation(initMethod.Body.Instructions, "System.Environment::GetCommandLineArgs", OpCodes.Call);
                if (instruct_GetArgs == null) throw new Exception("Unable to inject -LoadAssembly into NeosVR. The current version may be incompatible.");
                processer.InsertAfter(instruct_GetArgs, instruct_initLoader);
                InsertExtraAssembly(processer, instruct_GetArgs);

                module.Write(newpath);
                module.Dispose();
                File.Delete(oldpath);
            }
            catch (Exception ex)
            {
                if (module != null) module.Dispose();
                Console.WriteLine("Error injecting into FrooxEngine:\n{0}", ex.Message);
                Thread.Sleep(5000);
                Directory.Delete(PatchingHandler.WorkingPath, true);
                Environment.Exit(1);
                throw;
            }
        }
    }
}
