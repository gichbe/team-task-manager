using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using TaskStatusModel = TeamTaskManager.Model.TaskStatus;

namespace TaskManagerTest
{
    [TestClass]
    public class XmlTests
    {
        public static IEnumerable<object[]> XmlData()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "taskdata.xml");

            if (!File.Exists(path))
                throw new FileNotFoundException("XML NOT FOUND!", path);

            var doc = XDocument.Load(path);

            foreach (var t in doc.Root.Elements("Task"))
            {
                yield return new object[]
                {
                    (int)t.Attribute("Id"),
                    (string)t.Attribute("Title"),
                    (int)t.Attribute("Priority"),
                    (string)t.Attribute("Expected")
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(XmlData), DynamicDataSourceType.Method)]
        public void Xml_Test(int id, string title, int priority, string expected)
        {
            TaskStatusModel status = TaskStatusModel.ToDo;

            if (priority >= 3) status = TaskStatusModel.InProgress;
            if (priority == 4) status = TaskStatusModel.Testing;

            Assert.AreEqual(expected, status.ToString());
        }
    }
}
