
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Linq;
#endif
using JP.Notek.AtomicSoup.VRCCollection;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
namespace JP.Notek.AtomicSoup
{
    [UdonSharpProgramAsset]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [DIProvide(typeof(AtomHashSet), "PrimaryTargetNodes")]
    [DIProvide(typeof(AtomHashSet), "SecondaryTargetNodes")]
    public class AtomGraphManager : UdonSharpBehaviour
    {
        DataDictionary _PrimaryEdges = new DataDictionary();
        AtomSubscriber[][] _PrimaryEdgeValues = new AtomSubscriber[0][];
        [SerializeField, DIInject("PrimaryTargetNodes")] AtomHashSet _PrimaryTargetNodes;
        DataDictionary _SecondaryEdges = new DataDictionary();
        AtomSubscriber[][] _SecondaryEdgeValues = new AtomSubscriber[0][];
        [SerializeField, DIInject("SecondaryTargetNodes")] AtomHashSet _SecondaryTargetNodes;
        DataDictionary _InDegree = new DataDictionary();

        // 事前計算用のシリアライズフィールド
        [SerializeField] AtomSubscriber[] _IntermidiateTargetsFlatten = new AtomSubscriber[0];
        [SerializeField] AtomSubscriber[] _IntermidiateDependenciesFlatten = new AtomSubscriber[0];
        [SerializeField] AtomSubscriber[] _ViewTargetsFlatten = new AtomSubscriber[0];
        [SerializeField] AtomSubscriber[] _ViewDependenciesFlatten = new AtomSubscriber[0];
        [SerializeField] AtomSubscriber[] _SyncTargetsFlatten = new AtomSubscriber[0];
        [SerializeField] AtomSubscriber[] _SyncDependenciesFlatten = new AtomSubscriber[0];
        [SerializeField] AtomSubscriber[] _InDegreeKeys = new AtomSubscriber[0];
        [SerializeField] int[] _InDegreeValues = new int[0];
        [SerializeField] AtomSubscriber[] _InputNodes = new AtomSubscriber[0];


#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public void Clear()
        {
            _IntermidiateTargetsFlatten = new AtomSubscriber[0];
            _IntermidiateDependenciesFlatten = new AtomSubscriber[0];
            _ViewTargetsFlatten = new AtomSubscriber[0];
            _ViewDependenciesFlatten = new AtomSubscriber[0];
            _SyncTargetsFlatten = new AtomSubscriber[0];
            _SyncDependenciesFlatten = new AtomSubscriber[0];
            _InDegreeKeys = new AtomSubscriber[0];
            _InDegreeValues = new int[0];
            _InputNodes = new AtomSubscriber[0];
        }

        public void RegisterIntermidiateEdge(AtomSubscriber target, AtomSubscriber[] dependencies)
        {
            foreach (var dependency in dependencies)
            {
                _IntermidiateDependenciesFlatten = _IntermidiateDependenciesFlatten.Concat(dependency);
                _IntermidiateTargetsFlatten = _IntermidiateTargetsFlatten.Concat(target);
                if(dependency.IsInputNode)
                {
                    _InputNodes = _InputNodes.Concat(dependency);
                }
            }
            _InDegreeKeys = _InDegreeKeys.Concat(target);
            _InDegreeValues = _InDegreeValues.Concat(dependencies.Length);
        }
        public void RegisterViewOutputEdge(AtomSubscriber target, AtomSubscriber[] dependencies)
        {
            foreach (var dependency in dependencies)
            {
                _ViewDependenciesFlatten = _ViewDependenciesFlatten.Concat(dependency);
                _ViewTargetsFlatten = _ViewTargetsFlatten.Concat(target);
                if(dependency.IsInputNode)
                {
                    _InputNodes = _InputNodes.Concat(dependency);
                }
            }
            _InDegreeKeys = _InDegreeKeys.Concat(target);
            _InDegreeValues = _InDegreeValues.Concat(dependencies.Length);
        }
        public void RegisterSyncOutputEdge(AtomSubscriber target, AtomSubscriber[] dependencies)
        {
            foreach (var dependency in dependencies)
            {
                _SyncDependenciesFlatten = _SyncDependenciesFlatten.Concat(dependency);
                _SyncTargetsFlatten = _SyncTargetsFlatten.Concat(target);
                if(dependency.IsInputNode)
                {
                    _InputNodes = _InputNodes.Concat(dependency);
                }
            }
            _InDegreeKeys = _InDegreeKeys.Concat(target);
            _InDegreeValues = _InDegreeValues.Concat(dependencies.Length);
        }
        //inputnodesの重複を削除
        public void RemoveDuplicateInputNodes()
        {
            _InputNodes = _InputNodes.Distinct().ToArray();
        }
#endif

        public void Apply(DistributorConsistency consistency)
        {
            if (consistency == DistributorConsistency.Strong)
            {
                _PrimaryEdges.Clear();
                _PrimaryEdgeValues = new AtomSubscriber[0][];
                _SecondaryEdges.Clear();
                _SecondaryEdgeValues = new AtomSubscriber[0][];
                _InDegree.Clear();

                for (int i = 0; i < _IntermidiateTargetsFlatten.Length; i++)
                {
                    ApplyToPrimaryEdge(_IntermidiateDependenciesFlatten[i], _IntermidiateTargetsFlatten[i]);
                    _PrimaryTargetNodes.Add(_IntermidiateTargetsFlatten[i]);
                    ApplyToSecondaryEdge(_IntermidiateDependenciesFlatten[i], _IntermidiateTargetsFlatten[i]);
                    _SecondaryTargetNodes.Add(_IntermidiateTargetsFlatten[i]);
                }
                for (int i = 0; i < _ViewTargetsFlatten.Length; i++)
                {
                    ApplyToSecondaryEdge(_ViewDependenciesFlatten[i], _ViewTargetsFlatten[i]);
                }
                for (int i = 0; i < _SyncTargetsFlatten.Length; i++)
                {
                    ApplyToSecondaryEdge(_SyncDependenciesFlatten[i], _SyncTargetsFlatten[i]);
                }
                for (int i = 0; i < _InDegreeKeys.Length; i++)
                {
                    if (_InDegree.ContainsKey(_InDegreeKeys[i]))
                    {
                        _InDegree[_InDegreeKeys[i]] = _InDegreeValues[i];
                    }
                    else
                    {
                        _InDegree.Add(_InDegreeKeys[i], _InDegreeValues[i]);
                    }
                }
            }
            else
            {
                _PrimaryEdges.Clear();
                _PrimaryEdgeValues = new AtomSubscriber[0][];
                _InDegree.Clear();

                for (int i = 0; i < _IntermidiateTargetsFlatten.Length; i++)
                {
                    ApplyToPrimaryEdge(_IntermidiateDependenciesFlatten[i], _IntermidiateTargetsFlatten[i]);
                }
                for (int i = 0; i < _ViewTargetsFlatten.Length; i++)
                {
                    ApplyToPrimaryEdge(_ViewDependenciesFlatten[i], _ViewTargetsFlatten[i]);
                }
                for (int i = 0; i < _SyncTargetsFlatten.Length; i++)
                {
                    ApplyToPrimaryEdge(_SyncDependenciesFlatten[i], _SyncTargetsFlatten[i]);
                }
                for (int i = 0; i < _InDegreeKeys.Length; i++)
                {
                    if (_InDegree.ContainsKey(_InDegreeKeys[i]))
                    {
                        _InDegree[_InDegreeKeys[i]] = _InDegreeValues[i];
                    }
                    else
                    {
                        _InDegree.Add(_InDegreeKeys[i], _InDegreeValues[i]);
                    }
                }
            }
        }
        
        public void ApplyToPrimaryEdge(AtomSubscriber dependency, AtomSubscriber target)
        {
            if (!_PrimaryEdges.ContainsKey(dependency))
            {
                var newSubscribersSet = new DataDictionary();
                newSubscribersSet.Add("index", _PrimaryEdgeValues.Length);
                _PrimaryEdgeValues = _PrimaryEdgeValues.Concat(new AtomSubscriber[0]);
                _PrimaryEdges.Add(dependency, newSubscribersSet);
            }
            var subscribersSet = (DataDictionary)_PrimaryEdges[dependency];
            var values = _PrimaryEdgeValues[(int)subscribersSet["index"]];
            if (subscribersSet.ContainsKey(target))
            {
                values[(int)subscribersSet[target]] = target;
            }
            else
            {
                subscribersSet.Add(target, values.Length);
                _PrimaryEdgeValues[(int)subscribersSet["index"]] = values.Concat(target);
            }
        }
        public void ApplyToSecondaryEdge(Atom dependency, AtomSubscriber target)
        {
            if (!_SecondaryEdges.ContainsKey(dependency))
            {
                var newSubscribersSet = new DataDictionary();
                newSubscribersSet.Add("index", _SecondaryEdgeValues.Length);
                _SecondaryEdgeValues = _SecondaryEdgeValues.Concat(new AtomSubscriber[0]);
                _SecondaryEdges.Add(dependency, newSubscribersSet);
            }
            var subscribersSet = (DataDictionary)_SecondaryEdges[dependency];
            var values = _SecondaryEdgeValues[(int)subscribersSet["index"]];
            if (subscribersSet.ContainsKey(target))
            {
                values[(int)subscribersSet[target]] = target;
            }
            else
            {
                subscribersSet.Add(target, values.Length);
                _SecondaryEdgeValues[(int)subscribersSet["index"]] = values.Concat(target);
            }
        }

        public AtomSubscriber[] TopologicalSortKahn(AtomHashSet inputNodesChanged, DataDictionary inDegreeOriginal, DataDictionary edges, AtomSubscriber[][] edgeValues)
        {
            var queue = new AtomSubscriber[inDegreeOriginal.Count + inputNodesChanged.Count()];
            var queueIgnore = new AtomSubscriber[inDegreeOriginal.Count + _InputNodes.Length];
            var order = new AtomSubscriber[inDegreeOriginal.Count + inputNodesChanged.Count()];
            var inDegrees = new DataDictionary();
            var queueStart = 0;
            var queueEnd = 0;
            var queueIgnoreStart = 0;
            var queueIgnoreEnd = 0;
            var orderIndex = 0;
            foreach (var key in inDegreeOriginal.GetKeys().ToArray())
            {
                inDegrees.Add(key, inDegreeOriginal[key]);
            }
            foreach (var inputNode in _InputNodes)
            {
                if(inDegrees.ContainsKey(inputNode))
                {
                    inDegrees[inputNode] = 0;
                }
                else
                {
                    inDegrees.Add(inputNode, 0);
                }
                if(inputNodesChanged.Contains(inputNode))
                {
                    queue[queueEnd++] = inputNode;
                }
                else
                {
                    queueIgnore[queueIgnoreEnd++] = inputNode;
                }
            }
            while (queueIgnoreStart < queueIgnoreEnd)
            {
                var current = queueIgnore[queueIgnoreStart++];

                if (edges.ContainsKey(current))
                {
                    var values = GetTargetNode(current, edges, edgeValues);
                    foreach (var target in values)
                    {
                        inDegrees[target] = (int)inDegrees[target] - 1;
                        if ((int)inDegrees[target] == 0)
                        {
                            queueIgnore[queueIgnoreEnd++] = target;
                        }
                    }
                }
            }

            while (queueStart < queueEnd)
            {
                var current = queue[queueStart++];
                if(inDegrees.ContainsKey(current))
                    order[orderIndex++] = current;

                if (edges.ContainsKey(current))
                {
                    var values = GetTargetNode(current, edges, edgeValues);
                    foreach (var target in values)
                    {
                        inDegrees[target] = (int)inDegrees[target] - 1;
                        if ((int)inDegrees[target] == 0)
                        {
                            queue[queueEnd++] = target;
                        }
                    }
                }
            }

            // 結果の配列を適切なサイズに調整
            var result = new AtomSubscriber[orderIndex];
            for (int i = 0; i < orderIndex; i++)
            {
                result[i] = order[i];
            }
            return result;
        }
        public AtomSubscriber[] GetPrimaryQueue(AtomHashSet inputNodesChanged)
        {
            return TopologicalSortKahn(inputNodesChanged, _InDegree, _PrimaryEdges, _PrimaryEdgeValues);
        }
        public AtomSubscriber[] GetSecondaryQueue(AtomHashSet inputNodesChanged)
        {
            return TopologicalSortKahn(inputNodesChanged, _InDegree, _SecondaryEdges, _SecondaryEdgeValues);
        }

        AtomSubscriber[] GetTargetNode(Atom dependency, DataDictionary edges, AtomSubscriber[][] edgeValues)
        {
            if (edges.ContainsKey(dependency))
            {
                var subscribersSet = (DataDictionary)edges[dependency];
                var index = (int)subscribersSet["index"];
                return edgeValues[index];
            }
            return new AtomSubscriber[0];
        }
    }
}