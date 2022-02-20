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
using Universe.Lemmatizer.Implement.Agramtab;

namespace Universe.Lemmatizer.Implement.MorphWizard
{
    internal class CAccentModel
    {
        private byte[] _accents;

        public byte this[int index] => _accents[index];

        public override bool Equals(object obj)
        {
            return obj is CAccentModel caccentModel
                ? Tools.ListEquals(_accents, caccentModel._accents)
                : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var num = 0;
            foreach (var accent in _accents)
                num ^= accent.GetHashCode();
            return num;
        }

        public bool ReadFromString(string s)
        {
            var strArray = s.Split(new char[1] {';'}, StringSplitOptions.RemoveEmptyEntries);
            if (strArray == null || strArray.Length == 0)
                return false;
            _accents = new byte[strArray.Length];
            for (var index = 0; index < strArray.Length; ++index)
            {
                byte result;
                if (!byte.TryParse(strArray[index], out result))
                    return false;
                _accents[index] = result;
            }

            return true;
        }

        public override string ToString()
        {
            var str = "";
            for (var index = 0; index < _accents.Length; ++index)
                str = str + _accents[index] + ";";
            return str;
        }
    }
}