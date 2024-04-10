using macrix_api.Data;
using macrix_api.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace macrix_api.Controllers
{
    // disabled auth because, despite working correctly in swagger & postman, and having basic auth implemented correctly in the client, would lose auth header when receiving httpclient requests
    // due to 3xx redirections, but the requests in fiddler looked exactly the same
    [Route("api/[controller]"), /*Authorize*/]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _db;

        public UserController(DataContext db)
        {
            _db = db;
        }

        [HttpPost]
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

        [HttpPost("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, User newUserData)
        {
            var savedUser = _db.Users.FirstOrDefault(x => x.Id == id);
            if (savedUser != null)
            {
                try
                {
                    savedUser.FirstName = !String.IsNullOrEmpty(newUserData.FirstName) ? newUserData.FirstName : savedUser.FirstName;
                    savedUser.LastName = !String.IsNullOrEmpty(newUserData.LastName) ? newUserData.LastName : savedUser.LastName;
                    savedUser.StreetName = !String.IsNullOrEmpty(newUserData.StreetName) ? newUserData.StreetName : savedUser.StreetName;
                    savedUser.HouseNumber = !String.IsNullOrEmpty(newUserData.HouseNumber) ? newUserData.HouseNumber : savedUser.HouseNumber;
                    savedUser.ApartmentNumber = newUserData.ApartmentNumber != null ? newUserData.ApartmentNumber : savedUser.ApartmentNumber;
                    savedUser.PostalCode = !String.IsNullOrEmpty(newUserData.PostalCode) ? newUserData.PostalCode : savedUser.PostalCode;
                    savedUser.Town = !String.IsNullOrEmpty(newUserData.Town) ? newUserData.Town : savedUser.Town;
                    savedUser.PhoneNumber = !String.IsNullOrEmpty(newUserData.PhoneNumber) ? newUserData.PhoneNumber : savedUser.PhoneNumber;
                    savedUser.DateOfBirth = newUserData.DateOfBirth != DateTime.MinValue ? newUserData.DateOfBirth : savedUser.DateOfBirth;
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                       ex);
                }

                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status304NotModified,
                   "User with that Id not found");
            }
         


        }

        [HttpGet]
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

        [HttpGet("{id}")]
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
        [HttpDelete("{id}")]
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
