namespace WebAPITest.Classi
{
    public class Dipendente
    {
        public Int32 DipendenteID { get; set; }

        public string? Cognome { get; set; }

        public string? Nome { get; set; }

        public DateTime DataNascita { get; set; }

        public string? Sesso { get; set; }

        public string? ComuneNascita { get; set;}

        public string? ProvinciaNascita { get; set; }

        public string? Email {  get; set; }

        
        public Dipendente() 
        {
        
        }
    } 
}
