using System;
using System.Collections.Generic;
using System.Text;

namespace ItalyFoodApp.Models
{
    public class Dish
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public string Ingredients { get; set; }
        public string PreparationTime { get; set; }

        public string NameEt { get; set; }
        public string DescriptionEt { get; set; }
        public string IngredientsEt { get; set; }
        public string PreparationTimeEt { get; set; }
    }
}
