﻿using System;
using System.Linq;
using System.Xml;
using System.IO;

namespace Landis.Library.Metadata
{
    public class MetadataProvider
    {
        private XmlDocument doc = new XmlDocument();
        XmlNode metadataNode;
        //private ExtensionMetadata extensionMetadata;// = new ExtensionMetadata();
        private IMetadata metadata; 

        public MetadataProvider()
        {
        }

        public MetadataProvider(IMetadata metadata)
        {
            metadataNode = doc.CreateElement("landisMetadata");
            doc.AppendChild(metadataNode);
            this.metadata = metadata;
        }


        public XmlNode GetMetadataXmlNode()
        {
            metadataNode.AppendChild(this.metadata.Get_XmlNode(doc));
            return metadataNode;
        }
        public String GetMetadataString()
        {
            return this.metadata.Get_XmlNode(doc).OuterXml;
        }
        public void WriteMetadataToXMLFile(string metadataFolderPath, string folderName, string fileName)
        {

            if (!System.IO.Directory.Exists(metadataFolderPath))
                System.IO.Directory.CreateDirectory(metadataFolderPath);

            if (!System.IO.Directory.Exists(Path.Combine(metadataFolderPath, folderName)))
                //System.IO.Directory.CreateDirectory(metadataFolderPath + "\\" + folderName);
                System.IO.Directory.CreateDirectory(Path.Combine(metadataFolderPath, folderName));
            System.IO.StreamWriter file;

            try
            {
                if (metadata is ExtensionMetadata)
                {
                    foreach (OutputMetadata om in ((ExtensionMetadata)metadata).OutputMetadatas.Where(p => p.Type == OutputType.Table))
                    {
                        XmlDocument outDoc = new XmlDocument();
                        XmlNode outputMetadataNode = outDoc.CreateElement("landisMetadata");
                        XmlNode outputNode = outDoc.CreateElement("output");
                        outputMetadataNode.AppendChild(outputNode);

                        XmlNode extensionNode = outDoc.CreateElement("extension");

                        XmlAttribute outputExtNameAt = outDoc.CreateAttribute("name");
                        outputExtNameAt.Value = ((ExtensionMetadata)metadata).Name;
                        extensionNode.Attributes.Append(outputExtNameAt);

                        XmlAttribute pathAt = outDoc.CreateAttribute("metadataFilePath");
                        pathAt.Value = fileName + ".xml";
                        extensionNode.Attributes.Append(pathAt);
                        outputNode.AppendChild(extensionNode);

                        XmlNode fieldsNode = om.Get_Fields_XmlNode(outDoc);
                        outputNode.AppendChild(fieldsNode);


                        //file = new System.IO.StreamWriter(metadataFolderPath + "\\" + folderName + "\\" + om.Name + "_Metadata.xml", false);
                        file = new System.IO.StreamWriter(Path.Combine(metadataFolderPath, folderName, om.Name + "_Metadata.xml"), false);
                        //string strMetadata = GetMetadataString();
                        file.WriteLine(outputMetadataNode.OuterXml);
                        file.Close();
                        file.Dispose();

                        om.MetadataFilePath = Path.Combine(metadataFolderPath, folderName, om.Name + "_Metadata.xml");
                    }
                }
            }
            catch(InvalidCastException ex)
            {
                //string message = 
                throw new ApplicationException(String.Format("Error generating metadata: {0}.", ex.ToString()));
            }


            
            file = new System.IO.StreamWriter(Path.Combine(metadataFolderPath, folderName, fileName + ".xml"), false);
            //string strMetadata = GetMetadataString();
            XmlNode metadataNode = doc.CreateElement("landisMetadata");

            if (metadata is ExtensionMetadata)
            {
                metadataNode.AppendChild(((ExtensionMetadata)metadata).Get_XmlNode(doc));
            }
            else if (metadata is CoreMetadata)
            {
                metadataNode.AppendChild(((CoreMetadata)metadata).Get_XmlNode(doc));
            }

            file.WriteLine(metadataNode.OuterXml);
            file.Close();
            file.Dispose();


        }
    }
}
