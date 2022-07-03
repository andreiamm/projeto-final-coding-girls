namespace Escola.Models
{
    public class Turma
    {
        #region Properties
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool? Ativo { get; set; }
        #endregion

        #region Navigation Properties
        public virtual List<Aluno>? Alunos { get; set; }
        #endregion
    }
}
