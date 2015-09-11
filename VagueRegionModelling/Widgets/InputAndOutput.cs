using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using VagueRegionModelling.DataOperator;

namespace VagueRegionModelling.Widgets
{
    /// <summary>
    /// 
    /// </summary>
    public partial class InputAndOutput : UserControl
    {
        private IMapControl3 m_mapControl;
        private DataInformation m_dataInfo;


        public InputAndOutput()
        {
            InitializeComponent();
            m_dataInfo = new DataInformation();
        }


        public void SetValues(IMapControl3 mapControl, DataInformation dataInfo)
        {
            m_mapControl = mapControl;
            m_dataInfo = dataInfo;
            AddLayerNames();
        }


        public DataInformation GetDataInfo()
        {
            //通过textbox设置datainfo
            string file = textBoxOutput.Text;
            string filePath, fileName;
            int index = 0;
            index = file.LastIndexOf("\\");
            //路径不正确
            if (index <= 0)
            {
                m_dataInfo.SetOutputFilePath(string.Empty);
                //m_dataInfo.SetOutputFileName(string.Empty);
                return m_dataInfo;
            }
            filePath = file.Substring(0, index);
            fileName = file.Substring(index + 1, file.Length - index - 1);
            m_dataInfo.SetOutputFilePath(filePath);
            m_dataInfo.SetOutputFileName(fileName);
            return m_dataInfo;
        }


        /// <summary>
        /// 根据图层类型给comboBox中添加图层名
        /// </summary>
        /// <param name="comboBoxInput"></param>
        private void AddLayerNames()
        {
            //清空
            if (comboBoxInput.Items.Count != 0)
                comboBoxInput.Items.Clear();

            int i;
            ILayer layer = null;
            string layerName = string.Empty;

            for (i = 0; i < m_mapControl.LayerCount; i++)
            {
                //可以考虑判断是否过滤掉非点图层
                layer = m_mapControl.Map.get_Layer(i);
                IFeatureLayer fLayer = layer as IFeatureLayer;
                esriGeometryType gType = fLayer.FeatureClass.ShapeType;
                if (gType != esriGeometryType.esriGeometryPoint && gType != esriGeometryType.esriGeometryMultipoint)
                    continue;
                comboBoxInput.Items.Add(layer.Name);
            }
        }

        private void comboBoxInput_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string layerName = comboBoxInput.SelectedItem.ToString();
            m_dataInfo.SetInputLayerName(layerName);
            IDataset dataset = (IDataset)m_dataInfo.GetInputLayer();
            string filePath = dataset.Workspace.PathName;
            m_dataInfo.SetOutputFilePath(filePath);
 //           string fileName = "convexhull.shp";
            textBoxOutput.Text = m_dataInfo.GetOutputFilePath() + "\\" + m_dataInfo.GetOutputFileName();
        }

        private void buttonSavePath_Click(object sender, EventArgs e)
        {
            SaveFileDialog();
            if (m_dataInfo.GetOutputFilePath() != string.Empty)
                textBoxOutput.Text = m_dataInfo.GetOutputFilePath() + "\\" + m_dataInfo.GetOutputFileName();
        }

        /// <summary>
        /// 选择shp存放目录，并返回工作空间及文件名
        /// </summary>
        private void SaveFileDialog()
        {
            SaveFileDialog saveFileDialog;
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Output Feature Class";
            saveFileDialog.Filter = "shapefile(*.shp)|*.shp";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = saveFileDialog.FileName;
                string filePath, fileName;
                int index = 0;
                index = file.LastIndexOf("\\");
                filePath = file.Substring(0, index);
                fileName = file.Substring(index + 1, file.Length - index - 1);

                m_dataInfo.SetOutputFilePath(filePath);
                m_dataInfo.SetOutputFileName(fileName);
            }
        }
    }
}
