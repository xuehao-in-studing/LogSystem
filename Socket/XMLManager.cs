using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Socket
{
    /// <summary>
    /// 单例模式创建XMLManager
    /// 功能：查看指定节点内容、修改指定节点内容
    /// path:绝对路径
    /// </summary>
    public class XMLManager
    {
        //private static XMLManager insatnce;
        private readonly XmlDocument xmlDocument;

        private static string xmlPath;
        public string XMLPath
        {
            get
            {
                return xmlPath;
            }
            set
            {
                xmlPath = value;
            }
        }
        public XMLManager(string path)
        {
            XMLPath = path;
            xmlDocument = new XmlDocument();
            xmlDocument.Load(XMLPath);
        }

        public string GetAllText()
        {
            return xmlDocument.DocumentElement.InnerText;
        }

        public string GetAllTagText()
        {
            string ret=String.Empty;
            foreach(XmlNode node in xmlDocument.ChildNodes)
            {
                if(node.NodeType == XmlNodeType.Element)
                { 
                    ret += node.Name + node.InnerText + '\n'; 
                }
            }
            return ret;
        }

        public string GetNodeTextByName(string NodeName)
        {
            XmlNode node = xmlDocument.SelectSingleNode("//" + NodeName);
            if (node != null)
            {
                return node.InnerText;
            }
            return null;
        }

        public void UpdateNodeValue(string nodeName, string newValue)
        {
            XmlNode node = xmlDocument.SelectSingleNode("//" + nodeName);
            if (node != null)
            {
                node.InnerText = newValue;
            }
        }

    }
}
