using Microsoft.EntityFrameworkCore;

namespace InventoryTrackerTests;

public class Tests
{
    InventoryTrackerContext dbContext;
    
    [SetUp]
    public void Setup()
    {
        dbContext = new InventoryTrackerContext();
    }

    [Test]
    public void TestGetRecipes()
    {
        List<Recipe> recipes = dbContext.Recipes.OrderBy(r => r.Name).ToList();
        Assert.That(recipes.Count, Is.EqualTo(4));
        Assert.That(recipes[0].Name, Is.EqualTo("Cascade Orange Pale Ale"));
        PrintRecords(recipes);
    }

    [Test]
    public void TestGetRecipeIngredients()
    {
        // Get the relevant recipe with its related collections to access ingredients associated
        // with the recipe, on hand quantity, and whether an ingredient is on order or not
        Recipe? recipe = dbContext.Recipes.Include(
            recipe => recipe.RecipeIngredients).ThenInclude(
            i => i.Ingredient).ThenInclude(
            i => i.IngredientInventoryAdditions).SingleOrDefault(
            recipe => recipe.RecipeId == 3);
        // Note that some ingredients occur more than once, since some ingredients are used in multiple stages
        // The actual number of distinct ingredients is 6
        Assert.AreEqual(10, recipe.RecipeIngredients.Count());
        // Print out the name of the ingredient, on hand quantity, re-order point, and whether it is already on order
        foreach (RecipeIngredient i in recipe.RecipeIngredients.OrderBy(
                     i => i.Ingredient.Name))
        {
            Console.WriteLine($"Ingredient: {i.Ingredient.Name}\nOn Hand Quantity: " +
                              $"{i.Ingredient.OnHandQuantity}\nRe-Order Point: " +
                              $"{i.Ingredient.ReorderPoint}\nOn Order: " +
                              $"{i.Ingredient.IngredientInventoryAdditions.Count > 0}");
        }
    }

    [Test]
    public void TestGetProductOnHandFromRecipe()
    {
        // Get the relevant recipe with related collections to find the product associated with that recipe
        Recipe? recipe = dbContext.Recipes.Include(
            r => r.Batches).ThenInclude(
            b => b.Products).SingleOrDefault(
            r => r.RecipeId == 3);
        
        // Count the total number of products associated with the recipe
        int productCount = 0;
        foreach (Batch b in recipe.Batches)
        {
            productCount += b.Products.Count;
        }
        // No product currently: we need to make some!
        Assert.AreEqual(0, productCount);
        // Print out the Batch ID, amount made, and amount remaining (to see how well it's selling)
        foreach (Batch b in recipe.Batches)
        {
            foreach (Product p in b.Products)
            {
                Console.WriteLine($"Batch ID: {p.BatchId}\nQuantity Racked: " +
                                  $"{p.QuantityRacked}\nQuantity Remaining: " +
                                  $"{p.QuantityRemaining}");    
            }
        }
    } 

    [Test]
    public void TestGetBrewHistory()
    {
        // Get the list of batches associated with the relevant recipe
        Recipe? recipe = dbContext.Recipes.Include(
            r => r.Batches).SingleOrDefault(
            r => r.RecipeId == 3);
        // One record inserted in the testing process
        Assert.That(recipe.Batches, Has.Count.EqualTo(1));
        // display the batch number, date brewed, and notes
        foreach (Batch b in recipe.Batches)
        {
            Console.WriteLine($"Batch ID: {b.BatchId}\nBeginning Brew Date: {b.StartDate}\nNotes: {b.Notes}");
        }
    }
    
    [Test]
    public void TestAddBatch()
    {
        // Plan the batch details
        Batch batch = new Batch();
        batch.RecipeId = 3;
        batch.EquipmentId = 1;
        batch.Volume = 200;
        batch.StartDate = DateTime.Parse("11-20-2020");
        // Add it to the batch table
        dbContext.Batches.Add(batch);
        dbContext.SaveChanges();
        // Get the updated contents of the batch table
        List<Batch> batches = dbContext.Batches.ToList();
        Assert.That(batches, Has.Count.EqualTo(1));
        Assert.That(batches.First().RecipeId == 3);
        Assert.That(batches.First().Volume == 200);
        Assert.That(batches.First().StartDate.Equals( DateTime.Parse("11-20-2020")));
        // Show the inputted details of the batch to ascertain that it was created properly
        Console.WriteLine($"Batch ID: {batches.First().BatchId} " +
                          $"Recipe ID: {batches.First().RecipeId} " +
                          $"Volume: {batches.First().Volume}");
    }

    public void PrintRecords(List<Recipe> recipes)
    {
        foreach (var rec in recipes)
        {
            Console.WriteLine(rec.Name);
        }
    }
}