using NeosAPKPatchingTool.Config;
using System.IO;
using System.Xml;

namespace NeosAPKPatchingTool.Modding
{
    internal class ManifestPatcher
    {
        const string ANDROID_SCHEMA = "http://schemas.android.com/apk/res/android";

        private string ManifestPath;
        private XmlDocument _doc;
        public ManifestPatcher(string path)
        {
            ManifestPath = Path.Combine(path, "AndroidManifest.xml");
            _doc = new XmlDocument();
            _doc.Load(ManifestPath);
        }

        public void PatchManifest()
        {
            try
            {
                var config = ConfigManager.Config;
                if (config.UseHandTracking) {
                    AddFeature("oculus.software.handtracking", false);
                    AddPermission("com.oculus.permission.HAND_TRACKING");
                }

                ModifyApplicationNode("debuggable", config.Debuggable);
                ModifyApplicationNode("requestLegacyExternalStorage", true);
                RemoveMetadata("com.samsung.android.vr.application.mode");

                WriteChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error patching AndroidManifest.xml:\n{0}", ex.Message);
                Thread.Sleep(5000);
                Environment.Exit(1);
            }
        }

        private void ModifyApplicationNode(string attribute, bool value)
        {
            XmlNode applicationNode = _doc.DocumentElement.SelectSingleNode("application");
            XmlAttribute appAttr = applicationNode.Attributes[attribute];
            if (appAttr != null) appAttr.Value = value.ToString().ToLower();
            else {
                appAttr = _doc.CreateAttribute("android", attribute, ANDROID_SCHEMA);
                appAttr.Value = value.ToString().ToLower();
                applicationNode.Attributes.Append(appAttr);
            }

        }

        private void AddPermission(string perm)
        {
            XmlNode permNode = _doc.CreateElement("uses-permission");

            XmlAttribute nameAttr = _doc.CreateAttribute("android", "name", ANDROID_SCHEMA);
            nameAttr.Value = perm;

            permNode.Attributes.Append(nameAttr);
            _doc.DocumentElement.AppendChild(permNode);
        }

        private void AddFeature(string feature, bool required = false)
        {
            XmlNode featureNode = _doc.CreateElement("uses-feature");

            XmlAttribute nameAttr = _doc.CreateAttribute("android", "name", ANDROID_SCHEMA);
            nameAttr.Value = feature;

            XmlAttribute requiredAttr = _doc.CreateAttribute("android", "required", ANDROID_SCHEMA);
            requiredAttr.Value = required.ToString().ToLower();

            featureNode.Attributes.Append(nameAttr);
            featureNode.Attributes.Append(requiredAttr);
            _doc.DocumentElement.AppendChild(featureNode);
        }

        private void RemoveMetadata(string name)
        {
            XmlNode applicationNode = _doc.DocumentElement.SelectSingleNode("application");
            XmlNodeList? metaNodes = applicationNode.SelectNodes("meta-data");

            if (metaNodes == null) return;
            foreach (XmlNode node in metaNodes)
            {
                var atrib = node.Attributes["android:name"];
                if (atrib.Value == name) applicationNode.RemoveChild(node);
            }
        }

        public void WriteChanges()
        {
            _doc.Save(ManifestPath);
        }
    }
}
