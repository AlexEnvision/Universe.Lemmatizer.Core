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

namespace Universe.Lemmatizer.Implement.MorphWizard
{
    internal class CMorphForm
    {
        private string _prefixStr;

        public CMorphForm()
        {
        }

        public CMorphForm(string gramcode, string flexiaStr, string prefixStr)
        {
            Gramcode = gramcode;
            FlexiaStr = flexiaStr;
            _prefixStr = prefixStr;
            if (string.IsNullOrEmpty(Gramcode))
                throw new ArgumentNullException(nameof(Gramcode));
        }

        public string FlexiaStr { get; private set; }

        public string Gramcode { get; private set; }

        public string PrefixStr => _prefixStr ?? "";

        public override bool Equals(object obj)
        {
            if (!(obj is CMorphForm cmorphForm))
                return base.Equals(obj);
            return Gramcode == cmorphForm.Gramcode && FlexiaStr == cmorphForm.FlexiaStr &&
                   _prefixStr == cmorphForm._prefixStr;
        }

        public override int GetHashCode()
        {
            return Gramcode.GetHashCode() ^ (FlexiaStr ?? "").GetHashCode() ^ (_prefixStr ?? "").GetHashCode();
        }

        public bool ReadFromString(string s)
        {
            var strArray = s.Split('*');
            if (strArray.Length < 1)
                return false;
            FlexiaStr = strArray[0];
            if (strArray.Length > 1)
                Gramcode = strArray[1];
            if (strArray.Length > 2)
                _prefixStr = strArray[2];
            return true;
        }

        public override string ToString()
        {
            var str = FlexiaStr + "*" + Gramcode;
            if (!string.IsNullOrEmpty(_prefixStr))
                str = str + "*" + _prefixStr;
            return str;
        }
    }
}