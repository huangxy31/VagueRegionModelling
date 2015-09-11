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
        private IPoint m_point;
        private int m_nPointType = 0; //核心点1，边界点2，噪声0
        private bool m_bVisited = false;

        public DBSCANPoint(IPoint point)
        {
            m_point = point;
        }

        public IPoint GetPoint()
        {
            return m_point;
        }

        public void IsVisited(bool bFlag)
        {
            m_bVisited = bFlag;
        }
    }
}
