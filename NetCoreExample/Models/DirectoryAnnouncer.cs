using NetCoreExample.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreExample.Models
{
    internal class DirectoryAnnouncer : IAnnouncer
    {
        private readonly string _directory;

        public DirectoryAnnouncer(string directory)
        {
            _directory = directory;
        }
        public string Announce() => _directory;
    }
}
