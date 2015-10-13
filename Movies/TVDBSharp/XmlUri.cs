// Author: Adrian Hum
// Project: TVDBSharp/XmlUri.cs
// 
// Created : 2015-03-06  17:15 
// Modified: 2015-04-10 16:05)

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TVDBSharp {
    public class XmlUri : IXmlSerializable {
        private Uri _value;
        public XmlUri() {}

        public XmlUri(Uri source)
        {
            _value = source;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            _value = new Uri(reader.ReadElementContentAsString());
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteValue(_value.ToString());
        }

        public static implicit operator Uri(XmlUri o)
        {
            return o == null ? null : o._value;
        }

        public static implicit operator XmlUri(Uri o)
        {
            return o == null ? null : new XmlUri(o);
        }

        public override string ToString()
        {
            return _value.AbsoluteUri;
        }
    }
}