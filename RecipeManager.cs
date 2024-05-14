using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipeApplication
{
    public delegate void RecipeCaloriesExceededEventHandler(Recipe recipe, double totalCalories);

    public class RecipeManager
    {
        public List<Recipe> Recipes { get; } = new List<Recipe>(); // List to store recipes

        public event RecipeCaloriesExceededEventHandler RecipeCaloriesExceeded;

        public void AddRecipe()
        {
            Console.WriteLine("\nEnter details for the new recipe:");

            Console.Write("Name of the recipe: ");
            string name = Console.ReadLine();

            Recipe recipe = new Recipe();

            recipe.EnterIngredients();
            recipe.EnterSteps();

            Recipes.Add(recipe);
            Console.WriteLine("\nRecipe added successfully!");

            CheckRecipeCalories(recipe);
        }

        public void DisplayAllRecipes()
        {
            if (Recipes.Count == 0)
            {
                Console.WriteLine("\nNo recipes available.");
                return;
            }

            Console.WriteLine("\nAll Recipes:");
            var sortedRecipes = Recipes.OrderBy(r => r.Name);
            foreach (var recipe in sortedRecipes)
            {
                Console.WriteLine(recipe.Name);
            }

            Console.WriteLine("\nEnter the name of the recipe you want to display:");
            string selectedRecipeName = Console.ReadLine();
            var selectedRecipe = Recipes.FirstOrDefault(r => r.Name.Equals(selectedRecipeName, StringComparison.OrdinalIgnoreCase));
            if (selectedRecipe != null)
            {
                selectedRecipe.DisplayRecipe();
            }
            else
            {
                Console.WriteLine("Recipe not found.");
            }
        }

        public double CalculateTotalCalories(Recipe recipe)
        {
            return recipe.Ingredients.Sum(ingredient => ingredient.Calories);
        }

        public void CheckRecipeCalories(Recipe recipe)
        {
            double totalCalories = CalculateTotalCalories(recipe);
            if (totalCalories > 300)
            {
                RecipeCaloriesExceeded?.Invoke(recipe, totalCalories);
            }
        }
    }

    public class Recipe
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<Step> Steps { get; set; }

        public Recipe(string name)
        {
            Name = name;
            Ingredients = new List<Ingredient>();
            Steps = new List<Step>();
        }

        public void EnterIngredients()
        {
            Console.WriteLine("\nEnter ingredients:");
            while (true)
            {
                Console.WriteLine("Ingredient:");
                Console.Write("Name: ");
                string name = Console.ReadLine();
                if (string.IsNullOrEmpty(name))
                    break; // Exit loop if no name is provided

                Console.Write("Quantity: ");
                if (!double.TryParse(Console.ReadLine(), out double quantity))
                {
                    Console.WriteLine("Invalid input. Please enter a valid number for quantity.");
                    continue;
                }

                Console.Write("Unit: ");
                string unit = Console.ReadLine();

                Console.Write("Calories: ");
                if (!double.TryParse(Console.ReadLine(), out double calories))
                {
                    Console.WriteLine("Invalid input. Please enter a valid number for calories.");
                    continue;
                }

                string foodGroup;
                do
                {
                    Console.Write("Food Group (Carbohydrates, Protein, Dairy, Fruit and Veg, Fats and Sugars): ");
                    foodGroup = Console.ReadLine();
                    if (!IsValidFoodGroup(foodGroup))
                    {
                        Console.WriteLine("Invalid food group. Please enter one of the specified options.");
                    }
                } while (!IsValidFoodGroup(foodGroup));

                Ingredients.Add(new Ingredient { Name = name, Quantity = quantity, Unit = unit, Calories = calories, FoodGroup = foodGroup });
            }
        }

        public void EnterSteps()
        {
            Console.WriteLine("\nEnter steps:");
            while (true)
            {
                Console.WriteLine("Step:");
                Console.Write("Description: ");
                string description = Console.ReadLine();
                if (string.IsNullOrEmpty(description))
                    break; // Exit loop if no description is provided

                Steps.Add(new Step { Description = description });
            }
        }

        public void DisplayRecipe()
        {
            Console.WriteLine($"\nRecipe: {Name}");
            Console.WriteLine("Ingredients:");
            double totalCalories = 0;
            foreach (var ingredient in Ingredients)
            {
                Console.WriteLine($"{ingredient.Quantity} {ingredient.Unit} of {ingredient.Name}, Calories: {ingredient.Calories}, Food Group: {ingredient.FoodGroup}");
                totalCalories += ingredient.Calories;
            }
            Console.WriteLine($"\nTotal Calories: {totalCalories}");

            if (totalCalories > 300)
            {
                Console.WriteLine("\nWarning: Total calories exceed 300!");
            }

            Console.WriteLine("\nSteps:");
            for (int i = 0; i < Steps.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Steps[i].Description}");
            }
        }

        public void ScaleRecipe(double factor)
        {
            foreach (var ingredient in Ingredients)
            {
                ingredient.Quantity *= factor;
            }
        }

        public void ResetQuantities()
        {
            foreach (var ingredient in Ingredients)
            {
                ingredient.Quantity = 0;
            }
        }

        private bool IsValidFoodGroup(string foodGroup)
        {
            string[] validFoodGroups = { "Carbohydrates", "Protein", "Dairy", "Fruit and Veg", "Fats and Sugars" };
            return validFoodGroups.Contains(foodGroup, StringComparer.OrdinalIgnoreCase);
        }
    }

    public class Ingredient
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
        public double Calories { get; set; }
        public string FoodGroup { get; set; }
    }

    public class Step
    {
        public string Description { get; set; }
    }
}
