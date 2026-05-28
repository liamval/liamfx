using stm = System.IO;
using xml = System.Xml;
using xsr = System.Xml.Serialization;

namespace Liam
{
   /// <summary>
   /// Cobertura para la extensión de seralización Xml de un <see cref="Componente"/>.
   /// </summary>
   public static class ComponenteXml
   {
      /// <summary>
      /// Devuelve el documento Xml de la instancia.
      /// </summary>
      /// <param name="componente">Instancia de una clase derivada de <see cref="Componente"/>.</param>
      /// <param name="configuracion">Opcional. Configuración para la conversión de objeto a texto Xml.</param>
      /// <returns>Documento Xml de la instancia.</returns>
      public static xml.XmlDocument ComoXml(this Componente componente, xml.XmlWriterSettings configuracion = null)
      {
         var xmlSerializer = new xsr.XmlSerializer(componente.GetType());
         configuracion = configuracion ?? new xml.XmlWriterSettings()
         {
            Indent = true,
            IndentChars = "  ",
            NewLineOnAttributes = false,
            OmitXmlDeclaration = true
         };
         using (var stream = new stm.MemoryStream())
         {
            using (var xmlWriter = xml.XmlWriter.Create(stream, configuracion))
            {
               xmlSerializer.Serialize(xmlWriter, componente);
            }
            stream.Position = 0;
            var doc = new xml.XmlDocument();
            doc.Load(stream);
            return doc;
         }
      }

      /// <summary>
      /// Devuelve un texto Xml de la instancia.
      /// </summary>
      /// <param name="componente">Instancia de una clase derivada de <see cref="Componente"/>.</param>
      /// <param name="configuracion">Opcional. Configuración para la conversión de objeto a texto Xml.</param>
      /// <returns>Texto Xml de la instancia.</returns>
      public static string ComoTextoXml(this Componente componente, xml.XmlWriterSettings configuracion = null)
      {
         return ComoXml(componente, configuracion).OuterXml;
      }
   }
}
