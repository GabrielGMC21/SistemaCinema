using System;
using System.Collections.Generic;
using Sistema_Cinema.Models;

namespace Sistema_Cinema.Models
{
    public class FilmeDetalhesViewModel
    {
        public Filme Filme { get; set; }
        public List<Avaliacao> Avaliacoes { get; set; } = new();
        public decimal MediaAvaliacao { get; set; }
        public int TotalAvaliacoes { get; set; }
        public bool PodeAvaliar { get; set; }
        public Avaliacao? MinhaAvaliacao { get; set; }
    }
}
