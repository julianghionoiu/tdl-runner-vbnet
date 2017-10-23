using System.IO;

namespace BeFaster.Runner.Utils
{
    internal static class PathHelper
    {
        public static string RepositoryPath { get; }

        static PathHelper()
        {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var srcPath = FindParent(exePath, "src");

            RepositoryPath = new DirectoryInfo(srcPath).Parent.FullName;
        }

        private static string FindParent(string path, string parentName)
        {
            while (true)
            {
                var directory = new DirectoryInfo(path);
                
                if (directory.Parent == null)
                {
                    return null;
                }

                if (directory.Parent.Name == parentName)
                {
                    return directory.Parent.FullName;
                }

                path = directory.Parent.FullName;
            }
        }
    }
}
