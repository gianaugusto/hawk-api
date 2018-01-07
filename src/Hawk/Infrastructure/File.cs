namespace Hawk.Infrastructure
{
    using System.IO;
    using System.Reflection;

    internal class File
    {
        public virtual string ReadAllText(string name)
        {
            var assembly = typeof(File).GetTypeInfo().Assembly;

            using (var stream = assembly.GetManifestResourceStream(name))
            {
                var reader = new StreamReader(stream);
                var script = reader.ReadToEnd();
                return script;
            }
        }
    }
}