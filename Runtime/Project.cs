using System.Threading.Tasks;

namespace Forge
{
    public static class Project
    {
        public enum ImportType
        {
            Era,
            Folder
        }

        public static async Task Create(string path, ImportType type, string[] files)
        {
            await Import(type, files, path);

            await Task.Yield();
        }

        public static async Task Import(ImportType type, string[] files, string path)
        {
            switch (type)
            {
                case ImportType.Era:
                    await Decompiler.FromEra(files, path);
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
