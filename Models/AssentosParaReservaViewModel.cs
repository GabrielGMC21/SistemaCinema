using System.Collections.Generic;

namespace Sistema_Cinema.Models
{
    public class AssentoViewModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public bool Reservado { get; set; }
    }

    public class AssentosParaReservaViewModel
    {
        public Sessao Sessao { get; set; }
        public List<AssentoViewModel> Assentos { get; set; } = new List<AssentoViewModel>();
    }
}
