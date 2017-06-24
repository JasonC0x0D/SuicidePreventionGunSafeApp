using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Produces("text/plain")]
    //[Produces("application/json")]
    [Route("api/LocksApi")]
    public class LocksApiController : Controller
    {
        private readonly LockDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LocksApiController(LockDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // No current need for one lock to get all lock statuses.
        /*
        //// GET: api/LocksApi
        //[HttpGet]
        //public IEnumerable<Lock> GetLocks()
        //{
        //    return _context.Locks;
        //}
        */

        // GET: api/LocksApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLock([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @lock = await _context.Locks.SingleOrDefaultAsync(m => m.Id == id);

            if (@lock == null)
            {
                return NotFound();
            }

            if (@lock.locked == true)
            {
                return Content("[" + 1 + "]");
            }
            else
            {
                return Content("[" + 0 + "]");
            }

        }

        // GET: api/LocksApi/5/1
        [HttpGet("{id}/{alarm}")]
        [Route("{id}/{alarm}")]
        public async Task<IActionResult> AlarmLock([FromRoute] int id, [FromRoute] int alarm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @lock = await _context.Locks.SingleOrDefaultAsync(m => m.Id == id);

            if (@lock == null)
            {
                return NotFound();
            }

            bool alarmBool;

            if (alarm == 1) 
            {
                alarmBool = true;

                SendEmail emailAlert = new SendEmail();

                var allUsers = _userManager.Users;

                if (allUsers != null)
                {
                    foreach (var user in allUsers)
                    {
                        emailAlert.SendAlert(user.Email, @lock.Id);
                    }
                }
            }
            else alarmBool = false; 

            @lock.alarm = alarmBool;
            _context.Locks.Update(@lock);
            var result = await _context.SaveChangesAsync();

            if (result != 1)
            {
                return NotFound();
            }

            //return Ok(@lock);
            return Content("[" + @lock.locked + "]");
        }

        // Other CRUD methods are not needed.
        // May be usefull in some future verson.
        // Not doing any harm saying here.
        
        /*
        //// PUT: api/LocksApi/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutLock([FromRoute] int id, [FromBody] Lock @lock)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != @lock.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(@lock).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!LockExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/LocksApi
        //[HttpPost]
        //public async Task<IActionResult> PostLock([FromBody] Lock @lock)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.Locks.Add(@lock);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetLock", new { id = @lock.Id }, @lock);
        //}

        //// DELETE: api/LocksApi/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteLock([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var @lock = await _context.Locks.SingleOrDefaultAsync(m => m.Id == id);
        //    if (@lock == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Locks.Remove(@lock);
        //    await _context.SaveChangesAsync();

        //    return Ok(@lock);
        //}
        */

        private bool LockExists(int id)
        {
            return _context.Locks.Any(e => e.Id == id);
        }
    }
}