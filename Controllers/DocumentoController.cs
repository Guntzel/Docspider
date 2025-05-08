using Docspider.Data;
using Docspider.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace Docspider.Controllers
{
    public class DocumentoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DocumentoController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Página inicial que lista os documentos
        public IActionResult Index()
        {
            var documentos = _context.Documentos.ToList(); // Busca todos os documentos do banco
            return View(documentos); // Passa os documentos para a view
        }

        // Método GET para exibir o formulário de edição
        [HttpGet]
public IActionResult ExibirFormularioEdicao(int id)
{
    var documento = _context.Documentos.FirstOrDefault(d => d.Id == id);
    if (documento == null)
    {
        return NotFound();
    }
    return View("Editar", documento); // Exibe a view "Editar"
}

        // Método POST para salvar as alterações do formulário de edição
        [HttpPost]
public IActionResult SalvarEdicao(Documento documento)
{
    if (!ModelState.IsValid)
    {
        return View("Editar", documento); // Retorna a view "Editar" em caso de erro
    }

    var documentoExistente = _context.Documentos.FirstOrDefault(d => d.Id == documento.Id);
    if (documentoExistente == null)
    {
        return NotFound();
    }

    documentoExistente.Titulo = documento.Titulo;
    documentoExistente.Descricao = documento.Descricao;

    _context.SaveChanges();

    return RedirectToAction("Index");
}


        // Método POST para excluir um documento
        [HttpPost]
        public IActionResult Excluir(int id)
        {
            var documento = _context.Documentos.FirstOrDefault(d => d.Id == id);
            if (documento == null)
            {
                return NotFound();
            }

            // Exclui o arquivo do caminho físico
            var caminhoArquivo = Path.Combine(_webHostEnvironment.WebRootPath, documento.CaminhoArquivo.TrimStart('/'));
            if (System.IO.File.Exists(caminhoArquivo))
            {
                System.IO.File.Delete(caminhoArquivo);
            }

            // Remove o documento do banco de dados
            _context.Documentos.Remove(documento);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Método GET para exibir o formulário de adição
        [HttpGet]
        public IActionResult Adicionar()
        {
            return View();
        }

        // Método POST para adicionar um novo documento
        [HttpPost]
        public IActionResult Adicionar(Documento documento, IFormFile Arquivo)
        {
            Console.WriteLine("Método Adicionar chamado.");

            // Remove a validação do campo NomeArquivo
            ModelState.Remove("NomeArquivo");

            // Verifica se o título já existe no banco de dados
            if (_context.Documentos.Any(d => d.Titulo == documento.Titulo))
            {
                ModelState.AddModelError("Titulo", "Já existe um documento com este título.");
            }

            // Verifica se o arquivo possui uma extensão proibida
            if (Arquivo != null)
            {
                var extensao = Path.GetExtension(Arquivo.FileName).ToLower();
                var extensoesProibidas = new[] { ".exe", ".zip", ".bat" };

                if (extensoesProibidas.Contains(extensao))
                {
                    ModelState.AddModelError("Arquivo", "O tipo de arquivo enviado não é permitido.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(documento);
            }

            if (Arquivo != null && Arquivo.Length > 0)
            {
                // Define o caminho para salvar o arquivo
                var caminhoPasta = Path.Combine(_webHostEnvironment.WebRootPath, "documentos");
                if (!Directory.Exists(caminhoPasta))
                {
                    Directory.CreateDirectory(caminhoPasta);
                }

                // Define o nome do arquivo com a extensão original
                var nomeArquivo = Path.GetFileName(Arquivo.FileName);
                var caminhoArquivo = Path.Combine(caminhoPasta, nomeArquivo);

                // Salva o arquivo no diretório
                using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                {
                    Arquivo.CopyTo(stream);
                }

                // Atualiza os dados do documento
                documento.NomeArquivo = nomeArquivo;
                documento.CaminhoArquivo = $"/documentos/{nomeArquivo}";
            }

            _context.Documentos.Add(documento);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}