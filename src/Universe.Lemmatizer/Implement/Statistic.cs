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
using Universe.Lemmatizer.Implement.Agramtab;

namespace Universe.Lemmatizer.Implement
{
    internal class Statistic
    {
        private static readonly HomonodeComparer _homonodeComparer = new HomonodeComparer();

        private static readonly StatnodeComparer _statnodeComparer = new StatnodeComparer();

        private readonly List<Homonode> _homoWeights = new List<Homonode>();

        private readonly List<Statnode> _wordWeights = new List<Statnode>();

        public int GetHomoWeight(int paradigmid, int form)
        {
            var index = _homoWeights.BinarySearch(new Homonode
            {
                X1 = paradigmid,
                X2 = form
            }, _homonodeComparer);
            return index >= 0 && index < _homoWeights.Count ? _homoWeights[index].X3 : 0;
        }

        public int GetWordWeight(int paradigmid)
        {
            var index = _wordWeights.BinarySearch(new Statnode
            {
                X1 = paradigmid
            }, _statnodeComparer);
            return index >= 0 && index < _wordWeights.Count ? _wordWeights[index].X2 : 0;
        }

        public void Load(Lemmatizer lemmatizer, string prefix, FileManager manager)
        {
            Stream file1;
            using (file1 = manager.GetFile(lemmatizer.Registry, prefix + "homoweight.bin"))
            {
                Tools.LoadList(file1, _homoWeights);
            }

            Stream file2;
            using (file2 = manager.GetFile(lemmatizer.Registry, prefix + "wordweight.bin"))
            {
                Tools.LoadList(file2, _wordWeights);
            }
        }

        private class Homonode : ILoad
        {
            public int X1;
            public int X2;
            public int X3;

            public bool Load(Stream stream)
            {
                var count = 4;
                var buffer = new byte[count];
                if (stream.Read(buffer, 0, count) != count)
                    return false;
                X1 = BitConverter.ToInt32(buffer, 0);
                if (stream.Read(buffer, 0, count) != count)
                    return false;
                X2 = BitConverter.ToInt32(buffer, 0);
                if (stream.Read(buffer, 0, count) != count)
                    return false;
                X3 = BitConverter.ToInt32(buffer, 0);
                return true;
            }
        }

        private class HomonodeComparer : IComparer<Homonode>
        {
            public int Compare(Homonode x, Homonode y)
            {
                return x.X1 != y.X1 ? x.X1.CompareTo(y.X1) : x.X2.CompareTo(y.X2);
            }
        }

        private class Statnode : ILoad
        {
            public int X1;
            public int X2;

            public bool Load(Stream stream)
            {
                var count = 4;
                var buffer = new byte[count];
                if (stream.Read(buffer, 0, count) != count)
                    return false;
                X1 = BitConverter.ToInt32(buffer, 0);
                if (stream.Read(buffer, 0, count) != count)
                    return false;
                X2 = BitConverter.ToInt32(buffer, 0);
                return true;
            }
        }

        private class StatnodeComparer : IComparer<Statnode>
        {
            public int Compare(Statnode x, Statnode y)
            {
                return x.X1.CompareTo(y.X1);
            }
        }
    }
}