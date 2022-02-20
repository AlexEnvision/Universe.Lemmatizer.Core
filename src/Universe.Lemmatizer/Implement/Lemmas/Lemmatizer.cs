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
using System.Text;
using Universe.Lemmatizer.Implement.Agramtab;
using Universe.Lemmatizer.Implement.Morphology;
using Universe.Lemmatizer.Models;
using Universe.Lemmatizer.Types.Collections;

namespace Universe.Lemmatizer.Implement.Lemmas
{
    internal abstract class Lemmatizer : MorphDict, ILemmatizer
    {
        private readonly Set<string> _hyphenPostfixes;

        private readonly Set<string> _hyphenPrefixes;

        private bool _loaded;

        private bool _maximalPrediction;

        private readonly PredictBase _predict;

        public Lemmatizer(InternalMorphLanguage language)
            : base(language)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _hyphenPostfixes = new Set<string>();
            _hyphenPrefixes = new Set<string>();
            Statistic = new Statistic();
            _loaded = false;
            UseStatistic = false;
            _maximalPrediction = false;
            AllowRussianJo = false;
            _predict = new PredictBase(this, language);
            InitAutomat(new MorphAutomat(this, language, '+'));
        }

        public bool AllowRussianJo { get; set; }

        public ICollection<string> HyphenPostfixes => _hyphenPostfixes;

        public ICollection<string> HyphenPrefixes => _hyphenPrefixes;

        public InternalMorphLanguage Language => FormAutomat.Language;

        public Statistic Statistic { get; }

        public bool CheckABC(string wordForm)
        {
            return FormAutomat.CheckABCWithoutAnnotator(wordForm.ToUpper());
        }

        public IParadigmCollection CreateParadigmCollectionFromForm(
            string form,
            bool capital,
            bool usePrediction)
        {
            return CreateParadigmCollectionInner(false, form, capital, usePrediction);
        }

        public IParadigmCollection CreateParadigmCollectionFromNorm(
            string norm,
            bool capital,
            bool usePrediction)
        {
            return CreateParadigmCollectionInner(true, norm, capital, usePrediction);
        }

        public IParadigm CreateParadigmFromID(int id)
        {
            var formInfo = new FormInfo();
            formInfo.AttachLemmatizer(this);
            formInfo.SetParadigmId(id);
            return formInfo;
        }

        public void LoadDictionariesRegistry(FileManager manager)
        {
            _loaded = Load("morph.bin", manager);
            if (!_loaded)
                throw new MorphException("Dictionary not loaded");
            Statistic.Load(this, "l", manager);
            UseStatistic = true;
            _predict.Load("npredict.bin", manager);
            _predict.BuildPredict(LemmaInfos);
            ReadOptions("morph.options", manager);
        }

        public void LoadStatisticRegistry(Subject subj, FileManager manager)
        {
            string prefix;
            switch (subj)
            {
                case Subject.Finance:
                    prefix = "f";
                    break;
                case Subject.Computer:
                    prefix = "c";
                    break;
                case Subject.Literature:
                    prefix = "l";
                    break;
                default:
                    throw new MorphException("Unknown type of Subject");
            }

            Statistic.Load(this, prefix, manager);
        }

        public bool MaximalPrediction
        {
            set => _maximalPrediction = value;
        }

        public bool UseStatistic { get; set; }

        private void AssignWeightIfNeed(IList<AutomAnnotationInner> findResults)
        {
            for (var index = 0; index < findResults.Count; ++index)
            {
                var findResult = findResults[index];
                findResult.Weight =
                    !UseStatistic ? 0 : Statistic.GetHomoWeight(findResult.ParadigmId, findResult.ItemNo);
            }
        }

        private bool CheckAbbreviation(
            string inputWord,
            IList<AutomAnnotationInner> findResults,
            bool isCap)
        {
            for (var index = 0; index < inputWord.Length; ++index)
                if (!Lang.is_upper_consonant(Tools.GetByte(inputWord[index]), Language))
                    return false;
            var predictTupleList = new List<PredictTuple>();
            _predict.Find(FormAutomat.CriticalNounLetterPack, predictTupleList);
            findResults.Add(ConvertPredictTupleToAnnot(predictTupleList[0]));
            return true;
        }

        private AutomAnnotationInner ConvertPredictTupleToAnnot(PredictTuple input)
        {
            return new AutomAnnotationInner
            {
                LemmaInfoNo = input.LemmaInfoNo,
                ModelNo = LemmaInfos[input.LemmaInfoNo].LemmaInfo.FlexiaModelNo,
                Weight = 0,
                PrefixNo = 0,
                ItemNo = input.ItemNo
            };
        }

        private void CreateDecartProduction(
            IList<FormInfo> results1,
            IList<FormInfo> results2,
            IList<FormInfo> results)
        {
            results.Clear();
            for (var index1 = 0; index1 < results1.Count; ++index1)
            for (var index2 = 0; index2 < results2.Count; ++index2)
            {
                var formInfo = (FormInfo) results2[index2].Clone();
                formInfo.SetUserPrefix(results1[index1].GetForm(0) + "-");
                results.Add(formInfo);
            }
        }

        private bool CreateParadigmCollection(
            bool isNorm,
            string inputWord,
            bool capital,
            bool usePrediction,
            List<FormInfo> result)
        {
            result.Clear();
            inputWord = FilterSrc(inputWord);
            inputWord = inputWord.ToUpper();
            var automAnnotationInnerList = new List<AutomAnnotationInner>();
            var found = LemmatizeWord(inputWord, capital, usePrediction, automAnnotationInnerList, true);
            AssignWeightIfNeed(automAnnotationInnerList);
            for (var index = 0; index < automAnnotationInnerList.Count; ++index)
            {
                var a = automAnnotationInnerList[index];
                if (!isNorm || a.ItemNo == 0)
                    result.Add(new FormInfo(this, a, inputWord, found));
            }

            if (!IsFound(result))
            {
                var result1 = new List<FormInfo>();
                var result2 = new List<FormInfo>();
                var length = inputWord.IndexOf('-');
                if (length >= 0)
                {
                    var str1 = inputWord.Substring(0, length);
                    var str2 = inputWord.Substring(length + 1);
                    CreateParadigmCollection(false, str1, capital, false, result1);
                    if (str1 == str2 || IsHyphenPostfix(str2))
                    {
                        result.Clear();
                        result.AddRange(result1);
                    }
                    else if (IsHyphenPrefix(str1))
                    {
                        CreateParadigmCollection(false, str2, capital, false, result2);
                        if (IsFound(result2))
                        {
                            result.Clear();
                            result.AddRange(result2);
                            for (var index = 0; index < result.Count; ++index)
                            {
                                result[index].SetUserPrefix(str1 + "-");
                                result[index].SetUserUnknown();
                            }
                        }
                    }
                    else
                    {
                        CreateParadigmCollection(false, str2, false, false, result2);
                        if (IsFound(result1) && IsFound(result2) && str1.Length > 2 && str2.Length > 2)
                        {
                            CreateDecartProduction(result1, result2, result);
                            for (var index = 0; index < result.Count; ++index)
                                result[index].SetUserUnknown();
                        }
                    }
                }
            }

            return true;
        }

        private IParadigmCollection CreateParadigmCollectionInner(
            bool bNorm,
            string form,
            bool capital,
            bool use_prediction)
        {
            var formInfoList = new List<FormInfo>();
            CreateParadigmCollection(bNorm, form, capital, use_prediction, formInfoList);
            return new ParadigmCollection(formInfoList);
        }

        protected abstract string FilterSrc(string src);

        private bool IsFound(IList<FormInfo> results)
        {
            return results.Count > 0 && results[0].Founded;
        }

        private bool IsHyphenPostfix(string Postfix)
        {
            return _hyphenPostfixes.Contains(Postfix);
        }

        private bool IsHyphenPrefix(string Prefix)
        {
            return _hyphenPrefixes.Contains(Prefix);
        }

        private bool IsPrefix(string prefix)
        {
            return Prefixes.Contains(prefix);
        }

        private bool LemmatizeWord(
            string inputWord,
            bool cap,
            bool predict,
            IList<AutomAnnotationInner> result,
            bool getLemmaInfos)
        {
            var textOffset = 0;
            FormAutomat.GetInnerMorphInfos(inputWord, 0, result);
            var flag = result.Count > 0;
            if ((result.Count == 0) & predict)
            {
                PredictBySuffix(inputWord, out textOffset, 4, result);
                if (inputWord[textOffset - 1] != '-')
                {
                    var num = inputWord.Length - textOffset;
                    var length = textOffset;
                    if (num < 6 && !IsPrefix(inputWord.Substring(0, length)))
                        result.Clear();
                }

                for (var index = 0; index < result.Count; ++index)
                    if (NPSs[result[index].ModelNo] == byte.MaxValue)
                    {
                        result.Clear();
                        break;
                    }
            }

            if (result.Count > 0)
            {
                if (getLemmaInfos)
                    GetLemmaInfos(inputWord, textOffset, result);
                return flag;
            }

            if (predict)
            {
                PredictByDataBase(inputWord, result, cap);
                for (var index = result.Count - 1; index >= 0; --index)
                {
                    var automAnnotationInner = result[index];
                    if (FlexiaModels[automAnnotationInner.ModelNo][automAnnotationInner.ItemNo].FlexiaStr.Length >=
                        inputWord.Length)
                        result.RemoveAt(index);
                }
            }

            return flag;
        }

        private void PredictByDataBase(
            string inputWord,
            IList<AutomAnnotationInner> findResults,
            bool isCap)
        {
            var predictTupleList = new List<PredictTuple>();
            if (CheckAbbreviation(inputWord, findResults, isCap))
                return;
            if (CheckABC(inputWord))
            {
                var charArray = inputWord.ToCharArray();
                Array.Reverse((Array) charArray);
                _predict.Find(new string(charArray), predictTupleList);
            }

            var numArray = new int[32];
            for (var index = 0; index < numArray.Length; ++index)
                numArray[index] = -1;
            for (var index = 0; index < predictTupleList.Count; ++index)
            {
                var partOfSpeechNo = predictTupleList[index].PartOfSpeechNo;
                if (!_maximalPrediction && numArray[partOfSpeechNo] != -1)
                {
                    if (_predict.ModelFreq[findResults[numArray[partOfSpeechNo]].ModelNo] <
                        _predict.ModelFreq[LemmaInfos[predictTupleList[index].LemmaInfoNo].LemmaInfo.FlexiaModelNo])
                        findResults[numArray[partOfSpeechNo]] = ConvertPredictTupleToAnnot(predictTupleList[index]);
                }
                else
                {
                    numArray[partOfSpeechNo] = findResults.Count;
                    findResults.Add(ConvertPredictTupleToAnnot(predictTupleList[index]));
                }
            }

            if (numArray[0] != -1 && (!isCap || Language == InternalMorphLanguage.MorphGerman))
                return;
            _predict.Find(FormAutomat.CriticalNounLetterPack, predictTupleList);
            findResults.Add(ConvertPredictTupleToAnnot(predictTupleList[predictTupleList.Count - 1]));
        }

        private void ReadOptions(string fileName, FileManager manager)
        {
            string result;
            using (var file = manager.GetFile(Registry, fileName))
            {
                Tools.LoadFileToString(file, out result);
            }

            var str1 = result;
            var separator = new char[2] {'\r', '\n'};
            foreach (var str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                if (str2.Trim() == "AllowRussianJo")
                    AllowRussianJo = true;
        }
    }
}