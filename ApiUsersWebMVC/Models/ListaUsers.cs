using ApiUsersWebMVC.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiUsersWebMVC.Models
{
    public class ListaUsers
    {
       public List<UsuariosActivos> usuariosActivos { get; set; }
    }
}