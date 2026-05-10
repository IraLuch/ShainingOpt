using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShainingOpt.DataBase;
using ShainingOpt.Models;
using ShainingOpt.ViewModels;

namespace ShainingOpt.Areas_Admin_Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Orders.Include(o => o.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
     .Include(o => o.User)
     .Include(o => o.OrderItems)
         .ThenInclude(oi => oi.Variant)
             .ThenInclude(v => v.Product)
     .Include(o => o.OrderItems)
         .ThenInclude(oi => oi.Variant)
             .ThenInclude(v => v.Size)
     .Include(o => o.OrderItems)
         .ThenInclude(oi => oi.Variant)
             .ThenInclude(v => v.Color)
     .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderNumber,UserId,OrderStatus,TotalAmount,DeliveryAddress,CreatedDate")] Order order)
        {
            ModelState.Remove("User");
            ModelState.Remove("OrderItems");
            order.CreatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(order.OrderNumber))
                {
                    order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 5)}";

                }
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
  .Include(o => o.User)
  .Include(o => o.OrderItems)
      .ThenInclude(oi => oi.Variant)
          .ThenInclude(v => v.Product)
  .Include(o => o.OrderItems)
      .ThenInclude(oi => oi.Variant)
          .ThenInclude(v => v.Size)
  .Include(o => o.OrderItems)
      .ThenInclude(oi => oi.Variant)
          .ThenInclude(v => v.Color)
  .FirstOrDefaultAsync(m => m.OrderId == id);

            var products = await _context.ProductVariants.Include(c => c.Color).Include(s => s.Size)
                .Include(p => p.Product).Where(p => p.Product.IsActive && p.MinOrderQuantity <= p.Quantity).ToListAsync();
                if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", order.UserId);

            var model = new EditOrderViewModel
            {
                Products = products,
                Order = order
            };
            return View(model);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }
            var orderInDb = await _context.Orders
     .Include(o => o.User)
     .Include(o => o.OrderItems)
         .ThenInclude(oi => oi.Variant)
             .ThenInclude(v => v.Product)
     .Include(o => o.OrderItems)
         .ThenInclude(oi => oi.Variant)
             .ThenInclude(v => v.Size)
     .Include(o => o.OrderItems)
         .ThenInclude(oi => oi.Variant)
             .ThenInclude(v => v.Color)
     .FirstOrDefaultAsync(m => m.OrderId == id);

            if (orderInDb == null) return NotFound();

            // 2. Обновляем только разрешенные поля
            orderInDb.OrderNumber = order.OrderNumber;
            orderInDb.UserId = order.UserId;
            orderInDb.OrderStatus = order.OrderStatus;
            orderInDb.DeliveryAddress = order.DeliveryAddress;
            orderInDb.CreatedDate = order.CreatedDate;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderInDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(orderInDb.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _context.Orders.Update(orderInDb);
            await _context.SaveChangesAsync();
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", order.UserId);
            var products = await _context.ProductVariants.Include(c => c.Color).Include(s => s.Size)
             .Include(p => p.Product).Where(p => p.Product.IsActive && p.MinOrderQuantity <= p.Quantity).ToListAsync();
            var model = new EditOrderViewModel
            {
                Products = products,
                Order = orderInDb
            };
            return View(model);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

        public async Task<IActionResult> AddToOrder(int orderId, int variantId,  int quantity)
        {
            var order = await _context.Orders
  .Include(o => o.User)
  .Include(o => o.OrderItems)
      .ThenInclude(oi => oi.Variant)
          .ThenInclude(v => v.Product)
  .Include(o => o.OrderItems)
      .ThenInclude(oi => oi.Variant)
          .ThenInclude(v => v.Size)
  .Include(o => o.OrderItems)
      .ThenInclude(oi => oi.Variant)
          .ThenInclude(v => v.Color)
  .FirstOrDefaultAsync(m => m.OrderId == orderId);
            if (order == null)
            {
                return NotFound();
            }
            var variant = await _context.ProductVariants.Include(p => p.Product).FirstOrDefaultAsync(v => v.ProductVariantId == variantId);
            if (variant == null)
            {
                return NotFound();
            }
           
            var availableQnt = Math.Min(quantity, variant.Quantity);
            if(availableQnt > 0)
            {
                 variant.Quantity -= availableQnt;
                _context.Update(variant);

            var orderItem = await _context.OrderItems.FirstOrDefaultAsync(o => o.OrderId == orderId && o.VariantId == variantId);
            if (orderItem == null)
            {
                orderItem = new OrderItem
                 {
                    OrderId = orderId,
                    VariantId = variantId,
                    Quantity = availableQnt,
                    TotalPrice = variant.Product.WholesalePrice * availableQnt
                 };

                    _context.OrderItems.Add(orderItem);
            }
            else
            {
                orderItem.Quantity += availableQnt;
                orderItem.TotalPrice = availableQnt * variant.Product.WholesalePrice;
                _context.Update(orderItem);
            }

            }

            order.TotalAmount = order.OrderItems.Sum(o => o.Variant.Product.WholesalePrice * o.Quantity);
            _context.Update(order);

            await _context.SaveChangesAsync();

            var products = await _context.ProductVariants.Include(c => c.Color).Include(s => s.Size)
               .Include(p => p.Product).Where(p => p.Product.IsActive && p.MinOrderQuantity <= p.Quantity).ToListAsync();

            

            var model = new EditOrderViewModel
            {
                Products = products,
                Order = order
            };
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", order.UserId);
            return View("Edit", model);
        }
    }
}
