using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using eTransfert.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http.Internal;
using System.Threading.Tasks;

namespace eTransfert.Controllers
{
    
    public class AdminController : Controller
    {
        private ApplicationDbContext _context;
        int total = 0;
        double solde,benef,benefTransaction;
        int moisTran, anneeTran;
        string smoisTran;
        Comptes compte { get; set; }
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
             total = _context.Users.Count();
            compte = _context.Comptes.Where(c => c.Id == "1").FirstOrDefault();
            solde = compte.SoldeUnite;
            benef = _context.RechargeCptePrincTrace.Sum(s => s.Benef);
           // GetListAdmin("ADMIN");

            moisTran = DateTime.UtcNow.Month;
            smoisTran = DateTime.UtcNow.ToString("MMMM");
            anneeTran = DateTime.UtcNow.Year;
            benefTransaction = _context.Transactions.Where(c => c.DateTransaction.Month == moisTran && c.status.ToUpper().Equals("TERMINER")).Sum(s => (s.Montant * (s.Pourcentage + 3)) / 100);


        }

        // GET: Admin
        public IActionResult Index()
        {


            GetListAdmin("ADMIN");

            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;
            ViewBag.messageVIP = eTransfert.Services.ErrorMessage.message;
            
            return View(_context.RechargeCptePrincTrace.ToList());
        }



       // GET: Admin/Details/5
        public IActionResult Details(string id)
        {
            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;

            if (id == null)
            {
                return HttpNotFound();
            }

            RechargeCptePrincTrace comptes = _context.RechargeCptePrincTrace.Single(m => m.Id == id);
            if (comptes == null)
            {
                return HttpNotFound();
            }

            return View(comptes);
        }

       // GET: Admin/CreerNouveauRechargement
        public IActionResult CreerNouveauRechargement()
        {
            return View();
        }

        // POST: Admin/CreerNouveauRechargement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreerNouveauRechargement(RechargeCptePrincTrace rcompteprincipal)
        {
            if (ModelState.IsValid)
            {
                rcompteprincipal.Id = Guid.NewGuid().ToString();
                rcompteprincipal.DateTransaction = DateTime.UtcNow;
                rcompteprincipal.Benef = rcompteprincipal.Montant * (0.03);
                rcompteprincipal.Etat = "ACTIF";
                rcompteprincipal.Montant = rcompteprincipal.Montant + rcompteprincipal.Benef;
                ApplicationUser currentUser1 = _context.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();

                rcompteprincipal.useremail = currentUser1.Email;
                _context.RechargeCptePrincTrace.Add(rcompteprincipal);
                

                //mise a jour du solde principal a chaque rechargement
                Comptes compte = new Comptes();
                compte = _context.Comptes.Where(c => c.Id == "1").FirstOrDefault();
                compte.SoldeUnite += rcompteprincipal.Montant;
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(rcompteprincipal);
        }

        // GET: Admin/Edit/5
        public IActionResult Edit(string id)
        {
            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;

            if (id == null)
            {
                return HttpNotFound();
            }

            RechargeCptePrincTrace compt = _context.RechargeCptePrincTrace.Single(m => m.Id == id);
            if (compt == null)
            {
                return HttpNotFound();
            }
            return View(compt);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RechargeCptePrincTrace comptes)
        {
            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;
            if (ModelState.IsValid)
            {
                _context.Update(comptes);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(comptes);
        }

        // GET: Admin/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Comptes comptes = _context.Comptes.Single(m => m.Id == id);
            if (comptes == null)
            {
                return HttpNotFound();
            }

            return View(comptes);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            Comptes comptes = _context.Comptes.Single(m => m.Id == id);
            _context.Comptes.Remove(comptes);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }



        public IActionResult ListeUtilisateur()
        {
            GetListAdmin("ADMIN");

            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;


            return View(_context.ApplicationUser.ToList());
        }

        public IActionResult ListeUtilisateurFicheClient()
        {
            GetListAdmin("ADMIN");
            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;


            return View(_context.ApplicationUser.ToList());
        }

        public IActionResult ListeUtilisateurRemboursement()
        {
            GetListAdmin("ADMIN");
            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;


            return View(_context.ApplicationUser.ToList());
        }

        //[ActionName("TransfererUnite")]
        public IActionResult TransfererUniteTiers(string id,string email)
        {



            GetListAdmin("ADMIN");
            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;
            if (id == null || email==null)
            {
                return HttpNotFound();
            }

            ViewBag.id = id;
            ViewBag.email = email;

            //Comptes comptes = _context.Comptes.Single(m => m.Id == id);
            //if (comptes == null)
            //{
            //    return HttpNotFound();
            //}

            return View();
        }




        public IActionResult ListeUtilisateurRole()
        {
            GetListAdmin("ADMIN");
            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;


            return View(_context.ApplicationUser.ToList());
        }


        public IActionResult AjouterRole()
        {

            GetListAdmin("ADMIN");

            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;
            return View();
       
           
        }

        public IActionResult ListeRole()
        {
            GetListAdmin("ADMIN");

            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;
            var roles = _context.Roles.ToList();
            return View(roles);



        }

        [HttpPost]
        public IActionResult ValiderAjouterRole(string RoleName)
        {
            try
            {
                _context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = RoleName.ToUpper()
                });
                _context.SaveChanges();
                ViewBag.ResultMessage = "Role created successfully !";
                return RedirectToAction("ListeRole");
            }
            catch
            {
                return View();
            }


        }



        public ActionResult SupprimerRole(string RoleName)
        {
            var thisRole = _context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            _context.Roles.Remove(thisRole);
            _context.SaveChanges();
            return RedirectToAction("ListeRole");
        }


        public ActionResult ModifierRole(string roleName)
        {
            GetListAdmin("ADMIN");

            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;
            var thisRole = _context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            return View(thisRole);
        }



        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ValiderModifierRole(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                _context.Roles.Attach(role);
                IdentityRole roles= _context.Roles.Where(r=>r.Id==role.Id).FirstOrDefault();

                roles.Name = role.Name;
               // _context.Entry(role).State = EntityState.Modified;
               // _context.Entry<IdentityRole>(roles).State = EntityState.Modified;
                _context.SaveChanges();

                return RedirectToAction("ListeRole");
            }
            catch(Exception e)
            {
                return RedirectToAction("ListeRole");
            }
        }


        //[ActionName("TransfererUnite")]
        public IActionResult CreerRole(string id, string email,double mtseuil)
        {


            




            GetListAdmin("ADMIN");

            

            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;
            if (id == null || email == null)
            {
                return HttpNotFound();
            }

            ViewBag.id = id;
            ViewBag.email = email;

            //Comptes comptes = _context.Comptes.Single(m => m.Id == id);
            //if (comptes == null)
            //{
            //    return HttpNotFound();
            //}

            return View();
        }

        public async void AddUserToRole(string userId, string roleName,double mtseuil)
        {



            //var UserManager = new UserManager<ApplicationUser>();
            // var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));

            //var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //UserManager<ApplicationUser> userbManager = (UserManager<ApplicationUser>)HttpContext.ApplicationServices.GetService(typeof(UserManager<ApplicationUser>));
            try
            {
                //  IList<ApplicationUser> liste= await userbManager.GetUsersInRoleAsync("ADMIN");

                ApplicationUser applicationUser = new ApplicationUser();
                // applicationUser=  await userbManager.FindByIdAsync(userId);
                applicationUser = _context.Users.Where(c => c.Id == userId).FirstOrDefault();

                UserManager<ApplicationUser> userbManager = (UserManager<ApplicationUser>)HttpContext.ApplicationServices.GetService(typeof(UserManager<ApplicationUser>));


                await userbManager.AddToRoleAsync(applicationUser, roleName);

                ApplicationUser currentUser = _context.Users.Where(c => c.Id == userId).FirstOrDefault();
                currentUser.SeuilUnite = mtseuil;

                //var result =await userbManager.UpdateAsync(currentUser);
                
                // await _signInManager.RefreshSignInAsync(applicationUser);
                // _context.SaveChanges();
            }
            catch (System.Exception e)
            {
                throw;
            }



        }



        public  async void GetListAdmin(string roleName)
        {
            //var UserManager = new UserManager<ApplicationUser>();
            // var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));

            //var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            UserManager<ApplicationUser> userbManager = (UserManager<ApplicationUser>)HttpContext.ApplicationServices.GetService(typeof(UserManager<ApplicationUser>));
           
            try
            {
                var liste = await  userbManager.GetUsersInRoleAsync(roleName);
               
                ViewBag.lstadmin = liste.Count;

               // return c;
               
            }
            catch (System.Exception e)
            {
                ViewBag.lstadmin = 0;
            }

           

        }



        public IActionResult ValiderRole(string idUtilisateur, string roleName,double mtseuil)
        {

            ApplicationUser currentUser = _context.Users.Where(c => c.Id == idUtilisateur).FirstOrDefault();
            currentUser.SeuilUnite = mtseuil;
            UserManager<ApplicationUser> userbManager = (UserManager<ApplicationUser>)HttpContext.ApplicationServices.GetService(typeof(UserManager<ApplicationUser>));

          //  ApplicationUser user = _context.Users.Where(u => u.Id.Equals(idUtilisateur, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            //var account = new AdminController();
            userbManager.AddToRoleAsync(currentUser, roleName);


            // AddUserToRole(idUtilisateur, roleName,mtseuil);

            //currentUser.SeuilUnite = mtseuil;
            return RedirectToAction("ListeUtilisateurRole");
            
        }


        public IActionResult _ViewBenefice()
        {


            ViewBag.benefice = benef;
            return PartialView();
        }

        public IActionResult _ViewBeneficeTransaction()
        {


            ViewBag.beneficeTransaction = benefTransaction;
                           
            ViewBag.moisTran=smoisTran +" "+anneeTran;
            
            return PartialView();
        }

        public IActionResult _ViewSoldePrincipal()
        {


            ViewBag.solde = solde;
            return PartialView();
        }

        public IActionResult _ViewListeUtilisateur()
        {


            ViewBag.total = total;
            return PartialView();
        }

        public IActionResult _ViewListeAdmin()
        {


            GetListAdmin("ADMIN");
            return PartialView();
        }


        public IActionResult ListeUtilisateurPaiement()
        {
            GetListAdmin("ADMIN");

            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;
            return View(_context.ApplicationUser.ToList());
        }



        public IActionResult PaiementTiers(string id)
        {



            GetListAdmin("ADMIN");
            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;
            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.id = id;
            //ViewBag.email = email;

            //Comptes comptes = _context.Comptes.Single(m => m.Id == id);
            //if (comptes == null)
            //{
            //    return HttpNotFound();
            //}

            return View();
        }


        public IActionResult ListeCustomTransaction(string dates)
        {
            GetListAdmin("ADMIN");

            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;

            List<Transactions> lstTrans;
            List<CustomTransaction> lstCustom = new List<CustomTransaction>();

            int mois = DateTime.UtcNow.Month;
           // ViewBag.mois = DateTime.UtcNow.ToString("MMMM");
            



            if (dates != null)
            {
                string[] rec = dates.Split('-');
                string a = rec[0].ToString();
                string b = rec[1].ToString();
                DateTime dta = Convert.ToDateTime(a);
                DateTime dtb = Convert.ToDateTime(b);
                 lstTrans = _context.Transactions.Where(c => c.DateTransaction>= dta && c.DateTransaction<=dtb  ).OrderByDescending(c => c.DateTransaction).ToList();
                ViewBag.annee = " du "+dta.ToShortDateString()+" au "+dtb.ToShortDateString();
                ViewBag.mois = "";
            }
            else
            {
                 lstTrans = _context.Transactions.Where(c => c.DateTransaction.Month == mois).OrderByDescending(c => c.DateTransaction).ToList();
                ViewBag.annee = DateTime.UtcNow.Year;
                ViewBag.mois =" du mois de "+ DateTime.UtcNow.ToString("MMMM");
            }

            
            foreach (var item in lstTrans)
            {
                CustomTransaction cstrans = new CustomTransaction();
                ApplicationUser currentUser = _context.Users.Where(c => c.Id == item.Utilisateur).FirstOrDefault();


                cstrans.CompteUnite = currentUser.CompteUnite;
                cstrans.SeuilUnite = currentUser.SeuilUnite;
                cstrans.Email = currentUser.Email;

                cstrans.DateTransaction = item.DateTransaction;
                cstrans.Id = item.Id;
                cstrans.Montant = item.Montant;
                cstrans.Numero = item.Numero;
                cstrans.Pourcentage = item.Pourcentage;
                cstrans.status = item.status;
                cstrans.Total = item.Total;
                cstrans.Benefice = (item.Montant * (item.Pourcentage + 3))/100;
                cstrans.TypeTransaction = item.TypeTransaction;
                cstrans.TypeTransfert = item.TypeTransfert;

                lstCustom.Add(cstrans);

            }

            ViewBag.liste = lstCustom;


            moisTran = DateTime.UtcNow.Month;
            smoisTran = DateTime.UtcNow.ToString("MMMM");
            anneeTran = DateTime.UtcNow.Year;
            benefTransaction = _context.Transactions.Where(c => c.DateTransaction.Month == moisTran && c.status.ToUpper().Equals("TERMINER")).Sum(s => (s.Montant * (s.Pourcentage + 3)) / 100);


            return View();
        }


        public IActionResult ListeCustomTransactionFicheClient(string id,string email,string dates)
        {

           // GetListAdmin("ADMIN");
            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;

            if (id == null || email==null )
            {
                return HttpNotFound();
            }


            int mois = DateTime.UtcNow.Month;
            ViewBag.mois = DateTime.UtcNow.ToString("MMMM");
            ViewBag.annee = DateTime.UtcNow.Year;


            List<CustomTransaction> lstCustom = new List<CustomTransaction>();


           // var lstTrans = _context.Transactions.Where(t=>t.idUtilisateur.Equals(id)).OrderByDescending(c => c.DateTransaction).ToList();
            var lstTrans = _context.Transactions.Where(t=>t.Utilisateur.Equals(id) && t.DateTransaction.Month==mois).OrderByDescending(c => c.DateTransaction).ToList();

            foreach (var item in lstTrans)
            {
                CustomTransaction cstrans = new CustomTransaction();
                ApplicationUser currentUser = _context.Users.Where(c => c.Id == item.Utilisateur).FirstOrDefault();


                cstrans.CompteUnite = currentUser.CompteUnite;
                cstrans.SeuilUnite = currentUser.SeuilUnite;
                cstrans.Email = currentUser.Email;

                cstrans.DateTransaction = item.DateTransaction;
                
                cstrans.Id = item.Id;
                cstrans.Montant = item.Montant;
                cstrans.Numero = item.Numero;
                cstrans.Pourcentage = item.Pourcentage;
                cstrans.status = item.status;
                cstrans.Total = item.Total;
                cstrans.TypeTransaction = item.TypeTransaction;
                cstrans.TypeTransfert = item.TypeTransfert;

                lstCustom.Add(cstrans);

            }

            ViewBag.email = email;
            ViewBag.listeFicheClient = lstCustom;
            return View();
        }


        public IActionResult ListeCustomTransactionRemboursement(string id, string email, string dates)
        {

            // GetListAdmin("ADMIN");
            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;

            if (id == null || email == null)
            {
                return HttpNotFound();
            }


            int mois = DateTime.UtcNow.Month;
            ViewBag.mois = DateTime.UtcNow.ToString("MMMM");
            ViewBag.annee = DateTime.UtcNow.Year;


            List<CustomTransaction> lstCustom = new List<CustomTransaction>();


            // var lstTrans = _context.Transactions.Where(t=>t.idUtilisateur.Equals(id)).OrderByDescending(c => c.DateTransaction).ToList();
            var lstTrans = _context.Transactions.Where(t => t.Utilisateur.Equals(id) && t.DateTransaction.Month == mois).OrderByDescending(c => c.DateTransaction).ToList();

            foreach (var item in lstTrans)
            {
                CustomTransaction cstrans = new CustomTransaction();
                ApplicationUser currentUser = _context.Users.Where(c => c.Id == item.Utilisateur).FirstOrDefault();


                cstrans.CompteUnite = currentUser.CompteUnite;
                cstrans.SeuilUnite = currentUser.SeuilUnite;
                cstrans.Email = currentUser.Email;

                cstrans.DateTransaction = item.DateTransaction;
                cstrans.Id = item.Utilisateur;
                cstrans.Idtrans = item.Id;
                cstrans.Montant = item.Montant;
                cstrans.Numero = item.Numero;
                cstrans.Pourcentage = item.Pourcentage;
                cstrans.status = item.status;
                cstrans.Total = item.Total;
                cstrans.TypeTransaction = item.TypeTransaction;
                cstrans.TypeTransfert = item.TypeTransfert;

                lstCustom.Add(cstrans);

            }

            ViewBag.email = email;
            ViewBag.listeRemboursement = lstCustom;
            return View();
        }


        public IActionResult Rembourser(string iduser,string idtrans)
        {

            // GetListAdmin("ADMIN");
            ViewBag.total = total;
            ViewBag.solde = solde;
            ViewBag.benefice = benef;

            if (iduser == null && idtrans==null)
            {
                return HttpNotFound();
            }


            //int mois = DateTime.UtcNow.Month;
            //ViewBag.mois = DateTime.UtcNow.ToString("MMMM");
            //ViewBag.annee = DateTime.UtcNow.Year;


            //List<CustomTransaction> lstCustom = new List<CustomTransaction>();


            // var lstTrans = _context.Transactions.Where(t=>t.idUtilisateur.Equals(id)).OrderByDescending(c => c.DateTransaction).ToList();
            var transaction = _context.Transactions.Where(t => t.Id.Equals(idtrans)).FirstOrDefault();

            
                ApplicationUser currentUser = _context.Users.Where(c => c.Id == iduser).FirstOrDefault();


                currentUser.CompteUnite += transaction.Total;
            
                transaction.TypeTransaction = "REMBOURSEMENT";

                compte = _context.Comptes.Where(c => c.Id == "1").FirstOrDefault();
                compte.SoldeUnite += transaction.Montant;

                //_context.SaveChanges();


            return RedirectToAction("ListeCustomTransactionRemboursement",new { id=iduser,email=currentUser.Email });
        }


        public IActionResult CreerOffrePromo()
        {
            return View();
        }
        //public IActionResult _PromotionView()
        //{
        //    var e = _context.Promotion.Where(p => p.etat.Equals("1")).ToList();
        //    ViewBag.e = e;
        //    return PartialView(_context.Promotion.Where(p => p.etat.Equals("1")).ToList());
        //}


        // POST: Admin/CreerNouveauRechargement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreerNouvelleOffre(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(promotion.operateur) || string.IsNullOrEmpty(promotion.offre))
                {

                }
                else
                {
                    promotion.Id = Guid.NewGuid().ToString();
                    _context.Promotion.Add(promotion);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
               
                
            }
            return View(promotion);
        }





    }



}
