using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GerenciamentoEvento.Data;
using GerenciamentoEvento.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiRest.Controllers {
    [Route ("api/v1/eventos")]
    [ApiController]
    public class ApiEventosController : ControllerBase {

        private readonly ApplicationDbContext database;

        public ApiEventosController (ApplicationDbContext database) {

            this.database = database;
        }

        [HttpGet]

        public IActionResult Get () {
            var eventos = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).Where (p => p.Status == true).Where (p => p.CasaDeShow.Status == true).ToList ();
            return Ok (eventos); //Status code = 200 && Dados 
        }

        [HttpGet ("capacidade/asc")]
        public IActionResult GetAsc () {
            var local = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).Where (p => p.Status == true).ToList ().OrderBy (p => p.Capacidade);
            return Ok (local);
            //Status code = 200 && Dados 

        }

        [HttpGet ("capacidade/desc")]
        public IActionResult GetDesc () {
            var local = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).Where (p => p.Status == true).ToList ().OrderByDescending (p => p.Capacidade);
            return Ok (local);
            //Status code = 200 && Dados 

        }

        [HttpGet ("data/asc")]
        public IActionResult GetDataAsc () {
            var local = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).Where (p => p.Status == true).ToList ().OrderBy (p => p.Data);
            return Ok (local);
            //Status code = 200 && Dados 

        }

        [HttpGet ("data/desc")]
        public IActionResult GetDataDesc () {
            var local = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).Where (p => p.Status == true).ToList ().OrderByDescending (p => p.Data);
            return Ok (local);
            //Status code = 200 && Dados 
        }

        [HttpGet ("preco/asc")]
        public IActionResult GetPrecoAsc () {
            var local = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).Where (p => p.Status == true).ToList ().OrderBy (p => p.Preco);
            return Ok (local);
            //Status code = 200 && Dados 

        }

        [HttpGet ("preco/desc")]
        public IActionResult GetPrecoDesc () {
            var local = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).Where (p => p.Status == true).ToList ().OrderByDescending (p => p.Preco);
            return Ok (local);
            //Status code = 200 && Dados 
        }

        [HttpGet ("nome/asc")]
        public IActionResult GetNomeAsc () {
            var local = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).Where (p => p.Status == true).ToList ().OrderBy (p => p.Nome);
            return Ok (local);
        }

        [HttpGet ("nome/desc")]
        public IActionResult GetNomeDesc () {
            var local = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).Where (p => p.Status == true).ToList ().OrderByDescending (p => p.Nome);
            return Ok (local);
        }

        [HttpGet ("{id}")]
        public IActionResult Get (int id) {
            try {
                Evento evento = database.Evento.Include (e => e.CasaDeShow).Where (p => p.CasaDeShow.Status == true).Where (e => e.Status == true).First (p => p.Id == id);
                return Ok (evento);
            } catch (Exception) {

                Response.StatusCode = 404;
                return new ObjectResult (new { msg = "ID Inválido" });
            }

        }

        [HttpGet ("vendas")]
        public IActionResult GetUser () {
            var local = database.Venda.Include (e => e.CasaDeShow).ToList ().OrderByDescending (p => p.Nome);
            return Ok (local);
        }

        [HttpGet ("vendas/" + "{id}")]
        public IActionResult GetUser (int id) {
            try {
                Venda venda = database.Venda.Include (e => e.CasaDeShow).First (p => p.Id == id);
                return Ok (venda);
            } catch (Exception) {

                Response.StatusCode = 404;
                return new ObjectResult (new { msg = "ID Inválido" });
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post ([FromBody] EventoTemp eTemp) {
            //Validação
            if (eTemp != null) {
                try {
                    if (eTemp.CasaDeShowID == 0) {
                        return new ObjectResult (new { msg = "O Local do Evento não existe!" });
                    }
                    Evento e = new Evento ();
                    e.Nome = eTemp.Nome;
                    e.Capacidade = eTemp.Capacidade;
                    e.Data = eTemp.Data;
                    e.Preco = eTemp.Preco;
                    e.CasaDeShow = database.Local.First (c => c.Id == eTemp.CasaDeShowID);
                    e.Estilo = eTemp.Estilo.ToString ();
                    e.Imagem = eTemp.Imagem;
                    e.Status = eTemp.Status;
                    database.Evento.Add (e);
                    database.SaveChanges ();

                    //Set do Status Code
                    Response.StatusCode = 201;
                    return new ObjectResult (new { msg = "Evento criado com sucesso!" });
                } catch (Exception) {
                    Response.StatusCode = 400;
                    return BadRequest (new { msg = "Verifique se o ID do evento está correto" });
                }
            }
            return BadRequest (new { msg = "Verifique se as informações do evento estão corretas" });
        }

        [HttpPatch]
        [Authorize]
        public IActionResult Patch ([FromBody] EventoPatch ptemp) {

            if (ptemp.Id >= 0) {

                try {
                    var evento = database.Evento.First (p => p.Id == ptemp.Id);
                    //Editar 
                    if (ptemp.Nome != null) {
                        if (ptemp.Nome.Length > 2 && ptemp.Nome.Length < 150) {
                            evento.Nome = ptemp.Nome;
                        } else {
                            evento.Nome = evento.Nome;
                            Response.StatusCode = 404;
                            return new ObjectResult (new { msg = "Nome Inválido" });
                        }
                    }
                    if (ptemp.Capacidade != 0) {
                        if (ptemp.Capacidade > 10 && ptemp.Capacidade < 1000000) {
                            evento.Capacidade = ptemp.Capacidade;
                        } else {
                            evento.Capacidade = evento.Capacidade;
                            Response.StatusCode = 404;
                            return new ObjectResult (new { msg = "Capaidade Inválida" });
                        }
                    }
                    if (ptemp.Preco != 0) {
                        if (ptemp.Preco > 10 || ptemp.Preco < 1000000) {
                            evento.Preco = ptemp.Preco;
                        } else {
                            evento.Preco = evento.Preco;
                            Response.StatusCode = 404;
                            return new ObjectResult (new { msg = "Preço Inválida" });
                        }
                    }

                    if (ptemp.CasaDeShowID != 0) {
                        if (database.Local.First (p => p.Id == ptemp.CasaDeShowID) != null) {
                            evento.CasaDeShow = database.Local.First (p => p.Id == ptemp.CasaDeShowID);
                        } else {
                            Response.StatusCode = 404;
                            return new ObjectResult (new { msg = "Local de inválido" });
                        }
                    }
                    if (ptemp.Estilo != 0) {
                        if (evento.Estilo == ptemp.Estilo.ToString () && ptemp.Estilo > 0 || ptemp.Estilo < 8) {
                            evento.Estilo = ptemp.Estilo.ToString ();
                        } else {
                            evento.Estilo = evento.Estilo;
                            Response.StatusCode = 404;
                            return new ObjectResult (new { msg = "Estilo de Inválido" });
                        }
                    }

                    if (ptemp.Imagem != null) {
                        if (ptemp.Imagem.Length > 10 && ptemp.Imagem.Length < 1024) {
                            evento.Imagem = ptemp.Imagem;
                        } else {
                            evento.Imagem = evento.Imagem;
                            Response.StatusCode = 404;
                            return new ObjectResult (new { msg = "Imagem: Url inválida" });
                        }
                    }

                    database.SaveChanges ();
                    return Ok (new { msg = "Evento Atualizado!" });

                } catch (System.Exception) {
                    Response.StatusCode = 400;
                    return new ObjectResult (new { msg = "Requisição Inválida" });
                }

            } else {
                Response.StatusCode = 404;
                return new ObjectResult (new { msg = "ID Inválido" });
            }

        }

        [HttpDelete ("{id}")]
        [Authorize]
        public IActionResult Delete (int id) {
            try {
                var evento = database.Evento.First (p => p.Id == id);
                evento.Status = false;
                database.SaveChanges ();
                Response.StatusCode = 201;
                return new ObjectResult (new { msg = "Evento excluido!" });
            } catch (Exception) {
                Response.StatusCode = 404;
                return new ObjectResult (new { msg = "ID inválido" });
            }
        }

        public class EventoTemp {
            public int Id { get; set; }

            [Required (ErrorMessage = "O Nome é obrigatório.")]
            [StringLength (150, ErrorMessage = "Nome grande demais.")]
            [MinLength (2, ErrorMessage = "Nome curto demais.")]
            public string Nome { get; set; }

            [Range (10, 1000000, ErrorMessage = "Capacidade Inválida.")]
            public int Capacidade { get; set; }

            [Required (ErrorMessage = "A data é obrigatória.")]
            public System.DateTime Data { get; set; }

            [Range (1, 1000000, ErrorMessage = "Preço inválido.")]
            public double Preco { get; set; }

            [Required]
            public int CasaDeShowID { get; set; }

            [Range (0, 8, ErrorMessage = "Estilo inválido.")]
            public int Estilo { get; set; }

            [Required (ErrorMessage = "A URL da imagem é obrigatória.")]
            [MinLength (2, ErrorMessage = "Url curta demais.")]
            [StringLength (1024, ErrorMessage = "Url grande demais.")]
            public string Imagem { get; set; }

            public bool Status { get; set; }
        }
        public class EventoPatch {
            public int Id { get; set; }
            public string Nome { get; set; }
            public int Capacidade { get; set; }

            public System.DateTime Data { get; set; }

            public double Preco { get; set; }

            public int CasaDeShowID { get; set; }

            public int Estilo { get; set; }

            public string Imagem { get; set; }

            public bool Status { get; set; }
        }
    }
}