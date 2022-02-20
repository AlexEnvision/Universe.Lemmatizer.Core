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
using Universe.Lemmatizer.Implement.Helpers;
using Universe.Lemmatizer.Implement.Lemmas;
using Universe.Lemmatizer.Implement.MorphWizard;
using Universe.Lemmatizer.Models;

namespace Universe.Lemmatizer.Implement.Morphology
{
    internal class MorphDict
    {
        private readonly List<CAccentModel> _accentModels = new List<CAccentModel>();
        private readonly StringHolder _bases = new StringHolder();
        private readonly InternalComparer _comparer;
        private readonly List<CFlexiaModel> _flexiaModels = new List<CFlexiaModel>();
        private readonly List<LemmaInfoAndLemma> _lemmaInfos = new List<LemmaInfoAndLemma>();
        private FileManager _manager;
        private int[] _modelsIndex;
        private readonly List<string> _prefixes = new List<string>();

        protected MorphDict(InternalMorphLanguage language)
        {
            _comparer = new InternalComparer(_bases);
        }

        public IList<CAccentModel> AccentModels => _accentModels;

        public IList<string> Bases => _bases;

        public IList<CFlexiaModel> FlexiaModels => _flexiaModels;

        protected MorphAutomat FormAutomat { get; private set; }

        public IList<LemmaInfoAndLemma> LemmaInfos => _lemmaInfos;

        protected byte[] NPSs { get; private set; }

        public IList<string> Prefixes => _prefixes;

        public string Registry { get; protected set; }

        private void CreateModelsIndex()
        {
            if (_lemmaInfos.Count == 0)
                return;
            _modelsIndex = new int[_flexiaModels.Count + 1];
            var flexiaModelNo1 = (int) _lemmaInfos[0].LemmaInfo.FlexiaModelNo;
            _modelsIndex[flexiaModelNo1] = 0;
            for (var index = 0; index < _lemmaInfos.Count; ++index)
            for (; flexiaModelNo1 < (int) _lemmaInfos[index].LemmaInfo.FlexiaModelNo; ++flexiaModelNo1)
            {
                var flexiaModelNo2 = (int) _lemmaInfos[index].LemmaInfo.FlexiaModelNo;
                _modelsIndex[flexiaModelNo1 + 1] = index;
            }

            for (; flexiaModelNo1 < _flexiaModels.Count; ++flexiaModelNo1)
                _modelsIndex[flexiaModelNo1 + 1] = _lemmaInfos.Count;
            for (var index = 0; index < _lemmaInfos.Count; ++index)
            {
                var flexiaModelNo2 = (int) _lemmaInfos[index].LemmaInfo.FlexiaModelNo;
                if (_modelsIndex[flexiaModelNo2] > index)
                    throw new MorphException("_modelsIndex[debug] > i");
                if (index >= _modelsIndex[flexiaModelNo2 + 1])
                    throw new MorphException("i >= _modelsIndex[debug + 1");
            }
        }

        protected void GetLemmaInfos(string text, int textPos, IList<AutomAnnotationInner> infos)
        {
            var count = infos.Count;
            for (var index1 = 0; index1 < count; ++index1)
            {
                var info = infos[index1];
                var cmorphForm = _flexiaModels[info.ModelNo][info.ItemNo];
                var startIndex = textPos + _prefixes[info.PrefixNo].Length + cmorphForm.PrefixStr.Length;
                var search = _prefixes[info.PrefixNo] +
                             text.Substring(startIndex, text.Length - startIndex - cmorphForm.FlexiaStr.Length);
                var index2 = _modelsIndex[info.ModelNo];
                var num = _modelsIndex[info.ModelNo + 1];
                var index3 = _lemmaInfos.BinarySearch(index2, num - index2, new InternalLemmaInfoAndLemma(search),
                    _comparer);
                if (index3 < 0)
                    index3 = index3 * -1 - 1;
                if (index3 >= _lemmaInfos.Count)
                    throw new IndexOutOfRangeException();
                if (_bases[_lemmaInfos[index3].LemmaStrNo] != search)
                    throw new MorphException("LemmaStrNo!=Base");
                info.LemmaInfoNo = index3;
            }
        }

        protected void InitAutomat(MorphAutomat formAutomat)
        {
            if (FormAutomat != null)
                throw new InvalidOperationException("InitAutomat already done");
            if (formAutomat == null)
                throw new ArgumentNullException(nameof(formAutomat));
            FormAutomat = formAutomat;
        }

        protected bool Load(string grammarFileName, FileManager manager)
        {
            if (!FormAutomat.Load(MakeFName(grammarFileName, "forms_autom"), manager))
                return false;
            var name = MakeFName(grammarFileName, "annot");
            Stream file1;
            using (file1 = manager.GetFile(Registry, name))
            {
                var reader = new BinaryReader(file1, Tools.InternalEncoding);
                ReadFlexiaModels(reader, _flexiaModels);
                ReadAccentModels(reader, _accentModels);
                var s1 = Tools.ReadLine(reader);
                if (s1 == null)
                    return false;
                int result;
                int.TryParse(s1, out result);
                _prefixes.Clear();
                _prefixes.Add("");
                for (var index = 0; index < result; ++index)
                {
                    var str1 = Tools.ReadLine(reader);
                    if (str1 == null)
                        return false;
                    var str2 = str1.Trim();
                    if (string.IsNullOrEmpty(str2))
                        throw new Exception("line is empty");
                    _prefixes.Add(str2);
                }

                var s2 = Tools.ReadLine(reader);
                if (s2 == null)
                    return false;
                int.TryParse(s2, out result);
                if (!ReadLemmas(reader, result))
                    return false;
                var s3 = Tools.ReadLine(reader);
                if (s3 == null)
                    return false;
                int.TryParse(s3, out result);
                NPSs = reader.ReadBytes(result);
                if (NPSs.Length != result)
                    return false;
                if (NPSs.Length != _flexiaModels.Count)
                    throw new MorphException("_NPSs.Count!=_FlexiaModels.Count");
            }

            Stream file2;
            using (file2 = manager.GetFile(Registry, MakeFName(grammarFileName, "bases")))
            {
                _bases.ReadShortStringHolder(file2);
            }

            CreateModelsIndex();
            return true;
        }

        public void AddToBases(List<string> newRecords)
        {
            _bases.InsertRange(0, newRecords);
        }

        private string MakeFName(string inpitFileName, string ext)
        {
            var startIndex = inpitFileName.LastIndexOf('.');
            if (startIndex >= 0)
                inpitFileName = inpitFileName.Remove(startIndex);
            return inpitFileName + "." + ext;
        }

        protected void PredictBySuffix(
            string text,
            out int textOffset,
            int minimalPredictSuffixlen,
            IList<AutomAnnotationInner> infos)
        {
            var length = text.Length;
            textOffset = 1;
            while (textOffset + minimalPredictSuffixlen <= length)
            {
                FormAutomat.GetInnerMorphInfos(text, textOffset, infos);
                if (infos.Count > 0)
                    break;
                ++textOffset;
            }
        }

        private void ReadAccentModels(BinaryReader reader, IList<CAccentModel> accentModels)
        {
            var s1 = Tools.ReadLine(reader);
            if (s1 == null)
                throw new MorphException("Cannot read accent models from mrd file");
            accentModels.Clear();
            int result;
            int.TryParse(s1, out result);
            for (var index = 0; index < result; ++index)
            {
                var str = Tools.ReadLine(reader);
                if (str == null)
                    throw new MorphException("Too few lines in mrd file");
                var s2 = str.Trim();
                var caccentModel = new CAccentModel();
                if (!caccentModel.ReadFromString(s2))
                    throw new MorphException("Cannot parse line " + s2);
                accentModels.Add(caccentModel);
            }
        }

        private void ReadFlexiaModels(BinaryReader reader, IList<CFlexiaModel> flexiaModels)
        {
            var s1 = Tools.ReadLine(reader);
            if (s1 == null)
                throw new MorphException("Cannot parse mrd file");
            flexiaModels.Clear();
            int result;
            int.TryParse(s1, out result);
            for (var index = 0; index < result; ++index)
            {
                var str = Tools.ReadLine(reader);
                if (str == null)
                    throw new MorphException("Too few lines in mrd file");
                var s2 = str.Trim();
                var cflexiaModel = new CFlexiaModel();
                if (!cflexiaModel.ReadFromString(s2))
                    throw new MorphException("Cannot parse paradigm No " + (index + 1));
                flexiaModels.Add(cflexiaModel);
            }
        }

        private bool ReadLemmas(BinaryReader reader, int count)
        {
            _lemmaInfos.Clear();
            for (var index = 0; index < count; ++index)
            {
                var flexiaModelNo = reader.ReadInt16();
                var accentModelNo = reader.ReadInt16();
                var commonAncode = reader.ReadChars(2);
                var num = (int) reader.ReadInt16();
                _lemmaInfos.Add(new LemmaInfoAndLemma(reader.ReadInt32(), flexiaModelNo, accentModelNo, commonAncode));
            }

            return true;
        }

        private class InternalComparer : IComparer<LemmaInfoAndLemma>
        {
            private readonly StringHolder _searchInfos;

            public InternalComparer(StringHolder searchInfos)
            {
                _searchInfos = searchInfos;
            }

            public int Compare(LemmaInfoAndLemma x, LemmaInfoAndLemma y)
            {
                if (x == null || y == null)
                    throw new ArgumentNullException();
                var lemmaInfoAndLemma1 = x as InternalLemmaInfoAndLemma;
                var lemmaInfoAndLemma2 = y as InternalLemmaInfoAndLemma;
                return string.CompareOrdinal(
                    lemmaInfoAndLemma1 != null ? lemmaInfoAndLemma1.Search : _searchInfos[x.LemmaStrNo],
                    lemmaInfoAndLemma2 != null ? lemmaInfoAndLemma2.Search : _searchInfos[y.LemmaStrNo]);
            }
        }

        private class InternalLemmaInfoAndLemma : LemmaInfoAndLemma
        {
            public InternalLemmaInfoAndLemma(string search)
                : base(0, 0, 0, null)
            {
                Search = search;
            }

            public string Search { get; }
        }
    }
}