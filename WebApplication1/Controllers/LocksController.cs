using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class LocksController : Controller
    {
        private readonly LockDbContext _context;
        private readonly UserManager<ApplicationUser>_userManager;


        public LocksController(LockDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Locks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Locks.ToListAsync());
        }

        // GET: Locks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @lock = await _context.Locks
                .SingleOrDefaultAsync(m => m.Id == id);
            if (@lock == null)
            {
                return NotFound();
            }

            return View(@lock);
        }

        // GET: Locks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,locked,request,alarm")] Lock @lock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@lock);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(@lock);
        }

        // GET: Locks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @lock = await _context.Locks.SingleOrDefaultAsync(m => m.Id == id);
            if (@lock == null)
            {
                return NotFound();
            }
            return View(@lock);
        }

        // POST: Locks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,locked,request,alarm")] Lock @lock)
        {
            if (id != @lock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@lock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LockExists(@lock.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(@lock);
        }


        // GET: Locks/MakeRequest/5
        public async Task<IActionResult> MakeRequest(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @lock = await _context.Locks.SingleOrDefaultAsync(m => m.Id == id);
            if (@lock == null)
            {
                return NotFound();
            }
            return View(@lock);
        }

        // POST: Locks/MakeRequest/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeRequest(int id, [Bind("Id,name,locked,request,alarm")] Lock @lock)
        {
            if (id != @lock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@lock);
                    await _context.SaveChangesAsync();
                    if (@lock.request == true) // Set the claim requestor to true. Prevents user from approving request.
                    {
                        var user = await _userManager.FindByNameAsync(User.Identity.Name);
                        await _userManager.AddClaimAsync(user, new Claim("Requestor", "true"));

                        SendEmail emailAlert = new SendEmail();

                        var allUsers = _userManager.Users;

                        if (allUsers != null)
                        {
                            foreach (var user2 in allUsers)
                            {
                                emailAlert.SendRequest(user2.Email, user.Email);
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LockExists(@lock.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(@lock);
        }

        // GET: Locks/ApproveRequest/5
        public async Task<IActionResult> ApproveRequest(int? id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var storedClaims = await _userManager.GetClaimsAsync(user);
            var requestorClaim = storedClaims.FirstOrDefault(c => c.Type == "Requestor");
            if (requestorClaim != null)
            {
                if (requestorClaim.Value == "true")
                {
                    return NotFound();
                }
            }

            if (id == null)
            {
                return NotFound();
            }

            var @lock = await _context.Locks.SingleOrDefaultAsync(m => m.Id == id);
            if (@lock == null || @lock.request == false)
            {
                return NotFound();
            }
            return View(@lock);
        }

        // POST: Locks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Policy = "NotRequestor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequest(int id, [Bind("Id,name,locked,request,alarm")] Lock @lock)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var storedClaims = await _userManager.GetClaimsAsync(user);
            var requestorClaim = storedClaims.FirstOrDefault(c => c.Type == "Requestor");
            if (requestorClaim != null)
            {
                if (requestorClaim.Value == "true")
                {
                    return NotFound();
                }
            }

            if (id != @lock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(@lock.locked == true)
                    {
                        @lock.request = false;
                        _context.Update(@lock);
                        await _context.SaveChangesAsync();
                        await removeClaimsAsync();
                    }
                    else
                    {
                        _context.Update(@lock);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LockExists(@lock.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(@lock);
        }




        // GET: Locks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @lock = await _context.Locks
                .SingleOrDefaultAsync(m => m.Id == id);
            if (@lock == null)
            {
                return NotFound();
            }

            return View(@lock);
        }

        // POST: Locks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @lock = await _context.Locks.SingleOrDefaultAsync(m => m.Id == id);
            _context.Locks.Remove(@lock);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool LockExists(int id)
        {
            return _context.Locks.Any(e => e.Id == id);
        }

        private async Task removeClaimsAsync()
        {
            var allUsers = await _userManager.GetUsersForClaimAsync(new Claim("Requestor", "true"));

            if (allUsers != null)
            {
                foreach (var user2 in allUsers)
                {
                    await _userManager.RemoveClaimAsync(user2, new Claim("Requestor", "true"));
                }
            }
            else
            {
                // Exit - no claims
            }

        }


    }
}

