using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace TvEpisodeNamer {
    public class FileExtensions {
        private static FileExtensions _instance;

        static FileExtensions()
        {
            GetResourcesUnder("images");
        }

        public static FileExtensions Instance
        {
            get { return _instance ?? (_instance = new FileExtensions()); }
        }

        private static List<string> _resourceList;

        public List<string> ResourceList
        {
            get
            {
                if(_resourceList==null) GetResourcesUnder("/images");
                return _resourceList;
                
            }
        }

        public static void GetResourcesUnder(string folder)
        {
            folder = folder.ToLower() + "/";

            var assembly = Assembly.GetCallingAssembly();
            var resourcesName = assembly.GetName().Name + ".g.resources";
            var stream = assembly.GetManifestResourceStream(resourcesName);
            if (stream != null)
            {
                var resourceReader = new ResourceReader(stream);

                _resourceList =
                    (from p in resourceReader.OfType<DictionaryEntry>()
                        let theme = (string) p.Key
                        where theme.StartsWith(folder)
                        select theme.Substring(folder.Length)).ToList();



            }
        }


    }
}