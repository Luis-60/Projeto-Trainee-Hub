using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
namespace Projeto_Trainee_Hub.Helper
{
    public class Sessao : ISessao
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public Sessao(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Usuarios BuscarSessaoUsuario()
        {
        
            string sessaoUsuario = _httpContextAccessor.HttpContext.Session.GetString("UsuarioLogado");
            
            if (string.IsNullOrEmpty(sessaoUsuario)) return null;
            var usuario = JsonConvert.DeserializeObject<Usuarios>(sessaoUsuario);
            return usuario; 
        }
        public string BuscarSessaoUsuarioRole()
        {
            string sessaoUsuario = _httpContextAccessor.HttpContext.Session.GetString("UsuarioLogado");
            
            if (string.IsNullOrEmpty(sessaoUsuario)) return null;
            var usuario = JsonConvert.DeserializeObject<Usuarios>(sessaoUsuario);
            return usuario.IdTipo.ToString(); 
        }

        public void CriarSessaoUsuario(Usuarios usuario)
        {
            string valor = JsonConvert.SerializeObject(usuario);
            _httpContextAccessor.HttpContext.Session.SetString("UsuarioLogado", valor);       
        }
        public void RemoverSessaoUsuario()
        {
            _httpContextAccessor.HttpContext.Session.Remove("UsuarioLogado");
        }
    }
}