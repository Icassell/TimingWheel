using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimingWheel.UnitTest
{
    public class TimingWheelTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestTimingWheel()
        {
            var outputs = new Dictionary<string, DateTime>();

            // �뼶ʱ����
            var timer = TimingWheelTimer.Build(TimeSpan.FromSeconds(1), 10);

            outputs.Add("00", DateTime.Now);

            timer.AddTask(TimeSpan.FromMilliseconds(5000), () => { outputs.Add("20", DateTime.Now); });
            timer.AddTask(TimeSpan.FromMilliseconds(2000), () => { outputs.Add("11", DateTime.Now); });

            timer.Start();

            timer.AddTask(TimeSpan.FromSeconds(12), () => { outputs.Add("30", DateTime.Now); });
            timer.AddTask(TimeSpan.FromSeconds(2), () => { outputs.Add("12", DateTime.Now); });

            await Task.Delay(TimeSpan.FromSeconds(15));
            timer.Stop();

            outputs.Add("99", DateTime.Now);

            Console.WriteLine(string.Join(Environment.NewLine, outputs.Select(o => $"{o.Key}, {o.Value:HH:mm:ss.ffff}")));

            Assert.AreEqual(6, outputs.Count);
            Assert.AreEqual(2, Calc(outputs.Skip(1).First().Value, outputs.First().Value));
            Assert.AreEqual(2, Calc(outputs.Skip(2).First().Value, outputs.First().Value));
            Assert.AreEqual(5, Calc(outputs.Skip(3).First().Value, outputs.First().Value));
            Assert.AreEqual(12, Calc(outputs.Skip(4).First().Value, outputs.First().Value));
        }

        private static int Calc(DateTime dt1, DateTime dt2)
        {
            return (int)(CutOffMillisecond(dt1) - CutOffMillisecond(dt2)).TotalSeconds;
        }

        /// <summary>
        /// �ص����벿��
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static DateTime CutOffMillisecond(DateTime dt)
        {
            return new DateTime(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond), dt.Kind);
        }
    }
}