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
using System.Text;
using Universe.Lemmatizer.Infrastructure;
using Universe.Lemmatizer.Models;

namespace Universe.Lemmatizer.Implement.Agramtab
{
    internal static class Tools
    {
        private static Encoding _encoding;

        public static Encoding InternalEncoding
        {
            get
            {
                try
                {
                    return _encoding ??= Encoding.GetEncoding(1251);
                }
                catch (Exception)
                {
                    return _encoding ??= Encoding.GetEncoding("windows-1251");
                }
            }
        }

        public static byte GetByte(char ch)
        {
            return InternalEncoding.GetBytes(new char[1]
            {
                ch
            })[0];
        }

        public static char GetChar(byte b)
        {
            return InternalEncoding.GetChars(new byte[1]
            {
                b
            })[0];
        }

        public static string GetStringByLanguage(InternalMorphLanguage langua)
        {
            switch (langua)
            {
                case InternalMorphLanguage.MorphRussian:
                    return "Russian";
                case InternalMorphLanguage.MorphEnglish:
                    return "English";
                case InternalMorphLanguage.MorphGerman:
                    return "German";
                case InternalMorphLanguage.MorphGeneric:
                    return "Generic";
                case InternalMorphLanguage.MorphUrl:
                    return "URL_ABC";
                default:
                    return "unk";
            }
        }

        public static bool ListEquals<T>(IEnumerable<T> l1, IEnumerable<T> l2)
        {
            return ListEquals(l1, l2, null);
        }

        public static bool ListEquals<T>(
            IEnumerable<T> l1,
            IEnumerable<T> l2,
            IEqualityComparer<T> comparer)
        {
            if (!Equals(l1, l2))
            {
                if (comparer == null)
                    comparer = EqualityComparer<T>.Default;
                if (l1 == null)
                    throw new ArgumentNullException(nameof(l1));
                if (l2 == null)
                    throw new ArgumentNullException(nameof(l2));
                using (var enumerator1 = l1.GetEnumerator())
                {
                    using (var enumerator2 = l2.GetEnumerator())
                    {
                        while (enumerator1.MoveNext())
                            if (!enumerator2.MoveNext() || !comparer.Equals(enumerator1.Current, enumerator2.Current))
                                return false;
                        if (enumerator2.MoveNext())
                            return false;
                    }
                }
            }

            return true;
        }

        public static bool LoadFileToString(Stream stream, out string result)
        {
            try
            {
                result = new StreamReader(stream, InternalEncoding).ReadToEnd();
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static void LoadList<T>(Stream stream, ICollection<T> list) where T : ILoad, new()
        {
            while (stream.CanRead)
            {
                var obj = new T();
                if (!obj.Load(stream))
                    break;
                list.Add(obj);
            }
        }

        public static string ReadLine(BinaryReader reader)
        {
            var stringBuilder = new StringBuilder(80);
            while (true)
            {
                char ch;
                do
                {
                    ch = reader.ReadChar();
                    if (ch == '\n')
                        goto label_3;
                } while (ch == '\r');

                stringBuilder.Append(ch);
            }

            label_3:
            return stringBuilder.ToString();
        }

        public static byte TransferReverseVowelNoToCharNo(
            string form,
            byte accentCharNo,
            InternalMorphLanguage language)
        {
            if (accentCharNo != byte.MaxValue)
            {
                if (accentCharNo >= form.Length)
                    throw new MorphException("AccentCharNo >= form.Length");
                var num = -1;
                var index = form.Length - 1;
                if (index >= byte.MaxValue)
                    throw new MorphException("i >= UnknownAccent");
                for (; index >= 0; --index)
                {
                    if (Lang.is_lower_vowel(GetByte(form[index]), language) ||
                        Lang.is_upper_vowel(GetByte(form[index]), language))
                        ++num;
                    if (num == accentCharNo)
                        return (byte) index;
                }
            }

            return byte.MaxValue;
        }
    }
}