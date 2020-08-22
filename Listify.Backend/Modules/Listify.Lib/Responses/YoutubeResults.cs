﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Listify.Lib.Responses
{
    public class YoutubeResults
    {
        public ICollection<YoutubeResult> results { get; set; }

        public class YoutubeResult
        {
            public Guid Id { get; set; }
            public string SongName { get; set; }
            public int LengthSec { get; set; }
            public string VideoId { get; set; }
        }
    }
}
