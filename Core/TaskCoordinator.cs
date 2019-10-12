﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aptacode.Core.Tasks;

namespace Aptacode.Core
{
    public class TaskCoordinator
    {
        private readonly List<BaseTask> pendingTasks;
        private readonly List<BaseTask> runningTasks;
        private static readonly Object mutex = new Object();
        public bool IsRunning { get; set; }

        public TaskCoordinator()
        {
            pendingTasks = new List<BaseTask>();
            runningTasks = new List<BaseTask>();
            IsRunning = false;
        }

        public void Apply(BaseTask action)
        {
            pendingTasks.Add(action);
        }

        public void Start()
        {
            IsRunning = true;

            new TaskFactory().StartNew(() =>
            {
                run().Wait();
            });
        }

        public void Stop()
        {
            IsRunning = false;
        }

        private async Task run()
        {
            while (IsRunning)
            {
                lock (mutex)
                {
                    List<BaseTask> readyTasks = getReadyTasks();
                    cleanUpPendingTasks(readyTasks);
                    startTasks(readyTasks);
                }

                await Task.Delay(1).ConfigureAwait(false);
            }
        }

        private List<BaseTask> getReadyTasks()
        {
            List<BaseTask> readyTasks = new List<BaseTask>();

            foreach (var item in pendingTasks)
            {
                if (!runningTasks.Exists(t => t.CollidesWith(item)) && !readyTasks.Exists(t => t.CollidesWith(item)))
                {
                    readyTasks.Add(item);
                }
            }
            return readyTasks;
        }

        private void startTasks(List<BaseTask> readyTasks)
        {
            foreach (var task in readyTasks)
            {
                BaseTask localTask = task;
                runningTasks.Add(localTask);

                localTask.OnFinished += (s, e) =>
                {
                    lock (mutex)
                    {
                        runningTasks.Remove(localTask);
                    }
                };

                localTask.StartAsync().ConfigureAwait(false);
            }
        }
        private void cleanUpPendingTasks(List<BaseTask> startedTasks)
        {
            foreach (var item in startedTasks)
            {
                pendingTasks.Remove(item);
            }
        }
    }
}
