﻿using System;
using System.Collections.Generic;

namespace InventoryTracker.Models
{
    public partial class IngredientType
    {
        public IngredientType()
        {
            Ingredients = new HashSet<Ingredient>();
        }

        public int IngredientTypeId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }
}
