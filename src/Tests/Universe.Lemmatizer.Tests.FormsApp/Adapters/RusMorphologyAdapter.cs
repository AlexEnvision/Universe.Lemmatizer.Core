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

using System;
using System.Collections.Generic;
using System.IO;

namespace Universe.Lemmatizer.Tests.FormsApp.Adapters
{
    [Serializable]
    public class RusMorphologyAdapter
    {
        [NonSerialized]
        public readonly ILemmatizer Lemmatizer;

        private const string RootDirRelativeMorhologyBinaryPath = "\\LemmatizerBinary\\RML\\";

        public RusMorphologyAdapter()
        {
            var rootDir = Directory.GetCurrentDirectory();
            var lemmatizerMorphPath = rootDir + RootDirRelativeMorhologyBinaryPath;
        
            var lang = MorphLanguage.Russian;
            Lemmatizer = LemmatizerFactory.Create(lang);
            var manager = FileManager.GetFileManager(lemmatizerMorphPath);
            // инициализация словарей
            Lemmatizer.LoadDictionariesRegistry(manager);
        }

        public RusMorphologyAdapter(string morhologyBinaryRootDir)
        {
            var lemmatizerMorphPath = morhologyBinaryRootDir + RootDirRelativeMorhologyBinaryPath;

            var lang = MorphLanguage.Russian;
            Lemmatizer = LemmatizerFactory.Create(lang);
            var manager = FileManager.GetFileManager(lemmatizerMorphPath);
            // инициализация словарей
            Lemmatizer.LoadDictionariesRegistry(manager);
        }

        public string[] TransformMessage(string[] messageTokens)
        {
            List<string> editPhrase = new List<string>();
            foreach (string item in messageTokens)
            {
                IParadigmCollection
                    wordFormParadigmes =
                        Lemmatizer.CreateParadigmCollectionFromForm(item, false,
                            false); // создание парадигмы по словоформе

                if (wordFormParadigmes != null && wordFormParadigmes.Count > 0)
                {
                    var paradigmItem = wordFormParadigmes[0];

                    var paradigm = paradigmItem;
                    editPhrase.Add(paradigm.Norm.ToLower());
                }
            }

            return editPhrase.ToArray();
        }
    }
}