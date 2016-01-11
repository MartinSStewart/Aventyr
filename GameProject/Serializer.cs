﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Game
{
    public class Serializer
    {
        const string fileSuffixPhysics = "_phys";
        public const string fileExtension = "save";
        public const string fileExtensionName = "Save File";

        public Serializer()
        {
        }

        public void Serialize(SceneNode rootNode, string filename)
        {
            SceneNode clone = rootNode.DeepClone(new Scene());
            clone.SetParent(null);
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(delegate(object state)
                {
                    SerializeAsync(clone, filename);
                }), null);
        }

        private void SerializeAsync(SceneNode rootNode, string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            settings.OmitXmlDeclaration = true;
            //FarseerPhysics.Common.WorldSerializer.Serialize(scene.World, filename + fileSuffixPhysics);
            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                GetSerializer().WriteObject(writer, rootNode);
            }
        }

        public void Deserialize(Scene scene, string filename)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            using (XmlReader reader = XmlReader.Create(filename, settings))
            {
                SceneNode s = (SceneNode)GetSerializer().ReadObject(reader);
                s.SetScene(scene);
            }
        }

        private DataContractSerializer GetSerializer()
        {
            return new DataContractSerializer(typeof(SceneNode), "Game", "Game", GetKnownTypes(),
            0x7FFFF,
            false,
            true,
            null);
        }

        protected virtual IEnumerable<Type> GetKnownTypes()
        {
            return from t in Assembly.GetExecutingAssembly().GetTypes()
                   where Attribute.IsDefined(t, typeof(DataContractAttribute))
                   select t;
        }
    }
}
