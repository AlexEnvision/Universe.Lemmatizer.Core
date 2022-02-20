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

using System.Collections.Generic;
using Universe.Lemmatizer.Implement.Agramtab;

namespace Universe.Lemmatizer.Implement
{
    internal class PredictBase
    {
        private readonly List<int> _modelFreq = new List<int>();

        private readonly MorphAutomat _suffixAutomat;

        public PredictBase(Lemmatizer lemmatizer, InternalMorphLanguage lang)
        {
            _suffixAutomat = new MorphAutomat(lemmatizer, lang, '+');
        }

        public IList<int> ModelFreq => _modelFreq;

        public void BuildPredict(IList<LemmaInfoAndLemma> lemmaInfos)
        {
            var count = lemmaInfos.Count;
            for (var index = 0; index < count; ++index)
                if (_modelFreq.Count <= lemmaInfos[index].LemmaInfo.FlexiaModelNo)
                {
                    _modelFreq.Add(1);
                }
                else
                {
                    List<int> modelFreq;
                    int flexiaModelNo;
                    (modelFreq = _modelFreq)[flexiaModelNo = lemmaInfos[index].LemmaInfo.FlexiaModelNo] =
                        modelFreq[flexiaModelNo] + 1;
                }
        }

        public bool Find(string lettId, IList<PredictTuple> res)
        {
            var length = lettId.Length;
            var num1 = 0;
            int index;
            for (index = 0; index < length; ++index)
            {
                var num2 = _suffixAutomat.NextNode(num1, lettId[index]);
                if (num2 != -1)
                    num1 = num2;
                else
                    break;
            }

            if (index < 3)
                return false;
            if (num1 == -1)
                throw new MorphException("r == -1");
            FindRecursive(num1, "", res);
            return true;
        }

        private void FindRecursive(int r, string currPath, IList<PredictTuple> infos)
        {
            if (_suffixAutomat.GetNode(r).IsFinal)
            {
                var num1 = currPath.IndexOf(_suffixAutomat.AnnotChar);
                if (num1 < 0)
                    throw new MorphException("i<0");
                var num2 = currPath.IndexOf(_suffixAutomat.AnnotChar, num1 + 1);
                if (num2 < 0)
                    throw new MorphException("j<0");
                var num3 = currPath.IndexOf(_suffixAutomat.AnnotChar, num2 + 1);
                if (num3 < 0)
                    throw new MorphException("k<0");
                var predictTuple = new PredictTuple
                {
                    PartOfSpeechNo =
                        (byte) _suffixAutomat.DecodeFromAlphabet(currPath.Substring(num1 + 1, num2 - num1 - 1)),
                    LemmaInfoNo = _suffixAutomat.DecodeFromAlphabet(currPath.Substring(num2 + 1, num3 - num2 - 1)),
                    ItemNo = (short) _suffixAutomat.DecodeFromAlphabet(currPath.Substring(num3 + 1))
                };
                infos.Add(predictTuple);
            }

            var childrenCount = _suffixAutomat.GetChildrenCount(r);
            var length = currPath.Length;
            var destination = new char[length + 1];
            currPath.CopyTo(0, destination, 0, length);
            for (var index = 0; index < childrenCount; ++index)
            {
                var children = _suffixAutomat.GetChildren(r, index);
                destination[length] = Tools.GetChar(children.RelationalChar);
                FindRecursive(children.ChildNo, new string(destination), infos);
            }
        }

        public void Load(string path, FileManager manager)
        {
            _suffixAutomat.Load(path, manager);
        }
    }
}