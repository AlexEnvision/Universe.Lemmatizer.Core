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
using Universe.Lemmatizer.Implement.Encoders;
using Universe.Lemmatizer.Models;

namespace Universe.Lemmatizer.Implement.Morphology
{
    internal class MorphAutomat : ABCEncoder
    {
        private int[] _childrenCache;

        private MorphAutomNode[] _nodes;

        private MorphAutomRelation[] _relations;

        public MorphAutomat(Lemmas.Lemmatizer lemmatizer, InternalMorphLanguage language, char annotChar)
            : base(lemmatizer, language, annotChar)
        {
        }

        private void BuildChildrenCache()
        {
            var num = 1000;
            if (_nodes.Length < 1000)
                num = _nodes.Length;
            var length = num * 54;
            _childrenCache = new int[length];
            for (var index = 0; index < length; ++index)
                _childrenCache[index] = -1;
            for (var nodeNo = 0; nodeNo < num; ++nodeNo)
            {
                var childrenCount = GetChildrenCount(nodeNo);
                var childrenStart = _nodes[nodeNo].ChildrenStart;
                for (var index = 0; index < childrenCount; ++index)
                    _childrenCache[nodeNo * 54 + _alphabet2Code[_relations[index + childrenStart].RelationalChar]] =
                        _relations[index + childrenStart].ChildNo;
            }
        }

        private void Clear()
        {
            _nodes = null;
            _relations = null;
        }

        public void DecodeMorphAutomatInfo(
            int info,
            out int modelNo,
            out int itemNo,
            out int prefixNo)
        {
            modelNo = info >> 18;
            itemNo = (262143 & info) >> 9;
            prefixNo = 511 & info;
        }

        public int EncodeMorphAutomatInfo(int modelNo, int itemNo, int prefixNo)
        {
            return ((ushort) (modelNo & ushort.MaxValue) << 18) | ((ushort) (itemNo & ushort.MaxValue) << 9) | prefixNo;
        }

        private int FindStringAndPassAnnotChar(string text, int pos)
        {
            var length = text.Length;
            var nodeNo = 0;
            for (var index = pos; index < length; ++index)
            {
                var num = NextNode(nodeNo, text[index]);
                if (num == -1)
                    return -1;
                nodeNo = num;
            }

            return NextNode(nodeNo, AnnotChar);
        }

        private void GetAllMorphInterpsRecursive(
            int nodeNo,
            string currPath,
            IList<AutomAnnotationInner> infos)
        {
            if (_nodes[nodeNo].IsFinal)
            {
                var automAnnotationInner = new AutomAnnotationInner();
                int modelNo;
                int itemNo;
                int prefixNo;
                DecodeMorphAutomatInfo(DecodeFromAlphabet(currPath), out modelNo, out itemNo, out prefixNo);
                automAnnotationInner.ItemNo = (short) itemNo;
                automAnnotationInner.ModelNo = (short) modelNo;
                automAnnotationInner.PrefixNo = (short) prefixNo;
                infos.Add(automAnnotationInner);
            }

            var childrenCount = GetChildrenCount(nodeNo);
            var length = currPath.Length;
            var destination = new char[length + 1];
            currPath.CopyTo(0, destination, 0, length);
            for (var index = 0; index < childrenCount; ++index)
            {
                var children = GetChildren(nodeNo, index);
                destination[length] = Tools.GetChar(children.RelationalChar);
                GetAllMorphInterpsRecursive(children.ChildNo, new string(destination), infos);
            }
        }

        public MorphAutomRelation GetChildren(int nodeNo, int index)
        {
            return _relations[_nodes[nodeNo].ChildrenStart + index];
        }

        public int GetChildrenCount(int nodeNo)
        {
            return nodeNo + 1 == _nodes.Length
                ? _relations.Length - _nodes[nodeNo].ChildrenStart
                : _nodes[nodeNo + 1].ChildrenStart - _nodes[nodeNo].ChildrenStart;
        }

        public string GetFirstResult(string text)
        {
            var nodeNo = FindStringAndPassAnnotChar(text, 0);
            if (nodeNo == -1)
                return "";
            var str = "";
            MorphAutomRelation children;
            for (; !_nodes[nodeNo].IsFinal; nodeNo = children.ChildNo)
            {
                children = GetChildren(nodeNo, 0);
                str += (string) (object) children.RelationalChar;
            }

            return str;
        }

        public void GetInnerMorphInfos(string text, int textPos, IList<AutomAnnotationInner> infos)
        {
            infos.Clear();
            var andPassAnnotChar = FindStringAndPassAnnotChar(text, textPos);
            if (andPassAnnotChar == -1)
                return;
            GetAllMorphInterpsRecursive(andPassAnnotChar, "", infos);
        }

        public MorphAutomNode GetNode(int nodeNo)
        {
            return _nodes[nodeNo];
        }

        public bool Load(string grammarFileName, FileManager manager)
        {
            Clear();
            using (var file = manager.GetFile(Lemmatizer.Registry, grammarFileName))
            {
                var reader = new BinaryReader(file, Tools.InternalEncoding);
                var s1 = Tools.ReadLine(reader);
                if (s1 == null)
                    return false;
                int result;
                int.TryParse(s1, out result);
                if (result <= 0)
                    return false;
                if (_nodes != null)
                    throw new MorphException("_nodes != null");
                var count1 = result * 4;
                var numArray1 = reader.ReadBytes(count1);
                if (numArray1.Length != count1)
                    return false;
                var numArray2 = new int[result];
                Buffer.BlockCopy(numArray1, 0, numArray2, 0, numArray1.Length);
                _nodes = new MorphAutomNode[result];
                for (var index = 0; index < result; ++index)
                    _nodes[index] = new MorphAutomNode
                    {
                        Data = numArray2[index]
                    };
                var s2 = Tools.ReadLine(reader);
                if (s2 == null)
                    return false;
                int.TryParse(s2, out result);
                var count2 = result * 4;
                var numArray3 = reader.ReadBytes(count2);
                if (numArray3.Length != count2)
                    return false;
                var numArray4 = new int[result];
                Buffer.BlockCopy(numArray3, 0, numArray4, 0, numArray3.Length);
                _relations = new MorphAutomRelation[result];
                for (var index = 0; index < result; ++index)
                    _relations[index] = new MorphAutomRelation
                    {
                        Data = numArray4[index]
                    };
                var count3 = 1024;
                var numArray5 = reader.ReadBytes(count3);
                if (numArray5.Length != count3)
                    return false;
                var numArray6 = new int[256];
                Buffer.BlockCopy(numArray5, 0, numArray6, 0, numArray5.Length);
                if (!Tools.ListEquals(numArray6, _alphabet2Code))
                    throw new MorphException(Tools.GetStringByLanguage(Language) +
                                             "alphabet has changed; cannot load morph automat");
            }

            BuildChildrenCache();
            return true;
        }

        public int NextNode(int nodeNo, char relationChar)
        {
            if (nodeNo < 1000)
            {
                var num = _alphabet2Code[Tools.GetByte(relationChar)];
                return num == -1 ? -1 : _childrenCache[nodeNo * 54 + num];
            }

            var num1 = Tools.GetByte(relationChar);
            var childrenStart = _nodes[nodeNo].ChildrenStart;
            var childrenCount = GetChildrenCount(nodeNo);
            for (var index = 0; index < childrenCount; ++index)
                if (num1 == _relations[index + childrenStart].RelationalChar)
                    return _relations[index + childrenStart].ChildNo;
            return -1;
        }
    }
}