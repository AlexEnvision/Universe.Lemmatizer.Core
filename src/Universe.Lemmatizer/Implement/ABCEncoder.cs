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

namespace Universe.Lemmatizer.Implement
{
    internal class ABCEncoder
    {
        protected int[] _alphabet2Code = new int[256];

        private readonly int[] _alphabet2CodeWithoutAnnotator = new int[256];

        private readonly int _alphabetSize;

        private readonly int _alphabetSizeWithoutAnnotator;

        private readonly int[] _code2Alphabet = new int[54];

        private readonly int[] _code2AlphabetWithoutAnnotator = new int[54];

        public ABCEncoder(Lemmatizer lemmatizer, InternalMorphLanguage language, char annotChar)
        {
            Lemmatizer = lemmatizer;
            AnnotChar = annotChar;
            _alphabetSize = InitAlphabet(language, _code2Alphabet, _alphabet2Code, AnnotChar);
            _alphabetSizeWithoutAnnotator = InitAlphabet(language, _code2AlphabetWithoutAnnotator,
                _alphabet2CodeWithoutAnnotator, 'ā');
            if (_alphabetSizeWithoutAnnotator + 1 != _alphabetSize)
                throw new MorphException("_alphabetSizeWithoutAnnotator + 1 != _alphabetSize");
        }

        public char AnnotChar { get; }

        public string CriticalNounLetterPack => new string(AnnotChar, 3);

        public InternalMorphLanguage Language => Lemmatizer.Language;

        public Lemmatizer Lemmatizer { get; }

        public bool CheckABCWithAnnotator(string wordForm)
        {
            var length = wordForm.Length;
            for (var index = 0; index < length; ++index)
                if (_alphabet2Code[Tools.GetByte(wordForm[index])] == -1)
                    return false;
            return true;
        }

        public bool CheckABCWithoutAnnotator(string wordForm)
        {
            var length = wordForm.Length;
            for (var index = 0; index < length; ++index)
                if (_alphabet2CodeWithoutAnnotator[Tools.GetByte(wordForm[index])] == -1)
                    return false;
            return true;
        }

        public int DecodeFromAlphabet(string v)
        {
            var length = v.Length;
            var num1 = 1;
            var num2 = 0;
            for (var index = 0; index < length; ++index)
            {
                num2 += _alphabet2CodeWithoutAnnotator[Tools.GetByte(v[index])] * num1;
                num1 *= _alphabetSizeWithoutAnnotator;
            }

            return num2;
        }

        private static int InitAlphabet(
            InternalMorphLanguage language,
            int[] pCode2Alphabet,
            int[] pAlphabet2Code,
            char annotChar)
        {
            if (char.IsUpper(annotChar))
                throw new MorphException("annotChar is not upper");
            var str1 = "'1234567890";
            var str2 = "";
            var index1 = 0;
            for (var index2 = 0; index2 < 256; ++index2)
            {
                var ch = Convert.ToChar(index2);
                if (Lang.is_upper_alpha((byte) index2, language) || ch == '-' || ch == annotChar ||
                    language == InternalMorphLanguage.morphEnglish && str1.IndexOf(ch) >= 0 ||
                    language == InternalMorphLanguage.morphGerman && str2.IndexOf(ch) >= 0 ||
                    language == InternalMorphLanguage.morphURL && Lang.is_alpha((byte) index2, language))
                {
                    pCode2Alphabet[index1] = index2;
                    pAlphabet2Code[index2] = index1;
                    ++index1;
                }
                else
                {
                    pAlphabet2Code[index2] = -1;
                }
            }

            if (index1 > 54)
                throw new MorphException("Error! The  ABC is too large");
            return index1;
        }
    }
}