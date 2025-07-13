using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeSupplies.Core.Entities;
using OfficeSupplies.Infrastructure.Data;

namespace OfficeSupplies.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly OfficeSuppliesContext _context;

    public ItemsController(OfficeSuppliesContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetItems()
    {
        return await _context.Items
            .Where(i => i.IsActive)
            .OrderBy(i => i.ItemName)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> GetItem(int id)
    {
        var item = await _context.Items.FindAsync(id);

        if (item == null || !item.IsActive)
        {
            return NotFound();
        }

        return item;
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<ActionResult<Item>> GetItemByBarcode(string barcode)
    {
        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.ItemCode == barcode && i.IsActive);

        if (item == null)
        {
            return NotFound();
        }

        return item;
    }

    [HttpPost]
    public async Task<ActionResult<Item>> CreateItem(Item item)
    {
        item.CreatedAt = DateTime.Now;
        item.UpdatedAt = DateTime.Now;
        
        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetItem), new { id = item.ItemId }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(int id, Item item)
    {
        if (id != item.ItemId)
        {
            return BadRequest();
        }

        item.UpdatedAt = DateTime.Now;
        _context.Entry(item).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ItemExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    private bool ItemExists(int id)
    {
        return _context.Items.Any(e => e.ItemId == id);
    }
}