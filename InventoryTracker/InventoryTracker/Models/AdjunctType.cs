﻿using System;
using System.Collections.Generic;

namespace InventoryTracker.Models
{
    public partial class AdjunctType
    {
        public AdjunctType()
        {
            Adjuncts = new HashSet<Adjunct>();
        }

        public int AdjunctTypeId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Adjunct> Adjuncts { get; set; }
    }
}
