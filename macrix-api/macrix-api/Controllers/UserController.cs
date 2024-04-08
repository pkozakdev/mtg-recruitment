using macrix_api.Data;
using macrix_api.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace macrix_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _db;

        public UserController(DataContext db)
        {
            _db = db;
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<List<User>>> AddUser(User user)
        {
            try
            {
                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Error while adding a user");
            }
        }

        [HttpPost("{id}"), Authorize]
        public async Task UpdateUser(int id, User newUserData)
        {
            var savedUser = _db.Users.FirstOrDefault(x => x.Id == id);
            _db.Entry(savedUser).CurrentValues.SetValues(newUserData);
            await _db.SaveChangesAsync();
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            try
            {
                return Ok(await _db.Users.ToListAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Error while getting users");
            }
        }

        [HttpGet("{id}"), Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(x => x.Id == id);
                if (user == null)
                {
                    return BadRequest("User not found");
                }
                else
                {
                    return Ok(user);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Error while getting a user");
            }

        }
        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
                try
            {
                EF.User userToDelete = _db.Users.FirstOrDefault(x => x.Id == id);

                if (userToDelete == null)
                {
                    return NotFound($"User with Id = {id} not found");
                }

                _db.Remove(userToDelete);
                await _db.SaveChangesAsync();

                return Ok(userToDelete);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error while deleting a user");
            }
        }
    }
}
