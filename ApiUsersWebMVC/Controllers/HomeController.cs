using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ApiUsersWebMVC.Controllers;
using ApiUsersWebMVC.Models;
using Newtonsoft.Json;
using PagedList;
using PureCloudPlatform.Client.V2.Api;

using System.Text;
using System.Threading.Tasks;

using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Extensions;
using PureCloudPlatform.Client.V2.Model;


namespace ApiUsersWebMVC.Controllers
{
    public class HomeController : Controller
    {
        class Respuesta
        {
            public string status { get; set; }
            public string url { get; set; }
        }
        
        public ActionResult Index(int? page, string filtrar, string nombre)
        {
            
            if (filtrar==null||filtrar== "----------")
            {
                filtrar = "todos";
                ViewData["filtro"] = filtrar;
            }

            if(nombre == null )
            {
                nombre = "";
            }

            ViewData["filtro"] = filtrar;
            int pagesize = 6;
            int pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<UsuariosActivos> user = null;
            ApiUser apiUsers = new ApiUser();
            ListaUsers lista = new ListaUsers();
            lista.usuariosActivos = apiUsers.filtrarUsuario(filtrar, nombre);
            user = lista.usuariosActivos.ToPagedList(pageindex,pagesize);
            return View(user);
        }

        public ActionResult Usuarios()
        {
            return View();
        }
        
        public void Registrar(string nombre, string apellido, string telefono, string email)
        {
            Session["nom"] = nombre;
            Session["ape"] = apellido;
            Session["tel"] = telefono;
            Session["cor"] = email;
        }
        public JsonResult InicarVideoLlamada(string nombre)
        {
            string nom = (string)Session["nom"];
            string ape = (string)Session["ape"];
            string tel = (string)Session["tel"];
            string cor = (string)Session["cor"];

            UsersApi usersApi = new UsersApi();
            //UserEntityListing userList = null;
            List<String> expand = new List<String>();
            expand.Add("presence");
            expand.Add("routingStatus");


            var usuarioActivo = usersApi.GetUser(nombre, expand, "active");
            if ((usuarioActivo.Presence.PresenceDefinition.SystemPresence == "On Queue")&& (usuarioActivo.RoutingStatus.Status.ToString() == "Idle"))
            {
                string responseFromServer;
                WebRequest request2 = WebRequest.Create("https://video3.apifycloud.com/api/session_request.php?name=" + nom + "&duration=60&key=ad967e8e-73f4-11ea-9ba4-d3a8a6bb4ea4&type=user&lastname=" + ape + "&phone=" + tel + "&email=" + cor + "&session_type=video");
                // If required by the server, set the credentials.  
                request2.Credentials = CredentialCache.DefaultCredentials;
                // Get the response.  
                WebResponse response = request2.GetResponse();
                // Display the status.  
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server. 
                // The using block ensures the stream is automatically closed. 
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.  
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.  
                    responseFromServer = reader.ReadToEnd();
                    // Display the content.  

                    response.Close();
                }

                var res = JsonConvert.DeserializeObject<Respuesta>(responseFromServer);

                return Json(res.url, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json( new { success = false, message = "El Agente se encuentra ocupado, por favor intente más tarde" },  JsonRequestBehavior.AllowGet);
            }

        }

    }
}