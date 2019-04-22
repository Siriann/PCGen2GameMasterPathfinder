using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace XMLCharacterConversionTool
{
    class Character
    {
        public Character(string path, bool isPc)
        {
            string directoryName = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);
            if (filename.StartsWith("."))
            {
                filename = filename.Substring(1);
            }

            XmlDocument characterFile = new XmlDocument();
            characterFile.Load(path);
            XDocument newTree = Convert(characterFile, isPc);

            string outDirectory = ".\\ConvertedCharacters";
            if (!Directory.Exists(outDirectory))
            {
                Directory.CreateDirectory(outDirectory);
            }

            string newFilePath = outDirectory + "\\" + filename + ".xml";
            Console.WriteLine(newTree.ToString());
            newTree.Save(newFilePath);
        }

        public XDocument Convert(XmlDocument element, bool isPc)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(element.NameTable);
            nsmgr.AddNamespace("bk", "http://www.rpgprofiler.net/2003/3EProfiler");

            string name = GetAttrValue(element.DocumentElement, nsmgr, "Name");
            string raceClass = GetAttrValue(element.DocumentElement, nsmgr, "Race") + " " + RemoveNumbers(GetAttrValue(element.DocumentElement, nsmgr, "Class"));

            XElement mainElement = new XElement("character",
                new XElement("name", name),
                new XElement("raceClass", raceClass),
                new XElement("alignment", RemoveLowerCaseCharacters(GetAttrValue(element.DocumentElement, nsmgr, "Alignment"))),
                new XElement("size", RemoveLowerCaseCharacters(GetAttrValue(element.DocumentElement, nsmgr, "Size"))),
                new XElement("ac", GetAttrValue(element.DocumentElement, nsmgr, "AC")),
                new XElement("touch", GetAttrValue(element.DocumentElement, nsmgr, "TouchAC")),
                new XElement("flat", GetAttrValue(element.DocumentElement, nsmgr, "FFAC")),
                new XElement("armor", GetAttrValue(element.DocumentElement, nsmgr, "Armor1Bonus") + " "
                                    + GetAttrValue(element.DocumentElement, nsmgr, "Armor1Name")),//<armor>+4 armor, +2 deflection</armor>
                new XElement("hp", GetHPRow(element.DocumentElement, nsmgr)),
                new XElement("init", GetAttrValue(element.DocumentElement, nsmgr, "Init")),

                new XElement("fort", GetAttrValue(element.DocumentElement, nsmgr, "Fort")),
                new XElement("ref", GetAttrValue(element.DocumentElement, nsmgr, "Reflex")),
                new XElement("will", GetAttrValue(element.DocumentElement, nsmgr, "Will")),
                new XElement("speed", GetAttrValue(element.DocumentElement, nsmgr, "Speed")),

                new XElement("str", GetAttrValue(element.DocumentElement, nsmgr, "Str")),
                new XElement("dex", GetAttrValue(element.DocumentElement, nsmgr, "Dex")),
                new XElement("con", GetAttrValue(element.DocumentElement, nsmgr, "Con")),
                new XElement("int", GetAttrValue(element.DocumentElement, nsmgr, "Int")),
                new XElement("wis", GetAttrValue(element.DocumentElement, nsmgr, "Wis")),
                new XElement("cha", GetAttrValue(element.DocumentElement, nsmgr, "Cha")),

                new XElement("bab", GetAttrValue(element.DocumentElement, nsmgr, "BaseAttack")),
                new XElement("cmb", GetCMB(element.DocumentElement, nsmgr)),
                new XElement("cmd", GetCMD(element.DocumentElement, nsmgr)),

                new XElement("feats", GetAllWithLabelStartingWith(element.DocumentElement, nsmgr, "Feat")),
                new XElement("skills", GetSkills(element.DocumentElement, nsmgr)),
                new XElement("languages", GetAllWithLabelStartingWith(element.DocumentElement, nsmgr, "Lang")),
                new XElement("sq", GetAttrValue(element.DocumentElement, nsmgr, "Notes"))
            );

            GetActions(element.DocumentElement, nsmgr, mainElement);
            XDocument xmlTree = new XDocument(new XElement("campaign",new XElement(isPc?"pc":"npc", mainElement)));

            XDocument newTree = new XDocument();
            using (XmlWriter writer = newTree.CreateWriter())
            {
                // Load the style sheet.  
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(XmlReader.Create(@"character.xslt"));
                // Execute the transformation and output the results to a writer.  
                xslt.Transform(xmlTree.CreateReader(), writer);
            }
            return newTree;
        }

        private string GetAllWithLabelStartingWith(XmlElement doc, XmlNamespaceManager nsmgr, string name)
        {
            var nodes = doc.SelectNodes("descendant::bk:node[starts-with(@name,'"+name+"')]", nsmgr);
            string result = "";
            foreach (XmlNode node in nodes)
            {
                result += ", " + node.InnerText;
            }
            if(result.Length >0)
            {
                result = result.Remove(0, 2);
            }
            return result;
        }
        
        private void GetActions(XmlElement doc, XmlNamespaceManager nsmgr, XElement tree)
        {
            for (int i = 1; i < 100; i++)
            {
                string weapon = "Weapon" + i.ToString("0");
                XmlNode node = doc.SelectSingleNode("descendant::bk:node[@name='"+ weapon + "']", nsmgr);
                if (node != null)
                {
                    XElement item = new XElement("action",
                        new XElement("actionName", node.InnerText),
                        new XElement("actionAttack", GetAttrValue(doc, nsmgr, weapon + "AB")),
                        new XElement("actionDamage", GetAttrValue(doc, nsmgr, weapon + "Damage")),
                        new XElement("actionCritical", GetAttrValue(doc, nsmgr, "Weapon1"+ i.ToString("0") + "Crit")));
                    tree.Add(item);
                }
                else
                {
                    break;
                }
            }
        }

        private string GetSkills(XmlElement doc, XmlNamespaceManager nsmgr)
        {
            string skillsList = "";
            for (int i = 1; i < 100; i++)
            {
                string skillName = "Skill" + i.ToString("00");
                XmlNode node = doc.SelectSingleNode("descendant::bk:node[@name='" + skillName + "']", nsmgr);
                if (node != null)
                {
                    XmlNode other = doc.SelectSingleNode("descendant::bk:node[@name='" + skillName + "Mod']", nsmgr);
                    skillsList += ", " + node.InnerText + " +" + other.InnerText;
                }
                else
                {
                    break;
                }
            }
            if (skillsList.Length > 0)
            {
                skillsList = skillsList.Remove(0, 2);
            }
            return skillsList;
        }

        public string GetAttrValue(XmlElement doc, XmlNamespaceManager nsmgr, string field)
        {
            string output = null;
            XmlNode node = doc.SelectSingleNode("descendant::bk:node[@name = '" + field + "']", nsmgr);
            if (node != null)
            {
                output = node.InnerText;
            }
            return output;
        }

        public string RemoveLowerCaseCharacters(string oldFormat)
        {
            if (oldFormat == null) return null;

            Regex rgx = new Regex("[^A-Z]");
            return rgx.Replace(oldFormat, "");
        }

        public string RemoveNumbers(string oldFormat)
        {
            if (oldFormat == null) return null;

            Regex rgx = new Regex(@"[\d-]");
            return rgx.Replace(oldFormat, "");
        }

        public string GetHPRow(XmlElement doc, XmlNamespaceManager nsmgr)
        {
            string hp = GetAttrValue(doc, nsmgr, "HP");
            return hp + "/" + hp + " (1dX+X)";
        }

        public int GetCMB(XmlElement doc, XmlNamespaceManager nsmgr)
        {
            XmlNode node = doc.SelectSingleNode("descendant::bk:node[@name = 'BaseAttack']", nsmgr);
            int value = 0, total = 0;
            if (int.TryParse(node.InnerText, out value)) total += value;

            node = doc.SelectSingleNode("descendant::bk:node[@name = 'StrMod']", nsmgr);
            if (int.TryParse(node.InnerText, out value)) total += value;

            node = doc.SelectSingleNode("descendant::bk:node[@name = 'ACSize']", nsmgr);
            if (int.TryParse(node.InnerText, out value)) total += value;
            return total;
        }

        public int GetCMD(XmlElement doc, XmlNamespaceManager nsmgr)
        {
            XmlNode node = doc.SelectSingleNode("descendant::bk:node[@name = 'BaseAttack']", nsmgr);
            int value = 0, total = 0;

            if (int.TryParse(node.InnerText, out value)) total += value;
            total += GetCMB(doc, nsmgr) + value;
            return total;
        }
    }
}
