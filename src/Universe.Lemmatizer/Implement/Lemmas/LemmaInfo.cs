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

namespace Universe.Lemmatizer.Implement.Lemmas
{
    internal class LemmaInfo : IComparable<LemmaInfo>, IEquatable<LemmaInfo>
    {
        private readonly ushort _accentModelNo;

        private readonly char[] _commonAncode;

        private readonly ushort _flexiaModelNo;

        public LemmaInfo()
        {
            _commonAncode = new char[2];
            _flexiaModelNo = 65534;
            _accentModelNo = 65534;
        }

        public LemmaInfo(short flexiaModelNo, short accentModelNo, char[] commonAncode)
        {
            _commonAncode = commonAncode;
            _flexiaModelNo = (ushort) flexiaModelNo;
            _accentModelNo = (ushort) accentModelNo;
        }

        public short AccentModelNo => (short) _accentModelNo;

        public short FlexiaModelNo => (short) _flexiaModelNo;

        public string GetCommonAncodeIfCan => _commonAncode[0] == char.MinValue ? "" : new string(_commonAncode);

        public int CompareTo(LemmaInfo other)
        {
            if (_flexiaModelNo != other._flexiaModelNo)
                return _flexiaModelNo.CompareTo(other._flexiaModelNo);
            var num = new string(_commonAncode).CompareTo(new string(other._commonAncode));
            return num != 0 ? num : _accentModelNo.CompareTo(other._accentModelNo);
        }

        public bool Equals(LemmaInfo other)
        {
            return CompareTo(other) == 0;
        }

        public override string ToString()
        {
            return string.Format("Flexia={0};Accent={1},Ancode={2}", _flexiaModelNo, _accentModelNo,
                GetCommonAncodeIfCan);
        }
    }
}