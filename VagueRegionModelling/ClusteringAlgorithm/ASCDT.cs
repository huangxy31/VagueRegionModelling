using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System.IO;
using VagueRegionModelling.DataOperator;
using System.Windows.Forms;

namespace VagueRegionModelling.ClusteringAlgorithm
{
    public class ASCDT
    {
        private List<ASCDTSubGraph> m_globalSubGraphs = null;  //全局聚类结果
        private List<ASCDTSubGraph> m_localSubGraphs = null;  //局部聚类结果
        private Clusters m_Clusters = new Clusters();   //聚类结果
        private DataInformation m_dataInfo; //相关信息
        private bool m_bLocal;  //是否进行局部聚类
        private ITin m_DT = null;  //构建的三角网
        private List<int> m_nodeFlag = new List<int>();  //控制划分子图，-1为噪声点，0为待划分的点，1为已划分的点

        public ASCDT(DataInformation dataInfo, bool bLocal)
        {
            m_dataInfo = dataInfo;
            m_bLocal = bLocal;
        }

        /// <summary>
        /// 进行聚类操作，并获得结果
        /// </summary>
        /// <returns></returns>
        public Clusters GetClusters()
        {
            //构建三角网
            CreateDelaunay();
            //全局聚类
            GlobalClustering();
            //局部聚类
            if (m_bLocal == true)
            {
                LocalClustering();
            }
            //保存结果到m_Clusters
            CreateClusters();

            return m_Clusters;
        }

        /// <summary>
        /// 创建TIN
        /// </summary>
        private void CreateDelaunay()
        {
            //创建TIN
            IFeatureLayer featureLayer = m_dataInfo.GetInputLayer() as IFeatureLayer;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            IField pField = featureClass.Fields.get_Field(0);

            if (pField == null)
            {
                MessageBox.Show("创建Delaunay三角网失败");
                return;
            }

            IGeoDataset pGeoDataset = featureClass as IGeoDataset;
            IEnvelope pEnvelope = pGeoDataset.Extent;
            pEnvelope.SpatialReference = pGeoDataset.SpatialReference;

            ITinEdit pTinEdit = new TinClass();
            pTinEdit.InitNew(pEnvelope);
            object obj = Type.Missing;
            pTinEdit.AddFromFeatureClass(featureClass, null, pField, null, esriTinSurfaceType.esriTinMassPoint, ref obj);
            m_DT = pTinEdit as ITin;

            //将所有点标识为噪声点
            ITinAdvanced tinAdvanced = m_DT as ITinAdvanced;
            for (int i = 0; i < tinAdvanced.NodeCount; i++)
            {
                m_nodeFlag.Add(-1);
            }
        }

        #region 全局聚类，得到边
        /// <summary>
        /// 全局聚类
        /// </summary>
        private void GlobalClustering()
        {
            //全局聚类，得到符合条件的边与点
            List<ITinEdge> globalEdgeList = GetGlobalEdgeList();
            List<ITinNode> globalNodeList = GetNodeList(globalEdgeList);
            //得到全局聚类的子图
            m_globalSubGraphs = ConstractSubGraph(globalEdgeList, globalNodeList);
        }

        /// <summary>
        /// 获得全局聚类的EdgeList
        /// </summary>
        /// <returns></returns>
        private List<ITinEdge> GetGlobalEdgeList()
        {
            List<ITinEdge> globalEdge = new List<ITinEdge>();
            ITinAdvanced tinAdvanced = m_DT as ITinAdvanced;

            double globalMean = GlobalMean(m_DT);
            double globalStDev = GlobalStDev(m_DT, globalMean);
            int nodeCount = tinAdvanced.NodeCount;

            for (int i = 1; i <= nodeCount; i++)
            {
                ITinNode tinNode = tinAdvanced.GetNode(i);
                double local1Mean = Local1Mean(tinNode);
                double globalConstraint = globalMean * (1 + globalStDev / local1Mean);

                ITinEdgeArray incdentEdges = tinNode.GetIncidentEdges();
                int incdentEdgeCount = incdentEdges.Count;

                for (int j = 0; j < incdentEdgeCount; j++)
                {
                    ITinEdge currentEdge = incdentEdges.get_Element(j);
                    if (currentEdge.Length < globalConstraint && currentEdge.IsInsideDataArea)
                        globalEdge.Add(currentEdge);   
                }
            }

            //去掉多加的neighbor边
            globalEdge = CompleteEdges(globalEdge);            
            return globalEdge;
        }

        /// <summary>
        /// 去掉多加的neighbor边
        /// </summary>
        /// <param name="globalEdge"></param>
        /// <returns></returns>
        private List<ITinEdge> CompleteEdges(List<ITinEdge> globalEdge)
        {
            for (int i = 0; i < globalEdge.Count; i++)
            {
                //if (globalEdge.Contains(globalEdge[i].GetNeighbor()))
                if (IsInEdges(globalEdge[i].GetNeighbor(), globalEdge))
                    continue;
                else
                    globalEdge.RemoveAt(i);
            }
            return globalEdge;
        }
        #endregion

        #region 局部聚类
        /// <summary>
        /// 局部聚类
        /// </summary>
        private void LocalClustering()
        {
            m_localSubGraphs = new List<ASCDTSubGraph>();
            //划分前重置m_nodeFlag
            for (int i = 0; i < m_nodeFlag.Count; i++)
            {
                m_nodeFlag[i] = -1;
            }

            //每个子图进行计算、划分
            for (int i = 0; i < m_globalSubGraphs.Count; i++)
            {                
                List<ITinEdge> localEdge = GetLocalEdgeList(m_globalSubGraphs[i]);
                List<ITinNode> localNode = GetNodeList(localEdge);
                List<ASCDTSubGraph> subSubGraph = ConstractSubGraph(localEdge, localNode);
                for (int j = 0; j < subSubGraph.Count; j++)
                {
                    m_localSubGraphs.Add(subSubGraph[j]);
                }
            }
        }

        /// <summary>
        /// 得到局部聚类的EdgeList
        /// </summary>
        /// <param name="subGraph"></param>
        /// <returns></returns>
        private List<ITinEdge> GetLocalEdgeList(ASCDTSubGraph subGraph)
        {
            List<ITinEdge> localEdge = new List<ITinEdge>();

            double mean1StDev = Mean1StDev(subGraph);
            for (int i = 0; i < subGraph.GetNodeList().Count; i++)
            {
                ITinNode localNode = subGraph.GetNodeList()[i];
                double local2Mean = Local2Mean(localNode, subGraph);
                double localConstraint = local2Mean + mean1StDev;

                ITinEdgeArray incidentEdges = localNode.GetIncidentEdges();
                for (int j = 0; j < incidentEdges.Count; j++)
                {
                    ITinEdge currenEdge = incidentEdges.get_Element(j);
                    if (IsInEdges(currenEdge, subGraph.GetEdgeList()) && currenEdge.Length < localConstraint)
                        localEdge.Add(currenEdge);
                }
            }
            //去掉多加的neighbor边
            localEdge = CompleteEdges(localEdge);
            return localEdge;
        }
        #endregion

        #region 参数计算
        /// <summary>
        /// Delaunay三角网整体边长平均值(全局)
        /// </summary>
        /// <param name="tin"></param>
        /// <returns></returns>
        private double GlobalMean(ITin tin)
        {
            double length = 0;
            ITinAdvanced tinAdvanced = tin as ITinAdvanced;
            int edgeCount = tinAdvanced.EdgeCount;
            for (int i = 1; i <= edgeCount; i++)
            {
                if (tinAdvanced.GetEdge(i).IsInsideDataArea)
                    length = length + tinAdvanced.GetEdge(i).Length;
            }
            return length / (double)tinAdvanced.DataEdgeCount;
        }

        /// <summary>
        /// 整体边长标准差(全局)
        /// </summary>
        /// <param name="tin"></param>
        /// <param name="globalMean"></param>
        /// <returns></returns>
        private double GlobalStDev(ITin tin, double globalMean)
        {
            double a = 0;
            ITinAdvanced tinAdvanced = tin as ITinAdvanced;
            int edgeCount = tinAdvanced.EdgeCount;
            //            int edgeCount = tinAdvanced.DataEdgeCount;
            for (int i = 1; i <= edgeCount; i++)
            {
                if (tinAdvanced.GetEdge(i).IsInsideDataArea)
                {
                    ITinEdge tinEdge = tinAdvanced.GetEdge(i);
                    a = a + (globalMean - tinEdge.Length) * (globalMean - tinEdge.Length);
                }
            }
            return System.Math.Sqrt(a / (double)tinAdvanced.DataEdgeCount);
        }


        /// <summary>
        /// 一阶邻接边长均值
        /// </summary>
        /// <param name="tinNode"></param>
        /// <returns></returns>
        private double Local1Mean(ITinNode tinNode)
        {
            ITinEdgeArray incdentEdges = tinNode.GetIncidentEdges();

            double a = 0;
            int incdentEdgeCount = incdentEdges.Count;
            int dataIncdentEdgeCount = incdentEdgeCount;
            for (int i = 0; i < incdentEdgeCount; i++)
            {
                if (incdentEdges.get_Element(i).IsInsideDataArea)
                    a = a + incdentEdges.get_Element(i).Length;
                else
                    dataIncdentEdgeCount--;
            }
            if (dataIncdentEdgeCount <= 0)
                return 0;
            return a / (double)dataIncdentEdgeCount;
        }

        /// <summary>
        /// 一阶邻接边长均值（子图）
        /// </summary>
        /// <param name="tinNode"></param>
        /// <param name="globalEdge"></param>
        /// <returns></returns>
        private double Local1Mean(ITinNode tinNode, List<ITinEdge> globalEdge)
        {
            ITinEdgeArray incdentEdges = tinNode.GetIncidentEdges();

            double a = 0;
            int incdentEdgeCount = incdentEdges.Count;
            int dataIncdentEdgeCount = incdentEdgeCount;
            for (int i = 0; i < incdentEdgeCount; i++)
            {
                if (IsInEdges(incdentEdges.get_Element(i), globalEdge))
                    a = a + incdentEdges.get_Element(i).Length;
                else
                    dataIncdentEdgeCount--;
            }
            if (dataIncdentEdgeCount <= 0)
                return 0;
            return a / (double)dataIncdentEdgeCount;
        }

        /// <summary>
        /// 一阶邻接边长标准差（子图）
        /// </summary>
        /// <param name="tinNode"></param>
        /// <param name="globalEdge"></param>
        /// <param name="Local1Mean"></param>
        /// <returns></returns>
        private double Local1StDev(ITinNode tinNode, List<ITinEdge> globalEdge, double Local1Mean)
        {
            ITinEdgeArray incdentEdges = tinNode.GetIncidentEdges();
            double a = 0;
            int incdentEdgeCount = incdentEdges.Count;
            int dataIncdentEdgeCount = incdentEdgeCount;
            for (int i = 0; i < incdentEdgeCount; i++)
            {
                if (IsInEdges(incdentEdges.get_Element(i), globalEdge))
                    a = a + (Local1Mean - incdentEdges.get_Element(i).Length) * (Local1Mean - incdentEdges.get_Element(i).Length);
                else
                    dataIncdentEdgeCount--;
            }
            if (dataIncdentEdgeCount <= 0)
                return 0;
            return System.Math.Sqrt(a / (double)dataIncdentEdgeCount);
        }

        /// <summary>
        /// 一阶邻接边长标准差平均值（子图）
        /// </summary>
        /// <param name="subGraph"></param>
        /// <returns></returns>
        private double Mean1StDev(ASCDTSubGraph subGraph)
        {
            List<ITinNode> globalNode = subGraph.GetNodeList();
            List<ITinEdge> globalEdge = subGraph.GetEdgeList();
            int nodeCount = globalNode.Count;
            double a = 0;
            for (int i = 0; i < nodeCount; i++)
            {
                ITinNode tinNode = globalNode[i];
                double local1Mean = Local1Mean(tinNode, globalEdge);
                double local1StDev = Local1StDev(tinNode, globalEdge, local1Mean);
                a = a + local1StDev;
            }
            return a / (double)nodeCount;
        }

        /// <summary>
        /// 二阶邻接边长均值
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="subGraph"></param>
        /// <returns></returns>
        private double Local2Mean(ITinNode currentNode, ASCDTSubGraph subGraph)
        {
            double a = 0;
            List<ITinEdge> incident2Edges = Get2IncidentEdges(currentNode, subGraph);
            int edgeCount = incident2Edges.Count;
            for (int i = 0; i < edgeCount; i++)
            {
                a = a + incident2Edges[i].Length;
            }
            return a / (double)edgeCount;
        }

        /// <summary>
        /// 得到二阶邻接边
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="subGraph"></param>
        /// <returns></returns>
        private List<ITinEdge> Get2IncidentEdges(ITinNode currentNode, ASCDTSubGraph subGraph)
        {
            List<ITinEdge> incidentEdges = new List<ITinEdge>();
            List<ITinEdge> subGraphEdge = subGraph.GetEdgeList();
            List<ITinNode> subGraphNode = subGraph.GetNodeList();

            //加入一阶邻接边
            ITinEdgeArray incEdges = currentNode.GetIncidentEdges();
            for (int j = 0; j < incEdges.Count; j++)
            {
                if (IsInEdges(incEdges.get_Element(j), subGraphEdge))
                    incidentEdges.Add(incEdges.get_Element(j));
            }
            //查找相邻点，加入二阶邻接边
            ITinNodeArray adjNodes = currentNode.GetAdjacentNodes();
            for (int j = 0; j < adjNodes.Count; j++)
            {
                if (!IsInNodes(adjNodes.get_Element(j), subGraphNode))
                    continue;
                ITinEdgeArray inc2Edges = adjNodes.get_Element(j).GetIncidentEdges();
                for (int k = 0; k < inc2Edges.Count; k++)
                {
                    ITinEdge edge = inc2Edges.get_Element(k);
                    if (IsInEdges(edge, incidentEdges))
                        continue;
                    if (IsInEdges(edge, subGraphEdge))
                        incidentEdges.Add(edge);
                }
            }

            return incidentEdges;
        }
        #endregion

        #region 判断
        /// <summary>
        /// 判断边edge是否在edgeList中
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="edgeList"></param>
        /// <returns></returns>
        private bool IsInEdges(ITinEdge edge, List<ITinEdge> edgeList)
        {
            bool b = false;
            if (edgeList.Count == 0)
                return b;
            for (int i = 0; i < edgeList.Count; i++)
            {
                if (edge.Index == edgeList[i].Index)
                {
                    b = true;
                    break;
                }
            }
            return b;
        }

        /// <summary>
        /// 判断点node是否在nodeList中
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        private bool IsInNodes(ITinNode node, List<ITinNode> nodeList)
        {
            bool b = false;
            if (nodeList.Count == 0)
                return b;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (node.Index == nodeList[i].Index)
                {
                    b = true;
                    break;
                }
            }
            return b;
        }

        #endregion

        #region 聚类后操作
        /// <summary>
        /// 由聚类后的边得到聚类点
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
        private List<ITinNode> GetNodeList(List<ITinEdge> edges)
        {
            List<ITinNode> nodes = new List<ITinNode>();
            for (int i = 0; i < edges.Count; i++)
            {
                ITinEdge currentEdge = edges[i];
                ITinNode fromNode = currentEdge.FromNode;
                ITinNode toNode = currentEdge.ToNode;
                if (!IsInNodes(fromNode, nodes))
                {
                    nodes.Add(fromNode);
                    m_nodeFlag[fromNode.Index - 1] = 0; //标记为待划分的点
                }
                if (!IsInNodes(toNode, nodes))
                {
                    nodes.Add(toNode);
                    m_nodeFlag[toNode.Index - 1] = 0;
                }
            }
            return nodes;
        }

        /// <summary>
        /// 划分子图
        /// </summary>
        /// <param name="globalEdge"></param>
        /// <param name="globalNode"></param>
        /// <returns></returns>
        private List<ASCDTSubGraph> ConstractSubGraph(List<ITinEdge> globalEdge, List<ITinNode> globalNode)
        {
            List<ASCDTSubGraph> subGraphList = new List<ASCDTSubGraph>();
            for (int i = 0; i < globalNode.Count; i++)
            {
                ITinNode startNode = globalNode[i];
                if (m_nodeFlag[startNode.Index - 1] != 0)
                    continue;

                
                List<ITinEdge> clusterEdges = new List<ITinEdge>();
                List<ITinNode> clusterNodes = new List<ITinNode>();
                ASCDTSubGraph newSubGraph = new ASCDTSubGraph(clusterNodes, clusterEdges);

                newSubGraph = ExpandSubGraph(startNode, globalEdge, newSubGraph);
                subGraphList.Add(newSubGraph);
            }
            return subGraphList;
        }

        private ASCDTSubGraph ExpandSubGraph(ITinNode currentNode, List<ITinEdge> globalEdge, ASCDTSubGraph newSubGraph)
        {
            m_nodeFlag[currentNode.Index - 1] = 1;
            newSubGraph.AddNode(currentNode);

            ITinEdgeArray incidentEdges = currentNode.GetIncidentEdges();
            for (int i = 0; i < incidentEdges.Count; i++)
            {
                ITinEdge currentEdge = incidentEdges.get_Element(i);
                if (IsInEdges(currentEdge, globalEdge))
                {
                    newSubGraph.AddEdge(currentEdge);
                    ITinNode nextNode = currentEdge.ToNode;
                    if (m_nodeFlag[nextNode.Index - 1] == 0)
                        ExpandSubGraph(nextNode, globalEdge, newSubGraph);
                }
            }
            return newSubGraph;
        }

        #endregion

        /// <summary>
        /// 将ASCDTSubGraph的结果保存到clusters中
        /// </summary>
        private void CreateClusters()
        {
            if (m_bLocal == false)
            {
                for (int i = 0; i < m_globalSubGraphs.Count; i++)
                {
                    Cluster cluster = new Cluster();
                    cluster.SetClusterIndex(i + 1);
                    ASCDTSubGraph subGraph = m_globalSubGraphs[i];
                    List<ITinNode> tinNodes = subGraph.GetNodeList();
                    for (int j = 0; j < tinNodes.Count; j++)
                    {
                        IPoint newPoint = new PointClass();
                        tinNodes[j].QueryAsPoint(newPoint);
                        IZAware pZAware = newPoint as IZAware;
                        pZAware.ZAware = false;
                        cluster.AddPoint(newPoint);
                    }
                    cluster.CreateConvexHull(m_dataInfo.GetSpatialReference());
                    m_Clusters.AddCluster(cluster);
                }
            }
            else
            {
                for (int i = 0; i < m_localSubGraphs.Count; i++)
                {
                    Cluster cluster = new Cluster();
                    cluster.SetClusterIndex(i + 1);
                    ASCDTSubGraph subGraph = m_localSubGraphs[i];
                    List<ITinNode> tinNodes = subGraph.GetNodeList();
                    for (int j = 0; j < tinNodes.Count; j++)
                    {
                        IPoint newPoint = new PointClass();
                        tinNodes[j].QueryAsPoint(newPoint);
                        IZAware pZAware = newPoint as IZAware;
                        pZAware.ZAware = false;
                        cluster.AddPoint(newPoint);
                    }
                    cluster.CreateConvexHull(m_dataInfo.GetSpatialReference());
                    m_Clusters.AddCluster(cluster);
                }
            }
        }

    }
}
