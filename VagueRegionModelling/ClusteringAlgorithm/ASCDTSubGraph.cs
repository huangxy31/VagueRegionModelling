using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Geodatabase;

namespace VagueRegionModelling.ClusteringAlgorithm
{
    public class ASCDTSubGraph
    {
        private List<ITinNode> m_clusterNode;
        private List<ITinEdge> m_clusterEdge;

        public ASCDTSubGraph(List<ITinNode> clusterNode, List<ITinEdge> clusterEdge)
        {
            m_clusterNode = clusterNode;
            m_clusterEdge = clusterEdge;
        }

        public List<ITinNode> GetNodeList()
        {
            return m_clusterNode;
        }

        public List<ITinEdge> GetEdgeList()
        {
            return m_clusterEdge;
        }

        public void AddNode(ITinNode node)
        {
            m_clusterNode.Add(node);
        }

        public void AddEdge(ITinEdge edge)
        {
            m_clusterEdge.Add(edge);
        }
    }
}
