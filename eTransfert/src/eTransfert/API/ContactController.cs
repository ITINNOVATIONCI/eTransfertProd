using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using eTransfert.Models;
using System.Net.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Authorization;


// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace eTransfert.API
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public string currentUserId { get; set; }

        public ContactsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        [HttpGet]
        public IEnumerable<Contacts> Get()
        {
            currentUserId = HttpContext.User.GetUserId();
            //return _dbContext.Contacts;
            return _dbContext.Contacts.Where(c => c.idUtilisateur == currentUserId);
        }


        [HttpGet("{id}", Name = "GetContacts")]
        public IActionResult Get(string id)
        {
            var contact = _dbContext.Contacts.FirstOrDefault(m => m.Id == id);
            if (contact == null)
            {
                return new HttpNotFoundResult();
            }
            else
            {
                return new ObjectResult(contact);
            }
        }


        [HttpPost]
        public IActionResult Post(Contacts contact)
        {
            try
            {
                if (contact.Id == "0" || contact.Id == null)
                {
                    currentUserId = HttpContext.User.GetUserId();
                    contact.Id = Guid.NewGuid().ToString();
                    contact.idUtilisateur = currentUserId;
                    //contact.CreatedAt = DateTimeOffset.Now;
                    //contact.Deleted = false;
                    _dbContext.Contacts.Add(contact);
                    _dbContext.SaveChanges();
                    return new ObjectResult(contact);
                }
                else
                {
                    var original = _dbContext.Contacts.FirstOrDefault(m => m.Id == contact.Id);
                    original.Nom = contact.Nom;
                    original.Numero = contact.Numero;
                    //original.UpdatedAt = DateTimeOffset.Now;
                    _dbContext.SaveChanges();
                    return new ObjectResult(original);
                }

            }
            catch (Exception ex)
            {

                return new ObjectResult(null);
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var contact = _dbContext.Contacts.FirstOrDefault(m => m.Id == id);
            _dbContext.Contacts.Remove(contact);
            _dbContext.SaveChanges();
            return new HttpStatusCodeResult(200);
        }


        public virtual IActionResult Delete(Contacts entity)
        {
            return this.Delete(entity.Id);
        }

    }
}
