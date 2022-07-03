namespace Escola.Models
{
    public class Aluno
    {
        #region Properties
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public char Sexo { get; set; }
        public int? TotalFaltas { get; set; }
        public int TurmaId { get; set; }
        #endregion

        #region Navigation Properties
        public virtual Turma? Turma { get; set; }
        #endregion
    }
}
