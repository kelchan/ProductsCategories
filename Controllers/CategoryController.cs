// Using statements
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductsCategories.Models;
using Microsoft.EntityFrameworkCore;


namespace ProductsCategories.Controllers;

public class CategoryController : Controller
{
    private ProductsCategoriesContext _context;

    public CategoryController( ProductsCategoriesContext context )
    {
        _context = context;
    }

    [ HttpGet( "/categories" ) ]
    public IActionResult Categories()
    {
        // List<Category> AllCategories = _context.Categories.ToList();
        ViewBag.Categories = _context.Categories;
        return View( "Categories" );
    }

    [ HttpPost( "/category/create" ) ]
    public IActionResult Create( Category newCategory )
    {
        _context.Categories.Add( newCategory );
        _context.SaveChanges();
        return Categories();
    }

    [ HttpGet( "/category/view/{CategoryId}" ) ]
    public IActionResult ViewCategory( int CategoryId )
    {
        Category? category = _context.Categories.FirstOrDefault( category => category.CategoryId == CategoryId );
        if( category == null )
        {
            return Categories();
        }
        
        ViewBag.ProdNoAss = _context.Products
        .Include( c => c.CategoriesAssociated )
        .Where( p => !p.CategoriesAssociated.Any( p => p.CategoryId == CategoryId ) );

        ViewBag.ProdList = _context.Categories
        .Include( product => product.ProductsAssociated )
        .ThenInclude( product => product.Product )
        .FirstOrDefault( product => product.CategoryId == CategoryId );

        ViewBag.Category = category;
        return View( "ViewCategory" );
    }

    [ HttpPost( "/category/addproduct/{categoryId}" ) ]
    public IActionResult AddProduct( Association association, int categoryId )
    {
        association.CategoryId = categoryId;
        _context.Associations.Add( association );
        _context.SaveChanges();
        return ViewCategory( categoryId );
    }

}