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
        // Get the list of ingredient ids for the recipe to cross-reference with the ingredient table.
        // Note that some ingredients occur more than once in a recipe because they are used in multiple stages.
        List<int> ingredientById = dbContext.RecipeIngredients.Where(
            i => i.RecipeId == 3).Select(i => i.IngredientId).Distinct().ToList();
        // Use the ingredient id to get the list of ingredients for that recipe
        List <Ingredient> ingredients = dbContext.Ingredients.Where(i => ingredientById.Contains(i.IngredientId)).ToList();
        // Use the ingredient id to get the list of ingredients for that recipe that are on order
        List <IngredientInventoryAddition> ingredientsOnOrder = dbContext.IngredientInventoryAdditions.Where(i => ingredientById.Contains(i.IngredientId)).ToList();
        // Once duplicates have been removed, the number of ingredients in this recipe is 6
        Assert.That(ingredientById, Has.Count.EqualTo(6));
        Assert.That(ingredients, Has.Count.EqualTo(6));
        // Show the ingredient name along with on hand quantity
        foreach (Ingredient i in ingredients)
        {
            Console.WriteLine(i.Name + " - " + i.OnHandQuantity);
        }

        foreach (IngredientInventoryAddition o in ingredientsOnOrder)
        {
            Console.WriteLine(ingredients.FirstOrDefault(i => i.IngredientId == o.IngredientId)
                .Name + ": " + o.EstimatedDeliveryDate);
        }
    }

    [Test]
    public void TestGetProductOnHand()
    {
        // Get the batch number from the batch table for the desired recipe
        List<int> batch = dbContext.Batches.Where(b => b.RecipeId == 3).Select(b => b.BatchId).ToList();
        // Use the batch number to get the product on hand
        List<Product> products = dbContext.Products.Where(p => batch.Contains(p.BatchId)).ToList();
        // Currently, there are no records in either table
        Assert.That(products, Has.Count.EqualTo(0));
        // Show quantity on hand as well as quantity racked to see how much has sold, as well as how much remains
        foreach (Product p in products)
        {
            Console.WriteLine(p.BatchId + " - " + p.QuantityRemaining + " - " + p.QuantityRacked);
        }
    }

    [Test]
    public void TestGetBrewHistory()
    {
        List<Batch> batches = dbContext.Batches.Where(b => b.RecipeId == 3).ToList();
        // Once again, no records currently in table
        Assert.That(batches, Has.Count.EqualTo(1));
        // display the batch number, date brewed, and notes
        foreach (Batch b in batches)
        {
            Console.WriteLine(b.BatchId + " - " + b.StartDate + " - " + b.Notes);
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
        Console.WriteLine(batches.First().BatchId + " - " + batches.First().RecipeId + " - " + batches.First().Volume);
    }

    public void PrintRecords(List<Recipe> recipes)
    {
        foreach (var rec in recipes)
        {
            Console.WriteLine(rec.Name);
        }
    }
}