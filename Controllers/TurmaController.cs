using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Escola.Context;
using Escola.Models;
using System.Text;

namespace Escola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurmaController : ControllerBase
    {
        private readonly EscolaContext _context;

        public TurmaController(EscolaContext context)
        {
            _context = context;
        }

        // GET: api/Turma
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Turma>>> GetTurma()
        {
            if (_context.Turma == null || _context.Turma.Count() == 0)
                return NotFound("Não há nenhuma turma cadastrada.");

            var turmas = await _context.Turma.ToListAsync();
            List<Turma> turmasAtivas = turmas.Where(x => x.Ativo == true).ToList();
            StringBuilder sb = new StringBuilder();

            foreach (var turma in turmasAtivas)
            {
                    sb.AppendLine(ShowTurma(turma));
            }

            if (sb.Length == 0)
                sb.AppendLine("Não há turmas ativas.");

            return Ok("Relação de turmas ativas:\n\n" + sb.ToString());
        }

        // GET: api/Turma/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Turma>> GetTurma(int id)
        {
            if (_context.Turma == null)
                return NotFound();

            var turma = await _context.Turma.FindAsync(id);

            if (turma == null)
                return NotFound($"Não há nenhuma turma cadastrada com o id {id}.");

            return Ok(ShowTurma(turma));
        }

        // PUT: api/Turma/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTurma(int id, Turma turma)
        {
            if (id != turma.Id)
                return BadRequest("Favor informar apenas 1 id.");

            _context.Entry(turma).State = EntityState.Modified;

            try
            {
                if (turma.Nome == string.Empty)
                    return BadRequest("É obrigatório o preenchimento do campo Nome.");

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TurmaExists(id))
                    return NotFound($"Não há nenhuma turma cadastrada com o id {id}.");
                throw;
            }

            return Ok($"Alteração realizada com sucesso na turma {turma.Nome} (id {turma.Id}).");
        }

        // POST: api/Turma
        [HttpPost]
        public async Task<ActionResult<Turma>> PostContato(Turma turma)
        {

            if (_context.Turma == null)
                return BadRequest("Nenhuma turma foi informada!");

            if (string.IsNullOrEmpty(turma.Nome))
                return BadRequest("É obrigatório o preenchimento do campo Nome.");

            if (!TurmaExists(turma.Nome))
            {
                _context.Turma.Add(turma);
                await _context.SaveChangesAsync();
            }
            else
                return BadRequest($"Erro. A turma {turma.Nome} já está cadastrada.");

            return CreatedAtAction("GetTurma", new { id = turma.Id }, "Turma criada com sucesso\n\n" + ShowTurma(turma));
        }

        // DELETE: api/Turma/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTurma(int id)
        {
            if (_context.Turma == null)
                return NotFound();

            var turma = await _context.Turma.FindAsync(id);
            if (turma == null)
                return NotFound($"Não há nenhuma turma cadastrada com o id {id}.");

            if (TurmaContainsAlunos(id))
                return BadRequest("Não é possível excluir uma turma que possui alunos inscritos.");

            _context.Turma.Remove(turma);
            await _context.SaveChangesAsync();

            return Ok($"Turma {turma.Nome} (id {turma.Id}) excluída com sucesso.");
        }

        private bool TurmaExists(int id)
        {
            return (_context.Turma?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool TurmaExists(string nome)
        {
            return (_context.Turma?.Any(e => e.Nome == nome)).GetValueOrDefault();
        }

        private bool TurmaContainsAlunos(int id)
        {
            return (_context.Aluno?.Any(e => e.TurmaId == id)).GetValueOrDefault();
        }

        private string ShowTurma(Turma turma)
        {
            string status = (turma.Ativo == true) ? "ativa" : "inativa";
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Turma: {turma.Nome}");
            sb.AppendLine($"Id: {turma.Id}");
            sb.AppendLine($"Status: {status}");
            sb.AppendLine("Alunos matriculados:");

            if (turma.Alunos != null)
            {
                List<Aluno> alunos = new List<Aluno>(turma.Alunos);

                if (alunos.Count != 0)
                {
                    foreach (var aluno in alunos)
                    {
                        sb.AppendLine($"\tId: {aluno.Id}");
                        sb.AppendLine($"\tNome: {aluno.Nome}");
                        sb.AppendLine($"\tData de Nascimento: {aluno.DataNascimento.ToString("d")}");
                        sb.AppendLine($"\tSexo: {aluno.Sexo}");
                        sb.AppendLine($"\tTotal de faltas: {aluno.TotalFaltas}\n");
                    }
                }
                else
                    sb.AppendLine("\tNão há nenhum aluno matriculado nesta turma.");
            }
            else
                sb.AppendLine("\tNão há nenhum aluno matriculado nesta turma.");

            return sb.ToString();
        }
    }
}
