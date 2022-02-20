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
using Universe.Lemmatizer.Implement.Agramtab;

namespace Universe.Lemmatizer.Implement.MorphWizard
{
    internal class CFlexiaModel
    {
        private string _comments;
        private readonly List<CMorphForm> _flexia = new List<CMorphForm>();

        public int Count => _flexia.Count;

        public string FirstCode
        {
            get
            {
                if (_flexia.Count == 0)
                    throw new FieldAccessException("FirstCode failed");
                return _flexia[0].Gramcode;
            }
        }

        public string FirstFlex
        {
            get
            {
                if (_flexia.Count == 0)
                    throw new FieldAccessException("FirstFlex failed");
                return _flexia[0].FlexiaStr;
            }
        }

        public CMorphForm this[int index] => _flexia[index];

        public override bool Equals(object obj)
        {
            return obj is List<CMorphForm> cmorphFormList
                ? Tools.ListEquals(_flexia, cmorphFormList)
                : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var num = 0;
            foreach (var cmorphForm in _flexia)
                num ^= cmorphForm.GetHashCode();
            return num;
        }

        public bool has_ancode(string search_ancode)
        {
            foreach (var cmorphForm in _flexia)
            {
                var num = cmorphForm.Gramcode.IndexOf(search_ancode);
                if (num > 0 && num % 2 == 0)
                    return true;
            }

            return false;
        }

        public bool ReadFromString(string s)
        {
            s = s.Replace("q//q", "~");
            var strArray = s.Split('~');
            if (strArray.Length != 0)
            {
                var str = strArray[0];
                var separator = new char[1] {'%'};
                foreach (var s1 in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                {
                    var cmorphForm = new CMorphForm();
                    if (!cmorphForm.ReadFromString(s1))
                        return false;
                    _flexia.Add(cmorphForm);
                }
            }

            if (strArray.Length > 1)
                _comments = strArray[1];
            return true;
        }

        public override string ToString()
        {
            var str = "";
            foreach (var cmorphForm in _flexia)
                str = str + "%" + cmorphForm;
            if (!string.IsNullOrEmpty(_comments))
                str = str + "q//q" + _comments;
            return str;
        }
    }
}