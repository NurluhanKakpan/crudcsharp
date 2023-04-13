using System.Text.RegularExpressions;

List <Product>  products = new List<Product>
{
new(){  Id = Guid.NewGuid().ToString(),location = "Astana",Title = "Car",price = 20000000},
new(){  Id = Guid.NewGuid().ToString(),location = "Kyzylorda",Title = "IPhone" ,price = 1000000},
};


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    var path = request.Path;
    string expressionForGuid = @"^/api/products/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";
    if (path == "/api/products" && request.Method == "GET")
    {
        await GetAllProducts(response);
    }
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "GET")
    {
        string? id = path.Value?.Split("/")[3];
        await GetProductById(id, response);
    }

    else if (path == "/api/products" && request.Method == "POST")
    {
        await createProduct(response, request);
    }
    else if(Regex.IsMatch(path,expressionForGuid) && request.Method == "DELETE" )
    {
        string? id = path.Value?.Split("/")[3];
        await DeleteProduct(id, response);
    }
    else if (path == "/api/products" && request.Method == "PUT" )
    {
        await UpdateProduct(response, request);
    }
    else
    {
        response.ContentType = "text/html,charset = utf-8";
        await response.SendFileAsync("html/index.html");
    }
});

app.Run();


async Task GetAllProducts(HttpResponse response)
{
    await response.WriteAsJsonAsync(products);
}

async Task GetProductById(string? id, HttpResponse response)
{
    Product? product = products.FirstOrDefault((p) => p.Id == id);
    if (product != null)
    {
        await response.WriteAsJsonAsync(product);
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Can not find products" });
    }
}

async Task createProduct(HttpResponse response, HttpRequest request)
{
    try
    {
        var product = await request.ReadFromJsonAsync<Product>();
        if (product != null)
        {
            product.Id = Guid.NewGuid().ToString();
            products.Add(product);
            await response.WriteAsJsonAsync(product);
        }
        else
        {
            response.StatusCode = 400;
            await response.WriteAsJsonAsync(new { message = "You cannot create" });
        }

    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}

async Task DeleteProduct(string? id, HttpResponse response)
{
    Product? product = products.FirstOrDefault((p) => p.Id == id);
    if (product != null)
    {
        products.Remove(product);
        await response.WriteAsJsonAsync(product);
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "You cannot delete" });
        
    }
}


async Task UpdateProduct(HttpResponse response, HttpRequest request)
{
    try
    {
        Product? productData = await request.ReadFromJsonAsync<Product>();
        if (productData != null)
        {
            var product = products.FirstOrDefault((p) => p.Id == productData.Id);
            if (product != null)
            {
                product.location = productData.location;
                product.price = productData.price;
                product.Title = productData.Title;
                await response.WriteAsJsonAsync(product);
            }
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "You can not change" });
            }
        }
        else
        {
            response.StatusCode = 404;
            await response.WriteAsJsonAsync("you can not change");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
}

public class Product
{
    public string Id { get; set; } = "";
    public string location { get; set; } = "";
    public string Title { get; set; } = "";
    public int price { get; set; }
}