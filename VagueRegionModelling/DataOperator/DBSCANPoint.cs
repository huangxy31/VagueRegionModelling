using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Geometry;

namespace VagueRegionModelling.DataOperator
{
    public class DBSCANPoint
    {
        private IPoint m_point = null;
        private int m_nPointType = 0; //核心点1，边界点2，噪声0
        private bool m_bVisited = false;
        private List<DBSCANPoint> m_neighborPoints = null;
        private int m_nOID; //便于查找

        public DBSCANPoint()
        {
        }

        public DBSCANPoint(IPoint point, int OID)
        {
            m_point = point;
            m_nOID = OID;
        }

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
        public bool IsVisited()
        {
            return m_bVisited;
        }

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
    }
}
