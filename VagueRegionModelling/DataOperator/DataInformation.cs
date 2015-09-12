using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;

namespace VagueRegionModelling.DataOperator
{
    /// <summary>
    /// 存储输入输出相关信息
    /// </summary>
    public class DataInformation
    {
        private IMapControl3 m_mapControl = null;
        private ILayer m_inputLayer = null;
        private string m_inputLayerName = string.Empty;

        private string m_outputFileName = string.Empty;        
        private string m_outputFilePath = string.Empty;       
        

        public DataInformation()
        { }

        public DataInformation(IMapControl3 mapControl, string fileName)
        {
            m_mapControl = mapControl;
            m_outputFileName = fileName;
        }

        #region Get member value
        public string GetOutputFileName()
        {
            return m_outputFileName;
        }

        public string GetOutputFilePath()
        {
            return m_outputFilePath;
        }
        public ILayer GetInputLayer()
        {
            if (m_inputLayerName != null && m_inputLayer == null)
                m_inputLayer = GetLayerByName();
            return m_inputLayer;
        }
        #endregion

        #region Set member value
        public void SetOutputFileName(string fileName)
        {
            m_outputFileName = fileName;
        }
        public void SetOutputFilePath(string filePath)
        {
            m_outputFilePath = filePath;
        }
        public void SetInputLayerName(string layerName)
        {
            m_inputLayerName = layerName;
            m_inputLayer = GetLayerByName();
        }
        #endregion

        /// <summary>
        /// 通过图层名获取图层
        /// </summary>
        /// <returns></returns>
        public ILayer GetLayerByName()
        {
            ILayer Layer = null;
            for (int i = 0; i < m_mapControl.LayerCount; i++)
            {
                Layer = m_mapControl.Map.get_Layer(i);
                if (Layer.Name == m_inputLayerName)
                    break;
            }
            return Layer as ILayer;
        }

        /// <summary>
        /// 判断输入输出参数是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (m_inputLayer == null)
            {
                MessageBox.Show("未选择有效图层！");
                return false;
            }
            //判断fileName格式
            int index = 0;
            index = m_outputFileName.LastIndexOf(".");
            if (m_outputFilePath == "" || index <= 0)
            {
                MessageBox.Show("保存路径错误！");
                return false;
            }
            string format = m_outputFileName.Substring(index + 1, m_outputFileName.Length - index - 1);
            if (format != "shp")
            {
                MessageBox.Show("保存文件格式错误！");
                return false;
            }
            return true;
        }

        public double UnitsConvert(double dValue,esriUnits outUnits)
        {
            IUnitConverter unitConverter = new UnitConverterClass();
            double newValue = unitConverter.ConvertUnits(dValue, esriUnits.esriMeters, outUnits);
            return newValue;
        }
    }
}
