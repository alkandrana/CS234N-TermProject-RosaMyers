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
        Assert.AreEqual(4, recipes.Count);
        Assert.AreEqual("Cascade Orange Pale Ale", recipes[0].Name);
        foreach (Recipe r in recipes)
        {
            Console.WriteLine(r.Name);
        }
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
    public void TestGetProductOnHand()
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
        Assert.AreEqual(2, recipe.Batches.Count);
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
        batch.Volume = 37;
        batch.ScheduledStartDate = DateTime.Parse("11-20-2020");
        // Add it to the batch table
        dbContext.Batches.Add(batch);
        dbContext.SaveChanges();
        // Get the updated contents of the batch table
        List<Batch> batches = dbContext.Batches.ToList();
        Assert.AreEqual(2,batches.Count);
        Assert.AreEqual(3, batches.Last().RecipeId);
        Assert.AreEqual(37, batches.Last().Volume);
        Assert.AreEqual(DateTime.Parse("11-20-2020"), batches.Last().ScheduledStartDate);
        // Show the inputted details of the batch to ascertain that it was created properly
        Console.WriteLine($"Batch ID: {batches.Last().BatchId} " +
                          $"Recipe ID: {batches.Last().RecipeId} " +
                          $"Volume: {batches.Last().Volume}");
    }
}