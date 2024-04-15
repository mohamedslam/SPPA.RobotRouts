using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace SvgMath
{
    /* Minimal effort port of http://sourceforge.net/projects/svgmath/?source=navbar
	 * See also License.txt
	 */

    public class Mml
    {
        public Mml(string contentXML)
        {
            m_mathDocument = XDocument.Parse(contentXML);
        }

        public XElement MakeSvg(float fontSize, string colorHex)
        {
            // need to enforce point as decimal seperator, otherwise it will fail.
            CultureInfo customCulture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = customCulture;

            m_mathConfig = new MathConfig();
            m_mathConfig.Defaults["mathsize"] = $"{fontSize}pt";
            m_mathConfig.Defaults["mathcolor"] = colorHex;
            MathNode currentNode = new MathNode(
                m_mathDocument.Root.Name.LocalName,
                m_mathDocument.Root.Attributes().ToDictionary(kvp => kvp.Name.ToString(), kvp => kvp.Value),
                m_mathConfig,
                null);
            ParseMML(m_mathDocument.Root, currentNode, 0);
            return currentNode.MakeImage();
        }

        private void ParseMML(XElement root, MathNode parentNode, int depth)
        {
            int recDepth = depth + 1;
            foreach (XElement element in root.Elements())
            {
                MathNode mn = new MathNode(
                    element.Name.LocalName,
                    element.Attributes().ToDictionary(kvp => kvp.Name.ToString(), kvp => kvp.Value),
                    m_mathConfig, parentNode);

                element.Nodes()
                    .Where(x => x.NodeType == System.Xml.XmlNodeType.Text || x.NodeType == System.Xml.XmlNodeType.Whitespace)
                    .ToList()
                    .ForEach(x => mn.Text = mn.Text + string.Join(" ", ((XText)x).Value.Split(null)));

                ParseMML(element, mn, recDepth);
            }
        }

        private readonly XDocument m_mathDocument;
        private MathConfig m_mathConfig;
    }
}