using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using eTransfert.Models;
using System.Security.Claims;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Authorization;
using System.IO;
using System.Data;
using LumenWorks.Framework.IO.Csv;
using Microsoft.Net.Http.Headers;
using Microsoft.ApplicationInsights;
using eTransfert.Services;

namespace eTransfert.Controllers
{
    [Authorize]
    //[RequireHttps]
    public class HomeController : Controller
    {
        private TelemetryClient telemetry = new TelemetryClient();
        protected string URISignature = "http://api.sandbox.cinetpay.com/v1/?method=getSignatureByPost";
        protected string URIStatus = "http://api.sandbox.cinetpay.com/v1/?method=checkPayStatus";

        private ApplicationDbContext _dbContext;
        public string currentUserId { get; set; }
        DataTable Exdata;
        public ApplicationUser currentUser { get; set; }
        public Comptes compte { get; set; }
        public string Name { get; set; }
        string signature;
        public List<Promotion> lstPromo;
        public string message { get; set; }

        public HomeController(ApplicationDbContext dbContext, TelemetryClient Telemetry)
        {
            _dbContext = dbContext;
            telemetry = Telemetry;
            //Name = this.User.Identity.Name;
            lstPromo = _dbContext.Promotion.Where(p => p.etat.Equals("1")).ToList();
           

        }

        public IActionResult Index()
        {

            //if (HttpContext.User.IsInRole("ADMIN"))
            //{
            //    return RedirectToAction("Index", "Admin");
            //}





            ViewBag.liste=lstPromo;
            //telemetry.TrackEvent("WinGame");
            ViewBag.messageVIP=eTransfert.Services.ErrorMessage.message;
            eTransfert.Services.ErrorMessage.message = null;
            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
            ViewBag.Compte = currentUser.CompteUnite;
            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
            ViewBag.ComptePrincipal = compte.SoldeUnite;
            return View();
        }

        public IActionResult Contact()
        {
            //telemetry.TrackEvent("WinGame");
            ViewBag.Message = "Your contact page.";
            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
            ViewBag.Compte = currentUser.CompteUnite;
            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
            ViewBag.ComptePrincipal = compte.SoldeUnite;
            return View();
        }

        public IActionResult Historique()
        {
            //telemetry.TrackEvent("WinGame");
            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
            ViewBag.Compte = currentUser.CompteUnite;
            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
            ViewBag.ComptePrincipal = compte.SoldeUnite;
            return View();
        }

        public IActionResult CompteRecharge()
        {
            //telemetry.TrackEvent("WinGame");
            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
            ViewBag.Compte = currentUser.CompteUnite;
            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
            ViewBag.ComptePrincipal = compte.SoldeUnite;
            return View();
        }

        public IActionResult TransfertExcel()
        {
            //telemetry.TrackEvent("WinGame");
            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
            ViewBag.Compte = currentUser.CompteUnite;
            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
            ViewBag.ComptePrincipal = compte.SoldeUnite;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  ActionResult TransfertExcel(IFormFile upload, ExcelData result)
        {
            if (ModelState.IsValid)
            {
                //telemetry.TrackEvent("WinGame");
                if (upload != null && upload.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(upload.ContentDisposition).FileName.Trim('"');

                    if (fileName.EndsWith(".csv"))
                    {
                        Stream stream = upload.OpenReadStream();
                        ExcelData csvTable = new ExcelData();
                        csvTable.Exdata = new DataTable();
                        using (CsvReader csvReader =
                            new CsvReader(new StreamReader(stream), true))
                        {
                            csvTable.Exdata.Load(csvReader);
                        }

                        List<Transactions> lstTrans = new List<Transactions>();

                        foreach (DataRow row in csvTable.Exdata.Rows)
                        {
                            string data = row[0].ToString();
                            string[] li = data.Split(';');
                            string num = li[0];
                            string montant = li[1].ToString();

                            Transactions trans = new Transactions();
                            trans.Numero = num;
                            trans.Montant = Convert.ToDouble(montant);

                            lstTrans.Add(trans);

                        }

                        double Total = lstTrans.Sum(c => c.Montant);

                        currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();

                        if (currentUser.CompteUnite > 0 && currentUser.CompteUnite >= Total)
                        {

                            foreach (Transactions item in lstTrans)
                            {
                                Transactions trans = item;
                                trans.Pourcentage = 0;
                                trans.Total = trans.Montant;
                                currentUserId = HttpContext.User.GetUserId();
                                trans.Id = Guid.NewGuid().ToString();
                                trans.Date = DateTime.UtcNow.Date;
                                trans.DateTransaction = DateTime.UtcNow;
                                trans.Utilisateur = currentUserId;
                                trans.TypeTransaction = "TRANSFERT";
                                trans.TypeTransfert = "COMPTE";
                                trans.Etat = "ACTIF";

                                //currentUser.CompteUnite -= trans.Total;
                                PaiementCOMPTE(trans, currentUser);

                                trans.status = "Terminer";
                                //_dbContext.Transactions.Add(trans);
                                _dbContext.SaveChanges();

                            }

                        }
                        else if (HttpContext.User.IsInRole("VIP"))
                        {
                            double seuil = -(currentUser.CompteUnite - Total);
                            if (seuil < currentUser.SeuilUnite)
                            {
                                foreach (Transactions item in lstTrans)
                                {
                                    Transactions trans = item;
                                    trans.Pourcentage = 5;
                                    trans.Total = trans.Montant + (trans.Montant * trans.Pourcentage * 0.01);
                                    currentUserId = HttpContext.User.GetUserId();
                                    trans.Id = Guid.NewGuid().ToString();
                                    trans.Date = DateTime.UtcNow.Date;
                                    trans.DateTransaction = DateTime.UtcNow;
                                    trans.Utilisateur = currentUserId;
                                    trans.TypeTransaction = "TRANSFERT";
                                    trans.Etat = "ACTIF";

                                    if (currentUser.CompteUnite > 0 && currentUser.CompteUnite >= trans.Total)
                                    {
                                        trans.Pourcentage = 0;
                                        trans.Total = trans.Montant + (trans.Montant * trans.Pourcentage * 0.01);
                                        trans.TypeTransfert = "COMPTE";

                                        //currentUser.CompteUnite -= trans.Total;
                                        PaiementCOMPTE(trans, currentUser);

                                        trans.status = "Terminer";
                                        //_dbContext.Transactions.Add(trans);
                                        _dbContext.SaveChanges();
                                    }
                                    else
                                    {
                                        seuil = -(currentUser.CompteUnite - trans.Montant);
                                        if (seuil < currentUser.SeuilUnite)
                                        {

                                            trans.TypeTransfert = "VIP";

                                            //currentUser.CompteUnite -= trans.Total;
                                            PaiementVIP(trans, currentUser);

                                            trans.status = "Terminer";
                                            //_dbContext.Transactions.Add(trans);
                                            _dbContext.SaveChanges();
                                        }
                                        else
                                        {
                                            //Mettre un message pour dire que le seuil est atteind
                                            ModelState.AddModelError("File", "le seuil est atteind");
                                            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
                                            ViewBag.Compte = currentUser.CompteUnite;
                                            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
                                            ViewBag.ComptePrincipal = compte.SoldeUnite;
                                            return View();
                                        }

                                    }

                                }
                            }
                            else
                            {
                                //Mettre un message pour dire que le seuil est atteind
                                ViewBag.message = ErrorMessage.maxFunction;
                                ModelState.AddModelError("File", "le seuil est atteind");
                                currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
                                ViewBag.Compte = currentUser.CompteUnite;
                                compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
                                ViewBag.ComptePrincipal = compte.SoldeUnite;
                                return View();
                            }

                        }
                        else if (HttpContext.User.IsInRole("SUPERVIP") || HttpContext.User.IsInRole("ADMIN"))
                        {
                            double seuil = -(currentUser.CompteUnite - Total);
                            if (seuil < currentUser.SeuilUnite)
                            {
                                foreach (Transactions item in lstTrans)
                                {
                                    Transactions trans = item;
                                    trans.Pourcentage = 0;
                                    trans.Total = trans.Montant + (trans.Montant * trans.Pourcentage * 0.01);
                                    currentUserId = HttpContext.User.GetUserId();
                                    trans.Id = Guid.NewGuid().ToString();
                                    trans.Date = DateTime.UtcNow.Date;
                                    trans.DateTransaction = DateTime.UtcNow;
                                    trans.Utilisateur = currentUserId;
                                    trans.TypeTransaction = "TRANSFERT";
                                    trans.Etat = "ACTIF";

                                    if (currentUser.CompteUnite > 0 && currentUser.CompteUnite >= trans.Total)
                                    {
                                        trans.Pourcentage = 0;
                                        trans.Total = trans.Montant + (trans.Montant * trans.Pourcentage * 0.01);
                                        trans.TypeTransfert = "COMPTE";

                                        //currentUser.CompteUnite -= trans.Total;
                                        PaiementCOMPTE(trans, currentUser);

                                        trans.status = "Terminer";
                                        //_dbContext.Transactions.Add(trans);
                                        _dbContext.SaveChanges();
                                    }
                                    else
                                    {
                                        seuil = -(currentUser.CompteUnite - trans.Montant);
                                        if (seuil < currentUser.SeuilUnite)
                                        {

                                            trans.TypeTransfert = "VIP";

                                            //currentUser.CompteUnite -= trans.Total;
                                            PaiementVIP(trans, currentUser);

                                            trans.status = "Terminer";
                                            //_dbContext.Transactions.Add(trans);
                                            _dbContext.SaveChanges();
                                        }
                                        else
                                        {
                                            //Mettre un message pour dire que le seuil est atteind
                                            ModelState.AddModelError("File", "le seuil est atteind");
                                            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
                                            ViewBag.Compte = currentUser.CompteUnite;
                                            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
                                            ViewBag.ComptePrincipal = compte.SoldeUnite;
                                            return View();
                                        }

                                    }

                                }
                            }
                            else
                            {
                                //Mettre un message pour dire que le seuil est atteind
                                ViewBag.message = ErrorMessage.maxFunction;
                                ModelState.AddModelError("File", "le seuil est atteind");
                                currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
                                ViewBag.Compte = currentUser.CompteUnite;
                                compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
                                ViewBag.ComptePrincipal = compte.SoldeUnite;
                                return View();
                            }

                        }
                        else
                        {
                            ViewBag.message = ErrorMessage.VerifSoldeFunction;
                            ModelState.AddModelError("File", "Verifier votre solde avant de valider le transfert.");
                            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
                            ViewBag.Compte = currentUser.CompteUnite;
                            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
                            ViewBag.ComptePrincipal = compte.SoldeUnite;
                            return View();
                        }

                        currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
                        ViewBag.Compte = currentUser.CompteUnite;
                        compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
                        ViewBag.ComptePrincipal = compte.SoldeUnite;
                        return View(csvTable);
                    }
                    else
                    {
                        ViewBag.message = ErrorMessage.WrongFileFunction;
                        ModelState.AddModelError("File", "Le format de ce fichier n'est pas supporter seulement un fichier de type csv.");
                        currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
                        ViewBag.Compte = currentUser.CompteUnite;
                        compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
                        ViewBag.ComptePrincipal = compte.SoldeUnite;
                        return View();
                    }
                }
                else
                {

                    ModelState.AddModelError("File", "SVP ajoutez votre fichier avant de continuer");
                }
            }
            ViewBag.message = ErrorMessage.NoFileFunction;
            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
            ViewBag.Compte = currentUser.CompteUnite;
            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
            ViewBag.ComptePrincipal = compte.SoldeUnite;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Notification(NotifParametre notif)
        {
            //HelperSMS.SendSMS(Config.adminNumber, "Notif");
            ViewBag.Message = "Notification";
            telemetry.TrackEvent("Notification");

            using (WebClient client = new WebClient())
            {
                NameValueCollection data = new NameValueCollection();
                data.Add("apikey", Config.apikey);
                data.Add("cpm_site_id", notif.cpm_site_id);
                data.Add("cpm_trans_id", notif.cpm_trans_id);

                byte[] responsebytes = client.UploadValues(Config.URIStatus, "POST", data);
                string res = Encoding.UTF8.GetString(responsebytes);
                JsonResponse ci = JsonConvert.DeserializeObject<JsonResponse>(res);

                //debut test si ok cpm_result==00

                if (ci.transaction.cpm_result == "00")
                {
                    try
                    {
                        Transactions trans = _dbContext.Transactions.Where(c => c.Id == ci.transaction.cpm_trans_id && c.Etat == "ACTIF" && c.status != "Terminer").FirstOrDefault();

                        if (trans != null)
                        {
                            if (trans.TypeTransaction == "TRANSFERT")
                            {
                                telemetry.TrackEvent("Notification:TRANSFERT");
                                trans.status = "Payé, En Attente de transfert.";
                                trans.statuscinetpay = ci.transaction.cpm_error_message;
                                trans.buyer_name = ci.transaction.buyer_name;
                                _dbContext.SaveChanges();

                                try
                                {
                                    string num = trans.Numero;
                                    string result = Recharge(num, trans.Montant.ToString(), trans);

                                    if (result == "REUSSI")
                                    {
                                        trans.log = "REUSSI";
                                        trans.status = "Terminer";

                                        _dbContext.SaveChanges();

                                    }
                                    else
                                    {
                                        _dbContext.SaveChanges();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    trans.log = ex.Message;
                                    _dbContext.SaveChanges();

                                }
                            }
                            else
                            {
                                try
                                {
                                    telemetry.TrackEvent("Notification:RECHARGEMENT");
                                    trans.status = "Terminer";
                                    trans.statuscinetpay = ci.transaction.cpm_error_message;
                                    trans.buyer_name = ci.transaction.buyer_name;

                                    currentUser = _dbContext.Users.Where(c => c.Id == trans.Utilisateur).FirstOrDefault();

                                    currentUser.CompteUnite += trans.Total;

                                    _dbContext.SaveChanges();

                                }
                                catch (Exception ex)
                                {
                                    trans.log = ex.Message;
                                    _dbContext.SaveChanges();

                                }

                            }
                        }
                        else
                        {
                            HelperSMS.SendSMS(Config.adminNumber, "Trans null");
                            
                        }
                    }
                    catch (Exception)
                    {

                        HelperSMS.SendSMS(Config.adminNumber, "Trans null");
                    }

                }
                else
                {
                    //log
                }

                //HelperSMS.SendSMS(Config.adminNumber, ci.transaction.buyer_name + " " + ci.transaction.cel_phone_num + " " + ci.transaction.cpm_custom + " " + ci.transaction.cpm_error_message + " " + ci.transaction.cpm_payid + " " + ci.transaction.cpm_result + " " + ci.transaction.cpm_trans_status);

                ViewBag.Notif = res;
            }


            return null;
        }

        public string Recharge(string No, string Montant, Transactions trans)
        {

            try
            {
                No = No.Trim();

                string respStr = "";
                
                Uri uri = new Uri(String.Format("https://www.symtel.biz/fr/index.php?mod=cgibin&page=5&user=RS3680&code=6d776c682122de4f9be5bd0ba13cfb70&montant={0}&phone={1}", Montant, 225 + No));
                HttpWebRequest requestFile = (HttpWebRequest)WebRequest.Create(uri);
                requestFile.ContentType = "application/html";

                // Attaching the Certificate To the request

                System.Net.ServicePointManager.CertificatePolicy =
                                       new TrustAllCertificatePolicy();

                HttpWebResponse webResp = requestFile.GetResponse() as HttpWebResponse;
                if (requestFile.HaveResponse)
                {
                    if (webResp.StatusCode == HttpStatusCode.OK || webResp.StatusCode == HttpStatusCode.Accepted)
                    {
                        StreamReader respReader = new StreamReader(webResp.GetResponseStream());
                        respStr = respReader.ReadToEnd();
                        trans.htmlMessage = respStr;

                        try
                        {

                            string[] str = respStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                            var myString = str[7];

                            myString = myString.Replace("</body>", "");

                            string[] data = myString.Split(new string[] { "<br/>" }, StringSplitOptions.RemoveEmptyEntries);
                            trans.statustransfert = data[0].Replace("status=", "");
                            trans.numerotransaction = data[1].Replace("numero transaction=", "");
                            trans.message = data[2].Replace("message=", "");
                            trans.reponse = data[3].Replace("reponse=", "");

                            if (trans.statustransfert == "OK")
                            {
                                compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
                                compte.SoldeUnite -= trans.Montant;
                                telemetry.TrackEvent("TRANSFERT OK");
                                return "REUSSI";
                            }
                            else if (trans.statustransfert == "NO")
                            {
                                telemetry.TrackEvent("TRANSFERT ECHEC");
                                trans.log = "ECHEC";
                                return "ECHEC";
                            }
                            else
                            {
                                telemetry.TrackEvent("TRANSFERT ECHEC");
                                trans.log = "ECHEC";
                                return "ECHEC";
                            }



                        }
                        catch (Exception ex)
                        {
                            trans.htmlMessage = ex.Message;
                            telemetry.TrackEvent("TRANSFERT ECHEC");
                            trans.log = "ECHEC";
                            return "ECHEC";

                        }




                        //model.SaveChanges();
                    }
                    else
                    {
                        telemetry.TrackEvent("TRANSFERT ECHEC");
                        trans.log = "ECHEC";
                        return "ECHEC";
                    }
                }

                return "ECHEC";

            }
            catch (Exception ex)
            {
                trans.log = "Le seuveur symtel est tomber";
                telemetry.TrackEvent("TRANSFERT ECHEC: Le seuveur symtel est tomber");
                return "ECHEC";
            }
        }

        public ActionResult Paiement(Transactions trans)
        {
            //telemetry.TrackEvent("WinGame");
            //_dbContext = new ApplicationDbContext();
            ViewBag.messageVIP = "";
            currentUserId = HttpContext.User.GetUserId();
            trans.Id = Guid.NewGuid().ToString();
            trans.Date = DateTime.UtcNow.Date;
            trans.DateTransaction = DateTime.UtcNow;
            trans.Utilisateur = currentUserId;
            trans.TypeTransaction = "TRANSFERT";
            //trans.TypeTransfert = "RAPIDE";
            trans.Etat = "ACTIF";

            if (trans.TypeTransfert == "RAPIDE")
            {
                string num = trans.Numero = trans.Numero.Replace("-", "");
                trans.Pourcentage = 7;
                trans.Total = trans.Montant + (trans.Montant * trans.Pourcentage * 0.01);
                //trans.Pourcentage = 0;
                //trans.Total = trans.Montant;
                trans.status = "En Attente du Paiement";
                _dbContext.Transactions.Add(trans);

                try
                {
                    _dbContext.SaveChanges();
                }
                catch (Exception e)
                {

                    throw;
                }
                

                currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
                ViewBag.Compte = currentUser.CompteUnite;
                return View(PaiementRapide(trans, "Transfert de credit vers " + trans.Numero));

            }
            else if (trans.TypeTransfert == "COMPTE")
            {
                trans.Pourcentage = 0;
                trans.Total = trans.Montant;

                currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();

                if (currentUser.CompteUnite > 0 && currentUser.CompteUnite >= trans.Total)
                {

                    PaiementCOMPTE(trans, currentUser);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ErrorMessage.message = ErrorMessage.MinSoldeFunction;
                    return RedirectToAction("Index", "Home");
                }

            }
            else if (trans.TypeTransfert == "VIP")
            {

                currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();

                if (HttpContext.User.IsInRole("VIP"))
                {
                    if (currentUser.CompteUnite > 0 && currentUser.CompteUnite >= trans.Total)
                    {

                        PaiementCOMPTE(trans, currentUser);
                    }
                    else 
                    {
                        double seuil = -(currentUser.CompteUnite - trans.Montant);
                        if (seuil < currentUser.SeuilUnite)
                        {
                            trans.Pourcentage = 5;
                            trans.Total = trans.Montant + (trans.Montant * trans.Pourcentage * 0.01);

                            PaiementVIP(trans, currentUser);
                        }
                        else
                        {
                            //Mettre un message pour dire que le seuil est atteind
                            return RedirectToAction("Index", "Home");
                        }  

                    }

                    return RedirectToAction("Index", "Home");
                }
                else if (HttpContext.User.IsInRole("SUPERVIP") || HttpContext.User.IsInRole("ADMIN"))
                {
                    if (currentUser.CompteUnite > 0 && currentUser.CompteUnite >= trans.Total)
                    {

                        PaiementCOMPTE(trans, currentUser);
                    }
                    else
                    {
                        double seuil = -(currentUser.CompteUnite - trans.Montant);
                        if (seuil < currentUser.SeuilUnite)
                        {
                            trans.Pourcentage = 0;
                            trans.Total = trans.Montant + (trans.Montant * trans.Pourcentage * 0.01);

                            //ViewBag.messageVIP = "Transfert de crédit effectué avec succès";
                            //ViewBag.messageVIP = "ShowErrorPopup()";
                            // eTransfert.Services.ErrorMessage.message = eTransfert.Services.ErrorMessage.succesFunction;

                            PaiementVIP(trans, currentUser);
                        }
                        else
                        {
                            //Mettre un message pour dire que le seuil est atteind
                            ErrorMessage.message = ErrorMessage.maxFunction;
                            return RedirectToAction("Index", "Home");
                        }

                    }

                    //return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }


            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            //currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
            //ViewBag.Compte = currentUser.CompteUnite;
            //compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
            //ViewBag.ComptePrincipal = compte.SoldeUnite;
            return View();

        }

        public PaiementData PaiementRapide(Transactions trans, string Description)
        {
            string signature;
            string id = DateTime.UtcNow.ToString("yyyyMMddhhmmss");

            using (WebClient client = new WebClient())
            {

                Config.cpm_designation = Description;

                NameValueCollection data = new NameValueCollection();
                data.Add("apikey", "106612574455953b2d0e7775.94466351");
                data.Add("cpm_site_id", "883420");
                data.Add("cpm_currency", "CFA");
                data.Add("cpm_page_action", "PAYMENT");
                data.Add("cpm_payment_config", "SINGLE");
                data.Add("cpm_version", "V1");
                data.Add("cpm_language", "fr");
                data.Add("cpm_trans_date", id);
                data.Add("cpm_trans_id", trans.Id.ToString());
                data.Add("cpm_designation", Config.cpm_designation);
                data.Add("cpm_amount", trans.Total.ToString());
                data.Add("cpm_custom", HttpContext.User.Identity.Name);

                byte[] responsebytes = client.UploadValues(URISignature, "POST", data);
                signature = Encoding.UTF8.GetString(responsebytes);
                signature = JsonConvert.DeserializeObject<string>(signature);

            }

            Dictionary<string, object> postData = new Dictionary<string, object>();
            postData.Add("apikey", Config.apikey);
            postData.Add("cpm_site_id", Config.cpm_site_id);
            postData.Add("cpm_currency", Config.cpm_currency);
            postData.Add("cpm_page_action", Config.cpm_page_action);
            postData.Add("cpm_payment_config", Config.cpm_payment_config);
            postData.Add("cpm_version", Config.cpm_version);
            postData.Add("cpm_language", Config.cpm_language);
            postData.Add("cpm_trans_date", id);
            postData.Add("cpm_trans_id", trans.Id.ToString());
            postData.Add("cpm_designation", Config.cpm_designation);
            postData.Add("cpm_amount", trans.Total.ToString());
            postData.Add("cpm_custom", HttpContext.User.Identity.Name);
            postData.Add("signature", signature);

            //postData.Add("notify_url", "http://web.etransfert.net/Home/Notification");

            PaiementData pay = new PaiementData();
            pay.data = postData;

            return pay;
        }

        [Authorize(Roles = "VIP,ADMIN,SUPERVIP")]
        public void PaiementVIP(Transactions trans, ApplicationUser user)
        {

            trans.status = "En Attente de transfert.";
            _dbContext.Transactions.Add(trans);
            _dbContext.SaveChanges();

            try
            {
                string num = trans.Numero = trans.Numero.Replace("-", "");
                string result = Recharge(num, trans.Montant.ToString(), trans);

                if (result == "REUSSI")
                {
                    trans.log = "REUSSI";
                    trans.status = "Terminer";
                    currentUser.CompteUnite -= trans.Total;

                    _dbContext.SaveChanges();

                    ErrorMessage.message = ErrorMessage.succesFunction;
                }
                else
                {                    
                    _dbContext.SaveChanges();
                    ErrorMessage.message = ErrorMessage.errorFunction;
                }
            }
            catch (Exception ex)
            {
                trans.log = ex.Message;
                _dbContext.SaveChanges();
               ErrorMessage.message = ErrorMessage.errorFunction;

            }

        }

        [Authorize(Roles = "VIP,ADMIN,SUPERVIP")]
        public void PaiementVIPadmin(Transactions trans, ApplicationUser user)
        {

            trans.status = "En Attente de transfert.";
            _dbContext.Transactions.Add(trans);
            _dbContext.SaveChanges();

            try
            {
                string num = trans.Numero = trans.Numero.Replace("-", "");
                string result = Recharge(num, trans.Montant.ToString(), trans);

                if (result == "REUSSI")
                {
                    trans.log = "REUSSI";
                    trans.status = "Terminer";
                    currentUser.CompteUnite -= trans.Total;

                    _dbContext.SaveChanges();

                    ErrorMessage.message = ErrorMessage.succesFunction;


                    #region stock trace
                    //appel storeInfo
                    Trace tr = new Trace();
                    tr.Id = Guid.NewGuid().ToString();
                    tr.DateTransaction = DateTime.UtcNow;
                    tr.Montant = trans.Montant;
                    ApplicationUser currentUser1 = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
                    tr.TypeTransaction = "Depot unite" + " " + trans.TypeTransfert;
                    tr.Senderemail = currentUser1.Email;
                    tr.Receiveremail = currentUser.Email;
                    storeInfo(tr);
                    #endregion


                }
                else
                {
                    _dbContext.SaveChanges();
                    ErrorMessage.message = ErrorMessage.errorFunction;
                }
            }
            catch (Exception ex)
            {
                trans.log = ex.Message;
                _dbContext.SaveChanges();
                ErrorMessage.message = ErrorMessage.errorFunction;

            }

        }
        public void PaiementCOMPTE(Transactions trans, ApplicationUser user)
        {

            trans.status = "En Attente de transfert.";
            _dbContext.Transactions.Add(trans);
            _dbContext.SaveChanges();

            try
            {
                string num = trans.Numero = trans.Numero.Replace("-", "");
                string result = Recharge(num, trans.Montant.ToString(), trans);

                if (result == "REUSSI")
                {
                    trans.log = "REUSSI";
                    trans.status = "Terminer";
                    currentUser.CompteUnite -= trans.Total;

                    _dbContext.SaveChanges();
                    ErrorMessage.message = ErrorMessage.succesFunction;
                }
                else
                {
                    _dbContext.SaveChanges();
                    ErrorMessage.message = ErrorMessage.errorFunction;
                }
            }
            catch (Exception ex)
            {
                trans.log = ex.Message;
                _dbContext.SaveChanges();
                ErrorMessage.message = ErrorMessage.errorFunction;

            }
        }
        public void PaiementCOMPTEadmin(Transactions trans, ApplicationUser user)
        {

            trans.status = "En Attente de transfert.";
            _dbContext.Transactions.Add(trans);
            _dbContext.SaveChanges();

            try
            {
                string num = trans.Numero = trans.Numero.Replace("-", "");
                string result = Recharge(num, trans.Montant.ToString(), trans);

                if (result == "REUSSI")
                {
                    trans.log = "REUSSI";
                    trans.status = "Terminer";
                    currentUser.CompteUnite -= trans.Total;

                    _dbContext.SaveChanges();
                    ErrorMessage.message = ErrorMessage.succesFunction;


                    #region stock trace
                    //appel storeInfo
                    Trace tr = new Trace();
                    tr.Id = Guid.NewGuid().ToString();
                    tr.DateTransaction = DateTime.UtcNow;
                    tr.Montant = trans.Montant;
                    ApplicationUser currentUser1 = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
                    tr.TypeTransaction = "Depot unite" + " " + trans.TypeTransfert;
                    tr.Senderemail = currentUser1.Email;
                    tr.Receiveremail = currentUser.Email;
                    storeInfo(tr);
                    #endregion




                }
                else
                {
                    _dbContext.SaveChanges();
                    ErrorMessage.message = ErrorMessage.errorFunction;
                }
            }
            catch (Exception ex)
            {
                trans.log = ex.Message;
                _dbContext.SaveChanges();
                ErrorMessage.message = ErrorMessage.errorFunction;

            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rechargement(Transactions trans)
        {
            //telemetry.TrackEvent("WinGame");
            //_dbContext = new ApplicationDbContext();
            currentUserId = HttpContext.User.GetUserId();
            trans.Id = Guid.NewGuid().ToString();
            trans.Date = DateTime.UtcNow.Date;
            trans.DateTransaction = DateTime.UtcNow;
            trans.Utilisateur = currentUserId;
            trans.TypeTransaction = "PAIEMENT";
            trans.ModePaiement = "CINETPAY";
            trans.Etat = "ACTIF";

            trans.Pourcentage = 6;
            trans.Total = trans.Montant + (trans.Montant * trans.Pourcentage * 0.01);
            trans.status = "En Attente du Paiement";
            //trans.status = "Payer";
            _dbContext.Transactions.Add(trans);

            //currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();

            //currentUser.CompteUnite += trans.Total;

            _dbContext.SaveChanges();

            return View(PaiementRapide(trans, "Rechargement"));

            //return RedirectToAction("CompteRecharge", "Home");

        }

        [AllowAnonymous]
        public ActionResult PaiementMobile(string Id, string UserId)
        {
            //_dbContext = new ApplicationDbContext();
            currentUserId = UserId;
            IEnumerable<Transactions> lst = _dbContext.Transactions.Where(c => c.Utilisateur == currentUserId && c.Id == Id && c.Etat == "ACTIF" && c.status == "En Attente du Paiement");

            if (lst != null && lst.Count() != 0)
            {
                Transactions trans = lst.FirstOrDefault();
                string signature;
                string id = trans.DateTransaction.ToString("yyyyMMddhhmmss");

                using (WebClient client = new WebClient())
                {

                    Config.cpm_designation = "Transfert de credit vers " + trans.Numero;

                    NameValueCollection data = new NameValueCollection();
                    data.Add("apikey", "106612574455953b2d0e7775.94466351");
                    data.Add("cpm_site_id", "883420");
                    data.Add("cpm_currency", "CFA");
                    data.Add("cpm_page_action", "PAYMENT");
                    data.Add("cpm_payment_config", "SINGLE");
                    data.Add("cpm_version", "V1");
                    data.Add("cpm_language", "fr");
                    data.Add("cpm_trans_date", id);
                    data.Add("cpm_trans_id", trans.Id.ToString());
                    data.Add("cpm_designation", Config.cpm_designation);
                    data.Add("cpm_amount", trans.Total.ToString());
                    data.Add("cpm_custom", HttpContext.User.Identity.Name);

                    byte[] responsebytes = client.UploadValues(URISignature, "POST", data);
                    signature = Encoding.UTF8.GetString(responsebytes);
                    signature = JsonConvert.DeserializeObject<string>(signature);

                }

                Dictionary<string, object> postData = new Dictionary<string, object>();
                postData.Add("apikey", Config.apikey);
                postData.Add("cpm_site_id", Config.cpm_site_id);
                postData.Add("cpm_currency", Config.cpm_currency);
                postData.Add("cpm_page_action", Config.cpm_page_action);
                postData.Add("cpm_payment_config", Config.cpm_payment_config);
                postData.Add("cpm_version", Config.cpm_version);
                postData.Add("cpm_language", Config.cpm_language);
                postData.Add("cpm_trans_date", id);
                postData.Add("cpm_trans_id", trans.Id.ToString());
                postData.Add("cpm_designation", Config.cpm_designation);
                postData.Add("cpm_amount", trans.Total.ToString());
                postData.Add("cpm_custom", HttpContext.User.Identity.Name);
                postData.Add("signature", signature);

                postData.Add("notify_url", "http://etransfert.azurewebsites.net/Home/Notification");
                postData.Add("return_url", "http://etransfert.azurewebsites.net/");
                //postData.Add("cancel_url", "http://localhost:62378/");

                PaiementData pay = new PaiementData();
                pay.data = postData;

                return View(pay);
            }

            return View(null);

        }

        public IActionResult Feedback()
        {
            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
            ViewBag.Compte = currentUser.CompteUnite;
            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
            ViewBag.ComptePrincipal = compte.SoldeUnite;
            return View();
        }

        public IActionResult Help()
        {
            //telemetry.TrackEvent("WinGame");
            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
            ViewBag.Compte = currentUser.CompteUnite;
            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
            ViewBag.ComptePrincipal = compte.SoldeUnite;
            return View();
        }

        public IActionResult ContactUs()
        {
            //telemetry.TrackEvent("WinGame");
            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
            ViewBag.Compte = currentUser.CompteUnite;
            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
            ViewBag.ComptePrincipal = compte.SoldeUnite;
            return View();
        }


        


        public IActionResult Error()
        {
            try
            {
                currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
                ViewBag.Compte = currentUser.CompteUnite;
                compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
                ViewBag.ComptePrincipal = compte.SoldeUnite;
            }
            catch (Exception)
            {

            }

            return View("/Views/Shared/Error.cshtml");
        }








        public ActionResult PaiementUniteTiers(Transactions trans)
        {
            
            ViewBag.messageVIP = "";
            //currentUserId = trans.idUtilisateur;
            trans.Id = Guid.NewGuid().ToString();
            trans.Date = DateTime.UtcNow.Date;
            trans.DateTransaction = DateTime.UtcNow;
           // trans.idUtilisateur = currentUserId;
            trans.TypeTransaction = "TRANSFERT";
            //trans.TypeTransfert = "VIP";
            trans.Etat = "ACTIF";
            trans.Total = trans.Montant;

            //currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();

            //ApplicationUser currentUser1 = _dbContext.Users.Where(c => c.Id == trans.idUtilisateur).FirstOrDefault();

            //var rep = currentUser1.Roles.Select(x => x.UserId == trans.idUtilisateur).ToList();

            //var role = (from r in _dbContext.Roles where r.Name.Contains("Admin") select r).FirstOrDefault();






            if (trans.TypeTransfert == "RAPIDE")
            {
                string num = trans.Numero = trans.Numero.Replace("-", "");
                trans.Pourcentage = 7;
                trans.Total = trans.Montant + (trans.Montant * trans.Pourcentage * 0.01);
                //trans.Pourcentage = 0;
                //trans.Total = trans.Montant;
                trans.status = "En Attente du Transfert";
                _dbContext.Transactions.Add(trans);
                _dbContext.SaveChanges();

                currentUser = _dbContext.Users.Where(c => c.Id == trans.Utilisateur).FirstOrDefault();
                ViewBag.Compte = currentUser.CompteUnite;


                try
                {
                    //string num = trans.Numero = trans.Numero.Replace("-", "");
                    string result = Recharge(num, trans.Montant.ToString(), trans);

                    if (result == "REUSSI")
                    {
                        trans.log = "REUSSI";
                        trans.status = "Terminer";
                       // currentUser.CompteUnite -= trans.Total;

                        _dbContext.SaveChanges();
                        ErrorMessage.message = ErrorMessage.succesFunction;

                        #region stock trace
                        //appel storeInfo
                        Trace tr = new Trace();
                        tr.Id = Guid.NewGuid().ToString();                        
                        tr.DateTransaction = DateTime.UtcNow;
                        tr.Montant = trans.Montant;
                        ApplicationUser currentUser1 = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
                        tr.TypeTransaction = "Depot unite"+" "+trans.TypeTransfert;
                        tr.Senderemail = currentUser1.Email;
                        tr.Receiveremail= currentUser.Email;
                        storeInfo(tr);
                        #endregion




                    }
                    else
                    {
                        _dbContext.SaveChanges();
                        ErrorMessage.message = ErrorMessage.errorFunction;
                    }
                }
                catch (Exception ex)
                {
                    trans.log = ex.Message;
                    _dbContext.SaveChanges();
                    ErrorMessage.message = ErrorMessage.errorFunction;

                }



                // return View(PaiementRapide(trans, "Transfert de credit vers " + trans.Numero));

            }
            if (trans.TypeTransfert == "COMPTE")
            {
                trans.Pourcentage = 0;
                trans.Total = trans.Montant;



                currentUser = _dbContext.Users.Where(c => c.Id == trans.Utilisateur).FirstOrDefault();

                if (currentUser.CompteUnite > 0 && currentUser.CompteUnite >= trans.Total)
                {

                    PaiementCOMPTEadmin(trans, currentUser);

                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ErrorMessage.message = ErrorMessage.MinSoldeFunction;
                    return RedirectToAction("Index", "Admin");
                }

            }
            else if (trans.TypeTransfert == "VIP")
            {

                currentUser = _dbContext.Users.Where(c => c.Id == trans.Utilisateur).FirstOrDefault();

                // if (HttpContext.User.IsInRole("VIP"))
                // {
                if (currentUser.CompteUnite > 0 && currentUser.CompteUnite >= trans.Total)
                {

                    PaiementCOMPTEadmin(trans, currentUser);
                }
                else
                {
                    double seuil = -(currentUser.CompteUnite - trans.Montant);
                    if (seuil < currentUser.SeuilUnite)
                    {
                        trans.Pourcentage = 5;
                        trans.Total = trans.Montant + (trans.Montant * trans.Pourcentage * 0.01);

                        PaiementVIPadmin(trans, currentUser);
                    }
                    else
                    {
                        //Mettre un message pour dire que le seuil est atteind
                        return RedirectToAction("Index", "Admin");
                    }

                }

                return RedirectToAction("Index", "Admin");
                // }
            }
            else if (trans.TypeTransfert =="SUPERVIP" || trans.TypeTransfert == "ADMIN")
            {
                currentUser = _dbContext.Users.Where(c => c.Id == trans.Utilisateur).FirstOrDefault();

                if (currentUser.CompteUnite > 0 && currentUser.CompteUnite >= trans.Total)
                {

                    PaiementCOMPTEadmin(trans, currentUser);
                }
                else
                {
                    double seuil = -(currentUser.CompteUnite - trans.Montant);
                    if (seuil < currentUser.SeuilUnite)
                    {
                        trans.Pourcentage = 0;
                        trans.Total = trans.Montant + (trans.Montant * trans.Pourcentage * 0.01);

                        //ViewBag.messageVIP = "Transfert de crédit effectué avec succès";
                        //ViewBag.messageVIP = "ShowErrorPopup()";
                        // eTransfert.Services.ErrorMessage.message = eTransfert.Services.ErrorMessage.succesFunction;
                        PaiementVIPadmin(trans, currentUser);
                    }
                    else
                    {
                        //Mettre un message pour dire que le seuil est atteind
                        ErrorMessage.message = ErrorMessage.maxFunction;
                        return RedirectToAction("Index", "Admin");
                    }

                }

                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }


            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            currentUser = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
            ViewBag.Compte = currentUser.CompteUnite;
            compte = _dbContext.Comptes.Where(c => c.Id == "1").FirstOrDefault();
            ViewBag.ComptePrincipal = compte.SoldeUnite;
            return View();

        }

        public ActionResult PaiementArgentTiers(double Montant,string idUtilisateur)
        {

            ViewBag.messageVIP = "";
            Trace tr = new Trace();
            //currentUserId = trans.idUtilisateur;
            tr.Id = Guid.NewGuid().ToString();
            tr.Montant = Montant;
            tr.DateTransaction = DateTime.UtcNow;
           
            tr.TypeTransaction = "PAIEMENT";
            

            currentUser = _dbContext.Users.Where(c => c.Id == idUtilisateur).FirstOrDefault();
            tr.Receiveremail = currentUser.Email;
            
            ApplicationUser currentUser1 = _dbContext.Users.Where(c => c.Id == HttpContext.User.GetUserId()).FirstOrDefault();
            tr.Senderemail = currentUser1.Email;

            Transactions trans = new Transactions();
            trans.Id = Guid.NewGuid().ToString();
            trans.Date = DateTime.UtcNow.Date;
            trans.DateTransaction = DateTime.UtcNow;
            trans.Utilisateur = currentUserId;
            trans.TypeTransaction = "PAIEMENT ARGENT";
            trans.Montant = Montant;
            trans.Total = Montant;
            trans.Utilisateur = idUtilisateur;
            trans.Etat = "ACTIF";



            _dbContext.Add(tr);
            _dbContext.Add(trans);

            

            currentUser.CompteUnite += Montant;

            _dbContext.SaveChanges();

            return RedirectToAction("ListeUtilisateurPaiement");

        }


        private void storeInfo(Trace tr)
        {
            
            //tr.Senderemail = HttpContext.User.GetUserId();
            //tr.Id = Guid.NewGuid().ToString();
            //tr.Receiveremail = HttpContext.User.GetUserId();
            //tr.DateTransaction = DateTime.UtcNow;
            //tr.TypeTransaction = "depot unite";
            _dbContext.Add(tr);
            int i = _dbContext.SaveChanges();
        }


        public IActionResult _PromotionView()
        {
            
            ViewBag.liste = lstPromo;
            return PartialView();
        }

    }
}
