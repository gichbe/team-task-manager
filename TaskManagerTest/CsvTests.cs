using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using TaskStatusModel = TeamTaskManager.Model.TaskStatus;

namespace TaskManagerTest
{
    [TestClass]
    public class CsvTests
    {
        public static IEnumerable<object[]> CsvData()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "taskdata.csv");

            if (!File.Exists(path))
                throw new FileNotFoundException("CSV NOT FOUND!", path);

            foreach (var line in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var p = line.Split(',');

                if (p[0] == "Id")
                    continue;

                if (p.Length != 4)
                    continue;

                yield return new object[]
                {
                    int.Parse(p[0]),
                    p[1],
                    int.Parse(p[2]),
                    p[3]
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(CsvData), DynamicDataSourceType.Method)]
        public void Csv_Test(int id, string title, int priority, string expected)
        {
            TaskStatusModel status = TaskStatusModel.ToDo;

            if (priority >= 3) status = TaskStatusModel.InProgress;
            if (priority == 4) status = TaskStatusModel.Testing;

            Assert.AreEqual(expected, status.ToString());
        }
    }
}
