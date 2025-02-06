using System.IO;
using System.Threading.Tasks;

using UnityEngine;

namespace Forge
{
    public static class Project
    {
        public enum ImportType
        {
            Era,
            Folder
        }

        public static async Task Create(string name, ImportType type, string[] files)
        {
            await Import(type, files, name);

            await Task.Yield();
        }

        public static async Task Import(ImportType type, string[] files, string name)
        {
            switch (type)
            {
                case ImportType.Era:
                    await Decompiler.FromEra(files, name);
                    break;
                case ImportType.Folder:
                    break;
            }

            await Task.Yield();
        }

        public static async Task Export()
        {
            await Task.Yield();
        }
    }
}
