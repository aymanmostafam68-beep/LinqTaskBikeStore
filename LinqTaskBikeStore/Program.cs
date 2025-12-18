using System.Net;
using System.Threading.Channels;
using LinqTaskBikeStore.DataAccess;
using LinqTaskBikeStore.Models;

namespace LinqTaskBikeStore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BikeStoresContext context = new BikeStoresContext();
            bool exit = true;



            do
            {
                Console.WriteLine("---------------------------------------------------");


                loadData();
                string? input = Console.ReadLine();
              
                switch (input)
                {
                    case "1":
                        Console.WriteLine("1-List all customers' first and last names along with their email addresses.");

                        List<Customer> customers = context.Customers.ToList();
                        foreach (var customer in customers)
                        {
                            Console.WriteLine($"{customer.CustomerId}: {customer.FirstName} {customer.LastName} - {customer.Email}");

                        }
                        break;

                    case "2":
                        Console.WriteLine(value: "2- Retrieve all orders processed by a specific staff member (e.g., staff_id = 3).");
                        List<Order> orders = context.Orders
                            .Where(o => o.StaffId == 3)
                            .ToList();
                        if (orders.Count == 0)
                        {
                            Console.WriteLine("No orders found for the staff member.");
                            break;
                        }
                        foreach (var order in orders)
                        {
                            Console.WriteLine($"Order ID: {order.OrderId}, Customer ID: {order.CustomerId}, Order Date: {order.OrderDate}");
                        }
                            break;

                    case "3":
                        Console.WriteLine(value: "3- Get all products that belong to a category named \"Mountain Bikes\".");
                        var mountainBikeCategory = context.Categories
                            .FirstOrDefault(c => c.CategoryName == "Mountain Bikes");
                        try
                        {
                            var mountainBikeProducts = context.Products
                              .Where(p => p.CategoryId == mountainBikeCategory.CategoryId)
                              .ToList();
                            foreach (var product in mountainBikeProducts)
                                {
                                Console.WriteLine($"Product ID: {product.ProductId}, Product Name: {product.ProductName}, Category ID: {product.CategoryId}");
                            }

                        }
                        catch (Exception)
                        {

                            Console.WriteLine("Category 'Mountain Bikes' not found.");
                        }
                        break;


                    case "4":
                        Console.WriteLine(value: "4-Count the total number of orders per store.");
                        var CountOrdersPerStore = context.Orders
                            .GroupBy(o => o.StoreId).Select(g => new
                            {
                                StoreId = g.Key,
                                OrderCount = g.Count()
                            }).ToList();
                        foreach (var store in CountOrdersPerStore)
                        {
                            Console.WriteLine($"Store ID: {store.StoreId}, Total Orders: {store.OrderCount}");

                        }
                            break;

                    case "5":

                        Console.WriteLine(value: "5- List all orders that have not been shipped yet (shipped_date is null).");
                        try
                        {
                            var unshippedOrders = context.Orders
                       .Where(o => o.ShippedDate == null)
                       .ToList();
                            foreach (var order in unshippedOrders)
                            {
                                Console.WriteLine($"Order ID: {order.OrderId}, Customer ID: {order.CustomerId}, Order Date: {order.OrderDate}");
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("No unshipped orders found.");

                        }
                   

                        break;
                    case "6":

                        Console.WriteLine(value: "6- Display each customer’s full name and the number of orders they have placed.");

                        var CustomersOrders = context.Customers
                        .Select(c => new
                        {
                            FullName = c.FirstName + " " + c.LastName,
                            OrderCount = c.Orders.Count()
                        })
                        .ToList();

                        foreach (var c in CustomersOrders)
                        {
                            Console.WriteLine($"FullName: {c.FullName}, Total Orders: {c.OrderCount}");
                        }

                        break;
                    case "7":

                        Console.WriteLine(value: "7- List all products that have never been ordered (not found in order_items).");

                        var neverOrderedProducts = context.Products
                            .Where(p => !context.OrderItems.Any(oi => oi.ProductId == p.ProductId))
                            .ToList();
                        foreach (var product in neverOrderedProducts)
                        {
                            Console.WriteLine($"Product ID: {product.ProductId}, Product Name: {product.ProductName}");
                        }


                        break;
                    case "8":

                        Console.WriteLine(value: "8- Display products that have a quantity of less than 5 in any store stock.");


                        var lowStockProducts = context.Stocks
                            .Where(s => s.Quantity < 5)
                            .Select(g=> new
                            {
                                g.Product.ProductId,
                                g.Product.ProductName,
                                g.Quantity

                            } 
                            )
                            .ToList();

                        foreach (var product in lowStockProducts)
                        {
                            Console.WriteLine($"Product ID: {product.ProductId}, Product Name: {product.ProductName}.  Qty: {product.Quantity}");
                        }

                        break;
                    case "9":

                        Console.WriteLine(value: "9- Retrieve the first product from the products table.");

                       var firstProduct = context.Products
                            .FirstOrDefault();
                        if (firstProduct != null)
                        {

                            Console.WriteLine($"Product ID: {firstProduct.ProductId}, Product Name: {firstProduct.ProductName}");
                        }


                            break;
                    case "10":


                        Console.WriteLine(value: "11- Display each product with the number of times it was ordered.");
                      Console.WriteLine("Enter the model year to filter products:");
                        int modelYearInput =Convert.ToInt32( Console.ReadLine() ?? "0");

                        var productsByModelYear = context.Products
                            .Where(p => p.ModelYear == modelYearInput)
                            .ToList();

                        foreach (var product in productsByModelYear)
                        {
                            Console.WriteLine($"Product ID: {product.ProductId}, Product Name: {product.ProductName}, Model Year: {product.ModelYear}");
                        }



                        break;
                    case "11":

                        Console.WriteLine("11- Display each product with the number of times it was ordered.");

                        var productOrderCounts = context.Products
                            .Select(p => new
                            {
                                p.ProductId,
                                p.ProductName,
                                OrderCount = p.OrderItems.Count(),
                                UnitsSold = p.OrderItems.Sum(oi => oi.Quantity)

                            })
                            .ToList();


                        foreach (var product in productOrderCounts)
                        {
                            Console.WriteLine(
                                $"Product ID: {product.ProductId}, Product Name: {product.ProductName}, Times Ordered: {product.OrderCount},UnitsSold : {product.UnitsSold}"
                            );
                        }

                        break;


                    case "12":

                        Console.WriteLine(value: "12- Count the number of products in a specific category.");
                        Console.WriteLine("Enter the category name:");
                        string categoryNameInput = Console.ReadLine();

                        var productCount = context.Products
                         .Count(p => p.Category.CategoryName.ToLower() == categoryNameInput.ToLower());
                        if (productCount > 0)
                        {
                            Console.WriteLine($"Category: {categoryNameInput}, Product Count: {productCount}");
                        }
                        else
                        {
                            Console.WriteLine($"No products found in category: {categoryNameInput}");
                        }

                        break;

                    case "13":

                        Console.WriteLine(value: "13- Calculate the average list price of products.");
                        var averageListPrice = context.Products
                            .Average(p => p.ListPrice);
                        Console.WriteLine($"Average List Price of Products: {averageListPrice:C}");

                        break;

                    case "14":
                        Console.WriteLine(value: "14- Retrieve a specific product from the products table by ID.");
                        Console.WriteLine("Enter the product ID:");
                        int productIdInput = Convert.ToInt32(Console.ReadLine() ?? "0");
                        var specificProduct = context.Products
                            .SingleOrDefault(p => p.ProductId == productIdInput);
                        if (specificProduct != null)
                        {
                            Console.WriteLine($"Product ID: {specificProduct.ProductId}, Product Name: {specificProduct.ProductName}, List Price: {specificProduct.ListPrice:C}");
                        }

                        break;
                    case "15":
                        Console.WriteLine(value: "15- List all products that were ordered with a quantity greater than 3 in any order.");
                        var OrdersQtyGreaterThan3 =context.OrderItems
                            .Where(e => e.Quantity > 3)
                            .Select(oi => new
                            {
                                oi.Product.ProductId,
                                oi.Product.ProductName,
                                oi.Quantity
                            })
                            .Distinct()
                            .ToList();
                        if (OrdersQtyGreaterThan3.Count == 0)
                        {
                            Console.WriteLine("No products found with quantity greater than 3 in any order.");
                            break;
                        }
                        else
                        {
                            foreach (var item in OrdersQtyGreaterThan3)
                            {
                                Console.WriteLine($"Product ID: {item.ProductId}, Product Name: {item.ProductName}, Quantity Ordered: {item.Quantity}");
                            }
                        }
                         

                            break;
                    case "16":

                        Console.WriteLine(value: "16- Display each staff member’s name and how many orders they processed.");
                        var staffOrderCounts = context.Staffs
                            .Select(s => new
                            {
                                FullName = s.FirstName + " " + s.LastName,
                                OrderCount = s.Orders.Count()
                            })

                            .ToList();
                        foreach (var staffs in staffOrderCounts)
                        {
                            Console.WriteLine($"Staff Name: {staffs.FullName}, Orders Processed: {staffs.OrderCount}");
                        }
                        break;
                    case "17":

                        Console.WriteLine(value: "17- List active staff members only (active = true) along with their phone numbers.");

                        var activeStaff = context.Staffs
                            .Where(s => s.Active==1)
                            .Select(s => new
                            {
                                FullName = s.FirstName + " " + s.LastName,
                                s.Phone
                            })
                            .ToList();

                        foreach (var staff in activeStaff)
                        {
                            Console.WriteLine($"Staff Name: {staff.FullName}, Phone: {staff.Phone}");
                        }

                        break;
                    case "18":
                        Console.WriteLine("18- List all products with their brand name and category name.");
                        var productBrandCategry= context.Products
                            .Select(p => new
                            {
                                p.ProductId,
                                p.ProductName,
                                BrandName = p.Brand.BrandName,
                                CategoryName = p.Category.CategoryName
                            })
                            .ToList();
                        foreach (var item in productBrandCategry)
                        {
                            Console.WriteLine($"Product ID: {item.ProductId}, Product Name: {item.ProductName}, Brand: {item.BrandName}, Category: {item.CategoryName}");
                        }
                        break;
                    case "19":
                     

                        Console.WriteLine("19- Retrieve orders that are completed.");

                        var completedOrders = context.Orders
                            .Where(o => o.OrderStatus==1)
                            .ToList();
                        foreach (var order in completedOrders)
                        {
                            Console.WriteLine($"Order ID: {order.OrderId}, Customer ID: {order.CustomerId}, Order Date: {order.OrderDate}, Status: {order.OrderStatus}");
                        }
                            break;
                    case "20":

                        Console.WriteLine("20- List each product with the total quantity sold (sum of quantity from order_items).");
                        var productTotalQuantities = context.Products
                            .Select(p => new
                            {
                                p.ProductId,
                                p.ProductName,
                                TotalQuantitySold = p.OrderItems.Sum(oi => oi.Quantity)
                            })
                            .ToList();
                        foreach (var product in productTotalQuantities)
                        {
                            Console.WriteLine($"Product ID: {product.ProductId}, Product Name: {product.ProductName}, Total Quantity Sold: {product.TotalQuantitySold}");
                        }
                            break;

                    case "0":
                        exit = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

            } while (exit);






        }

        private static void loadData()
        {
            Console.WriteLine("1-List all customers' first and last names along with their email addresses.");
            Console.WriteLine(value: "2- Retrieve all orders processed by a specific staff member (e.g., staff_id = 3).");

            Console.WriteLine(value: "3- Get all products that belong to a category named \"Mountain Bikes\".");

            Console.WriteLine(value: "4-Count the total number of orders per store.");

            Console.WriteLine(value: "5- List all orders that have not been shipped yet (shipped_date is null).");

            Console.WriteLine(value: "6- Display each customer’s full name and the number of orders they have placed.");

            Console.WriteLine(value: "7- List all products that have never been ordered (not found in order_items).");

            Console.WriteLine(value: "8- Display products that have a quantity of less than 5 in any store stock.");

            Console.WriteLine(value: "9- Retrieve the first product from the products table.");

            Console.WriteLine(value: "10- Retrieve all products from the products table with a certain model year.");
            Console.WriteLine(value: "11- Display each product with the number of times it was ordered.");

            Console.WriteLine(value: "12- Count the number of products in a specific category.");

            Console.WriteLine(value: "13- Calculate the average list price of products.");

            Console.WriteLine(value: "14- Retrieve a specific product from the products table by ID.");

            Console.WriteLine(value: "15- List all products that were ordered with a quantity greater than 3 in any order.");

            Console.WriteLine(value: "16- Display each staff member’s name and how many orders they processed.");
            Console.WriteLine(value: "17- List active staff members only (active = true) along with their phone numbers.");

            Console.WriteLine("18- List all products with their brand name and category name.");
            Console.WriteLine("19- Retrieve orders that are completed.");

            Console.WriteLine("20- List each product with the total quantity sold (sum of quantity from order_items).");

            Console.WriteLine("---------------------------------------------------");

            Console.Write("Select an option (1-20) or '0' to quit: ");

        }
    }
}
