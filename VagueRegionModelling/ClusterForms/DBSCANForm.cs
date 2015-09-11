using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using VagueRegionModelling.DataOperator;
using System.IO;

namespace VagueRegionModelling.ClusterForms
{
    public partial class DBSCANForm : Form
    {
        private IMapControl3 m_mapControl = null;
        private DataInformation m_dataInfo;
        private float m_fEps = 0;
        private int m_nMinPts = 0;

        public DBSCANForm()
        {
            InitializeComponent();
        }

        public DBSCANForm(IMapControl3 mapControl)
        {
            InitializeComponent();
            m_mapControl = mapControl;
            string fileName = "DBSCAN_Cluster.shp";
            m_dataInfo = new DataInformation(mapControl, fileName);
            inputAndOutput.SetValues(mapControl, m_dataInfo);
          //  inputAndOutput.AddLayerNames();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //有效性验证
            
            m_dataInfo = inputAndOutput.GetDataInfo();
            string filePath = m_dataInfo.GetOutputFilePath();
            string fileName = m_dataInfo.GetOutputFileName();

            #region 参数有效性验证
            if (m_dataInfo.IsValid() == false)
                return;

            if (textBoxEps.Text == "")
            {
                MessageBox.Show("区域半径输入错误！");
                return;
            }
            if (textBoxMinPts.Text == "")
            {
                MessageBox.Show("最小点数输入错误！");
                return;
            }
            if (File.Exists(filePath + "\\" + fileName))
            {
                MessageBox.Show(fileName + "文件已存在，请重新输入！");
                return;
            }
            #endregion

            //参数获取
            m_fEps = float.Parse(textBoxEps.Text);
            m_nMinPts = int.Parse(textBoxMinPts.Text);

            //计算得到聚类结果
            //保存到shp
            //渲染显示
        }

        private void DBSCAN()
        {
            //计算每个数据点相邻的数据点，邻域定义为以该点为中心以边长为2*EPs的网格
            //定义邻域内相邻的数据点的个数大于m_nMinPts的为核心点
            //邻域内包含某个核心点的非核心点，定义为边界点
            //噪音点既不是边界点也不是核心点
            //各个核心点与其邻域内的所有核心点放在同一个簇中
            //边界点跟其邻域内的某个核心点放在同一个簇中
        }


    }
}
