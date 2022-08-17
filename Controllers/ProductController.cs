// Using statements
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductsCategories.Models;
using Microsoft.EntityFrameworkCore;


namespace ProductsCategories.Controllers;

public class ProductController : Controller
{
    private ProductsCategoriesContext _context;

    public ProductController( ProductsCategoriesContext context )
    {
        _context = context;
    }

    [ HttpGet("") ]
    public IActionResult Index()
    {
        // List<Product> AllProducts = _context.Products.ToList();
        // .Include( product => product.CategoriesAssociated )
        // foreach( Product product in AllProducts )
        // {
        //     Console.WriteLine( "Products: ", product.Name );
        // }
        ViewBag.Products = _context.Products;
        return View( "Products" );
    }

    [ HttpPost( "/product/create" ) ]
    public IActionResult Create( Product newProduct )
    {
        _context.Products.Add( newProduct );
        _context.SaveChanges();
        return Index();
    }

    [ HttpGet( "/product/view/{ProductId}" ) ]
    public IActionResult ViewProduct( int ProductId )
    {
        Product? product = _context.Products.FirstOrDefault( product => product.ProductId == ProductId );
        if( product == null )
        {
            return Index();
        }

        ViewBag.CatNoAss = _context.Categories
        .Include( c => c.ProductsAssociated )
        .Where( p => !p.ProductsAssociated.Any( p => p.ProductId == ProductId ) );

        ViewBag.CatList = _context.Products
        .Include( product => product.CategoriesAssociated )
        .ThenInclude( product => product.Category )
        .FirstOrDefault( product => product.ProductId == ProductId );

        ViewBag.Product = product;
        return View( "ViewProduct" );
    }

    [ HttpPost( "/product/addcategory/{productId}" ) ]
    public IActionResult AddCategory( Association association, int productId )
    {
        association.ProductId = productId;
        _context.Associations.Add( association );
        _context.SaveChanges();
        return ViewProduct( productId );
    }
}

