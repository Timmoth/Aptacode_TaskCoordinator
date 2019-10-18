﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Aptacode.TaskPlex.Tasks
{
    public abstract class GroupTask : BaseTask
    {
        protected GroupTask(List<IBaseTask> tasks)
        {
            Tasks = tasks;
        }

        protected List<IBaseTask> Tasks { get; set; }

        /// <summary>
        ///     Add a task to the group
        /// </summary>
        /// <param name="task"></param>
        public void Add(IBaseTask task)
        {
            if(task == null)
            {
                return;
            }

            Tasks.Add(task);
            Duration = GetTotalDuration(Tasks);
        }

        /// <summary>
        ///     Remove a task from the group
        /// </summary>
        /// <param name="task"></param>
        public void Remove(IBaseTask task)
        {
            if (task == null)
            {
                return;
            }

            Tasks.Remove(task);
            Duration = GetTotalDuration(Tasks);
        }
        public IEnumerable<int> GetHashCodes()
        {
            return Tasks.Select(p => p.GetHashCode());
        }
        protected abstract TimeSpan GetTotalDuration(List<IBaseTask> tasks);
    }
}