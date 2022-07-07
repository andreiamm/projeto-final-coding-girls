using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Escola.Context;
using Escola.Models;
using System.Text;

namespace Escola.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlunoController : ControllerBase
    {
        private readonly EscolaContext _context;

        public AlunoController(EscolaContext context)
        {
            _context = context;
        }

        // GET: api/Aluno
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Aluno>>> GetAluno()
        {
            if (_context.Aluno == null || _context.Aluno.Count() == 0)
                return NotFound("Não há nenhum aluno cadastrado");

            var alunos = await _context.Aluno.ToListAsync();

            List<Aluno> alunosAtivos = alunos.Where(x => x.Turma.Ativo == true).ToList();
            StringBuilder sb = new StringBuilder();

            foreach (Aluno aluno in alunosAtivos)
            {
                sb.Append(ShowAluno(aluno));
                sb.AppendLine($"Turma: {aluno.Turma.Nome}\n");
            }

            if (sb.Length == 0)
                sb.Append("Não há turmas ativas.");

            return Ok("Relação de alunos cuja turma está ativa:\n\n" + sb.ToString());
        }

        // GET: api/Aluno/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Aluno>> GetAluno(int id)
        {
            if (_context.Aluno == null)
                return NotFound();

            var aluno = await _context.Aluno.FindAsync(id);

            if (aluno == null)
                return NotFound($"Não há nenhum aluno cadastrado com o id {id}.");

            List<Turma> turmas = await _context.Turma.ToListAsync();

            return Ok(ShowAluno(aluno) + CheckTurma(aluno.TurmaId, turmas));
        }

        // PUT: api/Aluno/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAluno(int id, Aluno aluno)
        {
            if (id != aluno.Id)
                return BadRequest("Favor informar apenas um id.");

            _context.Entry(aluno).State = EntityState.Modified;

            try
            {
                string checkDataResult = CheckData(aluno);
                if (checkDataResult != null)
                    return BadRequest(checkDataResult);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlunoExists(id))
                    return NotFound("Não há nenhum aluno cadastrado com o id informado.");
                else
                    throw;
            }

            return Ok($"O cadastro de {aluno.Nome} foi alterado com sucesso.");
        }

        // POST: api/Aluno
        [HttpPost]
        public async Task<ActionResult<Aluno>> PostAluno(Aluno aluno)
        {

            if (_context.Aluno == null)
                return BadRequest("Não foi informado nenhum aluno.");

            string checkDataResult = CheckData(aluno);
            if (checkDataResult != null)
                return BadRequest(checkDataResult);

            _context.Aluno.Add(aluno);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAluno", new { id = aluno.Id }, "Cadastro criado com sucesso.\n\n" + ShowAluno(aluno));
        }

        // DELETE: api/Aluno/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAluno(int id)
        {
            if (_context.Aluno == null)
                return NotFound();

            var aluno = await _context.Aluno.FindAsync(id);
            if (aluno == null)
                return NotFound($"Não há nenhum aluno cadastrado com o id {id}.");

            _context.Aluno.Remove(aluno);
            await _context.SaveChangesAsync();

            return Ok($"Aluno(a) {aluno.Nome} excluído(a) com sucesso.");
        }

        private bool AlunoExists(int id)
        {
            return (_context.Aluno?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool TurmaExists(int id)
        {
            return (_context.Turma?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string CheckData(Aluno aluno)
        {
            if (string.IsNullOrEmpty(aluno.Nome))
                return "É obrigatório o preenchimento do campo 'Nome'.";

            if (aluno.TurmaId < 1)
                return "É obrigatório o preenchimento do campo 'Id da turma' com um valor positivo.";

            if (!TurmaExists(aluno.TurmaId))
                return $"Não existe nenhuma turma cadastrada com o id {aluno.TurmaId}.";

            if (aluno.Sexo == 0)
                return "É obrigatório o preenchimento do campo 'Sexo'.";

            if (aluno.DataNascimento.Ticks == 0)
                return "É obrigatório o preenchimento do campo 'Data de nascimento'.";

            return null;
        }

        private string ShowAluno(Aluno aluno)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Id do aluno: {aluno.Id}");
            sb.AppendLine($"Nome: {aluno.Nome}");
            sb.AppendLine($"Data de Nascimento: {aluno.DataNascimento.ToString("d")}");
            sb.AppendLine($"Sexo: {aluno.Sexo}");
            sb.AppendLine($"Total de faltas: {aluno.TotalFaltas}");
            sb.AppendLine($"Id da turma: {aluno.TurmaId}");
            return sb.ToString();
        }

        private string CheckTurma(int id, List<Turma> turmas)
        {
            Turma turma = (Turma)turmas.Where(x => x.Id == id);
            string status = (turma.Ativo == true) ? "ativa" : "inativa";
            return "Turma: " + turma.Nome + " (Status: " + status  + ")";
        }
    }
}
