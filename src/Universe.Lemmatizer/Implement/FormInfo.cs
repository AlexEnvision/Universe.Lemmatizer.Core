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
using Universe.Lemmatizer.Implement.MorphWizard;

namespace Universe.Lemmatizer.Implement
{
    internal class FormInfo : IParadigm, ICloneable
    {
        private bool _flexiaWasCut;

        private AutomAnnotationInner _innerAnnot;

        private Lemmatizer _parent;

        private bool _prefixesWereCut;

        private string _userPrefix;

        public FormInfo()
        {
            Founded = true;
        }

        public FormInfo(Lemmatizer parent, AutomAnnotationInner a, string inputWordForm, bool found)
        {
            _innerAnnot = a;
            _parent = parent;
            Founded = found;
            InputWordBase = inputWordForm;
            var cmorphForm = FlexiaModel[a.ItemNo];
            var length = cmorphForm.FlexiaStr.Length;
            if (Founded || InputWordBase.Length >= length &&
                InputWordBase.Substring(InputWordBase.Length - length) == cmorphForm.FlexiaStr)
            {
                _flexiaWasCut = true;
                var startIndex = InputWordBase.Length - cmorphForm.FlexiaStr.Length;
                if (startIndex < InputWordBase.Length)
                    InputWordBase = InputWordBase.Remove(startIndex);
            }
            else
            {
                _flexiaWasCut = false;
            }

            var prefix = _parent.Prefixes[_innerAnnot.PrefixNo];
            if (Founded || InputWordBase.Substring(0, prefix.Length) == prefix &&
                InputWordBase.Substring(prefix.Length, cmorphForm.PrefixStr.Length) == cmorphForm.PrefixStr)
            {
                InputWordBase = InputWordBase.Remove(0, prefix.Length + cmorphForm.PrefixStr.Length);
                _prefixesWereCut = true;
            }
            else
            {
                _prefixesWereCut = false;
            }
        }

        private CFlexiaModel FlexiaModel => _parent.FlexiaModels[LemmaInfo.LemmaInfo.FlexiaModelNo];

        public string InputWordBase { get; private set; }

        private bool IsValid => _parent != null && _innerAnnot.LemmaInfoNo != -1;

        private LemmaInfoAndLemma LemmaInfo => _parent.LemmaInfos[_innerAnnot.LemmaInfoNo];

        public char LemSign => !Founded ? '-' : '+';

        public int SrcAccentedVowel => GetAccent(_innerAnnot.ItemNo);

        public object Clone()
        {
            return new FormInfo
            {
                _flexiaWasCut = _flexiaWasCut,
                Founded = Founded,
                _prefixesWereCut = _prefixesWereCut,
                _innerAnnot = _innerAnnot,
                InputWordBase = InputWordBase,
                _parent = _parent,
                _userPrefix = _userPrefix
            };
        }

        public int GetAccent(int index)
        {
            if (!Founded || !IsValid || LemmaInfo.LemmaInfo.AccentModelNo == -2)
                return byte.MaxValue;
            var accentCharNo = _parent.AccentModels[LemmaInfo.LemmaInfo.AccentModelNo][index];
            return Tools.TransferReverseVowelNoToCharNo(GetForm(index).ToLower(), accentCharNo, _parent.Language);
        }

        public string GetAncode(int index)
        {
            return !IsValid ? "" : FlexiaModel[index].Gramcode.Substring(0, 2);
        }

        public string GetForm(int index)
        {
            if (!IsValid)
                return "";
            var cmorphForm = FlexiaModel[index];
            var str1 = _userPrefix ?? "";
            if (_prefixesWereCut)
                str1 = str1 + _parent.Prefixes[_innerAnnot.PrefixNo] + cmorphForm.PrefixStr;
            var str2 = str1 + InputWordBase;
            if (_flexiaWasCut)
                str2 += cmorphForm.FlexiaStr;
            return str2;
        }

        public int HomonymWeightWithForm(int index)
        {
            return !IsValid ? 0 : _parent.Statistic.GetHomoWeight(ParadigmID, index);
        }

        public int BaseLength => InputWordBase.Length;

        public int Count => !IsValid ? 0 : FlexiaModel.Count;

        public bool Founded { get; private set; }

        public int HomonymWeight => !IsValid || !Founded ? 0 : _innerAnnot.Weight;

        public int LemmaPrefixLength => !IsValid || !Founded ? 0 : _parent.Prefixes[_innerAnnot.PrefixNo].Length;

        public string Norm => GetForm(0);

        public int ParadigmID
        {
            get => !IsValid || !Founded ? -1 : _innerAnnot.ParadigmId;
            set => SetParadigmId(value);
        }

        public string SrcAncode => !IsValid ? "" : FlexiaModel[_innerAnnot.ItemNo].Gramcode;

        public string SrcNorm => !IsValid ? "" : _parent.Bases[LemmaInfo.LemmaStrNo] + FlexiaModel.FirstFlex;

        public string TypeAncode
        {
            get
            {
                if (IsValid)
                {
                    var commonAncodeIfCan = LemmaInfo.LemmaInfo.GetCommonAncodeIfCan;
                    if (!string.IsNullOrEmpty(commonAncodeIfCan))
                        return commonAncodeIfCan;
                }

                return "??";
            }
        }

        public int WordWeight => !IsValid ? 0 : _parent.Statistic.GetWordWeight(ParadigmID);

        public void AttachLemmatizer(Lemmatizer parent)
        {
            _parent = parent;
        }

        public bool SetParadigmId(int newVal)
        {
            if (_parent == null)
                throw new MorphException("_parent == null");
            var automAnnotationInner = new AutomAnnotationInner();
            automAnnotationInner.SplitParadigmId(newVal);
            if (automAnnotationInner.LemmaInfoNo > _parent.LemmaInfos.Count ||
                automAnnotationInner.PrefixNo > _parent.Prefixes.Count)
                return false;
            automAnnotationInner.ItemNo = 0;
            automAnnotationInner.Weight = _parent.Statistic.GetHomoWeight(automAnnotationInner.ParadigmId, 0);
            automAnnotationInner.ModelNo = _parent.LemmaInfos[automAnnotationInner.LemmaInfoNo].LemmaInfo.FlexiaModelNo;
            _innerAnnot = automAnnotationInner;
            _prefixesWereCut = true;
            _flexiaWasCut = true;
            Founded = true;
            InputWordBase = SrcNorm;
            InputWordBase = InputWordBase.Remove(InputWordBase.Length - FlexiaModel.FirstFlex.Length);
            return true;
        }

        public void SetUserPrefix(string userPrefix)
        {
            _userPrefix = userPrefix;
        }

        public void SetUserUnknown()
        {
            Founded = false;
        }

        public override string ToString()
        {
            var str = LemSign + TypeAncode + " " + GetForm(0) + " " + SrcAncode;
            return Founded ? str + string.Format(" {0} {1}", ParadigmID, HomonymWeight) : str + " -1 0";
        }
    }
}