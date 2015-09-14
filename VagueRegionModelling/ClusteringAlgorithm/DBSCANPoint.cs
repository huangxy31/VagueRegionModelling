using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Geometry;

namespace VagueRegionModelling.ClusteringAlgorithm
{
    /// <summary>
    /// 进行DBSCAN算法计算的基本数据结构
    /// </summary>
    public class DBSCANPoint
    {
        private IPoint m_point = null;
        private int m_nPointType = 0; //核心点1，边界点2，噪声0
        private bool m_bVisited = false;    //是否已被访问
        private List<DBSCANPoint> m_neighborPoints = null;  //存储相邻点
        private int m_nOID; //便于查找

        public DBSCANPoint()
        {
        }

        public DBSCANPoint(IPoint point, int OID)
        {
            m_point = point;
            m_nOID = OID;
        }

        #region Get方法
        public IPoint GetPoint()
        {
            return m_point;
        }
        public int GetPointType()
        {
            return m_nPointType;
        }
        public int GetOID()
        { 
            return m_nOID;
        }

        public List<DBSCANPoint> GetNeighborPoints()
        {
            return m_neighborPoints;
        }
        
        /// <summary>
        /// 判断该点是否已被访问
        /// </summary>
        /// <returns></returns>
        public bool IsVisited()
        {
            return m_bVisited;
        }
        #endregion

        #region Set方法
        public void SetPointType(int type)
        {
            m_nPointType = type;
        }

        public void SetNeighborPoints(List<DBSCANPoint> neighborPnts)
        {
            m_neighborPoints = neighborPnts;
        }

        public void SetAsVisited()
        {
            m_bVisited = true;
        }
        #endregion
    }
}
