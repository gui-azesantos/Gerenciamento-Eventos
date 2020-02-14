using System;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoEvento.Data;
using GerenciamentoEvento.DTO;
using GerenciamentoEvento.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoEvento.Controllers {

    public class EventosController : Controller {
        private readonly ApplicationDbContext database;

        private readonly UserManager<IdentityUser> _userManager;

        public EventosController (ApplicationDbContext database, UserManager<IdentityUser> userManager) {
            this.database = database;
            _userManager = userManager;
        }

        public IActionResult Home () {
            var eventos = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).ToList ();
            return View (eventos);
        }

        [Authorize]

        public async Task<IActionResult> Index () {

            var eventos = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).ToList ();
            var user = await _userManager.GetUserAsync (User);
            //Condição para validar Admin
            if (user.Email == "admin@gmail.com") {

                return View (eventos);
            } else {
                return Unauthorized ();
            }
        }

        [Authorize]
        public async Task<IActionResult> Criar () {
            ViewBag.CasaDeShow = database.Local.Where (p => p.Status == true).ToList ();
            var user = await _userManager.GetUserAsync (User);

            //Condição para validar a existência de Casas de show
            var local = database.Local.FirstOrDefault (p => p.Status);
            if (local == null) {
                return View ("Error");
            }
            //Condição para validar Admin
            if (user.Email == "admin@gmail.com") {
                return View ();
            } else {
                return Unauthorized ();
            }
        }

        [Authorize]
        public async Task<IActionResult> Editar (int id) {
            var user = await _userManager.GetUserAsync (User);
            //Condição para validar Admin
            if (user.Email == "admin@gmail.com") {
                if (ModelState.IsValid) {
                    var evento = database.Evento.Include (e => e.CasaDeShow).First (e => e.Id == id);
                    EventoDTO eventoview = new EventoDTO ();
                    eventoview.Id = evento.Id;
                    eventoview.Nome = evento.Nome;
                    eventoview.Capacidade = evento.Capacidade;
                    eventoview.Data = evento.Data;
                    eventoview.Preco = evento.Preco;
                    eventoview.CasaDeShowID = evento.CasaDeShow.Id;
                    eventoview.Estilo = evento.Estilo;
                    eventoview.Imagem = evento.Imagem;
                    ViewBag.CasaDeShow = database.Local.Where (p => p.Status == true).ToList ();
                    return View (eventoview);
                } else {
                    ViewBag.CasaDeShow = database.Local.ToList ();
                    return View ("Editar");
                }
            } else {
                return Unauthorized ();
            }
        }

        [HttpPost]
        public IActionResult Atualizar (EventoDTO _evento) {

            if (ModelState.IsValid) {
                var evento = database.Evento.First (e => e.Id == _evento.Id);
                evento.Nome = _evento.Nome;
                evento.Capacidade = _evento.Capacidade;
                evento.Data = _evento.Data;
                evento.Preco = _evento.Preco;
                evento.CasaDeShow = database.Local.First (casadeshow => casadeshow.Id == _evento.CasaDeShowID);
                evento.Estilo = _evento.Estilo;
                evento.Imagem = _evento.Imagem;
                evento.Status = true;
                database.SaveChanges ();
                return RedirectToAction ("Index", "Eventos");
            } else {
                ViewBag.CasaDeShow = database.Local.Where (p => p.Status == true).ToList ();
                return View ("Editar");
            }

        }

        [HttpPost]
        public IActionResult Salvar (EventoDTO _evento) {

            if (ModelState.IsValid) {

                Evento evento = new Evento ();
                evento.Nome = _evento.Nome;
                evento.Capacidade = _evento.Capacidade;
                evento.Data = _evento.Data;
                evento.Preco = _evento.Preco;
                evento.CasaDeShow = database.Local.First (casadeshow => casadeshow.Id == _evento.CasaDeShowID);
                evento.Estilo = _evento.Estilo;
                evento.Imagem = _evento.Imagem;
                evento.Status = true;
                database.Evento.Add (evento);
                database.SaveChanges ();
                ViewBag.CasaDeShow = database.Local.ToList ();
                return RedirectToAction ("Index", "Eventos");
            } else {
                ViewBag.CasaDeShow = database.Local.Where (p => p.Status == true).ToList ();
                return View ("Criar");
            }

        }

        public async Task<IActionResult> Deletar (int id) {
            var user = await _userManager.GetUserAsync (User);
            //Condição para validar Admin
            if (user.Email == "admin@gmail.com") {
                Evento evento = database.Evento.First (registro => registro.Id == id);
                evento.Status = false;
                database.SaveChanges ();
                return RedirectToAction ("Index");
            } else {
                return Unauthorized ();
            }
        }

    }
}