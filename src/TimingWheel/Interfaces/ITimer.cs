﻿using System;

namespace TimingWheel.Interfaces
{
    /// <summary>
    /// 时间轮计时器
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// 任务总数
        /// </summary>
        int TaskCount { get; }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="timeout">过期时间，相对时间</param>
        /// <param name="delegateTask">延时任务，请在内部处理异常</param>
        /// <returns>添加成功返回true，如果任务已过期会立即执行，然后返回false</returns>
        ITimeTask AddTask(TimeSpan timeout, Action delegateTask);

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="timeoutMs">过期时间戳，绝对时间</param>
        /// <param name="delegateTask">延时任务，请在内部处理异常</param>
        /// <returns></returns>
        /// <returns>添加成功返回true，如果任务已过期会立即执行，然后返回false</returns>
        ITimeTask AddTask(long timeoutMs, Action delegateTask);

        /// <summary>
        /// 启动
        /// </summary>
        void Start();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();

        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();

        /// <summary>
        /// 恢复
        /// </summary>
        void Resume();
    }
}