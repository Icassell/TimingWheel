using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TimingWheel.UnitTest
{
    public class TimingWheelTest
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// ����ʱ����
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// ��������״̬
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestTaskStatus()
        {
            var timer = TimingWheelTimer.Build(TimeSpan.FromSeconds(1), 10);
            timer.Start();

            var task1 = timer.AddTask(TimeSpan.FromSeconds(5), () => { Thread.Sleep(3000); });
            var task2 = timer.AddTask(TimeSpan.FromSeconds(5), () => { throw new Exception(); });
            var task3 = timer.AddTask(TimeSpan.FromSeconds(5), () => { throw new Exception(); });

            Assert.AreEqual(TimeTaskStaus.Wait, task1.TaskStaus);
            Assert.AreEqual(TimeTaskStaus.Wait, task2.TaskStaus);
            Assert.AreEqual(TimeTaskStaus.Wait, task3.TaskStaus);

            task3.Cancel();
            await Task.Delay(TimeSpan.FromSeconds(6));

            Assert.AreEqual(TimeTaskStaus.Running, task1.TaskStaus);
            Assert.AreEqual(TimeTaskStaus.Fail, task2.TaskStaus);
            Assert.AreEqual(TimeTaskStaus.Cancel, task3.TaskStaus);

            await Task.Delay(TimeSpan.FromSeconds(4));
            Assert.AreEqual(TimeTaskStaus.Success, task1.TaskStaus);

            timer.Stop();
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