using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qi.Threads;

namespace Qi.Test
{
    [TestClass]
    public class MultiThreadAssignmentTest
    {
        [TestMethod]
        public void TestCase_sum_0_100_in_synre()
        {
            var sumData = new int[100];
            var actual = 0;
            for (int i = 0; i < sumData.Length; i++)
            {
                sumData[i] = i;
                actual += i;
            }
            var result = 0;
            MultiExecute<int> MultiSummary = new MultiExecute<int>(5, s => result += s.Sum());
            MultiSummary.Execute(sumData);
            Assert.AreEqual(actual, result);
        }



        [TestMethod]
        public void TestCase_sum_0_100_in_asynre()
        {
            var sumData = new int[100];
            var actual = 0;
            for (int i = 0; i < sumData.Length; i++)
            {
                sumData[i] = i;
                actual += i;
            }
            var result = 0;
            MultiExecute<int> MultiSummary = new MultiExecute<int>(5, s => result += s.Sum());
            MultiSummary.Execute(sumData);
            Assert.AreEqual(actual, result);
        }

        [TestMethod]
        public void TestCase_sum_0_to_3_use_5_thread_on_setting_in_synre()
        {
            var sumData = new int[3];
            var actual = 0;
            for (int i = 0; i < sumData.Length; i++)
            {
                sumData[i] = i;
                actual += i;
            }
            var result = 0;
            var MultiSummary = new MultiExecute<int>(5, s => result += s.Sum());
            MultiSummary.Execute(sumData);
            Assert.AreEqual(actual, result);
        }

      
        
    }
}
