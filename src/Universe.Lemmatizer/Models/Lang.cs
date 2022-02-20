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
using Universe.Lemmatizer.Implement;

namespace Universe.Lemmatizer.Models
{
    internal static class Lang
    {
        private static readonly LangType[] ASCII = new LangType[256]
        {
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.RusLower | LangType.RusUpper,
            LangType.fWordDelim | LangType.OpnBrck | LangType.URL_CHAR,
            LangType.ClsBrck | LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.URL_CHAR,
            LangType.URL_CHAR,
            LangType.URL_CHAR,
            LangType.URL_CHAR,
            LangType.URL_CHAR,
            LangType.URL_CHAR,
            LangType.URL_CHAR,
            LangType.URL_CHAR,
            LangType.URL_CHAR,
            LangType.URL_CHAR,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.OpnBrck,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.ClsBrck | LangType.fWordDelim,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.URL_CHAR,
            LangType.EngUpper | LangType.GerUpper | LangType.LatinVowel,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.LatinVowel,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.LatinVowel | LangType.UpRomDigits,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.UpRomDigits,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.LatinVowel,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.LatinVowel,
            LangType.EngUpper | LangType.GerUpper | LangType.UpRomDigits,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.UpRomDigits,
            LangType.EngUpper | LangType.GerUpper,
            LangType.EngUpper | LangType.GerUpper,
            LangType.fWordDelim | LangType.OpnBrck,
            LangType.fWordDelim,
            LangType.ClsBrck | LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.EngLower | LangType.GerLower | LangType.LatinVowel | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.LatinVowel | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.LatinVowel | LangType.LwRomDigits | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.LwRomDigits | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.LatinVowel | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.LatinVowel | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.LwRomDigits | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.LwRomDigits | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.EngLower | LangType.GerLower | LangType.URL_CHAR,
            LangType.fWordDelim | LangType.OpnBrck,
            LangType.fWordDelim,
            LangType.ClsBrck | LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.None,
            LangType.fWordDelim,
            LangType.None,
            LangType.None,
            LangType.fWordDelim,
            LangType.None,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.None,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim | LangType.RussianVowel | LangType.RusUpper,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim | LangType.GerLower | LangType.GerUpper,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.RusLower | LangType.RussianVowel,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.fWordDelim,
            LangType.RussianVowel | LangType.RusUpper,
            LangType.RusUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.LatinVowel | LangType.RusUpper,
            LangType.RusUpper,
            LangType.GerUpper | LangType.LatinVowel | LangType.RusUpper,
            LangType.RussianVowel | LangType.RusUpper,
            LangType.RusUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.RusUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.LatinVowel | LangType.RussianVowel | LangType.RusUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.LatinVowel | LangType.RusUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.LatinVowel | LangType.RusUpper,
            LangType.RusUpper,
            LangType.RusUpper,
            LangType.RusUpper,
            LangType.RussianVowel | LangType.RusUpper,
            LangType.RusUpper,
            LangType.RusUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.RusUpper,
            LangType.RusUpper,
            LangType.RussianVowel | LangType.RusUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.LatinVowel | LangType.RusUpper,
            LangType.RusUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.LatinVowel | LangType.RusUpper,
            LangType.RusUpper,
            LangType.RusUpper | LangType.UpRomDigits,
            LangType.RusUpper,
            LangType.RusUpper,
            LangType.EngUpper | LangType.GerUpper | LangType.LatinVowel | LangType.RussianVowel | LangType.RusUpper,
            LangType.GerUpper | LangType.LatinVowel | LangType.RusUpper,
            LangType.RussianVowel | LangType.RusUpper,
            LangType.RussianVowel | LangType.RusUpper,
            LangType.GerLower | LangType.GerUpper | LangType.RussianVowel | LangType.RusUpper,
            LangType.RusLower | LangType.RussianVowel,
            LangType.RusLower,
            LangType.EngLower | LangType.GerLower | LangType.LatinVowel | LangType.RusLower,
            LangType.RusLower,
            LangType.GerLower | LangType.LatinVowel | LangType.RusLower,
            LangType.RusLower | LangType.RussianVowel,
            LangType.RusLower,
            LangType.EngLower | LangType.GerLower | LangType.RusLower,
            LangType.EngLower | LangType.GerLower | LangType.LatinVowel | LangType.RusLower | LangType.RussianVowel,
            LangType.EngLower | LangType.GerLower | LangType.LatinVowel | LangType.RusLower,
            LangType.EngLower | LangType.GerLower | LangType.LatinVowel | LangType.RusLower,
            LangType.RusLower,
            LangType.RusLower,
            LangType.RusLower,
            LangType.RusLower | LangType.RussianVowel,
            LangType.RusLower,
            LangType.RusLower,
            LangType.EngLower | LangType.GerLower | LangType.RusLower,
            LangType.RusLower,
            LangType.RusLower | LangType.RussianVowel,
            LangType.EngLower | LangType.GerLower | LangType.LatinVowel | LangType.RusLower,
            LangType.RusLower,
            LangType.EngLower | LangType.GerLower | LangType.LatinVowel | LangType.RusLower,
            LangType.RusLower,
            LangType.RusLower,
            LangType.RusLower,
            LangType.RusLower,
            LangType.EngLower | LangType.GerLower | LangType.LatinVowel | LangType.RusLower | LangType.RussianVowel,
            LangType.GerLower | LangType.LatinVowel | LangType.RusLower,
            LangType.RusLower | LangType.RussianVowel,
            LangType.RusLower | LangType.RussianVowel,
            LangType.RusLower | LangType.RussianVowel
        };

        internal static bool is_alpha(byte x)
        {
            return is_russian_alpha(x) || is_german_alpha(x);
        }

        internal static bool is_alpha(byte x, InternalMorphLanguage Langua)
        {
            switch (Langua)
            {
                case InternalMorphLanguage.MorphRussian:
                    return is_russian_alpha(x);
                case InternalMorphLanguage.MorphEnglish:
                    return is_english_alpha(x);
                case InternalMorphLanguage.MorphGerman:
                    return is_german_alpha(x);
                case InternalMorphLanguage.MorphGeneric:
                    return is_generic_alpha(x);
                case InternalMorphLanguage.MorphUrl:
                    return is_URL_alpha(x);
                default:
                    throw new MorphException("unknown char x");
            }
        }

        internal static bool is_english_alpha(byte x)
        {
            return is_english_lower(x) || is_english_upper(x);
        }

        internal static bool is_english_lower(byte x)
        {
            return (ASCII[x] & LangType.EngLower) > LangType.None;
        }

        internal static bool is_english_lower_vowel(byte x)
        {
            return (ASCII[x] & LangType.EngLower) > LangType.None && (ASCII[x] & LangType.LatinVowel) > LangType.None;
        }

        internal static bool is_english_upper(byte x)
        {
            return (ASCII[x] & LangType.EngUpper) > LangType.None;
        }

        internal static bool is_english_upper_vowel(byte x)
        {
            return (ASCII[x] & LangType.EngUpper) > LangType.None && (ASCII[x] & LangType.LatinVowel) > LangType.None;
        }

        internal static bool is_generic_alpha(byte x)
        {
            return is_english_alpha(x) || x >= 128;
        }

        internal static bool is_generic_upper(byte x)
        {
            return (ASCII[x] & LangType.EngUpper) > LangType.None;
        }

        internal static bool is_german_alpha(byte x)
        {
            return is_german_lower(x) || is_german_upper(x);
        }

        internal static bool is_german_lower(byte x)
        {
            return (ASCII[x] & LangType.GerLower) > LangType.None;
        }

        internal static bool is_german_lower_vowel(byte x)
        {
            return (ASCII[x] & LangType.GerLower) > LangType.None && (ASCII[x] & LangType.LatinVowel) > LangType.None;
        }

        internal static bool is_german_upper(byte x)
        {
            return (ASCII[x] & LangType.GerUpper) > LangType.None;
        }

        internal static bool is_german_upper_vowel(byte x)
        {
            return (ASCII[x] & LangType.GerUpper) > LangType.None && (ASCII[x] & LangType.LatinVowel) > LangType.None;
        }

        internal static bool is_lower_vowel(byte x, InternalMorphLanguage Langua)
        {
            switch (Langua)
            {
                case InternalMorphLanguage.MorphRussian:
                    return is_russian_lower_vowel(x);
                case InternalMorphLanguage.MorphEnglish:
                    return is_english_lower_vowel(x);
                case InternalMorphLanguage.MorphGerman:
                    return is_german_lower_vowel(x);
                default:
                    return false;
            }
        }

        internal static bool is_russian_alpha(byte x)
        {
            return is_russian_lower(x) || is_russian_upper(x);
        }

        internal static bool is_russian_lower(byte x)
        {
            return (ASCII[x] & LangType.RusLower) > LangType.None;
        }

        internal static bool is_russian_lower_vowel(byte x)
        {
            return (ASCII[x] & LangType.RusLower) > LangType.None && (ASCII[x] & LangType.RussianVowel) > LangType.None;
        }

        internal static bool is_russian_upper(byte x)
        {
            return (ASCII[x] & LangType.RusUpper) > LangType.None;
        }

        internal static bool is_russian_upper_vowel(byte x)
        {
            return (ASCII[x] & LangType.RusUpper) > LangType.None && (ASCII[x] & LangType.RussianVowel) > LangType.None;
        }

        internal static bool is_upper_alpha(byte x, InternalMorphLanguage Langua)
        {
            switch (Langua)
            {
                case InternalMorphLanguage.MorphRussian:
                    return is_russian_upper(x);
                case InternalMorphLanguage.MorphEnglish:
                    return is_english_upper(x);
                case InternalMorphLanguage.MorphGerman:
                    return is_german_upper(x);
                case InternalMorphLanguage.MorphGeneric:
                    return is_generic_upper(x);
                case InternalMorphLanguage.MorphUrl:
                    return false;
                default:
                    return false;
            }
        }

        internal static bool is_upper_consonant(byte x, InternalMorphLanguage Langua)
        {
            return is_upper_alpha(x, Langua) && !is_upper_vowel(x, Langua);
        }

        internal static bool is_upper_vowel(byte x, InternalMorphLanguage Langua)
        {
            switch (Langua)
            {
                case InternalMorphLanguage.MorphRussian:
                    return is_russian_upper_vowel(x);
                case InternalMorphLanguage.MorphEnglish:
                    return is_english_upper_vowel(x);
                case InternalMorphLanguage.MorphGerman:
                    return is_german_upper_vowel(x);
                default:
                    return false;
            }
        }

        internal static bool is_URL_alpha(byte x)
        {
            return (ASCII[x] & LangType.URL_CHAR) > LangType.None;
        }

        [Flags]
        private enum LangType : short
        {
            ClsBrck = 256, // 0x0100
            EngLower = 64, // 0x0040
            EngUpper = 32, // 0x0020
            fWordDelim = 1,
            GerLower = 16, // 0x0010
            GerUpper = 8,
            LatinVowel = 2048, // 0x0800
            LwRomDigits = 1024, // 0x0400
            None = 0,
            OpnBrck = 128, // 0x0080
            RusLower = 4,
            RussianVowel = 4096, // 0x1000
            RusUpper = 2,
            UpRomDigits = 512, // 0x0200
            URL_CHAR = 8192 // 0x2000
        }
    }
}