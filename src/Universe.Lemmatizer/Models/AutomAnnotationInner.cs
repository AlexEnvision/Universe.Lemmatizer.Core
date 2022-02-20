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

namespace Universe.Lemmatizer.Models
{
    internal class AutomAnnotationInner
    {
        private ushort _itemNo;

        private uint _lemmaInfoNo;

        private ushort _modelNo;

        private ushort _prefixNo;

        private uint _weight;

        public short ItemNo
        {
            get => (short) _itemNo;
            set => _itemNo = (ushort) value;
        }

        public int LemmaInfoNo
        {
            get => (int) _lemmaInfoNo;
            set => _lemmaInfoNo = (uint) value;
        }

        public short ModelNo
        {
            get => (short) _modelNo;
            set => _modelNo = (ushort) value;
        }

        public int ParadigmId => (_prefixNo << 23) | (int) _lemmaInfoNo;

        public short PrefixNo
        {
            get => (short) _prefixNo;
            set => _prefixNo = (ushort) value;
        }

        public int Weight
        {
            get => (int) _weight;
            set => _weight = (uint) value;
        }

        public void SplitParadigmId(int value)
        {
            _prefixNo = (ushort) (value >> 23);
            _lemmaInfoNo = (uint) (value & 8388607);
        }
    }
}