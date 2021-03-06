﻿namespace JsonTestAdapter.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;

    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    [Export(typeof(ITestFilesUpdateWatcher))]
    public class TestFilesUpdateWatcher : IDisposable, ITestFilesUpdateWatcher
    {
        private class FileWatcherInfo
        {
            public FileWatcherInfo(FileSystemWatcher watcher)
            {
                this.Watcher = watcher;
                this.LastEventTime = DateTime.MinValue;
            }

            public FileSystemWatcher Watcher { get; set; }
            public DateTime LastEventTime { get; set; }
        }

        private IDictionary<string, FileWatcherInfo> fileWatchers;
        public event EventHandler<TestFileChangedEventArgs> FileChangedEvent;

        public TestFilesUpdateWatcher()
        {
            this.fileWatchers = new Dictionary<string, FileWatcherInfo>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddWatch(string path)
        {
            ValidateArg.NotNullOrEmpty(path, "path");

            if (!String.IsNullOrEmpty(path))
            {
                var directoryName = Path.GetDirectoryName(path);
                var fileName = Path.GetFileName(path);

                FileWatcherInfo watcherInfo;
                if (!this.fileWatchers.TryGetValue(path, out watcherInfo))
                {
                    watcherInfo = new FileWatcherInfo(new FileSystemWatcher(directoryName, fileName));
                    this.fileWatchers.Add(path, watcherInfo);

                    watcherInfo.Watcher.Changed += this.OnChanged;
                    watcherInfo.Watcher.EnableRaisingEvents = true;
                }
            }
        }

        public void RemoveWatch(string path)
        {
            ValidateArg.NotNullOrEmpty(path, "path");

            if (!String.IsNullOrEmpty(path))
            {
                FileWatcherInfo watcherInfo;
                if (this.fileWatchers.TryGetValue(path, out watcherInfo))
                {
                    watcherInfo.Watcher.EnableRaisingEvents = false;

                    this.fileWatchers.Remove(path);

                    watcherInfo.Watcher.Changed -= this.OnChanged;
                    watcherInfo.Watcher.Dispose();
                    watcherInfo.Watcher = null;
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            FileWatcherInfo watcherInfo;
            if (this.FileChangedEvent != null && this.fileWatchers.TryGetValue(e.FullPath, out watcherInfo))
            {
                var writeTime = File.GetLastWriteTime(e.FullPath);
                // Only fire update if enough time has passed since last update to prevent duplicate events
                if (writeTime.Subtract(watcherInfo.LastEventTime).TotalMilliseconds > 500)
                {
                    watcherInfo.LastEventTime = writeTime;
                    this.FileChangedEvent(sender, new TestFileChangedEventArgs(e.FullPath, TestFileChangedReason.Changed));
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.fileWatchers != null)
            {
                foreach (var fileWatcher in this.fileWatchers.Values)
                {
                    if (fileWatcher != null && fileWatcher.Watcher != null)
                    {
                        fileWatcher.Watcher.Changed -= this.OnChanged;
                        fileWatcher.Watcher.Dispose();
                    }
                }

                this.fileWatchers.Clear();
                this.fileWatchers = null;
            }
        }
    }
}