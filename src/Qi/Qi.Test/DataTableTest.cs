using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qi.DataTables;

namespace Qi.Test
{
    /// <summary>
    ///This is a test class for DataTableTest and is intended
    ///to contain all DataTableTest Unit Tests
    ///</summary>
    [TestClass]
    public class DataTableTest
    {
        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion

        [TestMethod]
        public void Build()
        {
            decimal? f = 11m;
            decimal s = Convert.ToInt32(f);
            Assert.AreEqual(11m, s);
        }
        [TestMethod]
        public void CalcuteColumns()
        {
            var target = new DataTable<SettlementDetails>();
            var items = new List<SettlementDetails>
                            {
                                new SettlementDetails
                                    {
                                        Name = "A",
                                        Amount = 1m,
                                    },
                                new SettlementDetails
                                    {
                                        Name = "B",
                                        Amount = 2m,
                                        
                                    },
                                new SettlementDetails
                                    {
                                        Name = "C",
                                        Amount = 4m,
                                    }
                            };
            target.Column("Name", s => s.Name);
            target.Column("Ammount", s => s.Amount).Sum<decimal?>();
            target.Column("Amount1", s => s.Amount1);
            target.SetData(items);
            target.GetRows();
            Assert.AreEqual(7m, target.GetSummaries("Sum")[1]);
            Assert.AreEqual(7m, target.Columns["Ammount"].SumResult());
        }
        [TestMethod]
        public void BuildRepoert_empty()
        {
            var target = new DataTable<SettlementDetails>();
            target.Column("Name", s => s.Name);
            //計算列
            IColumn colum4CalculateRow1 = target.Column("Amount0", s => s.Amount).Sum<decimal?>();
            IColumn colum4CalculateRow2 = target.Column("Amount1", s => s.Amount1).Sum<decimal?>();
            target.Sum<decimal?>("Amount+Amount1", colum4CalculateRow1, colum4CalculateRow2);

            target.SetData(new List<SettlementDetails>());
            target.GetRows();
            Assert.AreEqual(null, colum4CalculateRow1.SumResult());
        }

        [TestMethod]
        public void BuildRepoert()
        {
            var items = new List<SettlementDetails>
                            {
                                new SettlementDetails
                                    {
                                        Name = "A",
                                        Amount = 1m,
                                    },
                                new SettlementDetails
                                    {
                                        Name = "B",
                                        Amount = 2m,
                                        Amount1 = 2,
                                    },
                                new SettlementDetails
                                    {
                                        Name = "C",
                                        Amount = 3m,
                                        Amount1 = 1,
                                    }
                            };
            var target = new DataTable<SettlementDetails>();
            target.Column("Name", s => s.Name);
            //計算列
            IColumn colum4CalculateRow1 = target.Column("Amount0", s => s.Amount).Sum<decimal?>();
            IColumn colum4CalculateRow2 = target.Column("Amount1", s => s.Amount1).Sum<decimal?>();
            target.Sum<decimal?>("Amount+Amount1", colum4CalculateRow1, colum4CalculateRow2);

            target.SetData(items);

            var columns = new[] { "Name", "Amount0", "Amount1", "Amount+Amount1" };
            int index = 0;
            //check column Name;
            foreach (IColumn column in target.Columns)
            {
                Assert.AreEqual(columns[index], column.Name);
                index++;
            }
            //check the row item0;
            index = 0;
            foreach (var item in target.GetRows())
            {
                Assert.AreEqual(items[index].Name, item[0]);
                Assert.AreEqual(items[index].Amount, item[1]);
                Assert.AreEqual(items[index].Amount1, item[2]);
                Assert.AreEqual((items[index].Amount ?? 0) + (items[index].Amount1 ?? 0), item[3], "row is " + index);
                index++;
            }

            Assert.AreEqual(items.Sum(item => item.Amount), (colum4CalculateRow1).SumResult());
            Assert.AreEqual(items.Sum(item => item.Amount1), (colum4CalculateRow2).SumResult());
        }

        #region Nested type: SettlementDetails

        public class SettlementDetails
        {
            public string Name { get; set; }
            public decimal? Amount { get; set; }

            public decimal? Amount1 { get; set; }
        }

        #endregion
    }
}