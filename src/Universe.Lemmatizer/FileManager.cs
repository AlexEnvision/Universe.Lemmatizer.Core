//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Lemmatizer                                            ║
//  ║                                                                                 ║
//  ║   Licensed under the Apache License, Version 2.0 (the "License");               ║
//  ║   you may not use this file except in compliance with the License.              ║
//  ║   You may obtain a copy of the License at                                       ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0                                ║
//  ║                                                                                 ║
//  ║   Unless required by applicable law or agreed to in writing, software           ║
//  ║   distributed under the License is distributed on an "AS IS" BASIS,             ║
//  ║   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.      ║
//  ║   See the License for the specific language governing permissions and           ║
//  ║   limitations under the License.                                                ║
//  ║                                                                                 ║
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Lemmatizer                                            ║
//  ║                                                                                 ║
//  ║   Лицензировано согласно Лицензии Apache, Версия 2.0 ("Лицензия");              ║
//  ║   вы можете использовать этот файл только в соответствии с Лицензией.           ║
//  ║   Вы можете найти копию Лицензии по адресу                                      ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0.                               ║
//  ║                                                                                 ║
//  ║   За исключением случаев, когда это регламентировано существующим               ║
//  ║   законодательством или если это не оговорено в письменном соглашении,          ║
//  ║   программное обеспечение распространяемое на условиях данной Лицензии,         ║
//  ║   предоставляется "КАК ЕСТЬ" и любые явные или неявные ГАРАНТИИ ОТВЕРГАЮТСЯ.    ║
//  ║   Информацию об основных правах и ограничениях,                                 ║
//  ║   применяемых к определенному языку согласно Лицензии,                          ║
//  ║   вы можете найти в данной Лицензии.                                            ║
//  ║                                                                                 ║
//  ╚═════════════════════════════════════════════════════════════════════════════════╝

using System.IO;
using System.Reflection;
using Universe.Lemmatizer.Files.FileSystem;
using Universe.Lemmatizer.Implement.Agramtab;

namespace Universe.Lemmatizer
{
    public abstract class FileManager
    {
        private string _registryPath;
        private string _registryValue;

        private string INIFileName => "/Bin/rml.ini";

        public static FileManager GetDefaultFileManager()
        {
            return new FileSystemFileManager(
                Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
        }

        protected abstract Stream GetFile(string name);

        public Stream GetFile(string registry, string name)
        {
            return GetFile(GetStringInnerFromTheFile(registry) + name);
        }

        public static FileManager GetFileManager(string path)
        {
            return new FileSystemFileManager(path);
        }

        private string GetStringInnerFromTheFile(string registryPath)
        {
            if (registryPath != _registryPath)
                using (var file = GetFile(INIFileName))
                {
                    using (var streamReader = new StreamReader(file, Tools.InternalEncoding))
                    {
                        var str1 = streamReader.ReadLine();
                        var flag = false;
                        for (; str1 != null; str1 = streamReader.ReadLine())
                        {
                            var strArray = str1.Split(' ', '\t');
                            if (strArray != null && strArray.Length == 2 && strArray[0] == registryPath)
                            {
                                var str2 = strArray[1];
                                if (str2.StartsWith("$RML"))
                                {
                                    _registryValue = str2.Replace("$RML", "");
                                    _registryPath = registryPath;
                                    flag = true;
                                    break;
                                }
                            }
                        }

                        if (!flag)
                        {
                            _registryPath = registryPath;
                            _registryValue = "";
                        }
                    }
                }

            return _registryValue;
        }
    }
}